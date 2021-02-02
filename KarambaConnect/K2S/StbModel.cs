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
    public class StbModel
    {
        // 0:column, 1:girder, 2:brace
        private readonly List<List<int>> _registeredCroSecId = new List<List<int>>
        {
            new List<int>(), new List<int>(), new List<int>()
        };
        private readonly List<List<string>> _registeredCroSecName = new List<List<string>>
        {
            new List<string>(), new List<string>(), new List<string>()
        };

        private readonly List<Section> _sections = new List<Section>();
        private Steel _secSteel = new Steel();

        public Model SetByAngle(Karamba.Models.Model kModel, double colMaxAngle)
        {
            List<string> croSec = kModel.crosecs.Select(sec => sec.name).ToList();
            int[] tagNum = {1, 1, 1};
            var members = new Members { Columns = new List<Column>(), Girders = new List<Girder>(), Braces = new List<Brace>()};

            foreach (ModelElement elem in kModel.elems)
            {
                if (!(elem is ModelElementStraightLine))
                {
                    continue;
                }
                int croSecId = croSec.IndexOf(elem.crosec.name);
                var elemLine = new Line(kModel.nodes[elem.node_inds[0]].pos.Convert(), kModel.nodes[elem.node_inds[1]].pos.Convert());
                double angle = Vector3d.VectorAngle(elemLine.Direction, Vector3d.ZAxis);

                switch (elem)
                {
                    case ModelBeam modelBeam:
                        string kind = GetElementKind(elem.crosec.material.family);
                        if (angle <= colMaxAngle && angle >= -1d * colMaxAngle)
                        {
                            members.Columns.Add(StbMember.CreateColumn(modelBeam, croSecId, kind));
                            if (_registeredCroSecId[0].IndexOf(croSecId) < 0)
                            {
                                AddColumnSection(kind, croSecId, tagNum[0]++, kModel);
                            }
                        }
                        else
                        {
                            members.Girders.Add(StbMember.CreateGirder(modelBeam, croSecId, kind));
                            if (_registeredCroSecId[1].IndexOf(croSecId) < 0)
                            {
                                AddBeamSection(kind, croSecId, tagNum[1]++, kModel);
                            }
                        }
                        break;
                    case ModelTruss modelTruss:
                        members.Braces.Add(StbMember.CreateBrace(modelTruss, croSecId));
                        if (_registeredCroSecId[2].IndexOf(croSecId) < 0)
                        {
                            AddBraceSection(croSecId, tagNum[2]++, kModel);
                        }
                        break;
                    default:
                        break;
                }
            }

            _sections.Add(_secSteel);
            return new Model { Members = members, Sections = _sections };
        }

        private void AddBraceSection(int croSecId, int vNum, Karamba.Models.Model kModel)
        {
            _sections.Add(StbSection.GetBraceSt(croSecId, vNum, kModel));
            _registeredCroSecId[2].Add(croSecId);

            if (_registeredCroSecName[2].IndexOf(kModel.crosecs[croSecId].name) < 0)
            {
                StbSecSteel.GetSection(ref _secSteel, kModel, croSecId);
                _registeredCroSecName[2].Add(kModel.crosecs[croSecId].name);
            }
        }

        private void AddBeamSection(string kind, int croSecId, int gNum, Karamba.Models.Model kModel)
        {
            switch (kind)
            {
                case "S":
                    _sections.Add(StbSection.GetBeamSt(croSecId, gNum, kModel));

                    if (_registeredCroSecName[1].IndexOf(kModel.crosecs[croSecId].name) < 0)
                    {
                        StbSecSteel.GetSection(ref _secSteel, kModel, croSecId);
                        _registeredCroSecName[1].Add(kModel.crosecs[croSecId].name);
                    }
                    break;
                case "RC":
                    _sections.Add(StbSection.GetBeamRc(croSecId, gNum, kModel));
                    break;
                default:
                    throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
            }
            _registeredCroSecId[1].Add(croSecId);
        }

        private void AddColumnSection(string kind, int croSecId, int cNum, Karamba.Models.Model kModel)
        {
            switch (kind)
            {
                case "S":
                    _sections.Add(StbSection.GetColumnSt(croSecId, cNum, kModel));

                    if (_registeredCroSecName[0].IndexOf(kModel.crosecs[croSecId].name) < 0)
                    {
                        StbSecSteel.GetSection(ref _secSteel, kModel, croSecId);
                        _registeredCroSecName[0].Add(kModel.crosecs[croSecId].name);
                    }
                    break;
                case "RC":
                    _sections.Add(StbSection.GetColumnRc(croSecId, cNum, kModel));
                    break;
                default:
                    throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
            }
            _registeredCroSecId[0].Add(croSecId);
        }

        private static string GetElementKind(string materialFamily)
        {
            switch (materialFamily)
            {
                case "Steel":
                    return "S";
                case "Concrete":
                    return "RC";
                default:
                    return string.Empty;
            }
        }
    }
}