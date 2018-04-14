using System;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Jhu.Graywulf.Build.Tasks
{
    public abstract class TaskBase : Task
    {
        #region Private member variables

        private string solutionDir;
        private string solutionName;
        private string configurationName;
        private string platformName;
        private string projectDir;
        private string projectName;
        private string targetName;
        private string outputType;
        private string outDir;

        #endregion
        #region Properties

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
        public string ConfigurationName
        {
            get { return configurationName; }
            set { configurationName = value; }
        }

        [Required]
        public string PlatformName
        {
            get { return platformName; }
            set { platformName = value; }
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
        public string TargetName
        {
            get { return targetName; }
            set { targetName = value; }
        }

        public string OutputType
        {
            get { return outputType; }
            set { outputType = value; }
        }

        [Required]
        public string OutDir
        {
            get { return outDir; }
            set { outDir = value; }
        }

        #endregion
        #region Constructors and iniializers

        protected TaskBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.solutionDir = null;
            this.solutionName = null;
            this.configurationName = null;
            this.platformName = null;
            this.projectDir = null;
            this.projectName = null;
            this.targetName = null;
            this.outputType = null;
            this.outDir = null;
        }

        #endregion

        protected string GetOutputExtension()
        {
            switch (outputType.ToLowerInvariant())
            {
                case "exe":
                case "winexe":
                    return ".exe";
                case "library":
                    return ".dll";
                default:
                    throw new NotImplementedException();
            }
        }

        protected string GetTargetPathWithoutExtension()
        {
            return Path.Combine(projectDir, outDir, targetName);
        }

        protected string GetTargetPath()
        {
             return GetTargetPathWithoutExtension() + GetOutputExtension();
        }
    }
}
