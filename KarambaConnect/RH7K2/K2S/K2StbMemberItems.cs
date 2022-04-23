using Karamba.Elements;

using STBDotNet.v202;

namespace KarambaConnect.K2S
{
    public static class K2StbMemberItems
    {
        internal static StbColumn CreateColumn(ModelBeam elem, int croSecId, StbColumnKind_structure kind)
        {
            return new StbColumn
            {
                id = (elem.ind + 1).ToString(),
                name = elem.id,
                id_node_bottom = (elem.node_inds[0] + 1).ToString(),
                id_node_top = (elem.node_inds[1] + 1).ToString(),
                rotate = 0d,
                id_section = (croSecId + 1).ToString(),
                kind_structure = kind
            };
        }

        internal static StbGirder CreateGirder(ModelBeam elem, int croSecId, StbGirderKind_structure kind)
        {
            return new StbGirder
            {
                id = (elem.ind + 1).ToString(),
                name = elem.id,
                id_node_start = (elem.node_inds[0] + 1).ToString(),
                id_node_end = (elem.node_inds[1] + 1).ToString(),
                rotate = 0d,
                id_section = (croSecId + 1).ToString(),
                kind_structure = kind
            };
        }

        internal static StbBrace CreateBrace(ModelTruss elem, int croSecId)
        {
            return new StbBrace
            {
                id = (elem.ind + 1).ToString(),
                name = elem.id,
                id_node_start = (elem.node_inds[0] + 1).ToString(),
                id_node_end = (elem.node_inds[1] + 1).ToString(),
                rotate = 0d,
                id_section = (croSecId + 1).ToString(),
                kind_structure = StbBraceKind_structure.S
            };
        }
    }
}
