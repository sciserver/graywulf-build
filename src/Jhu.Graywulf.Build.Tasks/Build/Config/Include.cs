using System;
using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.Config
{
    public class Include
    {
        private Config config;
        private string path;

        [XmlIgnore]
        public Config Config
        {
            get { return config; }
            set { config = value; }
        }

        [XmlAttribute("path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public XmlDocument LoadIncludeFile(string path)
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load(path);
                return xml;
            }
            catch (XmlException ex)
            {
                throw Error.XmlFormatError(ex, path);
            }
            catch (Exception ex)
            {
                throw Error.XmlLoadError(ex, path);
            }
        }
    }
}
