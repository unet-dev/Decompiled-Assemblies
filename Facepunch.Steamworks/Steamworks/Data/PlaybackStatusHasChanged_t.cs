using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct PlaybackStatusHasChanged_t
	{
		internal readonly static int StructSize;

		private static Action<PlaybackStatusHasChanged_t> actionClient;

		private static Action<PlaybackStatusHasChanged_t> actionServer;

		static PlaybackStatusHasChanged_t()
		{
			PlaybackStatusHasChanged_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(PlaybackStatusHasChanged_t) : typeof(PlaybackStatusHasChanged_t.Pack8)));
		}

		internal static PlaybackStatusHasChanged_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (PlaybackStatusHasChanged_t)Marshal.PtrToStructure(p, typeof(PlaybackStatusHasChanged_t)) : (PlaybackStatusHasChanged_t.Pack8)Marshal.PtrToStructure(p, typeof(PlaybackStatusHasChanged_t.Pack8)));
		}

		public static async Task<PlaybackStatusHasChanged_t?> GetResultAsync(SteamAPICall_t handle)
		{
			PlaybackStatusHasChanged_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(PlaybackStatusHasChanged_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, PlaybackStatusHasChanged_t.StructSize, 4001, ref flag) | flag))
					{
						nullable = new PlaybackStatusHasChanged_t?(PlaybackStatusHasChanged_t.Fill(intPtr));
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

		public static void Install(Action<PlaybackStatusHasChanged_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(PlaybackStatusHasChanged_t.OnClient), PlaybackStatusHasChanged_t.StructSize, 4001, false);
				PlaybackStatusHasChanged_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(PlaybackStatusHasChanged_t.OnServer), PlaybackStatusHasChanged_t.StructSize, 4001, true);
				PlaybackStatusHasChanged_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<PlaybackStatusHasChanged_t> action = PlaybackStatusHasChanged_t.actionClient;
			if (action != null)
			{
				action(PlaybackStatusHasChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<PlaybackStatusHasChanged_t> action = PlaybackStatusHasChanged_t.actionServer;
			if (action != null)
			{
				action(PlaybackStatusHasChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			public static implicit operator PlaybackStatusHasChanged_t(PlaybackStatusHasChanged_t.Pack8 d)
			{
				return new PlaybackStatusHasChanged_t();
			}

			public static implicit operator Pack8(PlaybackStatusHasChanged_t d)
			{
				return new PlaybackStatusHasChanged_t.Pack8();
			}
		}
	}
}