using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = RoseByte.SharpFiles.Core.Internal.File;

namespace RoseByte.SharpFiles.Core.Internal
{
    public class Folder : FsFolder
    {
        internal Folder(string value) : base(value)
        {
        }

        public override string Name => System.IO.Path.GetDirectoryName(Path);

        // copied from MSDN for now
        public override void Copy(FsFolder destination)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(Path);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + Path);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.


            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                var temppath = System.IO.Path.Combine(destination, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            foreach (var subdir in dirs)
            {
                var temppath = System.IO.Path.Combine(destination, subdir.Name).ToFolder();
                subdir.FullName.ToFolder().Copy(temppath);
            }
        }

        public override bool Exists => Directory.Exists(Path);
        public override long Size => Files.Sum(x => x.Size);

        public override FsFile CombineFile(string pathPart) => new File(System.IO.Path.Combine(this, pathPart));
        public override FsFolder CombineFolder(string pathPart) => new Folder(System.IO.Path.Combine(Path, pathPart));
        public override FsFolder Parent => new Folder(Directory.GetParent(Path).FullName);
        public override IEnumerable<FsFile> Files => GetFiles(Path);
        public override IEnumerable<FsFolder> Folders => GetFolders(Path);

        private IEnumerable<FsFile> GetFiles(string path)
        {
            var folders = new[] {path.ToFolder()}.Union(GetFolders(path).Select(x => x));

            foreach (var folder in folders)
            {
                var files = Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly)
                    .Select(x => new File(x));

                foreach (var file in files)
                {
                    yield return new File(file);
                }
            }
        }

        private IEnumerable<FsFolder> GetFolders(string path)
        {
            var files = Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                yield return new Folder(file);
            }
        }

        public override void CreateSubFile(string name)
        {
            var file = CombineFile(name);
            if (!file.Exists)
            {
                file.Write(string.Empty);
            }
        }

        public override void Remove(string child)
        {
            var folder = CombineFolder(child);
            if (folder.Exists)
            {
                folder.Delete();
                return;
            }

            var file = CombineFile(child);
            if (file.Exists)
            {
                file.Delete();
            }
        }

        public override void Rename(string name)
        {
            Directory.Move(Path, Parent.CombineFolder(name));
        }

        public override void Move(FsFolder destination)
        {
            Directory.Move(Path, destination.CombineFolder(Name));
        }

        public override void CreateSubFolder(string name)
        {
            var folder = CombineFolder(name);
            if (!folder.Exists)
            {
                folder.Create();
            }
        }

        public override void Delete()
        {
            if (!Exists)
            {
                return;
            }

            foreach (var file in Files)
            {
                file.Delete();
            }

            Directory.Delete(Path, true);
        }

        public override void Create()
        {
            if (Exists)
            {
                return;
            }

            if (Parent == null)
            {
                throw new Exception("Top level node (e.g. drive) can't be created.");
            }

            if (!Parent.Exists)
            {
                Parent.Create();
            }

            Directory.CreateDirectory(Path);
        }
    }
}