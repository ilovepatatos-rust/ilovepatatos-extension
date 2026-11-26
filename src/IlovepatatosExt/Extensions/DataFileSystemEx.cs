using JetBrains.Annotations;
using Newtonsoft.Json;
using Oxide.Core;

namespace Oxide.Ext.IlovepatatosExt;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public static class DataFileSystemEx
{
    extension(DataFileSystem self)
    {
        [MustUseReturnValue]
        public IEnumerable<T> ParseAllFilesAs<T>()
        {
            return self.GetFiles().Select(Path.GetFileNameWithoutExtension).Select(self.ReadObject<T>);
        }

        public void WriteObject<T>(string name, T obj, Formatting format)
        {
            self.GetFile(name).WriteObject(obj, format);
        }

        /// <summary>
        /// Parse a file as an object or return a new instance of the object if the file does not exist.
        /// </summary>
        /// <remarks>
        /// This method handles cases where the file exists but is empty.
        /// </remarks>
        [MustUseReturnValue]
        [Obsolete("Use " + nameof(DataFileSystem.ReadObject) + " instead. Reading empty file has been fixed on July 2025 update.")]
        public T ReadOrCreateObject<T>(string filename) where T : class
        {
            T value = null;

            if (self.ExistsDatafile(filename))
                value = self.GetFile(filename).ReadObject<T>();

            return value ?? Activator.CreateInstance<T>();
        }

        [Obsolete("Use " + nameof(DataFileSystem.ReadObject) + " instead. Reading empty file has been fixed on July 2025 update.")]
        public T ReadObjectOrFallback<T>(string filename) where T : class
        {
            return ReadOrCreateObject<T>(self, filename);
        }

        /// <summary>
        /// Tries to read an object from a file. Returns null if the file does not exist.
        /// </summary>
        [MustUseReturnValue]
        public T TryReadObject<T>(string filename) where T : class
        {
            return self.ExistsDatafile(filename) ? self.GetFile(filename).ReadObject<T>() : null;
        }
    }
}