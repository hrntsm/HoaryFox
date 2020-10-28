using System;
using Rhino.Geometry;

namespace HoaryFox.Member
{
    public readonly struct ShapeInfo : IEquatable<ShapeInfo>
    {
        public Point3d NodeStart { get; }
        public Point3d NodeEnd { get; }
        public double Height { get; }
        public double Width { get; }
        public double Rotate { get; }
        
        public ShapeInfo(Point3d nodeStart, Point3d nodeEnd, double height, double width, double rotate)
        {
            NodeStart = nodeStart;
            NodeEnd = nodeEnd;
            Height = height;
            Width = width;
            Rotate = rotate;
        }

        public bool Equals(ShapeInfo other)
        {
            return NodeStart.Equals(other.NodeStart) && NodeEnd.Equals(other.NodeEnd) && Height.Equals(other.Height) && Width.Equals(other.Width) && Rotate.Equals(other.Rotate);
        }

        public override bool Equals(object obj)
        {
            return obj is ShapeInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = NodeStart.GetHashCode();
                hashCode = (hashCode * 397) ^ NodeEnd.GetHashCode();
                hashCode = (hashCode * 397) ^ Height.GetHashCode();
                hashCode = (hashCode * 397) ^ Width.GetHashCode();
                hashCode = (hashCode * 397) ^ Rotate.GetHashCode();
                return hashCode;
            }
        }
    }
}