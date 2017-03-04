using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Collections;

namespace Jhu.Graywulf.Build.Tasks
{
    public class TestBuildEngine : IBuildEngine
    {
        public int ColumnNumberOfTaskNode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool ContinueOnError
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int LineNumberOfTaskNode
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string ProjectFileOfTaskNode
        {
            get
            {
                return "Test project";
            }
        }

        public bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
        {
            throw new NotImplementedException();
        }

        public void LogCustomEvent(CustomBuildEventArgs e)
        {
        }

        public void LogErrorEvent(BuildErrorEventArgs e)
        {
        }

        public void LogMessageEvent(BuildMessageEventArgs e)
        {
        }

        public void LogWarningEvent(BuildWarningEventArgs e)
        {
        }
    }
}
