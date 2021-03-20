using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using HoaryFox.Member;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using STBReader;
using STBReader.Member;

namespace HoaryFox.Component.Geometry
{
    public class BrepBaker : GH_Component
    {
        private StbData _stbData;

        private List<Brep> _slabBreps = new List<Brep>();
        private List<Brep> _wallBreps = new List<Brep>();
        private readonly List<List<Brep>> _frameBreps = new List<List<Brep>>();

        public BrepBaker()
          : base("BrepBaker", "BBake", "Bake Brep by Tags", "HoaryFox", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _slabBreps.Clear();
            _wallBreps.Clear();
            _frameBreps.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bake", "Bake", "If it true, bake geometry.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var isBake = false;
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Bake", ref isBake)) { return; }

            if (isBake)
            {
                this.BakeBrep();
            }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.Brep;
        public override Guid ComponentGuid => new Guid("F34C2967-2E58-48C6-A5A3-4770FDA4B3EC");

        private void BakeBrep()
        {
            var stbFrames = new List<StbFrame>
            {
                _stbData.Columns, _stbData.Girders, _stbData.Posts, _stbData.Beams, _stbData.Braces
            };

            var breps = new FrameBreps(_stbData);

            _slabBreps = breps.Slab(_stbData.Slabs);
            _wallBreps = breps.Wall(_stbData.Walls);

            foreach (StbFrame frame in stbFrames)
            {
                _frameBreps.Add(breps.Frame(frame));
            }
            _frameBreps.Add(_slabBreps);
            _frameBreps.Add(_wallBreps);

            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            var parentLayerNames = new[]
            {
                "Column", "Girder", "Post", "Beam", "Brace",
                "Slab", "Wall"
            };
            Color[] layerColors =
            {
                Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple,
                Color.DarkGray, Color.CornflowerBlue
            };

            MakeParentLayers(activeDoc, parentLayerNames, layerColors);

            //TODO: このネストは直す
            List<List<List<string>>> tagList = stbFrames.Select(stbFrame => GetTag(stbFrame)).ToList();

            foreach ((List<Brep> frameBreps, int index) in _frameBreps.Select((frameBrep, index) => (frameBrep, index)))
            { 
                Layer parentLayer = activeDoc.Layers.FindName(parentLayerNames[index]);
                int parentIndex = parentLayer.Index;
                Guid parentId = parentLayer.Id;

                foreach ((Brep brep, int bIndex) in frameBreps.Select((brep, bIndex) => (brep, bIndex)))
                {
                    var objAttr = new Rhino.DocObjects.ObjectAttributes();
                    objAttr.SetUserString("Type", parentLayerNames[index]);

                    if (index < 5)
                    {
                        List<List<string>> tags = tagList[index];
                        List<string> tag = tags[bIndex];
                        objAttr.SetUserString("Tag", tag[0]);
                        objAttr.SetUserString("ShapeType", tag[1]);
                        objAttr.SetUserString("Height", tag[2]);
                        objAttr.SetUserString("Width", tag[3]);
                        objAttr.SetUserString("t1", tag[4]);
                        objAttr.SetUserString("t2", tag[5]);

                        var layer = new Rhino.DocObjects.Layer { Name = tag[0], ParentLayerId = parentId, Color = layerColors[index] };
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

        private static void MakeParentLayers(RhinoDoc activeDoc, IEnumerable<string> parentLayerNames, IReadOnlyList<Color> layerColors)
        {
            foreach ((string name, int index) in parentLayerNames.Select((name, index) => (name, index)))
            {
                var parentLayer = new Rhino.DocObjects.Layer { Name = name, Color = layerColors[index] };
                activeDoc.Layers.Add(parentLayer);
            }
        }

        private List<List<string>> GetTag(StbFrame stbFrame)
        {
            var tags = new CreateTag(_stbData.Nodes, _stbData.SecColumnRc, _stbData.SecColumnS, _stbData.SecBeamRc, _stbData.SecBeamS, _stbData.SecBraceS, _stbData.SecSteel);
            return tags.FrameList(stbFrame);
        }
    }
}
