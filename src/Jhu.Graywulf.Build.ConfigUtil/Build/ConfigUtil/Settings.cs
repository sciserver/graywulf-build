using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Settings
    { 
        private bool skipMissingBaseConfig;

        public bool SkipMissingBaseConfig
        {
            get { return skipMissingBaseConfig; }
            set { skipMissingBaseConfig = value; }
        }

        public Settings()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.skipMissingBaseConfig = true;
        }
    }
}
