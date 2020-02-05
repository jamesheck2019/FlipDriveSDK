Imports FlipDriveSDK.utilitiez

Public Class Form1


    Async Function Tasks() As Task
        ''create account
        Dim acc = FlipDriveSDK.GetToken.Signup("username", "email", "pass")
        'first get auth token (one time only)
        Dim tokn = Await FlipDriveSDK.GetToken.Token("your_email", "your_password")
        ''set proxy and connection options
        Dim con As New FlipDriveSDK.ConnectionSettings With {.CloseConnection = True, .TimeOut = TimeSpan.FromMinutes(30), .Proxy = New FlipDriveSDK.ProxyConfig With {.SetProxy = True, .ProxyIP = "127.0.0.1", .ProxyPort = 8888, .ProxyUsername = "user", .ProxyPassword = "pass"}}
        ''set api client
        Dim CLNT As FlipDriveSDK.IClient = New FlipDriveSDK.FClient("xxxxxtokenxxxxx", con)

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
    End Function

    Private Sub Button3_Click(sender As Object, e As EventArgs)
        Dim interfaces As New List(Of System.Type)

        Dim FClient As New FlipDriveSDK.FClient("")
        interfaces.AddRange(FClient.GetType().GetInterfaces().ToList)

        Dim FilesClient As New FlipDriveSDK.FilesClient(New String() {})
        interfaces.AddRange(FilesClient.GetType().GetInterfaces().ToList)

        Dim FileClient As New FlipDriveSDK.FileClient(0)
        interfaces.AddRange(FileClient.GetType().GetInterfaces().ToList)

        Dim FoldersClient As New FlipDriveSDK.FoldersClient(New String() {})
        interfaces.AddRange(FoldersClient.GetType().GetInterfaces().ToList)

        Dim FolderClient As New FlipDriveSDK.FolderClient(0)
        interfaces.AddRange(FolderClient.GetType().GetInterfaces().ToList)

        Dim GetToken As New FlipDriveSDK.GetToken
        interfaces.AddRange(GetToken.GetType().GetInterfaces().ToList)

        For Each iface As Type In interfaces
            Dim methods = iface.GetMethods()

            For Each method As Reflection.MethodInfo In methods
                Dim methodName = method.Name
                DataGridView1.Rows.Add(methodName)
            Next
        Next
    End Sub
End Class
