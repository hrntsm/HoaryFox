using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using KarambaConnectCommon.Properties;

using STBDotNet.v202;

namespace KarambaConnect.Component.StbBuilder
{
    public class StoryBuilder : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;

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

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var count = 0;
            var nodes = new List<StbNode>();
            var height = new List<double>();
            var range = new List<double>();
            var names = new List<string>();

            if (!dataAccess.GetDataList(0, nodes)) { return; }
            if (!dataAccess.GetDataList(1, height)) { return; }
            if (!dataAccess.GetDataList(2, range)) { return; }
            if (!dataAccess.GetDataList(3, names)) { return; }

            var stories = new List<StbStory>();

            if (height.Count != range.Count || height.Count != names.Count || range.Count != names.Count)
            {
                throw new ArgumentException("The number of items does not match.");
            }

            foreach (double h in height)
            {
                StbNodeId[] nodeIds = (from StbNode node in nodes
                                       where node.Z > h - range[count] && node.Z < h + range[count]
                                       select new StbNodeId { id = node.id }).ToArray();
                CheckNodeIdsNull(nodeIds);
                stories.Add(CreateStory(count, names, h, nodeIds));
                count++;
            }

            dataAccess.SetDataList(0, stories);
        }

        private static StbStory CreateStory(int count, List<string> names, double h, StbNodeId[] nodeIds)
        {
            return new StbStory
            {
                id = (count + 1).ToString(),
                name = names[count],
                height = h,
                kind = StbStoryKind.GENERAL,
                StbNodeIdList = nodeIds
            };
        }

        private static void CheckNodeIdsNull(StbNodeId[] nodeIds)
        {
            if (nodeIds.Length == 0)
            {
                throw new ArgumentException("There are no nodes in the target height range.");
            }
        }

        protected override Bitmap Icon => Resource.StoryBuilder;
        public override Guid ComponentGuid => new Guid("438FB2A2-5EA8-474C-8897-5244AD00A188");
    }
}
