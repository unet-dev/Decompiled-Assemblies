using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamServerConnectFailure_t
	{
		internal Steamworks.Result Result;

		internal bool StillRetrying;

		internal readonly static int StructSize;

		private static Action<SteamServerConnectFailure_t> actionClient;

		private static Action<SteamServerConnectFailure_t> actionServer;

		static SteamServerConnectFailure_t()
		{
			SteamServerConnectFailure_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamServerConnectFailure_t) : typeof(SteamServerConnectFailure_t.Pack8)));
		}

		internal static SteamServerConnectFailure_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamServerConnectFailure_t)Marshal.PtrToStructure(p, typeof(SteamServerConnectFailure_t)) : (SteamServerConnectFailure_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamServerConnectFailure_t.Pack8)));
		}

		public static async Task<SteamServerConnectFailure_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamServerConnectFailure_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamServerConnectFailure_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamServerConnectFailure_t.StructSize, 102, ref flag) | flag))
					{
						nullable = new SteamServerConnectFailure_t?(SteamServerConnectFailure_t.Fill(intPtr));
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

		public static void Install(Action<SteamServerConnectFailure_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamServerConnectFailure_t.OnClient), SteamServerConnectFailure_t.StructSize, 102, false);
				SteamServerConnectFailure_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamServerConnectFailure_t.OnServer), SteamServerConnectFailure_t.StructSize, 102, true);
				SteamServerConnectFailure_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamServerConnectFailure_t> action = SteamServerConnectFailure_t.actionClient;
			if (action != null)
			{
				action(SteamServerConnectFailure_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamServerConnectFailure_t> action = SteamServerConnectFailure_t.actionServer;
			if (action != null)
			{
				action(SteamServerConnectFailure_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal bool StillRetrying;

			public static implicit operator SteamServerConnectFailure_t(SteamServerConnectFailure_t.Pack8 d)
			{
				SteamServerConnectFailure_t steamServerConnectFailureT = new SteamServerConnectFailure_t()
				{
					Result = d.Result,
					StillRetrying = d.StillRetrying
				};
				return steamServerConnectFailureT;
			}

			public static implicit operator Pack8(SteamServerConnectFailure_t d)
			{
				SteamServerConnectFailure_t.Pack8 pack8 = new SteamServerConnectFailure_t.Pack8()
				{
					Result = d.Result,
					StillRetrying = d.StillRetrying
				};
				return pack8;
			}
		}
	}
}