using System;
using System.IO;
using System.Linq;
using System.Reflection;
using SharpFilesCore.Internal;
using Xunit;

namespace SharpFilesCore.Tests.Internal
{
    public class FolderTests : IDisposable
    {
        private static FsFolder AppFsFolder => 
            (FsFolder)Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName.ToPath();

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
            
            var subfolder111 = subfolder11.CombineFolder("SubFolder_1_1_1");
            subfolder111.Create();
            subfolder111.CombineFile("Test_1_1_1_1.txt").Write("1_1_1_1");
            subfolder111.CombineFile("Test_1_1_1_2.txt").Write("1_1_1_2");
            
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
        
        public void Dispose() => _folder.Remove();

        [Fact]
        public void ShouldCompareToNull()
        {
            FsFolder sut = null;
            
            Assert.True(sut == null);
        }
        
        [Fact]
        public void ShouldReturnAllFiles()
        {
            Assert.Equal(
                16, 
                Directory.EnumerateFiles(_folder, "*", SearchOption.AllDirectories).Count());
        }
        
        [Fact]
        public void ShouldReturnAllFolders()
        {
            Assert.Equal(
                7, 
                Directory.EnumerateDirectories(_folder, "*", SearchOption.AllDirectories).Count());
        }
        
        [Fact]
        public void ShouldTestIfFolderExists()
        {
            Assert.True(_folder.Exists);
            Assert.False(_folder.CombineFolder("NOT_HERE").Exists);
        }

        [Fact]
        public void ShouldSumSIzeOfFolder()
        {
            Assert.Equal(44, _folder.CombineFolder("SubFolder_1").Size);
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
        public void ShouldGetParentDirectory()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var parentDir = Directory.GetParent(dir).FullName;
            var sut = System.IO.Path.Combine(dir).ToPath();
            var parent = sut.Parent;
            
            Assert.Equal(parent.ToString(), parentDir);
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
            
            AppFsFolder.CombineFolder("FolderCreationTest").Remove();
            
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
        public void ShouldCopySubfile()
        {
            var value = "SubFolder_1\\Test_1_1.txt";
            var parent = _folder.CombineFolder(nameof(ShouldCopySubfile));
            parent.Create();
            var subparent = parent.CombineFolder("SubFolder_1");
            subparent.Create();
            var file = parent.CombineFile(value);
            file.Write("1_1");
            
            Assert.False(parent.CombineFile("Test_1_1.txt").Exists);
            file.CopyToParent(parent);
            Assert.Equal("1_1", parent.CombineFile(value).Content);
            parent.Remove();
        }
        
        [Fact]
        public void ShouldRemoveSubfolder()
        {
            var value = nameof(ShouldRemoveSubfolder);
            _folder.CreateSubFolder(value);
            Assert.True(_folder.CombineFolder(value).Exists);
            _folder.Remove(value);
            Assert.False(_folder.CombineFolder(value).Exists);
        }

        [Fact]
        public void ShouldRemoveSubfile()
        {
            var value = $"{nameof(ShouldRemoveSubfile)}\\{nameof(ShouldRemoveSubfile)}";
            _folder.CreateSubFile(value);
            
            Assert.True(_folder.CombineFile(value).Exists);
            _folder.Remove(value);
            Assert.False(_folder.CombineFile(value).Exists);
        }

        [Fact]
        public void ShouldCreateSubFolder()
        {
            var value = nameof(ShouldCreateSubFolder);
            Assert.False(_folder.CombineFolder(value).Exists);
            _folder.CreateSubFolder(value);
            Assert.True(_folder.CombineFolder(value).Exists);
            _folder.CombineFolder(nameof(ShouldCreateSubFolder)).Remove();
        }
        
        [Fact]
        public void ShouldCreateSubFile()
        {
            var value = nameof(ShouldCreateSubFile);
            Assert.False(_folder.CombineFile(value).Exists);
            _folder.CreateSubFile(value);
            Assert.True(_folder.CombineFile(value).Exists);
            _folder.CombineFolder(nameof(ShouldCreateSubFile)).Remove();
        }

        [Fact]
        public void ShouldRemove()
        {
            var subFolder = _folder.CombineFolder(nameof(ShouldRemove));
            subFolder.Create();
            subFolder.CombineFile($"{nameof(ShouldRemove)}_1").Write("A");
            subFolder.CombineFile($"{nameof(ShouldRemove)}_2").Write("B");
            Assert.True(subFolder.Exists);
            subFolder.Remove();
            Assert.False(subFolder.Exists);
        }
    }
}