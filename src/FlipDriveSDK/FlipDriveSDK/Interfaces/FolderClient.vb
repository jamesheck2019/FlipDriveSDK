Imports Newtonsoft.Json.Linq
Imports FlipDriveSDK.JSON
Imports Newtonsoft.Json
Imports FlipDriveSDK.utilitiez

Public Class FolderClient
    Implements IFolder

    Private Property FolderID As String

    Sub New(FolderID As String)
        Me.FolderID = FolderID
    End Sub
    Sub New()
    End Sub


    Public ReadOnly Property Multiple(FolderIDs As String()) As IFolders Implements IFolder.Multiple
        Get
            Return New FoldersClient(FolderIDs)
        End Get
    End Property


#Region "List"
    Public Async Function GET_ListDrives(Filter As FilterEnum, Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList) Implements IFolder.List
        Dim parameters = New ADictionary
        parameters.Add("parent_hash", If(FolderID, Nothing))
        parameters.Add("section", DriveTypeEnum.main.ToString)
        parameters.Add("show", Filter.ToString)
        parameters.Add("sort", utilitiez.stringValueOf(Sort))
        parameters.Add("count", Limit)
        parameters.Add("offset", OffSet)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/list", parameters)
            Using resPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await resPonse.Content.ReadAsStringAsync()

                If resPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                    Dim fin As New JSON_FolderList
                    fin.total = result.Jobj.SelectToken("data").Value(Of Integer)("total")
                    fin.total_files = result.Jobj.SelectToken("data").Value(Of Integer)("total_files")
                    fin.total_folders = result.Jobj.SelectToken("data").Value(Of Integer)("total_folders")
                    fin.name = result.Jobj.SelectToken("data.name").ToString
                    fin.total_returned = result.Jobj("data").SelectTokens("$.items[*]").Count
                    'Dim res = result.Jobj("data").SelectTokens("$.items[?(@.type == 'file')]").Select(Function(f) JsonConvert.DeserializeObject(Of JSON_FileMetadata)(f.ToString, JSONhandler))
                    fin._Files = result.Jobj("data").SelectTokens("$.items[?(@.type == 'file')]").Select(Function(f) JsonConvert.DeserializeObject(Of JSON_FileMetadata)(f.ToString, JSONhandler)).ToList()
                    fin._Folders = result.Jobj("data").SelectTokens("$.items[?(@.type == 'folder')]").Select(Function(f) JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(f.ToString, JSONhandler)).ToList()
                    Return fin
                Else
                    Throw ExceptionCls.CreateException(JObject.Parse(result)("info")("type").ToString, resPonse.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ListFiles"
    Public Async Function _ListFiles(Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of List(Of JSON_FileMetadata)) Implements IFolder.ListFiles
        Dim parameters = New ADictionary
        parameters.Add("parent_hash", If(FolderID, Nothing))
        parameters.Add("section", DriveTypeEnum.main.ToString)
        parameters.Add("show", FilterEnum.files.ToString)
        parameters.Add("sort", utilitiez.stringValueOf(Sort))
        parameters.Add("count", Limit)
        parameters.Add("offset", OffSet)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/list", parameters)
            Using resPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await resPonse.Content.ReadAsStringAsync()

                If resPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                    Return result.Jobj("data").SelectTokens("$.items[?(@.type == 'file')]").Select(Function(f) JsonConvert.DeserializeObject(Of JSON_FileMetadata)(f.ToString, JSONhandler)).ToList()
                Else
                    Throw ExceptionCls.CreateException(JObject.Parse(result)("info")("type").ToString, resPonse.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ListFolders"
    Public Async Function _ListFolders(Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of List(Of JSON_FolderMetadata)) Implements IFolder.ListFolders
        Dim parameters = New ADictionary
        parameters.Add("parent_hash", If(FolderID, Nothing))
        parameters.Add("section", DriveTypeEnum.main.ToString)
        parameters.Add("sort", utilitiez.stringValueOf(Sort))
        parameters.Add("count", Limit)
        parameters.Add("offset", OffSet)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/list", parameters)
            Using resPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await resPonse.Content.ReadAsStringAsync()

                If resPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                    Return result.Jobj("data").SelectTokens("$.items[?(@.type == 'folder')]").Select(Function(f) JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(f.ToString, JSONhandler)).ToList()
                Else
                    Throw ExceptionCls.CreateException(JObject.Parse(result)("info")("type").ToString, resPonse.StatusCode)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "Metadate" 'tested
    Public Async Function _Metadate() As Task(Of JSON_FolderMetadata) Implements IFolder.Metadate
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/info" + AsQueryString(New ADictionary From {{"resource_hash", FolderID}}))
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(result.Jobj.SelectToken("data.resource").ToString, JSONhandler)
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Delete" 'tested
    Public Async Function _Delete() As Task(Of Boolean) Implements IFolder.Delete
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FolderID}).Delete
    End Function
