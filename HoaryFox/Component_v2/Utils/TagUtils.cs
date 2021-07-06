using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Utils
{
    internal static class TagUtils
    {
        internal static Point3d GetTagPosition(string idStart, string idEnd, IEnumerable<StbNode> nodes)
        {
            StbNode startNode = nodes.First(node => node.id == idStart);
            StbNode endNode = nodes.First(node => node.id == idEnd);

            return new Point3d(
                (startNode.X + endNode.X) / 2.0,
                (startNode.Y + endNode.Y) / 2.0,
                (startNode.Z + endNode.Z) / 2.0
            );
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
            }

            return ghSecStrings;
        }

        public static Dictionary<string, string>[][] GetAllSectionInfoDictionary(StbMembers members)
        {
            var allTagList = new Dictionary<string, string>[5][];

            // TODO: 必要な中身をDictionaryで返すようにする
            allTagList[0] = members.StbColumns != null
                ? members.StbColumns
                    .Select(column => column.name)
                    .Select(s => new Dictionary<string, string> { { "name", s } }).ToArray()
                : new Dictionary<string, string>[] { };
            allTagList[1] = members.StbGirders != null
                ? members.StbGirders
                    .Select(column => column.name)
                    .Select(s => new Dictionary<string, string> { { "name", s } }).ToArray()
                : new Dictionary<string, string>[] { };
            allTagList[2] = members.StbPosts != null
                ? members.StbPosts
                    .Select(column => column.name)
                    .Select(s => new Dictionary<string, string> { { "name", s } }).ToArray()
                : new Dictionary<string, string>[] { };
            allTagList[3] = members.StbBeams != null
                ? members.StbBeams
                    .Select(column => column.name)
                    .Select(s => new Dictionary<string, string> { { "name", s } }).ToArray()
                : new Dictionary<string, string>[] { };
            allTagList[4] = members.StbBraces != null
                ? members.StbBraces
                    .Select(column => column.name)
                    .Select(s => new Dictionary<string, string> { { "name", s } }).ToArray()
                : new Dictionary<string, string>[] { };

            return allTagList;
        }
    }
}
