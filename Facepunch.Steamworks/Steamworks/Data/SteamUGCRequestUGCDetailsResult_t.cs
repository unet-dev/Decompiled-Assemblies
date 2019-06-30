using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamUGCRequestUGCDetailsResult_t
	{
		internal SteamUGCDetails_t Details;

		internal bool CachedData;

		internal readonly static int StructSize;

		private static Action<SteamUGCRequestUGCDetailsResult_t> actionClient;

		private static Action<SteamUGCRequestUGCDetailsResult_t> actionServer;

		static SteamUGCRequestUGCDetailsResult_t()
		{
			SteamUGCRequestUGCDetailsResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamUGCRequestUGCDetailsResult_t) : typeof(SteamUGCRequestUGCDetailsResult_t.Pack8)));
		}

		internal static SteamUGCRequestUGCDetailsResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamUGCRequestUGCDetailsResult_t)Marshal.PtrToStructure(p, typeof(SteamUGCRequestUGCDetailsResult_t)) : (SteamUGCRequestUGCDetailsResult_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamUGCRequestUGCDetailsResult_t.Pack8)));
		}

		public static async Task<SteamUGCRequestUGCDetailsResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamUGCRequestUGCDetailsResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamUGCRequestUGCDetailsResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamUGCRequestUGCDetailsResult_t.StructSize, 3402, ref flag) | flag))
					{
						nullable = new SteamUGCRequestUGCDetailsResult_t?(SteamUGCRequestUGCDetailsResult_t.Fill(intPtr));
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

		public static void Install(Action<SteamUGCRequestUGCDetailsResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamUGCRequestUGCDetailsResult_t.OnClient), SteamUGCRequestUGCDetailsResult_t.StructSize, 3402, false);
				SteamUGCRequestUGCDetailsResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamUGCRequestUGCDetailsResult_t.OnServer), SteamUGCRequestUGCDetailsResult_t.StructSize, 3402, true);
				SteamUGCRequestUGCDetailsResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamUGCRequestUGCDetailsResult_t> action = SteamUGCRequestUGCDetailsResult_t.actionClient;
			if (action != null)
			{
				action(SteamUGCRequestUGCDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamUGCRequestUGCDetailsResult_t> action = SteamUGCRequestUGCDetailsResult_t.actionServer;
			if (action != null)
			{
				action(SteamUGCRequestUGCDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal SteamUGCDetails_t Details;

			internal bool CachedData;

			public static implicit operator SteamUGCRequestUGCDetailsResult_t(SteamUGCRequestUGCDetailsResult_t.Pack8 d)
			{
				SteamUGCRequestUGCDetailsResult_t steamUGCRequestUGCDetailsResultT = new SteamUGCRequestUGCDetailsResult_t()
				{
					Details = d.Details,
					CachedData = d.CachedData
				};
				return steamUGCRequestUGCDetailsResultT;
			}

			public static implicit operator Pack8(SteamUGCRequestUGCDetailsResult_t d)
			{
				SteamUGCRequestUGCDetailsResult_t.Pack8 pack8 = new SteamUGCRequestUGCDetailsResult_t.Pack8()
				{
					Details = d.Details,
					CachedData = d.CachedData
				};
				return pack8;
			}
		}
	}
}