using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using STBDotNet.v202;


namespace HoaryFox.Component.Utils
{
    internal static class TagUtils
    {
        internal static Point3d GetFrameTagPosition(string idStart, string idEnd, IEnumerable<StbNode> nodes)
        {
            StbNode startNode = nodes.First(node => node.id == idStart);
            StbNode endNode = nodes.First(node => node.id == idEnd);

            return new Point3d((startNode.X + endNode.X) / 2.0, (startNode.Y + endNode.Y) / 2.0, (startNode.Z + endNode.Z) / 2.0
            );
        }

        internal static Point3d GetPlateTagPosition(string idOrder, StbSlabOffset[] offsets, IEnumerable<StbNode> nodes)
        {
            string[] nodeIds = idOrder.Split(' ');
            var pts = new Point3d[nodeIds.Length];
            for (int i = 0; i < nodeIds.Length; i++)
            {
                string nodeId = nodeIds[i];
                StbNode node = nodes.First(n => n.id == nodeId);
                var offsetVec = new Vector3d();
                if (offsets != null)
                {
                    foreach (var offset in offsets.Where(offset => nodeId == offset.id_node))
                    {
                        offsetVec = new Vector3d(offset.offset_X, offset.offset_Y, offset.offset_Z);
                    }
                }
                pts[i] = new Point3d(node.X, node.Y, node.Z) + offsetVec;
            }

            return new Point3d(pts.Average(n => n.X), pts.Average(n => n.Y), pts.Average(n => n.Z));
        }

