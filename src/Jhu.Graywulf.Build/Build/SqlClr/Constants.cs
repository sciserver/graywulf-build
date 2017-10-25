using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlTypes;

namespace Jhu.Graywulf.Build.SqlClr
{
    static class Constants
    {
        public static readonly StringComparer StringComparer = StringComparer.InvariantCultureIgnoreCase;

        public static readonly HashSet<string> SystemSchemas = new HashSet<string>(StringComparer)
        {
            "dbo", "sys", "guest"
        };

        public static readonly Dictionary<string, string> SqlTypes = new Dictionary<string, string>(StringComparer)
        {
            { typeof(Boolean).FullName, "bit" },
            { typeof(Byte).FullName, "tinyint" },
            { typeof(SByte).FullName, "tinyint" },
            { typeof(Int16).FullName, "smallint" },
            { typeof(UInt16).FullName, "smallint" },
            { typeof(Int32).FullName, "int" },
            { typeof(UInt32).FullName, "int" },
            { typeof(Int64).FullName, "bigint" },
            { typeof(UInt64).FullName, "bitint" },
            { typeof(DateTime).FullName, "datetime" },
            { typeof(Decimal).FullName, "decimal" },
            { typeof(Single).FullName, "real" },
            { typeof(Double).FullName, "float" },
            { typeof(Guid).FullName, "uniqueidentifier" },
            { typeof(Byte[]).FullName, "varbinary(max)" },
            { typeof(String).FullName, "nvarchar(max)" },

            { typeof(SqlBoolean).FullName, "bit" },
            { typeof(SqlByte).FullName, "tinyint" },
            { typeof(SqlInt16).FullName, "smallint" },
            { typeof(SqlInt32).FullName, "int" },
            { typeof(SqlInt64).FullName, "bigint" },
            { typeof(SqlDateTime).FullName, "datetime" },
            { typeof(SqlDecimal).FullName, "decimal" },
            { typeof(SqlMoney).FullName, "decimal" },
            { typeof(SqlSingle).FullName, "real" },
            { typeof(SqlDouble).FullName, "float" },
            { typeof(SqlGuid).FullName, "uniqueidentifier" },
            { typeof(SqlBinary).FullName, "varbinary(max)" },
            { typeof(SqlBytes).FullName, "varbinary(max)" },
            { typeof(SqlString).FullName, "nvarchar(max)" },
            { typeof(SqlChars).FullName, "nvarchar(max)" },
        };

        public static readonly Dictionary<AssemblySecurityLevel, string> AssemblySecurityLevels = new Dictionary<AssemblySecurityLevel, string>()
        {
            { AssemblySecurityLevel.Safe, "SAFE" },
            { AssemblySecurityLevel.External, "EXTERNAL_ACCESS" },
            { AssemblySecurityLevel.Unrestricted, "UNSAFE" },
        };

        //.Net libraries supported by SQL were taken from https://msdn.microsoft.com/en-us/library/ms403279.aspx
        public static readonly HashSet<string> SQLSupportedLibraries = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "CustomMarshalers",
            "Microsoft.VisualBasic",
            "Microsoft.VisualC",
            "System.Configuration",
            "System.Data",
            "System.Data.OracleClient",
            "System.Data.SqlXml",
            "System.Deployment",
            "System.Security",
            "System.Transactions",
            "System.Web.Services",
            "System.Core",
            "System.Xml",
            "System.Xml.Linq",
            "System",
            "System.Numerics",
            "mscorlib"
        };


    }
}
