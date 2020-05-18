using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

using StbHopper.STB;
using StbHopper.Util;

namespace StbHopper.Component
{
    public class Stb2Bar:GH_Component
    {
        private string _path;
        public static readonly double LengthTolerance = GH_Component.DocumentTolerance();
        public static readonly double AngleTolerance = GH_Component.DocumentAngleTolerance();

        private static StbNodes _nodes;
        private static StbColumns _columns;
        private static StbPosts _posts;
        private static StbGirders _girders;
        private static StbBeams _beams;
        
        public static StbSecColRC SecColumnRc;
        public static StbSecBeamRC SecBeamRc;

        private readonly List<List<Brep>> _frameBreps = new List<List<Brep>>();

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Stb2Bar()
          : base("Stb to Bar", "S2B", "Read ST-Bridge file and display", "StbHopper", "Bar")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
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
            pManager.AddBrepParameter("Columns", "Col", "output StbColumns to Bar", GH_ParamAccess.list);
            pManager.AddBrepParameter("Girders", "Gird", "output StbGirders to Bar", GH_ParamAccess.list);
            pManager.AddBrepParameter("Posts", "Pst", "output StbPosts to Bar", GH_ParamAccess.list);
            pManager.AddBrepParameter("Beams", "Bm", "output StbBeams to Bar", GH_ParamAccess.list);
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
            MakeBar();

            for (var i = 0; i < 4; i++)
            {
                DA.SetDataList(i, _frameBreps[i]);
            }
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
        public override Guid ComponentGuid => new Guid("8C6A2977-EC3A-43D0-90B2-0B10EF2E124B");

        private static void Init()
        {
            _nodes = new StbNodes();
            _columns = new StbColumns();
            _posts = new StbPosts();
            _girders = new StbGirders();
            _beams = new StbBeams();
            SecColumnRc = new StbSecColRC();
            SecBeamRc = new StbSecBeamRC();
        }

        private static void Load(XDocument xDoc)
        {
            var members = new List<StbData>()
            {
                _nodes,
                _columns, _posts, _girders, _beams,
                SecColumnRc, SecBeamRc
            };

            foreach (var member in members)
            {
                member.Load(xDoc);
            }
        }

        private void MakeBar()
        {
            var stbFrames = new List<StbFrame>() {
                _columns, _girders, _posts, _beams
            };
            
            var breps = new CreateBar(_nodes);

            foreach (var frame in stbFrames)
                _frameBreps.Add(breps.Frame(frame));
        }
    }
}

