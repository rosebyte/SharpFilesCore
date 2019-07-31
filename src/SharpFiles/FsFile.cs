using System.Text;

namespace RoseByte.SharpFiles.Core
{
    public abstract class FsFile : FsPath
    {
        protected FsFile(string value) : base(value)
        {
        }

        public abstract string Name { get; }
        public abstract string NameWithoutExtension { get; }
        public abstract string Content { get; }
        public abstract bool HasEncoding(Encoding encoding);
        public abstract FsFile Write(string content);
        public override bool IsFile => true;
        public override bool IsFolder => false;
        public abstract byte[] Hash { get; }
        public abstract FsFile Create();
        public abstract void Copy(FsFile target);
        public abstract void CopyToFolder(FsFolder destination);
        public abstract Encoding Encoding { get; }
    }
}