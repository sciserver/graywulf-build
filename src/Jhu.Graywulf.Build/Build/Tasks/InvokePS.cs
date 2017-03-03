using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Jhu.Graywulf.Build.Tasks
{
    /// <summary>
    /// MSBuild task to execute PowerShell scripts
    /// </summary>
    public class InvokePS : Task
    {
        private string solutionDir;
        private string solutionName;
        private string projectDir;
        private string projectName;
        private string outDir;
        private string targetName;
        private string script;

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
        public string OutDir
        {
            get { return outDir; }
            set { outDir = value; }
        }

        [Required]
        public string TargetName
        {
            get { return targetName; }
            set { targetName = value; }
        }

        [Required]
        public string Script
        {
            get { return script; }
            set { script = value; }
        }

        public InvokePS()
        {
        }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "START: {0}", DateTime.Now);

            var iss = InitialSessionState.CreateDefault();
            iss.Variables.Add(new SessionStateVariableEntry("SolutionDir", solutionDir, null));
            iss.Variables.Add(new SessionStateVariableEntry("SolutionName", solutionName, null));
            iss.Variables.Add(new SessionStateVariableEntry("ProjectDir", projectDir, null));
            iss.Variables.Add(new SessionStateVariableEntry("ProjectName",  projectName, null));
            iss.Variables.Add(new SessionStateVariableEntry("OutDir",  outDir, null));
            iss.Variables.Add(new SessionStateVariableEntry("TargetName",  targetName, null));

            using (var rs = RunspaceFactory.CreateRunspace(iss))
            {
                rs.Open();

                using (var ps = PowerShell.Create())
                {
                    ps.Runspace = rs;

                    ps.AddScript(". " + script);

                    foreach (var res in ps.Invoke())
                    {
                        Log.LogMessage(MessageImportance.High, "{0}", res.BaseObject);
                    }

                    foreach (var e in ps.Streams.Error)
                    {
                        Log.LogError("{0}", e.ToString());
                    }

                    foreach (var e in ps.Streams.Verbose)
                    {
                        Log.LogMessage(MessageImportance.Low, "{1}", e.ToString());
                    }

                    Log.LogMessage(MessageImportance.High, "END: {0}", DateTime.Now);

                    return !ps.HadErrors;
                }
            }
        }
    }
}
