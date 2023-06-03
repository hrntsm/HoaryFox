using System;
using System.Collections.Generic;
using System.Linq;

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

        public static IEnumerable<PCPileFigure> GetFigureList(StbSections sections, StbPile member)
        {
            var figures = new List<PCPileFigure>();
            var idSection = member.id_section;

            StbSecPileProduct secPileProduct = sections.StbSecPileProduct.First(sec => sec.id == idSection);
            return GetFigureList(secPileProduct);
        }

        public static List<PCPileFigure> GetFigureList(StbSecPileProduct secPileProduct)
        {
            var figures = new List<PCPileFigure>();
            var secNodularCPRC = secPileProduct.StbSecFigurePileProduct.StbSecPileProductNodular_CPRC;
            if (secNodularCPRC != null)
            {
                figures.AddRange(secNodularCPRC.Select(figure => new PCPileFigure(figure)));
            }
            var secNodularPHC = secPileProduct.StbSecFigurePileProduct.StbSecPileProductNodular_PHC;
            if (secNodularPHC != null)
            {
                figures.AddRange(secNodularPHC.Select(figure => new PCPileFigure(figure)));
            }
            var secNodularPRC = secPileProduct.StbSecFigurePileProduct.StbSecPileProductNodular_PRC;
            if (secNodularPRC != null)
            {
                figures.AddRange(secNodularPRC.Select(figure => new PCPileFigure(figure)));
            }
            var secCPRC = secPileProduct.StbSecFigurePileProduct.StbSecPileProduct_CPRC;
            if (secCPRC != null)
            {
                figures.AddRange(secCPRC.Select(figure => new PCPileFigure(figure)));
            }
            var secPHC = secPileProduct.StbSecFigurePileProduct.StbSecPileProduct_PHC;
            if (secPHC != null)
            {
                figures.AddRange(secPHC.Select(figure => new PCPileFigure(figure)));
            }
            var secPRC = secPileProduct.StbSecFigurePileProduct.StbSecPileProduct_PRC;
            if (secPRC != null)
            {
                figures.AddRange(secPRC.Select(figure => new PCPileFigure(figure)));
            }
            var secSC = secPileProduct.StbSecFigurePileProduct.StbSecPileProduct_SC;
            if (secSC != null)
            {
                figures.AddRange(secSC.Select(figure => new PCPileFigure(figure)));
            }
            var secST = secPileProduct.StbSecFigurePileProduct.StbSecPileProduct_ST;
            if (secST != null)
            {
                figures.AddRange(secST.Select(figure => new PCPileFigure(figure)));
            }

            figures.Sort();
            return figures;
        }
    }
}
