using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using HoaryFox.Component.Utils.Geometry;

using HoaryFoxCommon.Properties;

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
        private readonly GH_Structure<GH_Line>[] _lineList = new GH_Structure<GH_Line>[8];

        public override GH_Exposure Exposure => GH_Exposure.primary;

        public Stb2Line()
          : base("Stb to Line", "S2L",
              "Display ST-Bridge model in line",
              "HoaryFox", "Geometry")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
            pManager.AddBooleanParameter("OffsetNode", "OfNd", "If it true, offset geometry.", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Bake", "Bake", "If it true, bake geometry.", GH_ParamAccess.item, false);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Nodes", "pt", "output StbNodes to point3d", GH_ParamAccess.list);
            pManager.AddLineParameter("Columns", "Col", "output StbColumns to Line", GH_ParamAccess.tree);
            pManager.AddLineParameter("Girders", "Gird", "output StbGirders to Line", GH_ParamAccess.tree);
            pManager.AddLineParameter("Posts", "Pst", "output StbPosts to Line", GH_ParamAccess.tree);
            pManager.AddLineParameter("Beams", "Beam", "output StbBeams to Line", GH_ParamAccess.tree);
            pManager.AddLineParameter("Braces", "Brc", "output StbBraces to Line", GH_ParamAccess.tree);
            pManager.AddLineParameter("Piles", "Pile", "output StbPiles to Line", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess dataAccess)
        {
            var isBake = false;
            var isOffset = false;
            if (!dataAccess.GetData("Data", ref _stBridge)) { return; }
            if (!dataAccess.GetData("OffsetNode", ref isOffset)) { return; }
            if (!dataAccess.GetData("Bake", ref isBake)) { return; }

            CreateLine(isOffset);
            if (isBake)
            {
                BakeLine();
            }

            dataAccess.SetDataList(0, _nodes);
            foreach ((GH_Structure<GH_Line> geometry, int i) in _lineList.Select((geo, index) => (geo, index + 1)))
            {
                if (i == 8)
                {
                    dataAccess.SetDataTree(6, geometry);
                }
                else if (i == 6 || i == 7)
                {
                    continue;
                }
                else
                {
                    dataAccess.SetDataTree(i, geometry);
                }
            }
        }

        private void BakeLine()
        {
            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;
            var parentLayerNames = new[] { "Column", "Girder", "Post", "Beam", "Brace", "Slab", "Wall", "Pile" };
            Color[] layerColors = { Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple, Color.DarkGray, Color.CornflowerBlue, Color.DarkOrange };
            GeometryBaker.MakeParentLayers(activeDoc, parentLayerNames, layerColors);

            Dictionary<string, string>[][] infoArray = Utils.TagUtils.GetAllSectionInfoArray(_stBridge.StbModel.StbMembers, _stBridge.StbModel.StbSections);

            foreach ((GH_Structure<GH_Line> lines, int i) in _lineList.Select((frameBrep, index) => (frameBrep, index)))
            {
                if (lines == null)
                {
                    continue;
                }
                Layer parentLayer = activeDoc.Layers.FindName(parentLayerNames[i]);
                int parentIndex = parentLayer.Index;
                Guid parentId = parentLayer.Id;
                foreach ((Line line, int bIndex) in lines.Select((geometry, bIndex) => (geometry.Value, bIndex)))
                {
                    var objAttr = new ObjectAttributes();

                    Dictionary<string, string>[] infos = infoArray[i];
                    Dictionary<string, string> info = infos[bIndex];

                    foreach (KeyValuePair<string, string> pair in info)
                    {
                        objAttr.SetUserString(pair.Key, pair.Value);
                    }

                    var layer = new Layer { Name = info["name"], ParentLayerId = parentId, Color = layerColors[i] };
                    int layerIndex = activeDoc.Layers.Add(layer);
                    if (layerIndex == -1)
                    {
                        layer = activeDoc.Layers.FindName(info["name"]);
                        layerIndex = layer.Index;
                    }
                    objAttr.LayerIndex = layerIndex;

                    activeDoc.Objects.AddLine(line, objAttr);
                }
            }
        }

        private void CreateLine(bool isOffset)
        {
            var createLines = new CreateLineFromStb(_stBridge, isOffset);
            _nodes = createLines.Nodes();
            _lineList[0] = createLines.Columns();
            _lineList[1] = createLines.Girders();
            _lineList[2] = createLines.Posts();
            _lineList[3] = createLines.Beams();
            _lineList[4] = createLines.Braces();
            _lineList[7] = createLines.Piles();
        }

        protected override Bitmap Icon => Resource.Line;
        public override Guid ComponentGuid => new Guid("D1E6793B-F75C-4AEE-9A9F-B9DD08D6EB77");
    }
}
