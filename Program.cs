using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;
using Xbim.COBieLite;

namespace IFCtoWexBIM
{
    class Program
    {
        static void Main(string[] args)
        {
            //const string filename = "20200404CV.01.ifc";
            //const string filename = "20200404UFCSPA-ARQ-PE-R0-correto.ifc";
            const string filename = "KICT신관_151102.ifc";

            using (var model = IfcStore.Open(filename))
            {
                var context = new Xbim3DModelContext(model);
                context.CreateContext();

                var wexBimFilename = Path.ChangeExtension(filename, "wexBim");
                using (var wexBimfile = File.Create(wexBimFilename))
                {
                    using (var wexBimBinaryWriter = new BinaryWriter(wexBimfile))
                    {
                        model.SaveAsWexBim(wexBimBinaryWriter);
                        wexBimBinaryWriter.Close();
                    }
                    wexBimfile.Close();
                }

                string cobieFileName = Path.ChangeExtension(filename, "json");
                using (var cobieFile = new FileStream(cobieFileName, FileMode.Create))
                {
                    var helper = new CoBieLiteHelper(model, "UniClass");
                    var fac = helper.GetFacilities().FirstOrDefault();
                    if (fac != null)
                    {
                        using (var writer = new StreamWriter(cobieFile))
                        {
                            CoBieLiteHelper.WriteJson(writer, fac);
                            writer.Close();
                        }
                    }
                }
                model.Close();
            }
        }
    }
}
