using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeTxGui.Helpers
{
    public static class DirectoryInfoExtension
    {
        public static void CreateWithParent(this DirectoryInfo directory)
        {
            if (directory.Exists)
            {
            }
            else
            {
                if (!directory.Parent.Exists)
                {
                    directory.Parent.CreateWithParent();
                }
                directory.Create();
            }
        }
    }
}
