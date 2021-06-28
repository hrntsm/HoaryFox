﻿using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Tag.Name
{
    public class ColumnNameTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;

        private readonly List<string> _frameName = new List<string>();
        private readonly List<Point3d> _framePos = new List<Point3d>();

        public override bool IsPreviewCapable => true;

        protected ColumnNameTag()
          : base(name: "Column Name Tag", nickname: "ColumnTag", description: "Display Column Name Tag", "HoaryFox", "NameTag_v2")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _frameName.Clear();
            _framePos.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("NameTag", "NTag", "output name tag", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stBridge)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            StbNode[] nodes = _stBridge.StbModel.StbNodes;
            StbColumn[] columns = _stBridge.StbModel.StbMembers.StbColumns;
            foreach (StbColumn column in columns)
            {
                _frameName.Add(column.name);

                string idNodeStart = column.id_node_bottom;
                string idNodeEnd = column.id_node_top;
                _framePos.Add(Util.GetFramePosition(idNodeStart, idNodeEnd, nodes));
            }
            DA.SetDataList(0, _frameName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _frameName.Count; i++)
            {
                args.Display.Draw2dText(_frameName[i], Color.Black, _framePos[i], true, _size);
            }
        }

        protected override Bitmap Icon => Properties.Resource.ColumnName;
        public override Guid ComponentGuid => new Guid("A21D711E-8AFB-4B72-8EEA-D8DBABA72462");
    }
}
