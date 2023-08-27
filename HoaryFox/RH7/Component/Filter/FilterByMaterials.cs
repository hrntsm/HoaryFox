using System;
using System.Collections.Generic;
using System.Drawing;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HoaryFoxCommon.Properties;

namespace HoaryFox.Component.Filter
{
    public class FilterByMaterials : GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public FilterByMaterials()
          : base("Filter by materials", "FilterByMat",
              "Filter geometry by material",
              "HoaryFox", "Filter")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Geometry", "Geo", "Geometry", GH_ParamAccess.tree);
            pManager.AddTextParameter("Materials", "Mats", "Geometry material info", GH_ParamAccess.tree);
            pManager.AddIntegerParameter("Stories", "Sts", "Geometry story info", GH_ParamAccess.tree);
            pManager[2].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("RC", "RC", "output RC geometry", GH_ParamAccess.tree);
            pManager.AddBrepParameter("S", "S", "output S geometry", GH_ParamAccess.tree);
            pManager.AddBrepParameter("SRC", "SRC", "output SRC geometry", GH_ParamAccess.tree);
            pManager.AddBrepParameter("CFT", "CFT", "output CFT geometry", GH_ParamAccess.tree);
            pManager.AddBrepParameter("PC", "PC", "output PC geometry", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetDataTree(0, out GH_Structure<GH_Brep> breps)) { return; }
            if (!dataAccess.GetDataTree(1, out GH_Structure<GH_String> materials)) { return; }
            if (!dataAccess.GetDataTree(2, out GH_Structure<GH_Integer> stories)) { return; }

            if (breps.Paths.Count != materials.Paths.Count || (breps.Paths.Count != stories.Paths.Count && !stories.IsEmpty))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Geometries, materials and stories must have the same number of items");
                return;
            }

            var filteredBreps = new GH_Structure<GH_Brep>[5];
            for (var i = 0; i < 5; i++)
            {
                filteredBreps[i] = new GH_Structure<GH_Brep>();
            }

            FilterValue(breps, stories, materials, filteredBreps);

            for (var i = 0; i < 5; i++)
            {
                dataAccess.SetDataTree(i, filteredBreps[i]);
            }
        }

        private void FilterValue(GH_Structure<GH_Brep> breps, GH_Structure<GH_Integer> stories, GH_Structure<GH_String> materials, IReadOnlyList<GH_Structure<GH_Brep>> filteredBreps)
        {
            for (var i = 0; i < breps.PathCount; i++)
            {
                GH_Path path = stories.IsEmpty ? breps.Paths[i] : new GH_Path(stories.Branches[i][0].Value, i);
                GH_Brep brep = breps.Branches[i][0];
                var materialType = materials.Branches[i][0].ToString();

                SetMaterialFilteredBrep(filteredBreps, brep, materialType, path);
            }
        }

        private bool SetMaterialFilteredBrep(IReadOnlyList<GH_Structure<GH_Brep>> results, GH_Brep brep, string materialType, GH_Path path)
        {
            if (brep != null)
            {
                switch (materialType)
                {
                    case "RC":
                        results[0].Append(brep.DuplicateBrep(), path);
                        break;
                    case "S":
                        results[1].Append(brep.DuplicateBrep(), path);
                        break;
                    case "SRC":
                        results[2].Append(brep.DuplicateBrep(), path);
                        break;
                    case "CFT":
                        results[3].Append(brep.DuplicateBrep(), path);
                        break;
                    case "PC":
                        results[4].Append(brep.DuplicateBrep(), path);
                        break;
                    default:
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Unknown material type");
                        return true;
                }
            }

            return false;
        }

        protected override Bitmap Icon => Resource.FilterByMaterial;
        public override Guid ComponentGuid => new Guid("a4bcebdb-2c9c-41e4-8328-e270ea70ed7b");
    }
}
