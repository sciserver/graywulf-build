using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Jhu.Graywulf.Build.Config
{
    [XmlRoot(Namespace = Constants.XmlNamespace, ElementName = "config")]
    public class Config : ConfigObjectBase
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

        public static Config LoadFile(Task task, string path)
        {
            Config config;
            XmlSchemaSet xsd;

            using (var xsdr = XmlReader.Create(new StringReader(Schemas.Config)))
            {
                xsd = new XmlSchemaSet();
                xsd.Add(Constants.XmlNamespace, xsdr);
            }

            using (var infile = System.IO.File.Open(path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
            {
                try
                {
                    var s = new XmlSerializer(typeof(Config));
                    var settings = new XmlReaderSettings()
                    {
                        Schemas = xsd,
                        ValidationType = ValidationType.Schema
                    };

                    using (var xr = XmlReader.Create(infile, settings))
                    {
                        config = (Config)s.Deserialize(xr);
                    }

                    config.path = path;

                    foreach (var include in config.includes)
                    {
                        include.Config = config;
                    }

                    return config;
                }
                catch (Exception ex)
                {
                    LogMessage(task, MessageImportance.High, "Error parsing or validating config file {0}.", path);

                    Exception ee = ex;

                    while (ee != null)
                    {
                        LogMessage(task, MessageImportance.High, ee.Message);
                        ee = ee.InnerException;
                    }

                    throw ex;
                }
            }
        }

        public override string ToString()
        {
            return path;
        }
    }
}
