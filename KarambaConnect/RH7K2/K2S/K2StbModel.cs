using System;
using System.Collections.Generic;
using System.Linq;
using Karamba.Elements;
using Rhino.Geometry;
using STBDotNet.v202;

namespace KarambaConnect.K2S
{
    public class K2StbModel
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
        private readonly int[] _tagNum = { 1, 1, 1 };
        private readonly List<string> _croSecNames = new List<string>();
        private readonly Karamba.Models.Model _kModel;

        public K2StbModel(Karamba.Models.Model kModel)
        {
            _kModel = kModel;
            _croSecNames = _kModel.crosecs.Select(sec => sec.name).ToList();
        }

        public StbModel SetByAngle(double colMaxAngle)
        {
            // TODO: リストが大量にあるの直す
            var columns = new List<StbColumn>();
            var girders = new List<StbGirder>();
            var braces = new List<StbBrace>();
            var secBeams_S = new List<StbSecBeam_S>();
            var secBeams_Rc = new List<StbSecBeam_RC>();
            var secColumn_S = new List<StbSecColumn_S>();
            var secColumn_Rc = new List<StbSecColumn_RC>();
            var secBrace_S = new List<StbSecBrace_S>();
            var secSteel = new K2SSecSteelItems();

            foreach (ModelElement elem in _kModel.elems)
            {
                if (!(elem is ModelElementStraightLine))
                {
                    continue;
                }
                Karamba.Geometry.Point3 to = _kModel.nodes[elem.node_inds[0]].pos;
                Karamba.Geometry.Point3 from = _kModel.nodes[elem.node_inds[1]].pos;
                var elemLine = new Line(new Point3d(to.X, to.Y, to.Z), new Point3d(from.X, from.Y, from.Z));
                double pAngle = Vector3d.VectorAngle(elemLine.Direction, Vector3d.ZAxis);
                double nAngle = Vector3d.VectorAngle(elemLine.Direction, -Vector3d.ZAxis);

                switch (elem)
                {
                    case ModelBeam modelBeam:
                        ModelBeamToStbColumnAndGirder(secBeams_Rc, secBeams_S, secColumn_Rc, secColumn_S, secSteel, colMaxAngle, columns, girders, pAngle, nAngle, modelBeam);
                        break;
                    case ModelTruss modelTruss:
                        ModelTrussToStbBrace(secBrace_S, secSteel, braces, modelTruss);
                        break;
                    default:
                        throw new ArgumentException("Karamba3D model parse error.");
                }
            }

            StbMembers members = BindMemberProps(columns, girders, braces);
            StbSections sections = BindSectionProps(secBeams_S, secBeams_Rc, secColumn_S, secColumn_Rc, secBrace_S, secSteel);

            return new StbModel() { StbNodes = _kModel.nodes.ToStb(), StbMembers = members, StbSections = sections };
        }

        private static StbMembers BindMemberProps(IEnumerable<StbColumn> columns, IEnumerable<StbGirder> girders, IEnumerable<StbBrace> braces)
        {
            return new StbMembers()
            {
                StbColumns = columns.ToArray(),
                StbGirders = girders.ToArray(),
                StbBraces = braces.ToArray()
            };
        }

        private static StbSections BindSectionProps(
            IEnumerable<StbSecBeam_S> secBeams_S, IEnumerable<StbSecBeam_RC> secBeams_Rc,
            IEnumerable<StbSecColumn_S> secColumn_S, IEnumerable<StbSecColumn_RC> secColumn_Rc,
            IEnumerable<StbSecBrace_S> secBrace_S,
            K2SSecSteelItems secSteelItems)
        {
            return new StbSections
            {
                StbSecColumn_S = secColumn_S.ToArray(),
                StbSecColumn_RC = secColumn_Rc.ToArray(),
                StbSecBeam_S = secBeams_S.ToArray(),
                StbSecBeam_RC = secBeams_Rc.ToArray(),
                StbSecBrace_S = secBrace_S.ToArray(),
                StbSecSteel = secSteelItems.ToStb(),
            };
        }

        private void ModelTrussToStbBrace(List<StbSecBrace_S> secBrace_S, K2SSecSteelItems secSteel, ICollection<StbBrace> braces, ModelTruss modelTruss)
        {
            int trussCroSecId = _croSecNames.IndexOf(modelTruss.crosec.name);
            braces.Add(K2StbMemberItems.CreateBrace(modelTruss, trussCroSecId));
            if (_registeredCroSecId[2].IndexOf(trussCroSecId) < 0)
            {
                AddBraceSection(secBrace_S, secSteel, trussCroSecId, _tagNum[2]++);
            }
        }

