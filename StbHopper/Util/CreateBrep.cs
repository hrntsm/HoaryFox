using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            foreach (var NodeIds in slabs.NodeIdList)
            {
                int[] nodeIndex = new int[NodeIds.Count];

                for (int i = 0; i < NodeIds.Count; i++)
                {
                    nodeIndex[i] = nodes.Id.IndexOf(NodeIds[i]);
                }
            }
            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            var brep = new List<Brep>();
            return brep;
        }

        public List<Brep> Frame(StbFrame frames)
        {
            var brep = new List<Brep>();
            return brep;
        }
    }
}
