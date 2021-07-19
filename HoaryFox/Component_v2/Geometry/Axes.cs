using System;
using System.Collections.Generic;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component_v2.Geometry
{
    public class Axis : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private readonly List<Line> _axisLines = new List<Line>();

        public Axis()
          : base("Axis", "Axis",
              "Description",
              "HoaryFox2", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _axisLines.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddNumberParameter("Factor", "F", "Axis length factor", GH_ParamAccess.item, 1.2);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "Ax", "output StbAxes to Line", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            double factor = 1;
            if (!dataAccess.GetData(0, ref _stBridge)) { return; }
            if (!dataAccess.GetData(1, ref factor)) { return; }

            StbAxes axis = _stBridge.StbModel.StbAxes;
            StbParallelAxes[] parallels = axis.StbParallelAxes;
            double length = GetMaxLength(_stBridge.StbModel.StbNodes);

            foreach (StbParallelAxes parallel in parallels)
            {
                var basePt = new Point3d(parallel.X, parallel.Y, 0);
                Vector3d axisVec = Vector3d.XAxis * length;
                axisVec.Rotate(parallel.angle * Math.PI / 180, -Vector3d.ZAxis);
                Vector3d distanceVec = Vector3d.YAxis;
                distanceVec.Rotate(parallel.angle * Math.PI / 180, Vector3d.ZAxis);

                foreach (StbParallelAxis pAxis in parallel.StbParallelAxis)
                {
                    _axisLines.Add(new Line(
                        basePt - axisVec * (factor - 1) + distanceVec * pAxis.distance,
                        basePt + axisVec * factor + distanceVec * pAxis.distance
                    ));
                }
            }

            dataAccess.SetDataList(0, _axisLines);
        }

        private static double GetMaxLength(StbNode[] stbNodes)
        {
            IEnumerable<double> xList = stbNodes.Select(n => n.X);
            IEnumerable<double> yList = stbNodes.Select(n => n.Y);

            return Math.Sqrt(Math.Pow(xList.Max() - xList.Min(), 2) + Math.Pow(yList.Max() - yList.Min(), 2));
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("98315013-7bb3-4ad9-8b69-ad1457ebe0b7");
    }
}
