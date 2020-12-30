using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using STBDotNet.Elements.StbModel;

namespace KarambaConnect.Component.StbBuilder
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
            pManager.AddNumberParameter("Height", "Height", "Story height[mm] from origin.", GH_ParamAccess.list);
            pManager.AddNumberParameter("Range", "Range", "The range of nodes to include in the axis. (height±Range)", GH_ParamAccess.list);
            pManager.AddTextParameter("Name", "Name", "Story Name", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Story", "Story", "StbStory Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var count = 0;
            var nodes = new List<Node>();
            var height = new List<double>();
            var range = new List<double>();
            var names = new List<string>();

            if (!DA.GetDataList(0, nodes)) { return; }
            if (!DA.GetDataList(1, height)) { return; }
            if (!DA.GetDataList(2, range)) { return; }
            if (!DA.GetDataList(3, names)) { return; }

            var stories = new List<Story>();

            if (height.Count != range.Count || height.Count != names.Count || range.Count != names.Count)
            {
                throw new ArgumentOutOfRangeException("The number of items does not match.");
            }

            foreach (double h in height)
            {
                var story = new Story
                {
                    Id = count + 1,
                    Name = names[count],
                    Height = h,
                    Kind = "GENERAL"
                };
                var nodeIds = new List<NodeId>();
                foreach (Node node in nodes)
                {
                    if (node.Z > h - range[count] && node.Z < h + range[count])
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
                count++;
            }

            DA.SetDataList(0, stories);
        }

        protected override Bitmap Icon => Properties.Resource.StoryBuilder;
        public override Guid ComponentGuid => new Guid("438FB2A2-5EA8-474C-8897-5244AD00A188");
    }
}
