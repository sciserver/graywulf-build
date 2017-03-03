using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.Config
{
    [TestClass]
    public class SolutionTest : TestBase
    {
        [TestMethod]
        public void LoadSolutionTest()
        {
            var path = GetTestSolutionPath();
            var s = new Solution();
            s.LoadSolution(path);
        }

        [TestMethod]
        public void MergeConfigTest()
        {
            var settings = new Settings()
            {
                SkipMissingBaseConfig = true,
            };

            var path = GetTestSolutionPath();
            var s = new Solution();
            s.LoadSolution(path);
            s.MergeConfigs(settings);
        }

        [TestMethod]
        [ExpectedException(typeof(ConfigException))]
        public void MissingBaseConfigTest()
        {
            var settings = new Settings()
            {
                SkipMissingBaseConfig = false,
            };

            var path = GetTestSolutionPath();
            var s = new Solution();
            s.LoadSolution(path);
            s.MergeConfigs(settings);
        }
    }
}
