using System;
using System.IO;
using System.Reflection;
using System.Text;
using Xunit;

namespace SharpFilesCore.Tests.Internal
{
    public class FileTests : IDisposable
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder("FileTests");

        public FileTests()
        {
            _folder.Create();
        }
        
        public void Dispose()
        {
            _folder.Remove();
        }
        
        [Fact(Skip = "Travis fail")]
        public void ShouldCreateInstance()
        {
            var path = "C:\\test.txt";
            var sut = path.ToFile();
            Assert.Equal(sut.Path, path);
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
        public void ShouldTestIfFileExists()
        {
            var sut = "C:\\test_not_here.txt".ToFile();
            Assert.False(sut.Exists);

            sut = Path.GetFullPath(Assembly.GetExecutingAssembly().Location).ToFile();
            Assert.True(sut.Exists);
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
        public void ShouldGetParentDirectory()
        {
            var file = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(file).FullName;
            var sut = Path.Combine(file).ToFile();
            var parent = sut.Parent;
            
            Assert.Equal(parent.ToString(), parentDir);
        }
        
        [Fact]
        public void ShouldCopySmallFileWithoutProgress()
        {
            var sut = _folder.CombineFile("ShouldCopySmallFileWithoutProgress_1.txt");
            
            File.WriteAllText(sut, "ABCD");

            var file = _folder.CombineFile("ShouldCopySmallFileWithoutProgress_2.txt");

            Assert.True(File.Exists(sut));
            
            sut.Copy(file);
            
            Assert.True(File.Exists(file));
            
            Assert.Equal(sut.Content, file.Content);
        }
        
        [Fact]
        public void ShouldRemoveFile()
        {
            var sut = _folder.CombineFile("ShouldRemoveFile_1.txt");
            File.WriteAllText(sut, "ABCD");
            Assert.True(File.Exists(sut));
            sut.Remove();
            Assert.True(!File.Exists(sut));
        }
        
        [Fact]
        public void ShouldGetFilesContent()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.Create();
            File.WriteAllText(folder.CombineFile("test1.txt"), "ABCD");
            
            var sut = folder.CombineFile("test1.txt");
            
            Assert.Equal("ABCD", sut.Content);
            
            folder.Remove();
        }
        
        [Fact]
        public void ShouldWriteFilesContent()
        {
            var sut = _folder.CombineFile("ShouldWriteFilesContent.txt");
            
            File.WriteAllText(sut, "");
            
            Assert.Equal(0, File.ReadAllText(sut).Length);
            
            sut.Write("AAAA");
            
            Assert.Equal("AAAA", File.ReadAllText(sut));
        }

        [Fact]
        public void ShouldCalculateHash()
        {
            var sut = _folder.CombineFile("ShouldCalculateHash.txt");
            
            sut.Write("1");
            
            Assert.Equal(string.Join('-', sut.Hash), string.Join('-', new []
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
            var message = "111";
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.Create();
            
            var sut = folder.CombineFile("test5.txt");
            
            sut.Write(message);
            
            Assert.Equal(sut.Size, message.Length * 2);
            
            folder.Remove();
        }
        
        [Fact]
        public void ShouldRecalculateSize()
        {
            var message = "111";
            
            var sut = _folder.CombineFile(nameof(ShouldRecalculateSize));
            
            sut.Write(message);
            
            Assert.Equal(sut.Size, message.Length * 2);
            
            sut.Write(message + message);
            
            sut.RefreshSize();
            
            Assert.Equal(sut.Size, message.Length * 3);
        }
        
        [Fact]
        public void ShouldWriteFilesContentWhenFileDoesNotExist()
        {
            var folder = AppFsFolder.CombineFolder("CopyTest");
            folder.Create();
            
            var sut = folder.CombineFile("test3.txt");
            
            sut.Write("AAAA");
            
            Assert.Equal("AAAA", File.ReadAllText(sut));
            
            folder.Remove();
        }
        
        [Fact]
        public void ShouldRemoveReadOnlyFile()
        {
            var sut = _folder.CombineFile(nameof(ShouldRemoveReadOnlyFile));
            File.WriteAllText(sut, "ABCD");
            File.SetAttributes(sut, FileAttributes.ReadOnly);
            Assert.True(File.Exists(sut));
            sut.Remove();
            Assert.True(!File.Exists(sut));
        }
    }
}