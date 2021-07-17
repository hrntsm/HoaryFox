using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using HoaryFox.Member;
using HoaryFox.Properties;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using STBReader;
using STBReader.Member;

namespace HoaryFox.Component.Geometry
{
    public class Stb2Brep : GH_Component
    {
        private StbData _stbData;

        private readonly List<List<Brep>> _geometryBreps = new List<List<Brep>>();

        public Stb2Brep()
          : base("Stb to Brep", "S2B", "Read ST-Bridge file and display", "HoaryFox", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _geometryBreps.Clear();
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
            if (!dataAccess.GetData("Data", ref _stbData)) { return; }
            if (!dataAccess.GetData("Bake", ref isBake)) { return; }
            this.MakeBrep(isBake);

            for (var i = 0; i < 7; i++)
            {
                dataAccess.SetDataList(i, _geometryBreps[i]);
            }
        }

        protected override Bitmap Icon => Resource.Brep;
        public override Guid ComponentGuid => new Guid("7d2f0c4e-4888-4607-8548-592104f6f06f");

        private void MakeBrep(bool isBake)
        {
            var stbFrames = new List<StbFrame>
            {
                _stbData.Columns, _stbData.Girders, _stbData.Posts, _stbData.Beams, _stbData.Braces
            };
            var breps = new FrameBreps(_stbData);

            foreach (StbFrame frame in stbFrames)
            {
                _geometryBreps.Add(breps.Frame(frame));
            }
            _geometryBreps.Add(breps.Slab(_stbData.Slabs));
            _geometryBreps.Add(breps.Wall(_stbData.Walls));

            if (isBake)
            {
                this.BakeBreps(stbFrames);
            }
        }

        private void BakeBreps(IEnumerable<StbFrame> stbFrames)
        {
            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            var parentLayerNames = new[] { "Column", "Girder", "Post", "Beam", "Brace", "Slab", "Wall" };
            Color[] layerColors = { Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple, Color.DarkGray, Color.CornflowerBlue };
            Misc.MakeParentLayers(activeDoc, parentLayerNames, layerColors);

            //TODO: このネストは直す
            List<List<List<string>>> tagList = stbFrames.Select(stbFrame => Misc.GetTag(_stbData, stbFrame)).ToList();

            foreach ((List<Brep> frameBreps, int index) in _geometryBreps.Select((frameBrep, index) => (frameBrep, index)))
            {
                Layer parentLayer = activeDoc.Layers.FindName(parentLayerNames[index]);
                int parentIndex = parentLayer.Index;
                Guid parentId = parentLayer.Id;

                foreach ((Brep brep, int bIndex) in frameBreps.Select((brep, bIndex) => (brep, bIndex)))
                {
                    var objAttr = new ObjectAttributes();
                    objAttr.SetUserString("Type", parentLayerNames[index]);

                    if (index < 5)
                    {
                        List<List<string>> tags = tagList[index];
                        List<string> tag = tags[bIndex];
                        Misc.SetFrameUserString(ref objAttr, tag);

                        var layer = new Layer { Name = tag[0], ParentLayerId = parentId, Color = layerColors[index] };
                        int layerIndex = activeDoc.Layers.Add(layer);
                        if (layerIndex == -1)
                        {
                            layer = activeDoc.Layers.FindName(tag[0]);
                            layerIndex = layer.Index;
                        }
                        objAttr.LayerIndex = layerIndex;
                    }
                    else
                    {
                        objAttr.LayerIndex = parentIndex;
                    }
                    activeDoc.Objects.AddBrep(brep, objAttr);
                }
            }
        }
    }
}
