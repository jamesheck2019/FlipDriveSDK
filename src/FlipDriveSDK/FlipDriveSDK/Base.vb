Imports Newtonsoft.Json
Imports FlipDriveSDK.JSON

Module Base


    Private Property APIbase As String = "https://api2.flipdrive.com/4.0/"
    Public Property m_TimeOut As System.TimeSpan = Threading.Timeout.InfiniteTimeSpan
    Public Property m_CloseConnection As Boolean = True
    Friend Property JSONhandler As New Newtonsoft.Json.JsonSerializerSettings() With {.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore, .NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore}
    Friend Property AccToken() As String
    Friend Property ConnectionSetting As ConnectionSettings


    Friend Class pUri
        Inherits Uri
        Sub New(Action As String, Optional Parameters As Dictionary(Of String, String) = Nothing)
            MyBase.New(APIbase + Action + If(Parameters Is Nothing, Nothing, utilitiez.AsQueryString(Parameters)))
        End Sub
    End Class

    Private _proxy As ProxyConfig
    Friend Property m_proxy As ProxyConfig
        Get
            Return If(_proxy, New ProxyConfig)
        End Get
        Set(value As ProxyConfig)
            _proxy = value
        End Set
    End Property

    Public Class HCHandler
        Inherits Net.Http.HttpClientHandler
        Sub New()
            MyBase.New()
            If m_proxy.SetProxy Then
                MaxRequestContentBufferSize = 1 * 1024 * 1024
                Proxy = New Net.WebProxy(String.Format("http://{0}:{1}", m_proxy.ProxyIP, m_proxy.ProxyPort), True, Nothing, New Net.NetworkCredential(m_proxy.ProxyUsername, m_proxy.ProxyPassword))
                UseProxy = m_proxy.SetProxy
            End If
        End Sub
    End Class

    Friend Class ADictionary
        Inherits Dictionary(Of String, String)
        Sub New()
            MyBase.Add("api_key", "rgNf3tDKH2XpsQQn") 'MyBase.Add("api_key", "web")
            MyBase.Add("auth_token", AccToken)
            MyBase.Add("response", "json")
        End Sub
    End Class

    Public Class HttpClient
        Inherits Net.Http.HttpClient
        Sub New(HCHandler As HCHandler)
            MyBase.New(HCHandler)
            MyBase.DefaultRequestHeaders.UserAgent.ParseAdd("FlipDriveSDK")
            DefaultRequestHeaders.ConnectionClose = m_CloseConnection
            Timeout = m_TimeOut
        End Sub
        Sub New(progressHandler As Net.Http.Handlers.ProgressMessageHandler)
            MyBase.New(progressHandler)
            MyBase.DefaultRequestHeaders.UserAgent.ParseAdd("FlipDriveSDK")
            DefaultRequestHeaders.ConnectionClose = m_CloseConnection
            Timeout = m_TimeOut
        End Sub
    End Class

    <Runtime.CompilerServices.Extension()>
    Function CheckStatus(result As String) As Boolean
        Select Case True
            Case Newtonsoft.Json.Linq.JObject.Parse(result).SelectToken("status") IsNot Nothing
                Return Newtonsoft.Json.Linq.JObject.Parse(result).SelectToken("status").ToString = "success"
            Case Newtonsoft.Json.Linq.JObject.Parse(result).SelectToken("response.status") IsNot Nothing
                Return Newtonsoft.Json.Linq.JObject.Parse(result).SelectToken("response.status").ToString = "success"
        End Select
    End Function

    <Runtime.CompilerServices.Extension()>
    Public Function Jobj(response As String) As Newtonsoft.Json.Linq.JObject
        Return Newtonsoft.Json.Linq.JObject.Parse(response)
    End Function
End Module
