## FlipDriveSDK ##

`Download:`[https://github.com/jamesheck2019/FlipDriveSDK/releases](https://github.com/jamesheck2019/FlipDriveSDK/releases)<br>
`NuGet:`
[![NuGet](https://img.shields.io/nuget/v/DeQmaTech.FlipDriveSDK.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/DeQmaTech.FlipDriveSDK)<br>

**Features**
* Assemblies for .NET 4.5.2 and .NET Standard 2.0 and .NET Core 2.1
* Just one external reference (Newtonsoft.Json)
* Easy installation using NuGet
* Upload/Download tracking support
* Proxy Support
* Upload/Download cancellation support

# Functions:
* UserInfo
* List
* Upload
* CreateNewFolder
* Rename
* Move
* Trash
* Delete
* DownloadAsStream
* Download
* FileThumbnail
* AddToFavorite
* RemoveFromFavorite
* ListRecents
* ListFavorites
* ListRecycleBin
* EmptyRecycleBin
* Search


# Example:
```vb.net
    Sub SetClient()
        Dim MyClient As FlipDriveSDK.IClient = New FlipDriveSDK.FClient("user", "pass")
    End Sub
```
```vb.net
    Sub SetClientWithOptions()
        Dim Optians As New FlipDriveSDK.ConnectionSettings With {.CloseConnection = True, .TimeOut = TimeSpan.FromMinutes(30), .Proxy = New FlipDriveSDK.ProxyConfig With {.ProxyIP = "172.0.0.0", .ProxyPort = 80, .ProxyUsername = "myname", .ProxyPassword = "myPass", .SetProxy = True}}
        Dim MyClient As FlipDriveSDK.IClient = New FlipDriveSDK.FClient("tkn.apiUrl", "tkn.authorizationToken", Optians)
    End Sub
```
```vb.net
    Async Sub ListFilesAndFolders()
        Dim result = Await MyClient.List("folder id / root=null", FilterEnum.all, SortEnum.Name, 500)
        For Each vid In result.data.items
            DataGridView1.Rows.Add(vid.name, vid.ID, vid.link, vid.size)
        Next
    End Sub
```
```vb.net
    Async Sub DeleteFileOrFolder()
        Dim result = Await MyClient.Delete(New List(Of String) From {"file or folder id"})
    End Sub
```
```vb.net
    Async Sub MoveFileOrFolder()
        Dim result = Await MyClient.Move(New List(Of String) From {"file or folder id"}, "folder id")
    End Sub
```
```vb.net
    Async Sub CreateNewFolder()
        Dim result = Await MyClient.CreateNewFolder("parent folder id", "new folder name")
    End Sub
```
```vb.net
    Async Sub RenameFileOrFolder()
        Dim result = Await MyClient.Rename("file or folder id id", "new name")
    End Sub
```
```vb.net
    Async Sub Search()
        Dim result = Await MyClient.Search("keyword", FilterEnum.all, SortEnum.Name, 600)
    End Sub
```
```vb.net
    Async Sub Upload_Local_WithProgressTracking()
        Dim UploadCancellationToken As New Threading.CancellationTokenSource()
        Dim _ReportCls As New Progress(Of FlipDriveSDK.ReportStatus)(Sub(ReportClass As FlipDriveSDK.ReportStatus)
                                                                         Label1.Text = String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes))
                                                                         ProgressBar1.Value = CInt(ReportClass.ProgressPercentage)
                                                                         Label2.Text = CStr(ReportClass.TextStatus)
                                                                     End Sub)
        Await MyClient.Upload("J:\DB\myvideo.mp4", UploadTypes.FilePath, "folder id", "myvideo.mp4", _ReportCls, UploadCancellationToken.Token)
    End Sub
```
```vb.net
    Async Sub Download_File_WithProgressTracking()
        Dim DownloadCancellationToken As New Threading.CancellationTokenSource()
        Dim _ReportCls As New Progress(Of FlipDriveSDK.ReportStatus)(Sub(ReportClass As FlipDriveSDK.ReportStatus)
                                                                         Label1.Text = String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes))
                                                                         ProgressBar1.Value = CInt(ReportClass.ProgressPercentage)
                                                                         Label2.Text = CStr(ReportClass.TextStatus)
                                                                     End Sub)
        Await MyClient.Download("file id", "J:\DB\", "myvideo.mp4", _ReportCls, DownloadCancellationToken.Token)
    End Sub
```
```vb.net
    Async Sub AddFileToFavorite()
        Dim result = Await MyClient.AddToFavorite(New List(Of String) From {"file/folder id"})
    End Sub
```
```vb.net
    Async Sub RemoveFileFromFavorite()
        Dim result = Await MyClient.RemoveFromFavorite(New List(Of String) From {"file/folder id"})
    End Sub
```
```vb.net
    Async Sub ListFavorites()
        Dim result = Await MyClient.ListFavorites(FilterEnum.all, SortEnum.Name, 200)
        For Each vid In result.data.items
            DataGridView1.Rows.Add(vid.name, vid.ID, vid.link, vid.size)
        Next
    End Sub
```
