using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    public class GitCommitInfo
    {
        private static readonly Regex gitDescribeRegex = new Regex(@"^(?<tag>[a-z]+)-v(?<version>[0-9]+(?:\.[0-9]+)*)-(?<revision>[0-9]+)-g[0-9a-f]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

        private string hash;
        private string tag;
        private string version;
        private string revision;

        public string Hash
        {
            get { return hash; }
            set { hash = value; }
        }

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        public string Version
        {
            get { return version; }
            set { version = value; }
        }

        public string Revision
        {
            get { return revision; }
            set { revision = value; }
        }

        public GitCommitInfo()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.hash = null;
            this.tag = null;
            this.version = null;
            this.revision = null;
        }

        public void Load(string path, string prefix)
        {
            hash = RunGit(path, "log --pretty=format:%H -n 1");

            // response format: skyquery-v1.2.3-0-gfd4a105
            var res = RunGit(path, "describe --long --match " + prefix + "-v*");
            var m = gitDescribeRegex.Match(res);

            tag = m.Groups["tag"].Value;
            version = m.Groups["version"].Value;
            revision = m.Groups["revision"].Value;
        }
        
        private string RunGit(string path, string arguments)
        {
            var pinfo = new ProcessStartInfo()
            {
                WorkingDirectory = path,
                FileName = "git.exe",
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            };

            var p = Process.Start(pinfo);
            p.WaitForExit();

            var res = p.StandardOutput.ReadToEnd();
            return res;
        }
    }
}
