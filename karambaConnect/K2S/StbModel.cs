using System;
using System.Collections.Generic;
using System.Linq;
using Karamba.Elements;
using Karamba.GHopper.Geometry;
using Rhino.Geometry;
using STBDotNet.Elements.StbModel;
using STBDotNet.Elements.StbModel.StbMember;
using STBDotNet.Elements.StbModel.StbSection;

namespace karambaConnect.K2S
{
    public class StbModel
    {
        public static Model Set(Karamba.Models.Model kModel)
        {
            List<string> croSec = kModel.crosecs.Select(sec => sec.name).ToList();
            var members = new Members
            {
                Columns = new List<Column>(),
                Girders = new List<Girder>(),
                Braces = new List<Brace>()
            };
            var sections = new List<Section>();
            var steelSec = new Steel();
            var rollL = new List<RollL>();

            foreach (ModelElement elem in kModel.elems)
            {
                if (elem.node_inds.Count != 2)
                {
                    continue;
                }

                Karamba.Nodes.Node[] node =
                {
                    kModel.nodes[elem.node_inds[0]],
                    kModel.nodes[elem.node_inds[1]]
                };

                int croSecId = croSec.IndexOf(elem.crosec.name);

                var orientation = new Vector3d(node[1].pos.Convert() - node[0].pos.Convert());
                double angle = Vector3d.VectorAngle(orientation, Vector3d.ZAxis);

                if (typeof(ModelTruss) == elem.GetType())
                {
                    var brace = new Brace
                    {
                        Id = elem.ind,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = croSecId,
                        Kind = elem.crosec.material.family == "Steel" ? "S" : "RC"
                    };
                    members.Braces.Add(brace);

                    var bSec = new BraceS
                    {
                        Id = croSecId,
                        Name = kModel.crosecs[croSecId].name,
                        SteelBrace = new []
                        {
                            new SecSteel
                            {
                                Position = "ALL",
                                Shape = kModel.crosecs[croSecId].name,
                                StrengthMain = "SN490B"
                            }
                        }
                    };
                    sections.Add(bSec);
                }
                else if (angle < Math.PI / 4d & angle > -Math.PI / 4d)
                {
                    var column = new Column
                    {
                        Id = elem.ind,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = croSecId,
                        Kind = elem.crosec.material.family == "Steel" ? "S" : "RC"
                    };
                    members.Columns.Add(column);
                }
                else
                {
                    var beam = new Girder
                    {
                        Id = elem.ind,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = croSecId,
                        IsFoundation = "false",
                        Kind = elem.crosec.material.family == "Steel" ? "S" : "RC"
                    };
                    members.Girders.Add(beam);
                }
            }

            var sModel = new Model
            {
                Nodes = kModel.nodes.ToStb(),
                Members = members,
                Sections = sections
            };

            return sModel;
        }

        private static Members SetMemberAndSection(Karamba.Models.Model kModel)
        {
            List<string> croSec = kModel.crosecs.Select(sec => sec.name).ToList();
            var member = new Members
            {
                Columns = new List<Column>(),
                Girders = new List<Girder>(),
                Braces = new List<Brace>()
            };
            
            foreach (ModelElement elem in kModel.elems)
            {
                if (elem.node_inds.Count != 2)
                {
                    continue;
                }

                Karamba.Nodes.Node[] node = 
                {
                    kModel.nodes[elem.node_inds[0]],
                    kModel.nodes[elem.node_inds[1]]
                };

                int croSecId = croSec.IndexOf(elem.crosec.name);

                var orientation = new Vector3d(node[1].pos.Convert() - node[0].pos.Convert());
                double angle = Vector3d.VectorAngle(orientation, Vector3d.ZAxis);

                if (typeof(ModelTruss) == elem.GetType())
                {
                    var brace = new Brace
                    {
                        Id = elem.ind,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = croSecId,
                        Kind = elem.crosec.material.family == "Steel" ? "S" : "RC"
                    };
                    member.Braces.Add(brace);
                }
                else if (angle < Math.PI / 4d & angle > - Math.PI / 4d)
                {
                    var column = new Column
                    {
                        Id = elem.ind,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = croSecId,
                        Kind = elem.crosec.material.family == "Steel" ? "S" : "RC"
                    };
                    member.Columns.Add(column);
                }
                else
                {
                    var beam = new Girder
                    {
                        Id = elem.ind,
                        Name = elem.id,
                        IdNodeStart = elem.node_inds[0],
                        IdNodeEnd = elem.node_inds[1],
                        Rotate = 0d,
                        IdSection = croSecId,
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