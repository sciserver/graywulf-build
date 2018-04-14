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
    public class InvokePS : TaskBase
    {
        #region Private member variables

        private string script;

        #endregion
        #region Properties

        [Required]
        public string Script
        {
            get { return script; }
            set { script = value; }
        }

        #endregion
        #region Constructors and initializers

        public InvokePS()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.script = null;
        }

        #endregion

        public override bool Execute()
        {
            bool res = true;
            var start = DateTime.Now;
            Log.LogMessage(MessageImportance.Low, "Starting PowerShell script: {0}.", script);

            var src = System.IO.File.ReadAllText(script);

            if (!String.IsNullOrWhiteSpace(src))
            {
                var iss = InitialSessionState.CreateDefault();
                iss.Variables.Add(new SessionStateVariableEntry("SolutionDir", SolutionDir, null));
                iss.Variables.Add(new SessionStateVariableEntry("SolutionName", SolutionName, null));
                iss.Variables.Add(new SessionStateVariableEntry("ConfigurationName", ConfigurationName, null));
                iss.Variables.Add(new SessionStateVariableEntry("PlatformName", PlatformName, null));
                iss.Variables.Add(new SessionStateVariableEntry("ProjectDir", ProjectDir, null));
                iss.Variables.Add(new SessionStateVariableEntry("ProjectName", ProjectName, null));
                iss.Variables.Add(new SessionStateVariableEntry("TargetName", TargetName, null));
                iss.Variables.Add(new SessionStateVariableEntry("OutDir", OutDir, null));
                
                using (var rs = RunspaceFactory.CreateRunspace(iss))
                {
                    rs.Open();

                    using (var ps = PowerShell.Create())
                    {
                        ps.Runspace = rs;

                        ps.AddScript(src);

                        foreach (var e in ps.Invoke())
                        {
                            Log.LogMessage(MessageImportance.High, "{0}", e.BaseObject);
                        }

                        foreach (var e in ps.Streams.Error)
                        {
                            Log.LogError("{0}", e.ToString());
                        }

                        foreach (var e in ps.Streams.Verbose)
                        {
                            Log.LogMessage(MessageImportance.Low, "{1}", e.ToString());
                        }

                        res = !ps.HadErrors;
                    }
                }

                Log.LogMessage(MessageImportance.Low, "Finished PowerShell script: {0} in {1} seconds.", script, (DateTime.Now - start).TotalSeconds);
            }
            else
            {
                Log.LogMessage(MessageImportance.Low, "Skipped empty PowerShell script: {0} in {1} seconds.", script, (DateTime.Now - start).TotalSeconds);
            }

            return res;
        }
    }
}
