using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Include
    {
        private string path;
        private bool local;

        [XmlAttribute("path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        [XmlAttribute("local")]
        public bool Local
        {
            get { return local; }
            set { local = value; }
        }

        private string GetAbsolutePath(Settings settings, SolutionProject project)
        {
            string path = null;

            if (local)
            {
                path = System.IO.Path.GetDirectoryName(project.Path);
                path = System.IO.Path.Combine(path, this.path);
            }
            else
            {
                path = System.IO.Path.Combine(settings.ConfigRoot, this.path);
            }

            path = System.IO.Path.GetFullPath(path);
            return path;
        }

        public XmlDocument LoadIncludeFile(Settings settings, SolutionProject project)
        {
            var path = GetAbsolutePath(settings, project);
            var xml = new XmlDocument();
            xml.Load(GetAbsolutePath());

            return xml;
        }
    }
}
