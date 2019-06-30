using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct JoinPartyCallback_t
	{
		internal Steamworks.Result Result;

		internal ulong BeaconID;

		internal ulong SteamIDBeaconOwner;

		internal string ConnectString;

		internal readonly static int StructSize;

		private static Action<JoinPartyCallback_t> actionClient;

		private static Action<JoinPartyCallback_t> actionServer;

		static JoinPartyCallback_t()
		{
			JoinPartyCallback_t.StructSize = Marshal.SizeOf(typeof(JoinPartyCallback_t));
		}

		internal static JoinPartyCallback_t Fill(IntPtr p)
		{
			return (JoinPartyCallback_t)Marshal.PtrToStructure(p, typeof(JoinPartyCallback_t));
		}

		public static async Task<JoinPartyCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			JoinPartyCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(JoinPartyCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, JoinPartyCallback_t.StructSize, 5301, ref flag) | flag))
					{
						nullable = new JoinPartyCallback_t?(JoinPartyCallback_t.Fill(intPtr));
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

		public static void Install(Action<JoinPartyCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(JoinPartyCallback_t.OnClient), JoinPartyCallback_t.StructSize, 5301, false);
				JoinPartyCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(JoinPartyCallback_t.OnServer), JoinPartyCallback_t.StructSize, 5301, true);
				JoinPartyCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<JoinPartyCallback_t> action = JoinPartyCallback_t.actionClient;
			if (action != null)
			{
				action(JoinPartyCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<JoinPartyCallback_t> action = JoinPartyCallback_t.actionServer;
			if (action != null)
			{
				action(JoinPartyCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}