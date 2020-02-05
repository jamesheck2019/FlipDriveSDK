Public Interface IFolders

    Function RemoveFromFavorite() As Task(Of Boolean)
    Function AddToFavorite() As Task(Of Boolean)
    Function Move(DestinationFolderID As String) As Task(Of Boolean)
    Function Exists() As Task(Of Dictionary(Of String, Boolean))
    Function UnTrash() As Task(Of Boolean)
    Function Trash() As Task(Of Boolean)
    Function Delete() As Task(Of Boolean)

End Interface
