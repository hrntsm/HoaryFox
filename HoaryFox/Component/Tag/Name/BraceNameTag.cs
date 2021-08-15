using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using HoaryFox.Component.Utils;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Tag.Name
{
    public class BraceNameTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;

        private readonly List<string> _frameName = new List<string>();
        private readonly List<Point3d> _framePos = new List<Point3d>();

        public override bool IsPreviewCapable => true;
        public BraceNameTag()
          : base("Brace Name Tag", "BraceTag",
              "Display Brace Name Tag",
              "HoaryFox", "NameTag")
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

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }
            if (!dataAccess.GetData("Size", ref _size)) { return; }

            StbNode[] nodes = _stBridge.StbModel.StbNodes;
            StbBrace[] braces = _stBridge.StbModel.StbMembers.StbBraces;
            foreach (StbBrace brace in braces)
            {
                _frameName.Add(brace.name);

                string idNodeStart = brace.id_node_start;
                string idNodeEnd = brace.id_node_end;
                _framePos.Add(TagUtils.GetTagPosition(idNodeStart, idNodeEnd, nodes));
            }
            dataAccess.SetDataList(0, _frameName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _frameName.Count; i++)
            {
                args.Display.Draw2dText(_frameName[i], Color.Black, _framePos[i], true, _size);
            }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.BraceName;

        public override Guid ComponentGuid => new Guid("21F7885E-6321-4C0D-8974-BC40769AAEAE");
    }
}
