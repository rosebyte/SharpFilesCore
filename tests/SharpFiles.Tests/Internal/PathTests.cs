using System;
using System.IO;
using System.Reflection;
using Xunit;

namespace RoseByte.SharpFiles.Core.Tests.Internal
{
    public class PathTests : IDisposable
    {
        private static FsFolder AppFsFolder =>
            (FsFolder) Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder(nameof(PathTests));

        public PathTests() => _folder.Create();

        public void Dispose() => _folder.Delete();

        [Fact]
        public void ShouldReturnFalseToFileAndFolder()
        {
            var sut = _folder
                .CombineFolder(nameof(ShouldReturnFalseToFileAndFolder))
                .ToString()
                .ToPath();

            Assert.False(sut.IsFile);
            Assert.False(sut.IsFolder);
        }

        [Fact]
        public void ShouldReturnTrueToFile()
        {
            _folder.CombineFile(nameof(ShouldReturnTrueToFile)).Create();
            var sut = $"{_folder}\\{nameof(ShouldReturnTrueToFile)}".ToPath();

            Assert.True(sut.IsFile);
            Assert.False(sut.IsFolder);
        }

        [Fact]
        public void ShouldReturnTrueToFolder()
        {
            _folder.CombineFolder(nameof(ShouldReturnTrueToFolder)).Create();
            var sut = $"{_folder}\\{nameof(ShouldReturnTrueToFolder)}".ToPath();

            Assert.False(sut.IsFile);
            Assert.True(sut.IsFolder);
        }

        [Fact]
        public void ShouldReturnFalseToFIleAndFolderTests()
        {
            var sut = "C:\\notHere.txt".ToPath();

            Assert.False(sut.IsFile);
            Assert.False(sut.IsFolder);
        }

        [Fact]
        public void ShouldNotThrowOnDeleteIfPathDoesntExist()
        {
            var sut = (_folder + nameof(ShouldNotThrowOnDeleteIfPathDoesntExist)).ToPath();
            sut.Delete();
        }

        [Fact]
        public void ShouldReturnZeroSizeIfPathDoesntExist()
        {
            var sut = (_folder + nameof(ShouldReturnZeroSizeIfPathDoesntExist)).ToPath();
            Assert.Equal(0, sut.Size);
        }

        [Fact]
        public void ShouldDeleteFolder()
        {
            var sut = _folder.CombineFolder(nameof(ShouldDeleteFolder))
                .Create();

            Assert.True(sut.Exists);
            sut.Delete();
            Assert.False(sut.Exists);
        }

        [Fact]
        public void ShouldDeleteFile()
        {
            var sut = _folder.CombineFile(nameof(ShouldDeleteFolder))
                .Create();

            Assert.True(sut.Exists);
            sut.Delete();
            Assert.False(sut.Exists);
        }

        [Fact]
        public void ShouldReturnFolderSize()
        {
            var subfolder = _folder.CombineFolder(nameof(ShouldReturnFolderSize))
                .Create();

            subfolder.CombineFile("first").Write("aa11");
            subfolder.CombineFile("second").Write("yyy");

            var sut = (_folder + "\\" + nameof(ShouldReturnFolderSize)).ToPath();

            Assert.Equal(13, sut.Size);
        }

        [Fact]
        public void ShouldReturnParent()
        {
            var subfolder = _folder.CombineFolder(nameof(ShouldReturnParent))
                .Create();

            subfolder.CombineFile("first").Create();

            var sut = (_folder + "\\" + nameof(ShouldReturnParent) + "\\first").ToPath();

            Assert.Equal(subfolder, sut.Parent);
        }

        [Fact]
        public void ShouldReturnParentForNonExistingPath()
        {
            var subfolder = _folder.CombineFolder(nameof(ShouldReturnParentForNonExistingPath))
                .Create();

            var sut = (_folder + "\\" + nameof(ShouldReturnParentForNonExistingPath) + "\\first")
                .ToPath();

            Assert.Equal(subfolder, sut.Parent);
        }

        [Fact]
        public void ShouldReturnFileSize()
        {
            _folder.CombineFile(nameof(ShouldReturnFileSize)).Write("aaAA11");
            var sut = (_folder + "\\" + nameof(ShouldReturnFileSize)).ToPath();
            Assert.Equal(9, sut.Size);
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