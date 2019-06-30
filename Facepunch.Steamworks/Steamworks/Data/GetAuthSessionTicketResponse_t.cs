using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GetAuthSessionTicketResponse_t
	{
		internal uint AuthTicket;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<GetAuthSessionTicketResponse_t> actionClient;

		private static Action<GetAuthSessionTicketResponse_t> actionServer;

		static GetAuthSessionTicketResponse_t()
		{
			GetAuthSessionTicketResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GetAuthSessionTicketResponse_t) : typeof(GetAuthSessionTicketResponse_t.Pack8)));
		}

		internal static GetAuthSessionTicketResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GetAuthSessionTicketResponse_t)Marshal.PtrToStructure(p, typeof(GetAuthSessionTicketResponse_t)) : (GetAuthSessionTicketResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(GetAuthSessionTicketResponse_t.Pack8)));
		}

		public static async Task<GetAuthSessionTicketResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GetAuthSessionTicketResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GetAuthSessionTicketResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GetAuthSessionTicketResponse_t.StructSize, 163, ref flag) | flag))
					{
						nullable = new GetAuthSessionTicketResponse_t?(GetAuthSessionTicketResponse_t.Fill(intPtr));
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

		public static void Install(Action<GetAuthSessionTicketResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GetAuthSessionTicketResponse_t.OnClient), GetAuthSessionTicketResponse_t.StructSize, 163, false);
				GetAuthSessionTicketResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GetAuthSessionTicketResponse_t.OnServer), GetAuthSessionTicketResponse_t.StructSize, 163, true);
				GetAuthSessionTicketResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetAuthSessionTicketResponse_t> action = GetAuthSessionTicketResponse_t.actionClient;
			if (action != null)
			{
				action(GetAuthSessionTicketResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetAuthSessionTicketResponse_t> action = GetAuthSessionTicketResponse_t.actionServer;
			if (action != null)
			{
				action(GetAuthSessionTicketResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint AuthTicket;

			internal Steamworks.Result Result;

			public static implicit operator GetAuthSessionTicketResponse_t(GetAuthSessionTicketResponse_t.Pack8 d)
			{
				GetAuthSessionTicketResponse_t getAuthSessionTicketResponseT = new GetAuthSessionTicketResponse_t()
				{
					AuthTicket = d.AuthTicket,
					Result = d.Result
				};
				return getAuthSessionTicketResponseT;
			}

			public static implicit operator Pack8(GetAuthSessionTicketResponse_t d)
			{
				GetAuthSessionTicketResponse_t.Pack8 pack8 = new GetAuthSessionTicketResponse_t.Pack8()
				{
					AuthTicket = d.AuthTicket,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}