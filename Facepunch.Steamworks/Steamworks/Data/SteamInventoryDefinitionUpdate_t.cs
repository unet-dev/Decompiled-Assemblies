using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamInventoryDefinitionUpdate_t
	{
		internal readonly static int StructSize;

		private static Action<SteamInventoryDefinitionUpdate_t> actionClient;

		private static Action<SteamInventoryDefinitionUpdate_t> actionServer;

		static SteamInventoryDefinitionUpdate_t()
		{
			SteamInventoryDefinitionUpdate_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamInventoryDefinitionUpdate_t) : typeof(SteamInventoryDefinitionUpdate_t.Pack8)));
		}

		internal static SteamInventoryDefinitionUpdate_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamInventoryDefinitionUpdate_t)Marshal.PtrToStructure(p, typeof(SteamInventoryDefinitionUpdate_t)) : (SteamInventoryDefinitionUpdate_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamInventoryDefinitionUpdate_t.Pack8)));
		}

		public static async Task<SteamInventoryDefinitionUpdate_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamInventoryDefinitionUpdate_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamInventoryDefinitionUpdate_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamInventoryDefinitionUpdate_t.StructSize, 4702, ref flag) | flag))
					{
						nullable = new SteamInventoryDefinitionUpdate_t?(SteamInventoryDefinitionUpdate_t.Fill(intPtr));
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

		public static void Install(Action<SteamInventoryDefinitionUpdate_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamInventoryDefinitionUpdate_t.OnClient), SteamInventoryDefinitionUpdate_t.StructSize, 4702, false);
				SteamInventoryDefinitionUpdate_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamInventoryDefinitionUpdate_t.OnServer), SteamInventoryDefinitionUpdate_t.StructSize, 4702, true);
				SteamInventoryDefinitionUpdate_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryDefinitionUpdate_t> action = SteamInventoryDefinitionUpdate_t.actionClient;
			if (action != null)
			{
				action(SteamInventoryDefinitionUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamInventoryDefinitionUpdate_t> action = SteamInventoryDefinitionUpdate_t.actionServer;
			if (action != null)
			{
				action(SteamInventoryDefinitionUpdate_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator SteamInventoryDefinitionUpdate_t(SteamInventoryDefinitionUpdate_t.Pack8 d)
			{
				return new SteamInventoryDefinitionUpdate_t();
			}

			public static implicit operator Pack8(SteamInventoryDefinitionUpdate_t d)
			{
				return new SteamInventoryDefinitionUpdate_t.Pack8();
			}
		}
	}
}