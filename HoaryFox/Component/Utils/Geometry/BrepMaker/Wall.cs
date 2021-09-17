using System.Linq;
using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public static class Wall
    {
        public static double GetThickness(StbSections sections, StbWall wall)
        {
            return sections.StbSecWall_RC.First(sec => sec.id == wall.id_section)
                .StbSecFigureWall_RC.StbSecWall_RC_Straight.t;
        }
    }
}
