using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct AvatarImageLoaded_t
	{
		internal ulong SteamID;

		internal int Image;

		internal int Wide;

		internal int Tall;

		internal readonly static int StructSize;

		private static Action<AvatarImageLoaded_t> actionClient;

		private static Action<AvatarImageLoaded_t> actionServer;

		static AvatarImageLoaded_t()
		{
			AvatarImageLoaded_t.StructSize = Marshal.SizeOf(typeof(AvatarImageLoaded_t));
		}

		internal static AvatarImageLoaded_t Fill(IntPtr p)
		{
			return (AvatarImageLoaded_t)Marshal.PtrToStructure(p, typeof(AvatarImageLoaded_t));
		}

		public static async Task<AvatarImageLoaded_t?> GetResultAsync(SteamAPICall_t handle)
		{
			AvatarImageLoaded_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(AvatarImageLoaded_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, AvatarImageLoaded_t.StructSize, 334, ref flag) | flag))
					{
						nullable = new AvatarImageLoaded_t?(AvatarImageLoaded_t.Fill(intPtr));
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

		public static void Install(Action<AvatarImageLoaded_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(AvatarImageLoaded_t.OnClient), AvatarImageLoaded_t.StructSize, 334, false);
				AvatarImageLoaded_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(AvatarImageLoaded_t.OnServer), AvatarImageLoaded_t.StructSize, 334, true);
				AvatarImageLoaded_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AvatarImageLoaded_t> action = AvatarImageLoaded_t.actionClient;
			if (action != null)
			{
				action(AvatarImageLoaded_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AvatarImageLoaded_t> action = AvatarImageLoaded_t.actionServer;
			if (action != null)
			{
				action(AvatarImageLoaded_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}