using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using STBReader;
using Rhino.Geometry;
using STBReader.Member;
using STBReader.Model;

namespace HoaryFox.Component.Tag.Name
{
    public class BeamNameTag:GH_Component
    {
        private StbData _stbData;
        private int _size;

        private readonly List<string> _beamName = new List<string>();
        private readonly List<Point3d> _beamPos = new List<Point3d>();

        public BeamNameTag()
          : base(name: "Beam Name Tag", nickname: "BeamTag", description: "Display Beam Name Tag", category: "HoaryFox", subCategory: "Name")
        {
        }
        
        public override bool IsPreviewCapable => true;

        public override void ClearData()
        {
            base.ClearData();
            _beamName.Clear();
            _beamPos.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge file data", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Size", "S", "Tag size", GH_ParamAccess.item, 12);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Beams", "Beam", "output StbBeams name tag", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            if (!DA.GetData("Size", ref _size)) { return; }

            StbNodes nodes = _stbData.Nodes;
            StbBeams beams = _stbData.Beams;
            
            for (var i = 0; i < beams.Id.Count; i++)
            {
                int idNodeStart = nodes.Id.IndexOf(beams.IdNodeStart[i]);
                int idNodeEnd = nodes.Id.IndexOf(beams.IdNodeEnd[i]);
                _beamName.Add(beams.Name[i]);
                _beamPos.Add(new Point3d(
                    (nodes.X[idNodeStart] + nodes.X[idNodeEnd]) / 2.0,
                    (nodes.Y[idNodeStart] + nodes.Y[idNodeEnd]) / 2.0,
                    (nodes.Z[idNodeStart] + nodes.Z[idNodeEnd]) / 2.0)
                );
            }

            DA.SetDataList(0, _beamName);
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            for (var i = 0; i < _beamName.Count; i++)
            {
                args.Display.Draw2dText(_beamName[i], Color.Black, _beamPos[i], true, _size);
            }
        }

        protected override Bitmap Icon => Properties.Resource.BeamName;
        public override Guid ComponentGuid => new Guid("758DE991-F652-4EDC-BC63-2A454BA43FB1");
    }
}