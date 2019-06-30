using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MusicPlayerWantsShuffled_t
	{
		internal bool Shuffled;

		internal readonly static int StructSize;

		private static Action<MusicPlayerWantsShuffled_t> actionClient;

		private static Action<MusicPlayerWantsShuffled_t> actionServer;

		static MusicPlayerWantsShuffled_t()
		{
			MusicPlayerWantsShuffled_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MusicPlayerWantsShuffled_t) : typeof(MusicPlayerWantsShuffled_t.Pack8)));
		}

		internal static MusicPlayerWantsShuffled_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MusicPlayerWantsShuffled_t)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsShuffled_t)) : (MusicPlayerWantsShuffled_t.Pack8)Marshal.PtrToStructure(p, typeof(MusicPlayerWantsShuffled_t.Pack8)));
		}

		public static async Task<MusicPlayerWantsShuffled_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MusicPlayerWantsShuffled_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MusicPlayerWantsShuffled_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MusicPlayerWantsShuffled_t.StructSize, 4109, ref flag) | flag))
					{
						nullable = new MusicPlayerWantsShuffled_t?(MusicPlayerWantsShuffled_t.Fill(intPtr));
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

		public static void Install(Action<MusicPlayerWantsShuffled_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MusicPlayerWantsShuffled_t.OnClient), MusicPlayerWantsShuffled_t.StructSize, 4109, false);
				MusicPlayerWantsShuffled_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MusicPlayerWantsShuffled_t.OnServer), MusicPlayerWantsShuffled_t.StructSize, 4109, true);
				MusicPlayerWantsShuffled_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsShuffled_t> action = MusicPlayerWantsShuffled_t.actionClient;
			if (action != null)
			{
				action(MusicPlayerWantsShuffled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerWantsShuffled_t> action = MusicPlayerWantsShuffled_t.actionServer;
			if (action != null)
			{
				action(MusicPlayerWantsShuffled_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal bool Shuffled;

			public static implicit operator MusicPlayerWantsShuffled_t(MusicPlayerWantsShuffled_t.Pack8 d)
			{
				return new MusicPlayerWantsShuffled_t()
				{
					Shuffled = d.Shuffled
				};
			}

			public static implicit operator Pack8(MusicPlayerWantsShuffled_t d)
			{
				return new MusicPlayerWantsShuffled_t.Pack8()
				{
					Shuffled = d.Shuffled
				};
			}
		}
	}
}