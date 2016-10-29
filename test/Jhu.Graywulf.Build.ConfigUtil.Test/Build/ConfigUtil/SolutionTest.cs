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
            var path = GetTestSolutionPath();
            var s = new Solution();
            s.LoadSolution(path);
        }

        [TestMethod]
        public void MergeConfigTest()
        {
            var settings = new Settings()
            {
                ConfigRoot =  GetTestConfigPath(),
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
                ConfigRoot = GetTestConfigPath(),
                SkipMissingBaseConfig = false,
            };

            var path = GetTestSolutionPath();
            var s = new Solution();
            s.LoadSolution(path);
            s.MergeConfigs(settings);
        }
    }
}
