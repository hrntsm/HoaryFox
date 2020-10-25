using System;
using System.Collections.Generic;
using System.Globalization;
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
        public List<Point3d> TagPos { get; } = new List<Point3d>();

        public CreateTag(StbNodes nodes)
        {
            this._nodes = nodes;
        }

        public GH_Structure<GH_String> Frame(StbFrame frame, StbSecColumnRc columnRc, StbSecColumnS colS, StbSecBeamRc beamRc, StbSecBeamS beamS, StbSecBraceS braceS, StbSecSteel secSteel)
        {
            var sec = new GH_Structure<GH_String>();
        
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;
            double p4 = 0;
            var shape = string.Empty;
            var name = string.Empty;
        
            for (var eNum = 0; eNum < frame.Id.Count; eNum++)
            {
                int idSection = frame.IdSection[eNum];
                KindsStructure kind = frame.KindStructure[eNum];
                var ghPath = new GH_Path(new[]{eNum});
        
                // 始点と終点の座標取得
                int startIndex = _nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
                int endIndex = _nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
                var nodeStart = new Point3d(_nodes.X[startIndex], _nodes.Y[startIndex], _nodes.Z[startIndex]);
                var nodeEnd = new Point3d(_nodes.X[endIndex], _nodes.Y[endIndex], _nodes.Z[endIndex]);
                TagPos.Add(new Point3d((nodeStart.X + nodeEnd.X) / 2.0, (nodeStart.Y + nodeEnd.Y) / 2.0, (nodeStart.Z + nodeEnd.Z) / 2.0));
        
                int secIndex;
                var shapeType = ShapeTypes.BOX;
                switch (kind)
                {
                    case KindsStructure.Rc:
                        switch (frame.FrameType)
                        {
                            case FrameType.Column:
                            case FrameType.Post:
                                secIndex = columnRc.Id.IndexOf(idSection);
                                name = columnRc.Name[secIndex];
                                p1 = columnRc.Height[secIndex];
                                p2 = columnRc.Width[secIndex];
                                break;
                            case FrameType.Girder:
                            case FrameType.Beam:
                                secIndex = beamRc.Id.IndexOf(idSection);
                                name = beamRc.Name[secIndex];
                                p1 = beamRc.Depth[secIndex];
                                p2 = beamRc.Width[secIndex];
                                p3 = 0;
                                p4 = 0;
                                break;
                            case FrameType.Brace:
                                break;
                            case FrameType.Slab:
                                break;
                            case FrameType.Wall:
                                break;
                            case FrameType.Any:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        shapeType = p1 <= 0 ? ShapeTypes.Pipe : ShapeTypes.BOX;
                        break;
                    case KindsStructure.S:
                    {
                        int idShape;
                        switch (frame.FrameType)
                        {
                            case FrameType.Column:
                            case FrameType.Post:
                                idShape = colS.Id.IndexOf(idSection);
                                shape = colS.Shape[idShape];
                                break;
                            case FrameType.Girder:
                            case FrameType.Beam:
                                idShape = beamS.Id.IndexOf(idSection);
                                shape = beamS.Shape[idShape];
                                break;
                            case FrameType.Brace:
                                idShape = braceS.Id.IndexOf(idSection);
                                shape = braceS.Shape[idShape];
                                break;
                            case FrameType.Slab:
                                break;
                            case FrameType.Wall:
                                break;
                            case FrameType.Any:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
        
                        secIndex = secSteel.Name.IndexOf(shape);
                        name = secSteel.Name[secIndex];
                        p1 = secSteel.P1[secIndex];
                        p2 = secSteel.P2[secIndex];
                        p3 = secSteel.P3[secIndex];
                        p4 = secSteel.P4[secIndex];
                        shapeType = secSteel.ShapeType[secIndex];
                        break;
                    }
                    case KindsStructure.Src:
                        break;
                    case KindsStructure.Cft:
                        break;
                    case KindsStructure.Deck:
                        break;
                    case KindsStructure.Precast:
                        break;
                    case KindsStructure.Other:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                        
                sec.Append(new GH_String(name), ghPath);
                sec.Append(new GH_String(shapeType.ToString()), ghPath);
                sec.Append(new GH_String(p1.ToString(CultureInfo.InvariantCulture)), ghPath);
                sec.Append(new GH_String(p2.ToString(CultureInfo.InvariantCulture)), ghPath);
                sec.Append(new GH_String(p3.ToString(CultureInfo.InvariantCulture)), ghPath);
                sec.Append(new GH_String(p4.ToString(CultureInfo.InvariantCulture)), ghPath);
            }
        
            return sec;
        }
    }
}
