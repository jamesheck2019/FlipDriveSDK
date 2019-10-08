## FlipDriveSDK ##

`Download:`[https://github.com/jamesheck2019/FlipDriveSDK/releases](https://github.com/jamesheck2019/FlipDriveSDK/releases)<br>
`NuGet:`
[![NuGet](https://img.shields.io/nuget/v/DeQmaTech.FlipDriveSDK.svg?style=flat-square&logo=nuget)](https://www.nuget.org/packages/DeQmaTech.FlipDriveSDK)<br>

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
**get access token**
```vb.net
Dim rslt = FlipDriveSDK.GetToken.Get_Token("username", "password")
```

**set client**
```vb.net
Dim cLENT As FlipDriveSDK.IClient = New FlipDriveSDK.FClient("token", Nothing, TimeSpan.FromMinutes(60), False)
OR
Dim cLENT As FlipDriveSDK.IClient = New FlipDriveSDK.FClient("username", "password", Nothing, TimeSpan.FromMinutes(60), False)
```

**set client with proxy**
```vb.net
Dim roxy = New FlipDriveSDK.ProxyConfig With {.ProxyIP = "172.0.0.0", .ProxyPort = 80, .ProxyUsername = "myname", .ProxyPassword = "myPass", .SetProxy = true}
Dim cLENT As FlipDriveSDK.IClient = New FlipDriveSDK.FClient("username", "password", roxy, TimeSpan.FromMinutes(60), False)
```

**List**
```vb.net
Dim RSLT = Await cLENT.List("", FlipDriveSDK.utilitiez.FilterEnum.all, FlipDriveSDK.utilitiez.SortEnum.Name, 1000)
For Each fold In RSLT.data.items
    DataGridView1.Rows.Add(fold.ID, fold.name, fold.link, fold.extension, fold.FileOrFolder, fold.direct, ISisFunctions.Bytes_To_KbMbGb.SetBytes(fold.size), fold.link)
Next
```

**create new Folder**
```vb.net
Dim RSLT = Await cLENT.CreateNewFolder("parent folder id", "gogo2")
```

**delete file/folder**
```vb.net
Dim RSLT = Await cLENT.Delete("file/folder ID")
```

**upload local file with progress tracking**
```vb.net
Try
Dim UploadCancellationToken As New Threading.CancellationTokenSource()
Dim _ReportCls As New Progress(Of FlipDriveSDK.ReportStatus)(Sub(ReportClass As FlipDriveSDK.ReportStatus)
                      Label1.Text = String.Format("{0}/{1}", (ReportClass.BytesTransferred),(ReportClass.TotalBytes))
                      ProgressBar1.Value = CInt(ReportClass.ProgressPercentage)
                      Label2.Text = CStr(ReportClass.TextStatus)
                      End Sub)
Dim RSLT = Await cLENT.Upload("J:\VB.jpg", UploadTypes.FilePath, "folder id", "VB.jpg", _ReportCls, UploadCancellationToken.Token)
DataGridView1.Rows.Add(RSLT.name)
Catch ex As FlipDriveSDK.PushbulletException
      MsgBox(ex.Message)
End Try
```

**download file with progress tracking**
```vb.net
Dim UploadCancellationToken As New Threading.CancellationTokenSource()
Dim _ReportCls As New Progress(Of FlipDriveSDK.ReportStatus)(Sub(ReportClass As FlipDriveSDK.ReportStatus)
               Label1.Text = String.Format("{0}/{1}", (ReportClass.BytesTransferred), (ReportClass.TotalBytes))
               ProgressBar1.Value = CInt(ReportClass.ProgressPercentage)
               Label2.Text =  CStr(ReportClass.TextStatus)
               End Sub)
Await cLENT.Download("file id", "J:\", "helo.jpg", _ReportCls, UploadCancellationToken.Token)
```
