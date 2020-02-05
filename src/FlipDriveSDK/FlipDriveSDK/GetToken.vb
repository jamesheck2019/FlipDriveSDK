Imports Newtonsoft.Json
Imports FlipDriveSDK.JSON
Imports Newtonsoft.Json.Linq

Public Class GetToken


#Region "Get_Token"
    Shared Async Function Token(Email As String, Password As String) As Task(Of JSON_GetToken)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("email", Email)
        parameters.Add("password", Password)
        parameters.Add("new", "1")
        parameters.Add("api_key", "EzJyErH5ewEMpGm4")
        parameters.Add("api_key_new", "rgNf3tDKH2XpsQQn")
        parameters.Add("response", "json")
        Dim encodedContent = New Net.Http.FormUrlEncodedContent(parameters)

        Using localHttpClient As New Net.Http.HttpClient()
            localHttpClient.DefaultRequestHeaders.Accept.Add(New Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"))
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            HtpReqMessage.RequestUri = New Uri("https://api.flipdrive.com/2.0/rest/account/authorization")
            HtpReqMessage.Content = encodedContent
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            'Dim result As String = Await ResPonse.Content.ReadAsStringAsync
            ''to solve server encoding shit
            Dim result As String = System.Text.Encoding.UTF8.GetString(Await ResPonse.Content.ReadAsByteArrayAsync())

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                Return JsonConvert.DeserializeObject(Of JSON_GetToken)(result.Jobj.SelectToken("response.data.new_api").ToString, JSONhandler)
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("response.status").ToString, 105), FlipDriveException)
            End If
        End Using
    End Function
#End Region

#Region "Signup"
    Shared Async Function Signup(Username As String, Email As String, Password As String) As Task(Of Boolean)
        Dim parameters = New Dictionary(Of String, String)
        parameters.Add("api_key", "EzJyErH5ewEMpGm4")
        parameters.Add("username", Username)
        parameters.Add("password", Password)
        parameters.Add("new", 1)
        parameters.Add("fname", Username)
        parameters.Add("lname", Username)
        parameters.Add("email", Email)
        parameters.Add("country", "US")
        parameters.Add("acc_type", "FP")
        'parameters.Add("acc_type", "MO")
        'parameters.Add("app_version", "1.0.0.0")
        'parameters.Add("device_code", "866316003726406")
        'parameters.Add("os_version", "83890177")
        'parameters.Add("os_code", "20170344")
        parameters.Add("response", "json")
        Dim encodedContent = New Net.Http.FormUrlEncodedContent(parameters)

        Using localHttpClient As New HttpClient(New HCHandler)
            Dim HtpReqMessage As New Net.Http.HttpRequestMessage()
            HtpReqMessage.Method = Net.Http.HttpMethod.Post
            HtpReqMessage.RequestUri = New Uri("https://api.flipdrive.com/2.0/rest/account/signup")
            HtpReqMessage.Content = encodedContent
            Dim ResPonse As Net.Http.HttpResponseMessage = Await localHttpClient.SendAsync(HtpReqMessage, Net.Http.HttpCompletionOption.ResponseContentRead).ConfigureAwait(False)
            Dim result As String = System.Text.Encoding.UTF8.GetString(Await ResPonse.Content.ReadAsByteArrayAsync())

            If ResPonse.StatusCode = Net.HttpStatusCode.OK AndAlso result.CheckStatus Then
                'Return JsonConvert.DeserializeObject(Of JSON_GetToken)(result.Jobj.SelectToken("response.data.new_api").ToString, JSONhandler)
            Else
                Throw CType(ExceptionCls.CreateException(result.Jobj.SelectToken("response.status").ToString, 105), FlipDriveException)
            End If
        End Using
    End Function
#End Region

End Class
