Imports System.ComponentModel

Public Class utilitiez

    Public Shared Function AsQueryString(parameters As Dictionary(Of String, String)) As String
        If Not parameters.Any() Then Return String.Empty
        Dim builder = New Text.StringBuilder("?")
        Dim separator = String.Empty
        For Each kvp In parameters.Where(Function(P) Not String.IsNullOrEmpty(P.Value))
            builder.AppendFormat("{0}{1}={2}", separator, Net.WebUtility.UrlEncode(kvp.Key), Net.WebUtility.UrlEncode(kvp.Value.ToString()))
            separator = "&"
        Next
        Return builder.ToString()
    End Function

    Shared Function Between(source As System.String, leftString As String, rightString As String) As String
        Return System.Text.RegularExpressions.Regex.Match(source, String.Format("{0}(.*){1}", leftString, rightString)).Groups(1).Value
    End Function

    Public Shared Function UnixTimeStampToDateTime(unixTimeStamp As Double) As DateTime
        Dim dtDateTime As System.DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime()
        Return dtDateTime
    End Function

    Shared Function hs1FileHash(FilePath As String) As String
        Using bs As IO.BufferedStream = New IO.BufferedStream(New IO.MemoryStream(IO.File.ReadAllBytes(FilePath)))
            Using sha1 As Security.Cryptography.SHA1Managed = New Security.Cryptography.SHA1Managed
                Dim hash As Byte() = sha1.ComputeHash(bs)
                Dim formatted As Text.StringBuilder = New Text.StringBuilder(2 * hash.Length)
                For Each b As Byte In hash
                    formatted.AppendFormat("{0:X2}", b)
                Next
                Return formatted.ToString
            End Using
        End Using
    End Function

#Region "EnumUtils"
    Public Shared Function stringValueOf(ByVal value As [Enum]) As String
        Dim fi As Reflection.FieldInfo = value.[GetType]().GetField(value.ToString())
        Dim attributes As DescriptionAttribute() = CType(fi.GetCustomAttributes(GetType(DescriptionAttribute), False), DescriptionAttribute())

        If attributes.Length > 0 Then
            Return attributes(0).Description
        Else
            Return value.ToString()
        End If
    End Function

    Public Shared Function enumValueOf(ByVal value As String, ByVal enumType As Type) As Object
        Dim names As String() = [Enum].GetNames(enumType)

        For Each name As String In names

            If stringValueOf(CType([Enum].Parse(enumType, name), [Enum])).Equals(value) Then
                Return [Enum].Parse(enumType, name)
            End If
        Next

        Throw New ArgumentException("The string is not a description or value of the specified enum.")
    End Function
#End Region



    Enum UploadTypes
        FilePath
        Stream
        BytesArry
    End Enum
    Enum ItemEnum
        folder
        file
    End Enum
    Enum SortEnum
        <DescriptionAttribute("NA")> Name
        <DescriptionAttribute("SA")> Size
        <DescriptionAttribute("DA")> UpdatedDate
    End Enum
    Enum DriveTypeEnum
        main
        trash
        favorite
        shared_to_me
        shared_by_me
        direct 'DirectLinks
    End Enum
    Enum FilterEnum
        all
        folders
        files
    End Enum


End Class

Public Class ConnectionSettings
    Public Property TimeOut As System.TimeSpan = Nothing
    Public Property CloseConnection As Boolean? = True
    Public Property Proxy As ProxyConfig = Nothing
End Class