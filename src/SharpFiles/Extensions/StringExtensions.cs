using RoseByte.SharpFiles.Core;
using RoseByte.SharpFiles.Core.Internal;

// ReSharper disable once CheckNamespace
namespace System
{
    public static class StringExtensions
    {
        public static FsPath ToPath(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            path = IO.Path.GetFullPath(path);

            if (IO.Directory.Exists(path))
            {
                return new Folder(path);
            }

            if (IO.File.Exists(path))
            {
                return new File(path);
            }

            return new Path(path);
        }

        public static FsFile ToFile(this string path)
        {
            return string.IsNullOrWhiteSpace(path)
                ? null
                : new File(IO.Path.GetFullPath(path));
        }

        public static FsFolder ToFolder(this string path)
        {
            return string.IsNullOrWhiteSpace(path)
                ? null
                : new Folder(IO.Path.GetFullPath(path));
        }
    }
}