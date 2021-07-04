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
    public class GirderSecTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;
        private GH_Structure<GH_String> _frameTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();

        public GirderSecTag()
          : base("Girder Section Tag", "GirderSec",
              "Display Girder Section Tag",
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

            _frameTags = GetTagStrings(_stBridge.StbModel.StbMembers.StbGirders, _stBridge.StbModel.StbSections);
            _tagPos = GetTagPosition(_stBridge.StbModel.StbMembers.StbGirders, _stBridge.StbModel.StbNodes);

            DA.SetDataTree(0, _frameTags);
        }
        private static GH_Structure<GH_String> GetTagStrings(IEnumerable<StbGirder> beams, StbSections sections)
        {
            var ghSecStrings = new GH_Structure<GH_String>();

            foreach (var item in beams.Select((beam, index) => new { beam, index }))
            {
                string secId = item.beam.id_section;
                var ghPath = new GH_Path(0, item.index);
                StbGirderKind_structure kindStruct = item.beam.kind_structure;

                switch (kindStruct)
                {
                    case StbGirderKind_structure.RC:
                        StbSecBeam_RC secRc = sections.StbSecBeam_RC.First(i => i.id == secId);
                        foreach (object figureObj in secRc.StbSecFigureBeam_RC.Items)
                        {
                            ghSecStrings.AppendRange(TagUtil.GetBeamRcSection(figureObj, secRc.strength_concrete), ghPath);
                        }
                        break;
                    case StbGirderKind_structure.S:
                        StbSecBeam_S secS = sections.StbSecBeam_S.First(i => i.id == secId);
                        foreach (object figureObj in secS.StbSecSteelFigureBeam_S.Items)
                        {
                            ghSecStrings.AppendRange(TagUtil.GetBeamSSection(figureObj), ghPath);
                        }
                        break;
                    case StbGirderKind_structure.SRC:
                        StbSecBeam_SRC secSrc = sections.StbSecBeam_SRC.First(i => i.id == secId);
                        foreach (object figureObj in secSrc.StbSecFigureBeam_SRC.Items)
                        {
                            ghSecStrings.AppendRange(TagUtil.GetBeamRcSection(figureObj, secSrc.strength_concrete), ghPath);
                        }
                        foreach (object figureObj in secSrc.StbSecSteelFigureBeam_SRC.Items)
                        {
                            ghSecStrings.AppendRange(TagUtil.GetBeamSSection(figureObj), ghPath);
                        }
                        break;
                    case StbGirderKind_structure.UNDEFINED:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            return ghSecStrings;
        }

        private static List<Point3d> GetTagPosition(IEnumerable<StbGirder> girders, IEnumerable<StbNode> nodes)
        {
            return girders.Select(girder => TagUtil.GetTagPosition(girder.id_node_start, girder.id_node_end, nodes)).ToList();
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

        protected override Bitmap Icon => Resource.GirderSection;
        public override Guid ComponentGuid => new Guid("6E1E7529-826B-4214-9C63-B77AF0715009");

    }
}
