using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamInventoryRequestPricesResult_t
	{
		internal Steamworks.Result Result;

		internal string Currency;

		internal readonly static int StructSize;

		private static Action<SteamInventoryRequestPricesResult_t> actionClient;

		private static Action<SteamInventoryRequestPricesResult_t> actionServer;

		static SteamInventoryRequestPricesResult_t()
		{
			SteamInventoryRequestPricesResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamInventoryRequestPricesResult_t) : typeof(SteamInventoryRequestPricesResult_t.Pack8)));
		}

		internal static SteamInventoryRequestPricesResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamInventoryRequestPricesResult_t)Marshal.PtrToStructure(p, typeof(SteamInventoryRequestPricesResult_t)) : (SteamInventoryRequestPricesResult_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamInventoryRequestPricesResult_t.Pack8)));
		}

		public static async Task<SteamInventoryRequestPricesResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamInventoryRequestPricesResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamInventoryRequestPricesResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamInventoryRequestPricesResult_t.StructSize, 4705, ref flag) | flag))
					{
						nullable = new SteamInventoryRequestPricesResult_t?(SteamInventoryRequestPricesResult_t.Fill(intPtr));
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

		public static void Install(Action<SteamInventoryRequestPricesResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamInventoryRequestPricesResult_t.OnClient), SteamInventoryRequestPricesResult_t.StructSize, 4705, false);
				SteamInventoryRequestPricesResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamInventoryRequestPricesResult_t.OnServer), SteamInventoryRequestPricesResult_t.StructSize, 4705, true);
				SteamInventoryRequestPricesResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryRequestPricesResult_t> action = SteamInventoryRequestPricesResult_t.actionClient;
			if (action != null)
			{
				action(SteamInventoryRequestPricesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryRequestPricesResult_t> action = SteamInventoryRequestPricesResult_t.actionServer;
			if (action != null)
			{
				action(SteamInventoryRequestPricesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal string Currency;

			public static implicit operator SteamInventoryRequestPricesResult_t(SteamInventoryRequestPricesResult_t.Pack8 d)
			{
				SteamInventoryRequestPricesResult_t steamInventoryRequestPricesResultT = new SteamInventoryRequestPricesResult_t()
				{
					Result = d.Result,
					Currency = d.Currency
				};
				return steamInventoryRequestPricesResultT;
			}

			public static implicit operator Pack8(SteamInventoryRequestPricesResult_t d)
			{
				SteamInventoryRequestPricesResult_t.Pack8 pack8 = new SteamInventoryRequestPricesResult_t.Pack8()
				{
					Result = d.Result,
					Currency = d.Currency
				};
				return pack8;
			}
		}
	}
}