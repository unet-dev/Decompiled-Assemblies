using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct MusicPlayerSelectsQueueEntry_t
	{
		internal int NID;

		internal readonly static int StructSize;

		private static Action<MusicPlayerSelectsQueueEntry_t> actionClient;

		private static Action<MusicPlayerSelectsQueueEntry_t> actionServer;

		static MusicPlayerSelectsQueueEntry_t()
		{
			MusicPlayerSelectsQueueEntry_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(MusicPlayerSelectsQueueEntry_t) : typeof(MusicPlayerSelectsQueueEntry_t.Pack8)));
		}

		internal static MusicPlayerSelectsQueueEntry_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (MusicPlayerSelectsQueueEntry_t)Marshal.PtrToStructure(p, typeof(MusicPlayerSelectsQueueEntry_t)) : (MusicPlayerSelectsQueueEntry_t.Pack8)Marshal.PtrToStructure(p, typeof(MusicPlayerSelectsQueueEntry_t.Pack8)));
		}

		public static async Task<MusicPlayerSelectsQueueEntry_t?> GetResultAsync(SteamAPICall_t handle)
		{
			MusicPlayerSelectsQueueEntry_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(MusicPlayerSelectsQueueEntry_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, MusicPlayerSelectsQueueEntry_t.StructSize, 4012, ref flag) | flag))
					{
						nullable = new MusicPlayerSelectsQueueEntry_t?(MusicPlayerSelectsQueueEntry_t.Fill(intPtr));
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

		public static void Install(Action<MusicPlayerSelectsQueueEntry_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(MusicPlayerSelectsQueueEntry_t.OnClient), MusicPlayerSelectsQueueEntry_t.StructSize, 4012, false);
				MusicPlayerSelectsQueueEntry_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(MusicPlayerSelectsQueueEntry_t.OnServer), MusicPlayerSelectsQueueEntry_t.StructSize, 4012, true);
				MusicPlayerSelectsQueueEntry_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerSelectsQueueEntry_t> action = MusicPlayerSelectsQueueEntry_t.actionClient;
			if (action != null)
			{
				action(MusicPlayerSelectsQueueEntry_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<MusicPlayerSelectsQueueEntry_t> action = MusicPlayerSelectsQueueEntry_t.actionServer;
			if (action != null)
			{
				action(MusicPlayerSelectsQueueEntry_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal int NID;

			public static implicit operator MusicPlayerSelectsQueueEntry_t(MusicPlayerSelectsQueueEntry_t.Pack8 d)
			{
				return new MusicPlayerSelectsQueueEntry_t()
				{
					NID = d.NID
				};
			}

			public static implicit operator Pack8(MusicPlayerSelectsQueueEntry_t d)
			{
				return new MusicPlayerSelectsQueueEntry_t.Pack8()
				{
					NID = d.NID
				};
			}
		}
	}
}