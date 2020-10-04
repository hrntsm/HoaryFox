using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using STBReader;
using Rhino.Geometry;

namespace HoaryFox.Component.Tag.Name
{
    public class GirderNameTag:GH_Component
    {
        private StbData _stbData;
        private int _size;
        
        private readonly List<string> _girderName = new List<string>();
        private readonly List<Point3d> _girderPos = new List<Point3d>();

        public GirderNameTag()
          : base(name: "Girder Name Tag", nickname: "GirderTag", description: "Display girder Name Tag ", category: "HoaryFox", subCategory: "Name")
        {
        }
        
        public override bool IsPreviewCapable => true;

        public override void ClearData()
        {
            base.ClearData();
            _girderName.Clear();
            _girderPos.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Girders", "Gird", "output StbGirders name tag", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            var nodes = _stbData.Nodes;
            var girders = _stbData.Girders;
            
            for (var i = 0; i < girders.Id.Count; i++)
            {
                var idNodeStart = nodes.Id.IndexOf(girders.IdNodeStart[i]);
                var idNodeEnd = nodes.Id.IndexOf(girders.IdNodeEnd[i]);
                _girderName.Add(girders.Name[i]);
                _girderPos.Add(new Point3d( 
                    (nodes.X[idNodeStart] + nodes.X[idNodeEnd]) / 2.0,
                    (nodes.Y[idNodeStart] + nodes.Y[idNodeEnd]) / 2.0,
                    (nodes.Z[idNodeStart] + nodes.Z[idNodeEnd]) / 2.0)
                );
            }

            DA.SetDataList(0, _girderName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _girderName.Count; i++)
                args.Display.Draw2dText(_girderName[i], Color.Black, _girderPos[i], true, _size);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.GirderName;

        public override Guid ComponentGuid => new Guid("35D72484-2675-487E-A970-5DE885582312");
    }
}