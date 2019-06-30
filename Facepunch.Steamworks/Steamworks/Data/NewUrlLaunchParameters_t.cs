using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct NewUrlLaunchParameters_t
	{
		internal readonly static int StructSize;

		private static Action<NewUrlLaunchParameters_t> actionClient;

		private static Action<NewUrlLaunchParameters_t> actionServer;

		static NewUrlLaunchParameters_t()
		{
			NewUrlLaunchParameters_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(NewUrlLaunchParameters_t) : typeof(NewUrlLaunchParameters_t.Pack8)));
		}

		internal static NewUrlLaunchParameters_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (NewUrlLaunchParameters_t)Marshal.PtrToStructure(p, typeof(NewUrlLaunchParameters_t)) : (NewUrlLaunchParameters_t.Pack8)Marshal.PtrToStructure(p, typeof(NewUrlLaunchParameters_t.Pack8)));
		}

		public static async Task<NewUrlLaunchParameters_t?> GetResultAsync(SteamAPICall_t handle)
		{
			NewUrlLaunchParameters_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(NewUrlLaunchParameters_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, NewUrlLaunchParameters_t.StructSize, 1014, ref flag) | flag))
					{
						nullable = new NewUrlLaunchParameters_t?(NewUrlLaunchParameters_t.Fill(intPtr));
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

		public static void Install(Action<NewUrlLaunchParameters_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(NewUrlLaunchParameters_t.OnClient), NewUrlLaunchParameters_t.StructSize, 1014, false);
				NewUrlLaunchParameters_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(NewUrlLaunchParameters_t.OnServer), NewUrlLaunchParameters_t.StructSize, 1014, true);
				NewUrlLaunchParameters_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<NewUrlLaunchParameters_t> action = NewUrlLaunchParameters_t.actionClient;
			if (action != null)
			{
				action(NewUrlLaunchParameters_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<NewUrlLaunchParameters_t> action = NewUrlLaunchParameters_t.actionServer;
			if (action != null)
			{
				action(NewUrlLaunchParameters_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator NewUrlLaunchParameters_t(NewUrlLaunchParameters_t.Pack8 d)
			{
				return new NewUrlLaunchParameters_t();
			}

			public static implicit operator Pack8(NewUrlLaunchParameters_t d)
			{
				return new NewUrlLaunchParameters_t.Pack8();
			}
		}
	}
}