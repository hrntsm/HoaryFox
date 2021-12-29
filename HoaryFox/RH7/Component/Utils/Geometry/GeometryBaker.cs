using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Rhino;
using Rhino.DocObjects;

namespace HoaryFox.Component.Utils.Geometry
{
    public class GeometryBaker
    {
        public static void MakeParentLayers(
            RhinoDoc activeDoc,
            IEnumerable<string> parentLayerNames,
            IReadOnlyList<Color> layerColors)
        {
            foreach ((string name, int index) in parentLayerNames.Select((name, index) => (name, index)))
            {
                var parentLayer = new Layer { Name = name, Color = layerColors[index] };
                activeDoc.Layers.Add(parentLayer);
            }
        }
    }
}
