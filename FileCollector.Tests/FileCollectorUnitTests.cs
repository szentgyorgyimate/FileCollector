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
        private IFileSystem _fileSystemMock;
        private IDirectory _directoryMock;
        private IFile _fileMock;

        public FileCollectorUnitTests()
        {
            _fileSystemMock = Mock.Of<IFileSystem>();
            _directoryMock = Mock.Of<IDirectory>();
            _fileMock = Mock.Of<IFile>();
            
            Mock.Get(_fileSystemMock).Setup(fsm => fsm.Directory).Returns(_directoryMock);
            Mock.Get(_fileSystemMock).Setup(fsm => fsm.File).Returns(_fileMock);
        }

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
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, null, _fileSystemMock);

            // Assert
            Assert.Throws<ArgumentNullException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenSourceIsEmpty()
        {
            // Arrange
            var fileCollector = new Collector(string.Empty, Paths.FakeDestinationPath, _fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenDestinationIsEmpty()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, string.Empty, _fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenSourceIsWhiteSpaceOnly()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(" ", Paths.FakeDestinationPath, _fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenDestinationIsWhiteSpaceOnly()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, " ", _fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenSourceHasInvalidCharacters()
        {
            // Arrange
            char invalidCharacter = Path.GetInvalidPathChars()[0];
            var fileCollector = new Collector($"{invalidCharacter}", Paths.FakeDestinationPath, _fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_ArgumentExceptionWhenDestinationHasInvalidCharacters()
        {

            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            char invalidCharacter = Path.GetInvalidPathChars()[0];
            var fileCollector = new Collector(Paths.FakeSourcePath, $"{invalidCharacter}", _fileSystemMock);

            // Assert
            Assert.Throws<ArgumentException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Throws_DirectoryNotFoundExceptionWhenSourceDoesNotExist()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(false);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Assert
            Assert.Throws<DirectoryNotFoundException>(() => fileCollector.CollectFiles());
        }

        [Fact]
        public void CollectFiles_Copy_CalledWhenCollectOperationIsCopy()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(Paths.FakeDuplicateFilePath, It.IsAny<string>(), It.IsAny<bool>())).Verifiable();

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(_fileMock).Verify(f => f.Copy(Paths.FakeDuplicateFilePath, It.IsAny<string>(), fileCollector.Overwrite), Times.Once);
        }

        [Fact]
        public void CollectFiles_Move_CalledWhenCollectOperationIsMove()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Move(Paths.FakeDuplicateFilePath, It.IsAny<string>())).Verifiable();

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock)
            {
                CollectOperation = CollectOperation.Move
            };

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(_fileMock).Verify(f => f.Move(Paths.FakeDuplicateFilePath, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CollectFiles_Delete_CalledWhenCollectOperationIsMoveAndOverwriteIsTrue()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Exists(It.IsAny<string>())).Returns(true);
            Mock.Get(_fileMock).Setup(fm => fm.Delete(It.IsAny<string>())).Verifiable();

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock)
            {
                Overwrite = true,
                CollectOperation = CollectOperation.Move
            };

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(_fileMock).Verify(f => f.Delete(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void CollectFiles_Files_NotNullWhenSourceIsEmpty()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotNull(files);
        }

        [Fact]
        public void CollectFiles_Files_NotEmptyWhenSourceIsEmpty()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            
            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotEmpty(files);
        }

        [Fact]
        public void CollectFiles_DestinationDirectoryPath_DoesNotExistVerifyCreateDirectoryCalled()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeDestinationPath)).Returns(false);
            Mock.Get(_directoryMock).Setup(dm => dm.CreateDirectory(Paths.FakeDestinationPath)).Verifiable();

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Mock.Get(_directoryMock).Verify(d => d.CreateDirectory(Paths.FakeDestinationPath), Times.Once());
        }

        [Fact]
        public void CollectFiles_HasSubDirectories_True()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] {
                Paths.FakeFirstSubDirectoryPath,
                Paths.FakeSecondSubDirectoryPath
            });
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[0]);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[0].HasSubDirectories);
        }

        [Fact]
        public void CollectFiles_HasSubDirectories_False()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].HasSubDirectories);
        }

        [Fact]
        public void CollectFiles_IsSucceeded_TrueWhenEmptyFolder()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_IsSucceeded_FalseWhenGetFilesThrowsException()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws<Exception>();

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_Path_EqualsSourcePath()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(Paths.FakeSourcePath, files[0].Path);
        }

        [Fact]
        public void CollectFiles_ErrorType_NoneWhenSuccess()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.None, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_UnauthorizedWhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new UnauthorizedAccessException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unauthorized, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_PathTooLongWhenPathTooLongExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new PathTooLongException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.PathTooLong, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_NotFoundWhenDirectoryNotFoundExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new DirectoryNotFoundException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.NotFound, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_IOWhenIOExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new IOException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.IO, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorType_UnknownWhenExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new Exception());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unknown, files[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_ErrorMessage_NullWhenSuccess()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Null(files[0].ErrorMessage);
        }

        [Fact]
        public void CollectFiles_ErrorMessage_NotNullWhenGetFilesException()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Throws(new Exception());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotNull(files[0].ErrorMessage);
        }

        [Fact]
        public void CollectFiles_ProcessedFileCount_ZeroWhenEmptySource()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            
            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(0, files[0].ProcessedFileCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_ZeroWhenEmptySource()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(0, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_ProcessedFileCount_HasCorrectValue()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(1, files[0].ProcessedFileCount);
        }

        [Fact]
        public void CollectFiles_ProcessedFileCount_HasCorrectValueWhenIgnoredExtensionsSet()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath, Paths.FakeSecondFilePath });

            var ignoredExtensions = new List<string>() { Path.GetExtension(Paths.FakeSecondFilePath) };
            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, false, CollectOperation.Copy, ignoredExtensions, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(1, files[0].ProcessedFileCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_HasCorrectValue()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(1, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_HasCorrectValueWhenException()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<IOException>();

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(0, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_EqualProcessedFileCountWhenSuccess()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(files[0].ProcessedFileCount, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_NotEqualProcessedFileCountWhenException()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<IOException>();

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.NotEqual(files[0].ProcessedFileCount, files[0].SucceededCount);
        }

        [Fact]
        public void CollectFiles_SucceededCount_HasCorrectValueWhenIgnoredExtensionsSet()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath, Paths.FakeSecondFilePath });

            var ignoredExtensions = new List<string>() { Path.GetExtension(Paths.FakeSecondFilePath) };
            
            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);
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
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

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
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[0].FileCollectResults[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_IsSucceeded_FalseWhenFileOperationException()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new Exception());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].FileCollectResults[0].IsSucceeded);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_UnauthorizedWhenUnauthorizedAccessExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new UnauthorizedAccessException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unauthorized, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_PathTooLongWhenPathTooLongExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new PathTooLongException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.PathTooLong, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_NotFoundWhenFileNotFoundExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new FileNotFoundException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.NotFound, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_NotSupportedWhenNotSupportedExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new NotSupportedException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.NotSupported, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_IOWhenIOExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new IOException());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.IO, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_CollectErrorType_UnknownWhenExceptionThrown()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            Mock.Get(_fileMock).Setup(fm => fm.Copy(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws(new Exception());

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.Equal(CollectErrorType.Unknown, files[0].FileCollectResults[0].ErrorType);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_IsRenamed_False()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.False(files[0].FileCollectResults[0].IsRenamed);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_IsRenamed_TrueWhenDuplicate()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeFirstSubDirectoryPath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubDirectoryPath)).Returns(new string[0]);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeDuplicatedSubFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            Assert.True(files[1].FileCollectResults[0].IsRenamed);
        }

        [Fact]
        public void CollectFiles_FileCollectResults_NewFilePath_HasIndexWhenRenamed()
        {
            // Arrange
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeFirstSubDirectoryPath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubDirectoryPath)).Returns(new string[0]);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeDuplicatedSubFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

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
            Mock.Get(_directoryMock).Setup(dm => dm.Exists(Paths.FakeSourcePath)).Returns(true);
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeFirstSubDirectoryPath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeSourcePath)).Returns(new string[] { Paths.FakeDuplicateFilePath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeFirstSubSubDirectoryPath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubDirectoryPath)).Returns(new string[] { Paths.FakeDuplicatedSubFilePath });
            Mock.Get(_directoryMock).Setup(dm => dm.GetDirectories(Paths.FakeFirstSubSubDirectoryPath)).Returns(new string[0]);
            Mock.Get(_directoryMock).Setup(dm => dm.GetFiles(Paths.FakeFirstSubSubDirectoryPath)).Returns(new string[] { Paths.FakeFirstSubSubDuplicatedFilePath });

            var fileCollector = new Collector(Paths.FakeSourcePath, Paths.FakeDestinationPath, _fileSystemMock);

            // Act
            List<CollectResult> files = fileCollector.CollectFiles();

            // Assert
            var expected = $"{Paths.FakeDestinationPath}\\{Path.GetFileNameWithoutExtension(Paths.DuplicateFileName)}_(2){Path.GetExtension(Paths.DuplicateFileName)}";
            Assert.Equal(expected, files[2].FileCollectResults[0].NewFilePath);
        }
    }
}
