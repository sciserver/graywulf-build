using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace Jhu.Graywulf.Build.SqlClr
{
    internal class SqlClrReflector : MarshalByRefObject, IDisposable
    {
        private SqlAssembly assembly;
        private HashSet<string> schemas;
        private List<SqlObject> objects;
        private Dictionary<string, string> types;

        public HashSet<string> Schames
        {
            get { return schemas; }
        }

        public List<SqlObject> Objects
        {
            get { return objects; }
        }

        public Dictionary<string, string> Types
        {
            get { return types; }
        }

        public SqlClrReflector()
        {
            InitializeMembers();

            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += AppDomain_ReflectionOnlyAssemblyResolve;
        }

        private Assembly AppDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            return SqlAssembly.ReflectionOnlyLoadAssembly(assemblyName);
        }

        public SqlClrReflector(string assemblyPath, AssemblySecurityLevel sec)
            : this()
        {
            ReflectAssembly(assemblyPath, sec);
        }

        public SqlClrReflector(Assembly assembly, AssemblySecurityLevel sec)
            : this()
        {
            CollectObjects(assembly, sec);
        }

        private void InitializeMembers()
        {
            this.objects = null;
            this.schemas = null;
            this.types = null;
        }

        public void Dispose()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve -= AppDomain_ReflectionOnlyAssemblyResolve;
        }

        public void ReflectAssembly(string assemblyPath, AssemblySecurityLevel sec)
        {
            var a = SqlAssembly.ReflectionOnlyLoadAssembly(assemblyPath);
            CollectObjects(a, sec);
            CollectSchemaNames();
        }

        private void CollectObjects(Assembly a, AssemblySecurityLevel sec)
        {
            assembly = new SqlAssembly(a, sec);
            objects = new List<SqlClr.SqlObject>();
            types = new Dictionary<string, string>(Constants.SqlTypes);

            foreach (var type in a.GetTypes())
            {
                var obj = SqlObject.FromType(type);

                if (obj != null)
                {
                    objects.Add(obj);

                    if (obj is SqlUserDefinedType)
                    {
                        types.Add(type.FullName, ((SqlUserDefinedType)obj).GetSql());
                    }
                }
                else
                {
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
                    {
                        obj = SqlObject.FromMethod(method);
                        if (obj != null)
                        {
                            objects.Add(obj);
                        }
                    }
                }
            }
        }

        private void CollectSchemaNames()
        {
            schemas = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            foreach (var obj in objects)
            {
                var schema = obj.Schema;

                if (!String.IsNullOrEmpty(schema) &&
                    !Constants.SystemSchemas.Contains(schema) &&
                    !schemas.Contains(obj.Schema))
                {
                    schemas.Add(obj.Schema);
                }
            }
        }

        public void ScriptCreate(string path)
        {
            using (var outfile = new StreamWriter(path))
            {
                ScriptCreate(outfile);
            }
        }

        public void ScriptCreate(TextWriter writer)
        {
            foreach (var schema in schemas)
            {
                ScriptCreateSchema(writer, schema);
            }

            foreach (var a in assembly.References.Values)
            {
                a.ScriptCreate(writer, true);
            }

            assembly.ScriptCreate(writer, false);

            foreach (var obj in objects.OrderBy(i => i.Rank))
            {
                obj.ScriptCreate(this, writer);
            }
        }

        public void ScriptDrop(string path)
        {
            using (var outfile = new StreamWriter(path))
            {
                ScriptDrop(outfile);
            }
        }

        public void ScriptDrop(TextWriter writer)
        {
            foreach (var obj in objects.OrderByDescending(i => i.Rank))
            {
                obj.ScriptDrop(this, writer);
            }

            assembly.ScriptDrop(writer, false);

            foreach (var a in assembly.References.Values.Reverse())
            {
                a.ScriptDrop(writer, true);
            }

            foreach (var schema in schemas)
            {
                ScriptDropSchema(writer, schema);
            }
        }

        private void ScriptCreateSchema(TextWriter writer, string schema)
        {
            if (!Constants.SystemSchemas.Contains(UnquoteIdentifier(schema)))
            {
                writer.Write(
@"
IF SCHEMA_ID('{0}') IS NULL
EXEC('CREATE SCHEMA [{0}]')

GO

",
                    schema);
            }
        }

        private void ScriptDropSchema(TextWriter writer, string schema)
        {
            if (!Constants.SystemSchemas.Contains(UnquoteIdentifier(schema)))
            {
                writer.Write(
@"
IF SCHEMA_ID('{0}') IS NOT NULL
EXEC('DROP SCHEMA [{0}]')

GO

",
                   schema);
            }
        }

        protected string UnquoteIdentifier(string identifier)
        {
            return identifier.Trim('[', ']', '"');
        }

        internal static CustomAttributeData GetAttribute(MemberInfo member, string attributeName)
        {
            var atts = member.GetCustomAttributesData();
            var att = atts.Where(a => a.AttributeType.FullName == attributeName).FirstOrDefault();
            return att;
        }

        internal static object GetAttributeArgument(CustomAttributeData att, string name)
        {
            var arg = att.NamedArguments.Where(a => a.MemberName == name).FirstOrDefault();
            return arg == null ? null : arg.TypedValue.Value;
        }
    }
}
