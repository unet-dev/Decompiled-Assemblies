using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MusicPlayerSelectsPlaylistEntry_t
	{
		internal int NID;

		internal readonly static int StructSize;

		private static Action<MusicPlayerSelectsPlaylistEntry_t> actionClient;

		private static Action<MusicPlayerSelectsPlaylistEntry_t> actionServer;

		static MusicPlayerSelectsPlaylistEntry_t()
		{
			MusicPlayerSelectsPlaylistEntry_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MusicPlayerSelectsPlaylistEntry_t) : typeof(MusicPlayerSelectsPlaylistEntry_t.Pack8)));
		}

		internal static MusicPlayerSelectsPlaylistEntry_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MusicPlayerSelectsPlaylistEntry_t)Marshal.PtrToStructure(p, typeof(MusicPlayerSelectsPlaylistEntry_t)) : (MusicPlayerSelectsPlaylistEntry_t.Pack8)Marshal.PtrToStructure(p, typeof(MusicPlayerSelectsPlaylistEntry_t.Pack8)));
		}

		public static async Task<MusicPlayerSelectsPlaylistEntry_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MusicPlayerSelectsPlaylistEntry_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MusicPlayerSelectsPlaylistEntry_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MusicPlayerSelectsPlaylistEntry_t.StructSize, 4013, ref flag) | flag))
					{
						nullable = new MusicPlayerSelectsPlaylistEntry_t?(MusicPlayerSelectsPlaylistEntry_t.Fill(intPtr));
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

		public static void Install(Action<MusicPlayerSelectsPlaylistEntry_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MusicPlayerSelectsPlaylistEntry_t.OnClient), MusicPlayerSelectsPlaylistEntry_t.StructSize, 4013, false);
				MusicPlayerSelectsPlaylistEntry_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MusicPlayerSelectsPlaylistEntry_t.OnServer), MusicPlayerSelectsPlaylistEntry_t.StructSize, 4013, true);
				MusicPlayerSelectsPlaylistEntry_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerSelectsPlaylistEntry_t> action = MusicPlayerSelectsPlaylistEntry_t.actionClient;
			if (action != null)
			{
				action(MusicPlayerSelectsPlaylistEntry_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerSelectsPlaylistEntry_t> action = MusicPlayerSelectsPlaylistEntry_t.actionServer;
			if (action != null)
			{
				action(MusicPlayerSelectsPlaylistEntry_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal int NID;

			public static implicit operator MusicPlayerSelectsPlaylistEntry_t(MusicPlayerSelectsPlaylistEntry_t.Pack8 d)
			{
				return new MusicPlayerSelectsPlaylistEntry_t()
				{
					NID = d.NID
				};
			}

			public static implicit operator Pack8(MusicPlayerSelectsPlaylistEntry_t d)
			{
				return new MusicPlayerSelectsPlaylistEntry_t.Pack8()
				{
					NID = d.NID
				};
			}
		}
	}
}