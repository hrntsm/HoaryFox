using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;

namespace StbHopper.Component
{
    public class Stb2Brep:GH_Component
    {

        private string _path, _elementShape, _xElementKind, _elementShapeType;
        private int _nodeIndexStart, _nodeIndexEnd,
                    _xNodeStart, _xNodeEnd, _xElementIdSection,
                    _stbSecIndex, _elementIdSection;
        private double _elementAngleY, _elementAngleZ, _elementHight, _elementWidth;
        private Point3d _nodeStart, _nodeEnd,
                        _vertexS1, _vertexS2, _vertexS3, _vertexS4, _vertexS5, _vertexS6,
                        _vertexE1, _vertexE2, _vertexE3, _vertexE4, _vertexE5, _vertexE6;
        private List<Point3d> _nodes = new List<Point3d>();
        private List<int> _nodeIDs = new List<int>();
        private List<int> _xSlabNodeIDs = new List<int>();
        private List<int> _xSecRcColumnId = new List<int>();
        private List<int> _xSecRcColumnDepth = new List<int>();
        private List<int> _xSecRcColumnWidth = new List<int>();
        private List<int> _xSecSColumnId = new List<int>();
        private List<string> _xSecSColumnShape = new List<string>();
        private List<int> _xSecRcBeamId = new List<int>();
        private List<int> _xSecRcBeamDepth = new List<int>();
        private List<int> _xSecRcBeamWidth = new List<int>();
        private List<int> _xSecSBeamId = new List<int>();
        private List<string> _xSecSBeamShape = new List<string>();
        private List<int> _xSecSBraceId = new List<int>();
        private List<string> _xSecSBraceShape = new List<string>();
        private List<string> _xStbSecSteelName = new List<string>();
        private List<double> _xStbSecSteelParamA = new List<double>();
        private List<double> _xStbSecSteelParamB = new List<double>();
        private List<string> _xStbSecSteelType = new List<string>();
        private List<Brep> _slabs = new List<Brep>();
        private List<Brep> _elementShapeBrep = new List<Brep>();
        private Brep[] _columns, _girders, _posts, _beams, _braces, _elementShapeBrepArray;

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
            _nodes.Clear();
            _nodeIDs.Clear();
            _slabs.Clear();
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
            var xDoc = XDocument.Load(_path);

            // StbNode の取得
            var xNodes = xDoc.Root.Descendants("StbNode");
            foreach (var xNode in xNodes)
            {
                var position = new Point3d();
                position.X = (double)xNode.Attribute("x");
                position.Y = (double)xNode.Attribute("y");
                position.Z = (double)xNode.Attribute("z");
                int nodeId = (int)xNode.Attribute("id");

                _nodes.Add(position);
                _nodeIDs.Add(nodeId);
            }

            // StbSlabs の取得
            var nodeIndex = new int[4];
            var xSlabs = xDoc.Root.Descendants("StbSlab");
            foreach (var xSlab in xSlabs)
            {
                var xNodeids = xSlab.Element("StbNodeid_List").Elements("StbNodeid");
                int countNode = 0;
                foreach (var xNodeid in xNodeids)
                {
                    _xSlabNodeIDs.Add((int)xNodeid.Attribute("id"));
                    countNode++;
                }

                Brep SlabBrep = new Brep();
                for (int i = 0; i < 3; i++)
                {
                    nodeIndex[i] = _nodeIDs.IndexOf(_xSlabNodeIDs[i]);
                }
                if (countNode == 4)
                {
                    nodeIndex[3] = _nodeIDs.IndexOf(_xSlabNodeIDs[3]);
                    SlabBrep = Brep.CreateFromCornerPoints(_nodes[nodeIndex[0]], _nodes[nodeIndex[1]], _nodes[nodeIndex[2]], _nodes[nodeIndex[3]], GH_Component.DocumentTolerance());
                }
                else
                {
                    SlabBrep = Brep.CreateFromCornerPoints(_nodes[nodeIndex[0]], _nodes[nodeIndex[1]], _nodes[nodeIndex[2]], GH_Component.DocumentTolerance());
                }
                _slabs.Add(SlabBrep);
                _xSlabNodeIDs.Clear(); // foreachごとでListにAddし続けてるのでここで値をClear
            }

