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
            var k3Ids = new[] { new List<string>(), new List<string>(), new List<string>() };
            StbMembers members = stBridge.StbModel.StbMembers;

            k3Ids[0].AddRange(members.StbColumns.Select(column => "Id" + column.id));
            k3Ids[1].AddRange(members.StbGirders.Select(girder => "Id" + girder.id));
            k3Ids[2].AddRange(members.StbBraces.Select(brace => "Id" + brace.id));

            return k3Ids;
        }

        public static List<CroSec> GetCroSec(StbSections sections, CroSecFamilyName familyName)
        {
            // TODO: 材軸の回転は未設定
            var k3CroSec = new List<CroSec>();

            var fc21 = new FemMaterial_Isotrop("Concrete", "Fc21", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray);
            var sn400 = new FemMaterial_Isotrop("Steel", "SN400", 20500_0000, 8076_0000, 8076_0000, 78.5, 235_0000, 1.20E-05, Color.Brown);

            k3CroSec.AddRange(StbSecColumnRcToK3dCroSec(sections.StbSecColumn_RC, fc21, familyName));
            k3CroSec.AddRange(StbSecBeamRcToK3dCroSec(sections.StbSecBeam_RC, fc21, familyName));
            k3CroSec.AddRange(StbSecSteelToK3dCroSec(sections, sn400, familyName));

            return k3CroSec;
        }

        private static List<CroSec> StbSecColumnRcToK3dCroSec(StbSecColumn_RC[] columns, FemMaterial material, CroSecFamilyName familyName)
        {
            var k3dCroSecList = new List<CroSec>();

            foreach (var column in columns)
            {
                string name = string.Empty;
                CroSec_Beam k3dCroSec;
                object figure = column.StbSecFigureColumn_RC.Item;
                switch (figure)
                {
                    case StbSecColumn_RC_Rect rect:
                        double widthX = rect.width_X / 10d;
                        double widthY = rect.width_Y / 10d;
                        name = $"CD-{widthX * 10}x{widthY * 10}";
                        // TODO:材料の設定は直す
                        k3dCroSec = new CroSec_Trapezoid(familyName.Box, name, null, null, material, widthX, widthY, widthY);
                        break;
                    case StbSecColumn_RC_Circle circle:
                        double d = circle.D / 10d;
                        name = $"P-{d * 10}";
                        k3dCroSec = new CroSec_Circle(familyName.Circle, name, null, null, material, d, d / 2);
                        break;
                    default:
                        throw new ArgumentException("Unknown figure type.");
                }
                k3dCroSec.AddElemId("Id" + column.id);
                k3dCroSecList.Add(k3dCroSec);
            }

            return k3dCroSecList;
        }

        private static List<CroSec> StbSecBeamRcToK3dCroSec(StbSecBeam_RC[] girders, FemMaterial material, CroSecFamilyName familyName)
        {
            var k3dCroSecList = new List<CroSec>();

            foreach (var girder in girders)
            {
                string name = string.Empty;
                double width = 0, depth = 0;
                object[] figures = girder.StbSecFigureBeam_RC.Items;

                switch (figures)
                {
                    case StbSecBeam_RC_Straight[] straights:
                        width = straights[0].width / 10d;
                        depth = straights[0].depth / 10d;
                        break;
                    case StbSecBeam_RC_Taper[] tapers:
                        width = tapers.First(figure => figure.pos == StbSecBeam_RC_TaperPos.START).width / 10d;
                        depth = tapers.First(figure => figure.pos == StbSecBeam_RC_TaperPos.START).depth / 10d;
                        break;
                    case StbSecBeam_RC_Haunch[] haunches:
                        width = haunches.First(figure => figure.pos == StbSecBeam_RC_HaunchPos.CENTER).width / 10d;
                        depth = haunches.First(figure => figure.pos == StbSecBeam_RC_HaunchPos.CENTER).depth / 10d;
                        break;
                }
                name = $"BD-{width * 10}x{depth * 10}";
                var k3dCroSec = new CroSec_Trapezoid(familyName.Box, name, null, null, material, depth, width, width);
                k3dCroSec.AddElemId("Id" + girder.id);
                k3dCroSecList.Add(k3dCroSec);
            }

            return k3dCroSecList;
        }

        private static List<CroSec> StbSecSteelToK3dCroSec(StbSections sections, FemMaterial material, CroSecFamilyName familyName)
        {
            var k3dCroSecList = new List<CroSec>();

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

        private static void SetRollHSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetBuildHSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetRollBoxSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetBuildBoxSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetRollTSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetPipeSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetRollCSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetRollLSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetLipCSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetFlatBarSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
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

        private static void SetRoundBarSection(StbSections sections, FemMaterial material, CroSecFamilyName familyName, List<CroSec> k3dCroSecList)
        {
            if (sections.StbSecSteel.StbSecRoundBar != null)
            {
                foreach (StbSecRoundBar roundBar in sections.StbSecSteel.StbSecRoundBar)
                {
                    // TODO: Karambaは中実円断面ないため、矩形の等価断面。
                    var eqLength = Math.Sqrt(roundBar.R * roundBar.R * Math.PI) / 10d;
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

        private static void CheckSecColumnIdMatching(StbSecColumn_S[] columns, CroSec k3dCroSec, string steelShapeName)
        {
            foreach (StbSecColumn_S column in columns)
            {
                var memberFigureName = string.Empty;
                object[] figures = column.StbSecSteelFigureColumn_S.Items;
                switch (figures)
                {
                    case StbSecSteelColumn_S_Same[] same:
                        memberFigureName = same[0].shape;
                        break;
                    case StbSecSteelColumn_S_NotSame[] notSame:
                        memberFigureName = notSame.First(figure => figure.pos == StbSecSteelColumn_S_NotSamePos.BOTTOM).shape;
                        break;
                    case StbSecSteelColumn_S_ThreeTypes[] three:
                        memberFigureName = three.First(figure => figure.pos == StbSecSteelColumn_S_ThreeTypesPos.CENTER).shape;
                        break;
                }
                if (memberFigureName == steelShapeName)
                {
                    k3dCroSec.AddElemId("Id" + column.id);
                }
            }
        }

        private static void CheckSecBeamIdMatching(StbSecBeam_S[] beams, CroSec k3dCroSec, string steelShapeName)
        {
            foreach (StbSecBeam_S beam in beams)
            {
                var memberFigureName = string.Empty;
                object[] figures = beam.StbSecSteelFigureBeam_S.Items;
                switch (figures)
                {
                    case StbSecSteelBeam_S_Straight[] straight:
                        memberFigureName = straight[0].shape;
                        break;
                    case StbSecSteelBeam_S_Taper[] taper:
                        memberFigureName = taper.First(figure => figure.pos == StbSecSteelBeam_S_TaperPos.START).shape;
                        break;
                    case StbSecSteelBeam_S_Joint[] joint:
                        memberFigureName = joint.First(figure => figure.pos == StbSecSteelBeam_S_JointPos.CENTER).shape;
                        break;
                    case StbSecSteelBeam_S_Haunch[] haunch:
                        memberFigureName = haunch.First(figure => figure.pos == StbSecSteelBeam_S_HaunchPos.CENTER).shape;
                        break;
                    case StbSecSteelBeam_S_FiveTypes[] fiveTypes:
                        memberFigureName = fiveTypes.First(figure => figure.pos == StbSecSteelBeam_S_FiveTypesPos.CENTER).shape;
                        break;
                }
                if (memberFigureName == steelShapeName)
                {
                    k3dCroSec.AddElemId("Id" + beam.id);
                }
            }
        }

        private static void CheckSecBraceIdMatching(StbSecBrace_S[] braces, CroSec k3dCroSec, string steelShapeName)
        {
            foreach (StbSecBrace_S brace in braces)
            {
                var memberFigureName = string.Empty;
                object[] figures = brace.StbSecSteelFigureBrace_S.Items;
                switch (figures)
                {
                    case StbSecSteelBrace_S_Same[] same:
                        memberFigureName = same[0].shape;
                        break;
                    case StbSecSteelBrace_S_NotSame[] notSame:
                        memberFigureName = notSame.First(figure => figure.pos == StbSecSteelBrace_S_NotSamePos.BOTTOM).shape;
                        break;
                    case StbSecSteelBrace_S_ThreeTypes[] three:
                        memberFigureName = three.First(figure => figure.pos == StbSecSteelBrace_S_ThreeTypesPos.CENTER).shape;
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
