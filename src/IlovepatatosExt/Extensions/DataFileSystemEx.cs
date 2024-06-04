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

    /// <summary>
    /// Parse a file as an object or return a new instance of the object if the file does not exist.
    /// </summary>
    /// <remarks>
    /// This method handles cases where the file exists but is empty.
    /// </remarks>
    public static T ReadObjectOrFallback<T>(this DataFileSystem self, string name) where T : class
    {
        T value = null;

        if (self.ExistsDatafile(name))
            value = self.GetFile(name).ReadObject<T>();

        if (value == null)
            value = Activator.CreateInstance<T>();

        return value;
    }
}