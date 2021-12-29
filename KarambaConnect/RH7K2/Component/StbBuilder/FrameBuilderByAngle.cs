using System;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Karamba.GHopper.Models;
using KarambaConnectCommon.Properties;
using Rhino.Geometry;
using STBDotNet.v202;
using Model = Karamba.Models.Model;

namespace KarambaConnect.Component.StbBuilder
{
    public class FrameBuilderByAngle : GH_Component
    {
        private StbModel _sModel;

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
            pManager.AddGenericParameter("Section", "Sec", "StbSection data", GH_ParamAccess.item);
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
            var k2S = new K2S.K2StbModel(kModel);
            _sModel = k2S.SetByAngle(colMaxAngle);

            dataAccess.SetData(0, _sModel.StbMembers);
            dataAccess.SetData(1, _sModel.StbSections);
        }

        protected override Bitmap Icon => Resource.FrameBuilder;
        public override Guid ComponentGuid => new Guid("38296D06-E47A-403F-BFE8-00E873A99CF8");

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            if (_sModel == null || _sModel.StbNodes.Length < 1)
            {
                return;
            }

            foreach (StbColumn column in _sModel.StbMembers.StbColumns)
            {
                Point3d ptFrom = _sModel.StbNodes.First(node => node.id == column.id_node_bottom).ToRhino();
                Point3d ptTo = _sModel.StbNodes.First(node => node.id == column.id_node_top).ToRhino();
                args.Display.Draw2dText("Column", Color.Brown, (ptFrom + ptTo) / 2, true, 12);
            }

            foreach (StbGirder girder in _sModel.StbMembers.StbGirders)
            {
                Point3d ptFrom = _sModel.StbNodes.First(node => node.id == girder.id_node_start).ToRhino();
                Point3d ptTo = _sModel.StbNodes.First(node => node.id == girder.id_node_end).ToRhino();
                args.Display.Draw2dText("Girder", Color.DarkGreen, (ptFrom + ptTo) / 2, true, 12);
            }

            foreach (StbBrace brace in _sModel.StbMembers.StbBraces)
            {
                Point3d ptFrom = _sModel.StbNodes.First(node => node.id == brace.id_node_start).ToRhino();
                Point3d ptTo = _sModel.StbNodes.First(node => node.id == brace.id_node_end).ToRhino();
                args.Display.Draw2dText("Brace", Color.Purple, (ptFrom + ptTo) / 2, true, 12);
            }
        }
    }
}
