﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HoaryFox.Component.Utils.Geometry;

using HoaryFoxCommon.Properties;

using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Geometry
{
    public class Stb2Brep : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private readonly GH_Structure<GH_Brep>[] _brepList = new GH_Structure<GH_Brep>[9];

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Stb2Brep()
          : base("Stb to Brep", "S2B",
              "Display ST-Bridge model in Brep",
              "HoaryFox", "Geometry")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bake", "Bake", "If it true, bake geometry.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Log", "Log", "Log", GH_ParamAccess.item);
            pManager.AddBrepParameter("Columns", "Col", "output StbColumns to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Girders", "Gird", "output StbGirders to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Posts", "Pst", "output StbPosts to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Beams", "Bm", "output StbBeams to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Braces", "Brc", "output StbBraces to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Slabs", "Slb", "output StbSlabs to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Walls", "Wl", "output StbWalls to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Piles", "Pil", "output StbPiles to Brep", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Footings", "Ftg", "output StbFootings to Brep", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var isBake = false;
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }
            if (!dataAccess.GetData("Bake", ref isBake)) { return; }

            var log = CreateBrep();
            if (isBake)
            {
                BakeBrep();
            }

            dataAccess.SetData(0, log);
            for (var i = 1; i < 10; i++)
            {
                dataAccess.SetDataTree(i, _brepList[i - 1]);
            }
        }

        protected override Bitmap Icon => Resource.Brep;
        public override Guid ComponentGuid => new Guid("B2D5EA7F-E75F-406B-8D22-C267B43C5E72");

        private string CreateBrep()
        {
            var path = Path.GetDirectoryName(Grasshopper.Instances.ComponentServer.FindAssemblyByObject(this).Location);
            StbMembers member = _stBridge.StbModel.StbMembers;
            var brepFromStb = new CreateMemberBrepListFromStb(_stBridge.StbModel.StbSections, _stBridge.StbModel.StbNodes, new[] { DocumentTolerance(), DocumentAngleTolerance() }, path);
            _brepList[0] = brepFromStb.Column(member.StbColumns);
            _brepList[1] = brepFromStb.Girder(member.StbGirders);
            _brepList[2] = brepFromStb.Post(member.StbPosts);
            _brepList[3] = brepFromStb.Beam(member.StbBeams);
            _brepList[4] = brepFromStb.Brace(member.StbBraces);
            _brepList[5] = brepFromStb.Slab(member.StbSlabs);
            _brepList[6] = brepFromStb.Wall(member.StbWalls, member.StbOpens);
            _brepList[7] = brepFromStb.Pile(member.StbPiles);
            _brepList[8] = brepFromStb.Footing(member.StbFootings);
            brepFromStb.SerializeLog();
            return brepFromStb.Logger.ToString();
        }

        private void BakeBrep()
        {
            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            var parentLayerNames = new[] { "Column", "Girder", "Post", "Beam", "Brace", "Slab", "Wall", "Pile", "Footing" };
            Color[] layerColors = new[] { Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple, Color.DarkGray, Color.CornflowerBlue, Color.DarkOrange, Color.DarkKhaki };
            GeometryBaker.MakeParentLayers(activeDoc, parentLayerNames, layerColors);

            Dictionary<string, string>[][] infoArray = Utils.TagUtils.GetAllSectionInfoArray(_stBridge.StbModel.StbMembers, _stBridge.StbModel.StbSections);

            foreach ((GH_Structure<GH_Brep> breps, int i) in _brepList.Select((frameBrep, index) => (frameBrep, index)))
            {
                Layer parentLayer = activeDoc.Layers.FindName(parentLayerNames[i]);
                Guid parentId = parentLayer.Id;
                foreach ((Brep brep, int bIndex) in breps.Select((brep, bIndex) => (brep.Value, bIndex)))
                {
                    var objAttr = new ObjectAttributes();

                    Dictionary<string, string>[] infos = infoArray[i];
                    Dictionary<string, string> info = infos[bIndex];

                    foreach (KeyValuePair<string, string> pair in info)
                    {
                        objAttr.SetUserString(pair.Key, pair.Value);
                    }

                    var layer = new Layer { Name = info["name"], ParentLayerId = parentId, Color = layerColors[i] };
                    int layerIndex = activeDoc.Layers.Add(layer);
                    if (layerIndex == -1)
                    {
                        layer = activeDoc.Layers.FindName(info["name"]);
                        layerIndex = layer.Index;
                    }
                    objAttr.LayerIndex = layerIndex;

                    activeDoc.Objects.AddBrep(brep, objAttr);
                }
            }
        }
    }
}
