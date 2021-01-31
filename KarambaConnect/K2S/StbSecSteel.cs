using System.Collections.Generic;
using System.Linq;
using STBDotNet.Elements.StbModel.StbSection;
using KCroSec = Karamba.CrossSections;

namespace KarambaConnect.K2S
{
    public class StbSecSteel
    {
        internal static void GetSection(ref Steel secSteel, Karamba.Models.Model kModel, int croSecId)
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
                    if (secSteel.FlatBar == null)
                    {
                        secSteel.FlatBar = new List<FlatBar>();
                    }
                    secSteel.FlatBar.Add(trapezoid);
                    break;
                case KCroSec.CroSec_Box secBox:
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
                    if (secSteel.RollT == null)
                    {
                        secSteel.RollT = new List<RollT>();
                    }
                    secSteel.RollT.Add(tShape);
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
                    if (secSteel.RollH == null)
                    {
                        secSteel.RollH = new List<RollH>();
                    }
                    secSteel.RollH.Add(hShape);
                    break;
                case KCroSec.CroSec_Circle secCircle:
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
                    break;
                default:
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
                    break;
            }
        }
    }
}