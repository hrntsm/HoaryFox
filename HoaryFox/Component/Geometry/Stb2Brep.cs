using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using HoaryFox.Component.Utils.Geometry;
using HoaryFox.Properties;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Geometry
{
    public class Stb2Brep : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private readonly List<List<Brep>> _brepList = new List<List<Brep>>();

        public Stb2Brep()
          : base("Stb to Brep", "S2B",
              "Display ST-Bridge model in Brep",
              "HoaryFox", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _brepList.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bake", "Bake", "If it true, bake geometry.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Columns", "Col", "output StbColumns to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Girders", "Gird", "output StbGirders to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Posts", "Pst", "output StbPosts to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Beams", "Bm", "output StbBeams to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Braces", "Brc", "output StbBraces to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Slabs", "Slb", "output StbSlabs to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Walls", "Wl", "output StbWalls to Brep", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var isBake = false;
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }
            if (!dataAccess.GetData("Bake", ref isBake)) { return; }

            CreateBrep();
            if (isBake)
            {
                BakeBrep();
            }

            for (var i = 0; i < 7; i++)
            {
                dataAccess.SetDataList(i, _brepList[i]);
            }
        }

        protected override Bitmap Icon => Resource.Brep;
        public override Guid ComponentGuid => new Guid("B2D5EA7F-E75F-406B-8D22-C267B43C5E72");

        private void CreateBrep()
        {
            StbMembers member = _stBridge.StbModel.StbMembers;
            var brepFromStb = new CreateMemberBrepListFromStb(_stBridge.StbModel.StbSections, _stBridge.StbModel.StbNodes, new[] { DocumentTolerance(), DocumentAngleTolerance() });
            _brepList.Add(brepFromStb.Column(member.StbColumns));
            _brepList.Add(brepFromStb.Girder(member.StbGirders));
            _brepList.Add(brepFromStb.Post(member.StbPosts));
            _brepList.Add(brepFromStb.Beam(member.StbBeams));
            _brepList.Add(brepFromStb.Brace(member.StbBraces));
            _brepList.Add(brepFromStb.Slab(member.StbSlabs));
            _brepList.Add(brepFromStb.Wall(member.StbWalls));
        }

        private void BakeBrep()
        {
            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            var parentLayerNames = new[] { "Column", "Girder", "Post", "Beam", "Brace", "Slab", "Wall" };
            Color[] layerColors = { Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple, Color.DarkGray, Color.CornflowerBlue };
            GeometryBaker.MakeParentLayers(activeDoc, parentLayerNames, layerColors);

            Dictionary<string, string>[][] infoArray = Utils.TagUtils.GetAllSectionInfoArray(_stBridge.StbModel.StbMembers, _stBridge.StbModel.StbSections);

            foreach ((List<Brep> breps, int index) in _brepList.Select((frameBrep, index) => (frameBrep, index)))
            {
                Layer parentLayer = activeDoc.Layers.FindName(parentLayerNames[index]);
                Guid parentId = parentLayer.Id;
                foreach ((Brep brep, int bIndex) in breps.Select((brep, bIndex) => (brep, bIndex)))
                {
                    var objAttr = new ObjectAttributes();

                    Dictionary<string, string>[] infos = infoArray[index];
                    Dictionary<string, string> info = infos[bIndex];

                    foreach (KeyValuePair<string, string> pair in info)
                    {
                        objAttr.SetUserString(pair.Key, pair.Value);
                    }

                    var layer = new Layer { Name = info["name"], ParentLayerId = parentId, Color = layerColors[index] };
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
