using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using HoaryFox.KarambaConnect;
using STBReader;
using Karamba.CrossSections;
using Karamba.Elements;
using Karamba.Geometry;
using Karamba.GHopper.CrossSections;
using Karamba.GHopper.Elements;
using Karamba.Utilities;
using STBReader.Member;
using STBReader.Model;

namespace HoaryFox.Component.IO
{
    public class Stb2KElem:GH_Component
    {
        private StbData _stbData;
        private List<GH_Element> _k3ElemBe = new List<GH_Element>();
        private List<GH_Element> _k3ElemSh = new List<GH_Element>();

        public Stb2KElem()
          : base("Stb to Karamba", "S2K", "Read ST-Bridge file and display", "HoaryFox", "IO")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _k3ElemBe.Clear();
            _k3ElemSh.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element(), "ElementBeam", "ElemBe", "Karamba Line Element", GH_ParamAccess.list);
            pManager.AddParameter(new Param_CrossSection(), "CrossSection", "CroSec", "Karamba CrossSection", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            
            List<string>[] k3Ids = CrossSection.GetIndex(_stbData);
            List<CroSec> k3CroSec = CrossSection.GetCroSec(_stbData);
            List<BuilderBeam> elems = Element.BuilderBeams(_stbData, k3Ids);
            List<GH_Element> ghElements = elems.Select(e => new GH_Element(e)).ToList();
            _k3ElemBe = ghElements;

            DA.SetDataList(0, _k3ElemBe);
            DA.SetDataList(1, k3CroSec);
        }

        protected override Bitmap Icon => Properties.Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("C57461DA-E79B-49A0-B44B-71CF32057709");
    }
}
