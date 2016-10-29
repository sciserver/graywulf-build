using System;
using System.Xml;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public static class Error
    {
        public static ConfigException MissingBaseConfig(SolutionProject project)
        {
            var msg = String.Format(ErrorMessages.MissingBaseConfig, project.Name);
            return new ConfigException(msg);
        }

        public static ConfigException IncompatibleRootNodes(XmlElement node1, XmlElement node2)
        {
            var msg = String.Format(ErrorMessages.IncompatibleRootNodes, node1.Name, node2.Name);
            return new ConfigException(msg);
        }
    }
}
