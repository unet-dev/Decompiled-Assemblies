using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ClanOfficerListResponse_t
	{
		internal ulong SteamIDClan;

		internal int COfficers;

		internal byte Success;

		internal readonly static int StructSize;

		private static Action<ClanOfficerListResponse_t> actionClient;

		private static Action<ClanOfficerListResponse_t> actionServer;

		static ClanOfficerListResponse_t()
		{
			ClanOfficerListResponse_t.StructSize = Marshal.SizeOf(typeof(ClanOfficerListResponse_t));
		}

		internal static ClanOfficerListResponse_t Fill(IntPtr p)
		{
			return (ClanOfficerListResponse_t)Marshal.PtrToStructure(p, typeof(ClanOfficerListResponse_t));
		}

		public static async Task<ClanOfficerListResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ClanOfficerListResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ClanOfficerListResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ClanOfficerListResponse_t.StructSize, 335, ref flag) | flag))
					{
						nullable = new ClanOfficerListResponse_t?(ClanOfficerListResponse_t.Fill(intPtr));
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

		public static void Install(Action<ClanOfficerListResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ClanOfficerListResponse_t.OnClient), ClanOfficerListResponse_t.StructSize, 335, false);
				ClanOfficerListResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ClanOfficerListResponse_t.OnServer), ClanOfficerListResponse_t.StructSize, 335, true);
				ClanOfficerListResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ClanOfficerListResponse_t> action = ClanOfficerListResponse_t.actionClient;
			if (action != null)
			{
				action(ClanOfficerListResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ClanOfficerListResponse_t> action = ClanOfficerListResponse_t.actionServer;
			if (action != null)
			{
				action(ClanOfficerListResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}