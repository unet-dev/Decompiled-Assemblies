using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct BroadcastUploadStop_t
	{
		internal BroadcastUploadResult Result;

		internal readonly static int StructSize;

		private static Action<BroadcastUploadStop_t> actionClient;

		private static Action<BroadcastUploadStop_t> actionServer;

		static BroadcastUploadStop_t()
		{
			BroadcastUploadStop_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(BroadcastUploadStop_t) : typeof(BroadcastUploadStop_t.Pack8)));
		}

		internal static BroadcastUploadStop_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (BroadcastUploadStop_t)Marshal.PtrToStructure(p, typeof(BroadcastUploadStop_t)) : (BroadcastUploadStop_t.Pack8)Marshal.PtrToStructure(p, typeof(BroadcastUploadStop_t.Pack8)));
		}

		public static async Task<BroadcastUploadStop_t?> GetResultAsync(SteamAPICall_t handle)
		{
			BroadcastUploadStop_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(BroadcastUploadStop_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, BroadcastUploadStop_t.StructSize, 4605, ref flag) | flag))
					{
						nullable = new BroadcastUploadStop_t?(BroadcastUploadStop_t.Fill(intPtr));
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

		public static void Install(Action<BroadcastUploadStop_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(BroadcastUploadStop_t.OnClient), BroadcastUploadStop_t.StructSize, 4605, false);
				BroadcastUploadStop_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(BroadcastUploadStop_t.OnServer), BroadcastUploadStop_t.StructSize, 4605, true);
				BroadcastUploadStop_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<BroadcastUploadStop_t> action = BroadcastUploadStop_t.actionClient;
			if (action != null)
			{
				action(BroadcastUploadStop_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<BroadcastUploadStop_t> action = BroadcastUploadStop_t.actionServer;
			if (action != null)
			{
				action(BroadcastUploadStop_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal BroadcastUploadResult Result;

			public static implicit operator BroadcastUploadStop_t(BroadcastUploadStop_t.Pack8 d)
			{
				return new BroadcastUploadStop_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(BroadcastUploadStop_t d)
			{
				return new BroadcastUploadStop_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}