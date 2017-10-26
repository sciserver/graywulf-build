using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Build.SqlClr
{
    class SqlProcedure : SqlObject
    {
        public override SqlObjectRank Rank
        {
            get { return SqlObjectRank.Procedure; }
        }

        public SqlProcedure()
        {
            InitializeMembers();
        }

        public SqlProcedure(SqlObject old)
            : base(old)
        {
            InitializeMembers();
        }

        public SqlProcedure(SqlProcedure old)
            : base(old)
        {
            CopyMembers(old);
        }

        public SqlProcedure(MethodInfo method)
            :base(method)
        {
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(SqlProcedure old)
        {
        }

        protected override void ReflectMethod(MethodInfo method)
        {
            base.ReflectMethod(method);
            ReflectReturnType(method);
        }

        protected override void ReflectAttributes(MethodInfo method)
        {
            base.ReflectAttributes(method);

            var att = SqlClrReflector.GetAttribute(method, typeof(SqlProcedureAttribute).FullName);

            ReflectObjectName((string)SqlClrReflector.GetAttributeArgument(att, "Name"));
            ReflectParameters(method);
        }

        public override void ScriptCreate(SqlClrReflector r, TextWriter writer)
        {
            ScriptDrop(r, writer);

            writer.Write(@"
CREATE PROCEDURE [{0}].[{1}]
({2})
AS
 EXTERNAL NAME [{3}].[{4}].[{5}]

GO

",
                Schema,
                Name,
                GetParametersSql(r),
                AssemblyName,
                ClassName,
                MethodName);
        }

        public override void ScriptDrop(SqlClrReflector r, TextWriter writer)
        {
            writer.Write(@"
IF (OBJECT_ID('{0}.{1}') IS NOT NULL)
BEGIN
    DROP PROCEDURE [{0}].[{1}]
END

GO

",
                Schema,
                Name);
        }
    }
}
