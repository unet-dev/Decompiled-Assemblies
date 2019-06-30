using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamAppUninstalled_t
	{
		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<SteamAppUninstalled_t> actionClient;

		private static Action<SteamAppUninstalled_t> actionServer;

		static SteamAppUninstalled_t()
		{
			SteamAppUninstalled_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamAppUninstalled_t) : typeof(SteamAppUninstalled_t.Pack8)));
		}

		internal static SteamAppUninstalled_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamAppUninstalled_t)Marshal.PtrToStructure(p, typeof(SteamAppUninstalled_t)) : (SteamAppUninstalled_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamAppUninstalled_t.Pack8)));
		}

		public static async Task<SteamAppUninstalled_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamAppUninstalled_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamAppUninstalled_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamAppUninstalled_t.StructSize, 3902, ref flag) | flag))
					{
						nullable = new SteamAppUninstalled_t?(SteamAppUninstalled_t.Fill(intPtr));
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

		public static void Install(Action<SteamAppUninstalled_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamAppUninstalled_t.OnClient), SteamAppUninstalled_t.StructSize, 3902, false);
				SteamAppUninstalled_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamAppUninstalled_t.OnServer), SteamAppUninstalled_t.StructSize, 3902, true);
				SteamAppUninstalled_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamAppUninstalled_t> action = SteamAppUninstalled_t.actionClient;
			if (action != null)
			{
				action(SteamAppUninstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamAppUninstalled_t> action = SteamAppUninstalled_t.actionServer;
			if (action != null)
			{
				action(SteamAppUninstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			public static implicit operator SteamAppUninstalled_t(SteamAppUninstalled_t.Pack8 d)
			{
				return new SteamAppUninstalled_t()
				{
					AppID = d.AppID
				};
			}

			public static implicit operator Pack8(SteamAppUninstalled_t d)
			{
				return new SteamAppUninstalled_t.Pack8()
				{
					AppID = d.AppID
				};
			}
		}
	}
}