using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;
using STBReader;
using STBReader.Member;
using STBReader.Model;

namespace HoaryFox.Component.Base
{
    public class NameTagBase : GH_Component
    {
        private StbData _stbData;
        private int _size;
        private readonly FrameType _frameType;

        private readonly List<string> _frameName = new List<string>();
        private readonly List<Point3d> _framePos = new List<Point3d>();

        public override bool IsPreviewCapable => true;

        protected NameTagBase(string name, string nickname, string description, FrameType frameType)
            : base(name, nickname, description, category: "HoaryFox", subCategory: "NameTag")
        {
            _frameType = frameType;
        }

        public override void ClearData()
        {
            base.ClearData();
            _frameName.Clear();
            _framePos.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("NameTag", "NTag", "output name tag", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            StbNodes nodes = _stbData.Nodes;
            StbFrame frame;
            switch (_frameType)
            {
                case FrameType.Column: frame = _stbData.Columns; break;
                case FrameType.Post: frame = _stbData.Posts; break;
                case FrameType.Girder: frame = _stbData.Girders; break;
                case FrameType.Beam: frame = _stbData.Beams; break;
                case FrameType.Brace: frame = _stbData.Braces; break;
                case FrameType.Slab:
                case FrameType.Wall:
                case FrameType.Any:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            for (var i = 0; i < frame.Id.Count; i++)
            {
                int idNodeStart = nodes.Id.IndexOf(frame.IdNodeStart[i]);
                int idNodeEnd = nodes.Id.IndexOf(frame.IdNodeEnd[i]);
                _frameName.Add(frame.Name[i]);
                _framePos.Add(new Point3d(
                    (nodes.X[idNodeStart] + nodes.X[idNodeEnd]) / 2.0,
                    (nodes.Y[idNodeStart] + nodes.Y[idNodeEnd]) / 2.0,
                    (nodes.Z[idNodeStart] + nodes.Z[idNodeEnd]) / 2.0)
                );
            }

            DA.SetDataList(0, _frameName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _frameName.Count; i++)
            {
                args.Display.Draw2dText(_frameName[i], Color.Black, _framePos[i], true, _size);
            }
        }

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("758DE991-F652-4EDC-BC63-2A454BA43FB0");
    }
}
