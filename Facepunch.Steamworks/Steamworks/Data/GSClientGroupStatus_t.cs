using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GSClientGroupStatus_t
	{
		internal ulong SteamIDUser;

		internal ulong SteamIDGroup;

		internal bool Member;

		internal bool Officer;

		internal readonly static int StructSize;

		private static Action<GSClientGroupStatus_t> actionClient;

		private static Action<GSClientGroupStatus_t> actionServer;

		static GSClientGroupStatus_t()
		{
			GSClientGroupStatus_t.StructSize = Marshal.SizeOf(typeof(GSClientGroupStatus_t));
		}

		internal static GSClientGroupStatus_t Fill(IntPtr p)
		{
			return (GSClientGroupStatus_t)Marshal.PtrToStructure(p, typeof(GSClientGroupStatus_t));
		}

		public static async Task<GSClientGroupStatus_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GSClientGroupStatus_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GSClientGroupStatus_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GSClientGroupStatus_t.StructSize, 208, ref flag) | flag))
					{
						nullable = new GSClientGroupStatus_t?(GSClientGroupStatus_t.Fill(intPtr));
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

		public static void Install(Action<GSClientGroupStatus_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GSClientGroupStatus_t.OnClient), GSClientGroupStatus_t.StructSize, 208, false);
				GSClientGroupStatus_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GSClientGroupStatus_t.OnServer), GSClientGroupStatus_t.StructSize, 208, true);
				GSClientGroupStatus_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientGroupStatus_t> action = GSClientGroupStatus_t.actionClient;
			if (action != null)
			{
				action(GSClientGroupStatus_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GSClientGroupStatus_t> action = GSClientGroupStatus_t.actionServer;
			if (action != null)
			{
				action(GSClientGroupStatus_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}