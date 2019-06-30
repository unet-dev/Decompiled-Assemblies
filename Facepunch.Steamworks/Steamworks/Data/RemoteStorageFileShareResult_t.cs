using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoteStorageFileShareResult_t
	{
		internal Steamworks.Result Result;

		internal ulong File;

		internal string Filename;

		internal readonly static int StructSize;

		private static Action<RemoteStorageFileShareResult_t> actionClient;

		private static Action<RemoteStorageFileShareResult_t> actionServer;

		static RemoteStorageFileShareResult_t()
		{
			RemoteStorageFileShareResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoteStorageFileShareResult_t) : typeof(RemoteStorageFileShareResult_t.Pack8)));
		}

		internal static RemoteStorageFileShareResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoteStorageFileShareResult_t)Marshal.PtrToStructure(p, typeof(RemoteStorageFileShareResult_t)) : (RemoteStorageFileShareResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoteStorageFileShareResult_t.Pack8)));
		}

		public static async Task<RemoteStorageFileShareResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoteStorageFileShareResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoteStorageFileShareResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoteStorageFileShareResult_t.StructSize, 1307, ref flag) | flag))
					{
						nullable = new RemoteStorageFileShareResult_t?(RemoteStorageFileShareResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoteStorageFileShareResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoteStorageFileShareResult_t.OnClient), RemoteStorageFileShareResult_t.StructSize, 1307, false);
				RemoteStorageFileShareResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoteStorageFileShareResult_t.OnServer), RemoteStorageFileShareResult_t.StructSize, 1307, true);
				RemoteStorageFileShareResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageFileShareResult_t> action = RemoteStorageFileShareResult_t.actionClient;
			if (action != null)
			{
				action(RemoteStorageFileShareResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoteStorageFileShareResult_t> action = RemoteStorageFileShareResult_t.actionServer;
			if (action != null)
			{
				action(RemoteStorageFileShareResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong File;

			internal string Filename;

			public static implicit operator RemoteStorageFileShareResult_t(RemoteStorageFileShareResult_t.Pack8 d)
			{
				RemoteStorageFileShareResult_t remoteStorageFileShareResultT = new RemoteStorageFileShareResult_t()
				{
					Result = d.Result,
					File = d.File,
					Filename = d.Filename
				};
				return remoteStorageFileShareResultT;
			}

			public static implicit operator Pack8(RemoteStorageFileShareResult_t d)
			{
				RemoteStorageFileShareResult_t.Pack8 pack8 = new RemoteStorageFileShareResult_t.Pack8()
				{
					Result = d.Result,
					File = d.File,
					Filename = d.Filename
				};
				return pack8;
			}
		}
	}
}