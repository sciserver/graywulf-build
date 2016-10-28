using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    [TestClass]
    public class SolutionTest : TestBase
    {
        [TestMethod]
        public void LoadSolutionTest()
        {
            var s = new Solution();
            s.LoadSolution(GetTestSolutionPath());
        }
    }
}
