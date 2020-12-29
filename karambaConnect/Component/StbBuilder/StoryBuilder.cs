using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;
using STBDotNet.Elements.StbModel;

namespace karambaConnect.Component.StbBuilder
{
    public class StoryBuilder:GH_Component
    {
        public StoryBuilder()
          : base("StoryBuilder", "Story", "Builder StbStory data", "HoaryFox", "StbBuilder")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Node", "StbNode data", GH_ParamAccess.list);
            pManager.AddIntervalParameter("Height", "Height", "Range of the target node Z coordinates[mm].", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "Name", "Story Name", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Story", "Story", "StbStory Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var count = 1;
            var nodes = new List<Node>();
            var intervals = new List<Interval>();
            var names = new List<string>();

            if (!DA.GetDataList(0, nodes)) { return; }
            if (!DA.GetDataList(1, intervals)) { return; }
            if (!DA.GetDataList(2, names)) { return; }

            var stories = new List<Story>();

            if (intervals.Count != names.Count)
            {
                throw new ArgumentOutOfRangeException("The number of floor heights does not match the number of floor names.");
            }

            foreach (Interval interval in intervals)
            {
                var story = new Story
                {
                    Id = count,
                    Name = names[count++ - 1],
                    Height = interval.Mid,
                    Kind = "GENERAL"
                };
                var nodeIds = new List<NodeId>();
                foreach (Node node in nodes)
                {
                    if (node.Z > interval.Min & node.Z < interval.Max)
                    {
                        nodeIds.Add(new NodeId(node.Id));
                    }
                }

                if (nodeIds.Count == 0)
                {
                    throw new ArgumentException("There are no nodes in the target height range.");
                }

                story.NodeIdList = nodeIds;
                stories.Add(story);
            }

            DA.SetDataList(0, stories);
        }

        // protected override Bitmap Icon => karambaConnect.Properties.Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("438FB2A2-5EA8-474C-8897-5244AD00A188");
    }
}
