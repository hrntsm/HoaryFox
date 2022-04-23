using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using HoaryFoxCommon.Properties;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Geometry
{
    public class Axis : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private int _size;
        private double _factor;
        private readonly GH_Structure<GH_Line> _axisLines = new GH_Structure<GH_Line>();
        private readonly GH_Structure<GH_Point> _axisPts = new GH_Structure<GH_Point>();
        private readonly GH_Structure<GH_String> _axisName = new GH_Structure<GH_String>();
        private readonly GH_Structure<GH_Point> _storyPts = new GH_Structure<GH_Point>();
        private readonly GH_Structure<GH_String> _storyName = new GH_Structure<GH_String>();

        public override bool IsPreviewCapable => true;
        public override GH_Exposure Exposure => GH_Exposure.secondary;

        public Axis()
          : base("Axis", "Axis",
              "Create Axis and Story lines",
              "HoaryFox", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _axisLines.Clear();
            _axisPts.Clear();
            _axisName.Clear();
            _storyPts.Clear();
            _storyName.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddNumberParameter("Factor", "F", "Axis length factor", GH_ParamAccess.item, 1.2);
            pManager.AddIntegerParameter("Size", "S", "Axis tag", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddLineParameter("Axis", "Ax", "output StbAxes to Line", GH_ParamAccess.tree);
            pManager.AddTextParameter("StoryName", "StName", "output StbAxes name", GH_ParamAccess.tree);
            pManager.AddTextParameter("AxisName", "AxName", "output StbStroy name", GH_ParamAccess.tree);
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

            dataAccess.SetDataTree(0, _axisLines);
            dataAccess.SetDataTree(1, _storyName);
            dataAccess.SetDataTree(2, _axisName);
        }

        private void StbParallelAxesToLine(double factor, StbParallelAxes[] parallels, IEnumerable<StbStory> stories, double length)
        {
            foreach ((StbStory story, int i) in stories.Select((s, i) => (s, i)))
            {
                var path = new GH_Path(0, i);
                double height = story.height;
                _storyName.Append(new GH_String(story.name), path);
                _storyPts.Append(new GH_Point(new Point3d(0, 0, height)), path);
                CreateEachAxis(factor, parallels, length, height, path);
            }
        }

        private void CreateEachAxis(double factor, IEnumerable<StbParallelAxes> parallels, double length, double height, GH_Path path)
        {
            foreach (StbParallelAxes parallel in parallels)
            {
                var basePt = new Point3d(parallel.X, parallel.Y, height);
                Vector3d axisVec = Vector3d.XAxis * length;
                axisVec.Rotate(parallel.angle * Math.PI / 180, -Vector3d.ZAxis);
                Vector3d distanceVec = Vector3d.YAxis;
                distanceVec.Rotate(parallel.angle * Math.PI / 180, Vector3d.ZAxis);

                foreach (StbParallelAxis pAxis in parallel.StbParallelAxis)
                {
                    _axisLines.Append(new GH_Line(new Line(
                        basePt - (axisVec * (factor - 1)) + (distanceVec * pAxis.distance),
                        basePt + (axisVec * factor) + (distanceVec * pAxis.distance)
                    )), path);
                    _axisPts.Append(new GH_Point(basePt - (axisVec * (factor - 1)) + (distanceVec * pAxis.distance)), path);
                    _axisName.Append(new GH_String(_storyName.get_Branch(path)[0] + " " + pAxis.name), path);
                }
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
            Line[] lines = _axisLines.FlattenData().Select(ln => ln.Value).ToArray();
            string[] names = _axisName.FlattenData().Select(nm => nm.Value).ToArray();
            Point3d[] pts = _axisPts.FlattenData().Select(pt => pt.Value).ToArray();

            for (var i = 0; i < lines.Length; i++)
            {
                args.Display.Draw2dText(names[i], Color.Black, pts[i], true, _size);
                args.Display.DrawPatternedLine(lines[i], Color.Black, 0x0000AFAF, 1);
            }
        }

        protected override Bitmap Icon => Resource.Axis;

        public override Guid ComponentGuid => new Guid("98315013-7bb3-4ad9-8b69-ad1457ebe0b7");
    }
}
