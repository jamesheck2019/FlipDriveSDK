using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FlipDriveSDK.JSON;

namespace FlipDriveSDK
{
	public interface IClient
	{
		Task<JSON_UserInfo> UserInfo();

		Task<JSON_List> List(string DestinationFolderID, utilitiez.FilterEnum Filter, utilitiez.SortEnum Sort, int Limit = 1000);

		Task<JSON_Item> Upload(object FileToUpload, utilitiez.UploadTypes UploadType, string DestinationFolderID, string FileName, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

		Task<JSON_Item> CreateNewFolder(string DestinationFolderID, string FolderName);

		Task<bool> Rename(string DestinationID, string NewName);

		Task<bool> Move(List<string> SourceIDs, string DestinationFolderID);

		Task<bool> Trash(List<string> DestinationIDs);

		Task<bool> Delete(List<string> DestinationIDs);

		Task<Stream> DownloadAsStream(string DestinationFileID, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

		Task Download(string DestinationFileID, string FileSaveDir, string FileName, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

		Task<Stream> FileThumbnail(string DestinationFileID, FClient.Size ThumbSize, IProgress<ReportStatus> ReportCls = null, CancellationToken token = default(CancellationToken));

		Task<bool> AddToFavorite(List<string> DestinationIDs);

		Task<bool> RemoveFromFavorite(List<string> DestinationIDs);

		Task<JSON_ListRecents> ListRecents(int Limit = 250);

		Task<JSON_List> ListFavorites(utilitiez.FilterEnum Filter, utilitiez.SortEnum Sort, int Limit = 1000);

		Task<JSON_List> ListRecycleBin(utilitiez.FilterEnum Filter, utilitiez.SortEnum Sort, int Limit = 1000);

		Task<bool> EmptyRecycleBin();

		Task<JSON_List> Search(string Keyword, utilitiez.FilterEnum Filter, utilitiez.SortEnum Sort, int Limit = 1000);
	}
}
