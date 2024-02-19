using System.Reflection;
using Newtonsoft.Json;
using Oxide.Core;
using Oxide.Core.Configuration;

namespace Oxide.Ext.IlovepatatosExt;

public static class DynamicConfigFileEx
{
    private static readonly MethodInfo s_checkPathMethod = typeof(DynamicConfigFile).GetMethod("CheckPath", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void WriteObject<T>(this DynamicConfigFile self, T obj, Formatting format)
    {
        string filename = (string)s_checkPathMethod.Invoke(self, new object[] { self.Filename });
        string directory = Utility.GetDirectoryName(filename);

        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        string json = JsonConvert.SerializeObject(obj, format, self.Settings);
        File.WriteAllText(filename, json);
    }
}