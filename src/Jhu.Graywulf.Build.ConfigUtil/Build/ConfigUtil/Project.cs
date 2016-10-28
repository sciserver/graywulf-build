using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Project
    {
        private string name;
        private Include[] includes;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlArray("includes")]
        [XmlArrayItem(ElementName = "include")]
        public Include[] Includes
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
