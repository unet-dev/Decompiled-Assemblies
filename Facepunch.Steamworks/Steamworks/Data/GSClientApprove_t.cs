using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSClientApprove_t
	{
		internal ulong SteamID;

		internal ulong OwnerSteamID;

		internal readonly static int StructSize;

		private static Action<GSClientApprove_t> actionClient;

		private static Action<GSClientApprove_t> actionServer;

		static GSClientApprove_t()
		{
			GSClientApprove_t.StructSize = Marshal.SizeOf(typeof(GSClientApprove_t));
		}

		internal static GSClientApprove_t Fill(IntPtr p)
		{
			return (GSClientApprove_t)Marshal.PtrToStructure(p, typeof(GSClientApprove_t));
		}

		public static async Task<GSClientApprove_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSClientApprove_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSClientApprove_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSClientApprove_t.StructSize, 201, ref flag) | flag))
					{
						nullable = new GSClientApprove_t?(GSClientApprove_t.Fill(intPtr));
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

		public static void Install(Action<GSClientApprove_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSClientApprove_t.OnClient), GSClientApprove_t.StructSize, 201, false);
				GSClientApprove_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSClientApprove_t.OnServer), GSClientApprove_t.StructSize, 201, true);
				GSClientApprove_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientApprove_t> action = GSClientApprove_t.actionClient;
			if (action != null)
			{
				action(GSClientApprove_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientApprove_t> action = GSClientApprove_t.actionServer;
			if (action != null)
			{
				action(GSClientApprove_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}