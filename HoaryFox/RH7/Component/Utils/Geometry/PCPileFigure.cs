using System;

using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry
{
    public class PCPileFigure : IComparable<PCPileFigure>
    {
        public int Order { get; private set; }
        public double Diameter { get; private set; }
        public double Thickness { get; private set; }
        public double Length { get; private set; }

        public PCPileFigure(int order, double diameter, double thickness, double length)
        {
            Order = order;
            Diameter = diameter;
            Thickness = thickness;
            Length = length;
        }

        public PCPileFigure(StbSecPileProductNodular_CPRC figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D1;
            Thickness = figure.tc;
            Length = figure.length_pile;
        }

        public PCPileFigure(StbSecPileProductNodular_PRC figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D1;
            Thickness = figure.tc;
            Length = figure.length_pile;
        }

        public PCPileFigure(StbSecPileProductNodular_PHC figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D1;
            Thickness = figure.t;
            Length = figure.length_pile;
        }

        public PCPileFigure(StbSecPileProduct_CPRC figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D;
            Thickness = figure.tc;
            Length = figure.length_pile;
        }

        public PCPileFigure(StbSecPileProduct_PRC figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D;
            Thickness = figure.tc;
            Length = figure.length_pile;
        }

        public PCPileFigure(StbSecPileProduct_PHC figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D;
            Thickness = figure.t;
            Length = figure.length_pile;
        }

        public PCPileFigure(StbSecPileProduct_SC figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D;
            Thickness = figure.tc;
            Length = figure.length_pile;
        }

        public PCPileFigure(StbSecPileProduct_ST figure)
        {
            Order = int.Parse(figure.id_order);
            Diameter = figure.D1;
            Thickness = figure.t1;
            Length = figure.length_pile;
        }

        public int CompareTo(PCPileFigure other)
        {
            return Order.CompareTo(other.Order);
        }
    }
}
