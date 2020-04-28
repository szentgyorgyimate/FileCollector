using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using FileCollector.Enums;
using FileCollector.Results;
using FileCollector.Tests.Helpers;
using Moq;
using Xunit;

namespace FileCollector.Tests
{
    public class FileCollectorUnitTests
    {
        [Fact]
        public void CollectFiles_Throws_ArgumentNullExceptionWhenSourceIsNull()
        {
            // Arrange
            var fileCollector = new Collector(null, Paths.FakeDestinationPath, Mock.Of<IFileSystem>());

            // Assert
            Assert.Throws<ArgumentNullException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentNullExceptionWhenDestinationIsNull()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, null, fileSystemMock);

            // Assert
            Assert.Throws<ArgumentNullException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenSourceIsEmpty()
        {
            // Arrange
            var fileCollector = new Collector(string.Empty, Paths.FakeDestinationPath, Mock.Of<IFileSystem>());

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenDestinationIsEmpty()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, string.Empty, fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenSourceIsWhiteSpaceOnly()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(" ", Paths.FakeDestinationPath, fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenDestinationIsWhiteSpaceOnly()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, " ", fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenSourceHasInvalidCharacters()
        {
            // Arrange
            char invalidCharacter = Path.GetInvalidPathChars()[0];
            var fileCollector = new Collector($"{invalidCharacter}", Paths.FakeDestinationPath, Mock.Of<IFileSystem>());

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenDestinationHasInvalidCharacters()
        {

            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            char invalidCharacter = Path.GetInvalidPathChars()[0];
            var fileCollector = new Collector(Paths.FakeSourcePath, $"{invalidCharacter}", fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_DirectoryNotFoundExceptionWhenSourceDoesNotExist()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(false);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Assert
            Assert.Throws<DirectoryNotFoundException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Copy_CalledWhenCollectOperationIsCopy()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(Paths.FakeDuplicateFilePath, It.IsAny<string>(), It.IsAny<bool>())).Verifiable();

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);
           
            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(fileMock).Verify(f => f.Copy(Paths.FakeDuplicateFilePath, It.IsAny<string>(), fileCollector.Overwrite), Times.Once);
        }

        [Fact]
        public void CollectFiles_Move_CalledWhenCollectOperationIsMove()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Move(Paths.FakeDuplicateFilePath, It.IsAny<string>())).Verifiable();

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock)
            {
                CollectOperation = CollectOperation.Move
            };

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(fileMock).Verify(f => f.Move(Paths.FakeDuplicateFilePath, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CollectFiles_Delete_CalledWhenCollectOperationIsMoveAndOverwriteIsTrue()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Exists(It.IsAny<string>())).Returns(true);
            Mock.Get(fileMock).Setup(fm => fm.Delete(It.IsAny<string>())).Verifiable();

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock)
            {
                Overwrite = true,
                CollectOperation = CollectOperation.Move
            };

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(fileMock).Verify(f => f.Delete(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CollectFiles_Files_NotNullWhenSourceIsEmpty()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotNull(files);
        }

        [Fact]
        public void CollectFiles_Files_NotEmptyWhenSourceIsEmpty()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            
            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotEmpty(files);
        }

