using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace Jhu.Graywulf
{
    public class UserFunction
    {
        [SqlFunction(Name = "region.Area",
            DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = false)]
        public static SqlDouble Area(SqlBytes bytes)
        {
            return 0;
        }

        [SqlFunction(Name = "dbo.UdtFunction")]
        public static SqlDouble UdtFunction(UserType value)
        {
            return 0;
        }

        private const string ArcTableDefinition =
            @"convexID int, patchID int, arcID int,
			  x float, y float, z float, c float,
              x1 float, y1 float, z1 float, ra1 float, dec1 float,
              x2 float, y2 float, z2 float, ra2 float, dec2 float,
              length float";
        private const string ArcFillMethodName = "FillArc";

        [SqlFunction(Name = "region.GetArcs",
            TableDefinition = ArcTableDefinition, FillRowMethodName = ArcFillMethodName,
            DataAccess = DataAccessKind.None, IsDeterministic = true, IsPrecise = false)]
        public static IEnumerable GetArcs(SqlBytes bytes)
        {
            return null;
        }

        public static void FillArc(
            object obj,
            out SqlInt32 convexID, out SqlInt32 patchID, out SqlInt32 arcID,
            out SqlDouble x, out SqlDouble y, out SqlDouble z, out SqlDouble d,
            out SqlDouble x1, out SqlDouble y1, out SqlDouble z1, out SqlDouble ra1, out SqlDouble dec1,
            out SqlDouble x2, out SqlDouble y2, out SqlDouble z2, out SqlDouble ra2, out SqlDouble dec2,
            out SqlDouble length)
        {
            convexID = 0;
            patchID = 0;
            arcID = 0;

            x = 0;
            y = 0;
            z = 0;
            d = 0;

            x1 = 0;
            y1 = 0;
            z1 = 0;
            ra1 = 0;
            dec1 = 0;

            x2 = 0;
            y2 = 0;
            z2 = 0;
            ra2 = 0;
            dec2 = 0;

            length = 0;
        }
    }
}
