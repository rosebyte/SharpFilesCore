using System.Collections.Generic;
using SharpFilesCore;

namespace RoseByte.SharpFiles.Core
{
    public abstract class FsFolder : FsPath
    {
        protected FsFolder(string value) : base(value)
        {
        }

        public abstract string Name { get; }
        public abstract FsFile CombineFile(string pathPart);
        public abstract FsFolder CombineFolder(string pathPart);
        public abstract void Create();
        public override bool IsFile => false;
        public override bool IsFolder => true;
        public abstract IEnumerable<FsFile> Files { get; }
        public abstract IEnumerable<FsFolder> Folders { get; }
        public abstract void Copy(FsFolder destination);
        public abstract void Move(FsFolder destination);
        public abstract void CreateSubFolder(string name);
        public abstract void CreateSubFile(string name);
        public abstract void Remove(string name);
        public abstract void Rename(string name);
    }
}