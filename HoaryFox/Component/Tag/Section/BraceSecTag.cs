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
    public class BraceSecTag:GH_Component
    {
        private StbData _stbData;
        private int _size;
        
        private GH_Structure<GH_String> _frameTags = new GH_Structure<GH_String>();
        private List<Point3d> _tagPos = new List<Point3d>();
        
        public BraceSecTag()
          : base("Brace Section Tag", "BraceSec", "Display Beam Section Tag", "HoaryFox", "Section")
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
            pManager.AddTextParameter("Braces", "Br", "output StbBeams to Section Tag", GH_ParamAccess.tree);
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
            if (_frameTags.DataCount != 0)
            {
                for (int i = 0; i < _frameTags.PathCount; i++)
                {
                    var tags = _frameTags.Branches[i];
                    string tag = tags[0].ToString() + "\n" + tags[1].ToString() + "\n" + tags[2].ToString() + "\n" + 
                                 tags[3].ToString() + "\n" + tags[4].ToString() + "\n" + tags[5].ToString();
                    args.Display.Draw2dText(tag, Color.Black, _tagPos[i], false, _size);
                }
            }
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.BraceSection;

        public override Guid ComponentGuid => new Guid("DE9D56D2-C82C-4AB1-96FF-D1253B420A43");

        private void GetTag()
        {
            var tags = new CreateTag(_stbData.Nodes);
            _frameTags = tags.Frame(_stbData.Braces, _stbData.SecColumnRc, _stbData.SecColumnS, _stbData.SecBeamRc, _stbData.SecBeamS, _stbData.SecBraceS, _stbData.SecSteel);
            _tagPos = tags.TagPos;
        }
    }
}
