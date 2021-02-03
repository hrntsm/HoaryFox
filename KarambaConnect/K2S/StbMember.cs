using Karamba.Elements;
using STBDotNet.Elements.StbModel.StbMember;

namespace KarambaConnect.K2S
{
    public static class StbMember
    {
        internal static Column CreateColumn(ModelBeam elem, int croSecId, string kind)
        {
            return new Column
            {
                Id = elem.ind + 1,
                Name = elem.id,
                IdNodeStart = elem.node_inds[0] + 1,
                IdNodeEnd = elem.node_inds[1] + 1,
                Rotate = 0d,
                IdSection = croSecId + 1,
                Kind = kind
            };
        }

        internal static Girder CreateGirder(ModelBeam elem, int croSecId, string kind)
        {
            return new Girder
            {
                Id = elem.ind + 1,
                Name = elem.id,
                IdNodeStart = elem.node_inds[0] + 1,
                IdNodeEnd = elem.node_inds[1] + 1,
                Rotate = 0d,
                IdSection = croSecId + 1,
                Kind = kind
            };
        }

        internal static Brace CreateBrace(ModelTruss elem, int croSecId)
        {
            return new Brace
            {
                Id = elem.ind + 1,
                Name = elem.id,
                IdNodeStart = elem.node_inds[0] + 1,
                IdNodeEnd = elem.node_inds[1] + 1,
                Rotate = 0d,
                IdSection = croSecId + 1,
                Kind = "S"
            };
        }
    }
}