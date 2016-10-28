using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    [TestClass]
    public class ProjectTest : TestBase
    {
        [TestMethod]
        public void LoadProjectTest()
        {
            var s = new Solution();
            s.LoadSolution(GetTestSolutionPath());
            var p = new Project(s);
            p.LoadProject(GetTestProjectPath());
        }
    }
}
