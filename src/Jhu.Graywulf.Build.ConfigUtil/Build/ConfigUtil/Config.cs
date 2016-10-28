using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    [XmlRoot("config")]
    public class Config
    {
        private string root;

        [XmlAttribute("root")]
        public string Root
        {
            get { return root; }
            set { root = value; }
        }
    }
}
