using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    [XmlRoot("config")]
    public class Config
    {
        private string root;
        private List<Project> projects;
        private List<Include> includes;

        [XmlAttribute("root")]
        public string Root
        {
            get { return root; }
            set { root = value; }
        }

        [XmlArray("projects")]
        [XmlArrayItem(ElementName = "project")]
        public List<Project> Projects
        {
            get { return projects; }
            set { projects = value; }
        }

        [XmlArray("includes")]
        [XmlArrayItem(ElementName = "include")]
        public List<Include> Includes
        {
            get { return includes; }
            set { includes = value; }
        }

        public Config()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.root = null;
            this.projects = null;
            this.includes = null;
        }

        public static Config LoadConfigFile(string path)
        {
            using (var infile = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                var s = new XmlSerializer(typeof(Config));
                var config = (Config)s.Deserialize(infile);
                return config;
            }
        }
    }
}
