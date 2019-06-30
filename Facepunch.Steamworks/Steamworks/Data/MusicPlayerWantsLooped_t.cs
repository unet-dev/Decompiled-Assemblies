using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MusicPlayerWantsLooped_t
	{
		internal bool Looped;

		internal readonly static int StructSize;

		private static Action<MusicPlayerWantsLooped_t> actionClient;

		private static Action<MusicPlayerWantsLooped_t> actionServer;

		static MusicPlayerWantsLooped_t()
		{
			MusicPlayerWantsLooped_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MusicPlayerWantsLooped_t) : typeof(MusicPlayerWantsLooped_t.Pack8)));
		}

		internal static MusicPlayerWantsLooped_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MusicPlayerWantsLooped_t)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsLooped_t)) : (MusicPlayerWantsLooped_t.Pack8)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsLooped_t.Pack8)));
		}

		public static async Task<MusicPlayerWantsLooped_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MusicPlayerWantsLooped_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MusicPlayerWantsLooped_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MusicPlayerWantsLooped_t.StructSize, 4110, ref flag) | flag))
					{
						nullable = new MusicPlayerWantsLooped_t?(MusicPlayerWantsLooped_t.Fill(intPtr));
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

		public static void Install(Action<MusicPlayerWantsLooped_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MusicPlayerWantsLooped_t.OnClient), MusicPlayerWantsLooped_t.StructSize, 4110, false);
				MusicPlayerWantsLooped_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MusicPlayerWantsLooped_t.OnServer), MusicPlayerWantsLooped_t.StructSize, 4110, true);
				MusicPlayerWantsLooped_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsLooped_t> action = MusicPlayerWantsLooped_t.actionClient;
			if (action != null)
			{
				action(MusicPlayerWantsLooped_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsLooped_t> action = MusicPlayerWantsLooped_t.actionServer;
			if (action != null)
			{
				action(MusicPlayerWantsLooped_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal bool Looped;

			public static implicit operator MusicPlayerWantsLooped_t(MusicPlayerWantsLooped_t.Pack8 d)
			{
				return new MusicPlayerWantsLooped_t()
				{
					Looped = d.Looped
				};
			}

			public static implicit operator Pack8(MusicPlayerWantsLooped_t d)
			{
				return new MusicPlayerWantsLooped_t.Pack8()
				{
					Looped = d.Looped
				};
			}
		}
	}
}