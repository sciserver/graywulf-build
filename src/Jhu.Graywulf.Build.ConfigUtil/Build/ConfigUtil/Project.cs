using System.Collections.Generic;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Project
    {
        private string name;
        private List<Include> includes;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlArray("includes")]
        [XmlArrayItem(ElementName = "include")]
        public List<Include> Includes
        {
            get { return includes; }
            set { includes = value; }
        }

        public Project()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.name = null;
            this.includes = null;
        }
    }
}
