using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct AppProofOfPurchaseKeyResponse_t
	{
		internal Steamworks.Result Result;

		internal uint AppID;

		internal uint CchKeyLength;

		internal string Key;

		internal readonly static int StructSize;

		private static Action<AppProofOfPurchaseKeyResponse_t> actionClient;

		private static Action<AppProofOfPurchaseKeyResponse_t> actionServer;

		static AppProofOfPurchaseKeyResponse_t()
		{
			AppProofOfPurchaseKeyResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(AppProofOfPurchaseKeyResponse_t) : typeof(AppProofOfPurchaseKeyResponse_t.Pack8)));
		}

		internal static AppProofOfPurchaseKeyResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (AppProofOfPurchaseKeyResponse_t)Marshal.PtrToStructure(p, typeof(AppProofOfPurchaseKeyResponse_t)) : (AppProofOfPurchaseKeyResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(AppProofOfPurchaseKeyResponse_t.Pack8)));
		}

		public static async Task<AppProofOfPurchaseKeyResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			AppProofOfPurchaseKeyResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(AppProofOfPurchaseKeyResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, AppProofOfPurchaseKeyResponse_t.StructSize, 1021, ref flag) | flag))
					{
						nullable = new AppProofOfPurchaseKeyResponse_t?(AppProofOfPurchaseKeyResponse_t.Fill(intPtr));
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

		public static void Install(Action<AppProofOfPurchaseKeyResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(AppProofOfPurchaseKeyResponse_t.OnClient), AppProofOfPurchaseKeyResponse_t.StructSize, 1021, false);
				AppProofOfPurchaseKeyResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(AppProofOfPurchaseKeyResponse_t.OnServer), AppProofOfPurchaseKeyResponse_t.StructSize, 1021, true);
				AppProofOfPurchaseKeyResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AppProofOfPurchaseKeyResponse_t> action = AppProofOfPurchaseKeyResponse_t.actionClient;
			if (action != null)
			{
				action(AppProofOfPurchaseKeyResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AppProofOfPurchaseKeyResponse_t> action = AppProofOfPurchaseKeyResponse_t.actionServer;
			if (action != null)
			{
				action(AppProofOfPurchaseKeyResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal uint AppID;

			internal uint CchKeyLength;

			internal string Key;

			public static implicit operator AppProofOfPurchaseKeyResponse_t(AppProofOfPurchaseKeyResponse_t.Pack8 d)
			{
				AppProofOfPurchaseKeyResponse_t appProofOfPurchaseKeyResponseT = new AppProofOfPurchaseKeyResponse_t()
				{
					Result = d.Result,
					AppID = d.AppID,
					CchKeyLength = d.CchKeyLength,
					Key = d.Key
				};
				return appProofOfPurchaseKeyResponseT;
			}

			public static implicit operator Pack8(AppProofOfPurchaseKeyResponse_t d)
			{
				AppProofOfPurchaseKeyResponse_t.Pack8 pack8 = new AppProofOfPurchaseKeyResponse_t.Pack8()
				{
					Result = d.Result,
					AppID = d.AppID,
					CchKeyLength = d.CchKeyLength,
					Key = d.Key
				};
				return pack8;
			}
		}
	}
}