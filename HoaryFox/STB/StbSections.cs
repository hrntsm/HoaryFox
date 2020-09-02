using System.Collections.Generic;
using System.Xml.Linq;

namespace HoaryFox.STB
{
    /// <summary>
    /// 主柱か間柱かの柱の種別
    /// </summary>
    public enum KindsColumn
    {
        Column,
        Post
    }

    /// <summary>
    /// 大梁か小梁かの梁種別
    /// </summary>
    public enum KindsBeam
    {
        Girder,
        Beam
    }

    /// <summary>
    /// ブレースが鉛直か水平かの梁種別
    /// </summary>
    public enum KindsBrace
    {
        Vertical,
        Horizontal
    }

    /// <summary>
    /// 柱脚形式
    /// </summary>
    public enum BaseTypes
    {
        /// <summary>
        /// 露出柱脚
        /// </summary>
        Expose,
        /// <summary>
        /// 埋込柱脚
        /// </summary>
        Embedded,
        /// <summary>
        /// 非埋込柱脚
        /// </summary>
        Unembedded,
        /// <summary>
        /// 根巻柱脚
        /// </summary>
        Wrap
    }

    /// <summary>
    /// ロールHの内での種別
    /// </summary>
    public enum RollHType
    {
        H,
        SH
    }

    /// <summary>
    /// ロールBOXの内での種別
    /// </summary>
    public enum RollBOXType
    {
        BCP,
        BCR,
        STKR,
        ELSE
    }

    /// <summary>
    /// ロールTの内での種別
    /// </summary>
    public enum RollTType
    {
        T,
        ST
    }

    /// <summary>
    /// 溝形の内での種別
    /// </summary>
    public enum RollCType
    {
        C,
        DoubleC
    }

    /// <summary>
    /// 山形の内での種別
    /// </summary>
    public enum RollLType
    {
        L,
        DoubleL
    }

    public enum ShapeTypes
    {
        H,
        L,
        T,
        C,
        FB,
        BOX,
        Bar,
        Pipe,
    }

    /// <summary>
    /// 断面情報
    /// </summary>
    class StbSections
    {
        // TODO 一括でStbMemberに属するものを読み込めるようにする
        // public void LoadAll(XDocument stbData) {
        // }
    }

    /// <summary>
    /// RC柱断面
    /// </summary>
    public class StbSecColRC:StbBase
    {
        /// <summary>
        /// 部材のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 部材が所属する階
        /// </summary>
        public List<string> Floor { get; } = new List<string>();
        /// <summary>
        /// 部材が主柱か間柱かの区別
        /// </summary>
        public List<KindsColumn> KindColumn { get; } = new List<KindsColumn>();
        /// <summary>
        /// 主筋径
        /// </summary>
        public List<string> DBarMain { get; } = new List<string>();
        /// <summary>
        /// フープ径
        /// </summary>
        public List<string> DBarBand { get; } = new List<string>();
        /// <summary>
        /// 部材幅
        /// </summary>
        public List<double> Width { get; } = new List<double>();
        /// <summary>
        /// 部材高さ
        /// </summary>
        public List<double> Height { get; } = new List<double>();
        /// <summary>
        /// 部材が矩形であるかどうか
        /// </summary>
        public List<bool> IsRect { get; } = new List<bool>();
        /// <summary>
        /// 各配筋の本数をまとめたリスト
        /// </summary>
        public List<List<double>> BarList { get; } = new List<List<double>>();

