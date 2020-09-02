using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Grasshopper.Kernel;
using HoaryFox.Member;
using HoaryFox.STB;
using Rhino.Geometry;


namespace HoaryFox.Component.Geometry
{
    public class Stb2Line:GH_Component
    {
        private StbData _stbData;
        private List<Point3d> _nodes = new List<Point3d>();
        private List<Line> _columns = new List<Line>();
        private List<Line> _girders = new List<Line>();
        private List<Line> _posts = new List<Line>();
        private List<Line> _beams = new List<Line>();
        private List<Line> _braces = new List<Line>();

        public Stb2Line()
          : base(name: "Stb to Line", nickname: "S2L", description: "Read ST-Bridge file and display", category: "HoaryFox", subCategory: "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _nodes.Clear();
            _columns.Clear();
            _girders.Clear();
            _posts.Clear();
            _beams.Clear();
            _braces.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Nodes", "pt", "output StbNodes to point3d", GH_ParamAccess.list);
            pManager.AddLineParameter("Columns", "Col", "output StbColumns to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Girders", "Gird", "output StbGirders to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Posts", "Pst", "output StbPosts to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Beams", "Beam", "output StbBeams to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Braces", "Brc", "output StbBraces to Line", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }

            MakeLine();

            DA.SetDataList(0, _nodes);
            DA.SetDataList(1, _columns);
            DA.SetDataList(2, _girders);
            DA.SetDataList(3, _posts);
            DA.SetDataList(4, _beams);
            DA.SetDataList(5, _braces);
        }

        private void MakeLine()
        {
            var createMemberLines = new CreateLines(_stbData);
            _nodes = createMemberLines.Nodes();
            _columns = createMemberLines.Columns();
            _girders = createMemberLines.Girders();
            _posts = createMemberLines.Posts();
            _beams = createMemberLines.Beams();
            _braces = createMemberLines.Braces();
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.Line;
        public override Guid ComponentGuid => new Guid("7d2f0c4e-4888-4607-8548-592104f6f06d");
    }
}