using System;
using System.IO;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public abstract class TestBase
    {
        protected string GetTestConfigPath()
        {
            var path = Environment.CurrentDirectory;
            path = Path.Combine(path, @"..\..\..\..\test\config\");

            return path;
        }

        protected string GetTestSolutionPath()
        {
            var path = Environment.CurrentDirectory;
            path = Path.Combine(path, @"..\..\..\..\graywulf-build.sln");

            return path;
        }

        protected string GetTestProjectPath()
        {
            var path = Environment.CurrentDirectory;
            path = Path.Combine(path, @"..\..\Jhu.Graywulf.Build.ConfigUtil.Test.csproj");

            return path;
        }
    }
}
