using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamAppInstalled_t
	{
		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<SteamAppInstalled_t> actionClient;

		private static Action<SteamAppInstalled_t> actionServer;

		static SteamAppInstalled_t()
		{
			SteamAppInstalled_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamAppInstalled_t) : typeof(SteamAppInstalled_t.Pack8)));
		}

		internal static SteamAppInstalled_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamAppInstalled_t)Marshal.PtrToStructure(p, typeof(SteamAppInstalled_t)) : (SteamAppInstalled_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamAppInstalled_t.Pack8)));
		}

		public static async Task<SteamAppInstalled_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamAppInstalled_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamAppInstalled_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamAppInstalled_t.StructSize, 3901, ref flag) | flag))
					{
						nullable = new SteamAppInstalled_t?(SteamAppInstalled_t.Fill(intPtr));
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

		public static void Install(Action<SteamAppInstalled_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamAppInstalled_t.OnClient), SteamAppInstalled_t.StructSize, 3901, false);
				SteamAppInstalled_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamAppInstalled_t.OnServer), SteamAppInstalled_t.StructSize, 3901, true);
				SteamAppInstalled_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamAppInstalled_t> action = SteamAppInstalled_t.actionClient;
			if (action != null)
			{
				action(SteamAppInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamAppInstalled_t> action = SteamAppInstalled_t.actionServer;
			if (action != null)
			{
				action(SteamAppInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			public static implicit operator SteamAppInstalled_t(SteamAppInstalled_t.Pack8 d)
			{
				return new SteamAppInstalled_t()
				{
					AppID = d.AppID
				};
			}

			public static implicit operator Pack8(SteamAppInstalled_t d)
			{
				return new SteamAppInstalled_t.Pack8()
				{
					AppID = d.AppID
				};
			}
		}
	}
}