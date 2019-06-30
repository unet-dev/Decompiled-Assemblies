using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SubmitItemUpdateResult_t
	{
		internal Steamworks.Result Result;

		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal readonly static int StructSize;

		private static Action<SubmitItemUpdateResult_t> actionClient;

		private static Action<SubmitItemUpdateResult_t> actionServer;

		static SubmitItemUpdateResult_t()
		{
			SubmitItemUpdateResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SubmitItemUpdateResult_t) : typeof(SubmitItemUpdateResult_t.Pack8)));
		}

		internal static SubmitItemUpdateResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SubmitItemUpdateResult_t)Marshal.PtrToStructure(p, typeof(SubmitItemUpdateResult_t)) : (SubmitItemUpdateResult_t.Pack8)Marshal.PtrToStructure(p, typeof(SubmitItemUpdateResult_t.Pack8)));
		}

		public static async Task<SubmitItemUpdateResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SubmitItemUpdateResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SubmitItemUpdateResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SubmitItemUpdateResult_t.StructSize, 3404, ref flag) | flag))
					{
						nullable = new SubmitItemUpdateResult_t?(SubmitItemUpdateResult_t.Fill(intPtr));
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

		public static void Install(Action<SubmitItemUpdateResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SubmitItemUpdateResult_t.OnClient), SubmitItemUpdateResult_t.StructSize, 3404, false);
				SubmitItemUpdateResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SubmitItemUpdateResult_t.OnServer), SubmitItemUpdateResult_t.StructSize, 3404, true);
				SubmitItemUpdateResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SubmitItemUpdateResult_t> action = SubmitItemUpdateResult_t.actionClient;
			if (action != null)
			{
				action(SubmitItemUpdateResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SubmitItemUpdateResult_t> action = SubmitItemUpdateResult_t.actionServer;
			if (action != null)
			{
				action(SubmitItemUpdateResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal bool UserNeedsToAcceptWorkshopLegalAgreement;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			public static implicit operator SubmitItemUpdateResult_t(SubmitItemUpdateResult_t.Pack8 d)
			{
				SubmitItemUpdateResult_t submitItemUpdateResultT = new SubmitItemUpdateResult_t()
				{
					Result = d.Result,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement,
					PublishedFileId = d.PublishedFileId
				};
				return submitItemUpdateResultT;
			}

			public static implicit operator Pack8(SubmitItemUpdateResult_t d)
			{
				SubmitItemUpdateResult_t.Pack8 pack8 = new SubmitItemUpdateResult_t.Pack8()
				{
					Result = d.Result,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement,
					PublishedFileId = d.PublishedFileId
				};
				return pack8;
			}
		}
	}
}