using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf
{
    public class Sql
    {
        [SqlFunction(Name = "region.Area",
            DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = false)]
        public static SqlDouble Area(SqlBytes bytes)
        {
            return 0;
        }
    }
}
