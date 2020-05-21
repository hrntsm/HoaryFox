using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StbHopper.STB;
using StbHopper.Util;

namespace StbHopper.Component.SectionTag
{
    public class Stb2ColSec:GH_Component
    {
        private string _path;
        private int _size;

        private static StbNodes _nodes;
        private static StbColumns _columns;
        private static StbPosts _posts;
        private static StbGirders _girders;
        private static StbBeams _beams;
        private static StbBraces _braces;
        
        public static StbSecColRC SecColumnRc;
        public static StbSecBeamRC SecBeamRc;
        public static StbSecColumnS SecColumnS;
        public static StbSecBeamS SecBeamS;
        public static StbSecBraceS SecBraceS;
        public static StbSecSteel StbSecSteel;

        private readonly List<GH_Structure<GH_String>> _frameTags = new List<GH_Structure<GH_String>>();
        private readonly List<List<Point3d>>_tagPos = new List<List<Point3d>>();
        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Stb2ColSec()
          : base("Stb to SectionTag", "S2Sec", "Read ST-Bridge file and display", "StbHopper", "Section")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _frameTags.Clear();
            _tagPos.Clear();
        }
        
        public override bool IsPreviewCapable => true;

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("path", "path", "input ST-Bridge file path", GH_ParamAccess.item);
            pManager.AddIntegerParameter("size", "size", "Tag size", GH_ParamAccess.item, 12);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Columns", "Col", "output StbColumns to Brep", GH_ParamAccess.tree);
            pManager.AddTextParameter("Girders", "Gird", "output StbGirders to Brep", GH_ParamAccess.tree);
            pManager.AddTextParameter("Posts", "Pst", "output StbPosts to Brep", GH_ParamAccess.tree);
            pManager.AddTextParameter("Beams", "Bm", "output StbBeams to Brep", GH_ParamAccess.tree);
            pManager.AddTextParameter("Braces", "Brc", "output StbBraces to Brep", GH_ParamAccess.tree);
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
            if (!DA.GetData("size", ref _size)) { return; }
            var xDocument = XDocument.Load(_path);

            Init();
            Load(xDocument);

            // meshの生成
            MakeGetTag();

            for (var i = 0; i < 5; i++)
            {
                DA.SetDataTree(i, _frameTags[i]);
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            var column = _frameTags[0];
            var columnPos = _tagPos[0];
            var test = column.PathCount;
            for (int i = 0; i < column.PathCount; i++)
            {
                var tags = column.Branches[i];
                string tag = tags[0].ToString() + "\n" + tags[1].ToString() + "\n" + tags[2].ToString() + "\n" + 
                             tags[3].ToString() + "\n" + tags[4].ToString() + "\n" + tags[5].ToString();
                args.Display.Draw2dText(tag, Color.Black, columnPos[i], true, _size);
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
        public override Guid ComponentGuid => new Guid("328D76C1-2295-48A0-A359-F7B4B556FC4B");

        private static void Init()
        {
            _nodes = new StbNodes();
            _columns = new StbColumns();
            _posts = new StbPosts();
            _girders = new StbGirders();
            _beams = new StbBeams();
            _braces = new StbBraces();
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
                _nodes,
                _columns, _posts, _girders, _beams, _braces,
                SecColumnRc, SecColumnS, SecBeamRc, SecBeamS, SecBraceS, StbSecSteel
            };

            foreach (var member in members)
            {
                member.Load(xDoc);
            }
        }

        private void MakeGetTag()
        {
            var stbFrames = new List<StbFrame>() {
                _columns, _girders, _posts, _beams, _braces
            };
            
            var tags = new CreateTag(_nodes);
            foreach (var frame in stbFrames)
            {
                _frameTags.Add(tags.Frame(frame));
                _tagPos.Add(tags.TagPos);
            }
        }
    }
}
