using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Grasshopper.Kernel;
using HoaryFox.STB;
using Rhino.Geometry;

namespace HoaryFox.Component.NameTag
{
    public class BraceNameTag:GH_Component
    {
        private string _path;
        private int _size;

        private static StbNodes _stbNodes;
        private static StbBraces _stbBraces;

        private readonly List<Point3d> _nodes = new List<Point3d>();
        private readonly List<string> _braces = new List<string>();
        private readonly List<Point3d> _bracePos = new List<Point3d>();

        public BraceNameTag()
          : base(name: "Brace Name Tag", nickname: "S2BrTg", description: "Read ST-Bridge file and display", category: "HoaryFox", subCategory: "Tags")
        {
        }
        
        public override bool IsPreviewCapable => true;

        public override void ClearData()
        {
            base.ClearData();
            _nodes.Clear();
            _braces.Clear();
            _bracePos.Clear();
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("path", "path", "input ST-Bridge file path", GH_ParamAccess.item);
            pManager.AddIntegerParameter("size", "size", "Tag size", GH_ParamAccess.item, 12);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Braces", "Brc", "output StbBraces to Line", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            // 対象の stb の pathを取得
            if (!DA.GetData("path", ref _path)) { return; }
            if (!DA.GetData("size", ref _size)) { return; }
            var xDocument = XDocument.Load(_path);

            Init();
            Load(xDocument);

            // StbNode の取得
            for (var i = 0; i < _stbNodes.Id.Count; i++)
            {
                var position = new Point3d(_stbNodes.X[i], _stbNodes.Y[i], _stbNodes.Z[i]);
                _nodes.Add(position);
            }
            
            // StbBrace の取得
            for (var i = 0; i < _stbBraces.Id.Count; i++)
            {
                var idNodeStart = _stbNodes.Id.IndexOf(_stbBraces.IdNodeStart[i]);
                var idNodeEnd = _stbNodes.Id.IndexOf(_stbBraces.IdNodeEnd[i]);
                var name = _stbBraces.Name[i];
                _bracePos.Add(new Point3d( (_nodes[idNodeStart].X + _nodes[idNodeEnd].X) / 2.0,
                    (_nodes[idNodeStart].Y + _nodes[idNodeEnd].Y) / 2.0,
                    (_nodes[idNodeStart].Z + _nodes[idNodeEnd].Z) / 2.0)
                );
                _braces.Add(name);
            }

            DA.SetDataList(0, _braces);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (int i = 0; i < _braces.Count; i++)
                args.Display.Draw2dText(_braces[i], Color.Black, _bracePos[i], true, _size);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("E566DDCB-4192-40B2-8E96-2083207CC5A8");

        private static void Init()
        {
            _stbNodes = new StbNodes();
            _stbBraces = new StbBraces();
        }

        private static void Load(XDocument xDocument)
        {
            var members = new List<StbData>()
            {
                _stbNodes, _stbBraces
            };

            foreach (var member in members)
            {
                member.Load(xDocument);
            }
        }
    }
}