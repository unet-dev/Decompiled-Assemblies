using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSClientKick_t
	{
		internal ulong SteamID;

		internal Steamworks.DenyReason DenyReason;

		internal readonly static int StructSize;

		private static Action<GSClientKick_t> actionClient;

		private static Action<GSClientKick_t> actionServer;

		static GSClientKick_t()
		{
			GSClientKick_t.StructSize = Marshal.SizeOf(typeof(GSClientKick_t));
		}

		internal static GSClientKick_t Fill(IntPtr p)
		{
			return (GSClientKick_t)Marshal.PtrToStructure(p, typeof(GSClientKick_t));
		}

		public static async Task<GSClientKick_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSClientKick_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSClientKick_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSClientKick_t.StructSize, 203, ref flag) | flag))
					{
						nullable = new GSClientKick_t?(GSClientKick_t.Fill(intPtr));
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

		public static void Install(Action<GSClientKick_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSClientKick_t.OnClient), GSClientKick_t.StructSize, 203, false);
				GSClientKick_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSClientKick_t.OnServer), GSClientKick_t.StructSize, 203, true);
				GSClientKick_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientKick_t> action = GSClientKick_t.actionClient;
			if (action != null)
			{
				action(GSClientKick_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientKick_t> action = GSClientKick_t.actionServer;
			if (action != null)
			{
				action(GSClientKick_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}