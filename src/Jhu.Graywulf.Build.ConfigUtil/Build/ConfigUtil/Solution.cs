using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Solution
    {
        private string path;
        private List<Project> projects;
        public List<Config> configurations;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public Solution()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.path = null;
            this.projects = null;
            this.configurations = null;
        }

        public void LoadSolution(string path)
        {
            this.path = path;
            this.projects = new List<Project>();

            var parser_type = Type.GetType("Microsoft.Build.Construction.SolutionParser, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            var parser_solutionReader = parser_type.GetProperty("SolutionReader", BindingFlags.NonPublic | BindingFlags.Instance);
            var parser_parseSolution = parser_type.GetMethod("ParseSolution", BindingFlags.NonPublic | BindingFlags.Instance);
            var parser_projects = parser_type.GetProperty("Projects", BindingFlags.NonPublic | BindingFlags.Instance);
            var project_type = Type.GetType("Microsoft.Build.Construction.ProjectInSolution, Microsoft.Build, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", false, false);
            var project_ProjectName = project_type.GetProperty("ProjectName", BindingFlags.NonPublic | BindingFlags.Instance);
            var project_RelativePath = project_type.GetProperty("RelativePath", BindingFlags.NonPublic | BindingFlags.Instance);
            var project_ProjectType = project_type.GetProperty("ProjectType", BindingFlags.NonPublic | BindingFlags.Instance);

            var parser = parser_type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First().Invoke(null);

            using (var streamReader = new StreamReader(path))
            {
                parser_solutionReader.SetValue(parser, streamReader, null);
                parser_parseSolution.Invoke(parser, null);
            }

            var array = (Array)parser_projects.GetValue(parser, null);

            for (int i = 0; i < array.Length; i++)
            {
                var project = array.GetValue(i);
                var type = (Microsoft.Build.Construction.SolutionProjectType)project_ProjectType.GetValue(project);
                if ((type & Microsoft.Build.Construction.SolutionProjectType.KnownToBeMSBuildFormat) != 0)
                {
                    var pp = (string)project_RelativePath.GetValue(project);
                    var p = new Project(this);
                    p.LoadProject(pp);
                    projects.Add(p);
                }
            }
        }
    }
}
