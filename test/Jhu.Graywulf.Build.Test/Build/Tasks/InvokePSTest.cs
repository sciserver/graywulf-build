using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.Tasks
{
    [TestClass]
    public class InvokePSTest
    {
        [TestMethod]
        public void ExecuteTest()
        {
            var task = new InvokePS()
            {
                BuildEngine = new TestBuildEngine(),
                SolutionDir = "Test",
                Script = Path.GetFullPath(@"..\..\test.ps1"),
            };

            Assert.IsTrue(task.Execute());
        }
    }
}
