using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamParentalSettingsChanged_t
	{
		internal readonly static int StructSize;

		private static Action<SteamParentalSettingsChanged_t> actionClient;

		private static Action<SteamParentalSettingsChanged_t> actionServer;

		static SteamParentalSettingsChanged_t()
		{
			SteamParentalSettingsChanged_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamParentalSettingsChanged_t) : typeof(SteamParentalSettingsChanged_t.Pack8)));
		}

		internal static SteamParentalSettingsChanged_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamParentalSettingsChanged_t)Marshal.PtrToStructure(p, typeof(SteamParentalSettingsChanged_t)) : (SteamParentalSettingsChanged_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamParentalSettingsChanged_t.Pack8)));
		}

		public static async Task<SteamParentalSettingsChanged_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamParentalSettingsChanged_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamParentalSettingsChanged_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamParentalSettingsChanged_t.StructSize, 5001, ref flag) | flag))
					{
						nullable = new SteamParentalSettingsChanged_t?(SteamParentalSettingsChanged_t.Fill(intPtr));
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

		public static void Install(Action<SteamParentalSettingsChanged_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SteamParentalSettingsChanged_t.OnClient), SteamParentalSettingsChanged_t.StructSize, 5001, false);
				SteamParentalSettingsChanged_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SteamParentalSettingsChanged_t.OnServer), SteamParentalSettingsChanged_t.StructSize, 5001, true);
				SteamParentalSettingsChanged_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamParentalSettingsChanged_t> action = SteamParentalSettingsChanged_t.actionClient;
			if (action != null)
			{
				action(SteamParentalSettingsChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamParentalSettingsChanged_t> action = SteamParentalSettingsChanged_t.actionServer;
			if (action != null)
			{
				action(SteamParentalSettingsChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator SteamParentalSettingsChanged_t(SteamParentalSettingsChanged_t.Pack8 d)
			{
				return new SteamParentalSettingsChanged_t();
			}

			public static implicit operator Pack8(SteamParentalSettingsChanged_t d)
			{
				return new SteamParentalSettingsChanged_t.Pack8();
			}
		}
	}
}