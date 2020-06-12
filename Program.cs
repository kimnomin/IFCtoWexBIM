using System.IO;
using System.Linq;
using Xbim.COBieLite;
using Xbim.Ifc;
using Xbim.ModelGeometry.Scene;

namespace IFCtoWexBIM
{
    class Program
    {
        static void Main(string[] args)
        {
            const string filename = "20200404UFCSPA-ARQ-PE-R0-correto.ifc";

            using (var model = IfcStore.Open(filename))
            {
                var context = new Xbim3DModelContext(model);
                context.CreateContext();

                var wexBimFilename = Path.ChangeExtension(filename, "wexBim");
                ConvertWexBim(model, wexBimFilename);

                string cobieFileName = Path.ChangeExtension(filename, "json");
                ConvertJson(model, cobieFileName);
                model.Close();
            }
        }

        /// <summary>
        /// IfcStore 모델을 wexbim 파일로 변환하여 저장한다.
        /// </summary>
        /// <param name="model">IfcStore 모델</param>
        /// <param name="filename">저장할 wexbim 파일명</param>
        /// <returns>성공/실패</returns>
        static bool ConvertWexBim(IfcStore model, string filename)
        {
            bool result = false;

            using (var wexBimfile = File.Create(filename))
            {
                using (var wexBimBinaryWriter = new BinaryWriter(wexBimfile))
                {
                    model.SaveAsWexBim(wexBimBinaryWriter);
                    wexBimBinaryWriter.Close();
                    result = true;
                }
                wexBimfile.Close();
            }

            return result;
        }

        /// <summary>
        /// IfcStore 모델을 json 파일로 변환하여 저장한다.
        /// </summary>
        /// <param name="model">IfcStore 모델</param>
        /// <param name="filename">저장할 json 파일명</param>
        /// <returns></returns>
        static bool ConvertJson(IfcStore model, string filename)
        {
            bool result = false;

            using (var cobieFile = new FileStream(filename, FileMode.Create))
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

            return result;
        }
    }
}
