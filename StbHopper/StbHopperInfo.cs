using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace StbHopper {
    public class StbHopperInfo : GH_AssemblyInfo {
        public override string Name {
            get {
                return "StbHopper";
            }
        }
        public override Bitmap Icon {
            get {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description {
            get {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id {
            get {
                return new Guid("093de648-746b-4b0b-85ef-495c6fb4514f");
            }
        }

        public override string AuthorName {
            get {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact {
            get {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
