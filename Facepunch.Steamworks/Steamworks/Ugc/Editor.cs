using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Ugc
{
	public struct Editor
	{
		private PublishedFileId fileId;

		private bool creatingNew;

		private WorkshopFileType creatingType;

		private AppId consumerAppId;

		private string Title;

		private string Description;

		private string MetaData;

		private string ChangeLog;

		private string Language;

		private string PreviewFile;

		private DirectoryInfo ContentFolder;

		private RemoteStoragePublishedFileVisibility? Visibility;

		private List<string> Tags;

		public static Editor NewCommunityFile
		{
			get
			{
				return new Editor(WorkshopFileType.First);
			}
		}

		public static Editor NewMicrotransactionFile
		{
			get
			{
				return new Editor(WorkshopFileType.Microtransaction);
			}
		}

		internal Editor(WorkshopFileType filetype)
		{
			this = new Editor()
			{
				creatingNew = true,
				creatingType = filetype
			};
		}

		public Editor(PublishedFileId fileId)
		{
			this = new Editor()
			{
				fileId = fileId
			};
		}

		public Editor ForAppId(AppId id)
		{
			this.consumerAppId = id;
			return this;
		}

		public Editor InLanguage(string t)
		{
			this.Language = t;
			return this;
		}

		public async Task<PublishResult> SubmitAsync(IProgress<float> progress = null)
		{
			PublishResult publishResult;
			bool flag;
			float single;
			PublishResult result = new PublishResult();
			IProgress<float> progress1 = progress;
			if (progress1 != null)
			{
				progress1.Report(0f);
			}
			else
			{
			}
			if (this.consumerAppId == 0)
			{
				this.consumerAppId = SteamClient.AppId;
			}
			if (this.creatingNew)
			{
				result.Result = Result.Fail;
				CreateItemResult_t? nullable = await SteamUGC.Internal.CreateItem(this.consumerAppId, this.creatingType);
				CreateItemResult_t? nullable1 = nullable;
				nullable = null;
				if (nullable1.HasValue)
				{
					result.Result = nullable1.Value.Result;
					if (result.Result == Result.OK)
					{
						this.fileId = nullable1.Value.PublishedFileId;
						result.NeedsWorkshopAgreement = nullable1.Value.UserNeedsToAcceptWorkshopLegalAgreement;
						result.FileId = this.fileId;
						nullable1 = null;
					}
					else
					{
						publishResult = result;
						return publishResult;
					}
				}
				else
				{
					publishResult = result;
					return publishResult;
				}
			}
			result.FileId = this.fileId;
			UGCUpdateHandle_t uGCUpdateHandleT = SteamUGC.Internal.StartItemUpdate(this.consumerAppId, this.fileId);
			if (uGCUpdateHandleT != (long)-1)
			{
				if ((object)this.Title != (object)null)
				{
					SteamUGC.Internal.SetItemTitle(uGCUpdateHandleT, this.Title);
				}
				if ((object)this.Description != (object)null)
				{
					SteamUGC.Internal.SetItemDescription(uGCUpdateHandleT, this.Description);
				}
				if ((object)this.MetaData != (object)null)
				{
					SteamUGC.Internal.SetItemMetadata(uGCUpdateHandleT, this.MetaData);
				}
				if ((object)this.Language != (object)null)
				{
					SteamUGC.Internal.SetItemUpdateLanguage(uGCUpdateHandleT, this.Language);
				}
				if (this.ContentFolder != null)
				{
					SteamUGC.Internal.SetItemContent(uGCUpdateHandleT, this.ContentFolder.FullName);
				}
				if ((object)this.PreviewFile != (object)null)
				{
					SteamUGC.Internal.SetItemPreview(uGCUpdateHandleT, this.PreviewFile);
				}
				if (this.Visibility.HasValue)
				{
					SteamUGC.Internal.SetItemVisibility(uGCUpdateHandleT, this.Visibility.Value);
				}
				flag = (this.Tags == null ? false : this.Tags.Count > 0);
				if (flag)
				{
					SteamParamStringArray steamParamStringArray = SteamParamStringArray.From(this.Tags.ToArray());
					try
					{
						SteamParamStringArray_t value = steamParamStringArray.Value;
						SteamUGC.Internal.SetItemTags(uGCUpdateHandleT, ref value);
					}
					finally
					{
						((IDisposable)steamParamStringArray).Dispose();
					}
					steamParamStringArray = new SteamParamStringArray();
				}
				result.Result = Result.Fail;
				if (this.ChangeLog == null)
				{
					this.ChangeLog = "";
				}
				Task<SubmitItemUpdateResult_t?> task = SteamUGC.Internal.SubmitItemUpdate(uGCUpdateHandleT, this.ChangeLog);
				while (!task.IsCompleted)
				{
					if (progress != null)
					{
						ulong num = (ulong)0;
						ulong num1 = (ulong)0;
						switch (SteamUGC.Internal.GetItemUpdateProgress(uGCUpdateHandleT, ref num1, ref num))
						{
							case ItemUpdateStatus.PreparingConfig:
							{
								IProgress<float> progress2 = progress;
								if (progress2 != null)
								{
									progress2.Report(0.1f);
								}
								else
								{
								}
								break;
							}
							case ItemUpdateStatus.PreparingContent:
							{
								IProgress<float> progress3 = progress;
								if (progress3 != null)
								{
									progress3.Report(0.2f);
								}
								else
								{
								}
								break;
							}
							case ItemUpdateStatus.UploadingContent:
							{
								single = (num > (long)0 ? (float)((float)num1) / (float)((float)num) : 0f);
								float single1 = single;
								IProgress<float> progress4 = progress;
								if (progress4 != null)
								{
									progress4.Report(0.2f + single1 * 0.7f);
								}
								else
								{
								}
								break;
							}
							case ItemUpdateStatus.UploadingPreviewFile:
							{
								IProgress<float> progress5 = progress;
								if (progress5 != null)
								{
									progress5.Report(8f);
								}
								else
								{
								}
								break;
							}
							case ItemUpdateStatus.CommittingChanges:
							{
								IProgress<float> progress6 = progress;
								if (progress6 != null)
								{
									progress6.Report(1f);
								}
								else
								{
								}
								break;
							}
						}
					}
					await Task.Delay(16);
				}
				IProgress<float> progress7 = progress;
				if (progress7 != null)
				{
					progress7.Report(1f);
				}
				else
				{
				}
				SubmitItemUpdateResult_t? result1 = task.Result;
				if (result1.HasValue)
				{
					result.Result = result1.Value.Result;
					if (result.Result == Result.OK)
					{
						result.NeedsWorkshopAgreement = result1.Value.UserNeedsToAcceptWorkshopLegalAgreement;
						result.FileId = this.fileId;
						task = null;
						result1 = null;
						publishResult = result;
					}
					else
					{
						publishResult = result;
					}
				}
				else
				{
					publishResult = result;
				}
			}
			else
			{
				publishResult = result;
			}
			return publishResult;
		}

		public Editor WithChangeLog(string t)
		{
			this.ChangeLog = t;
			return this;
		}

		public Editor WithContent(DirectoryInfo t)
		{
			this.ContentFolder = t;
			return this;
		}

		public Editor WithContent(string folderName)
		{
			return this.WithContent(new DirectoryInfo(folderName));
		}

		public Editor WithDescription(string t)
		{
			this.Description = t;
			return this;
		}

		public Editor WithFriendsOnlyVisibility()
		{
			this.Visibility = new RemoteStoragePublishedFileVisibility?(RemoteStoragePublishedFileVisibility.FriendsOnly);
			return this;
		}

		public Editor WithMetaData(string t)
		{
			this.MetaData = t;
			return this;
		}

		public Editor WithPreviewFile(string t)
		{
			this.PreviewFile = t;
			return this;
		}

		public Editor WithPrivateVisibility()
		{
			this.Visibility = new RemoteStoragePublishedFileVisibility?(RemoteStoragePublishedFileVisibility.Private);
			return this;
		}

		public Editor WithPublicVisibility()
		{
			this.Visibility = new RemoteStoragePublishedFileVisibility?(RemoteStoragePublishedFileVisibility.Public);
			return this;
		}

		public Editor WithTag(string tag)
		{
			if (this.Tags == null)
			{
				this.Tags = new List<string>();
			}
			this.Tags.Add(tag);
			return this;
		}

		public Editor WithTitle(string t)
		{
			this.Title = t;
			return this;
		}
	}
}