using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamInventoryEligiblePromoItemDefIDs_t
	{
		internal Steamworks.Result Result;

		internal ulong SteamID;

		internal int UmEligiblePromoItemDefs;

		internal bool CachedData;

		internal readonly static int StructSize;

		private static Action<SteamInventoryEligiblePromoItemDefIDs_t> actionClient;

		private static Action<SteamInventoryEligiblePromoItemDefIDs_t> actionServer;

		static SteamInventoryEligiblePromoItemDefIDs_t()
		{
			SteamInventoryEligiblePromoItemDefIDs_t.StructSize = Marshal.SizeOf(typeof(SteamInventoryEligiblePromoItemDefIDs_t));
		}

		internal static SteamInventoryEligiblePromoItemDefIDs_t Fill(IntPtr p)
		{
			return (SteamInventoryEligiblePromoItemDefIDs_t)Marshal.PtrToStructure(p, typeof(SteamInventoryEligiblePromoItemDefIDs_t));
		}

		public static async Task<SteamInventoryEligiblePromoItemDefIDs_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamInventoryEligiblePromoItemDefIDs_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamInventoryEligiblePromoItemDefIDs_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamInventoryEligiblePromoItemDefIDs_t.StructSize, 4703, ref flag) | flag))
					{
						nullable = new SteamInventoryEligiblePromoItemDefIDs_t?(SteamInventoryEligiblePromoItemDefIDs_t.Fill(intPtr));
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

		public static void Install(Action<SteamInventoryEligiblePromoItemDefIDs_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamInventoryEligiblePromoItemDefIDs_t.OnClient), SteamInventoryEligiblePromoItemDefIDs_t.StructSize, 4703, false);
				SteamInventoryEligiblePromoItemDefIDs_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamInventoryEligiblePromoItemDefIDs_t.OnServer), SteamInventoryEligiblePromoItemDefIDs_t.StructSize, 4703, true);
				SteamInventoryEligiblePromoItemDefIDs_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryEligiblePromoItemDefIDs_t> action = SteamInventoryEligiblePromoItemDefIDs_t.actionClient;
			if (action != null)
			{
				action(SteamInventoryEligiblePromoItemDefIDs_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryEligiblePromoItemDefIDs_t> action = SteamInventoryEligiblePromoItemDefIDs_t.actionServer;
			if (action != null)
			{
				action(SteamInventoryEligiblePromoItemDefIDs_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}