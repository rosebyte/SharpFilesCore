using System;
using System.IO;
using System.Linq;
using System.Reflection;
using RoseByte.SharpFiles.Core.Internal;
using Xunit;

namespace RoseByte.SharpFiles.Core.Tests.Internal
{
    public class FolderTests : IDisposable
    {
        private static FsFolder AppFsFolder =>
            (FsFolder) Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

        private readonly FsFolder _folder = AppFsFolder.CombineFolder(nameof(FolderTests));

        public FolderTests()
        {
            _folder.Create();

            _folder.CombineFile("Test_0_1.txt").Write("0_1");
            _folder.CombineFile("Test_0_2.txt").Write("0_2");

            var subfolder1 = _folder.CombineFolder("SubFolder_1");
            subfolder1.Create();
            subfolder1.CombineFile("Test_1_1.txt").Write("1_1");
            subfolder1.CombineFile("Test_1_2.txt").Write("1_2");

            var subfolder11 = subfolder1.CombineFolder("SubFolder_1_1");
            subfolder11.Create();
            subfolder11.CombineFile("Test_1_1_1.txt").Write("1_1_1");
            subfolder11.CombineFile("Test_1_1_2.txt").Write("1_1_2");

            var subfolder12 = subfolder1.CombineFolder("SubFolder_1_2");
            subfolder12.Create();
            subfolder12.CombineFile("Test_1_2_1.txt").Write("1_2_1");
            subfolder12.CombineFile("Test_1_2_2.txt").Write("1_2_2");

            var subfolder2 = _folder.CombineFolder("SubFolder_2");
            subfolder2.Create();
            subfolder2.CombineFile("Test_2_1.txt").Write("2_1");
            subfolder2.CombineFile("Test_2_2.txt").Write("2_2");

            var subfolder21 = subfolder2.CombineFolder("SubFolder_2_1");
            subfolder21.Create();
            subfolder21.CombineFile("Test_2_1_1.txt").Write("2_1_1");
            subfolder21.CombineFile("Test_2_1_2.txt").Write("2_1_2");

            var subfolder22 = subfolder2.CombineFolder("SubFolder_2_2");
            subfolder22.Create();
            subfolder22.CombineFile("Test_2_2_1.txt").Write("2_2_1");
            subfolder22.CombineFile("Test_2_2_2.txt").Write("2_2_2");
        }

        public void Dispose() => _folder.Delete();

        [Fact]
        public void ShouldCompareToNull()
        {
            FsFolder sut = null;

            Assert.True(sut == null);
        }

        [Fact]
        public void ShouldCreateFolderInstance()
        {
            var path = "C:\\";
            var sut = path.ToFolder();
            Assert.Equal("C:", sut.Path);
        }

        [Fact]
        public void ShouldReturnFalseToIsFolder()
        {
            var sut = "C:\\".ToFolder();
            Assert.False(sut.IsFile);
            Assert.True(sut.IsFolder);
        }

        [Fact]
        public void ShouldMoveSubfile()
        {
            var parent = _folder.CombineFolder(nameof(ShouldMoveSubfile)).Create();
            var subparent = parent.CombineFolder("SubFolder_1").Create();
            subparent.CombineFile("Test_1_1.txt").Write("1_1");
            var newParent = _folder.CombineFolder($"{nameof(ShouldMoveSubfile)}_2").Create();

            Assert.False(newParent.CombineFile("SubFolder_1").Exists);
            subparent.MoveToFolder(newParent);
            Assert.True(newParent.CombineFolder("SubFolder_1").Exists);
            Assert.False(parent.CombineFolder("SubFolder_1").Exists);

            var newFile = newParent.CombineFolder("SubFolder_1").CombineFile("Test_1_1.txt");
            Assert.Equal("1_1", newFile.Content);

            parent.Delete();
            newParent.Delete();
        }

