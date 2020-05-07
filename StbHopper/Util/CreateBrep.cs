using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Grasshopper.Kernel;
using Rhino.Geometry;

using StbHopper.STB;

namespace StbHopper.Util
{
    public class CreateBrep
    {
        StbNodes nodes = null;

        public CreateBrep(StbNodes nodes)
        {
            this.nodes = nodes;
        }

        public List<Brep> Slab(StbSlabs slabs)
        {   
            var brep = new List<Brep>();
            int slabNum = 0;

            foreach (var nodeIds in slabs.NodeIdList)
            {
                int[] index = new int[4];
                Point3d[] pt = new Point3d[4];
                double offset = slabs.Level[slabNum];

                for (int i = 0; i < nodeIds.Count; i++)
                {
                    index[i] = nodes.Id.IndexOf(nodeIds[i]);
                    pt[i] = new Point3d(nodes.X[index[i]], nodes.Y[index[i]], nodes.Z[index[i]] + offset);
                }

                brep.Add(nodeIds.Count == 4
                    ? Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], pt[3],1)
                    : Brep.CreateFromCornerPoints(pt[0], pt[1], pt[2], 1));
                slabNum++;
            }
            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            var brep = new List<Brep>();

            foreach (var nodeIds in walls.NodeIdList)
            {
                int[] index = new int[4];

                for (int i = 0; i < nodeIds.Count; i++)
                    index[i] = nodes.Id.IndexOf(nodeIds[i]);
                brep.Add(nodeIds.Count == 4
                    ? Brep.CreateFromCornerPoints(nodes.Pt[index[0]], nodes.Pt[index[1]], nodes.Pt[index[2]], nodes.Pt[index[3]], 1)
                    : Brep.CreateFromCornerPoints(nodes.Pt[index[0]], nodes.Pt[index[1]], nodes.Pt[index[2]], 1));
            }
            return brep;
        }

        public List<Brep> Frame(StbFrame frames)
        {
            var brep = new List<Brep>();
            return brep;
        }
    }
}
