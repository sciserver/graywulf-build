﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.Config
{
    [TestClass]
    public class GitCommitInfoTest : TestBase
    {
        [TestMethod]
        public void GetGitCommitInfoTest()
        {
            var s = new Solution();
            s.LoadSolution(GetTestSolutionPath());
            var p = s.Projects[TestProjectName];
            var gitinfo = p.GetGitCommitInfo("graywulf");

            Assert.IsNotNull(gitinfo.Hash);   
            Assert.IsNotNull(gitinfo.Tag);
            Assert.IsNotNull(gitinfo.Version);
            Assert.IsTrue(int.Parse(gitinfo.Revision) > -1);
        }
    }
}
