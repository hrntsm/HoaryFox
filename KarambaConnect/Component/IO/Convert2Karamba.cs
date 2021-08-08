using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Karamba.CrossSections;
using Karamba.Elements;
using Karamba.GHopper.CrossSections;
using Karamba.GHopper.Elements;
using KarambaConnect.Properties;
using KarambaConnect.S2K;
using STBDotNet.v202;

namespace KarambaConnect.Component.IO
{
    public class Convert2Karamba : GH_Component
    {

        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public Convert2Karamba()
          : base("Convert to Karamba", "S2K", "Convert ST-Bridge file to Karamba.", "HoaryFox", "IO")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "Input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddGenericParameter("FamilyName", "Family", "Each CrossSection Family Name", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element(), "ElementBeam", "ElemBe", "Karamba Line Element", GH_ParamAccess.list);
            pManager.AddParameter(new Param_CrossSection(), "CrossSection", "CroSec", "Karamba CrossSection", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var familyName = new CroSecFamilyName();
            ST_BRIDGE stBridge = new ST_BRIDGE();
            List<GH_Element> k3ElemBe = new List<GH_Element>();
            List<GH_Element> k3ElemSh = new List<GH_Element>();

            if (!dataAccess.GetData(0, ref stBridge)) { return; }
            if (!dataAccess.GetData(1, ref familyName))
            {
                familyName = CroSecFamilyName.Default();
            }


            List<string>[] k3Ids = CrossSection.GetIndex(stBridge);
            List<CroSec> k3CroSec = CrossSection.GetCroSec(stBridge.StbModel.StbSections, familyName);
            List<BuilderBeam> elems = ElementBuilder.BuilderBeams(stBridge.StbModel, k3Ids);
            List<GH_Element> ghElements = elems.Select(e => new GH_Element(e)).ToList();
            k3ElemBe = ghElements;

            dataAccess.SetDataList(0, k3ElemBe);
            dataAccess.SetDataList(1, k3CroSec);
        }

        protected override Bitmap Icon => Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("C57461DA-E79B-49A0-B44B-71CF32057709");
    }
}
