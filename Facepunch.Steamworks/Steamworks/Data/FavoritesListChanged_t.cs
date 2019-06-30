using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FavoritesListChanged_t
	{
		internal uint IP;

		internal uint QueryPort;

		internal uint ConnPort;

		internal uint AppID;

		internal uint Flags;

		internal bool Add;

		internal uint AccountId;

		internal readonly static int StructSize;

		private static Action<FavoritesListChanged_t> actionClient;

		private static Action<FavoritesListChanged_t> actionServer;

		static FavoritesListChanged_t()
		{
			FavoritesListChanged_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(FavoritesListChanged_t) : typeof(FavoritesListChanged_t.Pack8)));
		}

		internal static FavoritesListChanged_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (FavoritesListChanged_t)Marshal.PtrToStructure(p, typeof(FavoritesListChanged_t)) : (FavoritesListChanged_t.Pack8)Marshal.PtrToStructure(p, typeof(FavoritesListChanged_t.Pack8)));
		}

		public static async Task<FavoritesListChanged_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FavoritesListChanged_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FavoritesListChanged_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FavoritesListChanged_t.StructSize, 502, ref flag) | flag))
					{
						nullable = new FavoritesListChanged_t?(FavoritesListChanged_t.Fill(intPtr));
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

		public static void Install(Action<FavoritesListChanged_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FavoritesListChanged_t.OnClient), FavoritesListChanged_t.StructSize, 502, false);
				FavoritesListChanged_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FavoritesListChanged_t.OnServer), FavoritesListChanged_t.StructSize, 502, true);
				FavoritesListChanged_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FavoritesListChanged_t> action = FavoritesListChanged_t.actionClient;
			if (action != null)
			{
				action(FavoritesListChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FavoritesListChanged_t> action = FavoritesListChanged_t.actionServer;
			if (action != null)
			{
				action(FavoritesListChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint IP;

			internal uint QueryPort;

			internal uint ConnPort;

			internal uint AppID;

			internal uint Flags;

			internal bool Add;

			internal uint AccountId;

			public static implicit operator FavoritesListChanged_t(FavoritesListChanged_t.Pack8 d)
			{
				FavoritesListChanged_t favoritesListChangedT = new FavoritesListChanged_t()
				{
					IP = d.IP,
					QueryPort = d.QueryPort,
					ConnPort = d.ConnPort,
					AppID = d.AppID,
					Flags = d.Flags,
					Add = d.Add,
					AccountId = d.AccountId
				};
				return favoritesListChangedT;
			}

			public static implicit operator Pack8(FavoritesListChanged_t d)
			{
				FavoritesListChanged_t.Pack8 pack8 = new FavoritesListChanged_t.Pack8()
				{
					IP = d.IP,
					QueryPort = d.QueryPort,
					ConnPort = d.ConnPort,
					AppID = d.AppID,
					Flags = d.Flags,
					Add = d.Add,
					AccountId = d.AccountId
				};
				return pack8;
			}
		}
	}
}