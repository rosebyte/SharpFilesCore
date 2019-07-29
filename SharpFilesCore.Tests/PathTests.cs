using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace SharpFilesCore.Tests
{
    public class PathTests : IDisposable
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder(nameof(PathTests));

        public PathTests() => _folder.Create();
        
        public void Dispose() => _folder.Remove();

        [Fact]
        public void ShouldReturnFalseToFIleAndFolderTests()
        {
            var sut = "C:\\notHere.txt".ToPath();
            
            Assert.False(sut.IsFile);
            Assert.False(sut.IsFolder);
        }
        
        [Fact]
        public void ShouldReflectIfExists()
        {
            var folder = _folder.CombineFolder(nameof(ShouldReflectIfExists));
            folder.Create();
            var file = folder.CombineFile("exists.txt");

            var sut = file.ToString().ToPath();
            
            Assert.False(sut.Exists);
            
            file.Write("A");
            
            Assert.True(sut.Exists);
        }
    }
}