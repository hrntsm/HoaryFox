using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using HoaryFox.Member;
using Rhino;
using Rhino.DocObjects;
using Rhino.Geometry;
using STBReader;
using STBReader.Member;


namespace HoaryFox.Component.Geometry
{
    public class Stb2Line : GH_Component
    {
        private StbData _stbData;
        private List<Point3d> _nodes = new List<Point3d>();
        private readonly List<List<Line>> _lineList = new List<List<Line>>();

        public Stb2Line()
          : base(name: "Stb to Line", nickname: "S2L", description: "Read ST-Bridge file and display", category: "HoaryFox", subCategory: "Geometry")
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

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            var isBake = false;
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Bake", ref isBake)) { return; }

            MakeLine(isBake);
            DA.SetDataList(0, _nodes);
            foreach ((List<Line> geometry, int i) in _lineList.Select((geo, index) => (geo, index + 1)))
            {
                DA.SetDataList(i, geometry);
            }
        }

        private void MakeLine(bool isBake)
        {
            var createLines = new FrameLines(_stbData);
            _nodes = createLines.Nodes();
            _lineList.Add(createLines.Columns());
            _lineList.Add(createLines.Girders());
            _lineList.Add(createLines.Posts());
            _lineList.Add(createLines.Beams());
            _lineList.Add(createLines.Braces());

            if (isBake)
            {
                this.BakeLines();
            }
        }

        private void BakeLines()
        {
            RhinoDoc activeDoc = RhinoDoc.ActiveDoc;

            var parentLayerNames = new[] { "Column", "Girder", "Post", "Beam", "Brace", "Slab", "Wall" };
            Color[] layerColors = { Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple, Color.DarkGray, Color.CornflowerBlue };
            Misc.MakeParentLayers(activeDoc, parentLayerNames, layerColors);
            var stbFrames = new List<StbFrame> { _stbData.Columns, _stbData.Girders, _stbData.Posts, _stbData.Beams, _stbData.Braces };

            //TODO: このネストは直す
            List<List<List<string>>> tagList = stbFrames.Select(stbFrame => Misc.GetTag(_stbData, stbFrame)).ToList();

            foreach ((List<Line> lines, int index) in _lineList.Select((frameBrep, index) => (frameBrep, index)))
            {
                Layer parentLayer = activeDoc.Layers.FindName(parentLayerNames[index]);
                int parentIndex = parentLayer.Index;
                Guid parentId = parentLayer.Id;
                foreach ((Line line, int bIndex) in lines.Select((brep, bIndex) => (brep, bIndex)))
                {
                    var objAttr = new ObjectAttributes();
                    objAttr.SetUserString("Type", parentLayerNames[index]);

                    if (index < 5)
                    {
                        List<List<string>> tags = tagList[index];
                        List<string> tag = tags[bIndex];
                        Misc.SetFrameUserString(ref objAttr, tag);

                        var layer = new Layer { Name = tag[0], ParentLayerId = parentId, Color = layerColors[index] };
                        int layerIndex = activeDoc.Layers.Add(layer);
                        if (layerIndex == -1)
                        {
                            layer = activeDoc.Layers.FindName(tag[0]);
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

        protected override Bitmap Icon => Properties.Resource.Line;
        public override Guid ComponentGuid => new Guid("7d2f0c4e-4888-4607-8548-592104f6f06d");
    }
}
