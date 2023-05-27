using System.Collections.Generic;
using System.Linq;

using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry
{
    public class CreateMemberBrepListFromStb
    {
        private readonly IEnumerable<StbNode> _nodes;
        private readonly IReadOnlyList<double> _tolerance;
        private readonly StbSections _sections;

        public CreateMemberBrepListFromStb(StbSections sections, IEnumerable<StbNode> nodes, IReadOnlyList<double> tolerance)
        {
            _nodes = nodes;
            _tolerance = tolerance;
            _sections = sections;
        }

        public GH_Structure<GH_Brep> Column(IEnumerable<StbColumn> columns)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (columns == null)
            {
                return brepList;
            }

            foreach ((StbColumn column, int i) in columns.Select((column, index) => (column, index)))
            {
                StbColumnKind_structure kind = column.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == column.id_node_bottom),
                    _nodes.First(node => node.id == column.id_node_top)
                };
                Point3d[] offset =
                {
                    new Point3d(column.offset_bottom_X, column.offset_bottom_Y, column.offset_bottom_Z),
                    new Point3d(column.offset_top_X, column.offset_top_Y, column.offset_top_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    new Point3d(),
                    new Point3d(),
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * column.joint_bottom;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * column.joint_top;

                var brepMaker = new BrepMaker.Column(_sections, _tolerance);
                brepList.Append(new GH_Brep(brepMaker.CreateColumnBrep(column.id_section, column.rotate, kind, sectionPoints, memberAxis)), new GH_Path(0, i));
            }

            return brepList;
        }

        public GH_Structure<GH_Brep> Post(IEnumerable<StbPost> posts)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (posts == null)
            {
                return brepList;
            }

            foreach ((StbPost post, int i) in posts.Select((post, index) => (post, index)))
            {
                StbColumnKind_structure kind = post.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == post.id_node_bottom),
                    _nodes.First(node => node.id == post.id_node_top)
                };
                Point3d[] offset =
                {
                    new Point3d(post.offset_bottom_X, post.offset_bottom_Y, post.offset_bottom_Z),
                    new Point3d(post.offset_top_X, post.offset_top_Y, post.offset_top_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    new Point3d(),
                    new Point3d(),
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * post.joint_bottom;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * post.joint_top;

                var brepMaker = new BrepMaker.Column(_sections, _tolerance);
                brepList.Append(new GH_Brep(brepMaker.CreateColumnBrep(post.id_section, post.rotate, kind, sectionPoints, memberAxis)), new GH_Path(0, i));
            }

            return brepList;
        }

        public GH_Structure<GH_Brep> Girder(IEnumerable<StbGirder> girders)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (girders == null)
            {
                return brepList;
            }

            foreach ((StbGirder girder, int i) in girders.Select((girder, index) => (girder, index)))
            {
                StbGirderKind_structure kind = girder.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == girder.id_node_start),
                    _nodes.First(node => node.id == girder.id_node_end)
                };
                Point3d[] offset =
                {
                    new Point3d(girder.offset_start_X, girder.offset_start_Y, girder.offset_start_Z),
                    new Point3d(girder.offset_end_X, girder.offset_end_Y, girder.offset_end_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    Point3d.Origin, // haunch_s
                    Point3d.Origin, // joint_s
                    Point3d.Origin, // joint_e 
                    Point3d.Origin, // haunch_e
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[5] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * girder.haunch_start;
                sectionPoints[2] = sectionPoints[0] + memberAxis / memberAxis.Length * girder.joint_start;
                sectionPoints[3] = sectionPoints[5] - memberAxis / memberAxis.Length * girder.joint_end;
                sectionPoints[4] = sectionPoints[5] - memberAxis / memberAxis.Length * girder.haunch_end;

                var brepMaker = new BrepMaker.Girder(_sections, _tolerance);
                brepList.Append(new GH_Brep(brepMaker.CreateGirderBrep(girder.id_section, girder.rotate, kind, sectionPoints, memberAxis)), new GH_Path(0, i));
            }

            return brepList;
        }

        public GH_Structure<GH_Brep> Beam(IEnumerable<StbBeam> beams)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (beams == null)
            {
                return brepList;
            }

            foreach ((StbBeam beam, int i) in beams.Select((beam, index) => (beam, index)))
            {
                StbGirderKind_structure kind = beam.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == beam.id_node_start),
                    _nodes.First(node => node.id == beam.id_node_end)
                };
                Point3d[] offset =
                {
                    new Point3d(beam.offset_start_X, beam.offset_start_Y, beam.offset_start_Z),
                    new Point3d(beam.offset_end_X, beam.offset_end_Y, beam.offset_end_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    Point3d.Origin, // haunch_s
                    Point3d.Origin, // joint_s
                    Point3d.Origin, // joint_e 
                    Point3d.Origin, // haunch_e
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[5] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * beam.haunch_start;
                sectionPoints[2] = sectionPoints[0] + memberAxis / memberAxis.Length * beam.joint_start;
                sectionPoints[3] = sectionPoints[5] - memberAxis / memberAxis.Length * beam.joint_end;
                sectionPoints[4] = sectionPoints[5] - memberAxis / memberAxis.Length * beam.haunch_end;

                var brepMaker = new BrepMaker.Girder(_sections, _tolerance);
                brepList.Append(new GH_Brep(brepMaker.CreateGirderBrep(beam.id_section, beam.rotate, kind, sectionPoints, memberAxis)), new GH_Path(0, i));
            }

            return brepList;
        }

        public GH_Structure<GH_Brep> Brace(IEnumerable<StbBrace> braces)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (braces == null)
            {
                return brepList;
            }

            foreach ((StbBrace brace, int i) in braces.Select((brace, index) => (brace, index)))
            {
                StbBraceKind_structure kind = brace.kind_structure;

                StbNode[] endNodes =
                {
                    _nodes.First(node => node.id == brace.id_node_start),
                    _nodes.First(node => node.id == brace.id_node_end)
                };
                Point3d[] offset =
                {
                    new Point3d(brace.offset_start_X, brace.offset_start_Y, brace.offset_start_Z),
                    new Point3d(brace.offset_end_X, brace.offset_end_Y, brace.offset_end_Z)
                };
                Point3d[] sectionPoints =
                {
                    new Point3d(endNodes[0].X, endNodes[0].Y, endNodes[0].Z) + offset[0],
                    Point3d.Origin,
                    Point3d.Origin,
                    new Point3d(endNodes[1].X, endNodes[1].Y, endNodes[1].Z) + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * brace.joint_start;
                sectionPoints[2] = sectionPoints[3] - memberAxis / memberAxis.Length * brace.joint_end;

                var brepMaker = new BrepMaker.Brace(_sections, _tolerance);
                brepList.Append(new GH_Brep(brepMaker.CreateBraceBrep(brace.id_section, brace.rotate, kind, sectionPoints, memberAxis)), new GH_Path(0, i));
            }

            return brepList;
        }

        public GH_Structure<GH_Brep> Slab(IEnumerable<StbSlab> slabs)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (slabs == null)
            {
                return brepList;
            }

            foreach ((StbSlab slab, int i) in slabs.Select((slab, index) => (slab, index)))
            {
                StbSlabOffset[] offsets = slab.StbSlabOffsetList;
                var curveList = new PolylineCurve[2];
                double depth = BrepMaker.Slab.GetDepth(_sections, slab);
                string[] nodeIds = slab.StbNodeIdOrder.Split(' ');
                var topPts = new List<Point3d>();
                foreach (string nodeId in nodeIds)
                {
                    var offsetVec = new Vector3d();
                    if (offsets != null)
                    {
                        foreach (StbSlabOffset offset in offsets)
                        {
                            if (nodeId == offset.id_node)
                            {
                                offsetVec = new Vector3d(offset.offset_X, offset.offset_Y, offset.offset_Z);
                            }
                        }
                    }

                    StbNode node = _nodes.First(n => n.id == nodeId);
                    topPts.Add(new Point3d(node.X, node.Y, node.Z) + offsetVec);
                }

                topPts.Add(topPts[0]);
                curveList[0] = new PolylineCurve(topPts);
                brepList.Append(CreateSlabBrep(depth, curveList, topPts), new GH_Path(0, i));
            }

            return brepList;
        }

        private GH_Brep CreateSlabBrep(double depth, IList<PolylineCurve> curveList, IEnumerable<Point3d> topPts)
        {
            if (depth > 0)
            {
                Vector3d normal = Vector3d.CrossProduct(curveList[0].TangentAtEnd, curveList[0].TangentAtStart);
                curveList[1] = new PolylineCurve(topPts.Select(pt => pt - normal * depth));
                Brep loftBrep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0];
                Brep capedBrep = loftBrep.CapPlanarHoles(_tolerance[0]);

                if (capedBrep == null)
                {
                    return NonPlanarBrep(depth, curveList);
                }
                CheckBrepOrientation(capedBrep);

                return new GH_Brep(capedBrep);
            }

            return new GH_Brep(curveList[0].IsPlanar()
                     ? Brep.CreatePlanarBreps(curveList[0], _tolerance[0])[0]
                     : Brep.CreatePatch(new[] { curveList[0] }, 5, 5, _tolerance[0]));
        }

        private GH_Brep NonPlanarBrep(double depth, IList<PolylineCurve> curveList)
        {
            var nonPlanarBrep = new List<Brep>();
            var topBrep = Brep.CreatePatch(new[] { curveList[0] }, 5, 5, _tolerance[0]);
            nonPlanarBrep.Add(topBrep);

            BrepFace face = topBrep.Faces[0];
            Vector3d faceNormal = face.NormalAt(face.Domain(0).Mid, face.Domain(1).Mid);
            if (Vector3d.VectorAngle(faceNormal, Vector3d.ZAxis) < Vector3d.VectorAngle(faceNormal, -Vector3d.ZAxis))
            {
                faceNormal = -faceNormal;
            }

            Brep bottomBrep = topBrep.DuplicateBrep();
            bottomBrep.Translate(faceNormal * depth);
            nonPlanarBrep.Add(bottomBrep);

            IEnumerable<Curve> edgeCurveList = topBrep.Edges.Select(edge => edge.DuplicateCurve());
            nonPlanarBrep.AddRange(edgeCurveList.Select(edgeCurve =>
                Surface.CreateExtrusion(edgeCurve, faceNormal * depth).ToBrep()));
            return new GH_Brep(Brep.JoinBreps(nonPlanarBrep, _tolerance[0])[0] ?? topBrep);
        }

        public GH_Structure<GH_Brep> Wall(IEnumerable<StbWall> walls, IEnumerable<StbOpen> opens)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (walls == null)
            {
                return brepList;
            }

            foreach ((StbWall wall, int i) in walls.Select((wall, index) => (wall, index)))
            {
                StbWallOffset[] offsets = wall.StbWallOffsetList;
                var curveList = new PolylineCurve[2];
                double thickness = BrepMaker.Wall.GetThickness(_sections, wall);
                string[] nodeIds = wall.StbNodeIdOrder.Split(' ');
                var wallPts = new List<Point3d>();
                foreach (string nodeId in nodeIds)
                {
                    var offsetVec = new Vector3d();
                    if (offsets != null)
                    {
                        foreach (StbWallOffset offset in offsets)
                        {
                            if (nodeId == offset.id_node)
                            {
                                offsetVec = new Vector3d(offset.offset_X, offset.offset_Y, offset.offset_Z);
                                break;
                            }
                        }
                    }

                    StbNode node = _nodes.First(n => n.id == nodeId);
                    wallPts.Add(new Point3d(node.X, node.Y, node.Z) + offsetVec);
                }
                wallPts.Add(wallPts[0]);
                var centerCurve = new PolylineCurve(wallPts);
                Vector3d normal = Vector3d.CrossProduct(centerCurve.TangentAtEnd, centerCurve.TangentAtStart);
                curveList[0] = new PolylineCurve(wallPts.Select(pt => pt + normal * thickness / 2));
                curveList[1] = new PolylineCurve(wallPts.Select(pt => pt - normal * thickness / 2));
                Brep brep = Brep.CreateFromLoft(curveList, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0].CapPlanarHoles(_tolerance[0]);
                CheckBrepOrientation(brep);

                brep = ApplyWallOpen(opens, wall, wallPts, brep);
                brepList.Append(new GH_Brep(brep), new GH_Path(0, i));
            }

            return brepList;
        }

        private static void CheckBrepOrientation(Brep brep)
        {
            if (brep == null)
            {
                return;
            }

            if (brep.SolidOrientation == BrepSolidOrientation.Inward)
            {
                brep.Flip();
            }
            brep.Faces.SplitKinkyFaces();
        }

        private Brep ApplyWallOpen(IEnumerable<StbOpen> opens, StbWall wall, IReadOnlyList<Point3d> wallPts, Brep brep)
        {
            if (brep == null)
            {
                return brep;
            }

            double thickness = BrepMaker.Wall.GetThickness(_sections, wall);
            var centerCurve = new PolylineCurve(wallPts);
            Vector3d normal = Vector3d.CrossProduct(centerCurve.TangentAtEnd, centerCurve.TangentAtStart);
            var openIds = new List<string>();
            if (wall.StbOpenIdList != null)
            {
                openIds.AddRange(wall.StbOpenIdList.Select(openId => openId.id));
                foreach (string id in openIds)
                {
                    StbOpen open = opens.First(o => o.id == id);
                    Point3d[] openCurvePts = GetOpenCurvePts(wallPts, open);
                    PolylineCurve[] openCurve = GetOpenCurve(thickness, normal, openCurvePts);
                    Brep openBrep = Brep.CreateFromLoft(openCurve, Point3d.Unset, Point3d.Unset, LoftType.Straight, false)[0].CapPlanarHoles(_tolerance[0]);
                    CheckBrepOrientation(openBrep);

                    brep = Brep.CreateBooleanDifference(brep, openBrep, 1)[0];
                }
            }

            return brep;
        }

        private static PolylineCurve[] GetOpenCurve(double thickness, Vector3d normal, Point3d[] openCurvePts)
        {
            var openCurve = new PolylineCurve[2];
            openCurve[0] = new PolylineCurve(openCurvePts.Select(pt => pt + normal * thickness * 2));
            openCurve[1] = new PolylineCurve(openCurvePts.Select(pt => pt - normal * thickness * 2));
            return openCurve;
        }

        private static Point3d[] GetOpenCurvePts(IReadOnlyList<Point3d> wallPts, StbOpen open)
        {
            var openXVec = new Vector3d(wallPts[1] - wallPts[0]);
            openXVec.Unitize();
            Vector3d openYVec = Vector3d.ZAxis;

            var openCurvePts = new Point3d[5];
            openCurvePts[0] = wallPts[0] + openXVec * open.position_X + openYVec * open.position_Y;
            openCurvePts[1] = openCurvePts[0] + openXVec * open.length_X;
            openCurvePts[2] = openCurvePts[1] + openYVec * open.length_Y;
            openCurvePts[3] = openCurvePts[0] + openYVec * open.length_Y;
            openCurvePts[4] = openCurvePts[0];

            return openCurvePts;
        }

        public GH_Structure<GH_Brep> Pile(IEnumerable<StbPile> piles)
        {
            var brepList = new GH_Structure<GH_Brep>();
            if (piles == null)
            {
                return brepList;
            }

            foreach ((StbPile pile, int i) in piles.Select((pile, index) => (pile, index)))
            {
                StbPileKind_structure kind = pile.kind_structure;

                StbNode node = _nodes.First(n => n.id == pile.id_node);
                Point3d[] endNodes =
                {
                    new Point3d(node.X, node.Y, node.Z),
                    new Point3d(node.X, node.Y, node.Z - pile.length_all)
                };
                Point3d[] offset =
                {
                    new Point3d(pile.offset_X, pile.offset_Y, pile.level_top),
                    new Point3d(pile.offset_X, pile.offset_Y, pile.level_top),
                };
                Point3d[] sectionPoints =
                {
                    endNodes[0] + offset[0],
                    new Point3d(),
                    new Point3d(),
                    endNodes[1] + offset[1]
                };
                Vector3d memberAxis = sectionPoints[3] - sectionPoints[0];
                sectionPoints[1] = sectionPoints[0] + memberAxis / memberAxis.Length * pile.length_head;

                var brepMaker = new BrepMaker.Pile(_sections, _tolerance);
                brepList.Append(new GH_Brep(brepMaker.CreatePileBrep(pile.id_section, kind, sectionPoints, memberAxis / memberAxis.Length)), new GH_Path(0, i));
            }

            return brepList;
        }
    }
}
