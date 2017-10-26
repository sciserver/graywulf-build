using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.SqlClr
{
    [TestClass]
    public class SqlClrReflectorTest
    {
        [TestMethod]
        public void ReflectAssemblyTest()
        {
            var sec = AssemblySecurityLevel.Safe;
            var targetPath = @"..\..\..\Jhu.Graywulf.Build.TestProject\bin\Debug\Jhu.Graywulf.Build.TestProject.dll";
            var buildpath = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
            var adinfo = new AppDomainSetup() { ApplicationBase = buildpath };
            var ad = AppDomain.CreateDomain("SqlClrReflector", null, adinfo);
            var proxy = (SqlClrReflector)ad.CreateInstanceAndUnwrap(typeof(SqlClrReflector).Assembly.FullName, typeof(SqlClrReflector).FullName);

            proxy.ReflectAssembly(targetPath, sec);

            proxy.Dispose();
            AppDomain.Unload(ad);
        }
    }
}