            // StbSecColumn_RC の取得
            var xSecRcColumns = xDoc.Root.Descendants("StbSecColumn_RC");
            foreach (var xSecRcColumn in xSecRcColumns)
            {
                _xSecRcColumnId.Add((int)xSecRcColumn.Attribute("id"));
                var xSecFigure = xSecRcColumn.Element("StbSecFigure");

                // 子要素が StbSecRect か StbSecCircle を判定
                if (xSecFigure.Element("StbSecRect") != null)
                {
                    _xSecRcColumnDepth.Add((int)xSecFigure.Element("StbSecRect").Attribute("DY"));
                    _xSecRcColumnWidth.Add((int)xSecFigure.Element("StbSecRect").Attribute("DX"));
                }
                else
                {
                    _xSecRcColumnDepth.Add((int)xSecFigure.Element("StbSecCircle").Attribute("D"));
                    _xSecRcColumnWidth.Add(0); // Circle と判定用に width は 0
                }
            }

            // StbSecColumn_S の取得
            var xSecSColumns = xDoc.Root.Descendants("StbSecColumn_S");
            foreach (var xSecSColumn in xSecSColumns)
            {
                _xSecSColumnId.Add((int)xSecSColumn.Attribute("id"));
                _xSecSColumnShape.Add((string)xSecSColumn.Element("StbSecSteelColumn").Attribute("shape"));
            }

            // StbSecBeam_RC の取得
            var xSecRcBeams = xDoc.Root.Descendants("StbSecBeam_RC");
            foreach (var xSecRcBeam in xSecRcBeams)
            {
                _xSecRcBeamId.Add((int)xSecRcBeam.Attribute("id"));
                var xSecFigure = xSecRcBeam.Element("StbSecFigure");

                // 子要素が StbSecHaunch か StbSecStraight を判定
                if (xSecFigure.Element("StbSecHaunch") != null)
                {
                    _xSecRcBeamDepth.Add((int)xSecFigure.Element("StbSecHaunch").Attribute("depth_center"));
                    _xSecRcBeamWidth.Add((int)xSecFigure.Element("StbSecHaunch").Attribute("width_center"));
                }
                else
                {
                    _xSecRcBeamDepth.Add((int)xSecFigure.Element("StbSecStraight").Attribute("depth"));
                    _xSecRcBeamWidth.Add((int)xSecFigure.Element("StbSecStraight").Attribute("width"));
                }
            }

            // StbSecBeam_S の取得
            var xSecSBeams = xDoc.Root.Descendants("StbSecBeam_S");
            foreach (var xSecSBeam in xSecSBeams)
            {
                _xSecSBeamId.Add((int)xSecSBeam.Attribute("id"));
                _xSecSBeamShape.Add((string)xSecSBeam.Element("StbSecSteelBeam").Attribute("shape"));
            }

            // StbSecBrace_S の取得
            var xSecSBraces = xDoc.Root.Descendants("StbSecBrace_S");
            foreach (var xSecSBrace in xSecSBraces)
            {
                _xSecSBraceId.Add((int)xSecSBrace.Attribute("id"));
                _xSecSBraceShape.Add((string)xSecSBrace.Element("StbSecSteelBrace").Attribute("shape"));
            }

            // S断面形状の取得
            string[,] steelSecName = GetSteelSecNameArray();
            for (int i = 0; i < steelSecName.GetLength(0); i++)
                GetStbSteelSection(xDoc, steelSecName[i, 0], steelSecName[i, 1]);
            // 断面の生成
            _columns = MakeElementBrep(xDoc, "StbColumn", "Column");
            _girders = MakeElementBrep(xDoc, "StbGirder", "Beam");
            _posts = MakeElementBrep(xDoc, "StbPost", "Column");
            _beams = MakeElementBrep(xDoc, "StbBeam", "Beam");
            _braces = MakeElementBrep(xDoc, "StbBrace", "Brace");

            DA.SetDataList(0, _columns);
            DA.SetDataList(1, _girders);
            DA.SetDataList(2, _posts);
            DA.SetDataList(3, _beams);
            DA.SetDataList(4, _braces);
            DA.SetDataList(5, _slabs);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
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
        public override Guid ComponentGuid
        {
            get { return new Guid("7d2f0c4e-4888-4607-8548-592104f6f06f"); }
        }

