using System.Drawing;
using Karamba.Materials;

namespace KarambaConnect.S2K
{
    public static class Material
    {
        public static FemMaterial_Isotrop[] DefaultRcMaterials()
        {
            // TODO: 強度ごとに値を計算して設定する
            return new FemMaterial_Isotrop[]
            {
                new FemMaterial_Isotrop("Concrete", "Fc18", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc21", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc24", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc27", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc30", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc33", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc36", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc40", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc42", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc45", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc50", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc55", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
                new FemMaterial_Isotrop("Concrete", "Fc60", 2186_0000, 911_0000, 911_0000, 24, 14_0000, 1.00E-05, Color.Gray),
            };
        }

        public static FemMaterial_Isotrop[] DefaultSteelMaterials()
        {
            throw new System.NotImplementedException();
        }
    }
}
