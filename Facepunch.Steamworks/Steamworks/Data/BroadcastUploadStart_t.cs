using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct BroadcastUploadStart_t
	{
		internal readonly static int StructSize;

		private static Action<BroadcastUploadStart_t> actionClient;

		private static Action<BroadcastUploadStart_t> actionServer;

		static BroadcastUploadStart_t()
		{
			BroadcastUploadStart_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(BroadcastUploadStart_t) : typeof(BroadcastUploadStart_t.Pack8)));
		}

		internal static BroadcastUploadStart_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (BroadcastUploadStart_t)Marshal.PtrToStructure(p, typeof(BroadcastUploadStart_t)) : (BroadcastUploadStart_t.Pack8)Marshal.PtrToStructure(p, typeof(BroadcastUploadStart_t.Pack8)));
		}

		public static async Task<BroadcastUploadStart_t?> GetResultAsync(SteamAPICall_t handle)
		{
			BroadcastUploadStart_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(BroadcastUploadStart_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, BroadcastUploadStart_t.StructSize, 4604, ref flag) | flag))
					{
						nullable = new BroadcastUploadStart_t?(BroadcastUploadStart_t.Fill(intPtr));
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

		public static void Install(Action<BroadcastUploadStart_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(BroadcastUploadStart_t.OnClient), BroadcastUploadStart_t.StructSize, 4604, false);
				BroadcastUploadStart_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(BroadcastUploadStart_t.OnServer), BroadcastUploadStart_t.StructSize, 4604, true);
				BroadcastUploadStart_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<BroadcastUploadStart_t> action = BroadcastUploadStart_t.actionClient;
			if (action != null)
			{
				action(BroadcastUploadStart_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<BroadcastUploadStart_t> action = BroadcastUploadStart_t.actionServer;
			if (action != null)
			{
				action(BroadcastUploadStart_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator BroadcastUploadStart_t(BroadcastUploadStart_t.Pack8 d)
			{
				return new BroadcastUploadStart_t();
			}

			public static implicit operator Pack8(BroadcastUploadStart_t d)
			{
				return new BroadcastUploadStart_t.Pack8();
			}
		}
	}
}