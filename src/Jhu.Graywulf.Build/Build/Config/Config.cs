using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.Config
{
    [XmlRoot("config")]
    public class Config
    {
        private string path;
        private string root;
        private List<Include> includes;
        private AssemblySettings assemblySettings;

        [XmlIgnore]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        [XmlAttribute("root")]
        public string Root
        {
            get { return root; }
            set { root = value; }
        }

        [XmlArray("includes")]
        [XmlArrayItem(ElementName = "include")]
        public List<Include> Includes
        {
            get { return includes; }
            set { includes = value; }
        }

        [XmlElement("assemblySettings")]
        public AssemblySettings AssemblySettings
        {
            get { return assemblySettings; }
            set { assemblySettings = value; }
        }

        public Config()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.path = null;
            this.root = null;
            this.includes = null;
            this.assemblySettings = null;
        }

        public static Config LoadConfigFile(string path)
        {
            using (var infile = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                var s = new XmlSerializer(typeof(Config));
                var config = (Config)s.Deserialize(infile);

                config.path = path;

                foreach (var include in config.includes)
                {
                    include.Config = config;
                }

                return config;
            }
        }

        public override string ToString()
        {
            return path;
        }
    }
}
