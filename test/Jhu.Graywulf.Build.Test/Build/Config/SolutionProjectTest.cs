using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.Config
{
    [TestClass]
    public class SolutionProjectTest : TestBase
    {
        [TestMethod]
        public void LoadProjectTest()
        {
            var s = new Solution();
            s.LoadSolution(GetTestSolutionPath());
            var p = new SolutionProject(s);
            p.LoadProject(GetTestProjectPath());
        }

        [TestMethod]
        public void GenerateAssemblyInfoFileTest()
        {
            var s = new Solution();
            s.LoadSolution(GetTestSolutionPath());
            var p = s.Projects[TestProjectName];
            p.GenerateAssemblyInfoFile();
        }
    }
}
