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
using STBReader;

namespace KarambaConnect.Component.IO
{
    public class SetFamilyName:GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public SetFamilyName()
          : base("SetCroSecFamilyName", "CSFamName", "Set cross section family name", "HoaryFox", "IO")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Box", "Box", "Box shape cross section family name", GH_ParamAccess.item,"HF-Box");
            pManager.AddTextParameter("H", "H", "Box shape cross section family name", GH_ParamAccess.item, "HF-H");
            pManager.AddTextParameter("Circle", "Circle", "Circle shape cross section family name", GH_ParamAccess.item, "HF-Circle");
            pManager.AddTextParameter("Pipe", "Pipe", "Pipe shape cross section family name", GH_ParamAccess.item, "HF-Pipe");
            pManager.AddTextParameter("FB", "FB", "FB shape cross section family name", GH_ParamAccess.item, "HF-FB");
            pManager.AddTextParameter("L", "L", "L shape cross section family name", GH_ParamAccess.item, "HF-L");
            pManager.AddTextParameter("T", "T", "T shape cross section family name", GH_ParamAccess.item, "HF-T");
            pManager.AddTextParameter("Other", "Other", "Other cross section family name", GH_ParamAccess.item, "HF-Other");
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("FamilyName", "Family", "Each CrossSection Family Name", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var name = new string[8];
            if (!DA.GetData(0, ref name[0])) { return; }
            if (!DA.GetData(1, ref name[1])) { return; }
            if (!DA.GetData(2, ref name[2])) { return; }
            if (!DA.GetData(3, ref name[3])) { return; }
            if (!DA.GetData(4, ref name[4])) { return; }
            if (!DA.GetData(5, ref name[5])) { return; }
            if (!DA.GetData(6, ref name[6])) { return; }
            if (!DA.GetData(7, ref name[7])) { return; }

            var familyName = new CroSecFamilyName
            {
                Box = name[0], H = name[1], Circle = name[2], Pipe = name[3], FB = name[4], L = name[5], T = name[6], Other = name[7]
            };

            DA.SetData(0, familyName);
        }

        // protected override Bitmap Icon => Resource.ToKaramba;
        public override Guid ComponentGuid => new Guid("6479593D-DC0A-4362-BE28-515E6AC0E342");
    }
}
