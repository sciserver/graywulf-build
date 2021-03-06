﻿using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Build.Tasks
{
    [TestClass]
    public class GenerateSqlTest
    {
        [TestMethod]
        public void GenerateScriptsTest()
        {
            var task = new GenerateSql()
            {
                BuildEngine = new TestBuildEngine(),
                ProjectDir = Path.GetFullPath(@"..\..\..\Jhu.Graywulf.Build.TestProject\"),
                ProjectName = "Jhu.Graywulf.Build.TestProject",
                TargetName = "Jhu.Graywulf.Build.TestProject",
                OutputType = "Library",
                OutDir = @"bin\Debug\"
            };

            Assert.IsTrue(task.Execute());
        }
    }
}
