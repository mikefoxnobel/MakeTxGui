using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeTxGui.Helpers
{
    public class MakeTxHelper
    {
        #region Field
        private string _makeTxPath = @"C:\solidangle\mtoadeploy\2017\bin";
        private string _makeTxFilename = @"maketx.exe";
        private string _makeTxParameters = @"-v -u --unpremult --oiio";
        private string _makeTxFullFilename => Path.Combine(_makeTxPath, _makeTxFilename);
        #endregion

        #region Property
        public string MakeTxPath { get => _makeTxPath; set => _makeTxPath = value; }
        public string MakeTxFilename { get => _makeTxFilename; set => _makeTxFilename = value; }
        public string MakeTxParameters { get => _makeTxParameters; set => _makeTxParameters = value; }
        #endregion

        public int CallMakeTx(out List<string> stdout, out List<string> errout, string sourceFile, string targetFile)
        {
            string parameters = String.Format("{0} {1} -o {2}", _makeTxParameters, sourceFile, targetFile);

            FileInfo source = new FileInfo(sourceFile);
            if (!source.Exists)
            {
                stdout = new List<string>();
                errout = new List<string>();
                errout.Add(String.Format("File {0} doesn't exists.", sourceFile));
                return 255;
            }
            FileInfo target = new FileInfo(targetFile);
            if (!target.Directory.Exists)
            {
                target.Directory.CreateWithParent();
            }
            return ProcessHelper.StartProcess(out stdout, out errout, _makeTxFullFilename, parameters, _makeTxPath, 30000);
        }
    }
}
