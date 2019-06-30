using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct PSNGameBootInviteResult_t
	{
		internal bool GameBootInviteExists;

		internal ulong SteamIDLobby;

		internal readonly static int StructSize;

		private static Action<PSNGameBootInviteResult_t> actionClient;

		private static Action<PSNGameBootInviteResult_t> actionServer;

		static PSNGameBootInviteResult_t()
		{
			PSNGameBootInviteResult_t.StructSize = Marshal.SizeOf(typeof(PSNGameBootInviteResult_t));
		}

		internal static PSNGameBootInviteResult_t Fill(IntPtr p)
		{
			return (PSNGameBootInviteResult_t)Marshal.PtrToStructure(p, typeof(PSNGameBootInviteResult_t));
		}

		public static async Task<PSNGameBootInviteResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			PSNGameBootInviteResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(PSNGameBootInviteResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, PSNGameBootInviteResult_t.StructSize, 515, ref flag) | flag))
					{
						nullable = new PSNGameBootInviteResult_t?(PSNGameBootInviteResult_t.Fill(intPtr));
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

		public static void Install(Action<PSNGameBootInviteResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(PSNGameBootInviteResult_t.OnClient), PSNGameBootInviteResult_t.StructSize, 515, false);
				PSNGameBootInviteResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(PSNGameBootInviteResult_t.OnServer), PSNGameBootInviteResult_t.StructSize, 515, true);
				PSNGameBootInviteResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<PSNGameBootInviteResult_t> action = PSNGameBootInviteResult_t.actionClient;
			if (action != null)
			{
				action(PSNGameBootInviteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<PSNGameBootInviteResult_t> action = PSNGameBootInviteResult_t.actionServer;
			if (action != null)
			{
				action(PSNGameBootInviteResult_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}