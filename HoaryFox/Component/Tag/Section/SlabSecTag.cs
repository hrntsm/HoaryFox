using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.Component.Utils;
using HoaryFox.Properties;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Tag.Section
{
    public class SlabSecTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;
        private GH_Structure<GH_String> _plateTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();

        public override GH_Exposure Exposure => GH_Exposure.quarternary;

        public SlabSecTag()
          : base("Slab Section Tag", "SlabSec",
              "Display Slab Section Tag",
              "HoaryFox", "SectionTag")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _plateTags.Clear();
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

            _plateTags = GetTagStrings(_stBridge.StbModel.StbMembers.StbSlabs, _stBridge.StbModel.StbSections);
            _tagPos = GetTagPosition(_stBridge.StbModel.StbMembers.StbSlabs, _stBridge.StbModel.StbNodes);

            dataAccess.SetDataTree(0, _plateTags);
        }
        private static GH_Structure<GH_String> GetTagStrings(IEnumerable<StbSlab> slabs, StbSections sections)
        {
            var ghSecStrings = new GH_Structure<GH_String>();

            foreach (var item in slabs.Select((slab, index) => new { slab, index }))
            {
                SetSectionInfo(sections, ghSecStrings, item.slab, item.index);
            }

            return ghSecStrings;
        }

        private static void SetSectionInfo(StbSections sections, GH_Structure<GH_String> ghSecStrings, StbSlab slab, int index)
        {
            string secId = slab.id_section;
            var ghPath = new GH_Path(0, index);
            StbSlabKind_structure kindStruct = slab.kind_structure;
            switch (kindStruct)
            {
                case StbSlabKind_structure.RC:
                    StbSecSlab_RC secRc = sections.StbSecSlab_RC.First(i => i.id == secId);
                    foreach (object figure in secRc.StbSecFigureSlab_RC.Items)
                    {
                        ghSecStrings.AppendRange(TagUtils.GetSlabRcSection(figure, secRc.strength_concrete), ghPath);
                    }
                    break;
                case StbSlabKind_structure.DECK:
                    StbSecSlabDeck secDeck = sections.StbSecSlabDeck.First(i => i.id == secId);
                    ghSecStrings.AppendRange(TagUtils.GetSlabDeckSection(secDeck.StbSecFigureSlabDeck.StbSecSlabDeckStraight, secDeck.strength_concrete), ghPath);
                    break;
                case StbSlabKind_structure.PRECAST:
                    StbSecSlabPrecast secPrecast = sections.StbSecSlabPrecast.First(i => i.id == secId);
                    ghSecStrings.AppendRange(TagUtils.GetSlabPrecastSection(secPrecast.precast_type, secPrecast.StbSecProductSlabPrecast, secPrecast.strength_concrete), ghPath);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kindStruct), kindStruct, null);
            }
        }

        private static List<Point3d> GetTagPosition(IEnumerable<StbSlab> slabs, IEnumerable<StbNode> nodes)
        {
            return slabs.Select(slab => TagUtils.GetSlabTagPosition(slab.StbNodeIdOrder, slab.StbSlabOffsetList, nodes)).ToList();
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_plateTags.DataCount == 0)
            {
                return;
            }

            for (var i = 0; i < _plateTags.PathCount; i++)
            {
                List<GH_String> tags = _plateTags.Branches[i];
                string tag = tags.Aggregate(string.Empty, (current, tagString) => current + tagString + "\n");
                args.Display.Draw2dText(tag, Color.Black, _tagPos[i], true, _size);
            }
        }

        protected override Bitmap Icon => Resource.SlabSection;
        public override Guid ComponentGuid => new Guid("90b847fd-4bac-4ea1-bab9-6ed6cc7541ed");

    }
}
