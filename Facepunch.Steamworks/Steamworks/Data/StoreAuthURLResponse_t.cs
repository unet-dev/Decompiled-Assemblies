using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct StoreAuthURLResponse_t
	{
		internal string URL;

		internal readonly static int StructSize;

		private static Action<StoreAuthURLResponse_t> actionClient;

		private static Action<StoreAuthURLResponse_t> actionServer;

		static StoreAuthURLResponse_t()
		{
			StoreAuthURLResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(StoreAuthURLResponse_t) : typeof(StoreAuthURLResponse_t.Pack8)));
		}

		internal static StoreAuthURLResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (StoreAuthURLResponse_t)Marshal.PtrToStructure(p, typeof(StoreAuthURLResponse_t)) : (StoreAuthURLResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(StoreAuthURLResponse_t.Pack8)));
		}

		public static async Task<StoreAuthURLResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			StoreAuthURLResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(StoreAuthURLResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, StoreAuthURLResponse_t.StructSize, 165, ref flag) | flag))
					{
						nullable = new StoreAuthURLResponse_t?(StoreAuthURLResponse_t.Fill(intPtr));
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

		public static void Install(Action<StoreAuthURLResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(StoreAuthURLResponse_t.OnClient), StoreAuthURLResponse_t.StructSize, 165, false);
				StoreAuthURLResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(StoreAuthURLResponse_t.OnServer), StoreAuthURLResponse_t.StructSize, 165, true);
				StoreAuthURLResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<StoreAuthURLResponse_t> action = StoreAuthURLResponse_t.actionClient;
			if (action != null)
			{
				action(StoreAuthURLResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<StoreAuthURLResponse_t> action = StoreAuthURLResponse_t.actionServer;
			if (action != null)
			{
				action(StoreAuthURLResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal string URL;

			public static implicit operator StoreAuthURLResponse_t(StoreAuthURLResponse_t.Pack8 d)
			{
				return new StoreAuthURLResponse_t()
				{
					URL = d.URL
				};
			}

			public static implicit operator Pack8(StoreAuthURLResponse_t d)
			{
				return new StoreAuthURLResponse_t.Pack8()
				{
					URL = d.URL
				};
			}
		}
	}
}