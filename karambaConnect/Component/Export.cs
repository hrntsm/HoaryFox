using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using karambaConnect.K2S;
using STBDotNet.Elements;
using STBDotNet.Elements.StbModel;
using STBDotNet.Elements.StbModel.StbMember;
using STBDotNet.Elements.StbModel.StbSection;
using STBDotNet.Serialization;

namespace karambaConnect.Component
{
    public class Export:GH_Component
    {
        private readonly string _defaultOutPath =
            Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\model.stb";
        
        public Export()
          : base("Export Stb file", "Export", "Export ST-Bridge data.", "HoaryFox", "Export")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Nd", "StbNode data", GH_ParamAccess.list);
            pManager.AddGenericParameter("Axis", "Axs", "StbAxes data", GH_ParamAccess.list);
            pManager.AddGenericParameter("Story", "Sty", "StbStory data", GH_ParamAccess.list);
            pManager.AddGenericParameter("Member", "Mem","StbMember data", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Sec", "StbSection data", GH_ParamAccess.list);
            pManager.AddTextParameter("Path", "Path", "Output path", GH_ParamAccess.item, _defaultOutPath);
            pManager.AddBooleanParameter("Out?", "Out?", "If it is true, output stb file.", GH_ParamAccess.item, false);

        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Stb", "Stb", "StbModel Data", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var path = string.Empty;
            var isOutput = false;
            var node = new List<Node>();
            var axis = new List<Axis>();
            var story = new List<Story>();
            var members = new Members();
            var sections = new List<Section>();
            
            if (!DA.GetDataList(0, node)) { return; }
            if (!DA.GetDataList(1, axis)) { return; }
            if (!DA.GetDataList(2, story)) { return; }
            if (!DA.GetData(3, ref members)) { return; }
            if (!DA.GetDataList(4, sections)) { return; }
            if (!DA.GetData(5, ref path)) { return; }
            if (!DA.GetData(6, ref isOutput)) { return; }


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

            DA.SetData(0, elements);
        }

        // protected override Bitmap Icon => karambaConnect.Properties.Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("41401A49-3552-4741-B8F9-4C8E0C689323");
    }
}
