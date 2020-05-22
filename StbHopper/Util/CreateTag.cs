using System;
using System.Collections.Generic;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using StbHopper.Component;
using StbHopper.Component.SectionTag;
using StbHopper.STB;

namespace StbHopper.Util
{
    public class CreateTag
    {
        private readonly StbNodes _nodes;
        public List<Point3d> TagPos { get; } = new List<Point3d>();

        public CreateTag(StbNodes nodes)
        {
            this._nodes = nodes;
        }

        public GH_Structure<GH_String> Frame(StbFrame frame, StbSecColRC colRc, StbSecColumnS colS, StbSecBeamRC beamRc, StbSecBeamS beamS, StbSecBraceS braceS, StbSecSteel secSteel)
        {
            GH_Structure<GH_String> sec = new GH_Structure<GH_String>();
        
            double p1 = 0;
            double p2 = 0;
            double p3 = 0;
            double p4 = 0;
            string shape = string.Empty;
            string name = string.Empty;
        
            for (int eNum = 0; eNum < frame.Id.Count; eNum++)
            {
                int idSection = frame.IdSection[eNum];
                KindsStructure kind = frame.KindStructure[eNum];
                GH_Path ghPath = new GH_Path(new int[]{eNum});
        
                // 始点と終点の座標取得
                int startIndex = _nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
                int endIndex = _nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
                Point3d nodeStart = new Point3d(_nodes.X[startIndex], _nodes.Y[startIndex], _nodes.Z[startIndex]);
                Point3d nodeEnd = new Point3d(_nodes.X[endIndex], _nodes.Y[endIndex], _nodes.Z[endIndex]);
                TagPos.Add(new Point3d((nodeStart.X + nodeEnd.X) / 2.0, (nodeStart.Y + nodeEnd.Y) / 2.0, (nodeStart.Z + nodeEnd.Z) / 2.0));
        
                int secIndex;
                ShapeTypes shapeType = ShapeTypes.BOX;
                switch (kind)
                {
                    case KindsStructure.RC:
                        switch (frame.FrameType)
                        {
                            case FrameType.Column:
                            case FrameType.Post:
                                secIndex = colRc.Id.IndexOf(idSection);
                                name = colRc.Name[secIndex];
                                p1 = colRc.Height[secIndex];
                                p2 = colRc.Width[secIndex];
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
                }
                        
                sec.Append(new GH_String(name), ghPath);
                sec.Append(new GH_String(shapeType.ToString()), ghPath);
                sec.Append(new GH_String(p1.ToString()), ghPath);
                sec.Append(new GH_String(p2.ToString()), ghPath);
                sec.Append(new GH_String(p3.ToString()), ghPath);
                sec.Append(new GH_String(p4.ToString()), ghPath);
            }
        
            return sec;
        }
    }
}
