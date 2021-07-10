using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using HoaryFox.Component_v2.Utils.Geometry;
using HoaryFox.Properties;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Geometry
{
    public class Stb2Brep : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private readonly List<List<Brep>> _brepList = new List<List<Brep>>();

        public Stb2Brep()
          : base("Stb to Brep", "S2B",
              "Display ST-Bridge model in Brep",
              "HoaryFox2", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _brepList.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bake", "Bake", "If it true, bake geometry.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Columns", "Col", "output StbColumns to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Girders", "Gird", "output StbGirders to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Posts", "Pst", "output StbPosts to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Beams", "Bm", "output StbBeams to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Braces", "Brc", "output StbBraces to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Slabs", "Slb", "output StbSlabs to Brep", GH_ParamAccess.list);
            pManager.AddBrepParameter("Walls", "Wl", "output StbWalls to Brep", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var isBake = false;
            if (!DA.GetData("Data", ref _stBridge)) { return; }
            if (!DA.GetData("Bake", ref isBake)) { return; }

            MakeBrep();

            for (var i = 0; i < 7; i++)
            {
                DA.SetDataList(i, _brepList[i]);
            }
        }
        protected override Bitmap Icon => Resource.Brep;
        public override Guid ComponentGuid => new Guid("B2D5EA7F-E75F-406B-8D22-C267B43C5E72");

        private void MakeBrep()
        {
            var aaa = new CreateBrepFromStb(_stBridge.StbModel.StbNodes, new[] { DocumentTolerance(), DocumentAngleTolerance() });
            _brepList.Add(aaa.Column(_stBridge.StbModel.StbMembers.StbColumns, _stBridge.StbModel.StbSections));

            for (var i = 0; i < 6; i++)
            {
                _brepList.Add(new List<Brep>());
            }
        }
    }
}
