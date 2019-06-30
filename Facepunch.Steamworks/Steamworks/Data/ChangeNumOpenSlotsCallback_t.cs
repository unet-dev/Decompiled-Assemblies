using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ChangeNumOpenSlotsCallback_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<ChangeNumOpenSlotsCallback_t> actionClient;

		private static Action<ChangeNumOpenSlotsCallback_t> actionServer;

		static ChangeNumOpenSlotsCallback_t()
		{
			ChangeNumOpenSlotsCallback_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(ChangeNumOpenSlotsCallback_t) : typeof(ChangeNumOpenSlotsCallback_t.Pack8)));
		}

		internal static ChangeNumOpenSlotsCallback_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (ChangeNumOpenSlotsCallback_t)Marshal.PtrToStructure(p, typeof(ChangeNumOpenSlotsCallback_t)) : (ChangeNumOpenSlotsCallback_t.Pack8)Marshal.PtrToStructure(p, typeof(ChangeNumOpenSlotsCallback_t.Pack8)));
		}

		public static async Task<ChangeNumOpenSlotsCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ChangeNumOpenSlotsCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ChangeNumOpenSlotsCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ChangeNumOpenSlotsCallback_t.StructSize, 5304, ref flag) | flag))
					{
						nullable = new ChangeNumOpenSlotsCallback_t?(ChangeNumOpenSlotsCallback_t.Fill(intPtr));
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

		public static void Install(Action<ChangeNumOpenSlotsCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ChangeNumOpenSlotsCallback_t.OnClient), ChangeNumOpenSlotsCallback_t.StructSize, 5304, false);
				ChangeNumOpenSlotsCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ChangeNumOpenSlotsCallback_t.OnServer), ChangeNumOpenSlotsCallback_t.StructSize, 5304, true);
				ChangeNumOpenSlotsCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ChangeNumOpenSlotsCallback_t> action = ChangeNumOpenSlotsCallback_t.actionClient;
			if (action != null)
			{
				action(ChangeNumOpenSlotsCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ChangeNumOpenSlotsCallback_t> action = ChangeNumOpenSlotsCallback_t.actionServer;
			if (action != null)
			{
				action(ChangeNumOpenSlotsCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator ChangeNumOpenSlotsCallback_t(ChangeNumOpenSlotsCallback_t.Pack8 d)
			{
				return new ChangeNumOpenSlotsCallback_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(ChangeNumOpenSlotsCallback_t d)
			{
				return new ChangeNumOpenSlotsCallback_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}