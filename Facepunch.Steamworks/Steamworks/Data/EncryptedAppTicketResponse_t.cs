using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct EncryptedAppTicketResponse_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<EncryptedAppTicketResponse_t> actionClient;

		private static Action<EncryptedAppTicketResponse_t> actionServer;

		static EncryptedAppTicketResponse_t()
		{
			EncryptedAppTicketResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(EncryptedAppTicketResponse_t) : typeof(EncryptedAppTicketResponse_t.Pack8)));
		}

		internal static EncryptedAppTicketResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (EncryptedAppTicketResponse_t)Marshal.PtrToStructure(p, typeof(EncryptedAppTicketResponse_t)) : (EncryptedAppTicketResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(EncryptedAppTicketResponse_t.Pack8)));
		}

		public static async Task<EncryptedAppTicketResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			EncryptedAppTicketResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(EncryptedAppTicketResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, EncryptedAppTicketResponse_t.StructSize, 154, ref flag) | flag))
					{
						nullable = new EncryptedAppTicketResponse_t?(EncryptedAppTicketResponse_t.Fill(intPtr));
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

		public static void Install(Action<EncryptedAppTicketResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(EncryptedAppTicketResponse_t.OnClient), EncryptedAppTicketResponse_t.StructSize, 154, false);
				EncryptedAppTicketResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(EncryptedAppTicketResponse_t.OnServer), EncryptedAppTicketResponse_t.StructSize, 154, true);
				EncryptedAppTicketResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<EncryptedAppTicketResponse_t> action = EncryptedAppTicketResponse_t.actionClient;
			if (action != null)
			{
				action(EncryptedAppTicketResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<EncryptedAppTicketResponse_t> action = EncryptedAppTicketResponse_t.actionServer;
			if (action != null)
			{
				action(EncryptedAppTicketResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator EncryptedAppTicketResponse_t(EncryptedAppTicketResponse_t.Pack8 d)
			{
				return new EncryptedAppTicketResponse_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(EncryptedAppTicketResponse_t d)
			{
				return new EncryptedAppTicketResponse_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}