using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using HoaryFox.Member;
using Rhino;
using Rhino.DocObjects;
using STBReader;
using STBReader.Member;

namespace HoaryFox
{
    public static class Misc
    {
        public static List<List<string>> GetTag(StbData stbData, StbFrame stbFrame)
        {
            var tags = new CreateTag(stbData.Nodes, stbData.SecColumnRc, stbData.SecColumnS, stbData.SecBeamRc, stbData.SecBeamS, stbData.SecBraceS, stbData.SecSteel);
            return tags.FrameList(stbFrame);
        }

        public static void MakeParentLayers(RhinoDoc activeDoc, IEnumerable<string> parentLayerNames, IReadOnlyList<Color> layerColors)
        {
            foreach ((string name, int index) in parentLayerNames.Select((name, index) => (name, index)))
            {
                var parentLayer = new Layer { Name = name, Color = layerColors[index] };
                activeDoc.Layers.Add(parentLayer);
            }
        }
        public static void SetFrameUserString(ref ObjectAttributes objAttr, IReadOnlyList<string> tag)
        {
            objAttr.SetUserString("Tag", tag[0]);
            objAttr.SetUserString("ShapeType", tag[1]);
            objAttr.SetUserString("Height", tag[2]);
            objAttr.SetUserString("Width", tag[3]);
            objAttr.SetUserString("t1", tag[4]);
            objAttr.SetUserString("t2", tag[5]);
            objAttr.SetUserString("Kind", tag[6]);
        }
    }
}
