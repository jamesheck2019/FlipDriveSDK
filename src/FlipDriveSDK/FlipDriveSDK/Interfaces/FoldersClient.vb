Public Class FoldersClient
    Implements IFolders


    Private Property FolderIDs As String()

    Sub New(FolderIDs() As String)
        Me.FolderIDs = FolderIDs
    End Sub





#Region "Delete" 'tested
    Public Async Function _Delete() As Task(Of Boolean) Implements IFolders.Delete
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(FolderIDs).Delete
    End Function
#End Region

#Region "Trash" 'tested
    Public Async Function _Trash() As Task(Of Boolean) Implements IFolders.Trash
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(FolderIDs).Trash
    End Function
#End Region

#Region "UnTrash" 'tested
    Public Async Function _UnTrash() As Task(Of Boolean) Implements IFolders.UnTrash
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(FolderIDs).UnTrash
    End Function
#End Region

#Region "Exists" 'tested
    Public Async Function _Exists() As Task(Of Dictionary(Of String, Boolean)) Implements IFolders.Exists
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(FolderIDs).Exists
    End Function
#End Region

#Region "Move"
    Public Async Function _Move(DestinationFolderID As String) As Task(Of Boolean) Implements IFolders.Move
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(FolderIDs).Move(DestinationFolderID)
    End Function
#End Region

#Region "AddToFavorite"
    Public Async Function _AddToFavorite() As Task(Of Boolean) Implements IFolders.AddToFavorite
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(FolderIDs).AddToFavorite
    End Function
#End Region

#Region "RemoveFromFavorite"
    Public Async Function _RemoveFromFavorite() As Task(Of Boolean) Implements IFolders.RemoveFromFavorite
        Dim client As New FlipDriveSDK.FClient(AccToken, ConnectionSetting)
        Return Await client.File.Multiple(FolderIDs).RemoveFromFavorite
    End Function
#End Region




End Class
