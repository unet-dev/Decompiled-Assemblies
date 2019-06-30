using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct P2PSessionRequest_t
	{
		internal ulong SteamIDRemote;

		internal readonly static int StructSize;

		private static Action<P2PSessionRequest_t> actionClient;

		private static Action<P2PSessionRequest_t> actionServer;

		static P2PSessionRequest_t()
		{
			P2PSessionRequest_t.StructSize = Marshal.SizeOf(typeof(P2PSessionRequest_t));
		}

		internal static P2PSessionRequest_t Fill(IntPtr p)
		{
			return (P2PSessionRequest_t)Marshal.PtrToStructure(p, typeof(P2PSessionRequest_t));
		}

		public static async Task<P2PSessionRequest_t?> GetResultAsync(SteamAPICall_t handle)
		{
			P2PSessionRequest_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(P2PSessionRequest_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, P2PSessionRequest_t.StructSize, 1202, ref flag) | flag))
					{
						nullable = new P2PSessionRequest_t?(P2PSessionRequest_t.Fill(intPtr));
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

		public static void Install(Action<P2PSessionRequest_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(P2PSessionRequest_t.OnClient), P2PSessionRequest_t.StructSize, 1202, false);
				P2PSessionRequest_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(P2PSessionRequest_t.OnServer), P2PSessionRequest_t.StructSize, 1202, true);
				P2PSessionRequest_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<P2PSessionRequest_t> action = P2PSessionRequest_t.actionClient;
			if (action != null)
			{
				action(P2PSessionRequest_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<P2PSessionRequest_t> action = P2PSessionRequest_t.actionServer;
			if (action != null)
			{
				action(P2PSessionRequest_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}