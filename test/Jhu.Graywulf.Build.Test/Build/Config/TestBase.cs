using System;
using System.IO;

namespace Jhu.Graywulf.Build.Config
{
    public abstract class TestBase
    {
        public const string TestProjectName = "Jhu.Graywulf.Build.TestProject";

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
            return MapPath(String.Format(@"test\{0}\{0}.csproj", TestProjectName));
        }
    }
}
