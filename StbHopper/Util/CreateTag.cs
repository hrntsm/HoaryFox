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

        public GH_Structure<GH_String> Frame(StbFrame frame)
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
                int nodeIndexStart = _nodes.Id.IndexOf(frame.IdNodeStart[eNum]);
                int nodeIndexEnd = _nodes.Id.IndexOf(frame.IdNodeEnd[eNum]);
                Point3d nodeStart = new Point3d(_nodes.X[nodeIndexStart], _nodes.Y[nodeIndexStart], _nodes.Z[nodeIndexStart]);
                Point3d nodeEnd = new Point3d(_nodes.X[nodeIndexEnd], _nodes.Y[nodeIndexEnd], _nodes.Z[nodeIndexEnd]);
                TagPos.Add(new Point3d((nodeStart.X + nodeEnd.X) / 2.0,
                    (nodeStart.Y + nodeEnd.Y) / 2.0,
                    (nodeStart.Z + nodeEnd.Z) / 2.0));

                int secIndex;
                ShapeTypes shapeType = ShapeTypes.BOX;
                switch (kind)
                {
                    case KindsStructure.RC:
                        switch (frame.FrameType)
                        {
                            case FrameType.Column:
                            case FrameType.Post:
                                secIndex = Stb2ColSec.SecColumnRc.Id.IndexOf(idSection);
                                name = Stb2ColSec.SecColumnRc.Name[secIndex];
                                p1 = Stb2ColSec.SecColumnRc.Height[secIndex];
                                p2 = Stb2ColSec.SecColumnRc.Width[secIndex];
                                break;
                            case FrameType.Girder:
                            case FrameType.Beam:
                                secIndex = Stb2ColSec.SecBeamRc.Id.IndexOf(idSection);
                                name = Stb2ColSec.SecBeamRc.Name[secIndex];
                                p1 = Stb2ColSec.SecBeamRc.Depth[secIndex];
                                p2 = Stb2ColSec.SecBeamRc.Width[secIndex];
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
                                idShape = Stb2ColSec.SecColumnS.Id.IndexOf(idSection);
                                shape = Stb2ColSec.SecColumnS.Shape[idShape];
                                break;
                            case FrameType.Girder:
                            case FrameType.Beam:
                                idShape = Stb2ColSec.SecBeamS.Id.IndexOf(idSection);
                                shape = Stb2ColSec.SecBeamS.Shape[idShape];
                                break;
                            case FrameType.Brace:
                                idShape = Stb2ColSec.SecBraceS.Id.IndexOf(idSection);
                                shape = Stb2ColSec.SecBraceS.Shape[idShape];
                                break;
                        }

                        secIndex = Stb2ColSec.StbSecSteel.Name.IndexOf(shape);
                        name = Stb2ColSec.StbSecSteel.Name[secIndex];
                        p1 = Stb2ColSec.StbSecSteel.P1[secIndex];
                        p2 = Stb2ColSec.StbSecSteel.P2[secIndex];
                        p3 = Stb2ColSec.StbSecSteel.P3[secIndex];
                        p4 = Stb2ColSec.StbSecSteel.P4[secIndex];
                        shapeType = Stb2ColSec.StbSecSteel.ShapeType[secIndex];
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
