using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Karamba.CrossSections;
using Karamba.Materials;
using STBDotNet.v202;

namespace KarambaConnect.S2K
{
    public static class CrossSection
    {
        public static List<string>[] GetIndex(ST_BRIDGE stBridge)
        {
            var k3dIds = new[] { new List<string>(), new List<string>(), new List<string>() };
            StbMembers members = stBridge.StbModel.StbMembers;

            if (members.StbColumns != null)
            {
                k3dIds[0].AddRange(members.StbColumns.Select(column => "Id" + column.id_section));
            }
            if (members.StbGirders != null)
            {
                k3dIds[1].AddRange(members.StbGirders.Select(girder => "Id" + girder.id_section));
            }
            if (members.StbBraces != null)
            {
                k3dIds[2].AddRange(members.StbBraces.Select(brace => "Id" + brace.id_section));
            }

            return k3dIds;
        }

        public static List<CroSec> GetCroSec(StbSections sections, CroSecFamilyName familyName)
        {
            // TODO: 材軸の回転は未設定
            var k3dCroSec = new List<CroSec>();
#if karamba1
            var sn400 = new FemMaterial_Isotrop("Steel", "SN400", 20500_0000, 8076_0000, 8076_0000, 78.5, 23_5000, 1.20E-05, Color.Brown);
#elif karamba2
            var sn400 = new FemMaterial_Isotrop("Steel", "SN400", 20500_0000, 8076_0000, 8076_0000, 78.5, 23_5000, 23_5000, FemMaterial.FlowHypothesis.mises, 1.20E-05, Color.Brown);
#endif

            k3dCroSec.AddRange(StbSecColumnRcToK3dCroSec(sections.StbSecColumn_RC));
            k3dCroSec.AddRange(StbSecBeamRcToK3dCroSec(sections.StbSecBeam_RC));
            k3dCroSec.AddRange(StbSecSteelToK3dCroSec(sections, sn400, familyName));

            return k3dCroSec;
        }

        private static List<CroSec> StbSecColumnRcToK3dCroSec(IEnumerable<StbSecColumn_RC> columns)
        {
            var k3dCroSecList = new List<CroSec>();
            if (columns == null)
            {
                return k3dCroSecList;
            }

            foreach (StbSecColumn_RC column in columns)
            {
                string name;
                CroSec_Beam k3dCroSec;
                object figure = column.StbSecFigureColumn_RC.Item;
                FemMaterial_Isotrop material = Material.StbToRcFemMaterial(column.strength_concrete);
                switch (figure)
                {
                    case StbSecColumn_RC_Rect rect:
                        double widthX = rect.width_X / 10d;
                        double widthY = rect.width_Y / 10d;
                        name = $"CD-{widthX * 10}x{widthY * 10}";
                        k3dCroSec = new CroSec_Trapezoid("RcColRect", name, null, null, material, widthX, widthY, widthY);
                        break;
                    case StbSecColumn_RC_Circle circle:
                        double d = circle.D / 10d;
                        name = $"P-{d * 10}";
                        k3dCroSec = new CroSec_Circle("RcColCircle", name, null, null, material, d, d / 2);
                        break;
                    default:
                        throw new ArgumentException("Convert StbSecColumn_RC to karamba3d error");
                }
                k3dCroSec.AddElemId("Id" + column.id);
                k3dCroSecList.Add(k3dCroSec);
            }

            return k3dCroSecList;
        }

