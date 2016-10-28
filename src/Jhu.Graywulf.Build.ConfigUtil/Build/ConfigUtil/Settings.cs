using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Settings
    {
        private string configRoot;

        public string ConfigRoot
        {
            get { return configRoot; }
            set { configRoot = value; }
        }
    }
}
