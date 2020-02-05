## FlipDriveSDK ##

`Download:`[https://github.com/jamesheck2019/FlipDriveSDK/releases](https://github.com/jamesheck2019/FlipDriveSDK/releases)<br>
`NuGet:`
[![NuGet](https://img.shields.io/nuget/v/DeQmaTech.FlipDriveSDK.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/DeQmaTech.FlipDriveSDK)<br>
`Help:`
[https://github.com/jamesheck2019/FlipDriveSDK/wiki](https://github.com/jamesheck2019/FlipDriveSDK/wiki)<br>


# Features
* Assemblies for .NET 4.5.2 and .NET Standard 2.0 and .NET Core 2.1
* Just one external reference (Newtonsoft.Json)
* Easy installation using NuGet
* Upload/Download tracking support
* Proxy Support
* Upload/Download cancellation support

# List of functions:
**Token**
1. Token
1. Signup

**Main Drive**
1. DriveInfo
1. UserInfo
1. ListRecents
1. ListFavorites
1. ListRecycleBin
1. EmptyRecycleBin
1. Search

**Files**
1. RemoveFromFavorite
1. AddToFavorite
1. Move
1. Exists
1. UnTrash
1. Trash
1. Delete

**File**
1. Thumbnail
1. DownloadAsStream
1. Download
1. Rename
1. DirectUrl
1. Move
1. UnTrash
1. ImagePreview
1. Trash
1. ImagePreviewUrl
1. Exists
1. Delete
1. Path
1. Metadate
1. EditDescription

**Folders**
1. RemoveFromFavorite
1. AddToFavorite
1. Move
1. Exists
1. UnTrash
1. Trash
1. Delete

**Folder**
1. Upload
1. Create
1. Rename
1. Move
1. Exists
1. Trash
1. Delete
1. Metadate
1. List
1. ListFiles
1. ListFolders
1. EditDescription


# CodeMap:
![codemap](https://www.mediafire.com/convkey/b1fe74b8/uea8kj65tiuotwqzg.jpg)


# Code simple:
```vb.net
''create account
Dim acc = FlipDriveSDK.GetToken.Signup("username", "email", "pass")
'first get auth token (one time only)
Dim tokn = Await FlipDriveSDK.GetToken.Token("your_email", "your_password")
''set proxy and connection options
Dim con As New FlipDriveSDK.ConnectionSettings With {.CloseConnection = True, .TimeOut = TimeSpan.FromMinutes(30), .Proxy = New FlipDriveSDK.ProxyConfig With {.SetProxy = True, .ProxyIP = "127.0.0.1", .ProxyPort = 8888, .ProxyUsername = "user", .ProxyPassword = "pass"}}
''set api client
Dim CLNT As FlipDriveSDK.IClient = New FlipDriveSDK.FClient("xxxxxxxxxx", con)

''general
Await CLNT.UserInfo
Await CLNT.DriveInfo(DriveTypeEnum.main)
Await CLNT.EmptyRecycleBin
Await CLNT.ListFavorites(FilterEnum.all, SortEnum.Name, 50, 0)
Await CLNT.ListRecents(50)
Await CLNT.ListRecycleBin(FilterEnum.files, SortEnum.UpdatedDate, 50, 0)
Await CLNT.Search("emy", DriveTypeEnum.main, FilterEnum.folders, SortEnum.Size, 50, 0)

''file [Multiple]
Await CLNT.File.Multiple(New String() {"file_id", "file_id"}).AddToFavorite
Await CLNT.File.Multiple(New String() {"file_id", "file_id"}).Delete
Await CLNT.File.Multiple(New String() {"file_id", "file_id"}).Exists
Await CLNT.File.Multiple(New String() {"file_id", "file_id"}).Move("folder_id")
Await CLNT.File.Multiple(New String() {"file_id", "file_id"}).RemoveFromFavorite
Await CLNT.File.Multiple(New String() {"file_id", "file_id"}).Trash
Await CLNT.File.Multiple(New String() {"file_id", "file_id"}).UnTrash
''file [Single]
Await CLNT.File("file_id").Delete
Await CLNT.File("file_id").DirectUrl
Dim cts As New Threading.CancellationTokenSource()
Dim _ReportCls As New Progress(Of FlipDriveSDK.ReportStatus)(Sub(ReportClass As FlipDriveSDK.ReportStatus) Console.WriteLine(String.Format("{0} - {1}% - {2}", String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes)), CInt(ReportClass.ProgressPercentage), ReportClass.TextStatus)))
Await CLNT.File("file_id").Download("c:\\downloads", "myvid.mp4", _ReportCls, cts.Token)
Await CLNT.File("file_id").DownloadAsStream(_ReportCls, cts.Token)
Await CLNT.File("file_id").Exists
Await CLNT.File("file_id").ImagePreview(_ReportCls, cts.Token)
CLNT.File("file_id").ImagePreviewUrl()
Await CLNT.File("file_id").Metadate
Await CLNT.File("file_id").Move("folder_id")
Await CLNT.File("file_id").Path
Await CLNT.File("file_id").Rename("newname.jpg")
Await CLNT.File("file_id").Thumbnail(New FlipDriveSDK.FileClient.Size With {.Width = 300, .Height = 200}, _ReportCls, cts.Token)
Await CLNT.File("file_id").Trash
Await CLNT.File("file_id").UnTrash

''folder [Multiple]
Await CLNT.Folder.Multiple(New String() {"folder_id", "folder_id"}).AddToFavorite
Await CLNT.Folder.Multiple(New String() {"folder_id", "folder_id"}).Delete
Await CLNT.Folder.Multiple(New String() {"folder_id", "folder_id"}).Exists
Await CLNT.Folder.Multiple(New String() {"folder_id", "folder_id"}).Move("folder_id")
Await CLNT.Folder.Multiple(New String() {"folder_id", "folder_id"}).RemoveFromFavorite
Await CLNT.Folder.Multiple(New String() {"folder_id", "folder_id"}).Trash
Await CLNT.Folder.Multiple(New String() {"folder_id", "folder_id"}).UnTrash
''folder [Single]
Await CLNT.Folder("folder_id").Create("new folder")
Await CLNT.Folder("folder_id").Delete
Await CLNT.Folder("folder_id").Exists
Await CLNT.Folder("folder_id").List(FilterEnum.all, SortEnum.Name, 50, 0)
Await CLNT.Folder("folder_id").ListFiles(SortEnum.Size, 50.0)
Await CLNT.Folder("folder_id").ListFolders(SortEnum.Name, 50, 0)
Await CLNT.Folder("folder_id").Metadate
Await CLNT.Folder("folder_id").Move("folder_id")
Await CLNT.Folder("folder_id").Rename("new folder name")
Await CLNT.Folder("folder_id").Trash
Await CLNT.Folder("folder_id").Upload("c:\\myvid.mp4", UploadTypes.FilePath, "myvid.mp4", _ReportCls, cts.Token)
```
