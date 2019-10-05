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
            RhinoNodes.Clear();
            RhinoNodeIDs.Clear();
            RhinoColumns.Clear();
            RhinoGirders.Clear();
            RhinoPosts.Clear();
            RhinoBeams.Clear();
            RhinoBraces.Clear();
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

        private string path, Element_shape, xElement_kind, ElementShapeType;
        private int NodeID, Nodeindex_i, Nodeindex_j, Nodeindex_k, Nodeindex_l, Nodeindex_start, Nodeindex_end,
                    xNode_start, xNode_end, xElement_id_section,
                    StbSecIndex,  Element_id_section, ElementHight, ElementWidth;
        private double xPos, yPos, zPos, ElementAngleY, ElementAngleZ;
        private Point3d NodeStart, NodeEnd, 
                        VertexS1, VertexS2, VertexS3, VertexS4, VertexS5, VertexS6,
                        VertexE1, VertexE2, VertexE3, VertexE4, VertexE5, VertexE6;
        private List<Point3d> RhinoNodes = new List<Point3d>();
        private List<int> RhinoNodeIDs = new List<int>();
        private List<int> xSlabNodeIDs = new List<int>();
        private List<int> xSecRcColumn_id = new List<int>();
        private List<int> xSecRcColumn_Depth = new List<int>();
        private List<int> xSecRcColumn_Width = new List<int>();
        private List<int> xSecSColumn_id = new List<int>();
        private List<string> xSecSColumn_shape = new List<string>();
        private List<int> xSecRcBeam_id = new List<int>();
        private List<int> xSecRcBeam_Depth = new List<int>();
        private List<int> xSecRcBeam_Width = new List<int>();
        private List<int> xSecSBeam_id = new List<int>();
        private List<string> xSecSBeam_shape = new List<string>();
        private List<int> xSecSBrace_id = new List<int>();
        private List<string> xSecSBrace_shape = new List<string>();
        private List<string> xStbSecSteel_name = new List<string>();
        private List<int> xStbSecSteel_A = new List<int>();
        private List<int> xStbSecSteel_B = new List<int>();
        private List<string> xStbSecSteel_type = new List<string>();
        private List<Brep> RhinoSlabs = new List<Brep>();
        private List<Brep> ElementShapeBrep = new List<Brep>();
        private Brep[] RhinoColumns, RhinoGirders, RhinoPosts, RhinoBeams, S_Braces, ELementShapeBrepArray;

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
            RhinoNodes.Clear();
            RhinoNodeIDs.Clear();
            RhinoSlabs.Clear();
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
            pManager.AddBrepParameter("StbColumns", "StbColumns", "output StbColumns to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("StbGirders", "StbGirders", "output StbGirders to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("StbPosts", "StbPosts", "output StbPosts to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("StbBeams", "StbBeams", "output StbBeams to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("StbSlabs", "StbSlabs", "output StbSlabs to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("StbBraces", "StbBraces", "output StbBraces to Brep", GH_ParamAccess.list);
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

            // StbSlabs の取得
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

                // StbSlabの定義上4節点以外許容されているのか不明だが場合分けしてる
                if (CountNode == 4) {
                    Nodeindex_l = RhinoNodeIDs.IndexOf(xSlabNodeIDs[3]);
                    SlabBrep = Brep.CreateFromCornerPoints(RhinoNodes[Nodeindex_i], RhinoNodes[Nodeindex_j], RhinoNodes[Nodeindex_k], RhinoNodes[Nodeindex_l], GH_Component.DocumentTolerance());
                }
                else {
                    SlabBrep = Brep.CreateFromCornerPoints(RhinoNodes[Nodeindex_i], RhinoNodes[Nodeindex_j], RhinoNodes[Nodeindex_k], GH_Component.DocumentTolerance());
                }

                xSlabNodeIDs.Clear(); // foreachごとでListにAddし続けてるのでここで値をClear
                RhinoSlabs.Add(SlabBrep);
            }

            // StbSecColumn_RC の取得
            var xSecRcColumns = xdoc.Root.Descendants("StbSecColumn_RC");
            foreach (var xSecRcColumn in xSecRcColumns) {
                xSecRcColumn_id.Add((int)xSecRcColumn.Attribute("id"));
                var xSecFigure = xSecRcColumn.Element("StbSecFigure");

                // 子要素が StbSecRect か StbSecCircle を判定
                if (xSecFigure.Element("StbSecRect") != null) {
                    xSecRcColumn_Depth.Add((int)xSecFigure.Element("StbSecRect").Attribute("DY"));
                    xSecRcColumn_Width.Add((int)xSecFigure.Element("StbSecRect").Attribute("DX"));
                }
                else {
                    xSecRcColumn_Depth.Add((int)xSecFigure.Element("StbSecCircle").Attribute("D"));
                    xSecRcColumn_Width.Add( 0 ); // Circle と判定用に width は 0
                }
            }

            // StbSecColumn_S の取得
            var xSecSColumns = xdoc.Root.Descendants("StbSecColumn_S");
            foreach (var xSecSColumn in xSecSColumns) {
                xSecSColumn_id.Add((int)xSecSColumn.Attribute("id"));
                xSecSColumn_shape.Add((string)xSecSColumn.Element("StbSecSteelColumn").Attribute("shape"));
            }

            // StbSecBeam_RC の取得
            var xSecRcBeams = xdoc.Root.Descendants("StbSecBeam_RC");
            foreach (var xSecRcBeam in xSecRcBeams) {
                xSecRcBeam_id.Add((int)xSecRcBeam.Attribute("id"));
                var xSecFigure = xSecRcBeam.Element("StbSecFigure");
                
                // 子要素が StbSecHaunch か StbSecStraight を判定
                if (xSecFigure.Element("StbSecHaunch") != null) {
                    xSecRcBeam_Depth.Add((int)xSecFigure.Element("StbSecHaunch").Attribute("depth_center"));
                    xSecRcBeam_Width.Add((int)xSecFigure.Element("StbSecHaunch").Attribute("width_center"));
                } 
                else {
                    xSecRcBeam_Depth.Add((int)xSecFigure.Element("StbSecStraight").Attribute("depth"));
                    xSecRcBeam_Width.Add((int)xSecFigure.Element("StbSecStraight").Attribute("width"));
                }
            }

            // StbSecBeam_S の取得
            var xSecSBeams = xdoc.Root.Descendants("StbSecBeam_S");
            foreach (var xSecSBeam in xSecSBeams) {
                xSecSBeam_id.Add((int)xSecSBeam.Attribute("id"));
                xSecSBeam_shape.Add((string)xSecSBeam.Element("StbSecSteelBeam").Attribute("shape"));
            }

            // StbSecBrace_S の取得
            var xSecSBraces = xdoc.Root.Descendants("StbSecBrace_S");
            foreach (var xSecSBrace in xSecSBraces) {
                xSecSBrace_id.Add((int)xSecSBrace.Attribute("id"));
                xSecSBrace_shape.Add((string)xSecSBrace.Element("StbSecSteelBrace").Attribute("shape"));
            }

            // S断面形状の取得
            GetStbSteelSection(xdoc, "StbSecRoll-H", "H");
            GetStbSteelSection(xdoc, "StbSecBuild-H", "H");
            GetStbSteelSection(xdoc, "StbSecRoll-BOX", "BOX");
            GetStbSteelSection(xdoc, "StbSecBuild-BOX", "BOX");
            GetStbSteelSection(xdoc, "StbSecPipe", "Pipe");
            GetStbSteelSection(xdoc, "StbSecRoll-L", "L");


            // 柱の断面の生成
            RhinoColumns = MakeElementBrep(xdoc, "StbColumn", "Column");
            // 大梁の断面の生成
            RhinoGirders = MakeElementBrep(xdoc, "StbGirder", "Beam");
            // 間柱の断面の生成
            RhinoPosts = MakeElementBrep(xdoc, "StbPost", "Column");
            // 小梁の断面の生成
            RhinoBeams = MakeElementBrep(xdoc, "StbBeam", "Beam");
            // Sブレ―スの断面の生成
            S_Braces = MakeElementBrep(xdoc, "StbBrace", "Brace");

            DA.SetDataList(0, RhinoColumns);
            DA.SetDataList(1, RhinoGirders);
            DA.SetDataList(2, RhinoPosts);
            DA.SetDataList(3, RhinoBeams);
            DA.SetDataList(4, RhinoSlabs);
            DA.SetDataList(5, S_Braces);
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

        /// <summary>
        /// Get ST-Bridge Steel Section
        /// </summary>
        /// <param name="xdoc">XML data</param>
        /// <param name="xDateTag">XML data tag you want to read</param>
        /// <param name="SectionType">Section type, H or BOX or Pipe or L</param>
        public void GetStbSteelSection(XDocument xdoc, string xDateTag, string SectionType) {
            if(SectionType == "Pipe") {
                var xSteelSections = xdoc.Root.Descendants(xDateTag);
                foreach (var xSteelSection in xSteelSections) {
                    xStbSecSteel_name.Add((string)xSteelSection.Attribute("name"));
                    xStbSecSteel_A.Add((int)xSteelSection.Attribute("D"));
                    xStbSecSteel_B.Add((int)xSteelSection.Attribute("t"));
                    xStbSecSteel_type.Add(SectionType);
                }
            }
            else {
                var xSteelSections = xdoc.Root.Descendants(xDateTag);
                foreach (var xSteelSection in xSteelSections) {
                    xStbSecSteel_name.Add((string)xSteelSection.Attribute("name"));
                    xStbSecSteel_A.Add((int)xSteelSection.Attribute("A"));
                    xStbSecSteel_B.Add((int)xSteelSection.Attribute("B"));
                    xStbSecSteel_type.Add(SectionType);
                }
            }
        }

        /// <summary>
        /// Make Element to Brep
        /// </summary>
        /// <param name="xdoc">XML data</param>
        /// <param name="xDateTag">XML data tag you want to read</param>
        /// <param name="ElementStructureType">Element structure type, Column or Beam or Brace</param>
        /// <returns></returns>
        public Brep[] MakeElementBrep(XDocument xdoc, string xDateTag, string ElementStructureType) {
            ElementShapeBrep.Clear();

            var xElements = xdoc.Root.Descendants(xDateTag);
            foreach (var xElement in xElements) {
                if (ElementStructureType == "Beam" || ElementStructureType == "Brace") {
                    xNode_start = (int)xElement.Attribute("idNode_start");
                    xNode_end = (int)xElement.Attribute("idNode_end");
                }
                else {
                    xNode_start = (int)xElement.Attribute("idNode_bottom");
                    xNode_end = (int)xElement.Attribute("idNode_top");
                }
                xElement_id_section = (int)xElement.Attribute("id_section");
                xElement_kind = (string)xElement.Attribute("kind_structure");

                // 始点と終点の座標取得
                Nodeindex_start = RhinoNodeIDs.IndexOf(xNode_start);
                Nodeindex_end = RhinoNodeIDs.IndexOf(xNode_end);
                NodeStart = RhinoNodes[Nodeindex_start];
                NodeEnd = RhinoNodes[Nodeindex_end];

                if (xElement_kind == "RC") {
                    // 断面形状名（shape) と 断面形状（HxB）の取得の取得
                    if (ElementStructureType == "Beam") {
                        StbSecIndex = xSecRcBeam_id.IndexOf(xElement_id_section);
                        ElementHight = xSecRcBeam_Depth[StbSecIndex];
                        ElementWidth = xSecRcBeam_Width[StbSecIndex];
                    }
                    else if (ElementStructureType == "Column") {
                        StbSecIndex = xSecRcColumn_id.IndexOf(xElement_id_section);
                        ElementHight = xSecRcColumn_Depth[StbSecIndex];
                        ElementWidth = xSecRcColumn_Width[StbSecIndex];
                    }

                    if (ElementWidth == 0) {
                        ElementShapeType = "Pipe";
                    }
                    else {
                        ElementShapeType = "BOX";
                    }
                }
                else if (xElement_kind == "S") {
                    // 断面形状名（shapeの取得の取得
                    if (ElementStructureType == "Beam") {
                        Element_id_section = xSecSBeam_id.IndexOf(xElement_id_section);
                        Element_shape = xSecSBeam_shape[Element_id_section];
                    }
                    else if (ElementStructureType == "Column") {
                        Element_id_section = xSecSColumn_id.IndexOf(xElement_id_section);
                        Element_shape = xSecSColumn_shape[Element_id_section];
                    }
                    else if (ElementStructureType == "Brace") {
                        Element_id_section = xSecSBrace_id.IndexOf(xElement_id_section);
                        Element_shape = xSecSBrace_shape[Element_id_section];
                    }
                    // 断面形状（HxB）の取得の取得
                    StbSecIndex = xStbSecSteel_name.IndexOf(Element_shape);
                    ElementHight = xStbSecSteel_A[StbSecIndex];
                    ElementWidth = xStbSecSteel_B[StbSecIndex];
                    ElementShapeType = xStbSecSteel_type[StbSecIndex];
                }

                // 始点と終点から梁断面サーフェスの作成
                ElementShapeBrep = MakeElementsBrepFromVertex(NodeStart, NodeEnd, ElementHight, ElementWidth, ElementShapeType, ElementStructureType);
            }
            ELementShapeBrepArray = Brep.JoinBreps(ElementShapeBrep, GH_Component.DocumentTolerance());
            return ELementShapeBrepArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="NodeStart"></param>
        /// <param name="NodeEnd"></param>
        /// <param name="ElementHight"></param>
        /// <param name="ElementWidth"></param>
        /// <param name="ElementShapeType"></param>
        /// <param name="ElementStructureType"></param>
        /// <returns></returns>
        public List<Brep> MakeElementsBrepFromVertex(Point3d NodeStart, Point3d NodeEnd, int ElementHight, int ElementWidth, string ElementShapeType, string ElementStructureType) {

            // 部材のアングルの確認
            ElementAngleY = -1.0 * Math.Atan((NodeEnd.Y - NodeStart.Y) / (NodeEnd.X - NodeStart.X));
            ElementAngleZ = -1.0 * Math.Atan((NodeEnd.Z - NodeStart.Z) / (NodeEnd.X - NodeStart.X));

            // 描画用点の作成
            // 梁は部材天端の中心が起点に対して、柱・ブレースは部材芯が起点なので場合分け
            // NodeStart側   
            //  Y        S4 - S5 - S6 
            //  ^        |    |    |  
            //  o >  X   S1 - S2 - S3
            if (ElementStructureType == "Beam") {
                VertexS1 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z - ElementHight
                                       );
                VertexS2 = new Point3d(NodeStart.X,
                                       NodeStart.Y,
                                       NodeStart.Z - ElementHight
                                       );
                VertexS3 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z - ElementHight
                                       );
                VertexS4 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z
                                       );
                VertexS5 = NodeStart;
                VertexS6 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z
                                       );
            }
            else if (ElementStructureType == "Column") {
                VertexS1 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeStart.Y - (ElementHight / 2.0),
                                       NodeStart.Z - (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
                VertexS2 = new Point3d(NodeStart.X,
                                       NodeStart.Y + (ElementHight / 2.0),
                                       NodeStart.Z
                                       );
                VertexS3 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeStart.Y - (ElementHight / 2.0),
                                       NodeStart.Z + (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
                VertexS4 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeStart.Y + (ElementHight / 2.0),
                                       NodeStart.Z - (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
                VertexS5 = new Point3d(NodeStart.X,
                                       NodeStart.Y - (ElementHight / 2.0),
                                       NodeStart.Z
                                       );
                VertexS6 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeStart.Y + (ElementHight / 2.0),
                                       NodeStart.Z + (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
            }
            else if (ElementStructureType == "Brace") {
                VertexS1 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z - (ElementWidth / 2.0)
                                       );
                VertexS2 = new Point3d(NodeStart.X,
                                       NodeStart.Y,
                                       NodeStart.Z - (ElementWidth / 2.0)
                                       );
                VertexS3 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z - (ElementWidth / 2.0)
                                       );
                VertexS4 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z + (ElementWidth / 2.0)
                                       );
                VertexS5 = new Point3d(NodeStart.X,
                                       NodeStart.Y,
                                       NodeStart.Z + (ElementWidth / 2.0)
                                       );
                VertexS6 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeStart.Z + (ElementWidth / 2.0)
                                       );
            }
            // NodeEnd側
            //  Y        E4 - E5 - E6
            //  ^        |    |    |
            //  o >  X   E1 - E2 - E3
            if (ElementStructureType == "Beam") {
                VertexE1 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z - ElementHight
                                       );
                VertexE2 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y,
                                       NodeEnd.Z - ElementHight
                                       );
                VertexE3 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z - ElementHight
                                       );
                VertexE4 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z
                                       );
                VertexE5 = NodeEnd;
                VertexE6 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z
                                       );
            }
            else if (ElementStructureType == "Column") {
                VertexE1 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeEnd.Y - (ElementHight / 2.0),
                                       NodeEnd.Z - (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
                VertexE2 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y + (ElementHight / 2.0),
                                       NodeEnd.Z
                                       );
                VertexE3 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeEnd.Y - (ElementHight / 2.0),
                                       NodeEnd.Z + (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
                VertexE4 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeEnd.Y + (ElementHight / 2.0),
                                       NodeEnd.Z - (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
                VertexE5 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y - (ElementHight / 2.0),
                                       NodeEnd.Z
                                       );
                VertexE6 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleZ),
                                       NodeEnd.Y + (ElementHight / 2.0),
                                       NodeEnd.Z + (ElementWidth / 2.0) * Math.Cos(ElementAngleZ)
                                       );
            }
            else if (ElementStructureType == "Brace") {
                VertexE1 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z - (ElementWidth / 2.0)
                                       );
                VertexE2 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y,
                                       NodeEnd.Z - (ElementWidth / 2.0)
                                       );
                VertexE3 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z - (ElementWidth / 2.0)
                                       );
                VertexE4 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z + (ElementWidth / 2.0)
                                       );
                VertexE5 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y,
                                       NodeEnd.Z + (ElementWidth / 2.0)
                                       );
                VertexE6 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(ElementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(ElementAngleY),
                                       NodeEnd.Z + (ElementWidth / 2.0)
                                       );
            }

            if (this.ElementShapeType == "H") {
                // make upper flange
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS4, VertexS6, VertexE6, VertexE4, GH_Component.DocumentTolerance()));
                // make bottom flange
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS1, VertexS3, VertexE3, VertexE1, GH_Component.DocumentTolerance()));
                // make web 
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS5, VertexS2, VertexE2, VertexE5, GH_Component.DocumentTolerance()));
            }
            else if (this.ElementShapeType == "BOX") {
                // make upper flange
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS4, VertexS6, VertexE6, VertexE4, GH_Component.DocumentTolerance()));
                // make bottom flange
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS1, VertexS3, VertexE3, VertexE1, GH_Component.DocumentTolerance()));
                // make web 1
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS4, VertexS1, VertexE1, VertexE4, GH_Component.DocumentTolerance()));
                // make web 2
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS6, VertexS3, VertexE3, VertexE6, GH_Component.DocumentTolerance()));
            }
            else if (this.ElementShapeType == "Pipe") {
                LineCurve PipeCurve = new LineCurve(NodeStart, NodeEnd);
                ElementShapeBrep.Add(Brep.CreatePipe(PipeCurve, ElementHight / 2.0, false, 0, false, GH_Component.DocumentTolerance(), GH_Component.DocumentAngleTolerance())[0]);
            }
            else if (this.ElementShapeType == "L") {
                // make bottom flange
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS1, VertexS3, VertexE3, VertexE1, GH_Component.DocumentTolerance()));
                // make web
                ElementShapeBrep.Add(Brep.CreateFromCornerPoints(VertexS6, VertexS3, VertexE3, VertexE6, GH_Component.DocumentTolerance()));
            }
            else {
            }
            return ElementShapeBrep;
        }
    }
}
