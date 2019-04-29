using Facepunch.Steamworks.Callbacks;
using Facepunch.Steamworks.Interop;
using SteamNative;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Facepunch.Steamworks
{
	public class Leaderboard : IDisposable
	{
		private readonly static int[] subEntriesBuffer;

		internal ulong BoardId;

		internal Client client;

		private readonly Queue<Action> _onCreated = new Queue<Action>();

		public Leaderboard.Entry[] Results;

		public Action OnBoardInformation;

		[ThreadStatic]
		private static List<Leaderboard.Entry> _sEntryBuffer;

		public bool IsError
		{
			get;
			private set;
		}

		public bool IsQuerying
		{
			get;
			private set;
		}

		public bool IsValid
		{
			get
			{
				return this.BoardId != (long)0;
			}
		}

		public string Name
		{
			get;
			private set;
		}

		public int TotalEntries
		{
			get;
			private set;
		}

		static Leaderboard()
		{
			Leaderboard.subEntriesBuffer = new int[512];
		}

		internal Leaderboard(Client c)
		{
			this.client = c;
		}

		public bool AddScore(bool onlyIfBeatsOldScore, int score, params int[] subscores)
		{
			if (this.IsError)
			{
				return false;
			}
			if (!this.IsValid)
			{
				return this.DeferOnCreated(() => this.AddScore(onlyIfBeatsOldScore, score, subscores), null);
			}
			LeaderboardUploadScoreMethod leaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.ForceUpdate;
			if (onlyIfBeatsOldScore)
			{
				leaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.KeepBest;
			}
			this.client.native.userstats.UploadLeaderboardScore(this.BoardId, leaderboardUploadScoreMethod, score, subscores, (int)subscores.Length, null);
			return true;
		}

		public bool AddScore(bool onlyIfBeatsOldScore, int score, int[] subscores = null, Leaderboard.AddScoreCallback onSuccess = null, FailureCallback onFailure = null)
		{
			int[] numArray = subscores;
			if (this.IsError)
			{
				return false;
			}
			if (!this.IsValid)
			{
				return this.DeferOnCreated(() => this.AddScore(onlyIfBeatsOldScore, score, numArray, onSuccess, onFailure), onFailure);
			}
			if (numArray == null)
			{
				numArray = new int[0];
			}
			LeaderboardUploadScoreMethod leaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.ForceUpdate;
			if (onlyIfBeatsOldScore)
			{
				leaderboardUploadScoreMethod = LeaderboardUploadScoreMethod.KeepBest;
			}
			this.client.native.userstats.UploadLeaderboardScore(this.BoardId, leaderboardUploadScoreMethod, score, numArray, (int)numArray.Length, (LeaderboardScoreUploaded_t result, bool error) => {
				if (error || result.Success == 0)
				{
					FailureCallback failureCallback = onFailure;
					if (failureCallback == null)
					{
						return;
					}
					failureCallback((error ? Facepunch.Steamworks.Callbacks.Result.IOFailure : Facepunch.Steamworks.Callbacks.Result.Fail));
					return;
				}
				Leaderboard.AddScoreCallback addScoreCallback = onSuccess;
				if (addScoreCallback == null)
				{
					return;
				}
				addScoreCallback(new Leaderboard.AddScoreResult()
				{
					Score = result.Score,
					ScoreChanged = result.ScoreChanged != 0,
					GlobalRankNew = result.GlobalRankNew,
					GlobalRankPrevious = result.GlobalRankPrevious
				});
			});
			return true;
		}

		public bool AttachRemoteFile(RemoteFile file, Leaderboard.AttachRemoteFileCallback onSuccess = null, FailureCallback onFailure = null)
		{
			if (this.IsError)
			{
				return false;
			}
			if (!this.IsValid)
			{
				return this.DeferOnCreated(() => this.AttachRemoteFile(file, onSuccess, onFailure), onFailure);
			}
			if (!file.IsShared)
			{
				file.Share(() => {
					if (!file.IsShared || !this.AttachRemoteFile(file, onSuccess, onFailure))
					{
						FailureCallback failureCallback = onFailure;
						if (failureCallback == null)
						{
							return;
						}
						failureCallback(2);
					}
				}, onFailure);
				return true;
			}
			return this.client.native.userstats.AttachLeaderboardUGC(this.BoardId, file.UGCHandle, (LeaderboardUGCSet_t result, bool error) => {
				if (!error && result.Result == SteamNative.Result.OK)
				{
					Leaderboard.AttachRemoteFileCallback attachRemoteFileCallback = onSuccess;
					if (attachRemoteFileCallback == null)
					{
						return;
					}
					attachRemoteFileCallback();
					return;
				}
				FailureCallback failureCallback = onFailure;
				if (failureCallback == null)
				{
					return;
				}
				failureCallback(((int)result.Result == 0 ? Facepunch.Steamworks.Callbacks.Result.IOFailure : result.Result));
			}).IsValid;
		}

		private bool DeferOnCreated(Action onValid, FailureCallback onFailure = null)
		{
			if (this.IsValid || this.IsError)
			{
				return false;
			}
			this._onCreated.Enqueue(new Action(() => {
				if (this.IsValid)
				{
					onValid();
					return;
				}
				FailureCallback failureCallback = onFailure;
				if (failureCallback == null)
				{
					return;
				}
				failureCallback(2);
			}));
			return true;
		}

		private void DispatchOnCreatedCallbacks()
		{
			while (this._onCreated.Count > 0)
			{
				this._onCreated.Dequeue()();
			}
		}

		public void Dispose()
		{
			this.client = null;
		}

		public bool FetchScores(Leaderboard.RequestType RequestType, int start, int end)
		{
			if (!this.IsValid)
			{
				return false;
			}
			if (this.IsQuerying)
			{
				return false;
			}
			this.client.native.userstats.DownloadLeaderboardEntries(this.BoardId, (LeaderboardDataRequest)RequestType, start, end, new Action<LeaderboardScoresDownloaded_t, bool>(this.OnScores));
			this.Results = null;
			this.IsQuerying = true;
			return true;
		}

		public bool FetchScores(Leaderboard.RequestType RequestType, int start, int end, Leaderboard.FetchScoresCallback onSuccess, FailureCallback onFailure = null)
		{
			if (this.IsError)
			{
				return false;
			}
			if (!this.IsValid)
			{
				return this.DeferOnCreated(() => this.FetchScores(RequestType, start, end, onSuccess, onFailure), onFailure);
			}
			this.client.native.userstats.DownloadLeaderboardEntries(this.BoardId, (LeaderboardDataRequest)RequestType, start, end, (LeaderboardScoresDownloaded_t result, bool error) => {
				if (error)
				{
					FailureCallback failureCallback = onFailure;
					if (failureCallback == null)
					{
						return;
					}
					failureCallback(37);
					return;
				}
				if (Leaderboard._sEntryBuffer != null)
				{
					Leaderboard._sEntryBuffer.Clear();
				}
				else
				{
					Leaderboard._sEntryBuffer = new List<Leaderboard.Entry>();
				}
				this.ReadScores(result, Leaderboard._sEntryBuffer);
				onSuccess(Leaderboard._sEntryBuffer.ToArray());
			});
			return true;
		}

		public unsafe bool FetchUsersScores(Leaderboard.RequestType RequestType, ulong[] steamIds, Leaderboard.FetchScoresCallback onSuccess, FailureCallback onFailure = null)
		{
			// 
			// Current member / type: System.Boolean Facepunch.Steamworks.Leaderboard::FetchUsersScores(Facepunch.Steamworks.Leaderboard/RequestType,System.UInt64[],Facepunch.Steamworks.Leaderboard/FetchScoresCallback,Facepunch.Steamworks.Callbacks.FailureCallback)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Boolean FetchUsersScores(Facepunch.Steamworks.Leaderboard/RequestType,System.UInt64[],Facepunch.Steamworks.Leaderboard/FetchScoresCallback,Facepunch.Steamworks.Callbacks.FailureCallback)
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

		public ulong GetBoardId()
		{
			return this.BoardId;
		}

		internal void OnBoardCreated(LeaderboardFindResult_t result, bool error)
		{
			Console.WriteLine(string.Format("result.LeaderboardFound: {0}", result.LeaderboardFound));
			Console.WriteLine(string.Format("result.SteamLeaderboard: {0}", result.SteamLeaderboard));
			if (error || result.LeaderboardFound == 0)
			{
				this.IsError = true;
			}
			else
			{
				this.BoardId = result.SteamLeaderboard;
				if (this.IsValid)
				{
					this.Name = this.client.native.userstats.GetLeaderboardName(this.BoardId);
					this.TotalEntries = this.client.native.userstats.GetLeaderboardEntryCount(this.BoardId);
					Action onBoardInformation = this.OnBoardInformation;
					if (onBoardInformation != null)
					{
						onBoardInformation();
					}
					else
					{
					}
				}
			}
			this.DispatchOnCreatedCallbacks();
		}

		private void OnScores(LeaderboardScoresDownloaded_t result, bool error)
		{
			this.IsQuerying = false;
			if (this.client == null)
			{
				return;
			}
			if (error)
			{
				return;
			}
			this.TotalEntries = this.client.native.userstats.GetLeaderboardEntryCount(this.BoardId);
			List<Leaderboard.Entry> entries = new List<Leaderboard.Entry>();
			this.ReadScores(result, entries);
			this.Results = entries.ToArray();
		}

		private unsafe void ReadScores(LeaderboardScoresDownloaded_t result, List<Leaderboard.Entry> dest)
		{
			// 
			// Current member / type: System.Void Facepunch.Steamworks.Leaderboard::ReadScores(SteamNative.LeaderboardScoresDownloaded_t,System.Collections.Generic.List`1<Facepunch.Steamworks.Leaderboard/Entry>)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Facepunch.Steamworks.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void ReadScores(SteamNative.LeaderboardScoresDownloaded_t,System.Collections.Generic.List<Facepunch.Steamworks.Leaderboard/Entry>)
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

		public delegate void AddScoreCallback(Leaderboard.AddScoreResult result);

		public struct AddScoreResult
		{
			public int Score;

			public bool ScoreChanged;

			public int GlobalRankNew;

			public int GlobalRankPrevious;
		}

		public delegate void AttachRemoteFileCallback();

		public struct Entry
		{
			public ulong SteamId;

			public int Score;

			public int[] SubScores;

			public int GlobalRank;

			public RemoteFile AttachedFile;

			public string Name;
		}

		public delegate void FetchScoresCallback(Leaderboard.Entry[] results);

		public enum RequestType
		{
			Global,
			GlobalAroundUser,
			Friends
		}
	}
}