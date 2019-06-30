using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct LicensesUpdated_t
	{
		internal readonly static int StructSize;

		private static Action<LicensesUpdated_t> actionClient;

		private static Action<LicensesUpdated_t> actionServer;

		static LicensesUpdated_t()
		{
			LicensesUpdated_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(LicensesUpdated_t) : typeof(LicensesUpdated_t.Pack8)));
		}

		internal static LicensesUpdated_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (LicensesUpdated_t)Marshal.PtrToStructure(p, typeof(LicensesUpdated_t)) : (LicensesUpdated_t.Pack8)Marshal.PtrToStructure(p, typeof(LicensesUpdated_t.Pack8)));
		}

		public static async Task<LicensesUpdated_t?> GetResultAsync(SteamAPICall_t handle)
		{
			LicensesUpdated_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(LicensesUpdated_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, LicensesUpdated_t.StructSize, 125, ref flag) | flag))
					{
						nullable = new LicensesUpdated_t?(LicensesUpdated_t.Fill(intPtr));
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

		public static void Install(Action<LicensesUpdated_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(LicensesUpdated_t.OnClient), LicensesUpdated_t.StructSize, 125, false);
				LicensesUpdated_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(LicensesUpdated_t.OnServer), LicensesUpdated_t.StructSize, 125, true);
				LicensesUpdated_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LicensesUpdated_t> action = LicensesUpdated_t.actionClient;
			if (action != null)
			{
				action(LicensesUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<LicensesUpdated_t> action = LicensesUpdated_t.actionServer;
			if (action != null)
			{
				action(LicensesUpdated_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator LicensesUpdated_t(LicensesUpdated_t.Pack8 d)
			{
				return new LicensesUpdated_t();
			}

			public static implicit operator Pack8(LicensesUpdated_t d)
			{
				return new LicensesUpdated_t.Pack8();
			}
		}
	}
}