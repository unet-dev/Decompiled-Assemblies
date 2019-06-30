using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamUGCQueryCompleted_t
	{
		internal ulong Handle;

		internal Steamworks.Result Result;

		internal uint NumResultsReturned;

		internal uint TotalMatchingResults;

		internal bool CachedData;

		internal string NextCursor;

		internal readonly static int StructSize;

		private static Action<SteamUGCQueryCompleted_t> actionClient;

		private static Action<SteamUGCQueryCompleted_t> actionServer;

		static SteamUGCQueryCompleted_t()
		{
			SteamUGCQueryCompleted_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamUGCQueryCompleted_t) : typeof(SteamUGCQueryCompleted_t.Pack8)));
		}

		internal static SteamUGCQueryCompleted_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamUGCQueryCompleted_t)Marshal.PtrToStructure(p, typeof(SteamUGCQueryCompleted_t)) : (SteamUGCQueryCompleted_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamUGCQueryCompleted_t.Pack8)));
		}

		public static async Task<SteamUGCQueryCompleted_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamUGCQueryCompleted_t? nullable;
			bool flag = false;
			while (!SteamUtils.IsCallComplete(handle, out flag))
			{
				await Task.Delay(1);
				if ((SteamClient.IsValid ? false : !SteamServer.IsValid))
				{
					nullable = null;
					return nullable;
				}
			}
			if (!flag)
			{
				IntPtr intPtr = Marshal.AllocHGlobal(SteamUGCQueryCompleted_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamUGCQueryCompleted_t.StructSize, 3401, ref flag) | flag))
					{
						nullable = new SteamUGCQueryCompleted_t?(SteamUGCQueryCompleted_t.Fill(intPtr));
					}
					else
					{
						nullable = null;
					}
				}
				finally
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			else
			{
				nullable = null;
			}
			return nullable;
		}

		public static void Install(Action<SteamUGCQueryCompleted_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamUGCQueryCompleted_t.OnClient), SteamUGCQueryCompleted_t.StructSize, 3401, false);
				SteamUGCQueryCompleted_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamUGCQueryCompleted_t.OnServer), SteamUGCQueryCompleted_t.StructSize, 3401, true);
				SteamUGCQueryCompleted_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamUGCQueryCompleted_t> action = SteamUGCQueryCompleted_t.actionClient;
			if (action != null)
			{
				action(SteamUGCQueryCompleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamUGCQueryCompleted_t> action = SteamUGCQueryCompleted_t.actionServer;
			if (action != null)
			{
				action(SteamUGCQueryCompleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong Handle;

			internal Steamworks.Result Result;

			internal uint NumResultsReturned;

			internal uint TotalMatchingResults;

			internal bool CachedData;

			internal string NextCursor;

			public static implicit operator SteamUGCQueryCompleted_t(SteamUGCQueryCompleted_t.Pack8 d)
			{
				SteamUGCQueryCompleted_t steamUGCQueryCompletedT = new SteamUGCQueryCompleted_t()
				{
					Handle = d.Handle,
					Result = d.Result,
					NumResultsReturned = d.NumResultsReturned,
					TotalMatchingResults = d.TotalMatchingResults,
					CachedData = d.CachedData,
					NextCursor = d.NextCursor
				};
				return steamUGCQueryCompletedT;
			}

			public static implicit operator Pack8(SteamUGCQueryCompleted_t d)
			{
				SteamUGCQueryCompleted_t.Pack8 pack8 = new SteamUGCQueryCompleted_t.Pack8()
				{
					Handle = d.Handle,
					Result = d.Result,
					NumResultsReturned = d.NumResultsReturned,
					TotalMatchingResults = d.TotalMatchingResults,
					CachedData = d.CachedData,
					NextCursor = d.NextCursor
				};
				return pack8;
			}
		}
	}
}