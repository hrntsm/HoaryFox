using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HoaryFoxCommon.Properties;

using STBDotNet.v202;

namespace HoaryFox.Component.Check
{
    public class Story : GH_Component
    {
        private ST_BRIDGE _stBridge;
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Story()
          : base("Story", "St",
              "Get story info",
              "HoaryFox", "Filter")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Columns", "Col", "Output the story to which the StbColumn belongs", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Girders", "Gird", "Output the story to which the StbGirder belongs", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Posts", "Pst", "Output the story to which the StbPost belongs", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Beams", "Bm", "Output the story to which the StbBeam belongs", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Braces", "Brc", "Output the story to which the StbBrace belongs", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Slabs", "Slb", "Output the story to which the StbSlab belongs", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Walls", "Wl", "Output the story to which the StbWall belongs", GH_ParamAccess.tree);
            pManager.AddTextParameter("StoryName", "Name", "output each index story name", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }

            var floorList = new GH_Structure<GH_Integer>[7];
            Dictionary<string, string>[][] infoArray = Utils.TagUtils.GetAllSectionInfoArray(_stBridge.StbModel.StbMembers, _stBridge.StbModel.StbSections);

            foreach ((Dictionary<string, string>[] infoDict, int index) in infoArray.Select((dict, index) => (dict, index)))
            {
                var decisionNode = new List<string>();
                var result = new GH_Structure<GH_Integer>();
                GetFloorDecisionNode(infoDict, decisionNode);
                SetStoryIndex(decisionNode, result);

                floorList[index] = result;
            }

            List<string> storyName = _stBridge.StbModel.StbStories.Select(story => story.name).ToList();

            for (var i = 0; i < 7; i++)
            {
                dataAccess.SetDataTree(i, floorList[i]);
            }
            dataAccess.SetDataList("StoryName", storyName);
        }

        private void SetStoryIndex(IEnumerable<string> decisionNode, GH_Structure<GH_Integer> result)
        {
            foreach ((string node, int itemIndex) in decisionNode.Select((node, itemIndex) => (node, itemIndex)))
            {
                foreach ((StbStory story, int storyIndex) in _stBridge.StbModel.StbStories.Select((story, storyIndex) =>
                    (story, storyIndex)))
                {
                    IEnumerable<string> storyNodes = story.StbNodeIdList.Select(i => i.id);
                    if (storyNodes.Contains(node))
                    {
                        result.Append(new GH_Integer(storyIndex), new GH_Path(0, itemIndex));
                        break;
                    }
                }
            }
        }

        private void GetFloorDecisionNode(IEnumerable<Dictionary<string, string>> infoDict, ICollection<string> decisionNode)
        {
            foreach (Dictionary<string, string> info in infoDict)
            {
                switch (info["stb_element_type"])
                {
                    case "StbColumn":
                    case "StbPost":
                        decisionNode.Add(info["id_node_top"]);
                        break;
                    case "StbGirder":
                    case "StbBeam":
                    case "StbBrace":
                        decisionNode.Add(info["id_node_end"]);
                        break;
                    case "StbSlab":
                    case "StbWall":
                        decisionNode.Add(info["StbNodeIdOrder"].Split(' ')[2]);
                        break;
                    default:
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unknown stb element type");
                        break;
                }
            }
        }

        protected override Bitmap Icon => Resource.Story;
        public override Guid ComponentGuid => new Guid("5cc64205-b80e-48ba-b813-01c70110f02f");
    }
}
