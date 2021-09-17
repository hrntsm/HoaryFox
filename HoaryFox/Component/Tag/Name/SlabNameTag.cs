using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.Properties;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Tag.Name
{
    public class SlabNameTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;

        private readonly GH_Structure<GH_String> _plateName = new GH_Structure<GH_String>();
        private readonly List<Point3d> _platePos = new List<Point3d>();

        public override bool IsPreviewCapable => true;
        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        public SlabNameTag()
          : base("Slab Name Tag", "SlabTag",
              "Display Slab Name Tag",
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
            StbSlab[] slabs = _stBridge.StbModel.StbMembers.StbSlabs;
            foreach ((StbSlab slab, int i) in slabs.Select((slab, i) => (slab, i)))
            {
                _plateName.Append(new GH_String(slab.name), new GH_Path(0, i));
                StbSlabOffset[] offsets = slab.StbSlabOffsetList;
                string[] nodeIds = slab.StbNodeIdOrder.Split(' ');
                Point3d[] pts = SlabNodeToPoint3ds(nodeIds, nodes, offsets);
                _platePos.Add(new Point3d(pts.Average(n => n.X), pts.Average(n => n.Y), pts.Average(n => n.Z)));
            }
            dataAccess.SetDataTree(0, _plateName);
        }

        private static Point3d[] SlabNodeToPoint3ds(IReadOnlyList<string> nodeIds, StbNode[] nodes, StbSlabOffset[] offsets)
        {
            var pts = new Point3d[nodeIds.Count];
            for (var i = 0; i < nodeIds.Count; i++)
            {
                string nodeId = nodeIds[i];
                StbNode node = nodes.First(n => n.id == nodeId);
                var offsetVec = new Vector3d();
                if (offsets != null)
                {
                    foreach (StbSlabOffset offset in offsets.Where(offset => nodeId == offset.id_node))
                    {
                        offsetVec = new Vector3d(offset.offset_X, offset.offset_Y, offset.offset_Z);
                    }
                }

                pts[i] = new Point3d(node.X, node.Y, node.Z) + offsetVec;
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

        protected override Bitmap Icon => Resource.SlabName;

        public override Guid ComponentGuid => new Guid("9ee6efbb-20b5-49bb-aae9-02ca6031c09d");
    }
}
