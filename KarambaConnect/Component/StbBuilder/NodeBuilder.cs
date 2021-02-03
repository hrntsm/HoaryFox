using System;
using System.Drawing;
using Grasshopper.Kernel;
using Karamba.GHopper.Models;
using Model = Karamba.Models.Model;

namespace KarambaConnect.Component.StbBuilder
{
    public class NodeBuilder:GH_Component
    {
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public NodeBuilder()
          : base("NodeBuilder", "NodeBuilder", "Convert Karamba3D model to StbNode data.", "HoaryFox", "StbBuilder")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddParameter(new Param_Model(), "Model", "Model", "Karamba model data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Node", "Node", "StbNode Data", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var modelIn = new object();
            if (!DA.GetData(0, ref modelIn)) { return; }

            if (!(modelIn is GH_Model ghKModel))
            {
                throw new ArgumentException("The input is not Karamba3D model!");
            }
            Model kModel = ghKModel.Value;
            
            DA.SetDataList(0, kModel.nodes.ToStb());
        }

        protected override Bitmap Icon => Properties.Resource.NodeBuilder;
        public override Guid ComponentGuid => new Guid("D3FCFB17-E6C4-47D4-852A-24D92EC1EFEE");
    }
}
