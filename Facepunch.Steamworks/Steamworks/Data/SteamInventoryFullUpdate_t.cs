using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamInventoryFullUpdate_t
	{
		internal int Handle;

		internal readonly static int StructSize;

		private static Action<SteamInventoryFullUpdate_t> actionClient;

		private static Action<SteamInventoryFullUpdate_t> actionServer;

		static SteamInventoryFullUpdate_t()
		{
			SteamInventoryFullUpdate_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamInventoryFullUpdate_t) : typeof(SteamInventoryFullUpdate_t.Pack8)));
		}

		internal static SteamInventoryFullUpdate_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamInventoryFullUpdate_t)Marshal.PtrToStructure(p, typeof(SteamInventoryFullUpdate_t)) : (SteamInventoryFullUpdate_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamInventoryFullUpdate_t.Pack8)));
		}

		public static async Task<SteamInventoryFullUpdate_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamInventoryFullUpdate_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamInventoryFullUpdate_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamInventoryFullUpdate_t.StructSize, 4701, ref flag) | flag))
					{
						nullable = new SteamInventoryFullUpdate_t?(SteamInventoryFullUpdate_t.Fill(intPtr));
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

		public static void Install(Action<SteamInventoryFullUpdate_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamInventoryFullUpdate_t.OnClient), SteamInventoryFullUpdate_t.StructSize, 4701, false);
				SteamInventoryFullUpdate_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamInventoryFullUpdate_t.OnServer), SteamInventoryFullUpdate_t.StructSize, 4701, true);
				SteamInventoryFullUpdate_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryFullUpdate_t> action = SteamInventoryFullUpdate_t.actionClient;
			if (action != null)
			{
				action(SteamInventoryFullUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryFullUpdate_t> action = SteamInventoryFullUpdate_t.actionServer;
			if (action != null)
			{
				action(SteamInventoryFullUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal int Handle;

			public static implicit operator SteamInventoryFullUpdate_t(SteamInventoryFullUpdate_t.Pack8 d)
			{
				return new SteamInventoryFullUpdate_t()
				{
					Handle = d.Handle
				};
			}

			public static implicit operator Pack8(SteamInventoryFullUpdate_t d)
			{
				return new SteamInventoryFullUpdate_t.Pack8()
				{
					Handle = d.Handle
				};
			}
		}
	}
}