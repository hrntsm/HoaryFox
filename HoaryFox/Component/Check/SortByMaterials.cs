using System;
using System.Drawing;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace HoaryFox.Component.Check
{
    public class SortByMaterials : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public SortByMaterials()
          : base("Sort by materials", "SortByMat",
              "Sort geometry by material",
              "HoaryFox", "Check")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Geometry", "Geo", "Geometry", GH_ParamAccess.tree);
            pManager.AddTextParameter("Materials", "Mats", "Geometry material info", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("RC", "RC", "output RC geometry", GH_ParamAccess.tree);
            pManager.AddBrepParameter("S", "S", "output S geometry", GH_ParamAccess.tree);
            pManager.AddBrepParameter("SRC", "SRC", "output SRC geometry", GH_ParamAccess.tree);
            pManager.AddBrepParameter("CFT", "CFT", "output CFT geometry", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetDataTree(0, out GH_Structure<GH_Brep> breps)) { return; }
            if (!dataAccess.GetDataTree(1, out GH_Structure<GH_String> mats)) { return; }

            if (breps.Paths.Count != mats.Paths.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Geometry and materials must have the same number of items");
                return;
            }

            var results = new GH_Structure<GH_Brep>[4];
            for (var i = 0; i < 4; i++)
            {
                results[i] = new GH_Structure<GH_Brep>();
            }

            for (var i = 0; i < breps.PathCount; i++)
            {
                switch (mats.Branches[i][0].ToString())
                {
                    case "RC":
                        results[0].Append(breps.Branches[i][0].DuplicateBrep(), breps.Paths[i]);
                        break;
                    case "S":
                        results[1].Append(breps.Branches[i][0].DuplicateBrep(), breps.Paths[i]);
                        break;
                    case "SRC":
                        results[2].Append(breps.Branches[i][0].DuplicateBrep(), breps.Paths[i]);
                        break;
                    case "CFT":
                        results[3].Append(breps.Branches[i][0].DuplicateBrep(), breps.Paths[i]);
                        break;
                    default:
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unknown material type");
                        return;
                }
            }

            for (var i = 0; i < 4; i++)
            {
                dataAccess.SetDataTree(i, results[i]);
            }
        }

        protected override Bitmap Icon => null;
        public override Guid ComponentGuid => new Guid("a4bcebdb-2c9c-41e4-8328-e270ea70ed7b");
    }
}
