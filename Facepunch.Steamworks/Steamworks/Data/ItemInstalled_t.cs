using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ItemInstalled_t
	{
		internal AppId AppID;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal readonly static int StructSize;

		private static Action<ItemInstalled_t> actionClient;

		private static Action<ItemInstalled_t> actionServer;

		static ItemInstalled_t()
		{
			ItemInstalled_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(ItemInstalled_t) : typeof(ItemInstalled_t.Pack8)));
		}

		internal static ItemInstalled_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (ItemInstalled_t)Marshal.PtrToStructure(p, typeof(ItemInstalled_t)) : (ItemInstalled_t.Pack8)Marshal.PtrToStructure(p, typeof(ItemInstalled_t.Pack8)));
		}

		public static async Task<ItemInstalled_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ItemInstalled_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ItemInstalled_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ItemInstalled_t.StructSize, 3405, ref flag) | flag))
					{
						nullable = new ItemInstalled_t?(ItemInstalled_t.Fill(intPtr));
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

		public static void Install(Action<ItemInstalled_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ItemInstalled_t.OnClient), ItemInstalled_t.StructSize, 3405, false);
				ItemInstalled_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ItemInstalled_t.OnServer), ItemInstalled_t.StructSize, 3405, true);
				ItemInstalled_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ItemInstalled_t> action = ItemInstalled_t.actionClient;
			if (action != null)
			{
				action(ItemInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ItemInstalled_t> action = ItemInstalled_t.actionServer;
			if (action != null)
			{
				action(ItemInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			public static implicit operator ItemInstalled_t(ItemInstalled_t.Pack8 d)
			{
				ItemInstalled_t itemInstalledT = new ItemInstalled_t()
				{
					AppID = d.AppID,
					PublishedFileId = d.PublishedFileId
				};
				return itemInstalledT;
			}

			public static implicit operator Pack8(ItemInstalled_t d)
			{
				ItemInstalled_t.Pack8 pack8 = new ItemInstalled_t.Pack8()
				{
					AppID = d.AppID,
					PublishedFileId = d.PublishedFileId
				};
				return pack8;
			}
		}
	}
}