        private static List<CroSec> StbSecBeamRcToK3dCroSec(IEnumerable<StbSecBeam_RC> girders)
        {
            var k3dCroSecList = new List<CroSec>();
            if (girders == null)
            {
                return k3dCroSecList;
            }

            foreach (StbSecBeam_RC girder in girders)
            {
                double width, depth;
                FemMaterial_Isotrop material = Material.StbToRcFemMaterial(girder.strength_concrete);
                object[] figures = girder.StbSecFigureBeam_RC.Items;

                switch (figures[0])
                {
                    case StbSecBeam_RC_Straight straight:
                        width = straight.width / 10d;
                        depth = straight.depth / 10d;
                        break;
                    case StbSecBeam_RC_Taper _:
                        StbSecBeam_RC_Taper[] tapers = { figures[0] as StbSecBeam_RC_Taper, figures[1] as StbSecBeam_RC_Taper };
                        width = tapers.First(figure => figure.pos == StbSecBeam_RC_TaperPos.START).width / 10d;
                        depth = tapers.First(figure => figure.pos == StbSecBeam_RC_TaperPos.START).depth / 10d;
                        break;
                    case StbSecBeam_RC_Haunch _:
                        StbSecBeam_RC_Haunch[] haunches;
                        haunches = figures.Length == 2
                            ? new[] { figures[0] as StbSecBeam_RC_Haunch, figures[1] as StbSecBeam_RC_Haunch }
                            : new[] { figures[0] as StbSecBeam_RC_Haunch, figures[1] as StbSecBeam_RC_Haunch, figures[2] as StbSecBeam_RC_Haunch };
                        width = haunches.First(figure => figure.pos == StbSecBeam_RC_HaunchPos.CENTER).width / 10d;
                        depth = haunches.First(figure => figure.pos == StbSecBeam_RC_HaunchPos.CENTER).depth / 10d;
                        break;
                    default:
                        throw new ArgumentException("Convert StbSecBeam_RC to karamba3d error");
                }
                var name = $"BD-{width * 10}x{depth * 10}";
                var k3dCroSec = new CroSec_Trapezoid("RcBeam", name, null, null, material, depth, width, width);
                k3dCroSec.AddElemId("Id" + girder.id);
                k3dCroSecList.Add(k3dCroSec);
            }

            return k3dCroSecList;
        }

        private static List<CroSec> StbSecSteelToK3dCroSec(StbSections sections, FemMaterial material, CroSecFamilyName familyName)
        {
            var k3dCroSecList = new List<CroSec>();
            if (sections.StbSecSteel == null)
            {
                return k3dCroSecList;
            }

            SetRollHSection(sections, material, familyName, k3dCroSecList);
            SetBuildHSection(sections, material, familyName, k3dCroSecList);
            SetRollBoxSection(sections, material, familyName, k3dCroSecList);
            SetBuildBoxSection(sections, material, familyName, k3dCroSecList);
            SetRollTSection(sections, material, familyName, k3dCroSecList);
            SetPipeSection(sections, material, familyName, k3dCroSecList);
            SetRollCSection(sections, material, familyName, k3dCroSecList);
            SetRollLSection(sections, material, familyName, k3dCroSecList);
            SetLipCSection(sections, material, familyName, k3dCroSecList);
            SetFlatBarSection(sections, material, familyName, k3dCroSecList);
            SetRoundBarSection(sections, material, familyName, k3dCroSecList);

            return k3dCroSecList;
        }

