using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStoragePublishFileProgress_t
	{
		internal double DPercentFile;

		internal bool Preview;

		internal readonly static int StructSize;

		private static Action<RemoteStoragePublishFileProgress_t> actionClient;

		private static Action<RemoteStoragePublishFileProgress_t> actionServer;

		static RemoteStoragePublishFileProgress_t()
		{
			RemoteStoragePublishFileProgress_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStoragePublishFileProgress_t) : typeof(RemoteStoragePublishFileProgress_t.Pack8)));
		}

		internal static RemoteStoragePublishFileProgress_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStoragePublishFileProgress_t)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishFileProgress_t)) : (RemoteStoragePublishFileProgress_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStoragePublishFileProgress_t.Pack8)));
		}

		public static async Task<RemoteStoragePublishFileProgress_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStoragePublishFileProgress_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStoragePublishFileProgress_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStoragePublishFileProgress_t.StructSize, 1329, ref flag) | flag))
					{
						nullable = new RemoteStoragePublishFileProgress_t?(RemoteStoragePublishFileProgress_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStoragePublishFileProgress_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStoragePublishFileProgress_t.OnClient), RemoteStoragePublishFileProgress_t.StructSize, 1329, false);
				RemoteStoragePublishFileProgress_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStoragePublishFileProgress_t.OnServer), RemoteStoragePublishFileProgress_t.StructSize, 1329, true);
				RemoteStoragePublishFileProgress_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishFileProgress_t> action = RemoteStoragePublishFileProgress_t.actionClient;
			if (action != null)
			{
				action(RemoteStoragePublishFileProgress_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStoragePublishFileProgress_t> action = RemoteStoragePublishFileProgress_t.actionServer;
			if (action != null)
			{
				action(RemoteStoragePublishFileProgress_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal double DPercentFile;

			internal bool Preview;

			public static implicit operator RemoteStoragePublishFileProgress_t(RemoteStoragePublishFileProgress_t.Pack8 d)
			{
				RemoteStoragePublishFileProgress_t remoteStoragePublishFileProgressT = new RemoteStoragePublishFileProgress_t()
				{
					DPercentFile = d.DPercentFile,
					Preview = d.Preview
				};
				return remoteStoragePublishFileProgressT;
			}

			public static implicit operator Pack8(RemoteStoragePublishFileProgress_t d)
			{
				RemoteStoragePublishFileProgress_t.Pack8 pack8 = new RemoteStoragePublishFileProgress_t.Pack8()
				{
					DPercentFile = d.DPercentFile,
					Preview = d.Preview
				};
				return pack8;
			}
		}
	}
}