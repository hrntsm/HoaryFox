using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

using StbHopper.STB;
using StbHopper.Util;

namespace StbHopper.Component
{
    public class Stb2Brep:GH_Component
    {
        private string _path;
        public static readonly double LengthTolerance = GH_Component.DocumentTolerance();
        public static readonly double AngleTolerance = GH_Component.DocumentAngleTolerance();

        private static StbNodes _nodes;
        private static StbColumns _columns;
        private static StbPosts _posts;
        private static StbGirders _girders;
        private static StbBeams _beams;
        private static StbBraces _braces;
        private static StbSlabs _slabs;
        private static StbWalls _walls;
        
        public static StbSecColRC SecColumnRc;
        public static StbSecBeamRC SecBeamRc;
        public static StbSecColumnS SecColumnS;
        public static StbSecBeamS SecBeamS;
        public static StbSecBraceS SecBraceS;
        public static StbSecSteel StbSecSteel;

        private List<Brep> _slabBreps = new List<Brep>();
        private List<Brep> _wallBreps = new List<Brep>();
        private readonly List<List<Brep>> _frameBreps = new List<List<Brep>>();

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Stb2Brep()
          : base("Stb to Brep", "S2B", "Read ST-Bridge file and display", "StbHopper", "v1.4")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _slabBreps.Clear();
            _wallBreps.Clear();
            _frameBreps.Clear();
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
            pManager.AddBrepParameter("Columns", "Col", "output StbColumns to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Girders", "Gird", "output StbGirders to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Posts", "Pst", "output StbPosts to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Beams", "Bm", "output StbBeams to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Braces", "Brc", "output StbBraces to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Slabs", "Slb", "output StbSlabs to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Walls", "Wl", "output StbWalls to Brep", GH_ParamAccess.list);
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

            // meshの生成
            MakeMesh();

            for (var i = 0; i < 5; i++)
            {
                DA.SetDataList(i, _frameBreps[i]);
            }
            DA.SetDataList(5, _slabBreps);
            DA.SetDataList(6, _wallBreps);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resource.Stb2Brep;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7d2f0c4e-4888-4607-8548-592104f6f06f");

        private static void Init()
        {
            _nodes = new StbNodes();
            _columns = new StbColumns();
            _posts = new StbPosts();
            _girders = new StbGirders();
            _beams = new StbBeams();
            _braces = new StbBraces();
            _slabs = new StbSlabs();
            _walls = new StbWalls();
            SecColumnRc = new StbSecColRC();
            SecBeamRc = new StbSecBeamRC();
            SecColumnS = new StbSecColumnS();
            SecBeamS = new StbSecBeamS();
            SecBraceS = new StbSecBraceS();
            StbSecSteel = new StbSecSteel();
        }

        private static void Load(XDocument xDoc)
        {
            var members = new List<StbData>()
            {
                _nodes, _slabs, _walls,
                _columns, _posts, _girders, _beams, _braces,
                SecColumnRc, SecColumnS, SecBeamRc, SecBeamS, SecBraceS, StbSecSteel
            };

            foreach (var member in members)
            {
                member.Load(xDoc);
            }
        }

        private void MakeMesh()
        {
            var stbFrames = new List<StbFrame>() {
                _columns, _girders, _posts, _beams, _braces
            };
            
            var breps = new CreateBrep(_nodes);

            _slabBreps = breps.Slab(_slabs);
            _wallBreps = breps.Wall(_walls);

            foreach (var frame in stbFrames)
                _frameBreps.Add(breps.Frame(frame));
        }
    }
}
