using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct UserFavoriteItemsListChanged_t
	{
		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal Steamworks.Result Result;

		internal bool WasAddRequest;

		internal readonly static int StructSize;

		private static Action<UserFavoriteItemsListChanged_t> actionClient;

		private static Action<UserFavoriteItemsListChanged_t> actionServer;

		static UserFavoriteItemsListChanged_t()
		{
			UserFavoriteItemsListChanged_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(UserFavoriteItemsListChanged_t) : typeof(UserFavoriteItemsListChanged_t.Pack8)));
		}

		internal static UserFavoriteItemsListChanged_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (UserFavoriteItemsListChanged_t)Marshal.PtrToStructure(p, typeof(UserFavoriteItemsListChanged_t)) : (UserFavoriteItemsListChanged_t.Pack8)Marshal.PtrToStructure(p, typeof(UserFavoriteItemsListChanged_t.Pack8)));
		}

		public static async Task<UserFavoriteItemsListChanged_t?> GetResultAsync(SteamAPICall_t handle)
		{
			UserFavoriteItemsListChanged_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(UserFavoriteItemsListChanged_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, UserFavoriteItemsListChanged_t.StructSize, 3407, ref flag) | flag))
					{
						nullable = new UserFavoriteItemsListChanged_t?(UserFavoriteItemsListChanged_t.Fill(intPtr));
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

		public static void Install(Action<UserFavoriteItemsListChanged_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(UserFavoriteItemsListChanged_t.OnClient), UserFavoriteItemsListChanged_t.StructSize, 3407, false);
				UserFavoriteItemsListChanged_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(UserFavoriteItemsListChanged_t.OnServer), UserFavoriteItemsListChanged_t.StructSize, 3407, true);
				UserFavoriteItemsListChanged_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserFavoriteItemsListChanged_t> action = UserFavoriteItemsListChanged_t.actionClient;
			if (action != null)
			{
				action(UserFavoriteItemsListChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<UserFavoriteItemsListChanged_t> action = UserFavoriteItemsListChanged_t.actionServer;
			if (action != null)
			{
				action(UserFavoriteItemsListChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal Steamworks.Result Result;

			internal bool WasAddRequest;

			public static implicit operator UserFavoriteItemsListChanged_t(UserFavoriteItemsListChanged_t.Pack8 d)
			{
				UserFavoriteItemsListChanged_t userFavoriteItemsListChangedT = new UserFavoriteItemsListChanged_t()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					WasAddRequest = d.WasAddRequest
				};
				return userFavoriteItemsListChangedT;
			}

			public static implicit operator Pack8(UserFavoriteItemsListChanged_t d)
			{
				UserFavoriteItemsListChanged_t.Pack8 pack8 = new UserFavoriteItemsListChanged_t.Pack8()
				{
					PublishedFileId = d.PublishedFileId,
					Result = d.Result,
					WasAddRequest = d.WasAddRequest
				};
				return pack8;
			}
		}
	}
}