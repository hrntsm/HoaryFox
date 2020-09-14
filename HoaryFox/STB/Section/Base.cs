using System.Collections.Generic;

namespace HoaryFox.STB.Section
{
    /// <summary>
    /// 断面情報
    /// </summary>
    public class StbSections : StbBase
    {
        /// <summary>
        /// 部材のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 部材が所属する階
        /// </summary>
        public List<string> Floor { get; } = new List<string>();
    }

    public class StbRcSections : StbSections
    {
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
    }

    public class StbSteelSections : StbSections
    {
        /// <summary>
        /// 断面形状の名称
        /// </summary>
        public List<string> Shape { get; } = new List<string>();
    }
    
    public class StbSrcSections : StbSections
    {
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