using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Jhu.Graywulf.Build.Tasks
{
    /// <summary>
    /// MSBuild tasks to generate merge config files and generate AssemblyInfo.cs
    /// </summary>
    public class Configure : Task
    {
        private string solutionDir;
        private string solutionName;
        private string projectDir;
        private string projectName;
        private string assemblyName;
        private string outputType;
        private string projectTypeGuids;

        [Required]
        public string SolutionDir
        {
            get { return solutionDir; }
            set { solutionDir = value; }
        }

        [Required]
        public string SolutionName
        {
            get { return solutionName; }
            set { solutionName = value; }
        }

        [Required]
        public string ProjectDir
        {
            get { return projectDir; }
            set { projectDir = value; }
        }

        [Required]
        public string ProjectName
        {
            get { return projectName; }
            set { projectName = value; }
        }

        [Required]
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        [Required]
        public string OutputType
        {
            get { return outputType; }
            set { outputType = value; }
        }

        public string ProjectTypeGuids
        {
            get { return projectTypeGuids; }
            set { projectTypeGuids = value; }
        }

        public override bool Execute()
        {
            var settings = new Config.Settings()
            {
                SkipMissingBaseConfig = true,
            };

            var solution = new Config.Solution()
            {
                Path = System.IO.Path.Combine(solutionDir, solutionName),
            };

            var project = new Config.SolutionProject(solution)
            {
                Path = System.IO.Path.Combine(projectDir, projectName),
                Name = System.IO.Path.GetFileNameWithoutExtension(projectName),
                AssemblyName = assemblyName,
            };

            try
            {
                project.SetProjectType(outputType, projectTypeGuids);
                project.FindConfigs();
                project.MergeConfigs(settings);
                project.GenerateAssemblyInfoFile();
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
            }

            return true;
        }
    }
}
