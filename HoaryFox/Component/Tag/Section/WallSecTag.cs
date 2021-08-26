using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.Component.Utils;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Tag.Section
{
    public class WallSecTag : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;
        private GH_Structure<GH_String> _plateTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();

        public WallSecTag()
          : base("Wall Section Tag", "WallSec",
              "Display Wall Section Tag",
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

            _plateTags = GetTagStrings(_stBridge.StbModel.StbMembers.StbWalls, _stBridge.StbModel.StbSections);
            _tagPos = GetTagPosition(_stBridge.StbModel.StbMembers.StbWalls, _stBridge.StbModel.StbNodes);

            dataAccess.SetDataTree(0, _plateTags);
        }
        private static GH_Structure<GH_String> GetTagStrings(IEnumerable<StbWall> walls, StbSections sections)
        {
            var ghSecStrings = new GH_Structure<GH_String>();

            foreach (var item in walls.Select((wall, index) => new { wall, index }))
            {
                string secId = item.wall.id_section;
                var ghPath = new GH_Path(0, item.index);

                StbSecWall_RC secRc = sections.StbSecWall_RC.First(i => i.id == secId);
                StbSecWall_RC_Straight figure = secRc.StbSecFigureWall_RC.StbSecWall_RC_Straight;
                ghSecStrings.AppendRange(TagUtils.GetWallRcSection(figure, secRc.strength_concrete), ghPath);
            }

            return ghSecStrings;
        }

        private static List<Point3d> GetTagPosition(IEnumerable<StbWall> walls, IEnumerable<StbNode> nodes)
        {
            return walls.Select(wall => TagUtils.GetWallTagPosition(wall.StbNodeIdOrder, wall.StbWallOffsetList, nodes)).ToList();
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

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("fcb8d572-732a-473f-a807-7c2e8bc6f64f");

    }
}
