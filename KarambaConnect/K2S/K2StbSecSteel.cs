using System.Collections.Generic;
using System.Linq;
using Karamba.Models;
using STBDotNet.Elements.StbModel.StbSection;
using KCroSec = Karamba.CrossSections;

namespace KarambaConnect.K2S
{
    public static class K2StbSecSteel
    {
        internal static void GetSection(ref Steel secSteel, Model kModel, int croSecId)
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

        private static void AddUnsupportedSection(ref Steel secSteel, Model kModel, int croSecId)
        {
            var unsupported = new Pipe
            {
                Name = kModel.crosecs[croSecId].name,
                D = 10,
                T = 1
            };
            if (secSteel.Pipe == null)
            {
                secSteel.Pipe = new List<Pipe>();
            }
            secSteel.Pipe.Add(unsupported);
        }

        private static void AddCircleSection(ref Steel secSteel, KCroSec.CroSec_Circle secCircle)
        {
            var pipe = new Pipe
            {
                Name = secCircle.name,
                D = secCircle.getHeight() * 1000,
                T = secCircle.thick * 1000
            };
            if (secSteel.Pipe == null)
            {
                secSteel.Pipe = new List<Pipe>();
            }
            secSteel.Pipe.Add(pipe);
        }

        private static void AddHShapeSection(ref Steel secSteel, KCroSec.CroSec_I secH)
        {
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
            if (secSteel.RollH == null)
            {
                secSteel.RollH = new List<RollH>();
            }
            secSteel.RollH.Add(hShape);
        }

        private static void AddTShapeSection(ref Steel secSteel, KCroSec.CroSec_T secT)
        {
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
            if (secSteel.RollT == null)
            {
                secSteel.RollT = new List<RollT>();
            }
            secSteel.RollT.Add(tShape);
        }

        private static void AddBoxSection(ref Steel secSteel, KCroSec.CroSec_Box secBox)
        {
            double[] thickness = { secBox.w_thick, secBox.uf_thick, secBox.lf_thick };
            var box = new RollBox
            {
                Name = secBox.name,
                A = secBox._height * 1000,
                B = secBox.maxWidth() * 1000,
                R = secBox.fillet_r * 1000,
                T = thickness.Max() * 1000,
                Type = "ELSE"
            };
            if (secSteel.RollBox == null)
            {
                secSteel.RollBox = new List<RollBox>();
            }
            secSteel.RollBox.Add(box);
        }

        private static void AddTrapezoidSection(ref Steel secSteel, KCroSec.CroSec_Trapezoid secTrapezoid)
        {
            var trapezoid = new FlatBar
            {
                Name = secTrapezoid.name,
                B = secTrapezoid._height * 1000,
                T = secTrapezoid.uf_width * 1000
            };
            if (secSteel.FlatBar == null)
            {
                secSteel.FlatBar = new List<FlatBar>();
            }
            secSteel.FlatBar.Add(trapezoid);
        }
    }
}
