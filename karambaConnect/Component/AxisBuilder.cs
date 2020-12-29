using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using STBDotNet.Elements.StbModel;

namespace karambaConnect.Component
{
    public class AxisBuilder:GH_Component
    {
        public AxisBuilder()
          : base("AxisBuilder", "Axis", "Builder StbAxes data", "HoaryFox", "Export")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Node", "StbNode data", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Distance", "Dist", "Range of the target node coordinates[mm].", GH_ParamAccess.list);
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
            var intervals = new List<Interval>();
            var names = new List<string>();
            var dir = new List<int>();

            if (!DA.GetDataList(0, nodes)) { return; }
            if (!DA.GetDataList(1, intervals)) { return; }
            if (!DA.GetDataList(2, names)) { return; }
            if (!DA.GetDataList(3, dir)) { return; }

            var axes = new List<Axis>();

            if (intervals.Count != names.Count || intervals.Count != dir.Count || names.Count != dir.Count)
            {
                throw new ArgumentOutOfRangeException("The number of items does not match.");
            }

            foreach (Interval interval in intervals)
            {
                var nodeIds = new List<NodeId>();
                switch (dir[count])
                {
                    case 0:
                        var xAxis = new XAxis
                        {
                            Id = count,
                            Name = names[count++],
                            Distance = interval.Mid
                        };
                        foreach (Node node in nodes)
                        {
                            if (node.X > interval.Min & node.X < interval.Max)
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
                        break;
                    case 1:
                        var yAxis = new YAxis
                        {
                            Id = count,
                            Name = names[count++],
                            Distance = interval.Mid
                        };
                        foreach (Node node in nodes)
                        {
                            if (node.Y > interval.Min & node.Y < interval.Max)
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
                        break;
                }
            }

            DA.SetDataList(0, axes);
        }

        // protected override Bitmap Icon => karambaConnect.Properties.Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("A09E1BF4-90FD-436D-8142-092894CE5D86");
    }
}
