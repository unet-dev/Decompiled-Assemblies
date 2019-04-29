using Facepunch.Steamworks.Callbacks;
using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Facepunch.Steamworks
{
	public class Workshop : IDisposable
	{
		internal const ulong InvalidHandle = 18446744073709551615L;

		internal SteamUGC ugc;

		internal Friends friends;

		internal BaseSteamworks steamworks;

		internal SteamRemoteStorage remoteStorage;

		[ThreadStatic]
		private static ulong[] _sSubscribedItemBuffer;

		static Workshop()
		{
		}

		internal Workshop(BaseSteamworks steamworks, SteamUGC ugc, SteamRemoteStorage remoteStorage)
		{
			this.ugc = ugc;
			this.steamworks = steamworks;
			this.remoteStorage = remoteStorage;
			steamworks.RegisterCallback<DownloadItemResult_t>(new Action<DownloadItemResult_t>(this.onDownloadResult));
			steamworks.RegisterCallback<ItemInstalled_t>(new Action<ItemInstalled_t>(this.onItemInstalled));
		}

		public Workshop.Editor CreateItem(Workshop.ItemType type = 0)
		{
			return this.CreateItem(this.steamworks.AppId, type);
		}

		public Workshop.Editor CreateItem(uint workshopUploadAppId, Workshop.ItemType type = 0)
		{
			return new Workshop.Editor()
			{
				workshop = this,
				WorkshopUploadAppId = workshopUploadAppId,
				Type = new Workshop.ItemType?(type)
			};
		}

		public Workshop.Query CreateQuery()
		{
			return new Workshop.Query()
			{
				AppId = this.steamworks.AppId,
				workshop = this,
				friends = this.friends
			};
		}

		public void Dispose()
		{
			this.ugc = null;
			this.steamworks = null;
			this.remoteStorage = null;
			this.friends = null;
			this.OnFileDownloaded = null;
			this.OnItemInstalled = null;
		}

		public Workshop.Editor EditItem(ulong itemId)
		{
			return new Workshop.Editor()
			{
				workshop = this,
				Id = itemId,
				WorkshopUploadAppId = this.steamworks.AppId
			};
		}

		public Workshop.Item GetItem(ulong itemid)
		{
			return new Workshop.Item(itemid, this);
		}

		public unsafe ulong[] GetSubscribedItemIds()
		{
			// 
			// Current member / type: System.UInt64[] Facepunch.Steamworks.Workshop::GetSubscribedItemIds()
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.UInt64[] GetSubscribedItemIds()
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public unsafe int GetSubscribedItemIds(List<ulong> destList)
		{
			// 
			// Current member / type: System.Int32 Facepunch.Steamworks.Workshop::GetSubscribedItemIds(System.Collections.Generic.List`1<System.UInt64>)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Int32 GetSubscribedItemIds(System.Collections.Generic.List<System.UInt64>)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 109
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\UnaryExpression.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 143
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 73
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void onDownloadResult(DownloadItemResult_t obj)
		{
			if (this.OnFileDownloaded != null && obj.AppID == Client.Instance.AppId)
			{
				this.OnFileDownloaded(obj.PublishedFileId, obj.Result);
			}
		}

		private void onItemInstalled(ItemInstalled_t obj)
		{
			if (this.OnItemInstalled != null && obj.AppID == Client.Instance.AppId)
			{
				this.OnItemInstalled(obj.PublishedFileId);
			}
		}

		public event Action<ulong, Facepunch.Steamworks.Callbacks.Result> OnFileDownloaded;

		public event Action<ulong> OnItemInstalled;

		public class Editor
		{
			internal Workshop workshop;

			internal CallbackHandle CreateItem;

			internal CallbackHandle SubmitItemUpdate;

			internal UGCUpdateHandle_t UpdateHandle;

			private int bytesUploaded;

			private int bytesTotal;

			public int BytesTotal
			{
				get
				{
					if (!this.Publishing)
					{
						return this.bytesTotal;
					}
					if (this.UpdateHandle == 0)
					{
						return this.bytesTotal;
					}
					ulong num = (ulong)0;
					ulong num1 = (ulong)0;
					this.workshop.steamworks.native.ugc.GetItemUpdateProgress(this.UpdateHandle, out num, out num1);
					this.bytesTotal = Math.Max(this.bytesTotal, (int)num1);
					return this.bytesTotal;
				}
			}

			public int BytesUploaded
			{
				get
				{
					if (!this.Publishing)
					{
						return this.bytesUploaded;
					}
					if (this.UpdateHandle == 0)
					{
						return this.bytesUploaded;
					}
					ulong num = (ulong)0;
					ulong num1 = (ulong)0;
					this.workshop.steamworks.native.ugc.GetItemUpdateProgress(this.UpdateHandle, out num, out num1);
					this.bytesUploaded = Math.Max(this.bytesUploaded, (int)num);
					return this.bytesUploaded;
				}
			}

			public string ChangeNote
			{
				get;
				set;
			}

			public string Description
			{
				get;
				set;
			}

			public string Error
			{
				get;
				internal set;
			}

			public string Folder
			{
				get;
				set;
			}

			public ulong Id
			{
				get;
				internal set;
			}

			public Dictionary<string, string[]> KeyValues
			{
				get;
				set;
			}

			public string MetaData
			{
				get;
				set;
			}

			public bool NeedToAgreeToWorkshopLegal
			{
				get;
				internal set;
			}

			public string PreviewImage
			{
				get;
				set;
			}

			public double Progress
			{
				get
				{
					int bytesTotal = this.BytesTotal;
					if (bytesTotal == 0)
					{
						return 0;
					}
					return (double)this.BytesUploaded / (double)bytesTotal;
				}
			}

			public bool Publishing
			{
				get;
				internal set;
			}

			public List<string> Tags
			{
				get;
				set;
			}

			public string Title
			{
				get;
				set;
			}

			public Workshop.ItemType? Type
			{
				get;
				set;
			}

			public Workshop.Editor.VisibilityType? Visibility
			{
				get;
				set;
			}

			public uint WorkshopUploadAppId
			{
				get;
				set;
			}

			public Editor()
			{
			}

			public void Delete()
			{
				this.workshop.ugc.DeleteItem(this.Id, null);
				this.Id = (ulong)0;
			}

			private void OnChangesSubmittedInternal(SubmitItemUpdateResult_t obj, bool Failed)
			{
				string str;
				if (Failed)
				{
					throw new Exception("CreateItemResult_t Failed");
				}
				this.UpdateHandle = (long)0;
				this.SubmitItemUpdate = null;
				this.NeedToAgreeToWorkshopLegal = obj.UserNeedsToAcceptWorkshopLegalAgreement;
				this.Publishing = false;
				if (obj.Result != SteamNative.Result.OK)
				{
					str = string.Format("Error publishing changes: {0} ({1})", obj.Result, this.NeedToAgreeToWorkshopLegal);
				}
				else
				{
					str = null;
				}
				this.Error = str;
				Action<Facepunch.Steamworks.Callbacks.Result> action = this.OnChangesSubmitted;
				if (action == null)
				{
					return;
				}
				action(obj.Result);
			}

			private void OnItemCreated(CreateItemResult_t obj, bool Failed)
			{
				this.NeedToAgreeToWorkshopLegal = obj.UserNeedsToAcceptWorkshopLegalAgreement;
				this.CreateItem.Dispose();
				this.CreateItem = null;
				if (obj.Result == SteamNative.Result.OK && !Failed)
				{
					this.Error = null;
					this.Id = obj.PublishedFileId;
					this.PublishChanges();
					return;
				}
				this.Error = string.Format("Error creating new file: {0} ({1})", obj.Result, obj.PublishedFileId);
				this.Publishing = false;
				Action<Facepunch.Steamworks.Callbacks.Result> action = this.OnChangesSubmitted;
				if (action == null)
				{
					return;
				}
				action(obj.Result);
			}

			public void Publish()
			{
				this.bytesUploaded = 0;
				this.bytesTotal = 0;
				this.Publishing = true;
				this.Error = null;
				if (this.Id == 0)
				{
					this.StartCreatingItem();
					return;
				}
				this.PublishChanges();
			}

			private void PublishChanges()
			{
				if (this.WorkshopUploadAppId == 0)
				{
					throw new Exception("WorkshopUploadAppId should not be 0");
				}
				this.UpdateHandle = this.workshop.ugc.StartItemUpdate(this.WorkshopUploadAppId, this.Id);
				if (this.Title != null)
				{
					this.workshop.ugc.SetItemTitle(this.UpdateHandle, this.Title);
				}
				if (this.Description != null)
				{
					this.workshop.ugc.SetItemDescription(this.UpdateHandle, this.Description);
				}
				if (this.Folder != null)
				{
					if (!(new DirectoryInfo(this.Folder)).Exists)
					{
						throw new Exception(string.Concat("Folder doesn't exist (", this.Folder, ")"));
					}
					this.workshop.ugc.SetItemContent(this.UpdateHandle, this.Folder);
				}
				if (this.Tags != null && this.Tags.Count > 0)
				{
					this.workshop.ugc.SetItemTags(this.UpdateHandle, this.Tags.ToArray());
				}
				if (this.Visibility.HasValue)
				{
					SteamUGC steamUGC = this.workshop.ugc;
					UGCUpdateHandle_t updateHandle = this.UpdateHandle;
					Workshop.Editor.VisibilityType? visibility = this.Visibility;
					steamUGC.SetItemVisibility(updateHandle, (RemoteStoragePublishedFileVisibility)visibility.Value);
				}
				if (this.PreviewImage != null)
				{
					FileInfo fileInfo = new FileInfo(this.PreviewImage);
					if (!fileInfo.Exists)
					{
						throw new Exception(string.Concat("PreviewImage doesn't exist (", this.PreviewImage, ")"));
					}
					if (fileInfo.Length >= (long)1048576)
					{
						throw new Exception(string.Format("PreviewImage should be under 1MB ({0})", fileInfo.Length));
					}
					this.workshop.ugc.SetItemPreview(this.UpdateHandle, this.PreviewImage);
				}
				if (this.MetaData != null)
				{
					this.workshop.ugc.SetItemMetadata(this.UpdateHandle, this.MetaData);
				}
				if (this.KeyValues != null)
				{
					foreach (KeyValuePair<string, string[]> keyValue in this.KeyValues)
					{
						string[] value = keyValue.Value;
						for (int i = 0; i < (int)value.Length; i++)
						{
							string str = value[i];
							this.workshop.ugc.AddItemKeyValueTag(this.UpdateHandle, keyValue.Key, str);
						}
					}
				}
				this.SubmitItemUpdate = this.workshop.ugc.SubmitItemUpdate(this.UpdateHandle, this.ChangeNote, new Action<SubmitItemUpdateResult_t, bool>(this.OnChangesSubmittedInternal));
			}

			private void StartCreatingItem()
			{
				if (!this.Type.HasValue)
				{
					throw new Exception("Editor.Type must be set when creating a new item!");
				}
				if (this.WorkshopUploadAppId == 0)
				{
					throw new Exception("WorkshopUploadAppId should not be 0");
				}
				SteamUGC steamUGC = this.workshop.ugc;
				AppId_t workshopUploadAppId = this.WorkshopUploadAppId;
				Workshop.ItemType? type = this.Type;
				this.CreateItem = steamUGC.CreateItem(workshopUploadAppId, (WorkshopFileType)type.Value, new Action<CreateItemResult_t, bool>(this.OnItemCreated));
			}

			public event Action<Facepunch.Steamworks.Callbacks.Result> OnChangesSubmitted;

			public enum VisibilityType
			{
				Public,
				FriendsOnly,
				Private
			}
		}

		public class Item
		{
			internal Workshop workshop;

			private DirectoryInfo _directory;

			private ulong _BytesDownloaded;

			private ulong _BytesTotal;

			private int YourVote;

			private string _ownerName;

			public ulong BytesDownloaded
			{
				get
				{
					this.UpdateDownloadProgress();
					return this._BytesDownloaded;
				}
			}

			public ulong BytesTotalDownload
			{
				get
				{
					this.UpdateDownloadProgress();
					return this._BytesTotal;
				}
			}

			public string ChangelogUrl
			{
				get
				{
					return string.Format("http://steamcommunity.com/sharedfiles/filedetails/changelog/{0}", this.Id);
				}
			}

			public string CommentsUrl
			{
				get
				{
					return string.Format("http://steamcommunity.com/sharedfiles/filedetails/comments/{0}", this.Id);
				}
			}

			public DateTime Created
			{
				get;
				private set;
			}

			public string Description
			{
				get;
				private set;
			}

			public DirectoryInfo Directory
			{
				get
				{
					ulong num;
					string str;
					uint num1;
					if (this._directory != null)
					{
						return this._directory;
					}
					if (!this.Installed)
					{
						return null;
					}
					if (this.workshop.ugc.GetItemInstallInfo(this.Id, out num, out str, out num1))
					{
						this._directory = new DirectoryInfo(str);
						this.Size = num;
						bool exists = this._directory.Exists;
					}
					return this._directory;
				}
			}

			public string DiscussUrl
			{
				get
				{
					return string.Format("http://steamcommunity.com/sharedfiles/filedetails/discussions/{0}", this.Id);
				}
			}

			public bool Downloading
			{
				get
				{
					return (this.State & ItemState.Downloading) != ItemState.None;
				}
			}

			public bool DownloadPending
			{
				get
				{
					return (this.State & ItemState.DownloadPending) != ItemState.None;
				}
			}

			public double DownloadProgress
			{
				get
				{
					this.UpdateDownloadProgress();
					if (this._BytesTotal == 0)
					{
						return 0;
					}
					return (double)((float)this._BytesDownloaded) / (double)((float)this._BytesTotal);
				}
			}

			public int FavouriteCount
			{
				get;
				internal set;
			}

			public int FollowerCount
			{
				get;
				internal set;
			}

			public ulong Id
			{
				get;
				private set;
			}

			public bool Installed
			{
				get
				{
					return (this.State & ItemState.Installed) != ItemState.None;
				}
			}

			public DateTime Modified
			{
				get;
				private set;
			}

			public bool NeedsUpdate
			{
				get
				{
					return (this.State & ItemState.NeedsUpdate) != ItemState.None;
				}
			}

			public ulong OwnerId
			{
				get;
				private set;
			}

			public string OwnerName
			{
				get
				{
					if (this._ownerName == null && this.workshop.friends != null)
					{
						this._ownerName = this.workshop.friends.GetName(this.OwnerId);
						if (this._ownerName == "[unknown]")
						{
							this._ownerName = null;
							return string.Empty;
						}
					}
					if (this._ownerName == null)
					{
						return string.Empty;
					}
					return this._ownerName;
				}
			}

			public string PreviewImageUrl
			{
				get;
				internal set;
			}

			public int ReportScore
			{
				get;
				internal set;
			}

			public float Score
			{
				get;
				private set;
			}

			public ulong Size
			{
				get;
				private set;
			}

			public string StartsUrl
			{
				get
				{
					return string.Format("http://steamcommunity.com/sharedfiles/filedetails/stats/{0}", this.Id);
				}
			}

			private ItemState State
			{
				get
				{
					return (ItemState)this.workshop.ugc.GetItemState(this.Id);
				}
			}

			public bool Subscribed
			{
				get
				{
					return (this.State & ItemState.Subscribed) != ItemState.None;
				}
			}

			public int SubscriptionCount
			{
				get;
				internal set;
			}

			public string[] Tags
			{
				get;
				private set;
			}

			public string Title
			{
				get;
				private set;
			}

			public string Url
			{
				get
				{
					return string.Format("http://steamcommunity.com/sharedfiles/filedetails/?source=Facepunch.Steamworks&id={0}", this.Id);
				}
			}

			public uint VotesDown
			{
				get;
				private set;
			}

			public uint VotesUp
			{
				get;
				private set;
			}

			public int WebsiteViews
			{
				get;
				internal set;
			}

			public Item(ulong Id, Workshop workshop)
			{
				this.Id = Id;
				this.workshop = workshop;
			}

			public bool Download(bool highPriority = true)
			{
				if (this.Installed)
				{
					return true;
				}
				if (this.Downloading)
				{
					return true;
				}
				if (!this.workshop.ugc.DownloadItem(this.Id, highPriority))
				{
					Console.WriteLine("Download Failed");
					return false;
				}
				this.workshop.OnFileDownloaded += new Action<ulong, Facepunch.Steamworks.Callbacks.Result>(this.OnFileDownloaded);
				this.workshop.OnItemInstalled += new Action<ulong>(this.OnItemInstalled);
				return true;
			}

			public Workshop.Editor Edit()
			{
				return this.workshop.EditItem(this.Id);
			}

			internal static Workshop.Item From(SteamUGCDetails_t details, Workshop workshop)
			{
				Workshop.Item item = new Workshop.Item(details.PublishedFileId, workshop)
				{
					Title = details.Title,
					Description = details.Description,
					OwnerId = details.SteamIDOwner,
					Tags = (
						from x in details.Tags.Split(new char[] { ',' })
						select x.ToLower()).ToArray<string>(),
					Score = details.Score,
					VotesUp = details.VotesUp,
					VotesDown = details.VotesDown,
					Modified = Utility.Epoch.ToDateTime(details.TimeUpdated),
					Created = Utility.Epoch.ToDateTime(details.TimeCreated)
				};
				return item;
			}

			private void OnFileDownloaded(ulong fileid, Facepunch.Steamworks.Callbacks.Result result)
			{
				if (fileid != this.Id)
				{
					return;
				}
				this.workshop.OnFileDownloaded -= new Action<ulong, Facepunch.Steamworks.Callbacks.Result>(this.OnFileDownloaded);
			}

			private void OnItemInstalled(ulong fileid)
			{
				if (fileid != this.Id)
				{
					return;
				}
				this.workshop.OnItemInstalled -= new Action<ulong>(this.OnItemInstalled);
			}

			public void Subscribe()
			{
				this.workshop.ugc.SubscribeItem(this.Id, null);
				this.SubscriptionCount = this.SubscriptionCount + 1;
			}

			public void UnSubscribe()
			{
				this.workshop.ugc.UnsubscribeItem(this.Id, null);
				this.SubscriptionCount = this.SubscriptionCount - 1;
			}

			internal void UpdateDownloadProgress()
			{
				this.workshop.ugc.GetItemDownloadInfo(this.Id, out this._BytesDownloaded, out this._BytesTotal);
			}

			public void VoteDown()
			{
				if (this.YourVote == -1)
				{
					return;
				}
				if (this.YourVote == 1)
				{
					this.VotesUp = this.VotesUp - 1;
				}
				this.VotesDown = this.VotesDown + 1;
				this.workshop.ugc.SetUserItemVote(this.Id, false, null);
				this.YourVote = -1;
			}

			public void VoteUp()
			{
				if (this.YourVote == 1)
				{
					return;
				}
				if (this.YourVote == -1)
				{
					this.VotesDown = this.VotesDown - 1;
				}
				this.VotesUp = this.VotesUp + 1;
				this.workshop.ugc.SetUserItemVote(this.Id, true, null);
				this.YourVote = 1;
			}
		}

		private enum ItemStatistic : uint
		{
			NumSubscriptions,
			NumFavorites,
			NumFollowers,
			NumUniqueSubscriptions,
			NumUniqueFavorites,
			NumUniqueFollowers,
			NumUniqueWebsiteViews,
			ReportScore
		}

		public enum ItemType
		{
			Community,
			Microtransaction,
			Collection,
			Art,
			Video,
			Screenshot,
			Game,
			Software,
			Concept,
			WebGuide,
			IntegratedGuide,
			Merch,
			ControllerBinding,
			SteamworksAccessInvite,
			SteamVideo,
			GameManagedItem
		}

		public enum Order
		{
			RankedByVote,
			RankedByPublicationDate,
			AcceptedForGameRankedByAcceptanceDate,
			RankedByTrend,
			FavoritedByFriendsRankedByPublicationDate,
			CreatedByFriendsRankedByPublicationDate,
			RankedByNumTimesReported,
			CreatedByFollowedUsersRankedByPublicationDate,
			NotYetRated,
			RankedByTotalVotesAsc,
			RankedByVotesUp,
			RankedByTextSearch,
			RankedByTotalUniqueSubscriptions
		}

		public class Query : IDisposable
		{
			internal const int SteamResponseSize = 50;

			internal UGCQueryHandle_t Handle;

			internal CallbackHandle Callback;

			public Action<Workshop.Query> OnResult;

			internal Workshop workshop;

			internal Friends friends;

			private int _resultPage;

			private int _resultsRemain;

			private int _resultSkip;

			private List<Workshop.Item> _results;

			public uint AppId
			{
				get;
				set;
			}

			public List<string> ExcludeTags
			{
				get;
				set;
			}

			public List<ulong> FileId
			{
				get;
				set;
			}

			public bool IsRunning
			{
				get
				{
					return this.Callback != null;
				}
			}

			public Workshop.Item[] Items
			{
				get;
				set;
			}

			public Workshop.Order Order
			{
				get;
				set;
			}

			public int Page
			{
				get;
				set;
			}

			public int PerPage
			{
				get;
				set;
			}

			public Workshop.QueryType QueryType
			{
				get;
				set;
			}

			public int RankedByTrendDays
			{
				get;
				set;
			}

			public bool RequireAllTags
			{
				get;
				set;
			}

			public List<string> RequireTags
			{
				get;
				set;
			}

			public string SearchText
			{
				get;
				set;
			}

			public int TotalResults
			{
				get;
				set;
			}

			public uint UploaderAppId
			{
				get;
				set;
			}

			public ulong? UserId
			{
				get;
				set;
			}

			public Workshop.UserQueryType UserQueryType
			{
				get;
				set;
			}

			public Query()
			{
			}

			public void Block()
			{
				this.workshop.steamworks.Update();
				while (this.IsRunning)
				{
					Thread.Sleep(10);
					this.workshop.steamworks.Update();
				}
			}

			public void Dispose()
			{
			}

			private int GetStat(ulong handle, int index, Workshop.ItemStatistic stat)
			{
				ulong num = (ulong)0;
				if (!this.workshop.ugc.GetQueryUGCStatistic(handle, (uint)index, (SteamNative.ItemStatistic)stat, out num))
				{
					return 0;
				}
				return (int)num;
			}

			private void ResultCallback(SteamUGCQueryCompleted_t data, bool bFailed)
			{
				if (bFailed)
				{
					throw new Exception("bFailed!");
				}
				int num = 0;
				for (int i = 0; (long)i < (ulong)data.NumResultsReturned; i++)
				{
					if (this._resultSkip <= 0)
					{
						SteamUGCDetails_t steamUGCDetailsT = new SteamUGCDetails_t();
						if (this.workshop.ugc.GetQueryUGCResult(data.Handle, (uint)i, ref steamUGCDetailsT) && !this._results.Any<Workshop.Item>((Workshop.Item x) => x.Id == steamUGCDetailsT.PublishedFileId))
						{
							Workshop.Item stat = Workshop.Item.From(steamUGCDetailsT, this.workshop);
							stat.SubscriptionCount = this.GetStat(data.Handle, i, Workshop.ItemStatistic.NumSubscriptions);
							stat.FavouriteCount = this.GetStat(data.Handle, i, Workshop.ItemStatistic.NumFavorites);
							stat.FollowerCount = this.GetStat(data.Handle, i, Workshop.ItemStatistic.NumFollowers);
							stat.WebsiteViews = this.GetStat(data.Handle, i, Workshop.ItemStatistic.NumUniqueWebsiteViews);
							stat.ReportScore = this.GetStat(data.Handle, i, Workshop.ItemStatistic.ReportScore);
							string str = null;
							if (this.workshop.ugc.GetQueryUGCPreviewURL(data.Handle, (uint)i, out str))
							{
								stat.PreviewImageUrl = str;
							}
							this._results.Add(stat);
							this._resultsRemain--;
							num++;
							if (this._resultsRemain <= 0)
							{
								break;
							}
						}
					}
					else
					{
						this._resultSkip--;
					}
				}
				this.TotalResults = ((long)this.TotalResults > (ulong)data.TotalMatchingResults ? this.TotalResults : (int)data.TotalMatchingResults);
				this.Callback.Dispose();
				this.Callback = null;
				this._resultPage++;
				if (this._resultsRemain > 0 && num > 0)
				{
					this.RunInternal();
					return;
				}
				this.Items = this._results.ToArray();
				if (this.OnResult != null)
				{
					this.OnResult(this);
				}
			}

			public void Run()
			{
				if (this.Callback != null)
				{
					return;
				}
				if (this.Page <= 0)
				{
					throw new Exception("Page should be 1 or above");
				}
				int page = (this.Page - 1) * this.PerPage;
				this.TotalResults = 0;
				this._resultSkip = page % 50;
				this._resultsRemain = this.PerPage;
				this._resultPage = (int)Math.Floor((double)((float)page / 50f));
				this._results = new List<Workshop.Item>();
				this.RunInternal();
			}

			private void RunInternal()
			{
				if (this.FileId.Count != 0)
				{
					PublishedFileId_t[] array = (
						from x in this.FileId
						select x).ToArray<PublishedFileId_t>();
					this._resultsRemain = (int)array.Length;
					this.Handle = this.workshop.ugc.CreateQueryUGCDetailsRequest(array);
				}
				else if (!this.UserId.HasValue)
				{
					this.Handle = this.workshop.ugc.CreateQueryAllUGCRequest((UGCQuery)this.Order, (UGCMatchingUGCType)this.QueryType, this.UploaderAppId, this.AppId, (uint)(this._resultPage + 1));
				}
				else
				{
					uint value = (uint)(this.UserId.Value & (ulong)-1);
					this.Handle = this.workshop.ugc.CreateQueryUserUGCRequest(value, (UserUGCList)this.UserQueryType, (UGCMatchingUGCType)this.QueryType, UserUGCListSortOrder.LastUpdatedDesc, this.UploaderAppId, this.AppId, (uint)(this._resultPage + 1));
				}
				if (!string.IsNullOrEmpty(this.SearchText))
				{
					this.workshop.ugc.SetSearchText(this.Handle, this.SearchText);
				}
				foreach (string requireTag in this.RequireTags)
				{
					this.workshop.ugc.AddRequiredTag(this.Handle, requireTag);
				}
				if (this.RequireTags.Count > 0)
				{
					this.workshop.ugc.SetMatchAnyTag(this.Handle, !this.RequireAllTags);
				}
				if (this.RankedByTrendDays > 0)
				{
					this.workshop.ugc.SetRankedByTrendDays(this.Handle, (uint)this.RankedByTrendDays);
				}
				foreach (string excludeTag in this.ExcludeTags)
				{
					this.workshop.ugc.AddExcludedTag(this.Handle, excludeTag);
				}
				this.Callback = this.workshop.ugc.SendQueryUGCRequest(this.Handle, new Action<SteamUGCQueryCompleted_t, bool>(this.ResultCallback));
			}
		}

		public enum QueryType
		{
			Items,
			MicrotransactionItems,
			SubscriptionItems,
			Collections,
			Artwork,
			Videos,
			Screenshots,
			AllGuides,
			WebGuides,
			IntegratedGuides,
			UsableInGame,
			ControllerBindings,
			GameManagedItems
		}

		public enum UserQueryType : uint
		{
			Published,
			VotedOn,
			VotedUp,
			VotedDown,
			WillVoteLater,
			Favorited,
			Subscribed,
			UsedOrPlayed,
			Followed
		}
	}
}