        [Fact]
        public void ShouldMove()
        {
            var parent = _folder.CombineFolder(nameof(ShouldCopySubfile)).Create();
            var subparent = parent.CombineFolder("SubFolder_1").Create();
            subparent.CombineFile("Test_1_1.txt").Write("1_1");
            var newParent = _folder.CombineFolder($"{nameof(ShouldCopySubfile)}_2").Create();

            Assert.False(newParent.CombineFile("SubFolder_2").Exists);
            subparent.Move(newParent.CombineFolder("Subfolder_2"));
            Assert.True(newParent.CombineFolder("SubFolder_2").Exists);
            Assert.False(parent.CombineFolder("SubFolder_2").Exists);

            var newFile = newParent.CombineFolder("SubFolder_2").CombineFile("Test_1_1.txt");
            Assert.Equal("1_1", newFile.Content);

            parent.Delete();
            newParent.Delete();
        }

        [Fact]
        public void ShouldCopySubfile()
        {
            var parent = _folder.CombineFolder(nameof(ShouldCopySubfile)).Create();
            var subparent = parent.CombineFolder("SubFolder_1").Create();
            subparent.CombineFile("Test_1_1.txt").Write("1_1");
            var newParent = _folder.CombineFolder($"{nameof(ShouldCopySubfile)}_2").Create();

            Assert.False(newParent.CombineFile("SubFolder_1").Exists);
            subparent.CopyToFolder(newParent);
            Assert.True(newParent.CombineFolder("SubFolder_1").Exists);

            var newFile = newParent.CombineFolder("SubFolder_1").CombineFile("Test_1_1.txt");
            Assert.Equal("1_1", newFile.Content);

            parent.Delete();
            newParent.Delete();
        }

        [Fact]
        public void ShouldCopy()
        {
            var parent = _folder.CombineFolder(nameof(ShouldCopySubfile)).Create();
            var subparent = parent.CombineFolder("SubFolder_1").Create();
            subparent.CombineFile("Test_1_1.txt").Write("1_1");
            var newParent = _folder.CombineFolder($"{nameof(ShouldCopySubfile)}_2").Create();

            Assert.False(newParent.CombineFile("SubFolder_2").Exists);
            subparent.Copy(newParent.CombineFolder("Subfolder_2"));
            Assert.True(newParent.CombineFolder("SubFolder_2").Exists);

            var newFile = newParent.CombineFolder("SubFolder_2").CombineFile("Test_1_1.txt");
            Assert.Equal("1_1", newFile.Content);

            parent.Delete();
            newParent.Delete();
        }

        [Fact]
        public void ShouldReturnAllFiles()
        {
            Assert.Equal(
                14,
                Directory.EnumerateFiles(_folder, "*", SearchOption.AllDirectories).Count());
        }

        [Fact]
        public void ShouldReturnAllFolders()
        {
            Assert.Equal(
                6,
                Directory.EnumerateDirectories(_folder, "*", SearchOption.AllDirectories).Count());
        }

        [Fact]
        public void ShouldReturnFolderName()
        {
            var parent = _folder.CombineFolder(nameof(ShouldCopySubfile));
            Assert.Equal(nameof(ShouldCopySubfile), parent.Name);
            parent.Delete();
        }

        [Fact]
        public void ShouldTestIfFolderExists()
        {
            Assert.True(_folder.Exists);
            Assert.False(_folder.CombineFolder("NOT_HERE").Exists);
        }

        [Fact]
        public void ShouldSumSizeOfFolder()
        {
            Assert.Equal(44, _folder.CombineFolder("SubFolder_1").Size);
        }

        [Fact]
        public void ShouldGetParentDirectory()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(dir).FullName;
            var sut = System.IO.Path.Combine(dir).ToPath();
            var parent = sut.Parent;

            Assert.Equal(parent.ToString(), parentDir);
        }

        [Fact]
        public void ParentDirectoryChainShouldEndWithNull()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = System.IO.Path.Combine(dir).ToPath();

