using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using KarambaConnect.K2S;
using KarambaConnect.Properties;
using STBDotNet.Elements;
using STBDotNet.Elements.StbModel;
using STBDotNet.Elements.StbModel.StbMember;
using STBDotNet.Elements.StbModel.StbSection;
using STBDotNet.Serialization;

namespace KarambaConnect.Component.IO
{
    public class Export : GH_Component
    {
        private readonly string _defaultOutPath =
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\model.stb";
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public Export()
          : base("Export STB file", "Export", "Export ST-Bridge data.", "HoaryFox", "IO")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Node", "StbNode data", GH_ParamAccess.list);
            pManager.AddGenericParameter("Axis", "Axis", "StbAxes data", GH_ParamAccess.list);
            pManager.AddGenericParameter("Story", "Story", "StbStory data", GH_ParamAccess.list);
            pManager.AddGenericParameter("Member", "Mem", "StbMember data", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Sec", "StbSection data", GH_ParamAccess.list);
            pManager.AddTextParameter("Path", "Path", "Output path", GH_ParamAccess.item, _defaultOutPath);
            pManager.AddBooleanParameter("Out?", "Out?", "If it is true, output stb file.", GH_ParamAccess.item, false);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stb", "Stb", "StbModel Data", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var path = string.Empty;
            var isOutput = false;
            var node = new List<Node>();
            var axis = new List<Axis>();
            var story = new List<Story>();
            var members = new Members();
            var sections = new List<Section>();

            if (!dataAccess.GetDataList(0, node)) { return; }
            if (!dataAccess.GetDataList(1, axis)) { return; }
            if (!dataAccess.GetDataList(2, story)) { return; }
            if (!dataAccess.GetData(3, ref members)) { return; }
            if (!dataAccess.GetDataList(4, sections)) { return; }
            if (!dataAccess.GetData(5, ref path)) { return; }
            if (!dataAccess.GetData(6, ref isOutput)) { return; }


            var elements = new StbElements
            {
                Version = "1.4.00",
                Common = StbCommon.Set(),
                Model = new Model
                {
                    Axes = axis,
                    Members = members,
                    Nodes = node,
                    Sections = sections,
                    Stories = story
                }
            };

            if (isOutput)
            {
                var sr = new Serializer();
                sr.Serialize(elements, path);
            }

            dataAccess.SetData(0, elements);
        }

        protected override Bitmap Icon => Resource.ExportStb;
        public override Guid ComponentGuid => new Guid("41401A49-3552-4741-B8F9-4C8E0C689323");
    }
}
