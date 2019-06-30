using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ReservationNotificationCallback_t
	{
		internal ulong BeaconID;

		internal ulong SteamIDJoiner;

		internal readonly static int StructSize;

		private static Action<ReservationNotificationCallback_t> actionClient;

		private static Action<ReservationNotificationCallback_t> actionServer;

		static ReservationNotificationCallback_t()
		{
			ReservationNotificationCallback_t.StructSize = Marshal.SizeOf(typeof(ReservationNotificationCallback_t));
		}

		internal static ReservationNotificationCallback_t Fill(IntPtr p)
		{
			return (ReservationNotificationCallback_t)Marshal.PtrToStructure(p, typeof(ReservationNotificationCallback_t));
		}

		public static async Task<ReservationNotificationCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ReservationNotificationCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ReservationNotificationCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ReservationNotificationCallback_t.StructSize, 5303, ref flag) | flag))
					{
						nullable = new ReservationNotificationCallback_t?(ReservationNotificationCallback_t.Fill(intPtr));
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

		public static void Install(Action<ReservationNotificationCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ReservationNotificationCallback_t.OnClient), ReservationNotificationCallback_t.StructSize, 5303, false);
				ReservationNotificationCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ReservationNotificationCallback_t.OnServer), ReservationNotificationCallback_t.StructSize, 5303, true);
				ReservationNotificationCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ReservationNotificationCallback_t> action = ReservationNotificationCallback_t.actionClient;
			if (action != null)
			{
				action(ReservationNotificationCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ReservationNotificationCallback_t> action = ReservationNotificationCallback_t.actionServer;
			if (action != null)
			{
				action(ReservationNotificationCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}