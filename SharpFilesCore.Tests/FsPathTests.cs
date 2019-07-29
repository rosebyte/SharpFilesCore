using System;
using System.IO;
using System.Reflection;
using Moq;
using Moq.Protected;
using Xunit;

namespace SharpFilesCore.Tests
{
    public class FsPathTests
    {
        [Fact]
        public void ShouldHandleEqualityOperators()
        {
            var dir = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
            var sut = Path.Combine(dir).ToPath();
            var sut2 = Path.Combine(dir).ToPath();
            var sut3 = sut.Parent;
            
            Assert.True(sut.Equals(sut2));
            
            Assert.True(sut == sut2);
            Assert.False(sut == sut3);
            Assert.False(sut != sut2);
            Assert.True(sut != sut3);
        }

        [Fact]
        public void ShouldUseGetSize()
        {
            var sut = new Mock<FsPath>("C:\\test.txt");
            sut.Protected().Setup<long>("GetSize").Returns(100);
            
            Assert.Equal(sut.Object.Size, 100);
        }
        
        [Fact]
        public void ShouldImplicitlyConvertToString()
        {
            Assert.True(string.Equals("C:\\".ToPath(), "C:"));
        }

        [Fact]
        public void ToStringOverride()
        {
            var sut = "C:\\".ToPath();
            
            Assert.Equal(sut.ToString(), "C:");
        }

        [Fact]
        public void GetHashCodeOverride()
        {
            var sut = "C:\\".ToPath();
            
            Assert.Equal(sut.GetHashCode(), "C:".GetHashCode());
        }
    }
}