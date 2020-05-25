using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.STB;
using HoaryFox.Util;
using Rhino.Geometry;

namespace HoaryFox.Component.SectionTag
{
    public class ColumnSecTag:GH_Component
    {
        private string _path;
        private int _size;

        private static StbNodes _nodes;
        private static StbColumns _columns;
        
        private static StbSecColRC _secColumnRc;
        private static StbSecBeamRC _secBeamRc;
        private static StbSecColumnS _secColumnS;
        private static StbSecBeamS _secBeamS;
        private static StbSecBraceS _secBraceS;
        private static StbSecSteel _stbSecSteel;

        private GH_Structure<GH_String> _frameTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();
        
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public ColumnSecTag()
          : base("Column Section Tag", "S2ClSec", "Read ST-Bridge file and display Column Section Tag", "HoaryFox", "Section")
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
            pManager.AddTextParameter("Columns", "Col", "output StbColumns to Section Tag", GH_ParamAccess.tree);
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
            GetTag();

            DA.SetDataTree(0, _frameTags);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_frameTags.DataCount != 0)
            {
                for (int i = 0; i < _frameTags.PathCount; i++)
                {
                    var tags = _frameTags.Branches[i];
                    string tag = tags[0].ToString() + "\n" + tags[1].ToString() + "\n" + tags[2].ToString() + "\n" + 
                                 tags[3].ToString() + "\n" + tags[4].ToString() + "\n" + tags[5].ToString();
                    args.Display.Draw2dText(tag, Color.Black, _tagPos[i], false, _size);
                }
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
        public override Guid ComponentGuid => new Guid("63B2A2E1-A277-4ABA-B522-00D7969871C3");

        private static void Init()
        {
            _nodes = new StbNodes();
            _columns = new StbColumns();
            _secColumnRc = new StbSecColRC();
            _secBeamRc = new StbSecBeamRC();
            _secColumnS = new StbSecColumnS();
            _secBeamS = new StbSecBeamS();
            _secBraceS = new StbSecBraceS();
            _stbSecSteel = new StbSecSteel();
        }

        private static void Load(XDocument xDoc)
        {
            var members = new List<StbData>()
            {
                _nodes, _columns,
                _secColumnRc, _secColumnS, _secBeamRc, _secBeamS, _secBraceS, _stbSecSteel
            };

            foreach (var member in members)
            {
                member.Load(xDoc);
            }
        }

        private void GetTag()
        {
            var tags = new CreateTag(_nodes);
            _frameTags = tags.Frame(_columns, _secColumnRc, _secColumnS, _secBeamRc, _secBeamS, _secBraceS, _stbSecSteel);
            _tagPos = tags.TagPos;
        }
    }
}
