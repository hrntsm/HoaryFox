using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HoaryFox.Component.Utils;

using HoaryFoxCommon.Properties;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Tag.Section
{
    public class BraceSecTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;
        private GH_Structure<GH_String> _frameTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();
        public override GH_Exposure Exposure => GH_Exposure.tertiary;

        public BraceSecTag()
          : base("Brace Section Tag", "BraceSec",
              "Display Brace Section Tag",
              "HoaryFox", "SectionTag")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _frameTags.Clear();
            _tagPos.Clear();
        }

        public override bool IsPreviewCapable => true;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("SecTag", "STag", "output section tag", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }
            if (!dataAccess.GetData("Size", ref _size)) { return; }

            _frameTags = GetTagStrings(_stBridge.StbModel.StbMembers.StbBraces, _stBridge.StbModel.StbSections);
            _tagPos = GetTagPosition(_stBridge.StbModel.StbMembers.StbBraces, _stBridge.StbModel.StbNodes);

            dataAccess.SetDataTree(0, _frameTags);
        }
        private static GH_Structure<GH_String> GetTagStrings(IEnumerable<StbBrace> braces, StbSections sections)
        {
            var ghSecStrings = new GH_Structure<GH_String>();

            if (braces == null)
            {
                return ghSecStrings;
            }

            foreach (var item in braces.Select((brace, index) => new { brace, index }))
            {
                string secId = item.brace.id_section;
                var ghPath = new GH_Path(0, item.index);
                StbBraceKind_structure kindStruct = item.brace.kind_structure;

                switch (kindStruct)
                {
                    case StbBraceKind_structure.S:
                        StbSecBrace_S secS = sections.StbSecBrace_S.First(i => i.id == secId);
                        foreach (object figureObj in secS.StbSecSteelFigureBrace_S.Items)
                        {
                            ghSecStrings.AppendRange(TagUtils.GetBraceSSection(figureObj), ghPath);
                        }
                        break;
                    case StbBraceKind_structure.RC:
                    case StbBraceKind_structure.SRC:
                        throw new ArgumentException("Unsupported section type.");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return ghSecStrings;
        }

        private static List<Point3d> GetTagPosition(IEnumerable<StbBrace> braces, IEnumerable<StbNode> nodes)
        {
            return braces == null
                ? new List<Point3d>()
                : braces.Select(beam => TagUtils.GetFrameTagPosition(beam.id_node_start, beam.id_node_end, nodes)).ToList();
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_frameTags.DataCount == 0)
            {
                return;
            }

            for (var i = 0; i < _frameTags.PathCount; i++)
            {
                List<GH_String> tags = _frameTags.Branches[i];
                string tag = tags.Aggregate(string.Empty, (current, tagString) => current + (tagString + "\n"));
                args.Display.Draw2dText(tag, Color.Black, _tagPos[i], true, _size);
            }
        }

        protected override Bitmap Icon => Resource.BraceSection;
        public override Guid ComponentGuid => new Guid("86763C20-7C6C-4D8C-9AFF-CC4380127991");

    }
}
