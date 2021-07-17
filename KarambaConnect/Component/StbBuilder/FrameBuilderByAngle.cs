using System;
using System.Drawing;
using Grasshopper.Kernel;
using Karamba.GHopper.Models;
using KarambaConnect.K2S;
using Rhino.Geometry;
using STBDotNet.Elements.StbModel.StbMember;
using Model = Karamba.Models.Model;

namespace KarambaConnect.Component.StbBuilder
{
    public class FrameBuilderByAngle : GH_Component
    {
        private STBDotNet.Elements.StbModel.Model _sModel;

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public FrameBuilderByAngle()
          : base("FrameBuilder by angle", "Frame by angle", "Convert Karamba model to StbModel & StbSection by elements angle.", "HoaryFox", "StbBuilder")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Model(), "Model", "Model", "Karamba model data", GH_ParamAccess.item);
            pManager.AddNumberParameter("Angle", "Angle", "Maximum angle of the column to the Z-axis", GH_ParamAccess.item, Math.PI / 4d);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Member", "Mem", "StbMember data", GH_ParamAccess.item);
            pManager.AddGenericParameter("Section", "Sec", "StbSection data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            double colMaxAngle = 0;
            var modelIn = new object();
            if (!dataAccess.GetData(0, ref modelIn)) { return; }
            if (!dataAccess.GetData(1, ref colMaxAngle)) { return; }

            if (!(modelIn is GH_Model ghKModel))
            {
                throw new ArgumentException("The input is not Karamba3D model!");
            }
            Model kModel = ghKModel.Value;
            var stbModel = new StbModel(kModel);
            _sModel = stbModel.SetByAngle(colMaxAngle);
            _sModel.Nodes = kModel.nodes.ToStb();

            dataAccess.SetData(0, _sModel.Members);
            dataAccess.SetDataList(1, _sModel.Sections);
        }

        protected override Bitmap Icon => Properties.Resource.FrameBuilder;
        public override Guid ComponentGuid => new Guid("38296D06-E47A-403F-BFE8-00E873A99CF8");

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_sModel == null || _sModel.Nodes.Count < 1)
            {
                return;
            }

            foreach (Column column in _sModel.Members.Columns)
            {
                Point3d ptFrom = _sModel.Nodes[column.IdNodeStart - 1].ToRhino();
                Point3d ptTo = _sModel.Nodes[column.IdNodeEnd - 1].ToRhino();
                args.Display.Draw2dText("Column", Color.Brown, (ptFrom + ptTo) / 2, true, 12);
            }

            foreach (Girder girder in _sModel.Members.Girders)
            {
                Point3d ptFrom = _sModel.Nodes[girder.IdNodeStart - 1].ToRhino();
                Point3d ptTo = _sModel.Nodes[girder.IdNodeEnd - 1].ToRhino();
                args.Display.Draw2dText("Girder", Color.DarkGreen, (ptFrom + ptTo) / 2, true, 12);
            }

            foreach (Brace brace in _sModel.Members.Braces)
            {
                Point3d ptFrom = _sModel.Nodes[brace.IdNodeStart - 1].ToRhino();
                Point3d ptTo = _sModel.Nodes[brace.IdNodeEnd - 1].ToRhino();
                args.Display.Draw2dText("Brace", Color.Purple, (ptFrom + ptTo) / 2, true, 12);
            }
        }
    }
}
