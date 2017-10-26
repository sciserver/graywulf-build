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
    [SqlUserDefinedAggregate(Format.Native, Name = "point.AvgXyz")]
    public struct UserAggreagate
    {
        double value;
        long count;

        public void Init()
        {
            value = 0;
            count = 0;
        }

        public void Accumulate(SqlDouble value)
        {
            this.value += value.Value;
            count++;
        }
        
        public void Merge(UserAggreagate group)
        {
            this.value += group.value;
            this.count += group.count;
        }

        public SqlDouble Terminate()
        {
            if (count > 0)
            {
                return new SqlDouble(value / count);
            }
            else
            {
                return SqlDouble.Null;
            }
        }

    }
}
