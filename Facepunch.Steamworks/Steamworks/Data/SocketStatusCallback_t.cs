using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SocketStatusCallback_t
	{
		internal uint Socket;

		internal uint ListenSocket;

		internal ulong SteamIDRemote;

		internal int SNetSocketState;

		internal readonly static int StructSize;

		private static Action<SocketStatusCallback_t> actionClient;

		private static Action<SocketStatusCallback_t> actionServer;

		static SocketStatusCallback_t()
		{
			SocketStatusCallback_t.StructSize = Marshal.SizeOf(typeof(SocketStatusCallback_t));
		}

		internal static SocketStatusCallback_t Fill(IntPtr p)
		{
			return (SocketStatusCallback_t)Marshal.PtrToStructure(p, typeof(SocketStatusCallback_t));
		}

		public static async Task<SocketStatusCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SocketStatusCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SocketStatusCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SocketStatusCallback_t.StructSize, 1201, ref flag) | flag))
					{
						nullable = new SocketStatusCallback_t?(SocketStatusCallback_t.Fill(intPtr));
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

		public static void Install(Action<SocketStatusCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SocketStatusCallback_t.OnClient), SocketStatusCallback_t.StructSize, 1201, false);
				SocketStatusCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SocketStatusCallback_t.OnServer), SocketStatusCallback_t.StructSize, 1201, true);
				SocketStatusCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SocketStatusCallback_t> action = SocketStatusCallback_t.actionClient;
			if (action != null)
			{
				action(SocketStatusCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SocketStatusCallback_t> action = SocketStatusCallback_t.actionServer;
			if (action != null)
			{
				action(SocketStatusCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}