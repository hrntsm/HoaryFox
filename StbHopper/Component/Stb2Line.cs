using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;
using StbHopper.STB;


namespace StbHopper.Component
{
    public class Stb2Line:GH_Component
    {
        private string _path;

        private static StbNodes _stbNodes;
        private static StbColumns _stbColumns;
        private static StbPosts _stbPosts;
        private static StbGirders _stbGirders;
        private static StbBeams _stbBeams;
        private static StbBraces _stbBraces;

        private readonly List<Point3d> _nodes = new List<Point3d>();
        private readonly List<Line> _columns = new List<Line>();
        private readonly List<Line> _girders = new List<Line>();
        private readonly List<Line> _posts = new List<Line>();
        private readonly List<Line> _beams = new List<Line>();
        private readonly List<Line> _braces = new List<Line>();

        public Stb2Line()
          : base(name: "Stb to Line", nickname: "S2L", description: "Read ST-Bridge file and display", category: "StbHopper", subCategory: "Geometry")
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

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("path", "path", "input ST-Bridge file path", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Nodes", "pt", "output StbNodes to point3d", GH_ParamAccess.list);
            pManager.AddLineParameter("Columns", "Col", "output StbColumns to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Girders", "Gird", "output StbGirders to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Posts", "Pst", "output StbPosts to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Beams", "Beam", "output StbBeams to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Braces", "Brc", "output StbBraces to Line", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 対象の stb の pathを取得
            if (!DA.GetData("path", ref _path)) { return; }
            var xDocument = XDocument.Load(_path);

            Init();
            Load(xDocument);

            // StbNode の取得
            for (var i = 0; i < _stbNodes.Id.Count; i++)
            {
                var position = new Point3d(_stbNodes.X[i], _stbNodes.Y[i], _stbNodes.Z[i]);
                _nodes.Add(position);
            }

            // StbColumns の取得
            for (var i = 0; i < _stbColumns.Id.Count; i++)
            {
                var idNodeStart = _stbNodes.Id.IndexOf(_stbColumns.IdNodeStart[i]);
                var idNodeEnd = _stbNodes.Id.IndexOf(_stbColumns.IdNodeEnd[i]);
                _columns.Add(new Line(_nodes[idNodeStart], _nodes[idNodeEnd]));
            }

            // StbGirder の取得
            for (var i = 0; i < _stbGirders.Id.Count; i++)
            {
                var idNodeStart = _stbNodes.Id.IndexOf(_stbGirders.IdNodeStart[i]);
                var idNodeEnd = _stbNodes.Id.IndexOf(_stbGirders.IdNodeEnd[i]);
                _girders.Add(new Line(_nodes[idNodeStart], _nodes[idNodeEnd]));
            }

            // StbPosts の取得
            for (var i = 0; i < _stbPosts.Id.Count; i++)
            {
                var idNodeStart = _stbNodes.Id.IndexOf(_stbPosts.IdNodeStart[i]);
                var idNodeEnd = _stbNodes.Id.IndexOf(_stbPosts.IdNodeEnd[i]);
                _posts.Add(new Line(_nodes[idNodeStart], _nodes[idNodeEnd]));
            }

            // StbBeam の取得
            for (var i = 0; i < _stbBeams.Id.Count; i++)
            {
                var idNodeStart = _stbNodes.Id.IndexOf(_stbBeams.IdNodeStart[i]);
                var idNodeEnd = _stbNodes.Id.IndexOf(_stbBeams.IdNodeEnd[i]);
                _beams.Add(new Line(_nodes[idNodeStart], _nodes[idNodeEnd]));
            }

            // StbBrace の取得
            for (var i = 0; i < _stbBraces.Id.Count; i++)
            {
                var idNodeStart = _stbNodes.Id.IndexOf(_stbBraces.IdNodeStart[i]);
                var idNodeEnd = _stbNodes.Id.IndexOf(_stbBraces.IdNodeEnd[i]);
                _braces.Add(new Line(_nodes[idNodeStart], _nodes[idNodeEnd]));
            }

            DA.SetDataList(0, _nodes);
            DA.SetDataList(1, _columns);
            DA.SetDataList(2, _girders);
            DA.SetDataList(3, _posts);
            DA.SetDataList(4, _beams);
            DA.SetDataList(5, _braces);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resource.Stb2Line;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7d2f0c4e-4888-4607-8548-592104f6f06d");

        private static void Init()
        {
            _stbNodes = new StbNodes();
            _stbColumns = new StbColumns();
            _stbPosts = new StbPosts();
            _stbGirders = new StbGirders();
            _stbBeams = new StbBeams();
            _stbBraces = new StbBraces();
        }

        private static void Load(XDocument xDocument)
        {
            var members = new List<StbData>()
            {
                _stbNodes, _stbColumns, _stbPosts, _stbGirders, _stbBeams, _stbBraces
            };

            foreach (var member in members)
            {
                member.Load(xDocument);
            }
        }
    }
}