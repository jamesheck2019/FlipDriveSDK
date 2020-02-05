Imports Newtonsoft.Json
Imports FlipDriveSDK.JSON
Imports Newtonsoft.Json.Linq

Public Class FilesClient
    Implements IFiles


    Private Property FileIDs As String()

    Sub New(FileIDs() As String)
        Me.FileIDs = FileIDs
    End Sub


#Region "Delete" 'tested
    Public Async Function _Delete() As Task(Of Boolean) Implements IFiles.Delete
        Dim parameters = New ADictionary
        parameters.Add("resources", JsonConvert.SerializeObject(FileIDs))
        parameters.Add("permanent", 1)
        Dim encodedContent = New Net.Http.FormUrlEncodedContent(parameters)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Delete
            HtpReqMessage.RequestUri = New pUri("drive/list", parameters)
            HtpReqMessage.Content = encodedContent
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.warnings").ToString = "[]"
            Else
                Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
            End If
        End Using
    End Function
#End Region

#Region "Trash" 'tested
    Public Async Function _Trash() As Task(Of Boolean) Implements IFiles.Trash
        Dim parameters = New ADictionary
        parameters.Add("resources", JsonConvert.SerializeObject(FileIDs))
        parameters.Add("permanent", 0)
        Dim encodedContent = New Net.Http.FormUrlEncodedContent(parameters)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Delete
            HtpReqMessage.RequestUri = New pUri("drive/list", parameters)
            HtpReqMessage.Content = encodedContent
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.warnings").ToString = "[]"
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("data.warnings[0].type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "UnTrash" 'tested
    Public Async Function _UnTrash() As Task(Of Boolean) Implements IFiles.UnTrash
        Dim parameters = New ADictionary
        parameters.Add("resources", JsonConvert.SerializeObject(FileIDs))
        Dim encodedContent = New Net.Http.FormUrlEncodedContent(parameters)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            HtpReqMessage.RequestUri = New pUri("drive/recovery", parameters)
            HtpReqMessage.Content = encodedContent
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.warnings").ToString = "[]"
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("data.warnings[0].type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Exists" 'tested
    Public Async Function _Exists() As Task(Of Dictionary(Of String, Boolean)) Implements IFiles.Exists
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("file/existence", New ADictionary From {{"resources", JsonConvert.SerializeObject(FileIDs)}})
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.items").Select(Function(i) New KeyValuePair(Of String, Boolean)(i.Value(Of String)("hash"), i.Value(Of Boolean)("found"))).ToDictionary(Function(x) x.Key, Function(x) x.Value)
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Move"
    Public Async Function _Move(DestinationFolderID As String) As Task(Of Boolean) Implements IFiles.Move
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = New Net.Http.HttpMethod("PATCH")
            HtpReqMessage.RequestUri = New pUri("drive/path", New ADictionary)
            HtpReqMessage.Content = New Net.Http.FormUrlEncodedContent(New Dictionary(Of String, String) From {{"destination_hash", DestinationFolderID}, {"resources", JsonConvert.SerializeObject(FileIDs)}})
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.warnings").ToString = "[]"
            Else
                Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
            End If
        End Using
    End Function
#End Region

#Region "AddToFavorite"
    Public Async Function _AddToFavorite() As Task(Of Boolean) Implements IFiles.AddToFavorite
        Dim parameters = New ADictionary
        'parameters.Remove("api_key")
        parameters.Add("resources", JsonConvert.SerializeObject(FileIDs))
        'parameters.Add("api_key", "web")

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Post, New pUri("favorite", parameters))
            'HtpReqMessage.Content = New Net.Http.FormUrlEncodedContent(parameters)
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.warnings").ToString = "[]"
            Else
                Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
            End If
        End Using
    End Function
#End Region

#Region "RemoveFromFavorite"
    Public Async Function _RemoveFromFavorite() As Task(Of Boolean) Implements IFiles.RemoveFromFavorite
        Dim parameters = New ADictionary
        parameters.Add("resources", JsonConvert.SerializeObject(FileIDs))
        Dim encodedContent = New Net.Http.FormUrlEncodedContent(parameters)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage(Net.Http.HttpMethod.Delete, New pUri("favorite", parameters))
            'HtpReqMessage.Content = encodedContent
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return result.Jobj.SelectToken("data.warnings").ToString = "[]"
            Else
                Throw ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode)
            End If
        End Using
    End Function
#End Region


End Class
