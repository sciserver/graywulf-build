using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Build.Config
{
    [Flags]
    public enum ProjectType
    {
        Unknown,
        ClassLibrary = 1,
        Executable = 2,
        WebApplication = 4,
        UnitTest = 8
    }
}
