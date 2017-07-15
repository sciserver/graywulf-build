using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Jhu.Graywulf.Build.Tasks
{
    /// <summary>
    /// MSBuild tasks to generate merge config files and generate AssemblyInfo.cs
    /// </summary>
    public class Configure : TaskBase
    {
        #region Private member variables

        private string projectTypeGuids;

        #endregion
        #region Properties

        [Required]
        public string ProjectTypeGuids
        {
            get { return projectTypeGuids; }
            set { projectTypeGuids = value; }
        }

        #endregion
        #region Constructors and initializers

        public Configure()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.projectTypeGuids = null;
        }

        #endregion

        public override bool Execute()
        {
            bool res;
            var start = DateTime.Now;
            Log.LogMessage(MessageImportance.Low, "Starting Graywulf configuration task.");

            var settings = new Config.Settings()
            {
                SkipMissingBaseConfig = true,
            };

            var solution = new Config.Solution()
            {
                Path = System.IO.Path.Combine(SolutionDir, SolutionName),
            };

            var project = new Config.SolutionProject(solution)
            {
                Path = System.IO.Path.Combine(ProjectDir, ProjectName),
                Name = System.IO.Path.GetFileNameWithoutExtension(ProjectName),
                AssemblyName = TargetName,
            };

            try
            {
                project.SetProjectType(OutputType, projectTypeGuids);
                project.FindConfigs();
                project.MergeConfigs(settings);
                project.GenerateAssemblyInfoFile();

                res = true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                res = false;
            }

            Log.LogMessage(MessageImportance.Low, "Finished Graywulf configuration task.");

            return res;
        }
    }
}
