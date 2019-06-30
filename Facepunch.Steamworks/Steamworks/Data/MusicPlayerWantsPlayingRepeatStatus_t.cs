using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MusicPlayerWantsPlayingRepeatStatus_t
	{
		internal int PlayingRepeatStatus;

		internal readonly static int StructSize;

		private static Action<MusicPlayerWantsPlayingRepeatStatus_t> actionClient;

		private static Action<MusicPlayerWantsPlayingRepeatStatus_t> actionServer;

		static MusicPlayerWantsPlayingRepeatStatus_t()
		{
			MusicPlayerWantsPlayingRepeatStatus_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MusicPlayerWantsPlayingRepeatStatus_t) : typeof(MusicPlayerWantsPlayingRepeatStatus_t.Pack8)));
		}

		internal static MusicPlayerWantsPlayingRepeatStatus_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MusicPlayerWantsPlayingRepeatStatus_t)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsPlayingRepeatStatus_t)) : (MusicPlayerWantsPlayingRepeatStatus_t.Pack8)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsPlayingRepeatStatus_t.Pack8)));
		}

		public static async Task<MusicPlayerWantsPlayingRepeatStatus_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MusicPlayerWantsPlayingRepeatStatus_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MusicPlayerWantsPlayingRepeatStatus_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MusicPlayerWantsPlayingRepeatStatus_t.StructSize, 4114, ref flag) | flag))
					{
						nullable = new MusicPlayerWantsPlayingRepeatStatus_t?(MusicPlayerWantsPlayingRepeatStatus_t.Fill(intPtr));
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

		public static void Install(Action<MusicPlayerWantsPlayingRepeatStatus_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MusicPlayerWantsPlayingRepeatStatus_t.OnClient), MusicPlayerWantsPlayingRepeatStatus_t.StructSize, 4114, false);
				MusicPlayerWantsPlayingRepeatStatus_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MusicPlayerWantsPlayingRepeatStatus_t.OnServer), MusicPlayerWantsPlayingRepeatStatus_t.StructSize, 4114, true);
				MusicPlayerWantsPlayingRepeatStatus_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsPlayingRepeatStatus_t> action = MusicPlayerWantsPlayingRepeatStatus_t.actionClient;
			if (action != null)
			{
				action(MusicPlayerWantsPlayingRepeatStatus_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsPlayingRepeatStatus_t> action = MusicPlayerWantsPlayingRepeatStatus_t.actionServer;
			if (action != null)
			{
				action(MusicPlayerWantsPlayingRepeatStatus_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal int PlayingRepeatStatus;

			public static implicit operator MusicPlayerWantsPlayingRepeatStatus_t(MusicPlayerWantsPlayingRepeatStatus_t.Pack8 d)
			{
				return new MusicPlayerWantsPlayingRepeatStatus_t()
				{
					PlayingRepeatStatus = d.PlayingRepeatStatus
				};
			}

			public static implicit operator Pack8(MusicPlayerWantsPlayingRepeatStatus_t d)
			{
				return new MusicPlayerWantsPlayingRepeatStatus_t.Pack8()
				{
					PlayingRepeatStatus = d.PlayingRepeatStatus
				};
			}
		}
	}
}