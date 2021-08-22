using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Geometry
{
    public class Axis : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;
        private double _factor;
        private readonly List<Line> _axisLines = new List<Line>();
        private readonly List<Point3d> _axisPts = new List<Point3d>();
        private readonly List<string> _axisStr = new List<string>();
        private readonly List<Point3d> _storyPts = new List<Point3d>();
        private readonly List<string> _storyStr = new List<string>();

        public override bool IsPreviewCapable => true;
        public Axis()
          : base("Axis", "Axis",
              "Description",
              "HoaryFox", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _axisLines.Clear();
            _axisPts.Clear();
            _axisStr.Clear();
            _storyPts.Clear();
            _storyStr.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddNumberParameter("Factor", "F", "Axis length factor", GH_ParamAccess.item, 1.2);
            pManager.AddIntegerParameter("Size", "S", "Axis tag", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "Ax", "output StbAxes to Line", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            if (!dataAccess.GetData(0, ref _stBridge)) { return; }
            if (!dataAccess.GetData(1, ref _factor)) { return; }
            if (!dataAccess.GetData(2, ref _size)) { return; }

            StbAxes axis = _stBridge.StbModel.StbAxes;
            StbParallelAxes[] parallels = axis.StbParallelAxes;
            StbStory[] stories = _stBridge.StbModel.StbStories;
            double length = GetMaxLength(_stBridge.StbModel.StbNodes);

            StbParallelAxesToLine(_factor, parallels, stories, length);

            dataAccess.SetDataList(0, _axisLines);
        }

        private void StbParallelAxesToLine(double factor, StbParallelAxes[] parallels, StbStory[] stories, double length)
        {
            bool isFirst = true;
            for (int i = 0; i < stories.Length; i++)
            {
                StbStory story = stories[i];
                var height = story.height;
                _storyStr.Add(story.name);
                _storyPts.Add(new Point3d(0, 0, height));
                foreach (StbParallelAxes parallel in parallels)
                {
                    var basePt = new Point3d(parallel.X, parallel.Y, height);
                    Vector3d axisVec = Vector3d.XAxis * length;
                    axisVec.Rotate(parallel.angle * Math.PI / 180, -Vector3d.ZAxis);
                    Vector3d distanceVec = Vector3d.YAxis;
                    distanceVec.Rotate(parallel.angle * Math.PI / 180, Vector3d.ZAxis);

                    foreach (StbParallelAxis pAxis in parallel.StbParallelAxis)
                    {
                        _axisLines.Add(new Line(
                            basePt - (axisVec * (factor - 1)) + (distanceVec * pAxis.distance),
                            basePt + (axisVec * factor) + (distanceVec * pAxis.distance)
                        ));
                        _axisPts.Add(basePt - (axisVec * (factor - 1)) + (distanceVec * pAxis.distance));
                        _axisStr.Add(isFirst == true ? pAxis.name : string.Empty);
                    }
                }

                isFirst = false;
            }
        }

        private static double GetMaxLength(StbNode[] stbNodes)
        {
            IEnumerable<double> xList = stbNodes.Select(n => n.X);
            IEnumerable<double> yList = stbNodes.Select(n => n.Y);

            return Math.Sqrt(Math.Pow(xList.Max() - xList.Min(), 2) + Math.Pow(yList.Max() - yList.Min(), 2));
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _axisLines.Count; i++)
            {
                args.Display.Draw2dText(_axisStr[i], Color.Black, _axisPts[i], true, _size);
                args.Display.DrawPatternedLine(_axisLines[i], Color.Black, 0x0000AFAF, 1);
            }

            double xMin = _axisPts.Min(pt => pt.X);
            double yMin = _axisPts.Min(pt => pt.Y);
            Vector3d vec = new Vector3d(xMin, yMin, 0);
            double length = _axisLines.Max(line => line.Length);

            for (var i = 1; i < _storyPts.Count; i++)
            {
                args.Display.DrawLine(new Line(_storyPts[i - 1] + vec, _storyPts[i] + vec), Color.Black);
            }

            for (int i = 0; i < _storyPts.Count; i++)
            {
                args.Display.Draw2dText(_storyStr[i], Color.Black, _storyPts[i] + vec, true, _size);
                args.Display.DrawLine(new Line(_storyPts[i] + vec, _storyPts[i] + vec + length * Vector3d.XAxis), Color.Black);
                args.Display.DrawLine(new Line(_storyPts[i] + vec, _storyPts[i] + vec + length * Vector3d.YAxis), Color.Black);
            }
        }

        protected override Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("98315013-7bb3-4ad9-8b69-ad1457ebe0b7");
    }
}
