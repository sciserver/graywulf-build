using System;
using System.Xml;

namespace Jhu.Graywulf.Build.Config
{
    public static class Error
    {
        public static ConfigException MissingBaseConfig(SolutionProject project)
        {
            var msg = String.Format(ErrorMessages.MissingBaseConfig, project.Name);
            return new ConfigException(msg);
        }

        public static ConfigException MissingInclude(SolutionProject project, string configpath, string includepath)
        {
            var msg = String.Format(ErrorMessages.MissingInclude, project, configpath, includepath);
            return new ConfigException(msg);
        }

        public static ConfigException IncompatibleRootNodes(XmlElement node1, XmlElement node2)
        {
            var msg = String.Format(ErrorMessages.IncompatibleRootNodes, node1.Name, node2.Name);
            return new ConfigException(msg);
        }

        public static ConfigException XmlLoadError(Exception ex, string path)
        {
            var msg = String.Format(ErrorMessages.XmlLoadError, path);
            return new ConfigException(msg, ex);
        }

        public static ConfigException XmlFormatError(XmlException ex, string path)
        {
            var msg = String.Format(ErrorMessages.XmlFormatError, path);
            return new ConfigException(msg, ex);
        }

        public static ConfigException UnknownProjectType(string projectType)
        {
            var msg = String.Format(ErrorMessages.UnknownProjectType, projectType);
            return new ConfigException(msg);
        }
    }
}
