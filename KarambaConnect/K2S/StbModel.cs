using System;
using System.Collections.Generic;
using System.Linq;
using Karamba.Elements;
using Karamba.GHopper.Geometry;
using KCroSec = Karamba.CrossSections;
using Rhino.Geometry;
using STBDotNet.Elements.StbModel.StbMember;
using STBDotNet.Elements.StbModel.StbSection;
using Model = STBDotNet.Elements.StbModel.Model;

namespace KarambaConnect.K2S
{
    public static class StbModel
    {
        public static Model Set(Karamba.Models.Model kModel)
        {
            List<string> croSec = kModel.crosecs.Select(sec => sec.name).ToList();
            var vNum = 1;
            var cNum = 1;
            var gNum = 1;
            // 0: brace, 1:column, 2:girder
            var registeredCroSecId = new List<List<int>> {new List<int>(), new List<int>(), new List<int>()};
            var registeredCroSecName = new List<List<string>> {new List<string>(), new List<string>(), new List<string>()};
            var members = new Members
            {
                Columns = new List<Column>(),
                Girders = new List<Girder>(),
                Braces = new List<Brace>()
            };
            var sections = new List<Section>();
            var steelSec = new Steel();
            var rollTs = new List<RollT>();
            var rollHs = new List<RollH>();
            var rollBoxes = new List<RollBox>();
            var pipes = new List<Pipe>();
            var flatBars = new List<FlatBar>();

            foreach (ModelElement elem in kModel.elems)
            {
                if (elem.node_inds.Count != 2)
                {
                    continue;
                }

                Karamba.Nodes.Node[] node =
                {
                    kModel.nodes[elem.node_inds[0]],
                    kModel.nodes[elem.node_inds[1]]
                };

                int croSecId = croSec.IndexOf(elem.crosec.name);

                var orientation = new Vector3d(node[1].pos.Convert() - node[0].pos.Convert());
                double angle = Vector3d.VectorAngle(orientation, Vector3d.ZAxis);

                if (typeof(ModelTruss) == elem.GetType())
                {
                    var brace = new Brace
                    {
                        Id = elem.ind + 1,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0] + 1,
                        IdNodeEnd = elem.node_inds[1] + 1,
                        Rotate = 0d,
                        IdSection = croSecId + 1,
                        Kind =  "Steel"
                    };

                    members.Braces.Add(brace);

                    if (registeredCroSecId[0].IndexOf(croSecId) < 0)
                    {
                        var sec = new BraceS
                        {
                            Id = croSecId + 1,
                            Name = "V" + vNum++,
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
                        sections.Add(sec);
                        registeredCroSecId[0].Add(croSecId);

                        if (registeredCroSecName[0].IndexOf(kModel.crosecs[croSecId].name) < 0)
                        {
                            switch (kModel.crosecs[croSecId])
                            {
                                case KCroSec.CroSec_Trapezoid secTrapezoid:
                                    var trapezoid = new FlatBar
                                    {
                                        Name = secTrapezoid.name,
                                        B = secTrapezoid._height * 1000,
                                        T = secTrapezoid.uf_width * 1000
                                    };
                                    flatBars.Add(trapezoid);
                                    break;
                                case KCroSec.CroSec_Box secBox:
                                    double[] thickness = { secBox.w_thick, secBox.uf_thick, secBox.lf_thick};
                                    var box = new RollBox
                                    {
                                        Name = secBox.name,
                                        A = secBox._height * 1000,
                                        B = secBox.maxWidth() * 1000,
                                        R = secBox.fillet_r * 1000,
                                        T = thickness.Max() * 1000,
                                        Type = "ELSE"
                                    };
                                    rollBoxes.Add(box);
                                    break;
                                case KCroSec.CroSec_T secT:
                                    var tShape = new RollT
                                    {
                                        Name = secT.name,
                                        A = secT._height * 1000,
                                        B = secT.maxWidth() * 1000,
                                        R = secT.fillet_r * 1000,
                                        T1 = secT.w_thick * 1000,
                                        T2 = secT.uf_thick * 1000,
                                        Type = "T"
                                    };
                                    rollTs.Add(tShape);
                                    break;
                                case KCroSec.CroSec_I secH:
                                    var hShape = new RollH
                                    {
                                        Name = secH.name,
                                        A = secH._height * 1000,
                                        B = secH.maxWidth() * 1000,
                                        R = secH.fillet_r * 1000,
                                        T1 = secH.w_thick * 1000,
                                        T2 = secH.uf_thick * 1000,
                                        Type = "H"
                                    };
                                    rollHs.Add(hShape);
                                    break;
                                case KCroSec.CroSec_Circle secCircle:
                                    var pipe = new Pipe
                                    {
                                        Name = secCircle.name,
                                        D = secCircle.getHeight() * 1000,
                                        T = secCircle.thick * 1000
                                    };
                                    pipes.Add(pipe);
                                    break;
                                default:
                                    var unsupported = new Pipe
                                    {
                                        Name = kModel.crosecs[croSecId].name,
                                        D = 10,
                                        T = 1
                                    };
                                    pipes.Add(unsupported);
                                    break;
                            }
                            registeredCroSecName[0].Add(kModel.crosecs[croSecId].name);
                        }
                    }
                }
                else if (angle < Math.PI / 4d && angle > - Math.PI / 4d)
                {
                    var column = new Column
                    {
                        Id = elem.ind + 1,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0] + 1,
                        IdNodeEnd = elem.node_inds[1] + 1,
                        Rotate = 0d,
                        IdSection = croSecId + 1,
                    };
                    switch (elem.crosec.material.family)
                    {
                        case "Steel":
                            column.Kind = "S";
                            break;
                        case "Concrete":
                            column.Kind = "RC";
                            break;
                        default:
                            column.Kind = "";
                            break;
                    }
                    members.Columns.Add(column);
                    
                    if (registeredCroSecId[1].IndexOf(croSecId) < 0)
                    {
                        switch (column.Kind)
                        {
                            case "S":
                                var colS = new ColumnS
                                {
                                    Id = croSecId + 1,
                                    Name = "C" + cNum++,
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
                                sections.Add(colS);
                                
                                if (registeredCroSecName[1].IndexOf(kModel.crosecs[croSecId].name) < 0)
                                {
                                    switch (kModel.crosecs[croSecId])
                                    {
                                        case KCroSec.CroSec_Box secBox:
                                            var box = new RollBox
                                            {
                                                Name = secBox.name,
                                                A = secBox._height * 1000,
                                                B = secBox.maxWidth() * 1000,
                                                R = secBox.fillet_r * 1000,
                                                T = secBox.w_thick * 1000,
                                                Type = "ELSE"
                                            };
                                            rollBoxes.Add(box);
                                            break;
                                        case KCroSec.CroSec_T secT:
                                            var tShape = new RollT
                                            {
                                                Name = secT.name,
                                                A = secT._height * 1000,
                                                B = secT.maxWidth() * 1000,
                                                R = secT.fillet_r * 1000,
                                                T1 = secT.w_thick * 1000,
                                                T2 = secT.uf_thick * 1000,
                                                Type = "T"
                                            };
                                            rollTs.Add(tShape);
                                            break;
                                        case KCroSec.CroSec_I secH:
                                            var hShape = new RollH
                                            {
                                                Name = secH.name,
                                                A = secH._height * 1000,
                                                B = secH.maxWidth() * 1000,
                                                R = secH.fillet_r * 1000,
                                                T1 = secH.w_thick * 1000,
                                                T2 = secH.uf_thick * 1000,
                                                Type = "H"
                                            };
                                            rollHs.Add(hShape);
                                            break;
                                        case KCroSec.CroSec_Circle secCircle:
                                            var pipe = new Pipe
                                            {
                                                Name = secCircle.name,
                                                D = secCircle.getHeight() * 1000,
                                                T = secCircle.thick * 1000
                                            };
                                            pipes.Add(pipe);
                                            break;
                                        default:
                                            var unsupported = new Pipe
                                            {
                                                Name = kModel.crosecs[croSecId].name,
                                                D = 10,
                                                T = 1
                                            };
                                            pipes.Add(unsupported);
                                            break;
                                    }
                                    registeredCroSecName[1].Add(kModel.crosecs[croSecId].name);
                                }
                                break;
                            case "RC":
                                switch (kModel.crosecs[croSecId])
                                {
                                    case KCroSec.CroSec_Trapezoid trapezoid:
                                        var colTrape = new ColumnRc
                                        {
                                            Id = croSecId + 1,
                                            Name = "C" + cNum++,
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
                                        sections.Add(colTrape);
                                        break;
                                    case KCroSec.CroSec_Circle circle:
                                        var colCircle = new ColumnRc
                                        {
                                            Id = croSecId,
                                            Name = "C" + cNum++,
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
                                        sections.Add(colCircle);
                                        break;
                                    default:
                                        var unsupported = new ColumnRc
                                        {
                                            Id = croSecId + 1,
                                            Name = "C" + cNum++,
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
                                        sections.Add(unsupported);
                                        break;
                                }
                                break;
                            default:
                                throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
                        }
                        registeredCroSecId[1].Add(croSecId);
                    }
                }
                else
                {
                    var beam = new Girder
                    {
                        Id = elem.ind + 1,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0] + 1,
                        IdNodeEnd = elem.node_inds[1] + 1,
                        Rotate = 0d,
                        IdSection = croSecId + 1,
                        IsFoundation = "false",
                    };
                    switch (elem.crosec.material.family)
                    {
                        case "Steel":
                            beam.Kind = "S";
                            break;
                        case "Concrete":
                            beam.Kind = "RC";
                            break;
                        default:
                            beam.Kind = "";
                            break;
                    }
                    members.Girders.Add(beam);

                    if (registeredCroSecId[2].IndexOf(croSecId) < 0)
                    {
                        switch (beam.Kind)
                        {
                            case "S":
                                var beamS = new BeamS
                                {
                                    Id = croSecId + 1,
                                    Name = "G" + gNum++,
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
                                sections.Add(beamS);

                                if (registeredCroSecName[2].IndexOf(kModel.crosecs[croSecId].name) < 0)
                                {
                                    switch (kModel.crosecs[croSecId])
                                    {
                                        case KCroSec.CroSec_Box secBox:
                                            var box = new RollBox
                                            {
                                                Name = secBox.name,
                                                A = secBox._height * 1000,
                                                B = secBox.maxWidth() * 1000,
                                                R = secBox.fillet_r * 1000,
                                                T = secBox.w_thick * 1000,
                                                Type = "ELSE"
                                            };
                                            rollBoxes.Add(box);
                                            break;
                                        case KCroSec.CroSec_T secT:
                                            var tShape = new RollT
                                            {
                                                Name = secT.name,
                                                A = secT._height * 1000,
                                                B = secT.maxWidth() * 1000,
                                                R = secT.fillet_r * 1000,
                                                T1 = secT.w_thick * 1000,
                                                T2 = secT.uf_thick * 1000,
                                                Type = "T"
                                            };
                                            rollTs.Add(tShape);
                                            break;
                                        case KCroSec.CroSec_I secH:
                                            var hShape = new RollH
                                            {
                                                Name = secH.name,
                                                A = secH._height * 1000,
                                                B = secH.maxWidth() * 1000,
                                                R = secH.fillet_r * 1000,
                                                T1 = secH.w_thick * 1000,
                                                T2 = secH.uf_thick * 1000,
                                                Type = "H"
                                            };
                                            rollHs.Add(hShape);
                                            break;
                                        case KCroSec.CroSec_Circle secCircle:
                                            var pipe = new Pipe
                                            {
                                                Name = secCircle.name,
                                                D = secCircle.getHeight() * 1000,
                                                T = secCircle.thick * 1000
                                            };
                                            pipes.Add(pipe);
                                            break;
                                        default:
                                            var unsupported = new Pipe
                                            {
                                                Name = kModel.crosecs[croSecId].name,
                                                D = 10,
                                                T = 1
                                            };
                                            pipes.Add(unsupported);
                                            break;
                                    }
                                    registeredCroSecName[2].Add(kModel.crosecs[croSecId].name);
                                }
                                break;
                            case "RC":
                                if (kModel.crosecs[croSecId] is KCroSec.CroSec_Trapezoid trapezoid)
                                {
                                    var beamTrapezoid = new BeamRc
                                    {
                                        Id = croSecId + 1,
                                        Name = "G" + gNum++,
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
                                    sections.Add(beamTrapezoid);
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
                                    sections.Add(unsupported);
                                }

                                break;
                            default:
                                throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
                        }
                        registeredCroSecId[2].Add(croSecId);
                    }
                }
            }

            if (rollTs.Count > 0)
            {
                steelSec.RollT = rollTs;
            }

            if (rollHs.Count > 0)
            {
                steelSec.RollH = rollHs;
            }

            if (rollBoxes.Count > 0)
            {
                steelSec.RollBox = rollBoxes;
            }

            if (pipes.Count > 0)
            {
                steelSec.Pipe = pipes;
            }

            if (flatBars.Count > 0)
            {
                steelSec.FlatBar = flatBars;
            }
            sections.Add(steelSec);

            var sModel = new Model
            {
                Nodes = kModel.nodes.ToStb(),
                Members = members,
                Sections = sections
            };

            return sModel;
        }
    }
}