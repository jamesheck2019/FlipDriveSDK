Imports Newtonsoft.Json.Linq
Imports FlipDriveSDK.JSON
Imports Newtonsoft.Json
Imports FlipDriveSDK.utilitiez

Public Class FileClient
    Implements IFile

    Private Property FileID As String

    Sub New(FileID As String)
        Me.FileID = FileID
    End Sub
    Sub New()
    End Sub


    Public ReadOnly Property Multiple(FileIDs As String()) As IFiles Implements IFile.Multiple
        Get
            Return New FilesClient(FileIDs)
        End Get
    End Property


#Region "Metadate" 'tested
    Public Async Function _Metadate() As Task(Of JSON_FileMetadata) Implements IFile.Metadate
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/info" + AsQueryString(New ADictionary From {{"resource_hash", FileID}}))
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return JsonConvert.DeserializeObject(Of JSON_FileMetadata)(result.Jobj.SelectToken("data.resource").ToString, JSONhandler)
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Path" 'tested
    Public Async Function _Path() As Task(Of Dictionary(Of Integer, JSON_PathMetadata)) Implements IFile.Path
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/path" + AsQueryString(New ADictionary From {{"resource_hash", FileID}}))
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Dim lst As New Dictionary(Of Integer, JSON_PathMetadata)
                Dim fin = result.Jobj.SelectToken("data.path").ToString
                If String.IsNullOrEmpty(fin) Then Return New Dictionary(Of Integer, JSON_PathMetadata)
                Dim al = JsonConvert.DeserializeObject(Of List(Of JSON_PathMetadata))(fin, JSONhandler)
                For Each x In al
                    lst.Add(al.IndexOf(x), x)
                Next
                Return lst
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Delete" 'tested
    Public Async Function _Delete() As Task(Of Boolean) Implements IFile.Delete
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FileID}).Delete
    End Function
#End Region

#Region "Trash" 'tested
    Public Async Function _Trash() As Task(Of Boolean) Implements IFile.Trash
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FileID}).Trash
    End Function
#End Region

#Region "UnTrash" 'tested
    Public Async Function _UnTrash() As Task(Of Boolean) Implements IFile.UnTrash
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FileID}).UnTrash
    End Function
#End Region

#Region "Exists" 'tested
    Public Async Function _Exists() As Task(Of Dictionary(Of String, Boolean)) Implements IFile.Exists
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FileID}).Exists
    End Function
#End Region

#Region "ImagePreviewUrl" 'tested
    Public Function _ImagePreviewUrl() As String Implements IFile.ImagePreviewUrl
        Return (New pUri(String.Format("file/{0}/preview", FileID) + AsQueryString(New ADictionary))).ToString
    End Function
#End Region

#Region "ImagePreview" 'tested
    Public Async Function _ImagePreview(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream) Implements IFile.ImagePreview
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim RequestUri = New pUri(String.Format("file/{0}/preview", FileID), New ADictionary)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)

            token.ThrowIfCancellationRequested()
            If ResPonse.IsSuccessStatusCode Then
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ("File Downloaded successfully.")})
            Else
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
            End If

            ResPonse.EnsureSuccessStatusCode()
            Return Await ResPonse.Content.ReadAsStreamAsync()
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

#Region "Move" 'tested
    Public Async Function _Move(DestinationFolderID As String) As Task(Of Boolean) Implements IFile.Move
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(New String() {FileID}).Move(DestinationFolderID)
    End Function
#End Region

#Region "DirectUrl"
    Public Async Function _DirectUrl() As Task(Of String) Implements IFile.DirectUrl
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            HtpReqMessage.RequestUri = New pUri("file/direct", New ADictionary)
            HtpReqMessage.Content = New Net.Http.FormUrlEncodedContent(New Dictionary(Of String, String) From {{"file_hash", FileID}})
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return JObject.Parse(result)("status").ToString = "success"
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Rename"
    Public Async Function _Rename(NewName As String) As Task(Of Boolean) Implements IFile.Rename
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = New Net.Http.HttpMethod("PATCH")
            HtpReqMessage.RequestUri = New pUri("drive/info", New ADictionary)
            HtpReqMessage.Content = New Net.Http.FormUrlEncodedContent(New Dictionary(Of String, String) From {{"resource_hash", FileID}, {"name", NewName}})
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return True
            Else
                Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
            End If
        End Using
    End Function
