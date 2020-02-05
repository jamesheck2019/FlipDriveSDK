Imports FlipDriveSDK.JSON
Imports FlipDriveSDK.utilitiez

Public Interface IFolder


    ReadOnly Property Multiple(FolderIDs As String()) As IFolders

    Function Upload(FileToUpload As Object, UploadType As UploadTypes, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of JSON_FileMetadata)
    Function Create(FolderName As String) As Task(Of JSON_FolderMetadata)
    Function Rename(NewName As String) As Task(Of Boolean)
    Function Move(DestinationFolderID As String) As Task(Of Boolean)
    Function Exists() As Task(Of Dictionary(Of String, Boolean))
    Function Trash() As Task(Of Boolean)
    Function Delete() As Task(Of Boolean)
    Function Metadate() As Task(Of JSON_FolderMetadata)
    Function List(Filter As FilterEnum, Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of JSON_FolderList)
    Function ListFiles(Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of List(Of JSON_FileMetadata))
    Function ListFolders(Sort As SortEnum, Optional Limit As Integer = 1000, Optional OffSet As Integer = 0) As Task(Of List(Of JSON_FolderMetadata))
    Function EditDescription(Description As String) As Task(Of Boolean)

End Interface
