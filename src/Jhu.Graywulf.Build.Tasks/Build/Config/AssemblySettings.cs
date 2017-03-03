using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.Build.Config
{
    [XmlRoot("assembly")]
    public class AssemblySettings
    {
        private const string RegexStart = @"(\[\s*assembly\s*:\s*";
        private const string RegexEnd = @"\s*\()([^\)]*)(\)\s*\])";
        private static readonly Regex VersionRegex1 = new Regex(RegexStart + "AssemblyVersion" + RegexEnd);
        private static readonly Regex VersionRegex2 = new Regex(RegexStart + "AssemblyFileVersion" + RegexEnd);

        private string prefix;
        private string title;
        private string description;
        private string configuration;
        private string company;
        private string product;
        private string copyright;
        private string trademark;
        private string culture;
        private string version;
        private string fileVersion;
        private List<string> internalsVisibleTo;

        [XmlAttribute("prefix")]
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        [XmlAttribute("title")]
        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        [XmlAttribute("description")]
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        [XmlAttribute("configuration")]
        public string Configuration
        {
            get { return configuration; }
            set { configuration = value; }
        }

        [XmlAttribute("company")]
        public string Company
        {
            get { return company; }
            set { company = value; }
        }

        [XmlAttribute("product")]
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        [XmlAttribute("copyright")]
        public string Copyright
        {
            get { return copyright; }
            set { copyright = value; }
        }

        [XmlAttribute("trademark")]
        public string Trademark
        {
            get { return trademark; }
            set { trademark = value; }
        }

        [XmlAttribute("culture")]
        public string Culture
        {
            get { return culture; }
            set { culture = value; }
        }

        [XmlAttribute("version")]
        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        [XmlAttribute("fileVersion")]
        public string FileVersion
        {
            get { return fileVersion; }
            set { fileVersion = value; }
        }

        [XmlArray("internals")]
        [XmlArrayItem("visibleTo")]
        public List<string> InternalsVisibleTo
        {
            get { return internalsVisibleTo; }
            set { internalsVisibleTo = value; }
        }

        public AssemblySettings()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.prefix = null;
            this.title = null;
            this.description = null;
            this.configuration = null;
            this.company = null;
            this.product = null;
            this.copyright = null;
            this.trademark = null;
            this.culture = null;
            this.version = null;
            this.fileVersion = null;
            this.internalsVisibleTo = null;
        }

        public void Merge(AssemblySettings other)
        {
            this.prefix = other.prefix ?? this.prefix;
            this.title = other.title ?? this.title;
            this.description = other.description ?? this.description;
            this.configuration = other.configuration ?? this.configuration;
            this.company = other.company ?? this.company;
            this.product = other.product ?? this.product;
            this.copyright = other.copyright ?? this.copyright;
            this.trademark = other.trademark ?? this.trademark;
            this.culture = other.culture ?? this.culture;
            this.version = other.version ?? this.version;
            this.fileVersion = other.fileVersion ?? this.fileVersion;

            if (other.internalsVisibleTo != null)
            {
                if (this.internalsVisibleTo == null)
                {
                    this.internalsVisibleTo = new List<string>(other.internalsVisibleTo);
                }
                else
                {
                    this.internalsVisibleTo = new List<string>(other.internalsVisibleTo.Concat(this.internalsVisibleTo));
                }
            }
        }

        public void WriteAssemblyInfoFile(StreamWriter outfile, SolutionProject project)
        {
            outfile.WriteLine("using System.Reflection;");
            outfile.WriteLine("using System.Runtime.CompilerServices;");
            outfile.WriteLine("using System.Runtime.InteropServices;");
            outfile.WriteLine();

            WriteAssemblyAttribute(outfile, "AssemblyTitle", title);
            WriteAssemblyAttribute(outfile, "AssemblyDescription", description);
            WriteAssemblyAttribute(outfile, "AssemblyConfiguration", configuration);
            WriteAssemblyAttribute(outfile, "AssemblyCompany", company);
            WriteAssemblyAttribute(outfile, "AssemblyProduct", product);
            WriteAssemblyAttribute(outfile, "AssemblyCopyright", copyright);
            WriteAssemblyAttribute(outfile, "AssemblyTrademark", trademark);
            WriteAssemblyAttribute(outfile, "AssemblyCulture", culture);
            WriteAssemblyAttribute(outfile, "AssemblyVersion", version);
            WriteAssemblyAttribute(outfile, "AssemblyFileVersion", fileVersion);
            outfile.WriteLine();

            if (internalsVisibleTo != null)
            {
                foreach (var visibleto in internalsVisibleTo)
                {
                    WriteAssemblyAttribute(outfile, "InternalsVisibleTo", visibleto);
                }
            }
        }

        private void WriteAssemblyAttribute(StreamWriter outfile, string attribute, string value)
        {
            if (value != null)
            {
                outfile.WriteLine("[assembly: {0}(\"{1}\")]", attribute, value);
            }
        }

        public void UpdateAssemblyInfoFileVersion(string path)
        {
            var code = System.IO.File.ReadAllText(path);
            code = VersionRegex1.Replace(code, ReplaceVersion);
            code = VersionRegex2.Replace(code, ReplaceVersion);
            System.IO.File.WriteAllText(path, code);
        }

        public string ReplaceVersion(Match m)
        {
            return m.Groups[1] + "\"" + this.version + "\"" + m.Groups[3];
        }
    }
}
