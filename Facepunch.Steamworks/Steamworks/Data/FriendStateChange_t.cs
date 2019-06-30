using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FriendStateChange_t
	{
		internal ulong SteamID;

		internal int ChangeFlags;

		internal readonly static int StructSize;

		private static Action<FriendStateChange_t> actionClient;

		private static Action<FriendStateChange_t> actionServer;

		static FriendStateChange_t()
		{
			FriendStateChange_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(FriendStateChange_t) : typeof(FriendStateChange_t.Pack8)));
		}

		internal static FriendStateChange_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (FriendStateChange_t)Marshal.PtrToStructure(p, typeof(FriendStateChange_t)) : (FriendStateChange_t.Pack8)Marshal.PtrToStructure(p, typeof(FriendStateChange_t.Pack8)));
		}

		public static async Task<FriendStateChange_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FriendStateChange_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FriendStateChange_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FriendStateChange_t.StructSize, 304, ref flag) | flag))
					{
						nullable = new FriendStateChange_t?(FriendStateChange_t.Fill(intPtr));
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

		public static void Install(Action<FriendStateChange_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FriendStateChange_t.OnClient), FriendStateChange_t.StructSize, 304, false);
				FriendStateChange_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FriendStateChange_t.OnServer), FriendStateChange_t.StructSize, 304, true);
				FriendStateChange_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendStateChange_t> action = FriendStateChange_t.actionClient;
			if (action != null)
			{
				action(FriendStateChange_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FriendStateChange_t> action = FriendStateChange_t.actionServer;
			if (action != null)
			{
				action(FriendStateChange_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong SteamID;

			internal int ChangeFlags;

			public static implicit operator FriendStateChange_t(FriendStateChange_t.Pack8 d)
			{
				FriendStateChange_t friendStateChangeT = new FriendStateChange_t()
				{
					SteamID = d.SteamID,
					ChangeFlags = d.ChangeFlags
				};
				return friendStateChangeT;
			}

			public static implicit operator Pack8(FriendStateChange_t d)
			{
				FriendStateChange_t.Pack8 pack8 = new FriendStateChange_t.Pack8()
				{
					SteamID = d.SteamID,
					ChangeFlags = d.ChangeFlags
				};
				return pack8;
			}
		}
	}
}