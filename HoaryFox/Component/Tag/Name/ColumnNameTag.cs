using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using STBReader;
using Rhino.Geometry;
using STBReader.Member;
using STBReader.Model;

namespace HoaryFox.Component.Tag.Name
{
    public class ColumnNameTag:GH_Component
    {
        private StbData _stbData;
        private int _size;

        private readonly List<string> _columnName = new List<string>();
        private readonly List<Point3d> _columnPos = new List<Point3d>();
        
        public ColumnNameTag()
          : base(name: "Column Name Tag", nickname: "ColumnTag", description: "Display Column Name Tag", category: "HoaryFox", subCategory: "Name")
        {
        }
        
        public override bool IsPreviewCapable => true;

        public override void ClearData()
        {
            base.ClearData();
            _columnName.Clear();
            _columnPos.Clear();    
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Columns", "Col", "output StbColumns name tag", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            StbNodes nodes = _stbData.Nodes;
            StbColumns columns = _stbData.Columns;
            
            for (var i = 0; i < columns.Id.Count; i++)
            {
                int idNodeStart = nodes.Id.IndexOf(columns.IdNodeStart[i]);
                int idNodeEnd = nodes.Id.IndexOf(columns.IdNodeEnd[i]);
                _columnName.Add(columns.Name[i]);
                _columnPos.Add(new Point3d(
                    (nodes.X[idNodeStart] + nodes.X[idNodeEnd]) / 2.0,
                    (nodes.Y[idNodeStart] + nodes.Y[idNodeEnd]) / 2.0,
                    (nodes.Z[idNodeStart] + nodes.Z[idNodeEnd]) / 2.0)
                );
            }

            DA.SetDataList(0, _columnName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _columnName.Count; i++)
            {
                args.Display.Draw2dText(_columnName[i], Color.Black, _columnPos[i], true, _size);
            }
        }

        protected override Bitmap Icon => Properties.Resource.ColumnName;
        public override Guid ComponentGuid => new Guid("806B9DBE-0207-4E79-A1BE-DD0B37BA9B31");
    }
}