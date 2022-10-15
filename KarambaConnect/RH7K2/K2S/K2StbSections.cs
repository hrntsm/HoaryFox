using Karamba.CrossSections;
using Karamba.Models;

using STBDotNet.v202;

namespace KarambaConnect.K2S
{
    public static class K2StbSections
    {
        internal static StbSecBeam_RC BeamRc(int croSecId, int gNum, Model kModel)
        {
            if (kModel.crosecs[croSecId] is CroSec_Trapezoid trapezoid)
            {
                return CreateTrapezoidBeam(croSecId, gNum, trapezoid);
            }

            return CreateUnsupportedTypeBeam(croSecId, gNum);
        }

        private static StbSecBeam_RC CreateUnsupportedTypeBeam(int croSecId, int gNum)
        {
            return new StbSecBeam_RC
            {
                id = (croSecId + 1).ToString(),
                name = "G" + gNum,
                kind_beam = StbSecBeam_RCKind_beam.GIRDER,
                StbSecFigureBeam_RC = new StbSecFigureBeam_RC
                {
                    Items = new object[] { new StbSecBeam_RC_Straight { depth = 10, width = 10 } }
                },
            };
        }

        private static StbSecBeam_RC CreateTrapezoidBeam(int croSecId, int gNum, CroSec_Trapezoid trapezoid)
        {
            return new StbSecBeam_RC
            {
                id = (croSecId + 1).ToString(),
                name = "G" + gNum,
                kind_beam = StbSecBeam_RCKind_beam.GIRDER,
                StbSecFigureBeam_RC = new StbSecFigureBeam_RC
                {
                    Items = new object[] { new StbSecBeam_RC_Straight
                    {
                         depth = trapezoid._height * 1000,
                         width = trapezoid.maxWidth() * 1000
                    } }
                },
            };
        }

        internal static StbSecColumn_RC ColumnRc(int croSecId, int cNum, Model kModel)
        {
            switch (kModel.crosecs[croSecId])
            {
                case CroSec_Trapezoid trapezoid:
                    return CreateTrapezoidColumn(croSecId, cNum, trapezoid);
                case CroSec_Circle circle:
                    return CreateCircleColumn(croSecId, cNum, circle);
                default:
                    return CreateUnsupportedTypeColumn(croSecId, cNum);
            }
        }

        private static StbSecColumn_RC CreateUnsupportedTypeColumn(int croSecId, int cNum)
        {
            return new StbSecColumn_RC
            {
                id = (croSecId + 1).ToString(),
                name = "C" + cNum,
                kind_column = StbSecColumn_RCKind_column.COLUMN,
                StbSecFigureColumn_RC = new StbSecFigureColumn_RC
                {
                    Item = new StbSecColumn_RC_Rect { width_X = 10, width_Y = 10 }
                },
            };
        }

        private static StbSecColumn_RC CreateCircleColumn(int croSecId, int cNum, CroSec_Circle circle)
        {
            return new StbSecColumn_RC
            {
                id = (croSecId + 1).ToString(),
                name = "C" + cNum,
                kind_column = StbSecColumn_RCKind_column.COLUMN,
                StbSecFigureColumn_RC = new StbSecFigureColumn_RC
                {
                    Item = new StbSecColumn_RC_Circle { D = circle.getHeight() * 1000 }
                },
            };
        }

        private static StbSecColumn_RC CreateTrapezoidColumn(int croSecId, int cNum, CroSec_Trapezoid trapezoid)
        {
            return new StbSecColumn_RC
            {
                id = (croSecId + 1).ToString(),
                name = "C" + cNum,
                kind_column = StbSecColumn_RCKind_column.COLUMN,
                StbSecFigureColumn_RC = new StbSecFigureColumn_RC
                {
                    Item = new StbSecColumn_RC_Rect { width_X = trapezoid.maxWidth() * 1000, width_Y = trapezoid._height * 1000 }
                },
            };
        }

        internal static StbSecBrace_S BraceSteel(int croSecId, int vNum, Model kModel)
        {
            return new StbSecBrace_S
            {
                id = (croSecId + 1).ToString(),
                name = "V" + vNum,
                StbSecSteelFigureBrace_S = new StbSecSteelFigureBrace_S
                {
                    Items = new object[] { new StbSecSteelBrace_S_Same
                    {
                        shape = kModel.crosecs[croSecId].name,
                        strength_main = "SN400"
                    } }
                }
            };
        }

        internal static StbSecBeam_S BeamSteel(int croSecId, int gNum, Model kModel)
        {
            return new StbSecBeam_S
            {
                id = (croSecId + 1).ToString(),
                name = "G" + gNum,
                kind_beam = StbSecBeam_SKind_beam.GIRDER,
                StbSecSteelFigureBeam_S = new StbSecSteelFigureBeam_S
                {
                    Items = new object[] { new StbSecSteelBeam_S_Straight
                    {
                        shape = kModel.crosecs[croSecId].name,
                        strength_main = "SN400",
                    } }
                },
            };
        }

        internal static StbSecColumn_S ColumnSteel(int croSecId, int cNum, Model kModel)
        {
            return new StbSecColumn_S
            {
                id = (croSecId + 1).ToString(),
                name = "C" + cNum,
                kind_column = StbSecColumn_SKind_column.COLUMN,
                StbSecSteelFigureColumn_S = new StbSecSteelFigureColumn_S
                {
                    Items = new object[] { new StbSecSteelColumn_S_Same
                    {
                        shape = kModel.crosecs[croSecId].name,
                        strength_main = "SN400",
                    } }
                },
            };
        }
    }
}
