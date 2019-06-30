using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct DlcInstalled_t
	{
		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<DlcInstalled_t> actionClient;

		private static Action<DlcInstalled_t> actionServer;

		static DlcInstalled_t()
		{
			DlcInstalled_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(DlcInstalled_t) : typeof(DlcInstalled_t.Pack8)));
		}

		internal static DlcInstalled_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (DlcInstalled_t)Marshal.PtrToStructure(p, typeof(DlcInstalled_t)) : (DlcInstalled_t.Pack8)Marshal.PtrToStructure(p, typeof(DlcInstalled_t.Pack8)));
		}

		public static async Task<DlcInstalled_t?> GetResultAsync(SteamAPICall_t handle)
		{
			DlcInstalled_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(DlcInstalled_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, DlcInstalled_t.StructSize, 1005, ref flag) | flag))
					{
						nullable = new DlcInstalled_t?(DlcInstalled_t.Fill(intPtr));
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

		public static void Install(Action<DlcInstalled_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(DlcInstalled_t.OnClient), DlcInstalled_t.StructSize, 1005, false);
				DlcInstalled_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(DlcInstalled_t.OnServer), DlcInstalled_t.StructSize, 1005, true);
				DlcInstalled_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DlcInstalled_t> action = DlcInstalled_t.actionClient;
			if (action != null)
			{
				action(DlcInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DlcInstalled_t> action = DlcInstalled_t.actionServer;
			if (action != null)
			{
				action(DlcInstalled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal AppId AppID;

			public static implicit operator DlcInstalled_t(DlcInstalled_t.Pack8 d)
			{
				return new DlcInstalled_t()
				{
					AppID = d.AppID
				};
			}

			public static implicit operator Pack8(DlcInstalled_t d)
			{
				return new DlcInstalled_t.Pack8()
				{
					AppID = d.AppID
				};
			}
		}
	}
}