#End Region

#Region "Trash" 'tested
    Public Async Function _Trash() As Task(Of Boolean) Implements IFolder.Trash
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FolderID}).Delete
    End Function
#End Region

#Region "Exists" 'tested
    Public Async Function _Exists() As Task(Of Dictionary(Of String, Boolean)) Implements IFolder.Exists
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FolderID}).Exists
    End Function
#End Region

#Region "Move" 'tested
    Public Async Function _Move(DestinationFolderID As String) As Task(Of Boolean) Implements IFolder.Move
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FolderID}).Move(DestinationFolderID)
    End Function
#End Region

#Region "Rename" 'tested
    Public Async Function _Rename(NewName As String) As Task(Of Boolean) Implements IFolder.Rename
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File(FolderID).Rename(NewName)
    End Function
#End Region

#Region "CreateNewFolder" 'tested
    Public Async Function POST_CreateNewFolder(FolderName As String) As Task(Of JSON_FolderMetadata) Implements IFolder.Create
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("folder", New ADictionary))
            HtpReqMessage.Content = New Net.Http.FormUrlEncodedContent(New Dictionary(Of String, String) From {{"parent_hash", FolderID}, {"name", FolderName}})
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return JsonConvert.DeserializeObject(Of JSON_FolderMetadata)(result.Jobj.SelectToken("data").ToString, JSONhandler)
            Else
                Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
            End If
        End Using
    End Function
#End Region

#Region "UploadFile"
#Region "GetUploadHash"
    Private Async Function GetUploadHash(FileName As String, FileSize As Integer) As Task(Of String)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("parent_hash", If(FolderID, Nothing))
        parameters.Add("name", FileName)
        parameters.Add("size", FileSize)
        Dim encodedContent = New Net.Http.FormUrlEncodedContent(parameters)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            HtpReqMessage.RequestUri = New pUri("file/chunk_upload/entry", New ADictionary)
            HtpReqMessage.Content = encodedContent
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.hash").ToString
            Else
                Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
            End If
        End Using
    End Function
#End Region

    Public Async Function Get_UploadLocal(FileToUpload As Object, UploadType As UploadTypes, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_FileMetadata) Implements IFolder.Upload
        ReportCls = If(ReportCls, New Progress(Of ReportStatus))
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpSendProgress, (Function(sender, e)
                                                              ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Uploading..."})
                                                          End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            Dim MultipartsformData = New Net.Http.MultipartFormDataContent()
            Dim streamContent As Net.Http.StreamContent
            Select Case UploadType
                Case utilitiez.UploadTypes.FilePath
                    streamContent = New Net.Http.StreamContent(New IO.FileStream(FileToUpload, IO.FileMode.Open, IO.FileAccess.Read))
                Case utilitiez.UploadTypes.Stream
                    streamContent = New Net.Http.StreamContent(CType(FileToUpload, IO.Stream))
                Case utilitiez.UploadTypes.BytesArry
                    streamContent = New Net.Http.StreamContent(New IO.MemoryStream(CType(FileToUpload, Byte())))
            End Select
            Dim Filehash = Await GetUploadHash(FileName, (Await streamContent.ReadAsByteArrayAsync).Length)
            MultipartsformData.Add(New Net.Http.StringContent(0), """offset""")
            MultipartsformData.Add(New Net.Http.StringContent(Filehash), """file_hash""")
            MultipartsformData.Add(streamContent, "file", FileName)
            HtpReqMessage.Content = MultipartsformData
            HtpReqMessage.RequestUri = New pUri("file/chunk_upload/data", New ADictionary)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)
                Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

                token.ThrowIfCancellationRequested()
                If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = String.Format("[{0}] Uploaded successfully", FileName)})
                    Return JsonConvert.DeserializeObject(Of JSON_FileMetadata)(result.Jobj.SelectToken("data.resource").ToString, JSONhandler)
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = String.Format("The request returned with HTTP status code {0}", JObject.Parse(result)("info")("type").ToString)})
                    Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
                End If
            End Using
        Catch ex As Exception
            ReportCls.Report(New ReportStatus With {.Finished = True})
            If ex.Message.ToString.ToLower.Contains("a task was canceled") Then
                ReportCls.Report(New ReportStatus With {.TextStatus = ex.Message})
            Else
                Throw ExceptionCls.CreateException(ex.Message, ex.Message)
            End If
        End Try
    End Function
#End Region

#Region "EditDescription"
    Public Async Function _EditDescription(Description As String) As Task(Of Boolean) Implements IFolder.EditDescription
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File(FolderID).EditDescription(Description)
    End Function
#End Region

End Class
