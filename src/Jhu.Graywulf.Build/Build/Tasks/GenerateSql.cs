using System;
using System.Reflection;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Jhu.Graywulf.Build.SqlClr;

namespace Jhu.Graywulf.Build.Tasks
{
    public class GenerateSql : TaskBase
    {
        #region Private member variables

        private string securityLevel;

        #endregion
        #region Properties

        public string SecurityLevel
        {
            get { return securityLevel; }
            set { securityLevel = value; }
        }

        #endregion
        #region Constructors and initializers

        public GenerateSql()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.SecurityLevel = AssemblySecurityLevel.Safe.ToString();
        }

        #endregion

        public override bool Execute()
        {
            bool res;
            var start = DateTime.Now;
            Log.LogMessage(MessageImportance.Low, "Starting Graywulf SqlClr task.");

            try
            {
                var sec = AssemblySecurityLevel.Safe;
                var targetPath = GetTargetPath();
                var path = GetTargetPathWithoutExtension();
                var crpath = path + ".Create.sql";
                var drpath = path + ".Drop.sql";

                if (!String.IsNullOrWhiteSpace(securityLevel))
                {
                    Enum.TryParse<AssemblySecurityLevel>(securityLevel, out sec);
                }

                // Load reflector into new app domain to prevent locking of dll
                // by the MSBuild process
                var buildpath = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
                Log.LogMessage(MessageImportance.Low, "Running build in {0}", buildpath);

                var adinfo = new AppDomainSetup()
                {
                    ApplicationBase = buildpath
                };
                var ad = AppDomain.CreateDomain("SqlClrReflector", null, adinfo);
                var proxy = (SqlClrReflector)ad.CreateInstanceAndUnwrap(typeof(SqlClrReflector).Assembly.FullName, typeof(SqlClrReflector).FullName);

                proxy.ReflectAssembly(targetPath, sec);
                proxy.ScriptCreate(crpath);
                proxy.ScriptDrop(drpath);
                
                proxy.Dispose();
                AppDomain.Unload(ad);

                res = true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                Log.LogError(ex.StackTrace);
                res = false;
            }

            Log.LogMessage(MessageImportance.Low, "Finished Graywulf SqlClr task.");

            return res;
        }
    }
}
