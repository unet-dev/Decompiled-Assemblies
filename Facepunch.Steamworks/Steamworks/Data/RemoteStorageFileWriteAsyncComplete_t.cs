using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageFileWriteAsyncComplete_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<RemoteStorageFileWriteAsyncComplete_t> actionClient;

		private static Action<RemoteStorageFileWriteAsyncComplete_t> actionServer;

		static RemoteStorageFileWriteAsyncComplete_t()
		{
			RemoteStorageFileWriteAsyncComplete_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageFileWriteAsyncComplete_t) : typeof(RemoteStorageFileWriteAsyncComplete_t.Pack8)));
		}

		internal static RemoteStorageFileWriteAsyncComplete_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageFileWriteAsyncComplete_t)Marshal.PtrToStructure(p, typeof(RemoteStorageFileWriteAsyncComplete_t)) : (RemoteStorageFileWriteAsyncComplete_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageFileWriteAsyncComplete_t.Pack8)));
		}

		public static async Task<RemoteStorageFileWriteAsyncComplete_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageFileWriteAsyncComplete_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageFileWriteAsyncComplete_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageFileWriteAsyncComplete_t.StructSize, 1331, ref flag) | flag))
					{
						nullable = new RemoteStorageFileWriteAsyncComplete_t?(RemoteStorageFileWriteAsyncComplete_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageFileWriteAsyncComplete_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageFileWriteAsyncComplete_t.OnClient), RemoteStorageFileWriteAsyncComplete_t.StructSize, 1331, false);
				RemoteStorageFileWriteAsyncComplete_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageFileWriteAsyncComplete_t.OnServer), RemoteStorageFileWriteAsyncComplete_t.StructSize, 1331, true);
				RemoteStorageFileWriteAsyncComplete_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageFileWriteAsyncComplete_t> action = RemoteStorageFileWriteAsyncComplete_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageFileWriteAsyncComplete_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageFileWriteAsyncComplete_t> action = RemoteStorageFileWriteAsyncComplete_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageFileWriteAsyncComplete_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator RemoteStorageFileWriteAsyncComplete_t(RemoteStorageFileWriteAsyncComplete_t.Pack8 d)
			{
				return new RemoteStorageFileWriteAsyncComplete_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(RemoteStorageFileWriteAsyncComplete_t d)
			{
				return new RemoteStorageFileWriteAsyncComplete_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}