        /// <summary>
        /// 与えられたstbデータからRC柱断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        public override void Load(XDocument stbData)
        {
            var stbRcCols = stbData.Root.Descendants("StbSecColumn_RC");
            foreach (var stbRcCol in stbRcCols)
            {

                // 必須コード
                Id.Add((int)stbRcCol.Attribute("id"));
                Name.Add((string)stbRcCol.Attribute("name"));
                DBarMain.Add((string)stbRcCol.Attribute("D_reinforcement_main"));
                DBarBand.Add((string)stbRcCol.Attribute("D_reinforcement_band"));

                // 必須ではないコード
                if (stbRcCol.Attribute("Floor") != null)
                {
                    Floor.Add((string)stbRcCol.Attribute("Floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbRcCol.Attribute("kind_column") != null)
                {
                    KindColumn.Add((string) stbRcCol.Attribute("kind_column") == "COLUMN"
                        ? KindsColumn.Column
                        : KindsColumn.Post);
                }
                else
                {
                    KindColumn.Add(KindsColumn.Column);
                }

                // 子要素 StbSecFigure
                var stbColSecFigure = new StbColSecFigure();
                stbColSecFigure.Load(stbRcCol);
                Width.Add(stbColSecFigure.Width);
                Height.Add(stbColSecFigure.Height);
                IsRect.Add(stbColSecFigure.IsRect);

                // 子要素 StbSecBar_Arrangement
                var stbColSecBarArrangement = new StbColSecBarArrangement();
                stbColSecBarArrangement.Load(stbRcCol, stbColSecFigure.IsRect);
                BarList.Add(stbColSecBarArrangement.BarList);
            }
        }
    }

    /// <summary>
    /// RCとSRCの柱断面形状
    /// </summary>
    class StbColSecFigure
    {
        public double Width { get; private set; }
        public double Height { get; private set; }
        public bool IsRect { get; private set; }

        /// <summary>
        /// 与えられたstbデータからRC柱断面の形状を取得する。
        /// </summary>
        /// <param name="stbColumn"></param>
        public void Load(XElement stbColumn)
        {
            var stbFigure = stbColumn.Element("StbSecFigure");
            if (stbFigure.Element("StbSecRect") != null)
            {
                Width = (double)stbFigure.Element("StbSecRect").Attribute("DX");
                Height = (double)stbFigure.Element("StbSecRect").Attribute("DY");
                IsRect = true;
            }
            else if (stbFigure.Element("StbSecCircle") != null)
            {
                Width = (double)stbFigure.Element("StbSecCircle").Attribute("D");
                Height = 0;
                IsRect = false;
            }
            else
            {
                Width = 0;
                Height = 0;
                IsRect = false;
            }
        }
    }

    /// <summary>
    /// RC柱の配筋情報
    /// </summary>
    class StbColSecBarArrangement
    {
        public List<double> BarList { get; } = new List<double>();

        public void Load(XElement stbColumn, bool isRect)
        {
            string elementName;
            var stbBar = stbColumn.Element("StbSecBar_Arrangement");

            if (stbBar.Element("StbSecRect_Column_Same") != null)
            {
                elementName = "StbSecRect_Column_Same";
            }
            else if (stbBar.Element("StbSecRect_Column_Not_Same") != null)
            {
                elementName = "StbSecRect_Column_Not_Same";
            }
            else if (stbBar.Element("StbSecCircle_Column_Same") != null)
            {
                elementName = "StbSecCircle_Column_Same";
            }
            else if (stbBar.Element("StbSecCircle_Column_Not_Same") != null)
            {
                elementName = "StbSecCircle_Column_Not_Same";
            }
            else
            {
                BarList.AddRange(new List<double> { 2, 2, 0, 0, 4, 200, 2, 2 });
                return;
            }

            var stbBarElem = stbBar.Element(elementName);

            // Main 1
            if (stbBarElem.Attribute("count_main_X_1st") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_X_1st"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_main_X_1st") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_Y_1st"));
            else
                BarList.Add(0);

            // Main2
            if (stbBarElem.Attribute("count_main_X_2nd") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_X_2nd"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_main_Y_2nd") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_Y_2nd"));
            else
                BarList.Add(0);

            // Main total
            if (stbBarElem.Attribute("count_main_total") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_total"));
            else
                BarList.Add(0);

            // Band
            if (stbBarElem.Attribute("pitch_band") != null)
                BarList.Add((double)stbBarElem.Attribute("pitch_band"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_band_dir_X") != null)
                BarList.Add((double)stbBarElem.Attribute("count_band_dir_X"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_band_dir_Y") != null)
                BarList.Add((double)stbBarElem.Attribute("count_band_dir_Y"));
            else
                BarList.Add(0);
        }
    }

    /// <summary>
    /// S柱断面
    /// </summary>
    public class StbSecColumnS:StbBase
    {
        /// <summary>
        /// 部材のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 部材が所属する階
        /// </summary>
        public List<string> Floor { get; } = new List<string>();
        /// <summary>
        /// 部材が主柱か間柱かの区別
        /// </summary>
        public List<KindsColumn> KindColumn { get; } = new List<KindsColumn>();
        /// <summary>
        /// StbSecSteelの基準方向が部材座標系Xかどうか
        /// </summary>
        public List<bool> Direction { get; } = new List<bool>();
        /// <summary>
        /// 柱脚の形式
        /// </summary>
        public List<BaseTypes> BaseType { get; } = new List<BaseTypes>();
        /// <summary>
        /// 柱頭の継手のID
        /// </summary>
        public List<int> JointIdTop { get; } = new List<int>();
        /// <summary>
        /// 柱脚の継手のID
        /// </summary>
        public List<int> JointIdBottom { get; } = new List<int>();
        /// <summary>
        /// 断面形状の名称
        /// </summary>
        public List<string> Shape { get; } = new List<string>();

        /// <summary>
        /// 与えられたstbデータからS柱断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        public override void Load(XDocument stbData)
        {
            var stbStCols = stbData.Root.Descendants("StbSecColumn_S");
            foreach (var stbStCol in stbStCols)
            {
                // 必須コード
                Id.Add((int)stbStCol.Attribute("id"));
                Name.Add((string)stbStCol.Attribute("name"));

                // 必須ではないコード
                if (stbStCol.Attribute("Floor") != null)
                {
                    Floor.Add((string)stbStCol.Attribute("Floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbStCol.Attribute("kind_column") != null)
                {
                    if ((string)stbStCol.Attribute("kind_column") == "COLUMN")
                    {
                        KindColumn.Add(KindsColumn.Column);
                    }
                    else
                    {
                        KindColumn.Add(KindsColumn.Post);
                    }
                }
                else
                {
                    KindColumn.Add(KindsColumn.Column);
                }
                if (stbStCol.Attribute("base_type") != null)
                {
                    switch ((string)stbStCol.Attribute("base_type"))
                    {
                        case "EXPOSE":
                            BaseType.Add(BaseTypes.Expose); break;
                        case "EMBEDDED":
                            BaseType.Add(BaseTypes.Embedded); break;
                        case "WRAP":
                            BaseType.Add(BaseTypes.Wrap); break;
                        default:
                            break;
                    }
                }
                else
                {
                    BaseType.Add(BaseTypes.Expose);
                }
                if (stbStCol.Attribute("direction") != null)
                {
                    Direction.Add((bool)stbStCol.Attribute("direction"));
                }
                else
                {
                    Direction.Add(true);
                }
                if (stbStCol.Attribute("joint_id_top") != null)
                {
                    JointIdTop.Add((int)stbStCol.Attribute("joint_id_top"));
                }
                else
                {
                    JointIdTop.Add(-1);
                }
                if (stbStCol.Attribute("joint_id_bottom") != null)
                {
                    JointIdBottom.Add((int)stbStCol.Attribute("joint_id_bottom"));
                }
                else
                {
                    JointIdBottom.Add(-1);
                }

                // 子要素 StbSecSteelColumn
                var stbSecSteelColumn = new StbSecSteelColumn();
                stbSecSteelColumn.Load(stbStCol);
                Shape.Add(stbSecSteelColumn.Shape);
            }
        }
    }

    /// <summary>
    /// 柱断面形状の名称
    /// </summary>
    class StbSecSteelColumn
    {
        public string Pos { get; private set; }
        public string Shape { get; private set; }
        public string StrengthMain { get; private set; }
        public string StrengthWeb { get; private set; }

        public void Load(XElement stbStCol)
        {
            var secStCol = stbStCol.Element("StbSecSteelColumn");
            // 必須コード
            Pos = (string)secStCol.Attribute("pos");
            Shape = (string)secStCol.Attribute("shape");
            StrengthMain = (string)secStCol.Attribute("strength_main");

            // 必須ではないコード
            if (secStCol.Attribute("strength_web") != null)
            {
                StrengthWeb = (string)secStCol.Attribute("strength_web");
            }
            else
            {
                StrengthWeb = string.Empty;
            }
        }
    }

    /// <summary>
    /// SRC柱断面
    /// </summary>
    public class StbSecColumnSRC
    {
    }

    /// <summary>
    /// CFT柱断面
    /// </summary>
    public class StbSecColumnCFT
    {
    }

    /// <summary>
    /// RC梁断面
    /// </summary>
    public class StbSecBeamRC:StbBase
    {
        /// <summary>
        /// 部材のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 部材が所属する階
        /// </summary>
        public List<string> Floor { get; } = new List<string>();
        /// <summary>
        /// 部材が大梁か小梁かの区別
        /// </summary>
        public List<KindsBeam> KindBeam { get; } = new List<KindsBeam>();
        /// <summary>
        /// 部材が基礎梁であるかどうか
        /// </summary>
        public List<bool> IsFoundation { get; } = new List<bool>();
        /// <summary>
        /// 部材が片持ちであるかどうか
        /// </summary>
        public List<bool> IsCanti { get; } = new List<bool>();
        /// <summary>
        /// 部材が外端内端であるかどうか
        /// </summary>
        public List<bool> IsOutIn { get; } = new List<bool>();
        /// <summary>
        /// 主筋径
        /// </summary>
        public List<string> DBarMain { get; } = new List<string>();
        /// <summary>
        /// フープ径
        /// </summary>
        public List<string> DBarBand { get; } = new List<string>();
        /// <summary>
        /// 部材幅
        /// </summary>
        public List<double> Width { get; } = new List<double>();
        /// <summary>
        /// 部材高さ
        /// </summary>
        public List<double> Depth { get; } = new List<double>();
        /// <summary>
        /// 各配筋の本数をまとめたリスト
        /// </summary>
        public List<List<double>> BarList { get; } = new List<List<double>>();

        /// <summary>
        /// 与えられたstbデータからRC梁断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        public override void Load(XDocument stbData)
        {
            var stbRcBeams = stbData.Root.Descendants("StbSecBeam_RC");
            foreach (var stbRcBeam in stbRcBeams)
            {
                // 必須コード
                Id.Add((int)stbRcBeam.Attribute("id"));
                Name.Add((string)stbRcBeam.Attribute("name"));
                DBarMain.Add((string)stbRcBeam.Attribute("D_reinforcement_main"));
                DBarBand.Add((string)stbRcBeam.Attribute("D_reinforcement_band"));

                // 必須ではないコード
                if (stbRcBeam.Attribute("Floor") != null)
                {
                    Floor.Add((string)stbRcBeam.Attribute("Floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbRcBeam.Attribute("kind_column") != null)
                {
                    if ((string)stbRcBeam.Attribute("kind_beam") == "GIRDER")
                    {
                        KindBeam.Add(KindsBeam.Girder);
                    }
                    else
                    {
                        KindBeam.Add(KindsBeam.Beam);
                    }
                }
                else
                {
                    KindBeam.Add(KindsBeam.Girder);
                }

                // 子要素 StbSecFigure
                var stbBeamSecFigure = new StbBeamSecFigure();
                stbBeamSecFigure.Load(stbRcBeam);
                Width.Add(stbBeamSecFigure.Width);
                Depth.Add(stbBeamSecFigure.Depth);

                // 子要素 StbSecBar_Arrangement
                var stbBeamSecBarArrangement = new StbBeamSecBarArrangement();
                stbBeamSecBarArrangement.Load(stbRcBeam);
                BarList.Add(stbBeamSecBarArrangement.BarList);
            }
        }
    }

    /// <summary>
    /// RC梁断面の形状
    /// </summary>
    class StbBeamSecFigure
    {
        public double Width { get; private set; }
        public double Depth { get; private set; }

        /// <summary>
        /// 与えられたstbデータからRC梁断面の形状を取得する。
        /// </summary>
        /// <param name="stbBeam"></param>
        public void Load(XElement stbBeam)
        {
            var stbFigure = stbBeam.Element("StbSecFigure");

            if (stbFigure.Element("StbSecHaunch") != null)
            {
                Width = (int)stbFigure.Element("StbSecHaunch").Attribute("width_center");
                Depth = (int)stbFigure.Element("StbSecHaunch").Attribute("depth_center");
            }
            else if (stbFigure.Element("StbSecStraight") != null)
            {
                Width = (int)stbFigure.Element("StbSecStraight").Attribute("width");
                Depth = (int)stbFigure.Element("StbSecStraight").Attribute("depth");
            }
            else if (stbFigure.Element("StbSecTaper") != null)
            {
                Width = (int)stbFigure.Element("StbSecTaper").Attribute("width_end");
                Depth = (int)stbFigure.Element("StbSecTaper").Attribute("depth_end");
            }
            else
            {
                Width = 0;
                Depth = 0;
            }
        }
    }

    /// <summary>
    /// RC梁の配筋情報
    /// </summary>
    class StbBeamSecBarArrangement
    {
        public List<double> BarList { get; } = new List<double>();

        public void Load(XElement stbBeam)
        {
            string elementName;
            var stbBar = stbBeam.Element("StbSecBar_Arrangement");

            if (stbBar.Element("StbSecBeam_Start_Center_End_Section") != null)
                elementName = "StbSecBeam_Start_Center_End_Section";
            else if (stbBar.Element("StbSecBeam_Start_End_Section") != null)
                elementName = "StbSecBeam_Start_End_Section";
            else if (stbBar.Element("StbSecBeam_Same_Section") != null)
                elementName = "StbSecBeam_Same_Section";
            else
            {
                BarList.AddRange(new List<double> { 2, 2, 0, 0, 0, 0, 200, 2 });
                return;
            }

            var stbBarElem = stbBar.Element(elementName);

            // Main 1
            if (stbBarElem.Attribute("count_main_top_1st") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_top_1st"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_main_bottom_1st") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_bottom_1st"));
            else
                BarList.Add(0);

            // Main2
            if (stbBarElem.Attribute("count_main_top_2nd") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_top_2nd"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_main_bottom_2nd") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_bottom_2nd"));
            else
                BarList.Add(0);

            // Main3
            if (stbBarElem.Attribute("count_main_top_3rd") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_top_3rd"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_main_bottom_3rd") != null)
                BarList.Add((double)stbBarElem.Attribute("count_main_bottom_3rd"));
            else
                BarList.Add(0);

            // Band
            if (stbBarElem.Attribute("pitch_stirrup") != null)
                BarList.Add((double)stbBarElem.Attribute("pitch_stirrup"));
            else
                BarList.Add(0);
            if (stbBarElem.Attribute("count_stirrup") != null)
                BarList.Add((double)stbBarElem.Attribute("count_stirrup"));
            else
                BarList.Add(0);
        }
    }


    /// <summary>
    /// S梁断面
    /// </summary>
    public class StbSecBeamS:StbBase
    {
        /// <summary>
        /// 部材のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 部材が所属する階
        /// </summary>
        public List<string> Floor { get; } = new List<string>();
        /// <summary>
        /// 部材が大梁か小梁かの区別
        /// </summary>
        public List<KindsBeam> KindBeam { get; } = new List<KindsBeam>();
        /// <summary>
        /// 部材が片持ちであるかどうか
        /// </summary>
        public List<bool> IsCanti { get; } = new List<bool>();
        /// <summary>
        /// 部材が外端内端であるかどうか
        /// </summary>
        public List<bool> IsOutIn { get; } = new List<bool>();
        /// <summary>
        /// 始端の継手のID
        /// </summary>
        public List<int> JointIdStart { get; } = new List<int>();
        /// <summary>
        /// 終端の継手のID
        /// </summary>
        public List<int> JointIdEnd { get; } = new List<int>();
        /// <summary>
        /// 断面形状の名称
        /// </summary>
        public List<string> Shape { get; } = new List<string>();


        /// <summary>
        /// 与えられたstbデータからS梁断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        public override void Load(XDocument stbData)
        {
            var stbStBeams = stbData.Root.Descendants("StbSecBeam_S");
            foreach (var stbStBeam in stbStBeams)
            {

                // 必須コード
                Id.Add((int)stbStBeam.Attribute("id"));
                Name.Add((string)stbStBeam.Attribute("name"));

                // 必須ではないコード
                if (stbStBeam.Attribute("Floor") != null)
                {
                    Floor.Add((string)stbStBeam.Attribute("Floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbStBeam.Attribute("kind_beam") != null)
                {
                    if ((string)stbStBeam.Attribute("kind_beam") == "GIRDER")
                    {
                        KindBeam.Add(KindsBeam.Girder);
                    }
                    else
                    {
                        KindBeam.Add(KindsBeam.Beam);
                    }
                }
                else
                {
                    KindBeam.Add(KindsBeam.Girder);
                }
                if (stbStBeam.Attribute("isCanti") != null)
                {
                    IsCanti.Add((bool)stbStBeam.Attribute("isCanti"));
                }
                else
                {
                    IsCanti.Add(false);
                }
                if (stbStBeam.Attribute("isOutIn") != null)
                {
                    IsCanti.Add((bool)stbStBeam.Attribute("isOutIn"));
                }
                else
                {
                    IsCanti.Add(false);
                }
                if (stbStBeam.Attribute("joint_id_start") != null)
                {
                    JointIdStart.Add((int)stbStBeam.Attribute("joint_id_start"));
                }
                else
                {
                    JointIdStart.Add(-1);
                }
                if (stbStBeam.Attribute("joint_id_end") != null)
                {
                    JointIdEnd.Add((int)stbStBeam.Attribute("joint_id_end"));
                }
                else
                {
                    JointIdEnd.Add(-1);
                }

                // 子要素 StbSecSteelBeam
                var stbSecSteelBeam = new StbSecSteelBeam();
                stbSecSteelBeam.Load(stbStBeam);
                Shape.Add(stbSecSteelBeam.Shape);
            }
        }
    }

    /// <summary>
    /// S梁断面形状の名称
    /// </summary>
    class StbSecSteelBeam
    {
        public string Pos { get; private set; }
        public string Shape { get; private set; }
        public string StrengthMain { get; private set; }
        public string StrengthWeb { get; private set; }

        public void Load(XElement stbStBeam)
        {
            var secStBeam = stbStBeam.Element("StbSecSteelBeam");

            // 必須コード
            Pos = (string)secStBeam.Attribute("pos");
            Shape = (string)secStBeam.Attribute("shape");
            StrengthMain = (string)secStBeam.Attribute("strength_main");

            // 必須ではないコード
            if (secStBeam.Attribute("strength_web") != null)
            {
                StrengthWeb = (string)secStBeam.Attribute("strength_web");
            }
            else
            {
                StrengthWeb = string.Empty;
            }
        }
    }

    /// <summary>
    /// SRC梁断面
    /// </summary>
    public class StbSecBeamSRC
    {
    }

    /// <summary>
    /// Sブレース断面
    /// </summary>
    public class StbSecBraceS:StbBase
    {
        /// <summary>
        /// 部材のID
        /// </summary>
        public List<int> Id { get; } = new List<int>();
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 部材が所属する階
        /// </summary>
        public List<string> Floor { get; } = new List<string>();
        /// <summary>
        /// 部材が水平か鉛直かの区別
        /// </summary>
        public List<KindsBrace> KindBrace { get; } = new List<KindsBrace>();
        /// <summary>
        /// 断面形状の名称
        /// </summary>
        public List<string> Shape { get; } = new List<string>();

        /// <summary>
        /// 与えられたstbデータからSブレース断面を取得する。
        /// </summary>
        /// <param name="stbData"></param>
        public override void Load(XDocument stbData)
        {
            var stbStBraces = stbData.Root.Descendants("StbSecBrace_S");
            foreach (var stbStBrace in stbStBraces)
            {

                // 必須コード
                Id.Add((int)stbStBrace.Attribute("id"));
                Name.Add((string)stbStBrace.Attribute("name"));

                // 必須ではないコード
                if (stbStBrace.Attribute("Floor") != null)
                {
                    Floor.Add((string)stbStBrace.Attribute("Floor"));
                }
                else
                {
                    Floor.Add(string.Empty);
                }
                if (stbStBrace.Attribute("kind_brace") != null)
                {
                    if ((string)stbStBrace.Attribute("kind_brace") == "HORIZONTAL")
                    {
                        KindBrace.Add(KindsBrace.Horizontal);
                    }
                    else
                    {
                        KindBrace.Add(KindsBrace.Vertical);
                    }
                }
                else
                {
                    KindBrace.Add(KindsBrace.Vertical);
                }

                // 子要素 StbSecSteelBeam
                StbSecSteelBrace stbSecSteelBrace = new StbSecSteelBrace();
                stbSecSteelBrace.Load(stbStBrace);
                Shape.Add(stbSecSteelBrace.Shape);

            }
        }
    }

    /// <summary>
    /// Sブレース断面形状の名称
    /// </summary>
    public class StbSecSteelBrace
    {
        public string Pos { get; private set; }
        public string Shape { get; private set; }
        public string StrengthMain { get; private set; }
        public string StrengthWeb { get; private set; }

        /// <summary>
        /// 属性の読み込み
        /// </summary>
        /// <param name="stbStBrace"></param>
        public void Load(XElement stbStBrace)
        {
            var secStBrace = stbStBrace.Element("StbSecSteelBrace");

            // 必須コード
            Pos = (string)secStBrace.Attribute("pos");
            Shape = (string)secStBrace.Attribute("shape");
            StrengthMain = (string)secStBrace.Attribute("strength_main");

            // 必須ではないコード
            if (secStBrace.Attribute("strength_web") != null)
            {
                StrengthWeb = (string)secStBrace.Attribute("strength_web");
            }
            else
            {
                StrengthWeb = string.Empty;
            }
        }
    }

    /// <summary>
    /// RC壁断面
    /// </summary>
    public class StbSecWallRC
    {
    }

    /// <summary>
    /// RCスラブ断面
    /// </summary>
    public class StbSecSlabRC
    {
    }

    /// <summary>
    /// RC基礎断面
    /// </summary>
    public class StbSecFoundationRC
    {
    }

    /// <summary>
    /// 鉄骨断面
    /// </summary>
    public class StbSecSteel:StbBase
    {
        public List<string> Name { get; } = new List<string>();
        public List<float> P1 { get; } = new List<float>();
        public List<float> P2 { get; } = new List<float>();
        public List<float> P3 { get; } = new List<float>();
        public List<float> P4 { get; } = new List<float>();
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        public StbSecRollH RollH { get; } = new StbSecRollH();
        public StbSecBuildH BuildH { get; } = new StbSecBuildH();
        public StbSecRollBOX RollBOX { get; } = new StbSecRollBOX();
        public StbSecBuildBOX BuildBOX { get; } = new StbSecBuildBOX();
        public StbSecPipe Pipe { get; } = new StbSecPipe();
        public StbSecRollT RollT { get; } = new StbSecRollT();
        public StbSecRollC RollC { get; } = new StbSecRollC();
        public StbSecRollL RollL { get; } = new StbSecRollL();
        public StbSecRollLipC RollLipC { get; } = new StbSecRollLipC();
        public StbSecRollFB RollFB { get; } = new StbSecRollFB();
        public StbSecRollBar RollBar { get; } = new StbSecRollBar();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public override void Load(XDocument stbData)
        {
            // TODO 継承を使ってきれいに書き直す
            RollH.Load(stbData);
            Name.AddRange(RollH.Name);
            P1.AddRange(RollH.A);
            P2.AddRange(RollH.B);
            P3.AddRange(RollH.T1);
            P4.AddRange(RollH.T2);
            ShapeType.AddRange(RollH.ShapeType);

            BuildH.Load(stbData);
            Name.AddRange(BuildH.Name);
            P1.AddRange(BuildH.A);
            P2.AddRange(BuildH.B);
            P3.AddRange(BuildH.T1);
            P4.AddRange(BuildH.T2);
            ShapeType.AddRange(BuildH.ShapeType);

            RollBOX.Load(stbData);
            Name.AddRange(RollBOX.Name);
            P1.AddRange(RollBOX.A);
            P2.AddRange(RollBOX.B);
            P3.AddRange(RollBOX.T);
            P4.AddRange(RollBOX.R);
            ShapeType.AddRange(RollBOX.ShapeType);

            BuildBOX.Load(stbData);
            Name.AddRange(BuildBOX.Name);
            P1.AddRange(BuildBOX.A);
            P2.AddRange(BuildBOX.B);
            P3.AddRange(BuildBOX.T1);
            P4.AddRange(BuildBOX.T2);
            ShapeType.AddRange(BuildBOX.ShapeType);

            Pipe.Load(stbData);
            Name.AddRange(Pipe.Name);
            P1.AddRange(Pipe.T);
            P2.AddRange(Pipe.D);
            P3.AddRange(new List<float>(new float[Pipe.D.Count]));
            P4.AddRange(new List<float>(new float[Pipe.D.Count]));
            ShapeType.AddRange(Pipe.ShapeType);

            RollT.Load(stbData);
            Name.AddRange(RollT.Name);
            P1.AddRange(RollT.A);
            P2.AddRange(RollT.B);
            P3.AddRange(RollT.T1);
            P4.AddRange(RollT.T2);
            ShapeType.AddRange(RollT.ShapeType);

            RollC.Load(stbData);
            Name.AddRange(RollC.Name);
            P1.AddRange(RollC.A);
            P2.AddRange(RollC.B);
            P3.AddRange(RollC.T1);
            P4.AddRange(RollC.T2);
            ShapeType.AddRange(RollC.ShapeType);

            RollL.Load(stbData);
            Name.AddRange(RollL.Name);
            P1.AddRange(RollL.A);
            P2.AddRange(RollL.B);
            P3.AddRange(RollL.T1);
            P4.AddRange(RollL.T2);
            ShapeType.AddRange(RollL.ShapeType);

            RollLipC.Load(stbData);
            Name.AddRange(RollLipC.Name);
            P1.AddRange(RollLipC.H);
            P2.AddRange(RollLipC.A);
            P3.AddRange(RollLipC.C);
            P4.AddRange(RollLipC.T);
            ShapeType.AddRange(RollLipC.ShapeType);

            RollFB.Load(stbData);
            Name.AddRange(RollFB.Name);
            P1.AddRange(RollFB.B);
            P2.AddRange(RollFB.T);
            P3.AddRange(new List<float>(new float[Pipe.D.Count]));
            P4.AddRange(new List<float>(new float[Pipe.D.Count]));
            ShapeType.AddRange(RollFB.ShapeType);

            RollBar.Load(stbData);
            Name.AddRange(RollBar.Name);
            P1.AddRange(RollBar.R);
            P2.AddRange(RollBar.R);
            P3.AddRange(new List<float>(new float[Pipe.D.Count]));
            P4.AddRange(new List<float>(new float[Pipe.D.Count]));
            ShapeType.AddRange(RollBar.ShapeType);
        }
    }

    /// <summary>
    /// ロールH形断面
    /// </summary>
    public class StbSecRollH
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 形状のタイプ
        /// </summary>
        public List<RollHType> Type { get; } = new List<RollHType>();
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
        /// <summary>
        /// フィレット半径
        /// </summary>
        public List<float> R { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-H");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                A.Add((float)StSection.Attribute("A"));
                B.Add((float)StSection.Attribute("B"));
                T1.Add((float)StSection.Attribute("t1"));
                T2.Add((float)StSection.Attribute("t2"));
                R.Add((float)StSection.Attribute("r"));

                ShapeType.Add(ShapeTypes.H);
            }
        }
    }

    /// <summary>
    /// ビルトH形断面
    /// </summary>
    public class StbSecBuildH
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
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
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecBuild-H");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                A.Add((float)StSection.Attribute("A"));
                B.Add((float)StSection.Attribute("B"));
                T1.Add((float)StSection.Attribute("t1"));
                T2.Add((float)StSection.Attribute("t2"));

                ShapeType.Add(ShapeTypes.H);
            }
        }
    }

    /// <summary>
    /// ロール箱形断面
    /// </summary>
    public class StbSecRollBOX
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 形状のタイプ
        /// </summary>
        public List<RollBOXType> Type { get; } = new List<RollBOXType>();
        /// <summary>
        /// 部材せい
        /// </summary>
        public List<float> A { get; } = new List<float>();
        /// <summary>
        /// 部材幅
        /// </summary>
        public List<float> B { get; } = new List<float>();
        /// <summary>
        /// 板厚
        /// </summary>
        public List<float> T { get; } = new List<float>();
        /// <summary>
        /// コーナー半径
        /// </summary>
        public List<float> R { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-BOX");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                A.Add((float)StSection.Attribute("A"));
                B.Add((float)StSection.Attribute("B"));
                T.Add((float)StSection.Attribute("t"));
                R.Add((float)StSection.Attribute("R"));

                ShapeType.Add(ShapeTypes.BOX);
            }
        }
    }

    /// <summary>
    /// ビルト箱形断面
    /// </summary>
    public class StbSecBuildBOX
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 部材せい
        /// </summary>
        public List<float> A { get; } = new List<float>();
        /// <summary>
        /// 部材幅
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
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecBuild-BOX");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                A.Add((float)StSection.Attribute("A"));
                B.Add((float)StSection.Attribute("B"));
                T1.Add((float)StSection.Attribute("t1"));
                T2.Add((float)StSection.Attribute("t2"));

                ShapeType.Add(ShapeTypes.BOX);
            }
        }
    }

    /// <summary>
    /// 円形断面
    /// </summary>
    public class StbSecPipe
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 直径
        /// </summary>
        public List<float> D { get; } = new List<float>();
        /// <summary>
        /// 板厚
        /// </summary>
        public List<float> T { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecPipe");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                D.Add((float)StSection.Attribute("D"));
                T.Add((float)StSection.Attribute("t"));

                ShapeType.Add(ShapeTypes.Pipe);
            }
        }
    }

    /// <summary>
    /// T形断面
    /// </summary>
    public class StbSecRollT
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 形状のタイプ
        /// </summary>
        public List<RollTType> Type { get; } = new List<RollTType>();
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
        /// <summary>
        /// フィレット半径
        /// </summary>
        public List<float> R { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-T");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                A.Add((float)StSection.Attribute("A"));
                B.Add((float)StSection.Attribute("B"));
                T1.Add((float)StSection.Attribute("t1"));
                T2.Add((float)StSection.Attribute("t2"));
                R.Add((float)StSection.Attribute("r1"));

                ShapeType.Add(ShapeTypes.T);
            }
        }
    }

    /// <summary>
    /// 溝形断面
    /// </summary>
    public class StbSecRollC
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 形状のタイプ
        /// </summary>
        public List<RollCType> Type { get; } = new List<RollCType>();
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
        /// <summary>
        /// フィレット半径
        /// </summary>
        public List<float> R1 { get; } = new List<float>();
        /// <summary>
        /// フランジ先端半径
        /// </summary>
        public List<float> R2 { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-C");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                A.Add((float)StSection.Attribute("A"));
                B.Add((float)StSection.Attribute("B"));
                T1.Add((float)StSection.Attribute("t1"));
                T2.Add((float)StSection.Attribute("t2"));
                R1.Add((float)StSection.Attribute("r1"));
                R2.Add((float)StSection.Attribute("r2"));

                ShapeType.Add(ShapeTypes.C);
            }
        }
    }

