using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jhu.Graywulf.Build.Config
{
    public class Solution
    {
        private string path;
        private Dictionary<string, SolutionProject> projects;
        public List<Config> configurations;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public Dictionary<string, SolutionProject> Projects
        {
            get { return projects; }
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

        public void MergeConfigs(Settings settings)
        {
            foreach (var project in projects.Values)
            {
                project.MergeConfigs(settings);
            }
        }

        public override string ToString()
        {
            return path;
        }
    }
}
