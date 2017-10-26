using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf
{
    [Serializable]
    public class UserProcedure
    {
        [SqlProcedure(Name = "dbo.ClrProdecure")]
        public static SqlInt32 ClrProcedure(SqlInt32 input)
        {
            return input;
        }
    }
}
