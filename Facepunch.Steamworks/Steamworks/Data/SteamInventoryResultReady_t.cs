using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamInventoryResultReady_t
	{
		internal int Handle;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<SteamInventoryResultReady_t> actionClient;

		private static Action<SteamInventoryResultReady_t> actionServer;

		static SteamInventoryResultReady_t()
		{
			SteamInventoryResultReady_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamInventoryResultReady_t) : typeof(SteamInventoryResultReady_t.Pack8)));
		}

		internal static SteamInventoryResultReady_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamInventoryResultReady_t)Marshal.PtrToStructure(p, typeof(SteamInventoryResultReady_t)) : (SteamInventoryResultReady_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamInventoryResultReady_t.Pack8)));
		}

		public static async Task<SteamInventoryResultReady_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamInventoryResultReady_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamInventoryResultReady_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamInventoryResultReady_t.StructSize, 4700, ref flag) | flag))
					{
						nullable = new SteamInventoryResultReady_t?(SteamInventoryResultReady_t.Fill(intPtr));
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

		public static void Install(Action<SteamInventoryResultReady_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamInventoryResultReady_t.OnClient), SteamInventoryResultReady_t.StructSize, 4700, false);
				SteamInventoryResultReady_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamInventoryResultReady_t.OnServer), SteamInventoryResultReady_t.StructSize, 4700, true);
				SteamInventoryResultReady_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryResultReady_t> action = SteamInventoryResultReady_t.actionClient;
			if (action != null)
			{
				action(SteamInventoryResultReady_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryResultReady_t> action = SteamInventoryResultReady_t.actionServer;
			if (action != null)
			{
				action(SteamInventoryResultReady_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal int Handle;

			internal Steamworks.Result Result;

			public static implicit operator SteamInventoryResultReady_t(SteamInventoryResultReady_t.Pack8 d)
			{
				SteamInventoryResultReady_t steamInventoryResultReadyT = new SteamInventoryResultReady_t()
				{
					Handle = d.Handle,
					Result = d.Result
				};
				return steamInventoryResultReadyT;
			}

			public static implicit operator Pack8(SteamInventoryResultReady_t d)
			{
				SteamInventoryResultReady_t.Pack8 pack8 = new SteamInventoryResultReady_t.Pack8()
				{
					Handle = d.Handle,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}