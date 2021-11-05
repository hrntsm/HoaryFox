using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.Properties;
using STBDotNet.v202;

namespace HoaryFox.Component.Check
{
    public class Floor : GH_Component
    {
        private ST_BRIDGE _stBridge;
        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Floor()
          : base("Floor", "Fl",
              "Check floor",
              "HoaryFox", "Filter")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddIntegerParameter("Columns", "Col", "output StbColumn floors", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Girders", "Gird", "output StbGirders floors", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Posts", "Pst", "output StbPosts floors", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Beams", "Bm", "output StbBeams floors", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Braces", "Brc", "output StbBraces floors", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Slabs", "Slb", "output StbSlabs floors", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Walls", "Wl", "output StbWalls floors", GH_ParamAccess.tree);
            pManager.AddTextParameter("FloorName", "Name", "output each index floor name", GH_ParamAccess.list);
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
                SetFloorIndex(decisionNode, result);

                floorList[index] = result;
            }

            List<string> floorName = _stBridge.StbModel.StbStories.Select(story => story.name).ToList();

            for (var i = 0; i < 7; i++)
            {
                dataAccess.SetDataTree(i, floorList[i]);
            }
            dataAccess.SetDataList("FloorName", floorName);
        }

        private void SetFloorIndex(IEnumerable<string> decisionNode, GH_Structure<GH_Integer> result)
        {
            foreach ((string node, int itemIndex) in decisionNode.Select((node, itemIndex) => (node, itemIndex)))
            {
                foreach ((StbStory story, int storyIndex) in _stBridge.StbModel.StbStories.Select((story, storyIndex) =>
                    (story, storyIndex)))
                {
                    IEnumerable<string> floorNodes = story.StbNodeIdList.Select(i => i.id);
                    if (floorNodes.Contains(node))
                    {
                        result.Append(new GH_Integer(storyIndex), new GH_Path(0, itemIndex));
                        break;
                    }
                }
            }
        }

        private static void GetFloorDecisionNode(IEnumerable<Dictionary<string, string>> infoDict, ICollection<string> decisionNode)
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
                }
            }
        }

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("5cc64205-b80e-48ba-b813-01c70110f02f");
    }
}
