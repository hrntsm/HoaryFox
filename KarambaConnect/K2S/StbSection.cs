using Karamba.CrossSections;
using Karamba.Models;
using STBDotNet.Elements.StbModel.StbSection;

namespace KarambaConnect.K2S
{
    public static class StbSection
    {
        internal static Section GetBeamRc(int croSecId, int gNum, Model kModel)
        {
            if (kModel.crosecs[croSecId] is CroSec_Trapezoid trapezoid)
            {
                return CreateTrapezoidBeam(croSecId, gNum, trapezoid);
            }

            return CreateUnsupportedTypeBeam(croSecId, gNum);
        }

        private static Section CreateUnsupportedTypeBeam(int croSecId, int gNum)
        {
            return new BeamRc
            {
                Id = croSecId + 1,
                Name = "G" + gNum,
                DBarMain = "D22",
                DStirrup = "D10",
                Figure = new RcBeamSecFigure
                {
                    SecStraight = new RcBeamSecFigure.Straight
                    {
                        Depth = 10,
                        Width = 10
                    }
                },
                BarArrangement = UndefinedBeamArrangement()
            };
        }

        private static Section CreateTrapezoidBeam(int croSecId, int gNum, CroSec_Trapezoid trapezoid)
        {
            return new BeamRc
            {
                Id = croSecId + 1,
                Name = "G" + gNum,
                DBarMain = "D22",
                DStirrup = "D10",
                Figure = new RcBeamSecFigure
                {
                    SecStraight = new RcBeamSecFigure.Straight
                    {
                        Depth = trapezoid._height * 1000,
                        Width = trapezoid.maxWidth() * 1000
                    }
                },
                BarArrangement = UndefinedBeamArrangement()
            };
        }

        private static RcBeamSecBarArrangement UndefinedBeamArrangement()
        {
            return new RcBeamSecBarArrangement
            {
                SameSection = new RcBeamSecBarArrangement.Same
                {
                    CountMainTop1st = 3,
                    CountMainBottom1st = 3,
                    CountStirrup = 2,
                    PitchStirrup = 100
                }
            };
        }

        internal static Section GetColumnRc(int croSecId, int cNum, Model kModel)
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

        private static Section CreateUnsupportedTypeColumn(int croSecId, int cNum)
        {
            return new ColumnRc
            {
                Id = croSecId + 1,
                Name = "C" + cNum,
                DBarMain = "D22",
                DBarBand = "D10",
                Figure = new RcColumnSecFigure
                {
                    SecRect = new RcColumnSecFigure.Rectangle
                    {
                        DX = 10,
                        DY = 10
                    }
                },
                BarArrangement = UndefinedColumnArrangement()
            };
        }

        private static RcColumnSecBarArrangement UndefinedColumnArrangement()
        {
            return new RcColumnSecBarArrangement
            {
                RectSameSection = new RcColumnSecBarArrangement.RectSame
                {
                    CountMainX1st = 2,
                    CountMainY1st = 2,
                    CountMainTotal = 4,
                    PitchBand = 100,
                    CountBandDirX = 2,
                    CountBandDirY = 2,
                }
            };
        }

        private static Section CreateCircleColumn(int croSecId, int cNum, CroSec_Circle circle)
        {
            return new ColumnRc
            {
                Id = croSecId,
                Name = "C" + cNum,
                DBarMain = "D22",
                DBarBand = "D10",
                Figure = new RcColumnSecFigure
                {
                    SecCircle = new RcColumnSecFigure.Circle
                    {
                        D = circle.getHeight() * 1000
                    }
                },
                BarArrangement = new RcColumnSecBarArrangement
                {
                    CircleSameSection = new RcColumnSecBarArrangement.CircleSame
                    {
                        CountMain = 6,
                        PitchBand = 100
                    }
                }
            };
        }

        private static Section CreateTrapezoidColumn(int croSecId, int cNum, CroSec_Trapezoid trapezoid)
        {
            return new ColumnRc
            {
                Id = croSecId + 1,
                Name = "C" + cNum,
                DBarMain = "D22",
                DBarBand = "D10",
                Figure = new RcColumnSecFigure
                {
                    SecRect = new RcColumnSecFigure.Rectangle
                    {
                        DX = trapezoid.maxWidth() * 1000,
                        DY = trapezoid._height * 1000
                    }
                },
                BarArrangement = UndefinedColumnArrangement()
            };
        }

        internal static Section GetBraceSt(int croSecId, int vNum, Model kModel)
        {
            return new BraceS
            {
                Id = croSecId + 1,
                Name = "V" + vNum,
                SteelBrace = new[]
                {
                    new SecSteel
                    {
                        Position = "ALL",
                        Shape = kModel.crosecs[croSecId].name,
                        StrengthMain = "SN400",
                        StrengthWeb = "SN400"
                    }
                }
            };
        }

        internal static Section GetBeamSt(int croSecId, int gNum, Model kModel)
        {
            return new BeamS
            {
                Id = croSecId + 1,
                Name = "G" + gNum,
                SteelBeams = new[]
                {
                    new SecSteel
                    {
                        Position = "ALL",
                        Shape = kModel.crosecs[croSecId].name,
                        StrengthMain = "SN400",
                        StrengthWeb = "SN400"
                    }
                }
            };
        }

        internal static Section GetColumnSt(int croSecId, int cNum, Model kModel)
        {
            return new ColumnS
            {
                Id = croSecId + 1,
                Name = "C" + cNum,
                SteelColumn = new[]
                {
                    new SecSteel
                    {
                        Position = "ALL",
                        Shape = kModel.crosecs[croSecId].name,
                        StrengthMain = "SN400",
                        StrengthWeb = "SN400"
                    }
                }
            };
        }
    }
}