        private void ModelBeamToStbColumnAndGirder(
            List<StbSecBeam_RC> secBeams_RC, List<StbSecBeam_S> secBeams_S, List<StbSecColumn_RC> secColumn_RC, List<StbSecColumn_S> secColumn_S, K2SSecSteelItems secSteel,
            double colMaxAngle, ICollection<StbColumn> columns, ICollection<StbGirder> girders, double pAngle, double nAngle, ModelBeam modelBeam)
        {
            int croSecId = _croSecNames.IndexOf(modelBeam.crosec.name);
            bool positive = pAngle <= colMaxAngle && pAngle >= -1d * colMaxAngle;
            bool negative = nAngle <= colMaxAngle && nAngle >= -1d * colMaxAngle;

            if (positive || negative)
            {
                var kind = GetColumnStructureKind(modelBeam.crosec.material.family);
                columns.Add(K2StbMemberItems.CreateColumn(modelBeam, croSecId, kind));
                if (_registeredCroSecId[0].IndexOf(croSecId) < 0)
                {
                    AddColumnSection(secColumn_S, secColumn_RC, secSteel, kind, croSecId, _tagNum[0]++);
                }
            }
            else
            {
                var kind = GetGirderStructureKind(modelBeam.crosec.material.family);
                girders.Add(K2StbMemberItems.CreateGirder(modelBeam, croSecId, kind));
                if (_registeredCroSecId[1].IndexOf(croSecId) < 0)
                {
                    AddBeamSection(secBeams_S, secBeams_RC, secSteel, kind, croSecId, _tagNum[1]++);
                }
            }
        }

        private void AddBraceSection(ICollection<StbSecBrace_S> secBrace_S, K2SSecSteelItems secSteel, int croSecId, int vNum)
        {
            secBrace_S.Add(K2StbSections.BraceSteel(croSecId, vNum, _kModel));
            _registeredCroSecId[2].Add(croSecId);

            if (_registeredCroSecName[2].IndexOf(_kModel.crosecs[croSecId].name) < 0)
            {
                K2StbSecSteel.GetSection(ref secSteel, _kModel, croSecId);
                _registeredCroSecName[2].Add(_kModel.crosecs[croSecId].name);
            }
        }

        private void AddBeamSection(ICollection<StbSecBeam_S> secBeams_S, ICollection<StbSecBeam_RC> secBeams_RC, K2SSecSteelItems secSteel, StbGirderKind_structure kind, int croSecId, int gNum)
        {
            switch (kind)
            {
                case StbGirderKind_structure.S:
                    secBeams_S.Add(K2StbSections.BeamSteel(croSecId, gNum, _kModel));

                    if (_registeredCroSecName[1].IndexOf(_kModel.crosecs[croSecId].name) < 0)
                    {
                        K2StbSecSteel.GetSection(ref secSteel, _kModel, croSecId);
                        _registeredCroSecName[1].Add(_kModel.crosecs[croSecId].name);
                    }
                    break;
                case StbGirderKind_structure.RC:
                    secBeams_RC.Add(K2StbSections.BeamRc(croSecId, gNum, _kModel));
                    break;
                default:
                    throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
            }
            _registeredCroSecId[1].Add(croSecId);
        }

        private void AddColumnSection(ICollection<StbSecColumn_S> secColumns_S, ICollection<StbSecColumn_RC> secColumns_RC, K2SSecSteelItems secSteel, StbColumnKind_structure kind, int croSecId, int cNum)
        {
            switch (kind)
            {
                case StbColumnKind_structure.S:
                    secColumns_S.Add(K2StbSections.ColumnSteel(croSecId, cNum, _kModel));

                    if (_registeredCroSecName[0].IndexOf(_kModel.crosecs[croSecId].name) < 0)
                    {
                        K2StbSecSteel.GetSection(ref secSteel, _kModel, croSecId);
                        _registeredCroSecName[0].Add(_kModel.crosecs[croSecId].name);
                    }
                    break;
                case StbColumnKind_structure.RC:
                    secColumns_RC.Add(K2StbSections.ColumnRc(croSecId, cNum, _kModel));
                    break;
                default:
                    throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
            }
            _registeredCroSecId[0].Add(croSecId);
        }

        private static StbColumnKind_structure GetColumnStructureKind(string materialFamily)
        {
            switch (materialFamily)
            {
                case "Steel":
                    return StbColumnKind_structure.S;
                case "Concrete":
                    return StbColumnKind_structure.RC;
                default:
                    throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
            }
        }

        private static StbGirderKind_structure GetGirderStructureKind(string materialFamily)
        {
            switch (materialFamily)
            {
                case "Steel":
                    return StbGirderKind_structure.S;
                case "Concrete":
                    return StbGirderKind_structure.RC;
                default:
                    throw new ArgumentException("Make sure that the family name of the material is \"Concrete\" or \"Steel\".");
            }
        }
    }
}
