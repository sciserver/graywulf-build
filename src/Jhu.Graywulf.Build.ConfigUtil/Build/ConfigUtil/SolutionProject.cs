using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class SolutionProject
    {
        private Solution solution;
        private string path;
        private string name;
        private string assemblyName;
        private ProjectType type;
        private Config[] configs;

        [XmlIgnore]
        public Solution Solution
        {
            get { return solution; }
        }

        [XmlIgnore]
        public string Path
        {
            get { return path; }
        }

        [XmlAttribute("name")]
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        [XmlIgnore]
        public string AssemblyName
        {
            get { return assemblyName; }
            set { assemblyName = value; }
        }

        [XmlIgnore]
        public ProjectType Type
        {
            get { return type; }
        }

        [XmlIgnore]
        public Config[] Configs
        {
            get { return configs; }
        }

        public SolutionProject()
        {
            InitializeMembers();
        }

        public SolutionProject(Solution solution)
        {
            InitializeMembers();

            this.solution = solution;
        }

        private void InitializeMembers()
        {
            this.solution = null;
            this.path = null;
            this.name = null;
            this.assemblyName = null;
            this.type = ProjectType.Unknown;
            this.configs = null;
        }

        /// <summary>
        /// Returns the absolute path of the project file
        /// </summary>
        /// <returns></returns>
        private string GetProjectAbsolutePath()
        {
            var path = System.IO.Path.GetDirectoryName(solution.Path);
            path = System.IO.Path.Combine(path, this.path);
            path = System.IO.Path.GetFullPath(path);

            return path;
        }

        /// <summary>
        /// Returns the absolute path of the project base config file
        /// </summary>
        /// <returns></returns>
        private string GetProjectBaseConfigFile()
        {
            var path = GetProjectAbsolutePath();
            path = System.IO.Path.GetDirectoryName(path);
            path = System.IO.Path.Combine(path, Constants.BaseConfig);

            return path;
        }

        /// <summary>
        /// Returns the absolute path of the project config file
        /// </summary>
        /// <returns></returns>
        private string GetProjectConfigFile()
        {
            var file = (type & ProjectType.WebApplication) != 0 ?
                Constants.WebConfig :
                Constants.AppConfig;

            var path = GetProjectAbsolutePath();
            path = System.IO.Path.GetDirectoryName(path);
            path = System.IO.Path.Combine(path, file);

            return path;
        }

        public void LoadProject(string path)
        {
            this.path = path;

            LoadProjectFile();
            FindConfigs();
        }

        private void LoadProjectFile()
        {
            var xml = new XmlDocument();
            xml.Load(GetProjectAbsolutePath());

            var ns = new XmlNamespaceManager(xml.NameTable);
            ns.AddNamespace("msb", "http://schemas.microsoft.com/developer/msbuild/2003");

            // Project name
            this.name = System.IO.Path.GetFileNameWithoutExtension(path);
            this.assemblyName = xml.SelectSingleNode("//msb:AssemblyName", ns).InnerText;

            // Project type
            switch (xml.SelectSingleNode("//msb:OutputType", ns).InnerText.ToLowerInvariant())
            {
                case "exe":
                    this.type = ProjectType.Executable;
                    break;
                case "library":
                    this.type = ProjectType.ClassLibrary;
                    break;
                default:
                    throw new NotImplementedException();
            }

            // Is it a web application or unit test?
            var ptypes = xml.SelectSingleNode("//msb:ProjectTypeGuids", ns);
            if (ptypes != null && !String.IsNullOrWhiteSpace(ptypes.InnerText))
            {
                var parts = ptypes.InnerText.Split(',', ';');

                for (int i = 0; i < parts.Length; i++)
                {
                    var guid = Guid.Parse(parts[i].Trim('{', '}'));

                    if (guid == new Guid("349c5851-65df-11da-9384-00065b846f21"))
                    {
                        this.type |= ProjectType.WebApplication;
                    }
                    else if (guid == new Guid("3AC096D0-A1C2-E12C-1390-A8335801FDAB"))
                    {
                        this.type |= ProjectType.UnitTest;
                    }
                }
            }
        }

        /// <summary>
        /// Ascend in the file hierarcy from the project dir to the
        /// solution dir and collect config files that apply to the current
        /// project
        /// </summary>
        public void FindConfigs()
        {
            var res = new List<Config>();
            var dir = System.IO.Path.GetDirectoryName(GetProjectAbsolutePath());
            var slndir = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(solution.Path)).TrimEnd(System.IO.Path.DirectorySeparatorChar);

            while (true)
            {
                dir = System.IO.Path.GetFullPath(dir).TrimEnd(System.IO.Path.DirectorySeparatorChar);
                var file = System.IO.Path.Combine(dir, Constants.GraywulfConfig);

                if (System.IO.File.Exists(file))
                {
                    var config = Config.LoadConfigFile(file);
                    res.Add(config);
                }

                // If already in solution dir, exit
                if (StringComparer.InvariantCultureIgnoreCase.Compare(dir, slndir) == 0)
                {
                    break;
                }
                else
                {
                    dir = System.IO.Path.Combine(dir, "..");
                }
            }

            this.configs = res.ToArray();
        }

        public void MergeConfigs(Settings settings)
        {
            // Load base config file
            var basexml = new XmlDocument();
            basexml.Load(GetProjectBaseConfigFile());

            for (int c = 0; c < configs.Length; c++)
            {
                var config = configs[c];

                // Merge in all top level includes
                MergeConfigs(settings, basexml, config.Includes);

                // Merge in matching project level includes
                for (int p = 0; p < config.Projects.Length; p++)
                {
                    var project = config.Projects[p];

                    // Are the project names equal??
                    // TODO
                }
            }
        }

        private void MergeConfigs(Settings settings, XmlDocument basexml, Include[] includes)
        {
            for (int i = 0; i < includes.Length; i++)
            {
                var include = includes[i];
                var incxml = include.LoadIncludeFile(settings, this);

                ConfigXmlMerger.Merge(basexml, incxml);
            }
        }
    }
}
