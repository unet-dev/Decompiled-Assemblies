using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamInventoryStartPurchaseResult_t
	{
		internal Steamworks.Result Result;

		internal ulong OrderID;

		internal ulong TransID;

		internal readonly static int StructSize;

		private static Action<SteamInventoryStartPurchaseResult_t> actionClient;

		private static Action<SteamInventoryStartPurchaseResult_t> actionServer;

		static SteamInventoryStartPurchaseResult_t()
		{
			SteamInventoryStartPurchaseResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamInventoryStartPurchaseResult_t) : typeof(SteamInventoryStartPurchaseResult_t.Pack8)));
		}

		internal static SteamInventoryStartPurchaseResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamInventoryStartPurchaseResult_t)Marshal.PtrToStructure(p, typeof(SteamInventoryStartPurchaseResult_t)) : (SteamInventoryStartPurchaseResult_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamInventoryStartPurchaseResult_t.Pack8)));
		}

		public static async Task<SteamInventoryStartPurchaseResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamInventoryStartPurchaseResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamInventoryStartPurchaseResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamInventoryStartPurchaseResult_t.StructSize, 4704, ref flag) | flag))
					{
						nullable = new SteamInventoryStartPurchaseResult_t?(SteamInventoryStartPurchaseResult_t.Fill(intPtr));
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

		public static void Install(Action<SteamInventoryStartPurchaseResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamInventoryStartPurchaseResult_t.OnClient), SteamInventoryStartPurchaseResult_t.StructSize, 4704, false);
				SteamInventoryStartPurchaseResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamInventoryStartPurchaseResult_t.OnServer), SteamInventoryStartPurchaseResult_t.StructSize, 4704, true);
				SteamInventoryStartPurchaseResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryStartPurchaseResult_t> action = SteamInventoryStartPurchaseResult_t.actionClient;
			if (action != null)
			{
				action(SteamInventoryStartPurchaseResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryStartPurchaseResult_t> action = SteamInventoryStartPurchaseResult_t.actionServer;
			if (action != null)
			{
				action(SteamInventoryStartPurchaseResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong OrderID;

			internal ulong TransID;

			public static implicit operator SteamInventoryStartPurchaseResult_t(SteamInventoryStartPurchaseResult_t.Pack8 d)
			{
				SteamInventoryStartPurchaseResult_t steamInventoryStartPurchaseResultT = new SteamInventoryStartPurchaseResult_t()
				{
					Result = d.Result,
					OrderID = d.OrderID,
					TransID = d.TransID
				};
				return steamInventoryStartPurchaseResultT;
			}

			public static implicit operator Pack8(SteamInventoryStartPurchaseResult_t d)
			{
				SteamInventoryStartPurchaseResult_t.Pack8 pack8 = new SteamInventoryStartPurchaseResult_t.Pack8()
				{
					Result = d.Result,
					OrderID = d.OrderID,
					TransID = d.TransID
				};
				return pack8;
			}
		}
	}
}