        /// <summary>
        /// Get ST-Bridge Steel Section
        /// </summary>
        /// <param name="xdoc">XML data</param>
        /// <param name="xDateTag">XML data tag you want to read</param>
        /// <param name="SectionType">Section type, H or BOX or Pipe or L</param>
        public void GetStbSteelSection(XDocument xdoc, string xDateTag, string SectionType)
        {
            if (SectionType == "Pipe")
            {
                var xSteelSections = xdoc.Root.Descendants(xDateTag);
                foreach (var xSteelSection in xSteelSections)
                {
                    _xStbSecSteelName.Add((string)xSteelSection.Attribute("name"));
                    _xStbSecSteelParamA.Add((double)xSteelSection.Attribute("D"));
                    _xStbSecSteelParamB.Add((double)xSteelSection.Attribute("t"));
                    _xStbSecSteelType.Add(SectionType);
                }
            }
            else if (SectionType == "Bar")
            {
                var xSteelSections = xdoc.Root.Descendants(xDateTag);
                foreach (var xSteelSection in xSteelSections)
                {
                    _xStbSecSteelName.Add((string)xSteelSection.Attribute("name"));
                    _xStbSecSteelParamA.Add((double)xSteelSection.Attribute("R"));
                    _xStbSecSteelParamB.Add(0.0);
                    _xStbSecSteelType.Add(SectionType);
                }
            }
            else if (SectionType == "NotSupport")
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Some section are not supported. (T, FB, LipC, C) ");
            }
            else
            {
                var xSteelSections = xdoc.Root.Descendants(xDateTag);
                foreach (var xSteelSection in xSteelSections)
                {
                    _xStbSecSteelName.Add((string)xSteelSection.Attribute("name"));
                    _xStbSecSteelParamA.Add((double)xSteelSection.Attribute("A"));
                    _xStbSecSteelParamB.Add((double)xSteelSection.Attribute("B"));
                    _xStbSecSteelType.Add(SectionType);
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
        public Brep[] MakeElementBrep(XDocument xdoc, string xDateTag, string ElementStructureType)
        {
            _elementShapeBrep.Clear();

            var xElements = xdoc.Root.Descendants(xDateTag);
            foreach (var xElement in xElements)
            {
                if (ElementStructureType == "Beam" || ElementStructureType == "Brace")
                {
                    _xNodeStart = (int)xElement.Attribute("idNode_start");
                    _xNodeEnd = (int)xElement.Attribute("idNode_end");
                }
                else
                {
                    _xNodeStart = (int)xElement.Attribute("idNode_bottom");
                    _xNodeEnd = (int)xElement.Attribute("idNode_top");
                }
                _xElementIdSection = (int)xElement.Attribute("id_section");
                _xElementKind = (string)xElement.Attribute("kind_structure");

                // 始点と終点の座標取得
                _nodeIndexStart = _nodeIDs.IndexOf(_xNodeStart);
                _nodeIndexEnd = _nodeIDs.IndexOf(_xNodeEnd);
                _nodeStart = _nodes[_nodeIndexStart];
                _nodeEnd = _nodes[_nodeIndexEnd];

                if (_xElementKind == "RC")
                {
                    // 断面形状名（shape) と 断面形状（HxB）の取得の取得
                    if (ElementStructureType == "Beam")
                    {
                        _stbSecIndex = _xSecRcBeamId.IndexOf(_xElementIdSection);
                        _elementHight = _xSecRcBeamDepth[_stbSecIndex];
                        _elementWidth = _xSecRcBeamWidth[_stbSecIndex];
                    }
                    else if (ElementStructureType == "Column")
                    {
                        _stbSecIndex = _xSecRcColumnId.IndexOf(_xElementIdSection);
                        _elementHight = _xSecRcColumnDepth[_stbSecIndex];
                        _elementWidth = _xSecRcColumnWidth[_stbSecIndex];
                    }

                    if (_elementWidth == 0)
                    {
                        _elementShapeType = "Pipe";
                    }
                    else
                    {
                        _elementShapeType = "BOX";
                    }
                }
                else if (_xElementKind == "S")
                {
                    // 断面形状名（shape）の取得の取得
                    if (ElementStructureType == "Beam")
                    {
                        _elementIdSection = _xSecSBeamId.IndexOf(_xElementIdSection);
                        _elementShape = _xSecSBeamShape[_elementIdSection];
                    }
                    else if (ElementStructureType == "Column")
                    {
                        _elementIdSection = _xSecSColumnId.IndexOf(_xElementIdSection);
                        _elementShape = _xSecSColumnShape[_elementIdSection];
                    }
                    else if (ElementStructureType == "Brace")
                    {
                        _elementIdSection = _xSecSBraceId.IndexOf(_xElementIdSection);
                        _elementShape = _xSecSBraceShape[_elementIdSection];
                    }
                    // 断面形状（HxB）の取得の取得
                    _stbSecIndex = _xStbSecSteelName.IndexOf(_elementShape);
                    _elementHight = _xStbSecSteelParamA[_stbSecIndex];
                    _elementWidth = _xStbSecSteelParamB[_stbSecIndex];
                    _elementShapeType = _xStbSecSteelType[_stbSecIndex];
                }

                // 始点と終点から梁断面サーフェスの作成
                _elementShapeBrep = MakeElementsBrepFromVertex(_nodeStart, _nodeEnd, _elementHight, _elementWidth, _elementShapeType, ElementStructureType);
            }
            _elementShapeBrepArray = Brep.JoinBreps(_elementShapeBrep, GH_Component.DocumentTolerance());
            return _elementShapeBrepArray;
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
        public List<Brep> MakeElementsBrepFromVertex(Point3d NodeStart, Point3d NodeEnd, double ElementHight, double ElementWidth, string ElementShapeType, string ElementStructureType)
        {

            // 部材のアングルの確認
            _elementAngleY = -1.0 * Math.Atan((NodeEnd.Y - NodeStart.Y) / (NodeEnd.X - NodeStart.X));
            _elementAngleZ = -1.0 * Math.Atan((NodeEnd.Z - NodeStart.Z) / (NodeEnd.X - NodeStart.X));

            // 描画用点の作成
            // 梁は部材天端の中心が起点に対して、柱・ブレースは部材芯が起点なので場合分け
            // NodeStart側   
            //  Y        S4 - S5 - S6 
            //  ^        |    |    |  
            //  o >  X   S1 - S2 - S3
            if (ElementStructureType == "Beam")
            {
                _vertexS1 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z - ElementHight
                                       );
                _vertexS2 = new Point3d(NodeStart.X,
                                       NodeStart.Y,
                                       NodeStart.Z - ElementHight
                                       );
                _vertexS3 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z - ElementHight
                                       );
                _vertexS4 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z
                                       );
                _vertexS5 = NodeStart;
                _vertexS6 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z
                                       );
            }
            else if (ElementStructureType == "Column")
            {
                _vertexS1 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeStart.Y - (ElementHight / 2.0),
                                       NodeStart.Z - (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
                _vertexS2 = new Point3d(NodeStart.X,
                                       NodeStart.Y + (ElementHight / 2.0),
                                       NodeStart.Z
                                       );
                _vertexS3 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeStart.Y - (ElementHight / 2.0),
                                       NodeStart.Z + (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
                _vertexS4 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeStart.Y + (ElementHight / 2.0),
                                       NodeStart.Z - (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
                _vertexS5 = new Point3d(NodeStart.X,
                                       NodeStart.Y - (ElementHight / 2.0),
                                       NodeStart.Z
                                       );
                _vertexS6 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeStart.Y + (ElementHight / 2.0),
                                       NodeStart.Z + (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
            }
            else if (ElementStructureType == "Brace")
            {
                _vertexS1 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z - (ElementWidth / 2.0)
                                       );
                _vertexS2 = new Point3d(NodeStart.X,
                                       NodeStart.Y,
                                       NodeStart.Z - (ElementWidth / 2.0)
                                       );
                _vertexS3 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z - (ElementWidth / 2.0)
                                       );
                _vertexS4 = new Point3d(NodeStart.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z + (ElementWidth / 2.0)
                                       );
                _vertexS5 = new Point3d(NodeStart.X,
                                       NodeStart.Y,
                                       NodeStart.Z + (ElementWidth / 2.0)
                                       );
                _vertexS6 = new Point3d(NodeStart.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeStart.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeStart.Z + (ElementWidth / 2.0)
                                       );
            }
            // NodeEnd側
            //  Y        E4 - E5 - E6
            //  ^        |    |    |
            //  o >  X   E1 - E2 - E3
            if (ElementStructureType == "Beam")
            {
                _vertexE1 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z - ElementHight
                                       );
                _vertexE2 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y,
                                       NodeEnd.Z - ElementHight
                                       );
                _vertexE3 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z - ElementHight
                                       );
                _vertexE4 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z
                                       );
                _vertexE5 = NodeEnd;
                _vertexE6 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z
                                       );
            }
            else if (ElementStructureType == "Column")
            {
                _vertexE1 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeEnd.Y - (ElementHight / 2.0),
                                       NodeEnd.Z - (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
                _vertexE2 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y + (ElementHight / 2.0),
                                       NodeEnd.Z
                                       );
                _vertexE3 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeEnd.Y - (ElementHight / 2.0),
                                       NodeEnd.Z + (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
                _vertexE4 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeEnd.Y + (ElementHight / 2.0),
                                       NodeEnd.Z - (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
                _vertexE5 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y - (ElementHight / 2.0),
                                       NodeEnd.Z
                                       );
                _vertexE6 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleZ),
                                       NodeEnd.Y + (ElementHight / 2.0),
                                       NodeEnd.Z + (ElementWidth / 2.0) * Math.Cos(_elementAngleZ)
                                       );
            }
            else if (ElementStructureType == "Brace")
            {
                _vertexE1 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z - (ElementWidth / 2.0)
                                       );
                _vertexE2 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y,
                                       NodeEnd.Z - (ElementWidth / 2.0)
                                       );
                _vertexE3 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z - (ElementWidth / 2.0)
                                       );
                _vertexE4 = new Point3d(NodeEnd.X + (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y + (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z + (ElementWidth / 2.0)
                                       );
                _vertexE5 = new Point3d(NodeEnd.X,
                                       NodeEnd.Y,
                                       NodeEnd.Z + (ElementWidth / 2.0)
                                       );
                _vertexE6 = new Point3d(NodeEnd.X - (ElementWidth / 2.0) * Math.Sin(_elementAngleY),
                                       NodeEnd.Y - (ElementWidth / 2.0) * Math.Cos(_elementAngleY),
                                       NodeEnd.Z + (ElementWidth / 2.0)
                                       );
            }

            if (this._elementShapeType == "H")
            {
                // make upper flange
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS4, _vertexS6, _vertexE6, _vertexE4, GH_Component.DocumentTolerance()));
                // make bottom flange
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS1, _vertexS3, _vertexE3, _vertexE1, GH_Component.DocumentTolerance()));
                // make web 
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS5, _vertexS2, _vertexE2, _vertexE5, GH_Component.DocumentTolerance()));
            }
            else if (this._elementShapeType == "BOX")
            {
                // make upper flange
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS4, _vertexS6, _vertexE6, _vertexE4, GH_Component.DocumentTolerance()));
                // make bottom flange
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS1, _vertexS3, _vertexE3, _vertexE1, GH_Component.DocumentTolerance()));
                // make web 1
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS4, _vertexS1, _vertexE1, _vertexE4, GH_Component.DocumentTolerance()));
                // make web 2
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS6, _vertexS3, _vertexE3, _vertexE6, GH_Component.DocumentTolerance()));
            }
            else if (this._elementShapeType == "Pipe")
            {
                LineCurve PipeCurve = new LineCurve(NodeStart, NodeEnd);
                _elementShapeBrep.Add(Brep.CreatePipe(PipeCurve, ElementHight / 2.0, false, 0, false, GH_Component.DocumentTolerance(), GH_Component.DocumentAngleTolerance())[0]);
            }
            else if (this._elementShapeType == "L")
            {
                // make bottom flange
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS1, _vertexS3, _vertexE3, _vertexE1, GH_Component.DocumentTolerance()));
                // make web
                _elementShapeBrep.Add(Brep.CreateFromCornerPoints(_vertexS6, _vertexS3, _vertexE3, _vertexE6, GH_Component.DocumentTolerance()));
            }
            else
            {
            }
            return _elementShapeBrep;
        }

        string[,] GetSteelSecNameArray()
        {
            string[,] steelSecNameArray = new string[7, 2] {
                {"StbSecRoll-H", "H"},
                {"StbSecBuild-H", "H"},
                {"StbSecRoll-BOX", "BOX"},
                {"StbSecBuild-BOX", "BOX"},
                {"StbSecPipe", "Pipe"},
                {"StbSecRoll-L", "L"},
                {"StbSecRoll-Bar", "Bar"}
            };
            return (steelSecNameArray);
        }

        string[,] GetMemberNameArray()
        {
            string[,] memberNameArray = new string[5, 2] {
                {"StbColumn", "Column"},
                {"StbGirder", "Girder"},
                {"StbPost", "Post"},
                {"StbBeam", "Beam"},
                {"StbBrace", "Brace"}
            };
            return (memberNameArray);
        }
    }
}
