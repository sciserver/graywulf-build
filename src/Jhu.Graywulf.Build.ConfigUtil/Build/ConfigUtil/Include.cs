using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Include
    {
        private string path;

        [XmlAttribute("path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }
    }
}
