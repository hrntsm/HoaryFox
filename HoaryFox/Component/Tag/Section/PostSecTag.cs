using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFox.Member;
using STBReader;
using Rhino.Geometry;

namespace HoaryFox.Component.Tag.Section
{
    public class PostSecTag:GH_Component
    {
        private StbData _stbData;
        private int _size;

        private GH_Structure<GH_String> _frameTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();
        
        public PostSecTag()
          : base("Post Section Tag", "PostSec", "Display Post Section Tag", "HoaryFox", "Section")
        {
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
            pManager.AddTextParameter("Posts", "Pst", "output StbPosts to Brep", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }
            
            GetTag();

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

        protected override Bitmap Icon => Properties.Resource.PostSection;
        public override Guid ComponentGuid => new Guid("C5891374-37F7-43E8-9D28-A901D87B497E");

        private void GetTag()
        {
            var tags = new CreateTag(_stbData.Nodes);
            _frameTags = tags.Frame(_stbData.Posts, _stbData.SecColumnRc, _stbData.SecColumnS, _stbData.SecBeamRc, _stbData.SecBeamS, _stbData.SecBraceS, _stbData.SecSteel);
            _tagPos = tags.TagPos;
        }
    }
}
