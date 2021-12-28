using System;
using System.Drawing;
using Karamba.Materials;

namespace KarambaConnect.S2K
{
    public static class Material
    {
        public static FemMaterial_Isotrop[] DefaultRcMaterialArray()
        {
            // TODO: せん断弾性係数あってる？
            return new FemMaterial_Isotrop[]
            {
#if karamba1
                new FemMaterial_Isotrop("Concrete", "Fc18", RcYoungsModulus(18, 23.0), RcShareModulus(18, 23.0, 0.2), RcShareModulus(18, 23.0, 0.2), 24.0, 18_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc21", RcYoungsModulus(21, 23.0), RcShareModulus(21, 23.0, 0.2), RcShareModulus(21, 23.0, 0.2), 24.0, 21_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc24", RcYoungsModulus(24, 23.0), RcShareModulus(24, 23.0, 0.2), RcShareModulus(24, 23.0, 0.2), 24.0, 24_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc27", RcYoungsModulus(27, 23.0), RcShareModulus(27, 23.0, 0.2), RcShareModulus(27, 23.0, 0.2), 24.0, 27_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc30", RcYoungsModulus(30, 23.0), RcShareModulus(30, 23.0, 0.2), RcShareModulus(30, 23.0, 0.2), 24.0, 30_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc33", RcYoungsModulus(33, 23.0), RcShareModulus(33, 23.0, 0.2), RcShareModulus(33, 23.0, 0.2), 24.0, 33_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc36", RcYoungsModulus(36, 23.0), RcShareModulus(36, 23.0, 0.2), RcShareModulus(36, 23.0, 0.2), 24.0, 36_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc40", RcYoungsModulus(40, 23.5), RcShareModulus(40, 23.5, 0.2), RcShareModulus(40, 23.5, 0.2), 24.5, 40_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc42", RcYoungsModulus(42, 23.5), RcShareModulus(42, 23.5, 0.2), RcShareModulus(42, 23.5, 0.2), 24.5, 42_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc45", RcYoungsModulus(45, 23.5), RcShareModulus(45, 23.5, 0.2), RcShareModulus(45, 23.5, 0.2), 24.5, 45_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc50", RcYoungsModulus(50, 24.0), RcShareModulus(50, 24.0, 0.2), RcShareModulus(50, 24.0, 0.2), 25.0, 50_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc55", RcYoungsModulus(55, 24.0), RcShareModulus(55, 24.0, 0.2), RcShareModulus(55, 24.0, 0.2), 25.0, 55_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc60", RcYoungsModulus(60, 24.0), RcShareModulus(60, 24.0, 0.2), RcShareModulus(60, 24.0, 0.2), 25.0, 60_0000, 1.00E-05, Color.Gray),
#elif karamba2
                new FemMaterial_Isotrop("Concrete", "Fc18", RcYoungsModulus(18, 23.0), RcShareModulus(18, 23.0, 0.2), RcShareModulus(18, 23.0, 0.2), 24.0, 18_0000, 1_8000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc21", RcYoungsModulus(21, 23.0), RcShareModulus(21, 23.0, 0.2), RcShareModulus(21, 23.0, 0.2), 24.0, 21_0000, 2_1000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc24", RcYoungsModulus(24, 23.0), RcShareModulus(24, 23.0, 0.2), RcShareModulus(24, 23.0, 0.2), 24.0, 24_0000, 2_4000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc27", RcYoungsModulus(27, 23.0), RcShareModulus(27, 23.0, 0.2), RcShareModulus(27, 23.0, 0.2), 24.0, 27_0000, 2_7000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc30", RcYoungsModulus(30, 23.0), RcShareModulus(30, 23.0, 0.2), RcShareModulus(30, 23.0, 0.2), 24.0, 30_0000, 3_0000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc33", RcYoungsModulus(33, 23.0), RcShareModulus(33, 23.0, 0.2), RcShareModulus(33, 23.0, 0.2), 24.0, 33_0000, 3_3000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc36", RcYoungsModulus(36, 23.0), RcShareModulus(36, 23.0, 0.2), RcShareModulus(36, 23.0, 0.2), 24.0, 36_0000, 3_6000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc40", RcYoungsModulus(40, 23.5), RcShareModulus(40, 23.5, 0.2), RcShareModulus(40, 23.5, 0.2), 24.5, 40_0000, 4_0000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc42", RcYoungsModulus(42, 23.5), RcShareModulus(42, 23.5, 0.2), RcShareModulus(42, 23.5, 0.2), 24.5, 42_0000, 4_2000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc45", RcYoungsModulus(45, 23.5), RcShareModulus(45, 23.5, 0.2), RcShareModulus(45, 23.5, 0.2), 24.5, 45_0000, 4_5000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc50", RcYoungsModulus(50, 24.0), RcShareModulus(50, 24.0, 0.2), RcShareModulus(50, 24.0, 0.2), 25.0, 50_0000, 5_0000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc55", RcYoungsModulus(55, 24.0), RcShareModulus(55, 24.0, 0.2), RcShareModulus(55, 24.0, 0.2), 25.0, 55_0000, 5_5000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc60", RcYoungsModulus(60, 24.0), RcShareModulus(60, 24.0, 0.2), RcShareModulus(60, 24.0, 0.2), 25.0, 60_0000, 6_0000, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray),
#endif
            };
        }

