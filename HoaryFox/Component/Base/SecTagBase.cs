using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.Member;
using Rhino.Geometry;
using STBReader;
using STBReader.Member;

namespace HoaryFox.Component.Base
{
    public class SecTagBase:GH_Component
    {
        private StbData _stbData;
        private int _size;
        private readonly FrameType _frameType;

        private GH_Structure<GH_String> _frameTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();

        protected SecTagBase(string name, string nickname, string description, FrameType frameType)
            :base(name, nickname, description, category: "HoaryFox", subCategory: "Section")
        {
            _frameType = frameType;
        }

        public override void ClearData()
        {
            base.ClearData();
            _frameTags.Clear();
            _tagPos.Clear();
        }
        
        public override bool IsPreviewCapable => true;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("SecTag", "STag", "output section tag", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }
            
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
            GetTag(frame);

            DA.SetDataTree(0, _frameTags);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_frameTags.DataCount == 0)
            {
                return;
            }

            for (var i = 0; i < _frameTags.PathCount; i++)
            {
                List<GH_String> tags = _frameTags.Branches[i];
                string tag = tags[0].ToString() + "\n" + tags[1].ToString() + "\n" + tags[2].ToString() + "\n" + 
                             tags[3].ToString() + "\n" + tags[4].ToString() + "\n" + tags[5].ToString();
                args.Display.Draw2dText(tag, Color.Black, _tagPos[i], false, _size);
            }
        }

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("6300E95D-38AF-47A6-B792-E4680FE37F49");

        private void GetTag(StbFrame stbFrame)
        {
            var tags = new CreateTag(_stbData.Nodes, _stbData.SecColumnRc, _stbData.SecColumnS, _stbData.SecBeamRc, _stbData.SecBeamS, _stbData.SecBraceS, _stbData.SecSteel);
            _frameTags = tags.Frame(stbFrame);
            _tagPos = tags.Position;
        }
    }
}
