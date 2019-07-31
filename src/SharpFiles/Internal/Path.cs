using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("RoseByte.SharpFiles.Core.Tests")]

namespace RoseByte.SharpFiles.Core.Internal
{
    public class Path : FsPath
    {
        public Path(string value) : base(value)
        {
        }

        public override bool IsFile => System.IO.File.Exists(Path);

        public override bool IsFolder => Directory.Exists(Path);

        public override FsFolder Parent
        {
            get
            {
                var splited = Path.Split('/');

                if (splited.Length < 2)
                {
                    return null;
                }

                return string.Join('/', splited.Take(splited.Length - 1)).ToFolder();
            }
        }

        public override bool Exists => System.IO.File.Exists(Path) || Directory.Exists(Path);

        public override void Delete()
        {
            if (System.IO.File.Exists(Path))
            {
                System.IO.File.Delete(Path);
            }
            else if (Directory.Exists(Path))
            {
                Directory.Delete(Path);
            }
        }

        public override long Size
        {
            get
            {
                if (System.IO.File.Exists(Path))
                {
                    return new FileInfo(Path).Length;
                }

                if (Directory.Exists(Path))
                {
                    return Directory.GetFiles(Path, "*", SearchOption.AllDirectories)
                        .Sum(t => new FileInfo(t).Length);
                }

                return 0;
            }
        }
    }
}