        public static FemMaterial_Isotrop StbToRcFemMaterial(string stbStrengthFc)
        {
            double fc, gamma;
            const double Nu = 0.2d;

            if (stbStrengthFc == null)
            {
                fc = 21;
                gamma = 23;
            }
            else
            {
                // マッチしなかった場合は Fc21 相当で返す
                switch (stbStrengthFc.ToLower())
                {
                    case "fc18":
                        fc = 18;
                        gamma = 23;
                        break;
                    case "fc21":
                        fc = 21;
                        gamma = 23;
                        break;
                    case "fc24":
                        fc = 24;
                        gamma = 23;
                        break;
                    case "fc27":
                        fc = 27;
                        gamma = 23;
                        break;
                    case "fc30":
                        fc = 30;
                        gamma = 23;
                        break;
                    case "fc33":
                        fc = 33;
                        gamma = 23;
                        break;
                    case "fc36":
                        fc = 36;
                        gamma = 23;
                        break;
                    case "fc40":
                        fc = 40;
                        gamma = 23.5;
                        break;
                    case "fc42":
                        fc = 42;
                        gamma = 23.5;
                        break;
                    case "fc45":
                        fc = 45;
                        gamma = 23.5;
                        break;
                    case "fc50":
                        fc = 50;
                        gamma = 24;
                        break;
                    case "fc55":
                        fc = 55;
                        gamma = 24;
                        break;
                    case "fc60":
                        fc = 60;
                        gamma = 24;
                        break;
                    default:
                        fc = 21;
                        gamma = 23;
                        break;
                }
            }

#if karamba1
            return new FemMaterial_Isotrop("Concrete", stbStrengthFc,
                RcYoungsModulus(fc, gamma), RcShareModulus(fc, gamma, Nu), RcShareModulus(fc, gamma, Nu),
                gamma + 1, fc * 1000, 1.00E-05, Color.Gray);
#elif karamba2
            return new FemMaterial_Isotrop("Concrete", stbStrengthFc,
                RcYoungsModulus(fc, gamma), RcShareModulus(fc, gamma, Nu), RcShareModulus(fc, gamma, Nu),
                gamma + 1, fc * 1000, fc * 100, FemMaterial.FlowHypothesis.mises, 1.00E-05, Color.Gray);
#endif
        }

        private static double RcYoungsModulus(double fc, double gamma)
        {
            // Karamba3D の単位に合わせて kN/cm2
            return 3.34 * Math.Pow(10, 4) * Math.Pow(gamma / 24, 2) * Math.Pow(fc / 60, 1 / 3d) * 1000;
        }

        private static double RcShareModulus(double fc, double gamma, double poisson)
        {
            return RcYoungsModulus(fc, gamma) / (2 * (1 + poisson));
        }

        public static FemMaterial_Isotrop[] DefaultSteelMaterialArray()
        {
            throw new NotImplementedException();
        }
    }
}