            while (sut.Parent != null)
            {
                var current = sut;
                sut = sut.Parent;
                Assert.Equal(
                    current.ToString().Split("\\").Length - 1,
                    sut.ToString().Split("\\").Length);
            }

            Assert.Null(sut.Parent);
        }

        [Fact]
        public void ShouldCombineFile()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = System.IO.Path.Combine(dir).ToPath();
            var parent = sut.Parent;
            var name = sut.ToString().Split('\\').Last();

            Assert.Equal(parent.CombineFolder(name).ToString(), sut.ToString());
        }

        [Fact]
        public void ShouldCombineFolder()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = System.IO.Path.Combine(dir).ToPath();
            var parent = sut.Parent;
            var name = sut.ToString().Split('\\').Last();

            Assert.Equal(parent.CombineFolder(name).ToString(), sut.ToString());
        }

        [Fact]
        public void ShouldCreateFolderWithParentFolder()
        {
            AppFsFolder.CombineFolder("FolderCreationTest\\Subfolder\\OneMoreSubfolder").Create();

            var firstFolder = System.IO.Path.Combine(AppFsFolder, "FolderCreationTest");
            var secondFolder = System.IO.Path.Combine(AppFsFolder, "FolderCreationTest\\Subfolder");
            var thirdFolder = System.IO.Path.Combine(AppFsFolder, "FolderCreationTest\\Subfolder\\OneMoreSubfolder");

            Assert.True(Directory.Exists(firstFolder));
            Assert.True(Directory.Exists(secondFolder));
            Assert.True(Directory.Exists(thirdFolder));

            AppFsFolder.CombineFolder("FolderCreationTest").Delete();

            Assert.False(Directory.Exists(firstFolder));
            Assert.False(Directory.Exists(secondFolder));
            Assert.False(Directory.Exists(thirdFolder));
        }

        [Fact]
        public void ShouldCreateFolderWithDot()
        {
            var sut = new Folder("C:\\Test.Folder");

            Assert.Equal("C:\\Test.Folder", sut.ToString());
        }

        [Fact]
        public void ShouldRemoveSubfolder()
        {
            var value = nameof(ShouldRemoveSubfolder);
            _folder.CombineFolder(value).Create();
            Assert.True(_folder.CombineFolder(value).Exists);
            _folder.Delete(value);
            Assert.False(_folder.CombineFolder(value).Exists);
        }

        [Fact]
        public void ShouldRemoveSubfile()
        {
            var value = $"{nameof(ShouldRemoveSubfile)}\\{nameof(ShouldRemoveSubfile)}";
            _folder.CombineFile(value).Create();

            Assert.True(_folder.CombineFile(value).Exists);
            _folder.Delete(value);
            Assert.False(_folder.CombineFile(value).Exists);
        }

        [Fact]
        public void ShouldDelete()
        {
            var subFolder = _folder.CombineFolder(nameof(ShouldDelete));
            subFolder.Create();
            var file1 = subFolder.CombineFile($"{nameof(ShouldDelete)}_1").Create();
            var file2 = subFolder.CombineFile($"{nameof(ShouldDelete)}_2").Create();

            Assert.True(subFolder.Exists);
            Assert.True(file1.Exists);
            Assert.True(file2.Exists);
            subFolder.Delete();
            Assert.False(subFolder.Exists);
            Assert.False(file1.Exists);
            Assert.False(file2.Exists);
        }

        [Fact]
        public void ShouldRenameFolder()
        {
            var sut = _folder.CombineFolder(nameof(ShouldRenameFolder));
            sut.Create();
            var subfolder = sut.CombineFolder("OldName");
            subfolder.Create();

            Assert.True(sut.CombineFolder("OldName").Exists);
            Assert.False(sut.CombineFolder("NewName").Exists);

            subfolder.Rename("NewName");

            Assert.True(sut.CombineFolder("NewName").Exists);
            Assert.False(sut.CombineFolder("OldName").Exists);
        }
    }
}