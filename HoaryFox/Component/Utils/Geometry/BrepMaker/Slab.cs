using System;
using System.Linq;
using STBDotNet.v202;

namespace HoaryFox.Component.Utils.Geometry.BrepMaker
{
    public static class Slab
    {
        public static double GetDepth(StbSections sections, StbSlab slab)
        {
            double depth = 0;

            switch (slab.kind_structure)
            {
                case StbSlabKind_structure.RC:
                    object[] slabRc = sections.StbSecSlab_RC.First(sec => sec.id == slab.id_section).StbSecFigureSlab_RC.Items;
                    switch (slabRc.Length)
                    {
                        case 1:
                            var straight = slabRc[0] as StbSecSlab_RC_Straight;
                            depth = straight.depth;
                            break;
                        case 2:
                            var tapers = new[] { slabRc[0] as StbSecSlab_RC_Taper, slabRc[1] as StbSecSlab_RC_Taper };
                            depth = tapers.First(sec => sec.pos == StbSecSlab_RC_TaperPos.TIP).depth;
                            break;
                        case 3:
                            var haunches = new[]
                            {
                                slabRc[0] as StbSecSlab_RC_Haunch, slabRc[1] as StbSecSlab_RC_Haunch,
                                slabRc[2] as StbSecSlab_RC_Haunch
                            };
                            depth = haunches.First(sec => sec.pos == StbSecSlab_RC_HaunchPos.CENTER).depth;
                            break;
                    }

                    break;
                case StbSlabKind_structure.DECK:
                // StbSecSlabDeck slabDeck = _sections.StbSecSlabDeck.FirstOrDefault(sec => sec.id == slab.id_section);
                // break;
                case StbSlabKind_structure.PRECAST:
                // StbSecSlabPrecast slabPrecast = _sections.StbSecSlabPrecast.FirstOrDefault(sec => sec.id == slab.id_section);
                // break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return depth;
        }
    }
}
