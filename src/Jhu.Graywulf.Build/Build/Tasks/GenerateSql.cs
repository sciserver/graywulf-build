using System;
using System.Reflection;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using Jhu.Graywulf.Build.SqlClr;

namespace Jhu.Graywulf.Build.Tasks
{
    public class GenerateSql : Task
    {
        private string projectDir;
        private string projectName;
        private string assemblyName;
        private string outputType;
        private string outputPath;
        private string securityLevel;

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

        [Required]
        public string OutputPath
        {
            get { return outputPath; }
            set { outputPath = value; }
        }

        public string SecurityLevel
        {
            get { return securityLevel; }
            set { securityLevel = value; }
        }

        public override bool Execute()
        {
            bool res;
            var start = DateTime.Now;
            Log.LogMessage(MessageImportance.Low, "Starting Graywulf SqlClr task.");

            try
            {
                string extension;

                switch (outputType.ToLowerInvariant())
                {
                    case "exe":
                    case "winexe":
                        extension = ".exe";
                        break;
                    case "library":
                        extension = ".dll";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                var assemblyPath = Path.Combine(projectDir, outputPath, assemblyName) + extension;
                var sec = AssemblySecurityLevel.Safe;
                var dir = Path.GetDirectoryName(assemblyPath);
                var name = Path.GetFileNameWithoutExtension(assemblyPath);
                var path = Path.Combine(dir, name);
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

                proxy.ReflectAssembly(assemblyPath, sec);
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
