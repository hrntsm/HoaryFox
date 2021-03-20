using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using HoaryFox.Member;
using Rhino;
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
            var layerNames = new[]
            {
                "Column", "Girder", "Posts", "Beams",
                "Braces", "Slabs", "Walls"
            };
            Color[] layerColors =
            {
                Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral,
                Color.MediumPurple, Color.DarkGray, Color.CornflowerBlue
            };


            foreach ((List<Brep> frameBreps, int index) in _frameBreps.Select((frameBrep, index) => (frameBrep, index)))
            {
                var layer = new Rhino.DocObjects.Layer { Name = layerNames[index], Color = layerColors[index] };
                int layerIndex = activeDoc.Layers.Add(layer);
                if (layerIndex == -1)
                {
                    layer = activeDoc.Layers.FindName(layerNames[index]);
                    layerIndex = layer.Index;
                }

                var objAttr = new Rhino.DocObjects.ObjectAttributes { LayerIndex = layerIndex };


                foreach (Brep brep in frameBreps)
                {
                    activeDoc.Objects.AddBrep(brep, objAttr);
                }
            }
        }
    }
}
