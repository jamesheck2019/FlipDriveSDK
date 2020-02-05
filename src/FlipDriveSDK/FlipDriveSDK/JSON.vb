
Imports System.ComponentModel
Imports Newtonsoft.Json
Imports FlipDriveSDK.utilitiez

Namespace JSON


#Region "JSON_GetToken"
    Public Class JSON_GetToken
        Public Property auth_token As String
        Public Property account_hash As String
        Public Property user_hash As String
    End Class
#End Region

#Region "JSON_FolderList"
    Public Class JSON_FolderList
        'Public Property items As List(Of JSON_Item)
        Public Property total As Integer
        Public Property total_folders As Integer
        Public Property total_files As Integer
        Public Property total_returned As Integer
        Public Property name As String

        Public Property _Files As List(Of JSON_FileMetadata)
        Public Property _Folders As List(Of JSON_FolderMetadata)
    End Class
#End Region

#Region "JSON_PathMetadata"
    Public Class JSON_PathMetadata
        Public Property hash As String
        Public Property name As String
        Public Property total_folders As Integer
        Public Property total_files As Integer
    End Class
#End Region

#Region "JSON_DriveMetadata"
    Public Class JSON_DriveMetadata
        Public Property total_folders As Integer
        Public Property total_files As Integer
    End Class
#End Region

#Region "JSON_FolderMetadata"
    Public Class JSON_FolderMetadata
        <JsonProperty("hash")> Public Property ID As String
        <JsonProperty("link_hash")> Public Property LinkID As Object
        Public Property type As ItemEnum
        Public Property name As String
        Public Property description As String
        Public Property total_files As Integer
        Public Property total_folders As Integer
        Public Property favorite As Boolean
        Public Property link As Boolean
        Public Property [shared] As Boolean
        Public Property created_at As Integer
        Public Property updated_at As Integer
        <JsonProperty("owner_hash")> Public Property OwnerID As String
        Public Property owner_name As String
    End Class
#End Region

#Region "JSON_FileMetadata"
    Public Class JSON_FileMetadata
        Public Property found As Boolean
        <JsonProperty("hash")> Public Property ID As String
        <JsonProperty("link_hash")> Public Property LinkID As Object
        Public Property type As ItemEnum
        Public Property name As String
        Public Property extension As String
        Public Property description As String
        Public Property size As Integer
        Public Property revision As Integer
        Public Property uploading As Boolean
        Public Property favorite As Boolean
        Public Property direct As Boolean
        Public Property link As Boolean
        Public Property [shared] As Boolean
        Public Property created_at As Integer
        Public Property updated_at As Integer
        <JsonProperty("owner_hash")> Public Property OwnerID As String
        Public Property owner_name As String
    End Class
#End Region

#Region "JSON_ListRecents"
    Public Class JSON_ListRecents
        Public Property items As List(Of JSON_FileMetadata)
        <JsonProperty("more")> Public Property HasMore As Boolean
        Public Property last_updated As Integer
        Public Property pg_forward As Integer
    End Class
#End Region

#Region "JSON_UserInfo"
    Public Class JSON_UserInfo
        <JsonProperty("account_hash")> Public Property AccountID As String
        <JsonProperty("user_hash")> Public Property UserID As String
        <JsonProperty("total_space")> Public Property TotalStorage As Long
        <JsonProperty("space_occupied")> Public Property UsedStorage As Long
    End Class
#End Region


End Namespace

