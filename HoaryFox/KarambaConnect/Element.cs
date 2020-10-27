using System.Collections.Generic;
using Karamba.CrossSections;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.Utilities;
using STBReader;
using STBReader.Member;
using STBReader.Model;

namespace HoaryFox.KarambaConnect
{
    public class Element
    {
        public static List<BuilderBeam> BuilderBeams(StbData stbData, List<string>[] k3Ids)
        {
            var logger = new MessageLogger();
            var k3d = new KarambaCommon.Toolkit();
            var outNodes = new List<Karamba.Geometry.Point3>();
            var elems = new List<BuilderBeam>();
            StbNodes nodes = stbData.Nodes;
            
            var k3Elems = new List<Line3>[2];
            k3Elems[0] = new List<Line3>();
            k3Elems[1] = new List<Line3>();

            foreach (StbFrame frame in new List<StbFrame>{ stbData.Columns, stbData.Girders, stbData.Braces} )
            {
                int elemIndex = frame is StbBraces ? 1 : 0;
                bool bending = !(frame is StbBraces);
                    
                for (var i = 0; i < frame.Id.Count; i++)
                {
                    int idNodeStart = nodes.Id.IndexOf(frame.IdNodeStart[i]);
                    int idNodeEnd = nodes.Id.IndexOf(frame.IdNodeEnd[i]);
                    var p0 = new Karamba.Geometry.Point3(
                        nodes.Position[idNodeStart].X / 1000d,
                        nodes.Position[idNodeStart].Y / 1000d,
                        nodes.Position[idNodeStart].Z / 1000d
                    );
                    var p1 = new Karamba.Geometry.Point3(
                        nodes.Position[idNodeEnd].X / 1000d,
                        nodes.Position[idNodeEnd].Y / 1000d,
                        nodes.Position[idNodeEnd].Z / 1000d
                    );
                    k3Elems[elemIndex].Add(new Line3(p0, p1));
                }
                elems.AddRange(k3d.Part.LineToBeam(k3Elems[elemIndex], k3Ids[elemIndex], new List<CroSec>(), logger, out outNodes, bending));
            }

            return elems;
        }
    }
}