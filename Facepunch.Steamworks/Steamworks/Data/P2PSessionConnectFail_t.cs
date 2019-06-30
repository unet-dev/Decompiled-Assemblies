using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct P2PSessionConnectFail_t
	{
		internal ulong SteamIDRemote;

		internal byte P2PSessionError;

		internal readonly static int StructSize;

		private static Action<P2PSessionConnectFail_t> actionClient;

		private static Action<P2PSessionConnectFail_t> actionServer;

		static P2PSessionConnectFail_t()
		{
			P2PSessionConnectFail_t.StructSize = Marshal.SizeOf(typeof(P2PSessionConnectFail_t));
		}

		internal static P2PSessionConnectFail_t Fill(IntPtr p)
		{
			return (P2PSessionConnectFail_t)Marshal.PtrToStructure(p, typeof(P2PSessionConnectFail_t));
		}

		public static async Task<P2PSessionConnectFail_t?> GetResultAsync(SteamAPICall_t handle)
		{
			P2PSessionConnectFail_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(P2PSessionConnectFail_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, P2PSessionConnectFail_t.StructSize, 1203, ref flag) | flag))
					{
						nullable = new P2PSessionConnectFail_t?(P2PSessionConnectFail_t.Fill(intPtr));
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

		public static void Install(Action<P2PSessionConnectFail_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(P2PSessionConnectFail_t.OnClient), P2PSessionConnectFail_t.StructSize, 1203, false);
				P2PSessionConnectFail_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(P2PSessionConnectFail_t.OnServer), P2PSessionConnectFail_t.StructSize, 1203, true);
				P2PSessionConnectFail_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<P2PSessionConnectFail_t> action = P2PSessionConnectFail_t.actionClient;
			if (action != null)
			{
				action(P2PSessionConnectFail_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<P2PSessionConnectFail_t> action = P2PSessionConnectFail_t.actionServer;
			if (action != null)
			{
				action(P2PSessionConnectFail_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}