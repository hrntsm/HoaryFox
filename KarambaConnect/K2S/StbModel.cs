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
        private readonly Members _members = new Members
        {
            Columns = new List<Column>(), Girders = new List<Girder>(), Braces = new List<Brace>()
        };
        private readonly List<Section> _sections = new List<Section>();
        private Steel _secSteel = new Steel();

        private readonly int[] _tagNum = { 1, 1, 1 };
        private readonly List<string> _croSecNames = new List<string>();
        private readonly Karamba.Models.Model _kModel;

        public StbModel(Karamba.Models.Model kModel)
        {
            _kModel = kModel;
            _croSecNames = _kModel.crosecs.Select(sec => sec.name).ToList();
        }

        public Model SetByAngle(double colMaxAngle)
        {
            foreach (ModelElement elem in _kModel.elems)
            {
                if (!(elem is ModelElementStraightLine))
                {
                    continue;
                }
                var elemLine = new Line(_kModel.nodes[elem.node_inds[0]].pos.Convert(), _kModel.nodes[elem.node_inds[1]].pos.Convert());
                double angle = Vector3d.VectorAngle(elemLine.Direction, Vector3d.ZAxis);

                switch (elem)
                {
                    case ModelBeam modelBeam:
                        AddModelBeam(modelBeam, angle, colMaxAngle);
                        break;
                    case ModelTruss modelTruss:
                        AddModelTruss(modelTruss);
                        break;
                    default:
                        throw new ArgumentException("Karamba3D model parse error.");
                }
            }

            _sections.Add(_secSteel);
            return new Model { Members = _members, Sections = _sections };
        }

        private void AddModelBeam(ModelBeam modelBeam, double angle, double colMaxAngle)
        {
            int croSecId = _croSecNames.IndexOf(modelBeam.crosec.name);
            string kind = GetElementKind(modelBeam.crosec.material.family);

            if (angle <= colMaxAngle && angle >= -1d * colMaxAngle)
            {
                _members.Columns.Add(StbMember.CreateColumn(modelBeam, croSecId, kind));
                if (_registeredCroSecId[0].IndexOf(croSecId) < 0)
                {
                    AddColumnSection(kind, croSecId, _tagNum[0]++);
                }
            }
            else
            {
                _members.Girders.Add(StbMember.CreateGirder(modelBeam, croSecId, kind));
                if (_registeredCroSecId[1].IndexOf(croSecId) < 0)
                {
                    AddBeamSection(kind, croSecId, _tagNum[1]++);
                }
            }
        }

        private void AddModelTruss(ModelTruss modelTruss)
        {
            int croSecId = _croSecNames.IndexOf(modelTruss.crosec.name);
            _members.Braces.Add(StbMember.CreateBrace(modelTruss, croSecId));
            if (_registeredCroSecId[2].IndexOf(croSecId) < 0)
            {
                AddBraceSection(croSecId, _tagNum[2]++);
            }
        }

        private void AddBraceSection(int croSecId, int vNum)
        {
            _sections.Add(StbSection.GetBraceSt(croSecId, vNum, _kModel));
            _registeredCroSecId[2].Add(croSecId);

            if (_registeredCroSecName[2].IndexOf(_kModel.crosecs[croSecId].name) < 0)
            {
                StbSecSteel.GetSection(ref _secSteel, _kModel, croSecId);
                _registeredCroSecName[2].Add(_kModel.crosecs[croSecId].name);
            }
        }

        private void AddBeamSection(string kind, int croSecId, int gNum)
        {
            switch (kind)
            {
                case "S":
                    _sections.Add(StbSection.GetBeamSt(croSecId, gNum, _kModel));

                    if (_registeredCroSecName[1].IndexOf(_kModel.crosecs[croSecId].name) < 0)
                    {
                        StbSecSteel.GetSection(ref _secSteel, _kModel, croSecId);
                        _registeredCroSecName[1].Add(_kModel.crosecs[croSecId].name);
                    }
                    break;
                case "RC":
                    _sections.Add(StbSection.GetBeamRc(croSecId, gNum, _kModel));
                    break;
                default:
                    throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
            }
            _registeredCroSecId[1].Add(croSecId);
        }

        private void AddColumnSection(string kind, int croSecId, int cNum)
        {
            switch (kind)
            {
                case "S":
                    _sections.Add(StbSection.GetColumnSt(croSecId, cNum, _kModel));

                    if (_registeredCroSecName[0].IndexOf(_kModel.crosecs[croSecId].name) < 0)
                    {
                        StbSecSteel.GetSection(ref _secSteel, _kModel, croSecId);
                        _registeredCroSecName[0].Add(_kModel.crosecs[croSecId].name);
                    }
                    break;
                case "RC":
                    _sections.Add(StbSection.GetColumnRc(croSecId, cNum, _kModel));
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