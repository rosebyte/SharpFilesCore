using System;
using System.Linq;
using System.Reflection;
using RoseByte.SharpFiles.Core.Internal;
using SharpFilesCore;
using SharpFilesCore.Internal;
using Xunit;

namespace RoseByte.SharpFiles.Core.Tests.Extensions
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ToPathShouldReturnNullForEmptyString()
        {
            Assert.Null(string.Empty.ToPath());
            Assert.Null(((string)null).ToPath());
            Assert.Null(" ".ToPath());
        }
        
        [Fact]
        public void ToFolderShouldReturnNullForEmptyString()
        {
            Assert.Null(string.Empty.ToFolder());
            Assert.Null(((string)null).ToFolder());
            Assert.Null(" ".ToFolder());
        }
        
        [Fact]
        public void ToFileShouldReturnNullForEmptyString()
        {
            Assert.Null(string.Empty.ToFile());
            Assert.Null(((string)null).ToFile());
            Assert.Null(" ".ToFile());
        }

        [Fact]
        public void ShouldReturnFolderPath()
        {
            Assert.IsAssignableFrom<FsFolder>("C:\\Windows".ToPath());
        }

        [Fact]
        public void ShouldReturnPathIfPathDoesNotExist()
        {
            var sut = "C:\\test.txt".ToPath();
            
            Assert.IsType<Path>(sut);
        }
        
        [Fact]
        public void ShouldReturnFilePath()
        {
            var filePath = System.IO.Path.GetFullPath(Assembly.GetExecutingAssembly().Location);
            Assert.IsAssignableFrom<FsFile>(filePath.ToPath());
        }
        
        [Fact]
        public void ShouldResolveRelativeFolder()
        {
            var folderName = System.IO.Path.GetFullPath(".").Split('\\').Last();
            Assert.Equal($"..\\.\\{folderName}".ToFolder(), System.IO.Path.GetFullPath(".").ToFolder());
        }
        
        [Fact]
        public void ShouldResolveRelativeFile()
        {
            var fileName = System.IO.Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var fullName = $"{System.IO.Path.GetFullPath(".")}\\{fileName}";
            
            Assert.Equal($".\\{fileName}".ToFile(), fullName.ToFile());
        }
        
        [Fact]
        public void ShouldResolveFilePath()
        {
            var fileName = System.IO.Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var fullName = $"{System.IO.Path.GetFullPath(".")}\\{fileName}";
            
            Assert.Equal($".\\{fileName}".ToPath(), fullName.ToFile());
        }
        
        [Fact]
        public void ShouldResolveParentPath()
        {
            var fileName = System.IO.Path.GetFileName(Assembly.GetCallingAssembly().Location);
            var folderName = System.IO.Path.GetFullPath(".").Split('\\').Last();
            var fullName = $"{System.IO.Path.GetFullPath(".")}\\{fileName}";
            
            Assert.Equal($"..\\{folderName}\\{fileName}".ToPath(), fullName.ToFile());
        }
    }
}