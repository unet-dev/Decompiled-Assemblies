using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MicroTxnAuthorizationResponse_t
	{
		internal uint AppID;

		internal ulong OrderID;

		internal byte Authorized;

		internal readonly static int StructSize;

		private static Action<MicroTxnAuthorizationResponse_t> actionClient;

		private static Action<MicroTxnAuthorizationResponse_t> actionServer;

		static MicroTxnAuthorizationResponse_t()
		{
			MicroTxnAuthorizationResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MicroTxnAuthorizationResponse_t) : typeof(MicroTxnAuthorizationResponse_t.Pack8)));
		}

		internal static MicroTxnAuthorizationResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MicroTxnAuthorizationResponse_t)Marshal.PtrToStructure(p, typeof(MicroTxnAuthorizationResponse_t)) : (MicroTxnAuthorizationResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(MicroTxnAuthorizationResponse_t.Pack8)));
		}

		public static async Task<MicroTxnAuthorizationResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MicroTxnAuthorizationResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MicroTxnAuthorizationResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MicroTxnAuthorizationResponse_t.StructSize, 152, ref flag) | flag))
					{
						nullable = new MicroTxnAuthorizationResponse_t?(MicroTxnAuthorizationResponse_t.Fill(intPtr));
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

		public static void Install(Action<MicroTxnAuthorizationResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MicroTxnAuthorizationResponse_t.OnClient), MicroTxnAuthorizationResponse_t.StructSize, 152, false);
				MicroTxnAuthorizationResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MicroTxnAuthorizationResponse_t.OnServer), MicroTxnAuthorizationResponse_t.StructSize, 152, true);
				MicroTxnAuthorizationResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MicroTxnAuthorizationResponse_t> action = MicroTxnAuthorizationResponse_t.actionClient;
			if (action != null)
			{
				action(MicroTxnAuthorizationResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MicroTxnAuthorizationResponse_t> action = MicroTxnAuthorizationResponse_t.actionServer;
			if (action != null)
			{
				action(MicroTxnAuthorizationResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint AppID;

			internal ulong OrderID;

			internal byte Authorized;

			public static implicit operator MicroTxnAuthorizationResponse_t(MicroTxnAuthorizationResponse_t.Pack8 d)
			{
				MicroTxnAuthorizationResponse_t microTxnAuthorizationResponseT = new MicroTxnAuthorizationResponse_t()
				{
					AppID = d.AppID,
					OrderID = d.OrderID,
					Authorized = d.Authorized
				};
				return microTxnAuthorizationResponseT;
			}

			public static implicit operator Pack8(MicroTxnAuthorizationResponse_t d)
			{
				MicroTxnAuthorizationResponse_t.Pack8 pack8 = new MicroTxnAuthorizationResponse_t.Pack8()
				{
					AppID = d.AppID,
					OrderID = d.OrderID,
					Authorized = d.Authorized
				};
				return pack8;
			}
		}
	}
}