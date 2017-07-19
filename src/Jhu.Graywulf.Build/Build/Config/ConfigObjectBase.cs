using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Jhu.Graywulf.Build.Config
{
    public abstract class ConfigObjectBase
    {
        protected static void LogMessage(Task task, MessageImportance importance, string message, params object[] args)
        {
            if (task == null)
            {
                Console.WriteLine(message, args);
            }
            else
            {
                task.Log.LogMessage(importance, message, args);
            }
        }
    }
}
