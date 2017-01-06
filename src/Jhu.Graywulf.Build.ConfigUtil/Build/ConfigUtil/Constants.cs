using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Constants
    {
        public static readonly Guid WebApplicationGuid = new Guid("349c5851-65df-11da-9384-00065b846f21");
        public static readonly Guid UnitTestGuid = new Guid("3AC096D0-A1C2-E12C-1390-A8335801FDAB");

        public const string GraywulfConfig = "build.config";
        public const string AppConfig = "app.config";
        public const string WebConfig = "web.config";
        public const string BaseConfig = "base.config";

        public const string PropertiesFolder = "Properties";
        public const string AssemblyInfo = "AssemblyInfo.cs";
    }
}
