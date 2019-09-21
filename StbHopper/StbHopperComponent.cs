using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

using Grasshopper.Kernel;
using Rhino.Geometry;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace StbHopper {
    public class StbHopperLine : GH_Component {

        private string path;
        private int NodeID, xNode_start, xNode_end, Nodeindex_start, Nodeindex_end;
        private double xPos, yPos, zPos;
        private List<Point3d> RhinoNodes = new List<Point3d>();
        private List<int>  RhinoNodeIDs = new List<int>();
        private List<Line> RhinoColumns = new List<Line>();
        private List<Line> RhinoGirders = new List<Line>();
        private List<Line> RhinoPosts = new List<Line>();
        private List<Line> RhinoBeams = new List<Line>();
        private List<Line> RhinoBraces  = new List<Line>();

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public StbHopperLine()
          : base("StbtoLine", "StbtoLine", "Read ST-Bridge file and display", "StbHopper", "StbHopper") {
        }

        public override void ClearData() {
            base.ClearData();
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
            pManager.AddPointParameter("StbNodes", "StbNodes", "output StbNodes to point3d", GH_ParamAccess.list);
            pManager.AddLineParameter("StbColumns", "StbColumns", "output StbColumns to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("StbGirders", "StbGirders", "output StbGirders to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("StbPosts", "StbPosts", "output StbPosts to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("StbBeams", "StbBeams", "output StbBeams to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("StbBraces", "StbBraces", "output StbBraces to Line", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            // 対象の stb の pathを取得
            if (!DA.GetData("path", ref path)) { return; }

            var xdoc = XDocument.Load(path);

            // StbNode の取得
            var xNodes = xdoc.Root.Descendants("StbNode");
            foreach ( var xNode in xNodes) {
                xPos = (double)xNode.Attribute("x");
                yPos = (double)xNode.Attribute("y");
                zPos = (double)xNode.Attribute("z");
                NodeID = (int)xNode.Attribute("id");

                RhinoNodes.Add(new Point3d(xPos, yPos, zPos));
                RhinoNodeIDs.Add(NodeID);
            }

            // StbColumns の取得
            var xColumns = xdoc.Root.Descendants("StbColumn");
            foreach (var xColumn in xColumns) {
                xNode_start = (int)xColumn.Attribute("idNode_bottom");
                xNode_end = (int)xColumn.Attribute("idNode_top");

                Nodeindex_start = RhinoNodeIDs.IndexOf(xNode_start);
                Nodeindex_end = RhinoNodeIDs.IndexOf(xNode_end);

                RhinoColumns.Add(new Line(RhinoNodes[Nodeindex_start], RhinoNodes[Nodeindex_end]));
            }

            // StbGirder の取得
            var xGirders = xdoc.Root.Descendants("StbGirder");
            foreach (var xGirder in xGirders) {
                xNode_start = (int)xGirder.Attribute("idNode_start");
                xNode_end = (int)xGirder.Attribute("idNode_end");

                Nodeindex_start = RhinoNodeIDs.IndexOf(xNode_start);
                Nodeindex_end = RhinoNodeIDs.IndexOf(xNode_end);

                RhinoGirders.Add(new Line(RhinoNodes[Nodeindex_start], RhinoNodes[Nodeindex_end]));
            }

            // StbPosts の取得
            var xPosts = xdoc.Root.Descendants("StbPost");
            foreach (var xPost in xPosts) {
                xNode_start = (int)xPost.Attribute("idNode_bottom");
                xNode_end = (int)xPost.Attribute("idNode_top");

                Nodeindex_start = RhinoNodeIDs.IndexOf(xNode_start);
                Nodeindex_end = RhinoNodeIDs.IndexOf(xNode_end);

                RhinoPosts.Add(new Line(RhinoNodes[Nodeindex_start], RhinoNodes[Nodeindex_end]));
            }

            // StbBeam の取得
            var xBeams = xdoc.Root.Descendants("StbBeam");
            foreach (var xBeam in xBeams) {
                xNode_start = (int)xBeam.Attribute("idNode_start");
                xNode_end = (int)xBeam.Attribute("idNode_end");

                Nodeindex_start = RhinoNodeIDs.IndexOf(xNode_start);
                Nodeindex_end = RhinoNodeIDs.IndexOf(xNode_end);

                RhinoBeams.Add(new Line(RhinoNodes[Nodeindex_start], RhinoNodes[Nodeindex_end]));
            }

            // StbBrace の取得
            var xBraces = xdoc.Root.Descendants("StbBrace");
            foreach (var xBrace in xBraces) {
                xNode_start = (int)xBrace.Attribute("idNode_start");
                xNode_end = (int)xBrace.Attribute("idNode_end");

                Nodeindex_start = RhinoNodeIDs.IndexOf(xNode_start);
                Nodeindex_end = RhinoNodeIDs.IndexOf(xNode_end);

                RhinoBraces.Add(new Line(RhinoNodes[Nodeindex_start], RhinoNodes[Nodeindex_end]));
            }

            DA.SetDataList(0, RhinoNodes);
            DA.SetDataList(1, RhinoColumns);
            DA.SetDataList(2, RhinoGirders);
            DA.SetDataList(3, RhinoPosts);
            DA.SetDataList(4, RhinoBeams);
            DA.SetDataList(5, RhinoBraces);
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

    public class StbHopperBrep : GH_Component {

        private string path;
        private int NodeID,  Nodeindex_i, Nodeindex_j, Nodeindex_k, Nodeindex_l;
        private double xPos, yPos, zPos;
        private List<Point3d> RhinoNodes = new List<Point3d>();
        private List<int> RhinoNodeIDs = new List<int>();
        private List<int> xSlabNodeIDs = new List<int>();
        private List<Brep> RhinoSlabs = new List<Brep>();

        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public StbHopperBrep()
          : base("StbToBrep", "StbToBrep", "Read ST-Bridge file and display", "StbHopper", "StbHopper") {
        }

        public override void ClearData() {
            base.ClearData();
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
            pManager.AddBrepParameter("StbSlabs", "StbSlabs", "output StbSlabs to Brep", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA) {
            // 対象の stb の pathを取得
            if (!DA.GetData("path", ref path)) { return; }

            var xdoc = XDocument.Load(path);

            // StbNode の取得
            var xNodes = xdoc.Root.Descendants("StbNode");
            foreach (var xNode in xNodes) {
                xPos = (double)xNode.Attribute("x");
                yPos = (double)xNode.Attribute("y");
                zPos = (double)xNode.Attribute("z");
                NodeID = (int)xNode.Attribute("id");

                RhinoNodes.Add(new Point3d(xPos, yPos, zPos));
                RhinoNodeIDs.Add(NodeID);
            }

            // StbColumns の取得
            var xSlabs = xdoc.Root.Descendants("StbSlab");
            foreach (var xSlab in xSlabs) {
                var xNodeids = xSlab.Element("StbNodeid_List")
                                    .Elements("StbNodeid");
                int CountNode = 0;
                foreach (var xNodeid in xNodeids) {
                    xSlabNodeIDs.Add((int)xNodeid.Attribute("id"));

                    CountNode = CountNode + 1;
                }

                Brep SlabBrep = new Brep();
                Nodeindex_i = RhinoNodeIDs.IndexOf(xSlabNodeIDs[0]);
                Nodeindex_j = RhinoNodeIDs.IndexOf(xSlabNodeIDs[1]);
                Nodeindex_k = RhinoNodeIDs.IndexOf(xSlabNodeIDs[2]);

                if (CountNode == 4) {
                    Nodeindex_l = RhinoNodeIDs.IndexOf(xSlabNodeIDs[3]);
                    SlabBrep = Brep.CreateFromCornerPoints(RhinoNodes[Nodeindex_i], RhinoNodes[Nodeindex_j], RhinoNodes[Nodeindex_k], RhinoNodes[Nodeindex_l], GH_Component.DocumentTolerance());
                }
                else {
                    SlabBrep = Brep.CreateFromCornerPoints(RhinoNodes[Nodeindex_i], RhinoNodes[Nodeindex_j], RhinoNodes[Nodeindex_k], GH_Component.DocumentTolerance());
                }

                xSlabNodeIDs.Clear(); // foreachごとでListにAddし続けてるのでここで値をClear
                xSlabNodeIDs.Clear(); // foreachごとでListにAddし続けてるのでここで値をClear
                RhinoSlabs.Add(SlabBrep);
            }

            DA.SetDataList(0, RhinoSlabs);
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
            get { return new Guid("7d2f0c4e-4888-4607-8548-592104f6f06f"); }
        }
    }
}
