using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Tag.Name
{
    public class WallNameTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;

        private readonly List<string> _frameName = new List<string>();
        private readonly List<Point3d> _framePos = new List<Point3d>();

        public override bool IsPreviewCapable => true;
        public WallNameTag()
          : base("Wall Name Tag", "WallTag",
              "Display Wall Name Tag",
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
            StbWall[] walls = _stBridge.StbModel.StbMembers.StbWalls;
            foreach (StbWall wall in walls)
            {
                _frameName.Add(wall.name);

                string[] nodeIds = wall.StbNodeIdOrder.Split(' ');
                var pts = new Point3d[nodeIds.Length];
                for (int i = 0; i < nodeIds.Length; i++)
                {
                    string nodeId = nodeIds[i];
                    StbNode node = nodes.First(n => n.id == nodeId);
                    pts[i] = new Point3d(node.X, node.Y, node.Z);
                }
                _framePos.Add(new Point3d(pts.Average(n => n.X), pts.Average(n => n.Y), pts.Average(n => n.Z)));
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

        protected override Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("713d1503-eebd-4504-83f0-ddd072a11188");
    }
}
