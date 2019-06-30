using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct PS3TrophiesInstalled_t
	{
		internal ulong GameID;

		internal Steamworks.Result Result;

		internal ulong RequiredDiskSpace;

		internal readonly static int StructSize;

		private static Action<PS3TrophiesInstalled_t> actionClient;

		private static Action<PS3TrophiesInstalled_t> actionServer;

		static PS3TrophiesInstalled_t()
		{
			PS3TrophiesInstalled_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(PS3TrophiesInstalled_t) : typeof(PS3TrophiesInstalled_t.Pack8)));
		}

		internal static PS3TrophiesInstalled_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (PS3TrophiesInstalled_t)Marshal.PtrToStructure(p, typeof(PS3TrophiesInstalled_t)) : (PS3TrophiesInstalled_t.Pack8)Marshal.PtrToStructure(p, typeof(PS3TrophiesInstalled_t.Pack8)));
		}

		public static async Task<PS3TrophiesInstalled_t?> GetResultAsync(SteamAPICall_t handle)
		{
			PS3TrophiesInstalled_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(PS3TrophiesInstalled_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, PS3TrophiesInstalled_t.StructSize, 1112, ref flag) | flag))
					{
						nullable = new PS3TrophiesInstalled_t?(PS3TrophiesInstalled_t.Fill(intPtr));
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

		public static void Install(Action<PS3TrophiesInstalled_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(PS3TrophiesInstalled_t.OnClient), PS3TrophiesInstalled_t.StructSize, 1112, false);
				PS3TrophiesInstalled_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(PS3TrophiesInstalled_t.OnServer), PS3TrophiesInstalled_t.StructSize, 1112, true);
				PS3TrophiesInstalled_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<PS3TrophiesInstalled_t> action = PS3TrophiesInstalled_t.actionClient;
			if (action != null)
			{
				action(PS3TrophiesInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<PS3TrophiesInstalled_t> action = PS3TrophiesInstalled_t.actionServer;
			if (action != null)
			{
				action(PS3TrophiesInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong GameID;

			internal Steamworks.Result Result;

			internal ulong RequiredDiskSpace;

			public static implicit operator PS3TrophiesInstalled_t(PS3TrophiesInstalled_t.Pack8 d)
			{
				PS3TrophiesInstalled_t pS3TrophiesInstalledT = new PS3TrophiesInstalled_t()
				{
					GameID = d.GameID,
					Result = d.Result,
					RequiredDiskSpace = d.RequiredDiskSpace
				};
				return pS3TrophiesInstalledT;
			}

			public static implicit operator Pack8(PS3TrophiesInstalled_t d)
			{
				PS3TrophiesInstalled_t.Pack8 pack8 = new PS3TrophiesInstalled_t.Pack8()
				{
					GameID = d.GameID,
					Result = d.Result,
					RequiredDiskSpace = d.RequiredDiskSpace
				};
				return pack8;
			}
		}
	}
}