using SharpFilesCore;
using SharpFilesCore.Internal;

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
            
            path = System.IO.Path.GetFullPath(path);
			
            if (System.IO.Directory.Exists(path))
            {
                return new Folder(path);
            }
			
            if (System.IO.File.Exists(path))
            {
                return new File(path);
            }
			
            return new Path(path);
        }

        public static FsFile ToFile(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            
            path = System.IO.Path.GetFullPath(path);
            
            return new File(path);
        }

        public static FsFolder ToFolder(this string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            
            path = System.IO.Path.GetFullPath(path);
            
            return new Folder(path);
        }
    }
}