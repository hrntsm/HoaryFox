using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using KarambaConnect.Properties;
using STBDotNet.Serialization;
using STBDotNet.v202;

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
            pManager.AddGenericParameter("Axis", "Axis", "StbAxes data", GH_ParamAccess.item);
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
            var nodes = new List<StbNode>();
            var axes = new StbAxes();
            var stories = new List<StbStory>();
            var members = new StbMembers();
            var sections = new StbSections();

            if (!dataAccess.GetDataList(0, nodes)) { return; }
            if (!dataAccess.GetData(1, ref axes)) { return; }
            if (!dataAccess.GetDataList(2, stories)) { return; }
            if (!dataAccess.GetData(3, ref members)) { return; }
            if (!dataAccess.GetData(4, ref sections)) { return; }
            if (!dataAccess.GetData(5, ref path)) { return; }
            if (!dataAccess.GetData(6, ref isOutput)) { return; }

            var stbData = new ST_BRIDGE
            {
                version = "2.0.2",
                StbCommon = new StbCommon
                {
                    project_name = ActiveCanvasFileName(),
                    app_name = "HoaryFox",
                },
                StbModel = new StbModel
                {
                    StbAxes = axes,
                    StbStories = stories.ToArray(),
                    StbNodes = nodes.ToArray(),
                    StbMembers = members,
                    StbSections = sections,
                    StbJoints = new StbJoints(),
                },
                StbAnaModels = Array.Empty<StbAnaModel>(),
                StbCalData = new StbCalData(),
            };

            if (isOutput)
            {
                bool result = Serializer.Serialize(stbData, path, STBDotNet.Enums.Version.Stb202);
                if (!result)
                {
                    throw new Exception("Failed to serialize.");
                }
            }

            dataAccess.SetData(0, stbData);
        }

        private static string ActiveCanvasFileName()
        {
            var fileName = Grasshopper.Instances.ActiveCanvas.Document.ToString();
            if (fileName.EndsWith("*"))
            {
                fileName = fileName.Substring(0, fileName.Length - 1) + ".gh";
            }

            return fileName;
        }

        protected override Bitmap Icon => Resource.ExportStb;
        public override Guid ComponentGuid => new Guid("41401A49-3552-4741-B8F9-4C8E0C689323");
    }
}
