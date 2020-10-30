using System;
using System.Collections.Generic;
using System.Globalization;
using Grasshopper.GUI;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using STBReader;
using STBReader.Member;
using STBReader.Model;
using STBReader.Section;
using Rhino.Geometry;

namespace HoaryFox.Member
{
    public class CreateTag
    {
        private readonly StbNodes _nodes;
        private readonly StbSecColumnRc _colRc;
        private readonly StbSecColumnS _colS;
        private readonly StbSecBeamRc _beamRc;
        private readonly StbSecBeamS _beamS;
        private readonly StbSecBraceS _braceS;
        private readonly StbSecSteel _secSteel;
        public List<Point3d> Position { get; } = new List<Point3d>();

        public CreateTag(StbNodes nodes, StbSecColumnRc colRc, StbSecColumnS colS, StbSecBeamRc beamRc, StbSecBeamS beamS, StbSecBraceS braceS, StbSecSteel secSteel)
        {
            _nodes = nodes;
            _secSteel = secSteel;
            _braceS = braceS;
            _beamS = beamS;
            _beamRc = beamRc;
            _colS = colS;
            _colRc = colRc;
        }

        public GH_Structure<GH_String> Frame(StbFrame frameData)
        {
            var ghSecStrings = new GH_Structure<GH_String>();

            for (var eNum = 0; eNum < frameData.Id.Count; eNum++)
            {
                TagInfo tagInfo;
                int idSection = frameData.IdSection[eNum];
                var ghPath = new GH_Path(new[]{eNum});
                KindsStructure kind = frameData.KindStructure[eNum];
                SetTagPosition(frameData, eNum);

                switch (kind)
                {
                    case KindsStructure.Rc:
                        tagInfo = TagRc(frameData, idSection);
                        break;
                    case KindsStructure.S:
                        tagInfo = TagSteel(frameData, idSection);
                        break;
                    case KindsStructure.Src:
                    case KindsStructure.Cft:
                    case KindsStructure.Deck:
                    case KindsStructure.Precast:
                    case KindsStructure.Other:
                        throw new ArgumentException("Wrong kind structure");
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                        
                ghSecStrings.Append(new GH_String(tagInfo.Name), ghPath);
                ghSecStrings.Append(new GH_String(tagInfo.ShapeTypes.ToString()), ghPath);
                ghSecStrings.Append(new GH_String(tagInfo.P1.ToString(CultureInfo.InvariantCulture)), ghPath);
                ghSecStrings.Append(new GH_String(tagInfo.P2.ToString(CultureInfo.InvariantCulture)), ghPath);
                ghSecStrings.Append(new GH_String(tagInfo.P3.ToString(CultureInfo.InvariantCulture)), ghPath);
                ghSecStrings.Append(new GH_String(tagInfo.P4.ToString(CultureInfo.InvariantCulture)), ghPath);
            }
        
            return ghSecStrings;
        }

        private void SetTagPosition(StbFrame frame, int eNum)
        {
            // 始点と終点の座標取得
            int startIndex = _nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
            int endIndex = _nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
            var nodeStart = new Point3d(_nodes.X[startIndex], _nodes.Y[startIndex], _nodes.Z[startIndex]);
            var nodeEnd = new Point3d(_nodes.X[endIndex], _nodes.Y[endIndex], _nodes.Z[endIndex]);
            Position.Add(new Point3d((nodeStart.X + nodeEnd.X) / 2.0, (nodeStart.Y + nodeEnd.Y) / 2.0, (nodeStart.Z + nodeEnd.Z) / 2.0));
        }

        private TagInfo TagSteel(StbFrame frame, int idSection)
        {
            int idShape;
            string shapeName;
            switch (frame.FrameType)
            {
                case FrameType.Column:
                case FrameType.Post:
                    idShape = _colS.Id.IndexOf(idSection);
                    shapeName = _colS.Shape[idShape];
                    break;
                case FrameType.Girder:
                case FrameType.Beam:
                    idShape = _beamS.Id.IndexOf(idSection);
                    shapeName = _beamS.Shape[idShape];
                    break;
                case FrameType.Brace:
                    idShape = _braceS.Id.IndexOf(idSection);
                    shapeName = _braceS.Shape[idShape];
                    break;
                case FrameType.Slab:
                case FrameType.Wall:
                case FrameType.Any:
                    throw new ArgumentException("Wrong frame type");
                default:
                    throw new ArgumentOutOfRangeException();
            }

            int secIndex = _secSteel.Name.IndexOf(shapeName);
            var tagInfo = new TagInfo
            {
                Name = _secSteel.Name[secIndex],
                ShapeTypes = _secSteel.ShapeType[secIndex],
                P1 = _secSteel.P1[secIndex],
                P2 = _secSteel.P2[secIndex],
                P3 = _secSteel.P3[secIndex],
                P4 = _secSteel.P4[secIndex]
            };
            return tagInfo;
        }

        private TagInfo TagRc(StbFrame frame, int idSection)
        {
            int secIndex;
            TagInfo tagInfo;
            switch (frame.FrameType)
            {
                case FrameType.Column:
                case FrameType.Post:
                    secIndex = _colRc.Id.IndexOf(idSection);
                    tagInfo = new TagInfo(_colRc.Name[secIndex], _colRc.Height[secIndex], _colRc.Width[secIndex], 0d, 0d);
                    break;
                case FrameType.Girder:
                case FrameType.Beam:
                    secIndex = _beamRc.Id.IndexOf(idSection);
                    tagInfo = new TagInfo(_beamRc.Name[secIndex], _beamRc.Depth[secIndex], _beamRc.Width[secIndex], 0d, 0d);
                    break;
                case FrameType.Brace:
                case FrameType.Slab:
                case FrameType.Wall:
                case FrameType.Any:
                    throw new ArgumentException("Wrong frame type");
                default:
                    throw new ArgumentOutOfRangeException();
            }
            tagInfo.ShapeTypes = tagInfo.P1 <= 0 ? ShapeTypes.Pipe : ShapeTypes.BOX;
            return tagInfo;
        }
    }

    public class TagInfo
    {
        public string Name { get; set; }
        public ShapeTypes ShapeTypes { get;  set; }
        public double P1 { get; set; }
        public double P2 { get; set; }
        public double P3 { get; set; }
        public double P4 { get; set; }

        public TagInfo()
        {
        }

        public TagInfo(string name, double p1, double p2, double p3, double p4)
        {
            Name = name;
            P1 = p1;
            P2 = p2;
            P3 = p3;
            P4 = p4;
        }
    }
}
