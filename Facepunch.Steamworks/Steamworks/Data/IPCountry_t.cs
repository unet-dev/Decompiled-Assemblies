using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct IPCountry_t
	{
		internal readonly static int StructSize;

		private static Action<IPCountry_t> actionClient;

		private static Action<IPCountry_t> actionServer;

		static IPCountry_t()
		{
			IPCountry_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(IPCountry_t) : typeof(IPCountry_t.Pack8)));
		}

		internal static IPCountry_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (IPCountry_t)Marshal.PtrToStructure(p, typeof(IPCountry_t)) : (IPCountry_t.Pack8)Marshal.PtrToStructure(p, typeof(IPCountry_t.Pack8)));
		}

		public static async Task<IPCountry_t?> GetResultAsync(SteamAPICall_t handle)
		{
			IPCountry_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(IPCountry_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, IPCountry_t.StructSize, 701, ref flag) | flag))
					{
						nullable = new IPCountry_t?(IPCountry_t.Fill(intPtr));
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

		public static void Install(Action<IPCountry_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(IPCountry_t.OnClient), IPCountry_t.StructSize, 701, false);
				IPCountry_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(IPCountry_t.OnServer), IPCountry_t.StructSize, 701, true);
				IPCountry_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<IPCountry_t> action = IPCountry_t.actionClient;
			if (action != null)
			{
				action(IPCountry_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<IPCountry_t> action = IPCountry_t.actionServer;
			if (action != null)
			{
				action(IPCountry_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator IPCountry_t(IPCountry_t.Pack8 d)
			{
				return new IPCountry_t();
			}

			public static implicit operator Pack8(IPCountry_t d)
			{
				return new IPCountry_t.Pack8();
			}
		}
	}
}