using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using File = RoseByte.SharpFiles.Core.Internal.File;

namespace RoseByte.SharpFiles.Core.Internal
{
    public class Folder : FsFolder
    {
        internal Folder(string value) : base(value) { }

        public override FsFolder MoveToFolder(FsFolder destination)
        {
            var target = destination.CombineFolder(Name);
            Directory.Move(Path, target);
            return target;
        }

        public override FsFolder Move(FsFolder destination)
        {
            Directory.Move(Path, destination);
            return destination;
        }

        public override IEnumerable<FsFolder> SearchFolders(string mask = "*")
        {
            return Directory.EnumerateDirectories(Path, mask, SearchOption.AllDirectories)
                .Select(x => x.ToFolder());
        }

        public override void Copy(FsFolder destination)
        {
            destination.Create();

            foreach (var subFolder in SearchFolders())
            {
                var subpath = subFolder.Path.Substring(Path.Length);
                destination.CombineFolder(subpath).Create();
            }

            foreach (var file in SearchFiles())
            {
                var subFile = file.Path.Substring(Path.Length);
                file.Copy(destination.CombineFile(subFile));
            }
        }

        public override void CopyToFolder(FsFolder destination)
        {
            var target = destination.CombineFolder(Name).Create();

            foreach (var folder in Folders)
            {
                var subFolder = folder.Path.Substring(Path.Length);
                target.CombineFolder(subFolder).Create();
            }

            foreach (var file in Files)
            {
                var subFile = file.Path.Substring(Path.Length);
                file.CopyToFolder(target.CombineFile(subFile).Parent);
            }
        }

        public override IEnumerable<FsFile> Files
        {
            get
            {
                return Directory.EnumerateFiles(Path, "*", SearchOption.TopDirectoryOnly)
                    .Select(x => new File(x));
            }
        }

        public override IEnumerable<FsFile> SearchFiles(string mask = "*")
        {
            return Directory.EnumerateFiles(Path, mask, SearchOption.AllDirectories)
                .Select(x => new File(x));
        }

        public override IEnumerable<FsFolder> Folders
        {
            get
            {
                return Directory.EnumerateDirectories(Path, "*", SearchOption.TopDirectoryOnly)
                    .Select(x => x.ToFolder());
            }
        }

        public override string Name
        {
            get
            {
                var splited = Path.Replace("/", "\\").Split('\\');

                return splited.Last();
            }
        }

        public override bool Exists => Directory.Exists(Path);

        public override long Size => Files.Sum(x => x.Size);

        public override FsFolder Parent
        {
            get
            {
                var parent = Directory.GetParent(Path).FullName;
                return parent.Length > Path.Length ? null : new Folder(parent);
            }
        }

        public override FsFile CombineFile(string pathPart) =>
            new File(System.IO.Path.Combine(this, pathPart.TrimStart('/', '\\')));

        public override FsFolder CombineFolder(string pathPart) =>
            new Folder(System.IO.Path.Combine(Path, pathPart.TrimStart('/', '\\')));

        public override FsFolder Rename(string name)
        {
            Directory.Move(Path, Parent.CombineFolder(name));
            return this;
        }

        public override bool IsEmpty => !Directory.EnumerateFileSystemEntries(Path).Any();

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

        public override void Delete(string child)
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

        public override FsFolder Create()
        {
            if (Exists)
            {
                return this;
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

            return this;
        }
    }
}