namespace HoaryFox.STB.Section
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
        RollBOX,
        BuildBOX
    }
}