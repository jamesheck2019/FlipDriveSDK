Imports FlipDriveSDK.JSON
Imports Newtonsoft.Json
Imports System.ComponentModel
Imports Newtonsoft.Json.Linq
Imports FlipDriveSDK.utilitiez

Partial Public Class FClient
    Implements IClient


    Public Sub New(AccountToken As String, Optional Settings As ConnectionSettings = Nothing)
        AccToken = AccountToken
        ConnectionSetting = Settings
        If Settings Is Nothing Then
            m_proxy = Nothing
            m_CloseConnection = True
            m_TimeOut = TimeSpan.FromMinutes(60)
        Else
            m_proxy = Settings.Proxy
            m_CloseConnection = If(Settings.CloseConnection, True)
            m_TimeOut = If(Settings.TimeOut = Nothing, TimeSpan.FromMinutes(60), Settings.TimeOut)
        End If
        Net.ServicePointManager.Expect100Continue = True : Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls Or Net.SecurityProtocolType.Tls11 Or Net.SecurityProtocolType.Tls12 Or Net.SecurityProtocolType.Ssl3
    End Sub


    Public ReadOnly Property File(FileID As String) As IFile Implements IClient.File
        Get
            Return New FileClient(FileID)
        End Get
    End Property
    Public ReadOnly Property File() As IFile Implements IClient.File
        Get
            Return New FileClient()
        End Get
    End Property
    Public ReadOnly Property Folder(FolderID As String) As IFolder Implements IClient.Folder
        Get
            Return New FolderClient(FolderID)
        End Get
    End Property
    Public ReadOnly Property Folder() As IFolder Implements IClient.Folder
        Get
            Return New FolderClient()
        End Get
    End Property

#Region "UserInfo"
    Public Async Function POST_UserInfo() As Task(Of JSON_UserInfo) Implements IClient.UserInfo
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("me/info", New ADictionary)
            'Dim RequestUri = New Uri("https://api.flipdrive.com/2.0/rest/account/get_info" + utilitiez.AsQueryString(New ADictionary))
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return JsonConvert.DeserializeObject(Of JSON_UserInfo)(result.Jobj.SelectToken("data").ToString, JSONhandler)
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "DriveInfo" 'tested
    Public Async Function _DriveInfo(Drive As DriveTypeEnum) As Task(Of JSON_DriveMetadata) Implements IClient.DriveInfo
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Get
            HtpReqMessage.RequestUri = New pUri("drive/info" + AsQueryString(New ADictionary From {{"section", Drive.ToString}}))
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return JsonConvert.DeserializeObject(Of JSON_DriveMetadata)(result.Jobj.SelectToken("data").ToString, JSONhandler)
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "ListRecents" 'tested
    Public Async Function _ListRecents(Optional Limit As Integer = 250) As Task(Of JSON_ListRecents) Implements IClient.ListRecents
        Dim parameters = New ADictionary
        parameters.Add("pg_count", Limit)
        parameters.Add("initiator", "me")
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/recent/list", parameters)
            Using resPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await resPonse.Content.ReadAsStringAsync()

                If resPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                    Return JsonConvert.DeserializeObject(Of JSON_ListRecents)(result.Jobj.SelectToken("data").ToString, JSONhandler)
                Else
                    Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, resPonse.StatusCode), FlipDriveException)
                End If
            End Using
        End Using
    End Function
#End Region

#Region "ListFavorites" 'TESTED
    Public Async Function GET_ListFavorites(Filter As FilterEnum, Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList) Implements IClient.ListFavorites
        Dim parameters = New ADictionary
        parameters.Add("count", Limit)
        parameters.Add("section", DriveTypeEnum.favorite.ToString)
        parameters.Add("show", Filter.ToString) 'folders
        parameters.Add("sort", utilitiez.stringValueOf(Sort))

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

#Region "ListRecycleBin" 'tested
    Public Async Function GET_ListRecycleBin(Filter As FilterEnum, Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList) Implements IClient.ListRecycleBin
        Dim parameters = New ADictionary
        parameters.Add("count", Limit)
        parameters.Add("section", DriveTypeEnum.trash.ToString)
        parameters.Add("show", Filter.ToString)
        parameters.Add("sort", utilitiez.stringValueOf(Sort))

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

#Region "EmptyRecycleBin"
    Public Async Function POST_EmptyRecycleBin() As Task(Of Boolean) Implements IClient.EmptyRecycleBin
        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/trash", New ADictionary)
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.DeleteAsync(RequestUri).ConfigureAwait(False)
            Dim result As String = Await ResPonse.Content.ReadAsStringAsync()

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return True
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("info.type").ToString, ResPonse.StatusCode), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Search" 'tested
    Public Async Function GET_Search(Keyword As String, Drive As DriveTypeEnum, Filter As utilitiez.FilterEnum, Sort As utilitiez.SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList) Implements IClient.Search
        Dim parameters = New ADictionary
        parameters.Add("query", Keyword)
        parameters.Add("section", Drive.ToString)
        parameters.Add("show", Filter.ToString) 'folders
        parameters.Add("sort", utilitiez.stringValueOf(Sort))
        parameters.Add("count", Limit)
        parameters.Add("offset", OffSet)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim RequestUri = New pUri("drive/search", parameters)
            Using resPonse As Net.Http.HttpResponseMessage = Await localHttpClient.GetAsync(RequestUri).ConfigureAwait(False)
                Dim result As String = Await resPonse.Content.ReadAsStringAsync()

                If resPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                    Dim fin As New JSON_FolderList
                    fin.total = result.Jobj.SelectToken("data").Value(Of Integer)("total")
                    fin.total_files = result.Jobj.SelectToken("data").Value(Of Integer)("total_files")
                    fin.total_folders = result.Jobj.SelectToken("data").Value(Of Integer)("total_folders")
                    fin.total_returned = result.Jobj("data").SelectTokens("$.items[*]").Count
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

End Class
