using STBDotNet.Elements.StbModel.StbSection;

namespace KarambaConnect.K2S
{
    public static class StbSection
    {
        internal static Section GetBeamRc(int croSecId, int gNum, Karamba.Models.Model kModel)
        {
            if (kModel.crosecs[croSecId] is Karamba.CrossSections.CroSec_Trapezoid trapezoid)
            {
                var beamTrapezoid = new BeamRc
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
                    BarArrangement = new RcBeamSecBarArrangement
                    {
                        SameSection = new RcBeamSecBarArrangement.Same
                        {
                            CountMainTop1st = 3,
                            CountMainBottom1st = 3,
                            CountStirrup = 2,
                            PitchStirrup = 100
                        }
                    }
                };
                return beamTrapezoid;
            }
            else
            {
                var unsupported = new BeamRc
                {
                    Id = croSecId + 1,
                    Name = "G" + gNum++,
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
                    BarArrangement = new RcBeamSecBarArrangement
                    {
                        SameSection = new RcBeamSecBarArrangement.Same
                        {
                            CountMainTop1st = 2,
                            CountMainBottom1st = 3,
                            CountStirrup = 2,
                            PitchStirrup = 100
                        }
                    }
                };

                return unsupported;
            }
        }

        internal static Section GetBraceSt(int croSecId, int vNum, Karamba.Models.Model kModel)
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

        internal static Section GetColumnRc(int croSecId, int cNum, Karamba.Models.Model kModel)
        {
            switch (kModel.crosecs[croSecId])
            {
                case Karamba.CrossSections.CroSec_Trapezoid trapezoid:
                    var colTrape = new ColumnRc
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
                        BarArrangement = new RcColumnSecBarArrangement
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
                        }
                    };
                    return colTrape;
                case Karamba.CrossSections.CroSec_Circle circle:
                    var colCircle = new ColumnRc
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
                    return colCircle;
                default:
                    var unsupported = new ColumnRc
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
                        BarArrangement = new RcColumnSecBarArrangement
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
                        }
                    };
                    return unsupported;
            }
        }

        internal static Section GetBeamSt(int croSecId, int gNum, Karamba.Models.Model kModel)
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

        internal static Section GetColumnSt(int croSecId, int cNum, Karamba.Models.Model kModel)
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