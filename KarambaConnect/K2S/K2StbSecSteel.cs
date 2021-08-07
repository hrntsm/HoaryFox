using System.Collections.Generic;
using System.Linq;
using Karamba.Models;
using STBDotNet.v202;
using KCroSec = Karamba.CrossSections;

namespace KarambaConnect.K2S
{
    public static class K2StbSecSteel
    {
        internal static void GetSection(ref K2SSecSteelItems secSteel, Model kModel, int croSecId)
        {
            switch (kModel.crosecs[croSecId])
            {
                case KCroSec.CroSec_Trapezoid secTrapezoid:
                    AddTrapezoidSection(ref secSteel, secTrapezoid);
                    break;
                case KCroSec.CroSec_Box secBox:
                    AddBoxSection(ref secSteel, secBox);
                    break;
                case KCroSec.CroSec_T secT:
                    AddTShapeSection(ref secSteel, secT);
                    break;
                case KCroSec.CroSec_I secH:
                    AddHShapeSection(ref secSteel, secH);
                    break;
                case KCroSec.CroSec_Circle secCircle:
                    AddCircleSection(ref secSteel, secCircle);
                    break;
                default:
                    AddUnsupportedSection(ref secSteel, kModel, croSecId);
                    break;
            }
        }

        private static void AddUnsupportedSection(ref K2SSecSteelItems secSteel, Model kModel, int croSecId)
        {
            var unsupported = new StbSecPipe
            {
                name = kModel.crosecs[croSecId].name,
                D = 10,
                t = 1
            };
            if (secSteel.SecPipes == null)
            {
                secSteel.SecPipes = new List<StbSecPipe>();
            }

            secSteel.SecPipes.Add(unsupported);
        }

        private static void AddCircleSection(ref K2SSecSteelItems secSteel, KCroSec.CroSec_Circle secCircle)
        {
            var pipe = new StbSecPipe
            {
                name = secCircle.name,
                D = secCircle.getHeight() * 1000,
                t = secCircle.thick * 1000
            };
            if (secSteel.SecPipes == null)
            {
                secSteel.SecPipes = new List<StbSecPipe>();
            }
            secSteel.SecPipes.Add(pipe);
        }

        private static void AddHShapeSection(ref K2SSecSteelItems secSteel, KCroSec.CroSec_I secH)
        {
            var hShape = new StbSecRollH
            {
                name = secH.name,
                A = secH._height * 1000,
                B = secH.maxWidth() * 1000,
                r = secH.fillet_r * 1000,
                t1 = secH.w_thick * 1000,
                t2 = secH.uf_thick * 1000,
                type = StbSecRollHType.H
            };
            if (secSteel.SecRollHs == null)
            {
                secSteel.SecRollHs = new List<StbSecRollH>();
            }
            secSteel.SecRollHs.Add(hShape);
        }

        private static void AddTShapeSection(ref K2SSecSteelItems secSteel, KCroSec.CroSec_T secT)
        {
            var tShape = new StbSecRollT
            {
                name = secT.name,
                A = secT._height * 1000,
                B = secT.maxWidth() * 1000,
                r = secT.fillet_r * 1000,
                t1 = secT.w_thick * 1000,
                t2 = secT.uf_thick * 1000,
                type = StbSecRollTType.T,
            };
            if (secSteel.SecRollTs == null)
            {
                secSteel.SecRollTs = new List<StbSecRollT>();
            }
            secSteel.SecRollTs.Add(tShape);
        }

        private static void AddBoxSection(ref K2SSecSteelItems secSteel, KCroSec.CroSec_Box secBox)
        {
            double[] thickness = { secBox.w_thick, secBox.uf_thick, secBox.lf_thick };
            var box = new StbSecRollBOX
            {
                name = secBox.name,
                A = secBox._height * 1000,
                B = secBox.maxWidth() * 1000,
                r = secBox.fillet_r * 1000,
                t = thickness.Max() * 1000,
                type = StbSecRollBOXType.ELSE,
            };
            if (secSteel.SecRollBOXes == null)
            {
                secSteel.SecRollBOXes = new List<StbSecRollBOX>();
            }
            secSteel.SecRollBOXes.Add(box);
        }

        private static void AddTrapezoidSection(ref K2SSecSteelItems secSteel, KCroSec.CroSec_Trapezoid secTrapezoid)
        {
            var trapezoid = new StbSecFlatBar
            {
                name = secTrapezoid.name,
                B = secTrapezoid._height * 1000,
                t = secTrapezoid.uf_width * 1000
            };
            if (secSteel.SecFlatBars == null)
            {
                secSteel.SecFlatBars = new List<StbSecFlatBar>();
            }
            secSteel.SecFlatBars.Add(trapezoid);
        }
    }
}