        internal static IEnumerable<GH_String> GetBeamRcSection(object rcFigure, string strength)
        {
            var ghSecStrings = new GH_Structure<GH_String>();
            switch (rcFigure)
            {
                case StbSecBeam_RC_Straight figure:
                    ghSecStrings.Append(new GH_String("BD-" + figure.width + "x" + figure.depth + "(" + strength + ")"));
                    break;
                case StbSecBeam_RC_Haunch figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ": BD-" + figure.width + "x" + figure.depth + "(" + strength + ")"));
                    break;
                case StbSecBeam_RC_Taper figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ": BD-" + figure.width + "x" + figure.depth + "(" + strength + ")"));
                    break;
                case StbSecBeam_SRC_Straight figure:
                    ghSecStrings.Append(new GH_String("BD-" + figure.width + "x" + figure.depth + "(" + strength + ")"));
                    break;
                case StbSecBeam_SRC_Haunch figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ": BD-" + figure.width + "x" + figure.depth + "(" + strength + ")"));
                    break;
                case StbSecBeam_SRC_Taper figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ": BD-" + figure.width + "x" + figure.depth + "(" + strength + ")"));
                    break;
            }

            return ghSecStrings;
        }

        internal static IEnumerable<GH_String> GetColumnRcSection(object rcFigure, string strength)
        {
            var ghSecStrings = new GH_Structure<GH_String>();
            switch (rcFigure)
            {
                case StbSecColumn_RC_Rect figure:
                    ghSecStrings.Append(new GH_String("CD-" + figure.width_X + "x" + figure.width_Y + "(" + strength + ")"));
                    break;
                case StbSecColumn_RC_Circle figure:
                    ghSecStrings.Append(new GH_String("P-" + figure.D + "(" + strength + ")"));
                    break;
                case StbSecColumn_SRC_Rect figure:
                    ghSecStrings.Append(new GH_String("CD-" + figure.width_X + "x" + figure.width_Y + "(" + strength + ")"));
                    break;
                case StbSecColumn_SRC_Circle figure:
                    ghSecStrings.Append(new GH_String("P-" + figure.D + "(" + strength + ")"));
                    break;
            }

            return ghSecStrings;
        }

        internal static IEnumerable<GH_String> GetBeamSSection(object steelFigure)
        {
            var ghSecStrings = new GH_Structure<GH_String>();
            switch (steelFigure)
            {
                case StbSecSteelBeam_S_Haunch figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBeam_S_FiveTypes figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBeam_S_Taper figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBeam_S_Straight figure:
                    ghSecStrings.Append(new GH_String(figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBeam_SRC_Haunch figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBeam_SRC_FiveTypes figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBeam_SRC_Taper figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBeam_SRC_Straight figure:
                    ghSecStrings.Append(new GH_String(figure.shape + "(" + figure.strength_main + ")"));
                    break;
            }

            return ghSecStrings;
        }

        internal static IEnumerable<GH_String> GetBraceSSection(object steelFigure)
        {
            var ghSecStrings = new GH_Structure<GH_String>();
            switch (steelFigure)
            {
                case StbSecSteelBrace_S_Same figure:
                    ghSecStrings.Append(new GH_String(figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBrace_S_NotSame figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelBrace_S_ThreeTypes figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
            }

            return ghSecStrings;
        }

        internal static IEnumerable<GH_String> GetColumnSSection(object steelFigure)
        {
            var ghSecStrings = new GH_Structure<GH_String>();
            switch (steelFigure)
            {
                case StbSecSteelColumn_S_Same figure:
                    ghSecStrings.Append(new GH_String(figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelColumn_S_NotSame figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelColumn_S_ThreeTypes figure:
                    ghSecStrings.Append(new GH_String(figure.pos + ":" + figure.shape + "(" + figure.strength_main + ")"));
                    break;
                case StbSecSteelColumn_SRC_Same figure:
                    ghSecStrings.Append(GetColumnSrcInnerSteelShape(figure.Item, string.Empty));
                    break;
                case StbSecSteelColumn_SRC_NotSame figure:
                    ghSecStrings.Append(GetColumnSrcInnerSteelShape(figure.Item, figure.pos.ToString()));
                    break;
                case StbSecSteelColumn_SRC_ThreeTypes figure:
                    ghSecStrings.Append(GetColumnSrcInnerSteelShape(figure.Item, figure.pos.ToString()));
                    break;
            }

            return ghSecStrings;
        }

        private static GH_String GetColumnSrcInnerSteelShape(object figure, string pos)
        {
            switch (figure)
            {
                // Shape H
                case StbSecColumn_SRC_SameShapeH shape:
                    return new GH_String(shape.shape + "(" + shape.strength_main + ")");
                case StbSecColumn_SRC_NotSameShapeH shape:
                    return new GH_String(pos + ":" + shape.shape + "(" + shape.strength_main + ")");
                case StbSecColumn_SRC_ThreeTypesShapeH shape:
                    return new GH_String(pos + ":" + shape.shape + "(" + shape.strength_main + ")");
                // Shape Box
                case StbSecColumn_SRC_SameShapeBox shape:
                    return new GH_String(shape.shape + "(" + shape.strength + ")");
                case StbSecColumn_SRC_NotSameShapeBox shape:
                    return new GH_String(pos + ":" + shape.shape + "(" + shape.strength + ")");
                case StbSecColumn_SRC_ThreeTypesShapeBox shape:
                    return new GH_String(pos + ":" + shape.shape + "(" + shape.strength + ")");
                // Shape Pipe
                case StbSecColumn_SRC_SameShapePipe shape:
                    return new GH_String(shape.shape + "(" + shape.strength + ")");
                case StbSecColumn_SRC_NotSameShapePipe shape:
                    return new GH_String(pos + ":" + shape.shape + "(" + shape.strength + ")");
                case StbSecColumn_SRC_ThreeTypesShapePipe shape:
                    return new GH_String(pos + ":" + shape.shape + "(" + shape.strength + ")");
                // Shape Cross
                case StbSecColumn_SRC_SameShapeCross shape:
                    return new GH_String("X:" + shape.shape_X + "(" + shape.strength_main_X + ")" + "Y:" + shape.shape_Y + "(" + shape.strength_main_Y + ")");
                case StbSecColumn_SRC_NotSameShapeCross shape:
                    return new GH_String(pos + ":" + "X:" + shape.shape_X + "(" + shape.strength_main_X + ")" + "Y:" + shape.shape_Y + "(" + shape.strength_main_Y + ")");
                case StbSecColumn_SRC_ThreeTypesShapeCross shape:
                    return new GH_String(pos + ":" + "X:" + shape.shape_X + "(" + shape.strength_main_X + ")" + "Y:" + shape.shape_Y + "(" + shape.strength_main_Y + ")");
                // Shape T
                case StbSecColumn_SRC_SameShapeT shape:
                    return new GH_String("H:" + shape.shape_H + "(" + shape.strength_main_H + ")" + "T:" + shape.shape_T + "(" + shape.strength_main_T + ")");
                case StbSecColumn_SRC_NotSameShapeT shape:
                    return new GH_String(pos + ":" + "H:" + shape.shape_H + "(" + shape.strength_main_H + ")" + "T:" + shape.shape_T + "(" + shape.strength_main_T + ")");
                case StbSecColumn_SRC_ThreeTypesShapeT shape:
                    return new GH_String(pos + ":" + "H:" + shape.shape_H + "(" + shape.strength_main_H + ")" + "T:" + shape.shape_T + "(" + shape.strength_main_T + ")");
                default:
                    throw new ArgumentException("Unsupported SRC inner Steel type");
            }
        }

        internal static IEnumerable<GH_String> GetSlabRcSection(object slabFigure, string strength)
        {
            var ghSecString = new GH_Structure<GH_String>();
            switch (slabFigure)
            {
                case StbSecSlab_RC_Straight figure:
                    ghSecString.Append(new GH_String("t=" + figure.depth + "(" + strength + ")"));
                    break;
                case StbSecSlab_RC_Taper figure:
                    ghSecString.Append(new GH_String("t=" + figure.pos + ":" + figure.depth + "(" + strength + ")"));
                    break;
                case StbSecSlab_RC_Haunch figure:
                    ghSecString.Append(new GH_String("t=" + figure.pos + ":" + figure.depth + "(" + strength + ")"));
                    break;
            }

            return ghSecString;
        }

        internal static IEnumerable<GH_String> GetSlabDeckSection(StbSecSlabDeckStraight figure, string strength)
        {
            var ghSecString = new GH_Structure<GH_String>();
            ghSecString.Append(new GH_String("t=" + figure.depth + "(" + strength + ")"));

            return ghSecString;
        }

        internal static IEnumerable<GH_String> GetSlabPrecastSection(StbSecSlabPrecastPrecast_type type, StbSecProductSlabPrecast figure, string strength)
        {
            var ghSecString = new GH_Structure<GH_String>();
            ghSecString.Append(new GH_String("t=" + figure.depth + "(" + strength + ", type:" + type + ")"));

            return ghSecString;
        }

        internal static IEnumerable<GH_String> GetWallSection(StbSecWall_RC_Straight figure, string strength)
        {
            var ghSecString = new GH_Structure<GH_String>();
            ghSecString.Append(new GH_String("t=" + figure.t + "(" + strength + ")"));

            return ghSecString;
        }

        public static Dictionary<string, string>[][] GetAllSectionInfoArray(StbMembers members, StbSections sections)
        {
            var allTagList = new Dictionary<string, string>[7][];

            var memberArray = new object[][] { members.StbColumns, members.StbGirders, members.StbPosts, members.StbBeams, members.StbBraces, members.StbSlabs, members.StbWalls };
            for (var i = 0; i < 7; i++)
            {
                allTagList[i] = memberArray[i] != null ? StbMembersToDictArray(memberArray[i], sections) : Array.Empty<Dictionary<string, string>>();
            }

            return allTagList;
        }

        private static Dictionary<string, string>[] StbMembersToDictArray(IReadOnlyList<object> members, StbSections sections)
        {
            var propertiesArray = new Dictionary<string, string>[members.Count];

            object item = members[0];
            Type t = item.GetType();

            foreach ((object member, int index) in members.Select((column, index) => (column, index)))
            {
                propertiesArray[index] = GetMemberInfoDictionary(t, member, sections);
            }

            return propertiesArray;
        }

        private static Dictionary<string, string> GetMemberInfoDictionary(Type type, object member, StbSections sections)
        {
            PropertyInfo[] props = type.GetProperties();
            var instanceProps = new Dictionary<string, string> { { "stb_element_type", type.Name } };
            foreach (PropertyInfo prop in props)
            {
                if (prop.GetValue(member) == null)
                {
                    continue;
                }
                try
                {
                    instanceProps.Add(prop.Name, prop.GetValue(member).ToString());
                }
                catch
                {
                    // ignored
                }
            }
            AppendSectionInfos(instanceProps, sections);

            return instanceProps;
        }

        private static void AppendSectionInfos(IDictionary<string, string> pDict, StbSections sections)
        {
            var sectionInfo = new List<GH_String>();
            switch (pDict["stb_element_type"])
            {
                case "StbColumn":
                case "StbPost":
                    switch (pDict["kind_structure"])
                    {
                        case "RC":
                            StbSecColumn_RC columnRc = sections.StbSecColumn_RC.First(sec => sec.id == pDict["id_section"]);
                            sectionInfo = GetColumnRcSection(columnRc.StbSecFigureColumn_RC.Item, columnRc.strength_concrete).ToList();
                            break;
                        case "SRC":
                            StbSecColumn_SRC columnSrc = sections.StbSecColumn_SRC.First(sec => sec.id == pDict["id_section"]);
                            sectionInfo = GetColumnRcSection(columnSrc.StbSecFigureColumn_SRC.Item, columnSrc.strength_concrete).ToList();
                            foreach (object item in columnSrc.StbSecSteelFigureColumn_SRC.Items)
                            {
                                sectionInfo.AddRange(GetColumnSSection(item).ToList());
                            }
                            break;
                        case "S":
                            StbSecSteelFigureColumn_S sFigure = sections.StbSecColumn_S.First(sec => sec.id == pDict["id_section"]).StbSecSteelFigureColumn_S;
                            foreach (object item in sFigure.Items)
                            {
                                sectionInfo.AddRange(GetColumnSSection(item).ToList());
                            }
                            break;
                    }
                    break;
                case "StbGirder":
                case "StbBeam":
                    switch (pDict["kind_structure"])
                    {
                        case "RC":
                            StbSecBeam_RC beamRc = sections.StbSecBeam_RC.First(sec => sec.id == pDict["id_section"]);
                            foreach (object item in beamRc.StbSecFigureBeam_RC.Items)
                            {
                                sectionInfo.AddRange(GetBeamRcSection(item, beamRc.strength_concrete));
                            }
                            break;
                        case "SRC":
                            StbSecBeam_SRC beamSrc = sections.StbSecBeam_SRC.First(sec => sec.id == pDict["id_section"]);
                            foreach (object item in beamSrc.StbSecFigureBeam_SRC.Items)
                            {
                                sectionInfo.AddRange(GetBeamRcSection(item, beamSrc.strength_concrete));
                            }

                            foreach (object item in beamSrc.StbSecSteelFigureBeam_SRC.Items)
                            {
                                sectionInfo.AddRange(GetBeamSSection(item).ToList());
                            }
                            break;
                        case "S":
                            StbSecSteelFigureBeam_S sFigure = sections.StbSecBeam_S.First(sec => sec.id == pDict["id_section"]).StbSecSteelFigureBeam_S;
                            foreach (object item in sFigure.Items)
                            {
                                sectionInfo.AddRange(GetBeamSSection(item).ToList());
                            }
                            break;
                    }
                    break;
                case "StbBrace":
                    switch (pDict["kind_structure"])
                    {
                        case "S":
                            StbSecSteelFigureBrace_S sFigure = sections.StbSecBrace_S.First(sec => sec.id == pDict["id_section"]).StbSecSteelFigureBrace_S;
                            foreach (object item in sFigure.Items)
                            {
                                sectionInfo.AddRange(GetBraceSSection(item).ToList());
                            }
                            break;
                    }
                    break;
                case "StbSlab":
                    switch (pDict["kind_structure"])
                    {
                        case "RC":
                            StbSecSlab_RC slabRc = sections.StbSecSlab_RC.First(sec => sec.id == pDict["id_section"]);
                            foreach (object item in slabRc.StbSecFigureSlab_RC.Items)
                            {
                                sectionInfo.AddRange(GetSlabRcSection(item, slabRc.strength_concrete).ToList());
                            }
                            break;
                        case "DECK":
                        case "PRECAST":
                            break;
                    }
                    break;
                case "StbWall": // RC しかない
                    StbSecWall_RC wallRc = sections.StbSecWall_RC.First(sec => sec.id == pDict["id_section"]);
                    sectionInfo = GetWallSection(wallRc.StbSecFigureWall_RC.StbSecWall_RC_Straight, wallRc.strength_concrete).ToList();
                    break;
            }

            foreach ((string str, int i) in sectionInfo.Select((str, i) => (str.ToString(), i)))
            {
                pDict.Add($"Figure{i}", str);
            }
        }

    }
}
