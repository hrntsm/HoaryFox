using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Special.SketchElements;
using HoaryFox.Member;
using HoaryFox.STB;
using Rhino.Geometry;
using System.Drawing;
using Karamba.Utilities;
using Karamba.Geometry;
using Karamba.CrossSections;
using Karamba.Supports;
using Karamba.Loads;
using Karamba.GHopper;
using Karamba.GHopper.CrossSections;
using Karamba.GHopper.Elements;
using Karamba.Materials;


namespace HoaryFox.Component.Export
{
    public class Stb2KElem:GH_Component
    {
        private StbData _stbData;
        private List<GH_Element> _k3Elem = new List<GH_Element>();
        private List<string>[] _k3Ids = new List<string>[2];
        private List<CroSec> _k3CroSec = new List<CroSec>();

        public Stb2KElem()
          : base("Stb to Karamba", "S2K", "Read ST-Bridge file and display", "HoaryFox", "Export")
        {
        }

        public override void ClearData()
        {
            base.ClearData();
            _k3Elem.Clear();
            _k3CroSec.Clear();
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddGenericParameter("Data", "D", "input ST-Bridge Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddParameter(new Param_Element(), "Element", "Elem", "Karamba Element", GH_ParamAccess.list);
            pManager.AddParameter(new Param_CrossSection(), "CrossSection", "CroSec", "Karamba CrossSection", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (!DA.GetData("Data", ref _stbData)) { return; }
            
            var logger = new MessageLogger();
            var k3d = new KarambaCommon.Toolkit();
            var nodes = new List<Point3>();
            var k3Elems = new List<Line3>[2];
            k3Elems[0] = new List<Line3>();
            k3Elems[1] = new List<Line3>();
            _k3Ids[0] = new List<string>();
            _k3Ids[1] = new List<string>();

            GetCroSec();
            
            for (var i = 0; i < _stbData.Columns.Id.Count; i++)
            {
                var idNodeStart = _stbData.Nodes.Id.IndexOf(_stbData.Columns.IdNodeStart[i]);
                var idNodeEnd = _stbData.Nodes.Id.IndexOf(_stbData.Columns.IdNodeEnd[i]);
                var p0 = new Point3(
                    _stbData.Nodes.Pt[idNodeStart].X / 1000d,
                    _stbData.Nodes.Pt[idNodeStart].Y / 1000d,
                    _stbData.Nodes.Pt[idNodeStart].Z / 1000d
                );
                var p1 = new Point3(
                    _stbData.Nodes.Pt[idNodeEnd].X / 1000d,
                    _stbData.Nodes.Pt[idNodeEnd].Y / 1000d,
                    _stbData.Nodes.Pt[idNodeEnd].Z / 1000d
                );
                k3Elems[0].Add(new Line3(p0, p1));
            }
            for (var i = 0; i < _stbData.Girders.Id.Count; i++)
            {
                var idNodeStart = _stbData.Nodes.Id.IndexOf(_stbData.Girders.IdNodeStart[i]);
                var idNodeEnd = _stbData.Nodes.Id.IndexOf(_stbData.Girders.IdNodeEnd[i]);
                var p0 = new Point3(
                    _stbData.Nodes.Pt[idNodeStart].X / 1000d,
                    _stbData.Nodes.Pt[idNodeStart].Y / 1000d,
                    _stbData.Nodes.Pt[idNodeStart].Z / 1000d
                );
                var p1 = new Point3(
                    _stbData.Nodes.Pt[idNodeEnd].X / 1000d,
                    _stbData.Nodes.Pt[idNodeEnd].Y / 1000d,
                    _stbData.Nodes.Pt[idNodeEnd].Z / 1000d
                );
                k3Elems[0].Add(new Line3(p0, p1));
            }
            var elems = k3d.Part.LineToBeam(k3Elems[0], _k3Ids[0], new List<CroSec>(){}, logger, out nodes);
            
            for (var i = 0; i < _stbData.Braces.Id.Count; i++)
            {
                var idNodeStart = _stbData.Nodes.Id.IndexOf(_stbData.Braces.IdNodeStart[i]);
                var idNodeEnd = _stbData.Nodes.Id.IndexOf(_stbData.Braces.IdNodeEnd[i]);
                var p0 = new Point3(
                    _stbData.Nodes.Pt[idNodeStart].X / 1000d,
                    _stbData.Nodes.Pt[idNodeStart].Y / 1000d,
                    _stbData.Nodes.Pt[idNodeStart].Z / 1000d
                );
                var p1 = new Point3(
                    _stbData.Nodes.Pt[idNodeEnd].X / 1000d,
                    _stbData.Nodes.Pt[idNodeEnd].Y / 1000d,
                    _stbData.Nodes.Pt[idNodeEnd].Z / 1000d
                );
                k3Elems[1].Add(new Line3(p0, p1));
            }
            elems.AddRange(k3d.Part.LineToBeam(k3Elems[1], _k3Ids[1], new List<CroSec>(){}, logger, out nodes, false));
            
            var elemList = new List<GH_Element>();
            foreach (var e in elems)
            {
                elemList.Add(new GH_Element(e));
            }
            _k3Elem = elemList;

            DA.SetDataList(0, _k3Elem);
            DA.SetDataList(1, _k3CroSec);
        }

        protected override System.Drawing.Bitmap Icon => Properties.Resource.Brep;

        public override Guid ComponentGuid => new Guid("C57461DA-E79B-49A0-B44B-71CF32057709");

        private void GetCroSec()
        {
            double p1;
            double p2;
            double p3;
            double p4;
            string name;
            ShapeTypes shapeType;

            for (int eNum = 0; eNum < _stbData.Columns.Id.Count; eNum++)
                _k3Ids[0].Add(_stbData.Columns.Name[eNum]);
            
            for (int eNum = 0; eNum < _stbData.Girders.Id.Count; eNum++)
                _k3Ids[0].Add(_stbData.Girders.Name[eNum]);
            
            for (int eNum = 0; eNum < _stbData.Braces.Id.Count; eNum++)
                _k3Ids[1].Add(_stbData.Braces.Name[eNum]);

            var fc21 = new FemMaterial_Isotrop("Concrete", "Fc21", 2186, 910.8, 910.8, 24, 1.00E-05, 1.67, Color.Gray);
            var sn400 = new FemMaterial_Isotrop("Steel", "SN400", 20600, 8076, 8076, 78.5, 1.20E-05, 23.5, Color.Brown);


            for (var i = 0; i < _stbData.SecColumnRc.Id.Count; i++)
            {
                name = _stbData.SecColumnRc.Floor[i] + _stbData.SecColumnRc.Name[i];
                p1 = _stbData.SecColumnRc.Height[i] / 10d;
                p2 = _stbData.SecColumnRc.Width[i] / 10d;
                
                shapeType = _stbData.SecColumnRc.Height[i] <= 0 ? ShapeTypes.Pipe : ShapeTypes.BOX;
                
                if (shapeType == ShapeTypes.BOX)
                {
                    // TODO:材料の設定は直す
                    var croSec = new CroSec_Trapezoid("HoaryFox", name, null, null, fc21,
                        p1, p2, p2);
                    croSec.AddElemId(name);
                    _k3CroSec.Add(croSec);
                }
                else
                {
                    // TODO: Karambaは中実円断面ないため、PIPEに置換してる。任意断面設定できるはずなので、そっちの方がいい気がする。
                    var croSec = new CroSec_Circle("HoaryFox", name, null, null, fc21,
                        p2, p2/2);
                    croSec.AddElemId(name);
                    _k3CroSec.Add(croSec);
                }
            }

            for (var i = 0; i < _stbData.SecBeamRc.Id.Count; i++)
            {
                var floor = _stbData.SecBeamRc.IsFoundation[i] ? "1" : _stbData.SecBeamRc.Floor[i];
                name = floor + _stbData.SecBeamRc.Name[i];
                p1 = _stbData.SecBeamRc.Depth[i] / 10d;
                p2 = _stbData.SecBeamRc.Width[i] / 10d;
                
                var croSec = new CroSec_Trapezoid("HoaryFox", name, null, null, fc21,
                    p1, p2, p2);
                croSec.AddElemId(name);
                _k3CroSec.Add(croSec);
            }

            for (var i = 0; i < _stbData.SecSteel.Name.Count; i++)
            {
                name = _stbData.SecSteel.Name[i];
                p1 = _stbData.SecSteel.P1[i] / 10d;
                p2 = _stbData.SecSteel.P2[i] / 10d;
                if (_stbData.SecSteel.P3.Count < i+1)
                    p3 = 0;
                else
                    p3 = _stbData.SecSteel.P3[i] / 10d;
                if (_stbData.SecSteel.P4.Count < i+1)
                    p4 = 0;
                else
                    p4 = _stbData.SecSteel.P4[i] / 10d;
                shapeType = _stbData.SecSteel.ShapeType[i];

                CroSec croSec = null;
                double eLength;
                switch (shapeType)
                {
                    case ShapeTypes.H:
                        croSec = new CroSec_I("HoaryFox", name, null, null, sn400,
                            p1, p2, p2, p4, p4, p3);
                        break;
                    case ShapeTypes.L:
                        // TODO:Karambaに対応断面形状がないため等価軸断面積置換
                        eLength = Math.Sqrt(p1 * p3 + p2 * p4 - p3 * p4);
                        croSec = new CroSec_Trapezoid("HoaryFox", name, null, null, sn400,
                            eLength, eLength, eLength);
                        break;
                    case ShapeTypes.T:
                        croSec = new CroSec_T("HoaryFox", name, null, null, sn400,
                            p1, p2, p3, p4);
                        break;
                    case ShapeTypes.C:
                        // TODO:Karambaに対応断面形状がないため等価軸断面積置換
                        eLength = Math.Sqrt(p1 * p3 + p2 * p4 - 2 * p3 * p4);
                        croSec = new CroSec_Trapezoid("HoaryFox", name, null, null, sn400,
                            eLength, eLength, eLength);
                        break;
                    case ShapeTypes.FB:
                        croSec = new CroSec_Trapezoid("HoaryFox", name, null, null, sn400,
                            p1, p2, p2);
                        break;
                    case ShapeTypes.BOX:
                        break;
                    case ShapeTypes.Bar:
                        // TODO: Karambaは中実円断面ないため、PIPEに置換してる。任意断面設定できるはずなので、そっちの方がいい気がする。
                        croSec = new CroSec_Circle("HoaryFox", name, null, null, sn400,
                            p2, p2/2);
                        break;
                    case ShapeTypes.Pipe:
                        croSec = new CroSec_Circle("HoaryFox", name, null, null, sn400,
                            p2, p1);
                        break;
                    case ShapeTypes.RollBOX:
                        croSec = new CroSec_Box("HoaryFox", name, null, null, sn400,
                            p1, p2, p2, p3, p3, p3, p4);
                        break;
                    case ShapeTypes.BuildBOX:
                        croSec = new CroSec_Box("HoaryFox", name, null, null, sn400,
                            p1, p2, p2, p3, p3, p4, 0);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                for (var j = 0; j < _stbData.SecColumnS.Id.Count; j++)
                {
                    if (_stbData.SecColumnS.Shape[j] != name) continue;
                    if (croSec != null)
                        croSec.AddElemId(_stbData.SecColumnS.Floor[j] + _stbData.SecColumnS.Name[j]);
                }

                for (var j = 0; j < _stbData.SecBeamS.Id.Count; j++)
                {
                    if (_stbData.SecBeamS.Shape[j] != name) continue;
                    if (croSec != null)
                        croSec.AddElemId(_stbData.SecBeamS.Floor[j] + _stbData.SecBeamS.Name[j]);
                }

                for (var j = 0; j < _stbData.SecBraceS.Id.Count; j++)
                {
                    if (_stbData.SecBraceS.Shape[j] != name) continue;
                    if (croSec != null)
                        croSec.AddElemId(_stbData.SecBraceS.Floor[j] + _stbData.SecBraceS.Name[j]);
                }
                
                _k3CroSec.Add(croSec);
            }
        }
    }
}
