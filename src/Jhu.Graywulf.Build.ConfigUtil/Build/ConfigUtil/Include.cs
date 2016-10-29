using System;
using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Include
    {
        private string path;
        private bool global;

        [XmlAttribute("path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        [XmlAttribute("global")]
        public bool Global
        {
            get { return global; }
            set { global = value; }
        }

        private string GetAbsolutePath(Settings settings, SolutionProject project)
        {
            string path = null;

            if (global)
            {
                path = System.IO.Path.Combine(settings.ConfigRoot, this.path);
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(project.GetProjectAbsolutePath());
                path = System.IO.Path.Combine(path, this.path);
            }

            path = System.IO.Path.GetFullPath(path);
            return path;
        }

        public XmlDocument LoadIncludeFile(Settings settings, SolutionProject project)
        {
            var path = GetAbsolutePath(settings, project);

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
