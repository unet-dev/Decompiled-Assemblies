using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FavoritesListAccountsUpdated_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<FavoritesListAccountsUpdated_t> actionClient;

		private static Action<FavoritesListAccountsUpdated_t> actionServer;

		static FavoritesListAccountsUpdated_t()
		{
			FavoritesListAccountsUpdated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(FavoritesListAccountsUpdated_t) : typeof(FavoritesListAccountsUpdated_t.Pack8)));
		}

		internal static FavoritesListAccountsUpdated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (FavoritesListAccountsUpdated_t)Marshal.PtrToStructure(p, typeof(FavoritesListAccountsUpdated_t)) : (FavoritesListAccountsUpdated_t.Pack8)Marshal.PtrToStructure(p, typeof(FavoritesListAccountsUpdated_t.Pack8)));
		}

		public static async Task<FavoritesListAccountsUpdated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FavoritesListAccountsUpdated_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FavoritesListAccountsUpdated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FavoritesListAccountsUpdated_t.StructSize, 516, ref flag) | flag))
					{
						nullable = new FavoritesListAccountsUpdated_t?(FavoritesListAccountsUpdated_t.Fill(intPtr));
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

		public static void Install(Action<FavoritesListAccountsUpdated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FavoritesListAccountsUpdated_t.OnClient), FavoritesListAccountsUpdated_t.StructSize, 516, false);
				FavoritesListAccountsUpdated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FavoritesListAccountsUpdated_t.OnServer), FavoritesListAccountsUpdated_t.StructSize, 516, true);
				FavoritesListAccountsUpdated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FavoritesListAccountsUpdated_t> action = FavoritesListAccountsUpdated_t.actionClient;
			if (action != null)
			{
				action(FavoritesListAccountsUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FavoritesListAccountsUpdated_t> action = FavoritesListAccountsUpdated_t.actionServer;
			if (action != null)
			{
				action(FavoritesListAccountsUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator FavoritesListAccountsUpdated_t(FavoritesListAccountsUpdated_t.Pack8 d)
			{
				return new FavoritesListAccountsUpdated_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(FavoritesListAccountsUpdated_t d)
			{
				return new FavoritesListAccountsUpdated_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}