using System;
using System.Collections.Generic;
using STBReader;
using STBReader.Member;
using STBReader.Model;
using STBReader.Section;
using Rhino.Geometry;

namespace HoaryFox.Member
{
    public class FrameBreps
    {
        private readonly StbData _stbData;

        public FrameBreps(StbData stbData)
        {
            _stbData = stbData;
        }
        
        public List<Brep> Slab(StbSlabs slabs)
        {
            var brep = new List<Brep>();
            var count = 0;

            foreach (List<int> nodeIds in slabs.NodeIdList)
            {
                var index = new int[nodeIds.Count];
                var pts = new Point3d[nodeIds.Count];
                double offset = slabs.Level[count];

                for (var i = 0; i < nodeIds.Count; i++)
                {
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pts[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]] + offset);
                }
                
                brep.Add(CreateBreps.PlaneWithOpens(_stbData, pts, null));
                count++;
            }

            return brep;
        }

        public List<Brep> Wall(StbWalls walls)
        {
            var brep = new List<Brep>();
            var count = 0;

            foreach (List<int> nodeIds in walls.NodeIdList)
            {
                var index = new int[nodeIds.Count];
                var pts = new Point3d[nodeIds.Count];

                for (var i = 0; i < nodeIds.Count; i++)
                {
                    index[i] = _stbData.Nodes.Id.IndexOf(nodeIds[i]);
                    pts[i] = new Point3d(_stbData.Nodes.X[index[i]], _stbData.Nodes.Y[index[i]], _stbData.Nodes.Z[index[i]]);
                }

                brep.Add(CreateBreps.PlaneWithOpens(_stbData, pts, walls.Opens[count]));
                count++;
            }

            return brep;
        }

        public List<Brep> Frame(StbFrame frameData)
        {
            var brep = new List<Brep>();

            double height = -1;
            double width = -1;
            var shape = string.Empty;
            var shapeType = ShapeTypes.H;

            for (var eNum = 0; eNum < frameData.Id.Count; eNum++)
            {
                int idSection = frameData.IdSection[eNum];
                KindsStructure kind = frameData.KindStructure[eNum];
                double rotate = frameData.Rotate[eNum];

                // 始点と終点の座標取得
                int nodeIndexStart = _stbData.Nodes.Id.IndexOf(frameData.IdNodeStart[eNum]);
                int nodeIndexEnd = _stbData.Nodes.Id.IndexOf(frameData.IdNodeEnd[eNum]);
                var nodeStart = new Point3d(_stbData.Nodes.X[nodeIndexStart], _stbData.Nodes.Y[nodeIndexStart], _stbData.Nodes.Z[nodeIndexStart]);
                var nodeEnd = new Point3d(_stbData.Nodes.X[nodeIndexEnd], _stbData.Nodes.Y[nodeIndexEnd], _stbData.Nodes.Z[nodeIndexEnd]);

                int secIndex;
                switch (kind)
                {
                    case KindsStructure.Rc:
                        switch (frameData.FrameType)
                        {
                            case FrameType.Column:
                            case FrameType.Post:
                                secIndex = _stbData.SecColumnRc.Id.IndexOf(idSection);
                                height = _stbData.SecColumnRc.Height[secIndex];
                                width = _stbData.SecColumnRc.Width[secIndex];
                                break;
                            case FrameType.Girder:
                            case FrameType.Beam:
                                secIndex = _stbData.SecBeamRc.Id.IndexOf(idSection);
                                height = _stbData.SecBeamRc.Depth[secIndex];
                                width = _stbData.SecBeamRc.Width[secIndex];
                                break;
                            case FrameType.Brace:
                            case FrameType.Slab:
                            case FrameType.Wall:
                            case FrameType.Any:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        shapeType = height <= 0 ? ShapeTypes.Pipe : ShapeTypes.BOX;
                        break;
                    case KindsStructure.S:
                    {
                        int idShape;
                        switch (frameData.FrameType)
                        {
                            case FrameType.Column:
                            case FrameType.Post:
                                idShape = _stbData.SecColumnS.Id.IndexOf(idSection);
                                shape = _stbData.SecColumnS.Shape[idShape];
                                break;
                            case FrameType.Girder:
                            case FrameType.Beam:
                                idShape = _stbData.SecBeamS.Id.IndexOf(idSection);
                                shape = _stbData.SecBeamS.Shape[idShape];
                                break;
                            case FrameType.Brace:
                                idShape = _stbData.SecBraceS.Id.IndexOf(idSection);
                                shape = _stbData.SecBraceS.Shape[idShape];
                                break;
                            case FrameType.Slab:
                            case FrameType.Wall:
                            case FrameType.Any:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        secIndex = _stbData.SecSteel.Name.IndexOf(shape);
                        height = _stbData.SecSteel.P1[secIndex];
                        width = _stbData.SecSteel.P2[secIndex];
                        shapeType = _stbData.SecSteel.ShapeType[secIndex];
                        break;
                    }
                    case KindsStructure.Src:
                    case KindsStructure.Cft:
                    case KindsStructure.Deck:
                    case KindsStructure.Precast:
                    case KindsStructure.Other:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                brep.AddRange(CreateBreps.FromEndPoint(_stbData, nodeStart, nodeEnd, height, width, rotate, shapeType,  frameData.FrameType));
            }

            return brep;
        }
    }
}
