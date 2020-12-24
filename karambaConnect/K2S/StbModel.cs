using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;
using Karamba.Elements;
using Karamba.GHopper.Geometry;
using Microsoft.Win32.SafeHandles;
using Rhino.Geometry;
using STBDotNet.Elements.StbModel;
using STBDotNet.Elements.StbModel.StbMember;

namespace karambaConnect.K2S
{
    public class StbModel
    {
        public static Model Set(Karamba.Models.Model kModel)
        {
            var sModel = new Model
            {
                Nodes = SetNode(kModel),
                Members = SetMember(kModel)
            };
            
            return sModel;
        }

        private static Members SetMember(Karamba.Models.Model kModel)
        {
            var member = new Members
            {
                Columns = new List<Column>(),
                Girders = new List<Girder>()
            };

            foreach (ModelElement elem in kModel.elems)
            {
                if (elem.node_inds.Count != 2)
                {
                    continue;
                }

                var node = new Karamba.Nodes.Node[2]
                {
                    kModel.nodes[elem.node_inds[0]],
                    kModel.nodes[elem.node_inds[1]],
                };

                var orientation = new Vector3d(node[1].pos.Convert() - node[0].pos.Convert());
                double angle = Vector3d.VectorAngle(orientation, Vector3d.ZAxis);
                
                if (angle < Math.PI / 4d & angle > - Math.PI / 4d)
                {
                    var column = new Column
                    {
                        Id = elem.ind,
                        Name = elem.crosec.name,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = 1,
                        Kind = elem.crosec.material.family == "Steel" ? "S" : "RC"
                    };
                    member.Columns.Add(column);
                }
                else
                {
                    var beam = new Girder
                    {
                        Id = elem.ind,
                        Name = elem.crosec.name,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = 1,
                        IsFoundation = "false",
                        Kind = elem.crosec.material.family == "Steel" ? "S" : "RC"
                    };
                    member.Girders.Add(beam);
                }
            }

            return member;
        }

        private static List<Node> SetNode(Karamba.Models.Model kModel)
        {
            return kModel.nodes.ToStb();
        }
    }
}