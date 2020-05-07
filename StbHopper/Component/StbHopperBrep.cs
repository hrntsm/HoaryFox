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

        public static StbNodes StbNodes;
        public static StbColumns StbColumns;
        public static StbPosts StbPosts;
        public static StbGirders StbGirders;
        public static StbBeams StbBeams;
        public static StbBraces StbBraces;
        public static StbSlabs StbSlabs;
        public static StbWalls StbWalls;
        public static StbSecColRC StbSecColRc;
        public static StbSecBeamRC StbSecBeamRc;
        public static StbSecColumnS StbSecColumnS;
        public static StbSecBeamS StbSecBeamS;
        public static StbSecBraceS StbSecBraceS;
        public static StbSecSteel StbSecSteel;

        private List<Brep> _slabs = new List<Brep>();
        private List<Brep> _walls = new List<Brep>();
        private List<List<Brep>> _frames = new List<List<Brep>>();

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Stb2Brep()
          : base("StbtoBrep", "S2B", "Read ST-Bridge file and display", "StbHopper", "v1.4")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("path", "path", "input ST-Bridge file path", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Columns", "Col", "output StbColumns to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Girders", "Grdr", "output StbGirders to Brep", GH_ParamAccess.list);
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

            for (int i = 0; i < 5; i++)
                DA.SetDataList(i, _frames[i]);
            DA.SetDataList(5, _slabs);
            DA.SetDataList(6, _walls);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("7d2f0c4e-4888-4607-8548-592104f6f06f");

        private static void Init()
        {
            StbNodes = new StbNodes();
            StbColumns = new StbColumns();
            StbPosts = new StbPosts();
            StbGirders = new StbGirders();
            StbBeams = new StbBeams();
            StbBraces = new StbBraces();
            StbSlabs = new StbSlabs();
            StbWalls = new StbWalls();
        }

        private static void Load(XDocument xDocument)
        {
            var members = new List<StbData>()
            {
                StbNodes, StbColumns, StbPosts, StbGirders, StbBeams, StbBraces
            };

            StbSlabs.Load(xDocument);
            StbWalls.Load(xDocument);

            foreach (var member in members)
            {
                member.Load(xDocument);
            }
        }

        void MakeMesh()
        {
            List<StbFrame> stbFrames = new List<StbFrame>() {
                StbColumns, StbPosts, StbGirders, StbBeams, StbBraces
            };
            
            var breps = new CreateBrep(StbNodes);

            _slabs = breps.Slab(StbSlabs);
            _walls = breps.Wall(StbWalls);

            foreach (var frame in stbFrames)
                _frames.Add(breps.Frame(frame));
        }
    }
}
