using System.Collections.Generic;
using System.Xml.Linq;
using static HoaryFox.STB.StbData;

namespace HoaryFox.STB.Section
{
    /// <summary>
    /// 断面情報
    /// </summary>
    public class StbSections : StbBase
    {
        public override string Tag { get; } = "StbSections";
        /// <summary>
        /// 部材のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 部材が所属する階
        /// </summary>
        public List<string> Floor { get; } = new List<string>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            Id.Add((int)stbElem.Attribute("id"));
            
            if (stbElem.Attribute("floor") != null)
                Floor.Add((string)stbElem.Attribute("floor"));
            else
                Floor.Add(string.Empty);
        }
    }

    public class StbRcSections : StbSections
    {
        public override string Tag { get; } = "StbRcSections";
        /// <summary>
        /// 主筋径
        /// </summary>
        public List<string> DBarMain { get; } = new List<string>();
        /// <summary>
        /// せん断補強筋径
        /// </summary>
        public List<string> DBarBand { get; } = new List<string>();
        /// <summary>
        /// 各配筋の本数をまとめたリスト
        /// </summary>
        public List<List<double>> BarList { get; } = new List<List<double>>();

        protected override void ElementLoader(XElement stbElem, StbVersion version, string xmlns)
        {
            base.ElementLoader(stbElem, version, xmlns);
            switch (version)
            {
                case StbVersion.Ver1:
                    DBarMain.Add((string)stbElem.Attribute("D_reinforcement_main"));
                    DBarBand.Add((string)stbElem.Attribute("D_reinforcement_band"));
                    break;
                case StbVersion.Ver2:
                    break;
            }
            
            var stbColSecBarArrangement = new StbColSecBarArrangement();
            stbColSecBarArrangement.Load(stbElem, version, xmlns);
            BarList.Add(stbColSecBarArrangement.BarList);
        }
    }

    public class StbSteelSections : StbSections
    {
        public override string Tag { get; } = "StbSteelSections";
        /// <summary>
        /// 断面形状の名称
        /// </summary>
        public List<string> Shape { get; } = new List<string>();
    }
    
    public class StbSrcSections : StbSections
    {
        public override string Tag { get; } = "StbSrcSections";
    }

    public class StbSteelShapes
    {
        public string Pos { get; set; }
        public string Shape { get; set; }
        public string StrengthMain { get; set; }
        public string StrengthWeb { get; set; }
        
    }

    public class StbSteelParameters : StbBase
    {
        public override string Tag { get; } = "StbSecSteel";
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();
    }

    public class StbSectionHTCL : StbSteelParameters
    {
        /// <summary>
        /// 部材せい
        /// </summary>
        public List<float> A { get; } = new List<float>();
        /// <summary>
        /// フランジ幅
        /// </summary>
        public List<float> B { get; } = new List<float>();
        /// <summary>
        /// ウェブ厚
        /// </summary>
        public List<float> T1 { get; } = new List<float>();
        /// <summary>
        /// フランジ厚
        /// </summary>
        public List<float> T2 { get; } = new List<float>();
    }

    public class StbSectionBox : StbSteelParameters
    {
        /// <summary>
        /// 部材せい
        /// </summary>
        public List<float> A { get; } = new List<float>();
        /// <summary>
        /// 部材幅
        /// </summary>
        public List<float> B { get; } = new List<float>();
    }
}