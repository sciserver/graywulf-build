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

                var apath = Path.Combine(projectDir, outputPath, assemblyName) + extension;
                var a = Assembly.LoadFrom(apath);
                var sec = AssemblySecurityLevel.Safe;

                if (!String.IsNullOrWhiteSpace(securityLevel))
                {
                    Enum.TryParse<AssemblySecurityLevel>(securityLevel, out sec);
                }

                var r = new SqlClrReflector(a, sec);
                var dir = Path.GetDirectoryName(a.Location);
                var name = Path.GetFileNameWithoutExtension(a.Location);
                var path = Path.Combine(dir, name);
                var crpath = path + ".Create.sql";
                var drpath = path + ".Drop.sql";

                using (var outfile = new StreamWriter(crpath))
                {
                    r.ScriptCreate(outfile);
                }

                using (var outfile = new StreamWriter(drpath))
                {
                    r.ScriptDrop(outfile);
                }

                res = true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message);
                res = false;
            }

            Log.LogMessage(MessageImportance.Low, "Finished Graywulf SqlClr task.");

            return res;
        }
    }
}
