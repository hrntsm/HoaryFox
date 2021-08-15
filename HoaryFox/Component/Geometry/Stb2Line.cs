using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using HoaryFox.Component.Utils.Geometry;
using HoaryFox.Properties;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using STBDotNet.v202;

namespace HoaryFox.Component.Geometry
{
    public class Stb2Line : GH_Component
    {
        private ST_BRIDGE _stBridge;
        private List<Point3d> _nodes = new List<Point3d>();
        private readonly List<List<Line>> _lineList = new List<List<Line>>();

        public Stb2Line()
          : base("Stb to Line", "S2L",
              "Display ST-Bridge model in line",
              "HoaryFox", "Geometry")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _lineList.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Bake", "Bake", "If it true, bake geometry.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Nodes", "pt", "output StbNodes to point3d", GH_ParamAccess.list);
            pManager.AddLineParameter("Columns", "Col", "output StbColumns to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Girders", "Gird", "output StbGirders to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Posts", "Pst", "output StbPosts to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Beams", "Beam", "output StbBeams to Line", GH_ParamAccess.list);
            pManager.AddLineParameter("Braces", "Brc", "output StbBraces to Line", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var isBake = false;
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }
            if (!dataAccess.GetData("Bake", ref isBake)) { return; }

            CreateLine();
            if (isBake)
            {
                BakeLine();
            }

            dataAccess.SetDataList(0, _nodes);
            foreach ((List<Line> geometry, int i) in _lineList.Select((geo, index) => (geo, index + 1)))
            {
                dataAccess.SetDataList(i, geometry);
            }
        }

        private void BakeLine()
        {
            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            var parentLayerNames = new[] { "Column", "Girder", "Post", "Beam", "Brace", "Slab", "Wall" };
            Color[] layerColors = { Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple, Color.DarkGray, Color.CornflowerBlue };
            GeometryBaker.MakeParentLayers(activeDoc, parentLayerNames, layerColors);

            Dictionary<string, string>[][] infoArray = Utils.TagUtils.GetAllSectionInfoArray(_stBridge.StbModel.StbMembers, _stBridge.StbModel.StbSections);

            foreach ((List<Line> lines, int index) in _lineList.Select((frameBrep, index) => (frameBrep, index)))
            {
                Layer parentLayer = activeDoc.Layers.FindName(parentLayerNames[index]);
                int parentIndex = parentLayer.Index;
                Guid parentId = parentLayer.Id;
                foreach ((Line line, int bIndex) in lines.Select((brep, bIndex) => (brep, bIndex)))
                {
                    var objAttr = new ObjectAttributes();

                    if (index < 5)
                    {
                        Dictionary<string, string>[] infos = infoArray[index];
                        Dictionary<string, string> info = infos[bIndex];

                        foreach (KeyValuePair<string, string> pair in info)
                        {
                            objAttr.SetUserString(pair.Key, pair.Value);
                        }

                        var layer = new Layer { Name = info["name"], ParentLayerId = parentId, Color = layerColors[index] };
                        int layerIndex = activeDoc.Layers.Add(layer);
                        if (layerIndex == -1)
                        {
                            layer = activeDoc.Layers.FindName(info["name"]);
                            layerIndex = layer.Index;
                        }
                        objAttr.LayerIndex = layerIndex;
                    }
                    else
                    {
                        objAttr.LayerIndex = parentIndex;
                    }

                    activeDoc.Objects.AddLine(line, objAttr);
                }
            }
        }

        private void CreateLine()
        {
            var createLines = new CreateLineFromStb(_stBridge);
            _nodes = createLines.Nodes();
            _lineList.Add(createLines.Columns());
            _lineList.Add(createLines.Girders());
            _lineList.Add(createLines.Posts());
            _lineList.Add(createLines.Beams());
            _lineList.Add(createLines.Braces());
        }

        protected override Bitmap Icon => Resource.Line;
        public override Guid ComponentGuid => new Guid("D1E6793B-F75C-4AEE-9A9F-B9DD08D6EB77");
    }
}
