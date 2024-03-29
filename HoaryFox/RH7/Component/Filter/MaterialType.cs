﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HoaryFoxCommon.Properties;

using STBDotNet.v202;

namespace HoaryFox.Component.Filter
{
    public class MaterialType : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private readonly GH_Structure<GH_String>[] _materialTypeList = new GH_Structure<GH_String>[9];

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public MaterialType()
          : base("Material Type", "MatType",
              "Get material type",
              "HoaryFox", "Filter")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Columns", "Col", "output StbColumn material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Girders", "Gird", "output StbGirder material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Posts", "Pst", "output StbPost material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Beams", "Bm", "output StbBeam material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Braces", "Brc", "output StbBrace material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Slabs", "Slb", "output StbSlab material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Walls", "Wl", "output StbWall material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Piles", "Pil", "output StbPile material types", GH_ParamAccess.tree);
            pManager.AddTextParameter("Footings", "Ftg", "output StbFooting material types", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }

            Dictionary<string, string>[][] infoArray = Utils.TagUtils.GetAllSectionInfoArray(_stBridge.StbModel.StbMembers, _stBridge.StbModel.StbSections);

            foreach ((Dictionary<string, string>[] infoDict, int index) in infoArray.Select((dict, index) => (dict, index)))
            {
                var materialTypes = new GH_Structure<GH_String>();
                foreach ((Dictionary<string, string> info, int itemIndex) in infoDict.Select((dict, itemIndex) => (dict, itemIndex)))
                {
                    var materialType = info["stb_element_type"] == "StbFooting" ? "RC" : info["kind_structure"];
                    materialTypes.Append(new GH_String(materialType), new GH_Path(0, itemIndex));
                }
                _materialTypeList[index] = materialTypes;
            }

            for (var i = 0; i < 9; i++)
            {
                dataAccess.SetDataTree(i, _materialTypeList[i]);
            }
        }

        protected override Bitmap Icon => Resource.MaterialType;
        public override Guid ComponentGuid => new Guid("b6fde031-3532-40bb-a276-f61c36cdee90");
    }
}
