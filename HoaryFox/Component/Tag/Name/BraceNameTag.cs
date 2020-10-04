using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using STBReader;
using Rhino.Geometry;

namespace HoaryFox.Component.Tag.Name
{
    public class BraceNameTag:GH_Component
    {
        private StbData _stbData;
        private int _size;

        private readonly List<string> _braceName = new List<string>();
        private readonly List<Point3d> _bracePos = new List<Point3d>();

        public BraceNameTag()
          : base(name: "Brace Name Tag", nickname: "BraceTag", description: "Display Brace Name Tag", category: "HoaryFox", subCategory: "Name")
        {
        }
        
        public override bool IsPreviewCapable => true;

        public override void ClearData()
        {
            base.ClearData();
            _braceName.Clear();
            _bracePos.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Braces", "Brc", "output StbBraces name tag", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            var nodes = _stbData.Nodes;
            var braces = _stbData.Braces;
        
            for (var i = 0; i < braces.Id.Count; i++)
            {
                var idNodeStart = nodes.Id.IndexOf(braces.IdNodeStart[i]);
                var idNodeEnd = nodes.Id.IndexOf(braces.IdNodeEnd[i]);
                _braceName.Add(braces.Name[i]);
                _bracePos.Add(new Point3d( 
                    (nodes.X[idNodeStart] + nodes.X[idNodeEnd]) / 2.0,
                    (nodes.Y[idNodeStart] + nodes.Y[idNodeEnd]) / 2.0,
                    (nodes.Z[idNodeStart] + nodes.Z[idNodeEnd]) / 2.0)
                );
            }

            DA.SetDataList(0, _braceName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _braceName.Count; i++)
                args.Display.Draw2dText(_braceName[i], Color.Black, _bracePos[i], true, _size);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.BraceName;

        public override Guid ComponentGuid => new Guid("E566DDCB-4192-40B2-8E96-2083207CC5A8");
    }
}