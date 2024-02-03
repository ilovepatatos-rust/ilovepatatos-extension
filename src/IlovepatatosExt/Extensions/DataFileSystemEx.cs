using JetBrains.Annotations;
using Oxide.Core;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DataFileSystemEx
{
    public static IEnumerable<T> ParseAllFilesAs<T>(this DataFileSystem self)
    {
        return self.GetFiles().Select(Path.GetFileNameWithoutExtension).Select(self.ReadObject<T>);
    }
}