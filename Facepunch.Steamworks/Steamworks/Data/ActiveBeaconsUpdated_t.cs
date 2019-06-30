using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ActiveBeaconsUpdated_t
	{
		internal readonly static int StructSize;

		private static Action<ActiveBeaconsUpdated_t> actionClient;

		private static Action<ActiveBeaconsUpdated_t> actionServer;

		static ActiveBeaconsUpdated_t()
		{
			ActiveBeaconsUpdated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(ActiveBeaconsUpdated_t) : typeof(ActiveBeaconsUpdated_t.Pack8)));
		}

		internal static ActiveBeaconsUpdated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (ActiveBeaconsUpdated_t)Marshal.PtrToStructure(p, typeof(ActiveBeaconsUpdated_t)) : (ActiveBeaconsUpdated_t.Pack8)Marshal.PtrToStructure(p, typeof(ActiveBeaconsUpdated_t.Pack8)));
		}

		public static async Task<ActiveBeaconsUpdated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ActiveBeaconsUpdated_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ActiveBeaconsUpdated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ActiveBeaconsUpdated_t.StructSize, 5306, ref flag) | flag))
					{
						nullable = new ActiveBeaconsUpdated_t?(ActiveBeaconsUpdated_t.Fill(intPtr));
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

		public static void Install(Action<ActiveBeaconsUpdated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ActiveBeaconsUpdated_t.OnClient), ActiveBeaconsUpdated_t.StructSize, 5306, false);
				ActiveBeaconsUpdated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ActiveBeaconsUpdated_t.OnServer), ActiveBeaconsUpdated_t.StructSize, 5306, true);
				ActiveBeaconsUpdated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ActiveBeaconsUpdated_t> action = ActiveBeaconsUpdated_t.actionClient;
			if (action != null)
			{
				action(ActiveBeaconsUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ActiveBeaconsUpdated_t> action = ActiveBeaconsUpdated_t.actionServer;
			if (action != null)
			{
				action(ActiveBeaconsUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator ActiveBeaconsUpdated_t(ActiveBeaconsUpdated_t.Pack8 d)
			{
				return new ActiveBeaconsUpdated_t();
			}

			public static implicit operator Pack8(ActiveBeaconsUpdated_t d)
			{
				return new ActiveBeaconsUpdated_t.Pack8();
			}
		}
	}
}