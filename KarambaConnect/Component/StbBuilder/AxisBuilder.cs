using System;
using System.Drawing;
using System.Collections.Generic;
using Grasshopper.Kernel;
using STBDotNet.Elements.StbModel;

namespace KarambaConnect.Component.StbBuilder
{
    public class AxisBuilder:GH_Component
    {
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
            pManager.AddGenericParameter("Axis", "Axis", "StbAxis Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var count = 0;
            var nodes = new List<Node>();
            var distance = new List<double>();
            var range = new List<double>();
            var names = new List<string>();
            var dir = new List<int>();

            if (!DA.GetDataList(0, nodes)) { return; }
            if (!DA.GetDataList(1, distance)) { return; }
            if (!DA.GetDataList(2, range)) { return; }
            if (!DA.GetDataList(3, names)) { return; }
            if (!DA.GetDataList(4, dir)) { return; }

            var axes = new List<Axis>();

            if (distance.Count != names.Count || distance.Count != range.Count || distance.Count != dir.Count ||
                range.Count != names.Count || range.Count != dir.Count || names.Count != dir.Count)
            {
                throw new ArgumentOutOfRangeException("The number of items does not match.");
            }

            foreach (double dist in distance)
            {
                var nodeIds = new List<NodeId>();
                if (dir[count] == 0)
                {
                    var xAxis = new XAxis
                    {
                        Id = count + 1,
                        Name = names[count],
                        Distance = dist
                    };
                    foreach (Node node in nodes)
                    {
                        if (node.X > dist - range[count] && node.X < dist + range[count])
                        {
                            nodeIds.Add(new NodeId(node.Id));
                        }
                    }

                    if (nodeIds.Count == 0)
                    {
                        throw new ArgumentException("There are no nodes in the target height range.");
                    }

                    xAxis.NodeIdList = nodeIds;
                    axes.Add(xAxis);
                }
                else if (dir[count] == 1)
                {
                    var yAxis = new YAxis
                    {
                        Id = count + 1,
                        Name = names[count],
                        Distance = dist
                    };
                    foreach (Node node in nodes)
                    {
                        if (node.Y > dist - range[count] && node.Y < dist + range[count])
                        {
                            nodeIds.Add(new NodeId(node.Id));
                        }
                    }

                    if (nodeIds.Count == 0)
                    {
                        throw new ArgumentException("There are no nodes in the target distance range.");
                    }

                    yAxis.NodeIdList = nodeIds;
                    axes.Add(yAxis);
                }

                count++;
            }

            DA.SetDataList(0, axes);
        }

        protected override Bitmap Icon => Properties.Resource.AxisBuilder;
        public override Guid ComponentGuid => new Guid("A09E1BF4-90FD-436D-8142-092894CE5D86");
    }
}
