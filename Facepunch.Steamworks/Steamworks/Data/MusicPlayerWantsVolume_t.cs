using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MusicPlayerWantsVolume_t
	{
		internal float NewVolume;

		internal readonly static int StructSize;

		private static Action<MusicPlayerWantsVolume_t> actionClient;

		private static Action<MusicPlayerWantsVolume_t> actionServer;

		static MusicPlayerWantsVolume_t()
		{
			MusicPlayerWantsVolume_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MusicPlayerWantsVolume_t) : typeof(MusicPlayerWantsVolume_t.Pack8)));
		}

		internal static MusicPlayerWantsVolume_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MusicPlayerWantsVolume_t)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsVolume_t)) : (MusicPlayerWantsVolume_t.Pack8)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsVolume_t.Pack8)));
		}

		public static async Task<MusicPlayerWantsVolume_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MusicPlayerWantsVolume_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MusicPlayerWantsVolume_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MusicPlayerWantsVolume_t.StructSize, 4011, ref flag) | flag))
					{
						nullable = new MusicPlayerWantsVolume_t?(MusicPlayerWantsVolume_t.Fill(intPtr));
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

		public static void Install(Action<MusicPlayerWantsVolume_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MusicPlayerWantsVolume_t.OnClient), MusicPlayerWantsVolume_t.StructSize, 4011, false);
				MusicPlayerWantsVolume_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MusicPlayerWantsVolume_t.OnServer), MusicPlayerWantsVolume_t.StructSize, 4011, true);
				MusicPlayerWantsVolume_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsVolume_t> action = MusicPlayerWantsVolume_t.actionClient;
			if (action != null)
			{
				action(MusicPlayerWantsVolume_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsVolume_t> action = MusicPlayerWantsVolume_t.actionServer;
			if (action != null)
			{
				action(MusicPlayerWantsVolume_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal float NewVolume;

			public static implicit operator MusicPlayerWantsVolume_t(MusicPlayerWantsVolume_t.Pack8 d)
			{
				return new MusicPlayerWantsVolume_t()
				{
					NewVolume = d.NewVolume
				};
			}

			public static implicit operator Pack8(MusicPlayerWantsVolume_t d)
			{
				return new MusicPlayerWantsVolume_t.Pack8()
				{
					NewVolume = d.NewVolume
				};
			}
		}
	}
}