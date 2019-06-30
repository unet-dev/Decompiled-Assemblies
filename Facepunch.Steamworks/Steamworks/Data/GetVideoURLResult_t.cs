using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GetVideoURLResult_t
	{
		internal Steamworks.Result Result;

		internal AppId VideoAppID;

		internal string URL;

		internal readonly static int StructSize;

		private static Action<GetVideoURLResult_t> actionClient;

		private static Action<GetVideoURLResult_t> actionServer;

		static GetVideoURLResult_t()
		{
			GetVideoURLResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GetVideoURLResult_t) : typeof(GetVideoURLResult_t.Pack8)));
		}

		internal static GetVideoURLResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GetVideoURLResult_t)Marshal.PtrToStructure(p, typeof(GetVideoURLResult_t)) : (GetVideoURLResult_t.Pack8)Marshal.PtrToStructure(p, typeof(GetVideoURLResult_t.Pack8)));
		}

		public static async Task<GetVideoURLResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GetVideoURLResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GetVideoURLResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GetVideoURLResult_t.StructSize, 4611, ref flag) | flag))
					{
						nullable = new GetVideoURLResult_t?(GetVideoURLResult_t.Fill(intPtr));
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

		public static void Install(Action<GetVideoURLResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GetVideoURLResult_t.OnClient), GetVideoURLResult_t.StructSize, 4611, false);
				GetVideoURLResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GetVideoURLResult_t.OnServer), GetVideoURLResult_t.StructSize, 4611, true);
				GetVideoURLResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetVideoURLResult_t> action = GetVideoURLResult_t.actionClient;
			if (action != null)
			{
				action(GetVideoURLResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetVideoURLResult_t> action = GetVideoURLResult_t.actionServer;
			if (action != null)
			{
				action(GetVideoURLResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal AppId VideoAppID;

			internal string URL;

			public static implicit operator GetVideoURLResult_t(GetVideoURLResult_t.Pack8 d)
			{
				GetVideoURLResult_t getVideoURLResultT = new GetVideoURLResult_t()
				{
					Result = d.Result,
					VideoAppID = d.VideoAppID,
					URL = d.URL
				};
				return getVideoURLResultT;
			}

			public static implicit operator Pack8(GetVideoURLResult_t d)
			{
				GetVideoURLResult_t.Pack8 pack8 = new GetVideoURLResult_t.Pack8()
				{
					Result = d.Result,
					VideoAppID = d.VideoAppID,
					URL = d.URL
				};
				return pack8;
			}
		}
	}
}