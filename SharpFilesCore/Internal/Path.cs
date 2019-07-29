using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SharpFilesCore.Tests")]
namespace SharpFilesCore.Internal
{
    public class Path : FsPath
    {
        public Path(string value) : base(value) { }
        
        public override bool IsFile => false;
        public override bool IsFolder => false;
        public override FsFolder Parent => null;
        public override bool Exists => System.IO.File.Exists(Path) || System.IO.Directory.Exists(Path);
        public override void Remove() { }

        protected override long GetSize() => 0;
    }
}