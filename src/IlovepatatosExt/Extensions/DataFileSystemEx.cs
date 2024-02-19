using JetBrains.Annotations;
using Newtonsoft.Json;
using Oxide.Core;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DataFileSystemEx
{
    public static IEnumerable<T> ParseAllFilesAs<T>(this DataFileSystem self)
    {
        return self.GetFiles().Select(Path.GetFileNameWithoutExtension).Select(self.ReadObject<T>);
    }

    public static void WriteObject<T>(this DataFileSystem self, string name, T obj, Formatting format)
    {
        self.GetFile(name).WriteObject(obj, format);
    }
}