        [Fact]
        public void CollectFiles_DestinationDirectoryPath_DoesNotExistVerifyCreateDirectoryCalled()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeDestinationPath)).Returns(false);
            Mock.Get(directoryMock).Setup(dm => dm.CreateDirectory(Paths.FakeDestinationPath)).Verifiable();

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(directoryMock).Verify(d => d.CreateDirectory(Paths.FakeDestinationPath), Times.Once());
        }

        [Fact]
        public void CollectFiles_HasSubDirectories_True()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] {
                Paths.FakeFirstSubDirectoryPath,
                Paths.FakeSecondSubDirectoryPath
            });
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[0]);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[0].HasSubDirectories);
        }

        [Fact]
        public void CollectFiles_HasSubDirectories_False()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].HasSubDirectories);
        }

        [Fact]
        public void CollectFiles_IsSucceeded_TrueWhenEmptyFolder()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_IsSucceeded_FalseWhenGetFilesThrowsException()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws<Exception>();

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_Path_EqualsSourcePath()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(Paths.FakeSourcePath, files[0].Path);
        }

        [Fact]
        public void CollectFiles_ErrorType_NoneWhenSuccess()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.None, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_UnauthorizedWhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new UnauthorizedAccessException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unauthorized, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_PathTooLongWhenPathTooLongExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new PathTooLongException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.PathTooLong, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_NotFoundWhenDirectoryNotFoundExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new DirectoryNotFoundException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.NotFound, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_IOWhenIOExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new IOException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.IO, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_UnknownWhenExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new Exception());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unknown, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorMessage_NullWhenSuccess()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Null(files[0].ErrorMessage);
        }

        [Fact]
        public void CollectFiles_ErrorMessage_NotNullWhenGetFilesException()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new Exception());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotNull(files[0].ErrorMessage);
        }

        [Fact]
        public void CollectFiles_ProcessedFileCount_ZeroWhenEmptySource()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            
            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(0, files[0].ProcessedFileCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_ZeroWhenEmptySource()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(0, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_ProcessedFileCount_HasCorrectValue()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(1, files[0].ProcessedFileCount);
        }

        [Fact]
        public void CollectFiles_ProcessedFileCount_HasCorrectValueWhenIgnoredExtensionsSet()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath, Paths.FakeSecondFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var ignoredExtensions = new List<string>() { Path.GetExtension(Paths.FakeSecondFilePath) };
            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, false, CollectOperation.Copy, ignoredExtensions, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(1, files[0].ProcessedFileCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_HasCorrectValue()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(1, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_HasCorrectValueWhenException()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<IOException>();

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(0, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_EqualProcessedFileCountWhenSuccess()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(files[0].ProcessedFileCount, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_NotEqualProcessedFileCountWhenException()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<IOException>();

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotEqual(files[0].ProcessedFileCount, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_HasCorrectValueWhenIgnoredExtensionsSet()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath, Paths.FakeSecondFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var ignoredExtensions = new List<string>() { Path.GetExtension(Paths.FakeSecondFilePath) };
            
            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);
            fileCollector.ExtensionsToIgnore = ignoredExtensions;

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(1, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_NewFilePath_IsDestinationPlusFileName()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            var expected = $"{Paths.FakeDestinationPath}\\{Paths.DuplicateFileName}";

            Assert.Equal(expected, files[0].FileCollectResults[0].NewFilePath);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_IsSucceeded_True()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[0].FileCollectResults[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_IsSucceeded_FalseWhenFileOperationException()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].FileCollectResults[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_UnauthorizedWhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new UnauthorizedAccessException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unauthorized, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_PathTooLongWhenPathTooLongExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new PathTooLongException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.PathTooLong, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_NotFoundWhenFileNotFoundExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new FileNotFoundException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.NotFound, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_NotSupportedWhenNotSupportedExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new NotSupportedException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.NotSupported, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_IOWhenIOExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new IOException());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.IO, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_UnknownWhenExceptionThrown()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileMock = Mock.Of<IFile>();
            Mock.Get(fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new Exception());

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(fileMock);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unknown, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_IsRenamed_False()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].FileCollectResults[0].IsRenamed);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_IsRenamed_TrueWhenDuplicate()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeFirstSubDirectoryPath });
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubDirectoryPath)).Returns(new string[0]);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeDuplicatedSubFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[1].FileCollectResults[0].IsRenamed);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_NewFilePath_HasIndexWhenRenamed()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeFirstSubDirectoryPath });
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubDirectoryPath)).Returns(new string[0]);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeDuplicatedSubFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            var expected = $"{Paths.FakeDestinationPath}\\{Path.GetFileNameWithoutExtension(Paths.DuplicateFileName)}_(1){Path.GetExtension(Paths.DuplicateFileName)}";
            Assert.Equal(expected, files[1].FileCollectResults[0].NewFilePath);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_NewFilePath_HasCorrectIndexWhenMultipleRename()
        {
            // Arrange
            var directoryMock = Mock.Of<IDirectory>();
            Mock.Get(directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeFirstSubDirectoryPath });
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeFirstSubSubDirectoryPath });
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeDuplicatedSubFilePath });
            Mock.Get(directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubSubDirectoryPath)).Returns(new string[0]);
            Mock.Get(directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubSubDirectoryPath)).Returns(new string[] { Paths.FakeFirstSubSubDuplicatedFilePath });

            var fileSystemMock = Mock.Of<IFileSystem>();
            Mock.Get(fileSystemMock).Setup(fsm => fsm.Directory).Returns(directoryMock);
            Mock.Get(fileSystemMock).Setup(fsm => fsm.File).Returns(Mock.Of<IFile>());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            var expected = $"{Paths.FakeDestinationPath}\\{Path.GetFileNameWithoutExtension(Paths.DuplicateFileName)}_(2){Path.GetExtension(Paths.DuplicateFileName)}";
            Assert.Equal(expected, files[2].FileCollectResults[0].NewFilePath);
        }
    }
}
