using System;
using System.IO;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public abstract class TestBase
    {
        private static string GetSolutionPath()
        {
            var sln = Path.GetDirectoryName(Environment.GetEnvironmentVariable("SolutionPath"));
            return sln;
        }

        private string MapPath(string path)
        {
            string file;

            file = Path.Combine(Environment.CurrentDirectory, @"..\..\..\..", path);

            if (!File.Exists(file) && !Directory.Exists(file))
            {
                file = Path.Combine(GetSolutionPath(), path);
            }

            if (!File.Exists(file) && !Directory.Exists(file))
            {
                file = Path.Combine(GetSolutionPath(), "graywulf-build", path);
            }

            return file;
        }

        protected string GetTestConfigPath()
        {
            return MapPath(@"\test\config\");
        }

        protected string GetTestSolutionPath()
        {
            return MapPath(@"graywulf-build.sln");
        }

        protected string GetTestProjectPath()
        {
            return MapPath(@"test\Jhu.Graywulf.Build.ConfigUtil.Test\Jhu.Graywulf.Build.ConfigUtil.Test.csproj");
        }
    }
}
