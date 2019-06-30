using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ValidateAuthTicketResponse_t
	{
		internal ulong SteamID;

		internal AuthResponse AuthSessionResponse;

		internal ulong OwnerSteamID;

		internal readonly static int StructSize;

		private static Action<ValidateAuthTicketResponse_t> actionClient;

		private static Action<ValidateAuthTicketResponse_t> actionServer;

		static ValidateAuthTicketResponse_t()
		{
			ValidateAuthTicketResponse_t.StructSize = Marshal.SizeOf(typeof(ValidateAuthTicketResponse_t));
		}

		internal static ValidateAuthTicketResponse_t Fill(IntPtr p)
		{
			return (ValidateAuthTicketResponse_t)Marshal.PtrToStructure(p, typeof(ValidateAuthTicketResponse_t));
		}

		public static async Task<ValidateAuthTicketResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ValidateAuthTicketResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ValidateAuthTicketResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ValidateAuthTicketResponse_t.StructSize, 143, ref flag) | flag))
					{
						nullable = new ValidateAuthTicketResponse_t?(ValidateAuthTicketResponse_t.Fill(intPtr));
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

		public static void Install(Action<ValidateAuthTicketResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ValidateAuthTicketResponse_t.OnClient), ValidateAuthTicketResponse_t.StructSize, 143, false);
				ValidateAuthTicketResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ValidateAuthTicketResponse_t.OnServer), ValidateAuthTicketResponse_t.StructSize, 143, true);
				ValidateAuthTicketResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ValidateAuthTicketResponse_t> action = ValidateAuthTicketResponse_t.actionClient;
			if (action != null)
			{
				action(ValidateAuthTicketResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ValidateAuthTicketResponse_t> action = ValidateAuthTicketResponse_t.actionServer;
			if (action != null)
			{
				action(ValidateAuthTicketResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}