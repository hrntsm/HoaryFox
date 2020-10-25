using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using STBReader;
using Rhino.Geometry;
using STBReader.Member;
using STBReader.Model;

namespace HoaryFox.Component.Tag.Name
{
    public class PostNameTag:GH_Component
    {
        private StbData _stbData;
        private int _size;
        
        private readonly List<string> _postName = new List<string>();
        private readonly List<Point3d> _postPos = new List<Point3d>();

        public PostNameTag()
          : base(name: "Post Name Tag", nickname: "PostTag", description: "Display Post Name Tag", category: "HoaryFox", subCategory: "Name")
        {
        }
        
        public override bool IsPreviewCapable => true;

        public override void ClearData()
        {
            base.ClearData();
            _postName.Clear();
            _postPos.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Posts", "Pst", "output StbPosts name tag", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            StbNodes nodes = _stbData.Nodes;
            StbPosts posts = _stbData.Posts;
            
            for (var i = 0; i < posts.Id.Count; i++)
            {
                int idNodeStart = nodes.Id.IndexOf(posts.IdNodeStart[i]);
                int idNodeEnd = nodes.Id.IndexOf(posts.IdNodeEnd[i]);
                _postName.Add(posts.Name[i]);
                _postPos.Add(new Point3d(
                    (nodes.X[idNodeStart] + nodes.X[idNodeEnd]) / 2.0,
                    (nodes.Y[idNodeStart] + nodes.Y[idNodeEnd]) / 2.0,
                    (nodes.Z[idNodeStart] + nodes.Z[idNodeEnd]) / 2.0)
                );
            }

            DA.SetDataList(0, _postName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _postName.Count; i++)
            {
                args.Display.Draw2dText(_postName[i], Color.Black, _postPos[i], true, _size);
            }
        }

        protected override Bitmap Icon => Properties.Resource.PostName;
        public override Guid ComponentGuid => new Guid("8FAC9887-B49F-4FC1-8B6B-7847FCE49339");
    }
}