        private static void SetRollHSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecRollH != null)
            {
                foreach (StbSecRollH rollH in sections.StbSecSteel.StbSecRollH)
                {
                    var k3dCroSec = new CroSec_I(familyName.H, rollH.name, null, null, material,
                        rollH.A / 10d, rollH.B / 10d, rollH.B / 10d,
                        rollH.t2 / 10d, rollH.t2 / 10d, rollH.t1 / 10d, rollH.r / 10d, rollH.r / 10d);
                    SetK3dCroSecElemId(sections, k3dCroSec, rollH.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetBuildHSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecBuildH != null)
            {
                foreach (StbSecBuildH buildH in sections.StbSecSteel.StbSecBuildH)
                {
                    var k3dCroSec = new CroSec_I(familyName.H, buildH.name, null, null, material,
                        buildH.A / 10d, buildH.B / 10d, buildH.B / 10d,
                        buildH.t2 / 10d, buildH.t2 / 10d, buildH.t1 / 10d);
                    SetK3dCroSecElemId(sections, k3dCroSec, buildH.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetRollBoxSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecRollBOX != null)
            {
                foreach (StbSecRollBOX rollBOX in sections.StbSecSteel.StbSecRollBOX)
                {
                    var k3dCroSec = new CroSec_Box(familyName.Box, rollBOX.name, null, null, material,
                        rollBOX.A / 10d, rollBOX.B / 10d, rollBOX.B / 10d,
                        rollBOX.t / 10d, rollBOX.t / 10d, rollBOX.t / 10d, rollBOX.r / 10d, rollBOX.r / 10d);
                    SetK3dCroSecElemId(sections, k3dCroSec, rollBOX.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetBuildBoxSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecBuildBOX != null)
            {
                foreach (StbSecBuildBOX buildBOX in sections.StbSecSteel.StbSecBuildBOX)
                {
                    var k3dCroSec = new CroSec_Box(familyName.Box, buildBOX.name, null, null, material,
                        buildBOX.A / 10d, buildBOX.B / 10d, buildBOX.B / 10d,
                        buildBOX.t2 / 10d, buildBOX.t2 / 10d, buildBOX.t1 / 10d, 0, -1);
                    SetK3dCroSecElemId(sections, k3dCroSec, buildBOX.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetRollTSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecRollT != null)
            {
                foreach (StbSecRollT rollT in sections.StbSecSteel.StbSecRollT)
                {
                    var k3dCroSec = new CroSec_T(familyName.T, rollT.name, null, null, material,
                        rollT.A / 10d, rollT.B / 10d,
                        rollT.t2 / 10d, rollT.t1 / 10d, rollT.r / 10d, rollT.r / 10d);
                    SetK3dCroSecElemId(sections, k3dCroSec, rollT.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetPipeSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecPipe != null)
            {
                foreach (StbSecPipe pipe in sections.StbSecSteel.StbSecPipe)
                {
                    var k3dCroSec = new CroSec_Circle(familyName.Circle, pipe.name, null, null, material,
                        pipe.D / 10d, pipe.t / 10d);
                    SetK3dCroSecElemId(sections, k3dCroSec, pipe.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetRollCSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecRollC != null)
            {
                foreach (StbSecRollC rollC in sections.StbSecSteel.StbSecRollC)
                {
                    // TODO: 正確な形状に合わせて等価断面計算する。フランジとウェブの角度が95°なのとフィレットは非考慮
                    int typeFactor = rollC.type == StbSecRollCType.SINGLE ? 1 : 2;
                    var eqLength = Math.Sqrt((rollC.A * rollC.t1 + 2 * rollC.B * rollC.t2 - 2 * rollC.t1 * rollC.t2) * typeFactor) / 10d;
                    var k3dCroSec = new CroSec_Trapezoid(familyName.Circle, rollC.name, null, null, material, eqLength, eqLength, eqLength);
                    SetK3dCroSecElemId(sections, k3dCroSec, rollC.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetRollLSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecRollL != null)
            {
                foreach (StbSecRollL rollL in sections.StbSecSteel.StbSecRollL)
                {
                    // TODO: 正確な形状に合わせて等価断面計算する。フィレットは非考慮
                    int typeFactor = rollL.type == StbSecRollLType.SINGLE ? 1 : 2;
                    var eqLength = Math.Sqrt((rollL.A * rollL.t1 + rollL.B * rollL.t2 - rollL.t1 * rollL.t2) * typeFactor) / 10d;
                    var k3dCroSec = new CroSec_Trapezoid(familyName.Circle, rollL.name, null, null, material, eqLength, eqLength, eqLength);
                    SetK3dCroSecElemId(sections, k3dCroSec, rollL.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetLipCSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecLipC != null)
            {
                foreach (StbSecLipC lipC in sections.StbSecSteel.StbSecLipC)
                {
                    // TODO: 正確な形状に合わせて等価断面計算する。フィレットは非考慮
                    int typeFactor = lipC.type == StbSecLipCType.SINGLE ? 1 : 2;
                    var eqLength = Math.Sqrt((2 * lipC.A * lipC.t + lipC.H * lipC.t + 2 * lipC.C * lipC.t - 4 * lipC.t * lipC.t) * typeFactor) / 10d;
                    var k3dCroSec = new CroSec_Trapezoid(familyName.Circle, lipC.name, null, null, material, eqLength, eqLength, eqLength);
                    SetK3dCroSecElemId(sections, k3dCroSec, lipC.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetFlatBarSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecFlatBar != null)
            {
                foreach (StbSecFlatBar flatBar in sections.StbSecSteel.StbSecFlatBar)
                {
                    var k3dCroSec = new CroSec_Trapezoid(familyName.FB, flatBar.name, null, null, material, flatBar.B, flatBar.t, flatBar.t);
                    SetK3dCroSecElemId(sections, k3dCroSec, flatBar.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetRoundBarSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, ICollection<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecRoundBar != null)
            {
                foreach (StbSecRoundBar roundBar in sections.StbSecSteel.StbSecRoundBar)
                {
                    // TODO: Karambaは中実円断面ないため、矩形の等価断面。
                    double eqLength = Math.Sqrt(roundBar.R * roundBar.R * Math.PI) / 10d;
                    var k3dCroSec = new CroSec_Trapezoid(familyName.Circle, roundBar.name, null, null, material, eqLength, eqLength, eqLength);
                    SetK3dCroSecElemId(sections, k3dCroSec, roundBar.name);
                    k3dCroSecList.Add(k3dCroSec);
                }
            }
        }

        private static void SetK3dCroSecElemId(StbSections sections, CroSec k3dCroSec, string steelShapeName)
        {
            CheckSecColumnIdMatching(sections.StbSecColumn_S, k3dCroSec, steelShapeName);
            CheckSecBeamIdMatching(sections.StbSecBeam_S, k3dCroSec, steelShapeName);
            CheckSecBraceIdMatching(sections.StbSecBrace_S, k3dCroSec, steelShapeName);
        }

        private static void CheckSecColumnIdMatching(IEnumerable<StbSecColumn_S> columns, CroSec k3dCroSec, string steelShapeName)
        {
            if (columns == null)
            {
                return;
            }
            foreach (StbSecColumn_S column in columns)
            {
                var memberFigureName = string.Empty;
                object[] figures = column.StbSecSteelFigureColumn_S.Items;
                switch (figures[0])
                {
                    case StbSecSteelColumn_S_Same same:
                        memberFigureName = same.shape;
                        break;
                    case StbSecSteelColumn_S_NotSame _:
                        memberFigureName = new[] { figures[0] as StbSecSteelColumn_S_NotSame, figures[1] as StbSecSteelColumn_S_NotSame }
                                .First(figure => figure.pos == StbSecSteelColumn_S_NotSamePos.BOTTOM).shape;
                        break;
                    case StbSecSteelColumn_S_ThreeTypes _:
                        memberFigureName = new[] { figures[0] as StbSecSteelColumn_S_ThreeTypes, figures[1] as StbSecSteelColumn_S_ThreeTypes, figures[2] as StbSecSteelColumn_S_ThreeTypes }
                                .First(figure => figure.pos == StbSecSteelColumn_S_ThreeTypesPos.CENTER).shape;
                        break;
                }
                if (memberFigureName == steelShapeName)
                {
                    k3dCroSec.AddElemId("Id" + column.id);
                }
            }
        }

        private static void CheckSecBeamIdMatching(IEnumerable<StbSecBeam_S> beams, CroSec k3dCroSec, string steelShapeName)
        {
            if (beams == null)
            {
                return;
            }
            foreach (StbSecBeam_S beam in beams)
            {
                var memberFigureName = string.Empty;
                object[] figures = beam.StbSecSteelFigureBeam_S.Items;
                switch (figures[0])
                {
                    case StbSecSteelBeam_S_Straight straight:
                        memberFigureName = straight.shape;
                        break;
                    case StbSecSteelBeam_S_Taper _:
                        memberFigureName = new[] { figures[0] as StbSecSteelBeam_S_Taper, figures[1] as StbSecSteelBeam_S_Taper }
                            .First(figure => figure.pos == StbSecSteelBeam_S_TaperPos.START).shape;
                        break;
                    case StbSecSteelBeam_S_Joint _:
                        var joints = figures.Length == 2
                             ? new[] { figures[0] as StbSecSteelBeam_S_Joint, figures[1] as StbSecSteelBeam_S_Joint }
                             : new[] { figures[0] as StbSecSteelBeam_S_Joint, figures[1] as StbSecSteelBeam_S_Joint, figures[2] as StbSecSteelBeam_S_Joint };
                        memberFigureName = joints.First(figure => figure.pos == StbSecSteelBeam_S_JointPos.CENTER).shape;
                        break;
                    case StbSecSteelBeam_S_Haunch _:
                        var haunch = figures.Length == 2
                             ? new[] { figures[0] as StbSecSteelBeam_S_Haunch, figures[1] as StbSecSteelBeam_S_Haunch }
                             : new[] { figures[0] as StbSecSteelBeam_S_Haunch, figures[1] as StbSecSteelBeam_S_Haunch, figures[2] as StbSecSteelBeam_S_Haunch };
                        memberFigureName = haunch.First(figure => figure.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                        break;
                    case StbSecSteelBeam_S_FiveTypes _:
                        StbSecSteelBeam_S_FiveTypes[] fiveTypes;
                        switch (figures.Length)
                        {
                            case 3:
                                fiveTypes = new[] { figures[0] as StbSecSteelBeam_S_FiveTypes, figures[1] as StbSecSteelBeam_S_FiveTypes, figures[2] as StbSecSteelBeam_S_FiveTypes };
                                break;
                            case 4:
                                fiveTypes = new[] { figures[0] as StbSecSteelBeam_S_FiveTypes, figures[1] as StbSecSteelBeam_S_FiveTypes, figures[2] as StbSecSteelBeam_S_FiveTypes, figures[3] as StbSecSteelBeam_S_FiveTypes };
                                break;
                            case 5:
                                fiveTypes = new[] { figures[0] as StbSecSteelBeam_S_FiveTypes, figures[1] as StbSecSteelBeam_S_FiveTypes, figures[2] as StbSecSteelBeam_S_FiveTypes, figures[3] as StbSecSteelBeam_S_FiveTypes, figures[4] as StbSecSteelBeam_S_FiveTypes };
                                break;
                            default:
                                throw new ArgumentException("StbSecSteelBeam_S_FiveTypes parse error");
                        }
                        memberFigureName = fiveTypes.First(figure => figure.pos == StbSecSteelBeam_S_FiveTypesPos.CENTER).shape;
                        break;
                }
                if (memberFigureName == steelShapeName)
                {
                    k3dCroSec.AddElemId("Id" + beam.id);
                }
            }
        }

        private static void CheckSecBraceIdMatching(IEnumerable<StbSecBrace_S> braces, CroSec k3dCroSec, string steelShapeName)
        {
            if (braces == null)
            {
                return;
            }
            foreach (StbSecBrace_S brace in braces)
            {
                var memberFigureName = string.Empty;
                object[] figures = brace.StbSecSteelFigureBrace_S.Items;
                switch (figures[0])
                {
                    case StbSecSteelBrace_S_Same same:
                        memberFigureName = same.shape;
                        break;
                    case StbSecSteelBrace_S_NotSame _:
                        memberFigureName = new[] { figures[0] as StbSecSteelBrace_S_NotSame, figures[1] as StbSecSteelBrace_S_NotSame }
                            .First(figure => figure.pos == StbSecSteelBrace_S_NotSamePos.BOTTOM).shape;
                        break;
                    case StbSecSteelBrace_S_ThreeTypes _:
                        memberFigureName = new[] { figures[0] as StbSecSteelBrace_S_ThreeTypes, figures[1] as StbSecSteelBrace_S_ThreeTypes, figures[2] as StbSecSteelBrace_S_ThreeTypes }
                            .First(figure => figure.pos == StbSecSteelBrace_S_ThreeTypesPos.CENTER).shape;
                        break;
                }
                if (memberFigureName == steelShapeName)
                {
                    k3dCroSec.AddElemId("Id" + brace.id);
                }
            }
        }
    }
}
