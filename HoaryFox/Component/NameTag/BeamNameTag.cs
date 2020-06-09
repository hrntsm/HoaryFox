using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using Grasshopper.Kernel;
using HoaryFox.STB;
using Rhino.Geometry;

namespace HoaryFox.Component.NameTag
{
    public class BeamNameTag:GH_Component
    {
        private string _path;
        private int _size;

        private static StbNodes _stbNodes;
        private static StbBeams _stbBeams;

        private readonly List<Point3d> _nodes = new List<Point3d>();
        private readonly List<string> _beams = new List<string>();
        private readonly List<Point3d> _beamPos = new List<Point3d>();

        public BeamNameTag()
          : base(name: "Beam Name Tag", nickname: "BeamTag", description: "Display Beam Name Tag", category: "HoaryFox", subCategory: "Name")
        {
        }
        
        public override bool IsPreviewCapable => true;

        public override void ClearData()
        {
            base.ClearData();
            _nodes.Clear();
            _beams.Clear();
            _beamPos.Clear();
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
            pManager.AddTextParameter("Beams", "Beam", "output StbBeams to Line", GH_ParamAccess.list);
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

            // StbBeam の取得
            for (var i = 0; i < _stbBeams.Id.Count; i++)
            {
                var idNodeStart = _stbNodes.Id.IndexOf(_stbBeams.IdNodeStart[i]);
                var idNodeEnd = _stbNodes.Id.IndexOf(_stbBeams.IdNodeEnd[i]);
                var name = _stbBeams.Name[i];
                _beamPos.Add(new Point3d( (_nodes[idNodeStart].X + _nodes[idNodeEnd].X) / 2.0,
                    (_nodes[idNodeStart].Y + _nodes[idNodeEnd].Y) / 2.0,
                    (_nodes[idNodeStart].Z + _nodes[idNodeEnd].Z) / 2.0)
                );
                _beams.Add(name);
            }

            DA.SetDataList(0, _beams);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (int i = 0; i < _beams.Count; i++)
                args.Display.Draw2dText(_beams[i], Color.Black, _beamPos[i], true, _size);
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => Properties.Resource.BeamName;

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("758DE991-F652-4EDC-BC63-2A454BA43FB1");

        private static void Init()
        {
            _stbNodes = new StbNodes();
            _stbBeams = new StbBeams();
        }

        private static void Load(XDocument xDocument)
        {
            var members = new List<StbData>()
            {
                _stbNodes, _stbBeams
            };

            foreach (var member in members)
            {
                member.Load(xDocument);
            }
        }
    }
}