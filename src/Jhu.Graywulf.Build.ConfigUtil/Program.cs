using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Build.ConfigUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            PrintHeader();

            var verb = args[0].ToLowerInvariant();

            switch (verb)
            {
                case "merge":
                    MergeConfig(args);
                    break;
                default:
                    // TODO
                    throw new NotImplementedException();
            }
        }

        static void PrintHeader()
        {
            Console.WriteLine("Graywulf configuration utility.");
        }

        static void MergeConfig(string[] args)
        {
            Console.Write("Merging configuration files for ");

            var settings = new Settings()
            {
                SkipMissingBaseConfig = false,
            };

            var spath = args[1];
            var solution = new Solution();
            solution.LoadSolution(spath);

            if (args.Length == 2)
            {
                Console.WriteLine("solution '{0}'...", System.IO.Path.GetFileName(spath));

                // Configure entire solution
                // TODO
            }
            else if (args.Length == 3)
            {
                // Configure a specific project
                var project = solution.Projects[args[2]];

                Console.WriteLine("project '{0}'...", project.Name);

                project.MergeConfigs(settings);
                project.UpdateVersion();
            }
            else
            {
                // TODO
                throw new NotImplementedException();
            }
        }
    }
}