    /// <summary>
    /// 山形断面
    /// </summary>
    public class StbSecRollL
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 形状のタイプ
        /// </summary>
        public List<RollLType> Type { get; } = new List<RollLType>();
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
        /// <summary>
        /// フィレット半径
        /// </summary>
        public List<float> R1 { get; } = new List<float>();
        /// <summary>
        /// フランジ先端半径
        /// </summary>
        public List<float> R2 { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-L");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                A.Add((float)StSection.Attribute("A"));
                B.Add((float)StSection.Attribute("B"));
                T1.Add((float)StSection.Attribute("t1"));
                T2.Add((float)StSection.Attribute("t2"));
                R1.Add((float)StSection.Attribute("r1"));
                R2.Add((float)StSection.Attribute("r2"));

                ShapeType.Add(ShapeTypes.L);
            }
        }
    }

    /// <summary>
    /// リップ溝形断面
    /// </summary>
    public class StbSecRollLipC
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 形状のタイプ
        /// </summary>
        public List<RollCType> Type { get; } = new List<RollCType>();
        /// <summary>
        /// 部材せい
        /// </summary>
        public List<float> H { get; } = new List<float>();
        /// <summary>
        /// フランジ幅
        /// </summary>
        public List<float> A { get; } = new List<float>();
        /// <summary>
        /// リップ長
        /// </summary>
        public List<float> C { get; } = new List<float>();
        /// <summary>
        /// 板厚
        /// </summary>
        public List<float> T { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-LipC");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                H.Add((float)StSection.Attribute("H"));
                A.Add((float)StSection.Attribute("A"));
                C.Add((float)StSection.Attribute("C"));
                T.Add((float)StSection.Attribute("t"));

                ShapeType.Add(ShapeTypes.C);
            }
        }
    }

    /// <summary>
    /// フラットバー断面
    /// </summary>
    public class StbSecRollFB
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 幅
        /// </summary>
        public List<float> B { get; } = new List<float>();
        /// <summary>
        /// 板厚
        /// </summary>
        public List<float> T { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-FB");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                B.Add((float)StSection.Attribute("B"));
                T.Add((float)StSection.Attribute("t"));

                ShapeType.Add(ShapeTypes.FB);
            }
        }
    }

    /// <summary>
    /// 丸鋼断面
    /// </summary>
    public class StbSecRollBar
    {
        /// <summary>
        /// 部材の名前
        /// </summary>
        public List<string> Name { get; } = new List<string>();
        /// <summary>
        /// 直径
        /// </summary>
        public List<float> R { get; } = new List<float>();
        /// <summary>
        /// 断面形状タイプ
        /// </summary>
        public List<ShapeTypes> ShapeType { get; } = new List<ShapeTypes>();

        /// <summary>
        /// 属性情報の読み込み
        /// </summary>
        /// <param name="stbData"></param>
        public void Load(XDocument stbData)
        {
            var StSections = stbData.Root.Descendants("StbSecRoll-Bar");

            foreach (var StSection in StSections)
            {
                // 必須コード
                Name.Add((string)StSection.Attribute("name"));
                R.Add((float)StSection.Attribute("R"));

                ShapeType.Add(ShapeTypes.Bar);
            }
        }
    }
}
