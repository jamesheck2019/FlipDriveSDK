Imports FlipDriveSDK.JSON

Public Interface IFile

    ReadOnly Property Multiple(FileIDs As String()) As IFiles

    Function EditDescription(Description As String) As Task(Of Boolean)
    Function Thumbnail(ThumbSize As FileClient.Size, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream)
    Function DownloadAsStream(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream)
    Function Download(FileSaveDir As String, FileName As String, Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task
    Function Rename(NewName As String) As Task(Of Boolean)
    Function DirectUrl() As Task(Of String)
    Function Move(DestinationFolderID As String) As Task(Of Boolean)
    Function UnTrash() As Task(Of Boolean)
    Function ImagePreview(Optional ReportCls As IProgress(Of ReportStatus) = Nothing, Optional token As Threading.CancellationToken = Nothing) As Task(Of IO.Stream)
    Function Trash() As Task(Of Boolean)
    Function ImagePreviewUrl() As String
    Function Exists() As Task(Of Dictionary(Of String, Boolean))
    Function Delete() As Task(Of Boolean)
    Function Path() As Task(Of Dictionary(Of Integer, JSON_PathMetadata))
    Function Metadate() As Task(Of JSON_FileMetadata)

End Interface
