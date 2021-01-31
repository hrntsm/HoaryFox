using System;
using System.Collections.Generic;
using System.Linq;
using Rhino.Geometry;

using Karamba.Elements;
using Karamba.GHopper.Geometry;

using STBDotNet.Elements.StbModel.StbMember;
using STBDotNet.Elements.StbModel.StbSection;
using Model = STBDotNet.Elements.StbModel.Model;

namespace KarambaConnect.K2S
{
    public static class StbModel
    {
        public static Model SetByAngle(Karamba.Models.Model kModel, double colMaxAngle)
        {
            List<string> croSec = kModel.crosecs.Select(sec => sec.name).ToList();
            var vNum = 1;
            var cNum = 1;
            var gNum = 1;
            // 0:column, 1:girder, 2:brace
            var registeredCroSecId = new List<List<int>> {new List<int>(), new List<int>(), new List<int>()};
            var registeredCroSecName = new List<List<string>> {new List<string>(), new List<string>(), new List<string>()};
            var members = new Members
            {
                Columns = new List<Column>(),
                Girders = new List<Girder>(),
                Braces = new List<Brace>()
            };
            var sections = new List<Section>();
            var secSteel = new Steel();

            foreach (ModelElement elem in kModel.elems)
            {
                if (!(elem is ModelElementStraightLine))
                {
                    continue;
                }

                int croSecId = croSec.IndexOf(elem.crosec.name);
                
                var elemLine = new Line
                (
                    kModel.nodes[elem.node_inds[0]].pos.Convert(),
                    kModel.nodes[elem.node_inds[1]].pos.Convert()
                );
                
                double angle = Vector3d.VectorAngle(elemLine.Direction, Vector3d.ZAxis);

                switch (elem)
                {
                    case ModelBeam modelBeam:
                        string kind;
                        switch (elem.crosec.material.family)
                        {
                            case "Steel": kind = "S"; break;
                            case "Concrete": kind = "RC"; break;
                            default: kind = ""; break;
                        }
                        if (angle <= colMaxAngle && angle >= -1d * colMaxAngle)
                        {
                            members.Columns.Add(StbMember.CreateColumn(modelBeam, croSecId, kind));

                            if (registeredCroSecId[0].IndexOf(croSecId) < 0)
                            {
                                switch (kind)
                                {
                                    case "S":
                                        sections.Add(StbSection.GetColumnSt(croSecId, cNum, kModel));

                                        if (registeredCroSecName[0].IndexOf(kModel.crosecs[croSecId].name) < 0)
                                        {
                                            StbSecSteel.GetSection(ref secSteel, kModel, croSecId);
                                            registeredCroSecName[0].Add(kModel.crosecs[croSecId].name);
                                        }
                                        break;
                                    case "RC":
                                        sections.Add(StbSection.GetColumnRc(croSecId, cNum, kModel));
                                        break;
                                    default:
                                        throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
                                }
                                registeredCroSecId[0].Add(croSecId);
                                cNum++;
                            }
                        }
                        else
                        {
                            members.Girders.Add(StbMember.CreateGirder(modelBeam, croSecId, kind));

                            if (registeredCroSecId[1].IndexOf(croSecId) < 0)
                            {
                                switch (kind)
                                {
                                    case "S":
                                        sections.Add(StbSection.GetBeamSt(croSecId, gNum, kModel));

                                        if (registeredCroSecName[1].IndexOf(kModel.crosecs[croSecId].name) < 0)
                                        {
                                            StbSecSteel.GetSection(ref secSteel, kModel, croSecId);
                                            registeredCroSecName[1].Add(kModel.crosecs[croSecId].name);
                                        }
                                        break;
                                    case "RC":
                                        sections.Add(StbSection.GetBeamRc(croSecId, gNum, kModel));
                                        break;
                                    default:
                                        throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
                                }
                                registeredCroSecId[1].Add(croSecId);
                                gNum++;
                            }
                        }
                        break;
                    case ModelTruss modelTruss:
                        members.Braces.Add(StbMember.CreateBrace(modelTruss, croSecId));

                        if (registeredCroSecId[2].IndexOf(croSecId) < 0)
                        {
                            sections.Add(StbSection.GetBraceSt(croSecId, vNum, kModel));
                            registeredCroSecId[2].Add(croSecId);

                            if (registeredCroSecName[2].IndexOf(kModel.crosecs[croSecId].name) < 0)
                            {
                                StbSecSteel.GetSection(ref secSteel, kModel, croSecId);
                                registeredCroSecName[2].Add(kModel.crosecs[croSecId].name);
                            }

                            vNum++;
                        }
                        break;
                    default:
                        break;
                }
            }

            sections.Add(secSteel);
            var sModel = new Model
            {
                Members = members,
                Sections = sections
            };

            return sModel;
        }
    }
}