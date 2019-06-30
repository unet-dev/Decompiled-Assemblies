using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GCMessageFailed_t
	{
		internal readonly static int StructSize;

		private static Action<GCMessageFailed_t> actionClient;

		private static Action<GCMessageFailed_t> actionServer;

		static GCMessageFailed_t()
		{
			GCMessageFailed_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GCMessageFailed_t) : typeof(GCMessageFailed_t.Pack8)));
		}

		internal static GCMessageFailed_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GCMessageFailed_t)Marshal.PtrToStructure(p, typeof(GCMessageFailed_t)) : (GCMessageFailed_t.Pack8)Marshal.PtrToStructure(p, typeof(GCMessageFailed_t.Pack8)));
		}

		public static async Task<GCMessageFailed_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GCMessageFailed_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GCMessageFailed_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GCMessageFailed_t.StructSize, 1702, ref flag) | flag))
					{
						nullable = new GCMessageFailed_t?(GCMessageFailed_t.Fill(intPtr));
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

		public static void Install(Action<GCMessageFailed_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GCMessageFailed_t.OnClient), GCMessageFailed_t.StructSize, 1702, false);
				GCMessageFailed_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GCMessageFailed_t.OnServer), GCMessageFailed_t.StructSize, 1702, true);
				GCMessageFailed_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GCMessageFailed_t> action = GCMessageFailed_t.actionClient;
			if (action != null)
			{
				action(GCMessageFailed_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GCMessageFailed_t> action = GCMessageFailed_t.actionServer;
			if (action != null)
			{
				action(GCMessageFailed_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator GCMessageFailed_t(GCMessageFailed_t.Pack8 d)
			{
				return new GCMessageFailed_t();
			}

			public static implicit operator Pack8(GCMessageFailed_t d)
			{
				return new GCMessageFailed_t.Pack8();
			}
		}
	}
}