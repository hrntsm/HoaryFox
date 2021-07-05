using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.Properties;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Tag.Section
{
    public class PostSecTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;
        private GH_Structure<GH_String> _frameTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();

        public PostSecTag()
          : base("Post Section Tag", "PostSec",
              "Display Post Section Tag",
              "HoaryFox2", "Section")
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

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stBridge)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            _frameTags = GetTagStrings(_stBridge.StbModel.StbMembers.StbPosts, _stBridge.StbModel.StbSections);
            _tagPos = GetTagPosition(_stBridge.StbModel.StbMembers.StbPosts, _stBridge.StbModel.StbNodes);

            DA.SetDataTree(0, _frameTags);
        }
        private static GH_Structure<GH_String> GetTagStrings(IEnumerable<StbPost> columns, StbSections sections)
        {
            var ghSecStrings = new GH_Structure<GH_String>();

            foreach (var item in columns.Select((column, index) => new { column, index }))
            {
                string secId = item.column.id_section;
                var ghPath = new GH_Path(0, item.index);
                StbColumnKind_structure kindStruct = item.column.kind_structure;

                switch (kindStruct)
                {
                    case StbColumnKind_structure.RC:
                        StbSecColumn_RC secRc = sections.StbSecColumn_RC.First(i => i.id == secId);
                        ghSecStrings.AppendRange(TagUtil.GetColumnRcSection(secRc.StbSecFigureColumn_RC.Item, secRc.strength_concrete), ghPath);
                        break;
                    case StbColumnKind_structure.S:
                        StbSecColumn_S secS = sections.StbSecColumn_S.First(i => i.id == secId);
                        foreach (object figureObj in secS.StbSecSteelFigureColumn_S.Items)
                        {
                            ghSecStrings.AppendRange(TagUtil.GetColumnSSection(figureObj), ghPath);
                        }
                        break;
                    case StbColumnKind_structure.SRC:
                        StbSecColumn_SRC secSrc = sections.StbSecColumn_SRC.First(i => i.id == secId);
                        ghSecStrings.AppendRange(TagUtil.GetColumnRcSection(secSrc.StbSecFigureColumn_SRC.Item, secSrc.strength_concrete), ghPath);
                        foreach (object figureObj in secSrc.StbSecSteelFigureColumn_SRC.Items)
                        {
                            ghSecStrings.AppendRange(TagUtil.GetColumnSSection(figureObj), ghPath);
                        }
                        break;
                    case StbColumnKind_structure.CFT:
                    case StbColumnKind_structure.UNDEFINED:
                        throw new ArgumentException("Unsupported section type.");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return ghSecStrings;
        }

        private static List<Point3d> GetTagPosition(IEnumerable<StbPost> columns, IEnumerable<StbNode> nodes)
        {
            return columns.Select(beam => TagUtil.GetTagPosition(beam.id_node_bottom, beam.id_node_top, nodes)).ToList();
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

        protected override Bitmap Icon => Resource.PostSection;
        public override Guid ComponentGuid => new Guid("B0C5FFF4-B510-4A0F-B441-C0B3144BF757");

    }
}
