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
        private List<List<Line>> _lineList = new List<List<Line>>();

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
            var parentLayerNames = new[]
            {
                "Column", "Girder", "Post", "Beam", "Brace",
                "Slab", "Wall"
            };
            Color[] layerColors =
            {
                Color.Red, Color.Green, Color.Aquamarine, Color.LightCoral, Color.MediumPurple,
                Color.DarkGray, Color.CornflowerBlue
            };

            MakeParentLayers(activeDoc, parentLayerNames, layerColors);


            var stbFrames = new List<StbFrame>
            {
                _stbData.Columns, _stbData.Girders, _stbData.Posts, _stbData.Beams, _stbData.Braces
            };

            //TODO: このネストは直す
            List<List<List<string>>> tagList = stbFrames.Select(stbFrame => GetTag(stbFrame)).ToList();

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
                        objAttr.SetUserString("Tag", tag[0]);
                        objAttr.SetUserString("ShapeType", tag[1]);
                        objAttr.SetUserString("Height", tag[2]);
                        objAttr.SetUserString("Width", tag[3]);
                        objAttr.SetUserString("t1", tag[4]);
                        objAttr.SetUserString("t2", tag[5]);
                        objAttr.SetUserString("Kind", tag[6]);

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

        private static void MakeParentLayers(RhinoDoc activeDoc, IEnumerable<string> parentLayerNames, IReadOnlyList<Color> layerColors)
        {
            foreach ((string name, int index) in parentLayerNames.Select((name, index) => (name, index)))
            {
                var parentLayer = new Rhino.DocObjects.Layer { Name = name, Color = layerColors[index] };
                activeDoc.Layers.Add(parentLayer);
            }
        }

        private List<List<string>> GetTag(StbFrame stbFrame)
        {
            var tags = new CreateTag(_stbData.Nodes, _stbData.SecColumnRc, _stbData.SecColumnS, _stbData.SecBeamRc, _stbData.SecBeamS, _stbData.SecBraceS, _stbData.SecSteel);
            return tags.FrameList(stbFrame);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.Line;
        public override Guid ComponentGuid => new Guid("7d2f0c4e-4888-4607-8548-592104f6f06d");
    }
}
