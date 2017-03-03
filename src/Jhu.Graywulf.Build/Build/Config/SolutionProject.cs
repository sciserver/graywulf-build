using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Jhu.Graywulf.Build.Config
{
    public class SolutionProject
    {
        private Solution solution;
        private string path;
        private string name;
        private string assemblyName;
        private ProjectType type;
        private List<Config> configs;

        [XmlIgnore]
        public Solution Solution
        {
            get { return solution; }
        }

        [XmlIgnore]
        public string Path
        {
            get { return path; }
            set { path = value; }
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
        public List<Config> Configs
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
        public string GetProjectAbsolutePath()
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
        private string GetBaseConfigFile()
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

        private string GetAssemblyInfoFile()
        {
            var path = GetProjectAbsolutePath();
            path = System.IO.Path.GetDirectoryName(path);
            path = System.IO.Path.Combine(path, Constants.PropertiesFolder, Constants.AssemblyInfo);

            return path;
        }

        /// <summary>
        /// Loads details from the csproj file
        /// </summary>
        /// <param name="path"></param>
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
            var projectType = xml.SelectSingleNode("//msb:OutputType", ns).InnerText;
            // Is it a web application or unit test?
            var ptypes = xml.SelectSingleNode("//msb:ProjectTypeGuids", ns);

            SetProjectType(projectType, ptypes == null ? null : ptypes.InnerText);
        }

        public void SetProjectType(string outputType, string projectTypeGuids)
        {
            switch (outputType.ToLowerInvariant())
            {
                case "exe":
                case "winexe":
                    this.type = ProjectType.Executable;
                    break;
                case "library":
                    this.type = ProjectType.ClassLibrary;
                    break;
                default:
                    throw Error.UnknownProjectType(outputType);
            }

            if (!String.IsNullOrWhiteSpace(projectTypeGuids))
            {
                var parts = projectTypeGuids.Split(',', ';');

                for (int i = 0; i < parts.Length; i++)
                {
                    var guid = Guid.Parse(parts[i].Trim('{', '}'));

                    if (guid == Constants.WebApplicationGuid)
                    {
                        this.type |= ProjectType.WebApplication;
                    }
                    else if (guid == Constants.UnitTestGuid)
                    {
                        this.type |= ProjectType.UnitTest;
                    }
                }
            }
        }

        public void GenerateAssemblyInfoFile()
        {
            if (configs.Count == 0)
            {
                return;
            }

            var settings = GetAssemblySettings();
            var path = GetAssemblyInfoFile();
            var dir = System.IO.Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            var gitinfo = GetGitCommitInfo(settings.Prefix);

            if (settings.Title == null)
            {
                settings.Title = this.Name;
            }

            settings.Title += " (" + gitinfo.Hash + ")";
            settings.Version = gitinfo.Version + "." + gitinfo.Revision.ToString();

            using (var outfile = new StreamWriter(path))
            {
                settings.WriteAssemblyInfoFile(outfile, this);
            }
        }

        public GitCommitInfo GetGitCommitInfo(string prefix)
        {
            var repodir = System.IO.Path.GetDirectoryName(GetProjectAbsolutePath());
            var gitinfo = new GitCommitInfo();
            gitinfo.Load(repodir, prefix);

            return gitinfo;
        }

        private AssemblySettings GetAssemblySettings()
        {
            var settings = new AssemblySettings();

            foreach (var config in configs)
            {
                if (config.AssemblySettings != null)
                {
                    settings.Merge(config.AssemblySettings);
                }
            }

            return settings;
        }

        public void UpdateAssemblyInfoFileVersion()
        {
            if (configs.Count == 0)
            {
                return;
            }

            var settings = GetAssemblySettings();

            Console.WriteLine("Updating project version number to '{0}'.", settings.Version);

            var path = GetAssemblyInfoFile();
            settings.UpdateAssemblyInfoFileVersion(path);
        }
      

        /// <summary>
        /// Ascend in the file hierarcy from the project dir to the
        /// solution dir and collect config files that apply to the current
        /// project
        /// </summary>
        public void FindConfigs()
        {
            var dir = System.IO.Path.GetDirectoryName(GetProjectAbsolutePath());
            var slndir = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(solution.Path)).TrimEnd(System.IO.Path.DirectorySeparatorChar);

            configs = new List<Config>();

            while (true)
            {
                dir = System.IO.Path.GetFullPath(dir).TrimEnd(System.IO.Path.DirectorySeparatorChar);
                var file = System.IO.Path.Combine(dir, Constants.GraywulfConfig);

                if (System.IO.File.Exists(file))
                {
                    var config = Config.LoadConfigFile(file);
                    configs.Add(config);
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

            // Reverse, so that configs will be applied top-down
            configs.Reverse();
        }

        public void MergeConfigs(Settings settings)
        {
            // Load base config file
            var path = GetBaseConfigFile();

            if (!System.IO.File.Exists(path))
            {
                if (settings.SkipMissingBaseConfig)
                {
                    return;
                }
                else
                {
                    throw Error.MissingBaseConfig(this);
                }
            }

            var basexml = new XmlDocument();
            basexml.Load(path);

            if (configs.Count == 0)
            {
                return;
            }

            string root = null;

            foreach(var config in configs)
            {
                root = GetRootPath(root, config);

                // Update assembly info with version number

                // Merge in all includes, if any
                Console.WriteLine("Including files from config file '{0}'.", config.Path);

                if (config.Includes != null)
                {
                    foreach (var include in config.Includes)
                    {
                        var incpath = GetIncludePath(root, include);
                        var incxml = include.LoadIncludeFile(incpath);
                        ConfigXmlMerger.Merge(basexml, incxml);

                        Console.WriteLine("Merged configuration from '{0}'.", incpath);
                    }
                }
            }

            // Write results
            path = GetProjectConfigFile();
            basexml.Save(path);
        }

        private string GetRootPath(string root, Config config)
        {
            if (config.Root != null)
            {
                root = System.IO.Path.GetDirectoryName(config.Path);
                root = System.IO.Path.Combine(root, config.Root);
            }

            return root;
        }

        private string GetIncludePath(string root, Include include)
        {
            string path = null;

            if (include.Path.StartsWith("/") || include.Path.StartsWith("\\"))
            {
                path = System.IO.Path.Combine(root, include.Path.Substring(1));
            }
            else
            {
                path = System.IO.Path.GetDirectoryName(include.Config.Path);
                path = System.IO.Path.Combine(path, include.Path);
            }

            path = System.IO.Path.GetFullPath(path);
            return path;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
