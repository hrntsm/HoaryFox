using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;

using Karamba.CrossSections;
using Karamba.Elements;
using Karamba.GHopper.CrossSections;
using Karamba.GHopper.Elements;

using KarambaConnect.S2K;

using KarambaConnectCommon.Properties;

using STBDotNet.v202;

namespace KarambaConnect.Component.IO
{
    public class Convert2Karamba : GH_Component
    {
        private ST_BRIDGE _stBridge;
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
            var k3dElemSh = new List<GH_Element>();

            if (!dataAccess.GetData(0, ref _stBridge)) { return; }
            if (!dataAccess.GetData(1, ref familyName))
            {
                familyName = CroSecFamilyName.Default();
            }


            List<string>[] k3dIds = CrossSection.GetIndex(_stBridge);
            List<CroSec> k3dCroSec = CrossSection.GetCroSec(_stBridge.StbModel.StbSections, familyName);
            List<BuilderBeam> k3dBeamElems = ElementBuilder.BuilderBeams(_stBridge.StbModel, k3dIds);
            List<GH_Element> ghK3dElements = k3dBeamElems.Select(e => new GH_Element(e)).ToList();

            dataAccess.SetDataList(0, ghK3dElements);
            dataAccess.SetDataList(1, k3dCroSec);
        }

        protected override Bitmap Icon => Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("C57461DA-E79B-49A0-B44B-71CF32057709");
    }
}
