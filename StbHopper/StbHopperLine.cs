using System;
using System.Collections.Generic;
using System.Xml.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;


namespace StbHopper {
    public class Stb2Line:GH_Component {

        string _path;
        List<Point3d> _nodes = new List<Point3d>();
        List<Line> _columns = new List<Line>();
        List<Line> _girders = new List<Line>();
        List<Line> _posts = new List<Line>();
        List<Line> _beams = new List<Line>();
        List<Line> _braces = new List<Line>();

        public Stb2Line()
          : base("StbtoLine", "S2L", "Read ST-Bridge file and display", "StbHopper", "v1.4") {
        }

        public override void ClearData() {
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
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager) {
            pManager.AddTextParameter("path", "path", "input ST-Bridge file path", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager) {
            pManager.AddPointParameter("Nodes", "pt", "output StbNodes to point3d", GH_ParamAccess.list);
            pManager.AddLineParameter("Columns", "Col", "output StbColumns to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Girders", "Grdr", "output StbGirders to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Posts", "Pst", "output StbPosts to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Beams", "Bem", "output StbBeams to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Braces", "Brc", "output StbBraces to Line", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            // 対象の stb の pathを取得
            if (!DA.GetData("path", ref _path)) { return; }
            var xdoc = XDocument.Load(_path);

            // StbNode の取得
            var nodeIDs = new List<int>();
            var xNodes = xdoc.Root.Descendants("StbNode");
            foreach (var xNode in xNodes) {
                var position = new Point3d();
                position.X = (double)xNode.Attribute("x");
                position.Y = (double)xNode.Attribute("y");
                position.Z = (double)xNode.Attribute("z");
                var nodeId = (int)xNode.Attribute("id");

                _nodes.Add(position);
                nodeIDs.Add(nodeId);
            }

            int xNodeStart, xNodeEnd;
            int nodeIndexStart, nodeIndexEnd;
            // StbColumns の取得
            var xColumns = xdoc.Root.Descendants("StbColumn");
            foreach (var xColumn in xColumns) {
                xNodeStart = (int)xColumn.Attribute("idNode_bottom");
                xNodeEnd = (int)xColumn.Attribute("idNode_top");

                nodeIndexStart = nodeIDs.IndexOf(xNodeStart);
                nodeIndexEnd = nodeIDs.IndexOf(xNodeEnd);

                _columns.Add(new Line(_nodes[nodeIndexStart], _nodes[nodeIndexEnd]));
            }

            // StbGirder の取得
            var xGirders = xdoc.Root.Descendants("StbGirder");
            foreach (var xGirder in xGirders) {
                xNodeStart = (int)xGirder.Attribute("idNode_start");
                xNodeEnd = (int)xGirder.Attribute("idNode_end");

                nodeIndexStart = nodeIDs.IndexOf(xNodeStart);
                nodeIndexEnd = nodeIDs.IndexOf(xNodeEnd);

                _girders.Add(new Line(_nodes[nodeIndexStart], _nodes[nodeIndexEnd]));
            }

            // StbPosts の取得
            var xPosts = xdoc.Root.Descendants("StbPost");
            foreach (var xPost in xPosts) {
                xNodeStart = (int)xPost.Attribute("idNode_bottom");
                xNodeEnd = (int)xPost.Attribute("idNode_top");

                nodeIndexStart = nodeIDs.IndexOf(xNodeStart);
                nodeIndexEnd = nodeIDs.IndexOf(xNodeEnd);

                _posts.Add(new Line(_nodes[nodeIndexStart], _nodes[nodeIndexEnd]));
            }

            // StbBeam の取得
            var xBeams = xdoc.Root.Descendants("StbBeam");
            foreach (var xBeam in xBeams) {
                xNodeStart = (int)xBeam.Attribute("idNode_start");
                xNodeEnd = (int)xBeam.Attribute("idNode_end");

                nodeIndexStart = nodeIDs.IndexOf(xNodeStart);
                nodeIndexEnd = nodeIDs.IndexOf(xNodeEnd);

                _beams.Add(new Line(_nodes[nodeIndexStart], _nodes[nodeIndexEnd]));
            }

            // StbBrace の取得
            var xBraces = xdoc.Root.Descendants("StbBrace");
            foreach (var xBrace in xBraces) {
                xNodeStart = (int)xBrace.Attribute("idNode_start");
                xNodeEnd = (int)xBrace.Attribute("idNode_end");

                nodeIndexStart = nodeIDs.IndexOf(xNodeStart);
                nodeIndexEnd = nodeIDs.IndexOf(xNodeEnd);

                _braces.Add(new Line(_nodes[nodeIndexStart], _nodes[nodeIndexEnd]));
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
        protected override System.Drawing.Bitmap Icon {
            get {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid {
            get { return new Guid("7d2f0c4e-4888-4607-8548-592104f6f06d"); }
        }
    }
}