Imports FlipDriveSDK.JSON
Imports FlipDriveSDK.utilitiez

Public Interface IClient

    ''http://api2.flipdrive.com/4.0/share/public/download?api_key=web&code=K_BCd5G
    ''https://api.flipdrive.com/2.0/rest

    ReadOnly Property File(FileID As String) As IFile
    ReadOnly Property File As IFile
    ReadOnly Property Folder(FolderID As String) As IFolder
    ReadOnly Property Folder As IFolder

    Function DriveInfo(Drive As DriveTypeEnum) As Task(Of JSON_DriveMetadata)
    Function UserInfo() As Task(Of JSON_UserInfo)
    Function ListRecents(Optional Limit As Integer = 250) As Task(Of JSON_ListRecents)
    Function ListFavorites(Filter As utilitiez.FilterEnum, Sort As utilitiez.SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList)
    Function ListRecycleBin(Filter As utilitiez.FilterEnum, Sort As utilitiez.SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList)
    Function EmptyRecycleBin() As Task(Of Boolean)
    Function Search(Keyword As String, Drive As DriveTypeEnum, Filter As utilitiez.FilterEnum, Sort As utilitiez.SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList)

End Interface
