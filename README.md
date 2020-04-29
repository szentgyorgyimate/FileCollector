# File collector
File collector is a tool for collect files from the given root directory and its subdirectories into one specific directory.

## Motivation
I had a lot of old pictures and various files buried in folders and their subfolders (categorised by year, month, etc.), and I wanted to collect them all in one place to easily save them to the cloud from there. That was my main motivation to write this tool.

## Table of contents
1. [Basic usage](#basic-usage)
2. [Configuration](#configuration)
3. [CollectFiles() method](#collectfiles-method)
4. [Result of the collect operation](#result-of-the-collect-operation)
   - [CollectResultBase class](#collectresultbase-class) 
   - [CollectResult class](#collectresult-class)
   - [FileCollectResult class](#filecollectresult-class)

## Basic usage
1. Add `using FileCollector`.
2. Instantiate the `Collector` class. The simplest constructor takes 2 arguments. The source path and the destination path.
3. Call the [`CollectFiles()`](#collectfiles-method) method on your newly created instance.
4. That's all. Enjoy the result!

``` C#
using FileCollector;
...
var collector = new Collector("c:\\source", "c:\\destination");
List<CollectResult> result = collector.CollectFiles();
```

## Configuration
The parameters using by the [`CollectFiles()`](#collectfiles-method) method can be set via constructor and properties. The `Collector` class has the following properties.

### SourceRootPath
The path where the files are collected from. The files are collected from the given directory and its subdirectories. If it is not a valid path, the [`CollectFiles()`](#collectfiles-method) method can throw `ArgumentNullException`, `ArgumentException`, or `DirectoryNotFoundException`.

### DestinationDirectoryPath
The path of the directory where the files will be copied or moved. If it is not a valid path, the [`CollectFiles()`](#collectfiles-method) method can throw `ArgumentNullException` or `ArgumentException`.

### Overwrite
If it is `true`, the [`CollectFiles()`](#collectfiles-method) method overwrites the files if they already exists in the destination directory. If it is `true` and the [`CollectOperation`](#collectoperation) is `Move`, first it deletes the existing files before moving the new ones. It is `false` by default.

### CollectOperation
Specifies the type of the collect operation. It can be `Copy` and `Move`. It is `Copy` by default.

### ExtensionsToIgnore
A list of file extensions to ignore when copying or moving files.

#### Example
``` C#
using FileCollector;
...
var collector = new Collector("c:\\source", "c:\\destination")
collector.ExtensionsToIgnore = new List<string>() { ".bin", ".exe" };

List<CollectResult> result = collector.CollectFiles();
```

## CollectFiles() method
The [FileCollector](#file-collector) has one method only and it is the `CollectFiles()`. It does the following steps.
1. Checks if the [`SourceRootPath`](#sourcerootpath) is valid path and exists. If not, it throws the appropriate exception.
2. Checks if the [`DestinationDirectoryPath`](#destinationdirectorypath) is valid path. If not, it throws the appropriate exception. If the specified directory does not exist, it will try to create it.
3. Crawls through the source directory and its subdirectories and tries to copy or move (depends on the [`CollectOperation`](#collectoperation)) the files from there to the destination directory. Meanwhile it builds up the list of [result](#collectresult-class) objects.
4. Returns a list of [`CollectResult`](#collectresult-class) objects.

## Result of the collect operation

### CollectResultBase class
The `CollectResultBase` class is the base class of the [`CollectResult`](#collectresult-class) and the [`FileCollectResult`](#filecollectresult-class) classes. It has the following properties.

#### Path
The path of the processed directory or file.

#### IsSucceeded
It indicates whether the directory or file collect operation is succeeded.

#### ErrorType
The type of the error which occured during the directory or file collect operation. It is `None` by default.

#### ErrorMessage
The message of the error (if any happens). Its value is the message of the exception which has been thrown during the directory or file collect operation.

### CollectResult class
The `CollectResult` object is the result of a directory collect operation. It is derived from the [`CollectResultBase`](#collectresultbase-class) and has the following properties.

#### FileCollectResults
The list of the collect results of the files which sit in the processed directory. It contains [`FileCollectResult`](#filecollectresult-class) objects.

#### HasSubDirectories
It indicates whether the processed directory has subdirectories.

#### ProcessedFileCount
The processed file count of the processed directory. It is readonly.

#### SucceededCount
The succeeded file operation count of the processed directory. It is readonly.

### FileCollectResult class
The `FileCollectResult` object is the result of a file collect operation. It is derived from the [`CollectResultBase`](#collectresultbase-class) and has the following properties.

#### NewFilePath
The new path of the copied or moved file.

#### IsRenamed
It indicates whether the file has been renamed. When two or more files share the same name and extension, every duplication will be renamed in the destination directory.

#### Example
Consider the following directory structure.

```
├── ...
├── destination
├── source
│   ├── duplicate.txt
│   └── subdir
│   	└── duplicate.txt
└── ...
```

And the following code.

``` C#
using FileCollector;
...
var collector = new Collector("c:\\source", "c:\\destination");
List<CollectResult> result = collector.CollectFiles();

// Source directory
CollectResult sourceDirResult = result[0];
string sourceDirPath = sourceDirResult.Path // "c:\source\"

CollectFileResult fileInSourceResult = sourceDirResult.FileCollectResults[0];
string fileInSourcePath = fileInSourceResult.Path // "c:\source\duplicate.txt"
string fileInSourceNewPath = fileInSourceResult.NewFilePath // "c:\destination\duplicate.txt"

// Sub directory
CollectResult subDirResult = result[1];
string subDirPath = subDirResult.Path // "c:\source\subdir"

CollectFileResult fileInSubDirResult = subDirResult.FileCollectResults[0];
string fileInSubDirPath = fileInSubDirResult.Path // "c:\source\subdir\duplicate.txt"
string fileInSubDirNewPath = fileInSubDirResult.NewFilePath // "c:\destination\duplicate_(1).txt"

```

The `NewFilePath` of the `duplicate.txt` file which sits in the `c:\source\subdir` directory will be `"c:\destination\duplicate_(1).txt"` and its `IsRenamed` property will be `true`. 
