﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace Jhu.Graywulf.Build.SqlClr
{
    public class SqlAssembly
    {
        private Assembly assembly;
        private string name;
        private string path;
        private AssemblySecurityLevel assemblySecurityLevel;
        private Dictionary<string, SqlAssembly> references;

        public string Name
        {
            get { return name; }
        }

        public string Path
        {
            get { return path; }
        }

        public AssemblySecurityLevel AssemblySecurityLevel
        {
            get { return assemblySecurityLevel; }
            set { assemblySecurityLevel = value; }
        }

        public Dictionary<string, SqlAssembly> References
        {
            get { return references; }
        }

        public SqlAssembly()
        {
            InitializeMembers();
        }

        public SqlAssembly(SqlAssembly old)
        {
            CopyMembers(old);
        }

        public SqlAssembly(Assembly assembly, AssemblySecurityLevel sec = AssemblySecurityLevel.Safe)
        {
            InitializeMembers();
            this.assemblySecurityLevel = sec;
            ReflectAssembly(assembly);
        }

        private void InitializeMembers()
        {
            this.assembly = null;
            this.name = null;
            this.path = null;
            this.assemblySecurityLevel = AssemblySecurityLevel.Safe;
            this.references = new Dictionary<string, SqlAssembly>(StringComparer.InvariantCultureIgnoreCase);
        }

        private void CopyMembers(SqlAssembly old)
        {
            this.assembly = old.assembly;
            this.name = old.name;
            this.path = old.path;
            this.assemblySecurityLevel = old.assemblySecurityLevel;
            this.references = new Dictionary<string, SqlAssembly>(old.references);
        }

        internal static Assembly ReflectionOnlyLoadAssembly(AssemblyName name)
        {
            var a = Assembly.ReflectionOnlyLoad(name.ToString());
            ReflectionOnlyPreloadDependencies(a);
            return a;
        }

        internal static Assembly ReflectionOnlyLoadAssembly(string path)
        {
            var a = Assembly.ReflectionOnlyLoadFrom(path);
            ReflectionOnlyPreloadDependencies(a);
            return a;
        }

        private static void ReflectionOnlyPreloadDependencies(Assembly assembly)
        {
            foreach (var assemblyName in assembly.GetReferencedAssemblies())
            {
                ReflectionOnlyPreloadAssembly(assemblyName, assembly.Location);
            }
        }

        internal static void ReflectionOnlyPreloadAssembly(AssemblyName assemblyName, string location)
        {
            try
            {
                Assembly.ReflectionOnlyLoad(assemblyName.FullName);
            }
            catch
            {
                var path = System.IO.Path.GetDirectoryName(location);
                path = System.IO.Path.Combine(path, assemblyName.Name + ".dll");
                Assembly.ReflectionOnlyLoadFrom(path);
            }
        }

        private void ReflectAssembly(Assembly assembly)
        {
            this.assembly = assembly;
            this.name = assembly.GetName().Name;
            this.path = assembly.Location;
            
            CollectReferences();
        }

        public void ScriptCreate(TextWriter writer)
        {
            writer.Write(
@"
IF (SELECT COUNT(*) FROM sys.assemblies WHERE name = '{0}') = 0
CREATE ASSEMBLY [{0}]
    AUTHORIZATION [dbo]
    FROM 0x",
                name);

            using (var infile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var buffer = new byte[0x10000];
                int res;

                while ((res = infile.Read(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < res; i++)
                    {
                        writer.Write(BitConverter.ToString(buffer, i, 1));
                    }
                }
            }

            writer.WriteLine();
            writer.WriteLine("WITH PERMISSION_SET = {0}",
                Constants.AssemblySecurityLevels[assemblySecurityLevel]);

            writer.WriteLine("GO");
            writer.WriteLine();
            writer.WriteLine();
        }

        public void ScriptDrop(TextWriter writer)
        {
            writer.Write(@"
IF (SELECT COUNT(*) FROM sys.assemblies WHERE name = '{0}') > 0
DROP ASSEMBLY [{0}]

GO

",
                name);
        }

        private SqlAssembly[] GetReferencedAssemblies()
        {
            var refs = assembly.GetReferencedAssemblies();
            var assemblies = new List<SqlAssembly>();

            for (int i = 0; i < refs.Length; i++)
            {
                //if (!refs[i].Name.StartsWith("System") && !refs[i].Name.StartsWith("ms"))
                if (!Constants.SQLSupportedLibraries.Contains(refs[i].Name))
                {
                    var a = LoadAssembly(System.IO.Path.GetDirectoryName(this.path), refs[i]);
                    assemblies.Add(new SqlAssembly(a,this.AssemblySecurityLevel));
                }
            }

            return assemblies.ToArray();
        }

        private void CollectReferences()
        {
            foreach (var a in GetReferencedAssemblies())
            {
                a.CollectReferences();

                foreach (var aSub in a.References.Values)
                {
                    if (!references.ContainsKey(aSub.Name))
                    {
                        references.Add(aSub.Name, aSub);
                    }
                }

                if (!references.ContainsKey(a.Name))
                {
                    references.Add(a.Name, a);
                }

            }
        }

        private Assembly LoadAssembly(string dir, AssemblyName name)
        {
            Assembly a = null;

            // Attempt default location
            try
            {
                a = ReflectionOnlyLoadAssembly(name);
            }
            catch { }

            if (a == null)
            {
                // Try from the directory of referencing
                try
                {
                    a = ReflectionOnlyLoadAssembly(System.IO.Path.Combine(dir, name.Name + ".dll"));
                }
                catch { }
            }

            return a;
        }
    }
}
