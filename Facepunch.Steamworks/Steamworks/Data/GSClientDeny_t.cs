using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSClientDeny_t
	{
		internal ulong SteamID;

		internal Steamworks.DenyReason DenyReason;

		internal string OptionalText;

		internal readonly static int StructSize;

		private static Action<GSClientDeny_t> actionClient;

		private static Action<GSClientDeny_t> actionServer;

		static GSClientDeny_t()
		{
			GSClientDeny_t.StructSize = Marshal.SizeOf(typeof(GSClientDeny_t));
		}

		internal static GSClientDeny_t Fill(IntPtr p)
		{
			return (GSClientDeny_t)Marshal.PtrToStructure(p, typeof(GSClientDeny_t));
		}

		public static async Task<GSClientDeny_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSClientDeny_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSClientDeny_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSClientDeny_t.StructSize, 202, ref flag) | flag))
					{
						nullable = new GSClientDeny_t?(GSClientDeny_t.Fill(intPtr));
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

		public static void Install(Action<GSClientDeny_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSClientDeny_t.OnClient), GSClientDeny_t.StructSize, 202, false);
				GSClientDeny_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSClientDeny_t.OnServer), GSClientDeny_t.StructSize, 202, true);
				GSClientDeny_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientDeny_t> action = GSClientDeny_t.actionClient;
			if (action != null)
			{
				action(GSClientDeny_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientDeny_t> action = GSClientDeny_t.actionServer;
			if (action != null)
			{
				action(GSClientDeny_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}