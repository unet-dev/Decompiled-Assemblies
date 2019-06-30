using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MarketEligibilityResponse_t
	{
		internal bool Allowed;

		internal MarketNotAllowedReasonFlags NotAllowedReason;

		internal uint TAllowedAtTime;

		internal int CdaySteamGuardRequiredDays;

		internal int CdayNewDeviceCooldown;

		internal readonly static int StructSize;

		private static Action<MarketEligibilityResponse_t> actionClient;

		private static Action<MarketEligibilityResponse_t> actionServer;

		static MarketEligibilityResponse_t()
		{
			MarketEligibilityResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MarketEligibilityResponse_t) : typeof(MarketEligibilityResponse_t.Pack8)));
		}

		internal static MarketEligibilityResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MarketEligibilityResponse_t)Marshal.PtrToStructure(p, typeof(MarketEligibilityResponse_t)) : (MarketEligibilityResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(MarketEligibilityResponse_t.Pack8)));
		}

		public static async Task<MarketEligibilityResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MarketEligibilityResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MarketEligibilityResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MarketEligibilityResponse_t.StructSize, 166, ref flag) | flag))
					{
						nullable = new MarketEligibilityResponse_t?(MarketEligibilityResponse_t.Fill(intPtr));
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

		public static void Install(Action<MarketEligibilityResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MarketEligibilityResponse_t.OnClient), MarketEligibilityResponse_t.StructSize, 166, false);
				MarketEligibilityResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MarketEligibilityResponse_t.OnServer), MarketEligibilityResponse_t.StructSize, 166, true);
				MarketEligibilityResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MarketEligibilityResponse_t> action = MarketEligibilityResponse_t.actionClient;
			if (action != null)
			{
				action(MarketEligibilityResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MarketEligibilityResponse_t> action = MarketEligibilityResponse_t.actionServer;
			if (action != null)
			{
				action(MarketEligibilityResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal bool Allowed;

			internal MarketNotAllowedReasonFlags NotAllowedReason;

			internal uint TAllowedAtTime;

			internal int CdaySteamGuardRequiredDays;

			internal int CdayNewDeviceCooldown;

			public static implicit operator MarketEligibilityResponse_t(MarketEligibilityResponse_t.Pack8 d)
			{
				MarketEligibilityResponse_t marketEligibilityResponseT = new MarketEligibilityResponse_t()
				{
					Allowed = d.Allowed,
					NotAllowedReason = d.NotAllowedReason,
					TAllowedAtTime = d.TAllowedAtTime,
					CdaySteamGuardRequiredDays = d.CdaySteamGuardRequiredDays,
					CdayNewDeviceCooldown = d.CdayNewDeviceCooldown
				};
				return marketEligibilityResponseT;
			}

			public static implicit operator Pack8(MarketEligibilityResponse_t d)
			{
				MarketEligibilityResponse_t.Pack8 pack8 = new MarketEligibilityResponse_t.Pack8()
				{
					Allowed = d.Allowed,
					NotAllowedReason = d.NotAllowedReason,
					TAllowedAtTime = d.TAllowedAtTime,
					CdaySteamGuardRequiredDays = d.CdaySteamGuardRequiredDays,
					CdayNewDeviceCooldown = d.CdayNewDeviceCooldown
				};
				return pack8;
			}
		}
	}
}