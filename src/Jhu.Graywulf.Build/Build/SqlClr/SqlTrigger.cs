using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf.Build.SqlClr
{
    class SqlTrigger : SqlObject
    {
        public override SqlObjectRank Rank
        {
            get { return SqlObjectRank.Trigger; }
        }

        public SqlTrigger()
        {
            InitializeMembers();
        }

        public SqlTrigger(SqlObject old)
            : base(old)
        {
            InitializeMembers();
        }

        public SqlTrigger(SqlTrigger old)
            :base(old)
        {
            CopyMembers(old);
        }

        public SqlTrigger(MethodInfo method)
            : base(method)
        {
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(SqlTrigger old)
        {
        }

        protected override void ReflectAttributes(Type type)
        {
            throw new NotImplementedException();
        }

        public override void ScriptCreate(SqlClrReflector r, TextWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void ScriptDrop(SqlClrReflector r, TextWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
