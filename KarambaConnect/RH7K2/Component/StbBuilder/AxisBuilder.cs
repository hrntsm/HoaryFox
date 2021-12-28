using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using KarambaConnectCommon.Properties;
using STBDotNet.v202;

namespace KarambaConnect.Component.StbBuilder
{
    public class AxisBuilder : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public AxisBuilder()
          : base("AxisBuilder", "Axis", "Builder StbAxes data", "HoaryFox", "StbBuilder")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Node", "StbNode data", GH_ParamAccess.list);
            pManager.AddNumberParameter("Distance", "Dist", "Axis coordinates[mm] from origin.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Range", "Range", "The range of nodes to include in the axis. (Dist±Range)", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "Name", "Axis Name", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Direction", "Dir", "0: X Axis, 1: Y Axis", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Axis", "Axis", "StbAxis Data", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var count = 0;
            var nodes = new List<StbNode>();
            var distance = new List<double>();
            var range = new List<double>();
            var names = new List<string>();
            var dir = new List<int>();

            if (!dataAccess.GetDataList(0, nodes)) { return; }
            if (!dataAccess.GetDataList(1, distance)) { return; }
            if (!dataAccess.GetDataList(2, range)) { return; }
            if (!dataAccess.GetDataList(3, names)) { return; }
            if (!dataAccess.GetDataList(4, dir)) { return; }

            if (distance.Count != names.Count || distance.Count != range.Count || distance.Count != dir.Count ||
                range.Count != names.Count || range.Count != dir.Count || names.Count != dir.Count)
            {
                throw new ArgumentException("The number of items does not match.");
            }

            var xAxisList = new List<StbParallelAxis>();
            var yAxisList = new List<StbParallelAxis>();

            foreach (double dist in distance)
            {
                var nodeIds = new List<StbNodeId>();
                if (dir[count] == 0)
                {
                    nodeIds.AddRange(from StbNode node in nodes
                                     where node.X > dist - range[count] && node.X < dist + range[count]
                                     select new StbNodeId { id = node.id });
                    CheckNodeIdsNull(nodeIds);
                    xAxisList.Add(CreateParallelAxis(count, names, dist, nodeIds));
                }
                else if (dir[count] == 1)
                {
                    nodeIds.AddRange(from StbNode node in nodes
                                     where node.Y > dist - range[count] && node.Y < dist + range[count]
                                     select new StbNodeId { id = node.id });
                    CheckNodeIdsNull(nodeIds);
                    yAxisList.Add(CreateParallelAxis(count, names, dist, nodeIds));
                }

                count++;
            }

            var axes = new StbAxes();
            var xAxes = new StbParallelAxes { X = 0, Y = 0, angle = 270, group_name = "X", StbParallelAxis = xAxisList.ToArray() };
            var yAxes = new StbParallelAxes { X = 0, Y = 0, angle = 0, group_name = "Y", StbParallelAxis = yAxisList.ToArray() };
            axes.StbParallelAxes = new[] { xAxes, yAxes };

            dataAccess.SetData(0, axes);
        }

        private static void CheckNodeIdsNull(IReadOnlyCollection<StbNodeId> nodeIds)
        {
            if (nodeIds.Count == 0)
            {
                throw new ArgumentException("There are no nodes in the target distance range.");
            }
        }

        private static StbParallelAxis CreateParallelAxis(int count, IReadOnlyList<string> names, double dist, List<StbNodeId> nodeIds)
        {
            return new StbParallelAxis()
            {
                id = (count + 1).ToString(),
                name = names[count],
                distance = dist,
                StbNodeIdList = nodeIds.ToArray()
            };
        }

        protected override Bitmap Icon => Resource.AxisBuilder;
        public override Guid ComponentGuid => new Guid("A09E1BF4-90FD-436D-8142-092894CE5D86");
    }
}
