using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace RoseByte.SharpFiles.Core.Internal
{
    public class File : FsFile
    {
        public override string Name => System.IO.Path.GetFileName(Path);
        public override string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Path);

        private string _content;
        public override string Content => _content ?? (_content = System.IO.File.ReadAllText(Path));

        private byte[] _hash;

        public override byte[] Hash
        {
            get => _hash ?? (_hash = SHA256.Create().ComputeHash(System.IO.File.ReadAllBytes(Path)));
        }

        public override long Size => new FileInfo(Path).Length;

        internal File(string value) : base(value)
        {
        }

        private void PrepareCopy(FsFile target)
        {
            target.Parent.Create();

            if (target.Exists && (System.IO.File.GetAttributes(Path) & FileAttributes.ReadOnly) != 0)
            {
                System.IO.File.SetAttributes(Path, FileAttributes.Normal);
            }
        }

        public override Encoding Encoding
        {
            get
            {
                // Read the BOM
                var bom = new byte[4];
                using (var file = new FileStream(Path, FileMode.Open, FileAccess.Read))
                {
                    file.Read(bom, 0, 4);
                }

                // Analyze the BOM
                if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
                if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
                if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
                if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
                if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
                return Encoding.ASCII;
            }
        }

        public override bool HasEncoding(Encoding encoding) => Equals(Encoding, encoding);

        public override void Copy(FsFile target)
        {
            try
            {
                PrepareCopy(target);
                System.IO.File.Copy(Path, target, true);
            }
            catch (Exception exception)
            {
                throw new Exception($"File '{Path}' could not be copied to '{target}': {exception.Message}");
            }
        }

        public override void CopyToFolder(FsFolder target)
        {
            try
            {
                var destination = target.CombineFile(Name);

                System.IO.File.Copy(Path, destination, true);
            }
            catch (Exception exception)
            {
                throw new Exception($"File '{Path}' could not be copied to '{target}': {exception.Message}");
            }
        }

        public override FsFolder Parent => System.IO.Path.GetDirectoryName(Path).ToFolder();

        public override bool Exists => System.IO.File.Exists(Path);

        public override void Delete()
        {
            if (!Exists)
            {
                return;
            }

            try
            {
                if ((System.IO.File.GetAttributes(Path) & FileAttributes.ReadOnly) != 0)
                {
                    System.IO.File.SetAttributes(Path, FileAttributes.Normal);
                }

                System.IO.File.Delete(Path);
            }
            catch (Exception exception)
            {
                throw new Exception($"File '{Path}' could not be deleted: {exception.Message}");
            }
        }

        public override FsFile Write(string content)
        {
            try
            {
                PrepareCopy(this);
                System.IO.File.WriteAllText(Path, content, Encoding.UTF8);
            }
            catch (Exception exception)
            {
                throw new Exception($"Cannot write to '{Path}': {exception.Message}");
            }

            return this;
        }

        public override FsFile Create()
        {
            if (!Exists)
            {
                Write(string.Empty);
            }

            return this;
        }
    }
}