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
    [SqlUserDefinedType(Format.Native, IsFixedLength = true, Name = "dbo.UserType")]
    public struct UserType : INullable
    {
        private bool _null;
        private double value;

        public bool IsNull
        {
            get { return _null; }
        }


        public double Value
        {
            get { return value; }
            set { this.value = value; }
        }
        
        #region Constructors and initializers

        public UserType(double value)
        {
            this._null = false;
            this.value = value;
        }

        public static UserType Null
        {
            get
            {
                var h = new UserType();
                h._null = true;
                return h;
            }
        }

        #endregion

        public static UserType Parse(SqlString s)
        {
            if (s.IsNull)
            {
                return Null;
            }

            return new UserType(double.Parse(s.Value));
        }

        public override string ToString()
        {
            return value.ToString();
        }
    }
}
