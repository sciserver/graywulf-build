using System;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class Project
    {
        private Solution solution;
        private string path;
        private string name;
        private string assemblyName;
        private ProjectType type;
        private Include[] includes;
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

        [XmlArray("includes")]
        [XmlArrayItem(ElementName = "include")]
        public Include[] Includes
        {
            get { return includes; }
            set { includes = value; }
        }

        [XmlIgnore]
        public Config[] Configs
        {
            get { return configs; }
        }

        public Project(Solution solution)
        {
            this.solution = solution;
        }

        private void InitializeMembers()
        {
            this.solution = null;
            this.path = null;
            this.name = null;
            this.assemblyName = null;
            this.type = ProjectType.Unknown;
            this.includes = null;
            this.configs = null;
        }

        private string GetAbsolutePath()
        {
            var path = System.IO.Path.GetDirectoryName(solution.Path);
            path = System.IO.Path.Combine(path, this.path);
            path = System.IO.Path.GetFullPath(path);

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
            xml.Load(GetAbsolutePath());

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
            var dir = System.IO.Path.GetDirectoryName(GetAbsolutePath());
            var slndir = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(solution.Path)).TrimEnd(System.IO.Path.DirectorySeparatorChar);

            while (true)
            {
                dir = System.IO.Path.GetFullPath(dir).TrimEnd(System.IO.Path.DirectorySeparatorChar);
                var file = System.IO.Path.Combine(dir, Constants.ConfigFileName);

                if (System.IO.File.Exists(file))
                {
                    using (var infile = System.IO.File.Open(file, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite))
                    {
                        var s = new XmlSerializer(typeof(Config));
                        var config = (Config)s.Deserialize(infile);
                        res.Add(config);
                    }
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
    }
}
