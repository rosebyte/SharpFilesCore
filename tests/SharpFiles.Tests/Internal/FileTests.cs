using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace RoseByte.SharpFiles.Core.Tests.Internal
{
    public class FileTests : IDisposable
    {
        private static FsFolder AppFsFolder =>
            (FsFolder) Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder("FileTests");

        public FileTests() => _folder.Create();

        public void Dispose() => _folder.Delete();

        [Fact]
        public void ShouldCreateInstance()
        {
            var path = "C:\\test.txt";
            var sut = path.ToFile();
            Assert.Equal(sut.Path, path);
        }

        [Fact]
        public void ShouldReturnFalseToIsFolder()
        {
            var sut = "C:\\test.txt".ToFile();
            Assert.True(sut.IsFile);
            Assert.False(sut.IsFolder);
        }

        [Fact]
        public void ShouldReturnFileName()
        {
            var fileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            var sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.Equal(sut.Name, fileName);
        }

        [Fact]
        public void ShouldReturnFileNameWithoutExtension()
        {
            var fileName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            var sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.Equal(sut.NameWithoutExtension, fileName);
        }

        [Fact]
        public void ShouldGetFilesContent()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.Create();
            File.WriteAllText(folder.CombineFile("test1.txt"), "ABCD");

            var sut = folder.CombineFile("test1.txt");

            Assert.Equal("ABCD", sut.Content);

            folder.Delete();
        }

        [Fact]
        public void ShouldCopyToFolder()
        {
            var sut = _folder.CombineFile($"{nameof(ShouldCopyToFolder)}_source").Write("ABCD");
            var folder = _folder.CombineFolder(nameof(ShouldCopyToFolder)).Create();

            var file = folder.CombineFile($"{nameof(ShouldCopyToFolder)}_source");

            Assert.True(File.Exists(sut));
            Assert.False(File.Exists(file));

            sut.CopyToFolder(folder);

            Assert.True(File.Exists(file));

            Assert.Equal(sut.Content, file.Content);
        }

        [Fact]
        public void ShouldCopyFile()
        {
            var sut = _folder.CombineFile(nameof(ShouldCopyFile)).Write("ABCD");

            var file = _folder.CombineFile($"{nameof(ShouldCopyFile)}_2");

            Assert.True(File.Exists(sut));
            Assert.False(File.Exists(file));

            sut.Copy(file);

            Assert.True(File.Exists(file));

            Assert.Equal(sut.Content, file.Content);
        }

        [Fact]
        public void ShouldReturnEncoding()
        {
            var sut = _folder.CombineFile(nameof(ShouldReturnEncoding));

            sut.Write("A");

            Assert.Equal(sut.Encoding, Encoding.UTF8);
            File.WriteAllText(sut, "AV", Encoding.ASCII);
            Assert.Equal(sut.Encoding, Encoding.ASCII);
        }

        [Fact]
        public void ShouldTestEncoding()
        {
            var sut = _folder.CombineFile(nameof(ShouldTestEncoding));
            sut.Write("A");
            Assert.True(sut.HasEncoding(Encoding.UTF8));
            Assert.False(sut.HasEncoding(Encoding.ASCII));
            File.WriteAllText(sut, "AV", Encoding.ASCII);
            Assert.True(sut.HasEncoding(Encoding.ASCII));
            Assert.False(sut.HasEncoding(Encoding.UTF8));
        }

        [Fact]
        public void ShouldGetParentDirectory()
        {
            var file = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(file).FullName;
            var sut = Path.Combine(file).ToFile();
            var parent = sut.Parent;

            Assert.Equal(parent.ToString(), parentDir);
        }

        [Fact]
        public void ShouldTestIfFileExists()
        {
            var sut = "C:\\test_not_here.txt".ToFile();
            Assert.False(sut.Exists);

            sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.True(sut.Exists);
        }

        [Fact]
        public void ShouldCalculateHash()
        {
            var sut = _folder.CombineFile(nameof(ShouldCalculateHash)).Write("1");

            Assert.Equal(string.Join('-', sut.Hash), string.Join('-', new[]
            {
                90, 189, 182, 23, 95, 130, 15, 10, 195, 216,
                100, 127, 187, 31, 122, 11, 204, 145, 117, 122,
                120, 42, 138, 20, 85, 112, 148, 76, 166, 160,
                12, 150
            }));
        }

        [Fact]
        public void ShouldCalculateSize()
        {
            const string message = "111";
            var sut = AppFsFolder.CombineFile(nameof(ShouldCalculateSize))
                .Write(message);

            sut.Write(message);

            Assert.Equal(sut.Size, message.Length + 3);

            sut.Delete();
        }

        [Fact]
        public void ShouldWriteFilesContent()
        {
            var sut = _folder.CombineFile(nameof(ShouldWriteFilesContent)).Create();

            Assert.Equal(0, File.ReadAllText(sut).Length);

            sut.Write("AAAA");

            Assert.Equal("AAAA", File.ReadAllText(sut));
        }

        [Fact]
        public void ShouldWriteFilesContentWhenFileDoesNotExist()
        {
            var sut = _folder.CombineFile(nameof(ShouldWriteFilesContentWhenFileDoesNotExist));

            Assert.False(sut.Exists);

            sut.Write("AAAA");

            Assert.Equal("AAAA", File.ReadAllText(sut));

            sut.Delete();
        }

        [Fact]
        public void ShouldDeleteReadOnlyFile()
        {
            var sut = _folder.CombineFile(nameof(ShouldDeleteReadOnlyFile)).Create();
            File.SetAttributes(sut, FileAttributes.ReadOnly);
            Assert.True(File.Exists(sut));
            sut.Delete();
            Assert.False(File.Exists(sut));
        }

        [Fact]
        public void ShouldDeleteFile()
        {
            var sut = _folder.CombineFile(nameof(ShouldDeleteFile)).Create();
            Assert.True(File.Exists(sut));
            sut.Delete();
            Assert.True(!File.Exists(sut));
        }

        [Fact]
        public void ShouldCreateFile()
        {
            var sut = _folder.CombineFile(nameof(ShouldCreateFile));
            Assert.False(File.Exists(sut));
            sut.Create();
            Assert.True(File.Exists(sut));
        }
    }
}