#End Region

#Region "DownloadFile"
    Public Async Function GET_DownloadFile(FileSaveDir As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task Implements IFile.Download
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim RequestUri = New pUri(String.Format("file/{0}/download", FileID), New ADictionary)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Using ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)

                token.ThrowIfCancellationRequested()
                If ResPonse.IsSuccessStatusCode Then
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = (String.Format("[{0}] Downloaded successfully.", FileName))})
                Else
                    ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
                End If

                ResPonse.EnsureSuccessStatusCode()
                Dim stream_ = Await ResPonse.Content.ReadAsStreamAsync()
                Dim FPathname As String = String.Concat(FileSaveDir.TrimEnd("\"), "\", FileName)
                Using fileStream = New IO.FileStream(FPathname, IO.FileMode.Append, IO.FileAccess.Write)
                    stream_.CopyTo(fileStream)
                End Using
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

#Region "DownloadFileAsStream"
    Public Async Function GET_DownloadFileAsStream(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream) Implements IFile.DownloadAsStream
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim RequestUri = New pUri(String.Format("file/{0}/download", FileID), New ADictionary)
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)

            token.ThrowIfCancellationRequested()
            If ResPonse.IsSuccessStatusCode Then
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ("File Downloaded successfully.")})
            Else
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
            End If

            ResPonse.EnsureSuccessStatusCode()
            Dim stream_ As IO.Stream = Await ResPonse.Content.ReadAsStreamAsync()
            Return stream_
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

#Region "FileThumbnail"
    Structure Size
        Property Width As Integer
        Property Height As Integer
    End Structure
    Public Async Function GET_FileThumbnail(ThumbSize As Size, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream) Implements IFile.Thumbnail
        If ReportCls Is Nothing Then ReportCls = New Progress(Of ReportStatus)
        ReportCls.Report(New ReportStatus With {.Finished = False, .TextStatus = "Initializing..."})
        Try
            Dim progressHandler As New Net.Http.Handlers.ProgressMessageHandler(New HCHandler)
            AddHandler progressHandler.HttpReceiveProgress, (Function(sender, e)
                                                                 ReportCls.Report(New ReportStatus With {.ProgressPercentage = e.ProgressPercentage, .BytesTransferred = e.BytesTransferred, .TotalBytes = If(e.TotalBytes Is Nothing, 0, e.TotalBytes), .TextStatus = "Downloading..."})
                                                             End Function)
            Dim localHttpClient As New HttpClient(progressHandler)
            Dim RequestUri = New pUri(String.Format("file/{0}/icon", FileID), New ADictionary From {{"height", ThumbSize.Height}, {"width", ThumbSize.Width}})
            '''''''''''''''''will write the whole content to H.D WHEN download completed'''''''''''''''''''''''''''''

            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(False)

            token.ThrowIfCancellationRequested()
            If ResPonse.IsSuccessStatusCode Then
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ("File Downloaded successfully.")})
            Else
                ReportCls.Report(New ReportStatus With {.Finished = True, .TextStatus = ((String.Format("Error code: {0}", ResPonse.StatusCode)))})
            End If

            ResPonse.EnsureSuccessStatusCode()
            Dim stream_ As IO.Stream = Await ResPonse.Content.ReadAsStreamAsync()
            Return stream_
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
    Public Async Function _EditDescription(Description As String) As Task(Of Boolean) Implements IFile.EditDescription
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(New Net.Http.HttpMethod("PATCH"), New pUri("drive/info", New ADictionary))
            HtpReqMessage.Content = New Net.Http.FormUrlEncodedContent(New Dictionary(Of String, String) From {{"resource_hash", FileID}, {"description", Description}})
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK Then
                Return JObject.Parse(result)("status").ToString = "success"
            Else
                Throw CType(ExceptionCls.CreateException(JObject.Parse(result)("info")("type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region



End Class
