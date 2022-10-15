using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HoaryFoxCommon.Properties;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Tag.Name
{
    public class WallNameTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;

        private readonly GH_Structure<GH_String> _plateName = new GH_Structure<GH_String>();
        private readonly List<Point3d> _platePos = new List<Point3d>();

        public override bool IsPreviewCapable => true;
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        public WallNameTag()
          : base("Wall Name Tag", "WallTag",
              "Display Wall Name Tag",
              "HoaryFox", "NameTag")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _plateName.Clear();
            _platePos.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("NameTag", "NTag", "output name tag", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }
            if (!dataAccess.GetData("Size", ref _size)) { return; }

            StbNode[] nodes = _stBridge.StbModel.StbNodes;
            StbWall[] walls = _stBridge.StbModel.StbMembers.StbWalls;
            foreach ((StbWall wall, int i) in walls.Select((wall, index) => (wall, index)))
            {
                _plateName.Append(new GH_String(wall.name), new GH_Path(0, i));

                Point3d[] pts = WallNodeToPoint3ds(nodes, wall.StbNodeIdOrder.Split(' '));
                _platePos.Add(new Point3d(pts.Average(n => n.X), pts.Average(n => n.Y), pts.Average(n => n.Z)));
            }
            dataAccess.SetDataTree(0, _plateName);
        }

        private static Point3d[] WallNodeToPoint3ds(StbNode[] nodes, string[] nodeIds)
        {
            var pts = new Point3d[nodeIds.Length];
            for (int i = 0; i < nodeIds.Length; i++)
            {
                string nodeId = nodeIds[i];
                StbNode node = nodes.First(n => n.id == nodeId);
                pts[i] = new Point3d(node.X, node.Y, node.Z);
            }

            return pts;
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _plateName.PathCount; i++)
            {
                args.Display.Draw2dText(_plateName.Branches[i][0].Value, Color.Black, _platePos[i], true, _size);
            }
        }

        protected override Bitmap Icon => Resource.WallName;

        public override Guid ComponentGuid => new Guid("713d1503-eebd-4504-83f0-ddd072a11188");
    }
}
