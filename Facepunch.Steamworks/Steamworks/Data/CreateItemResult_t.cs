using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct CreateItemResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal bool UserNeedsToAcceptWorkshopLegalAgreement;

		internal readonly static int StructSize;

		private static Action<CreateItemResult_t> actionClient;

		private static Action<CreateItemResult_t> actionServer;

		static CreateItemResult_t()
		{
			CreateItemResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(CreateItemResult_t) : typeof(CreateItemResult_t.Pack8)));
		}

		internal static CreateItemResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (CreateItemResult_t)Marshal.PtrToStructure(p, typeof(CreateItemResult_t)) : (CreateItemResult_t.Pack8)Marshal.PtrToStructure(p, typeof(CreateItemResult_t.Pack8)));
		}

		public static async Task<CreateItemResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			CreateItemResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(CreateItemResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, CreateItemResult_t.StructSize, 3403, ref flag) | flag))
					{
						nullable = new CreateItemResult_t?(CreateItemResult_t.Fill(intPtr));
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

		public static void Install(Action<CreateItemResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(CreateItemResult_t.OnClient), CreateItemResult_t.StructSize, 3403, false);
				CreateItemResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(CreateItemResult_t.OnServer), CreateItemResult_t.StructSize, 3403, true);
				CreateItemResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<CreateItemResult_t> action = CreateItemResult_t.actionClient;
			if (action != null)
			{
				action(CreateItemResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<CreateItemResult_t> action = CreateItemResult_t.actionServer;
			if (action != null)
			{
				action(CreateItemResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal bool UserNeedsToAcceptWorkshopLegalAgreement;

			public static implicit operator CreateItemResult_t(CreateItemResult_t.Pack8 d)
			{
				CreateItemResult_t createItemResultT = new CreateItemResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement
				};
				return createItemResultT;
			}

			public static implicit operator Pack8(CreateItemResult_t d)
			{
				CreateItemResult_t.Pack8 pack8 = new CreateItemResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					UserNeedsToAcceptWorkshopLegalAgreement = d.UserNeedsToAcceptWorkshopLegalAgreement
				};
				return pack8;
			}
		}
	}
}