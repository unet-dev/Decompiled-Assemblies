using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageFileReadAsyncComplete_t
	{
		internal ulong FileReadAsync;

		internal Steamworks.Result Result;

		internal uint Offset;

		internal uint Read;

		internal readonly static int StructSize;

		private static Action<RemoteStorageFileReadAsyncComplete_t> actionClient;

		private static Action<RemoteStorageFileReadAsyncComplete_t> actionServer;

		static RemoteStorageFileReadAsyncComplete_t()
		{
			RemoteStorageFileReadAsyncComplete_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageFileReadAsyncComplete_t) : typeof(RemoteStorageFileReadAsyncComplete_t.Pack8)));
		}

		internal static RemoteStorageFileReadAsyncComplete_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageFileReadAsyncComplete_t)Marshal.PtrToStructure(p, typeof(RemoteStorageFileReadAsyncComplete_t)) : (RemoteStorageFileReadAsyncComplete_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageFileReadAsyncComplete_t.Pack8)));
		}

		public static async Task<RemoteStorageFileReadAsyncComplete_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageFileReadAsyncComplete_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageFileReadAsyncComplete_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageFileReadAsyncComplete_t.StructSize, 1332, ref flag) | flag))
					{
						nullable = new RemoteStorageFileReadAsyncComplete_t?(RemoteStorageFileReadAsyncComplete_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageFileReadAsyncComplete_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageFileReadAsyncComplete_t.OnClient), RemoteStorageFileReadAsyncComplete_t.StructSize, 1332, false);
				RemoteStorageFileReadAsyncComplete_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageFileReadAsyncComplete_t.OnServer), RemoteStorageFileReadAsyncComplete_t.StructSize, 1332, true);
				RemoteStorageFileReadAsyncComplete_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageFileReadAsyncComplete_t> action = RemoteStorageFileReadAsyncComplete_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageFileReadAsyncComplete_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageFileReadAsyncComplete_t> action = RemoteStorageFileReadAsyncComplete_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageFileReadAsyncComplete_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong FileReadAsync;

			internal Steamworks.Result Result;

			internal uint Offset;

			internal uint Read;

			public static implicit operator RemoteStorageFileReadAsyncComplete_t(RemoteStorageFileReadAsyncComplete_t.Pack8 d)
			{
				RemoteStorageFileReadAsyncComplete_t remoteStorageFileReadAsyncCompleteT = new RemoteStorageFileReadAsyncComplete_t()
				{
					FileReadAsync = d.FileReadAsync,
					Result = d.Result,
					Offset = d.Offset,
					Read = d.Read
				};
				return remoteStorageFileReadAsyncCompleteT;
			}

			public static implicit operator Pack8(RemoteStorageFileReadAsyncComplete_t d)
			{
				RemoteStorageFileReadAsyncComplete_t.Pack8 pack8 = new RemoteStorageFileReadAsyncComplete_t.Pack8()
				{
					FileReadAsync = d.FileReadAsync,
					Result = d.Result,
					Offset = d.Offset,
					Read = d.Read
				};
				return pack8;
			}
		}
	}
}