using System.Collections.Generic;

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
        public abstract FsFolder Create();
        public override bool IsFile => false;
        public override bool IsFolder => true;
        public abstract IEnumerable<FsFile> Files { get; }
        public abstract IEnumerable<FsFolder> Folders { get; }
        public abstract void Copy(FsFolder destination);
        public abstract void CopyToFolder(FsFolder destination);
        public abstract FsFolder Move(FsFolder destination);
        public abstract FsFolder MoveToFolder(FsFolder destination);
        public abstract void Delete(string name);
        public abstract FsFolder Rename(string name);
    }
}