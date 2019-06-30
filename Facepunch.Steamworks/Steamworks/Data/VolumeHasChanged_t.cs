using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct VolumeHasChanged_t
	{
		internal float NewVolume;

		internal readonly static int StructSize;

		private static Action<VolumeHasChanged_t> actionClient;

		private static Action<VolumeHasChanged_t> actionServer;

		static VolumeHasChanged_t()
		{
			VolumeHasChanged_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(VolumeHasChanged_t) : typeof(VolumeHasChanged_t.Pack8)));
		}

		internal static VolumeHasChanged_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (VolumeHasChanged_t)Marshal.PtrToStructure(p, typeof(VolumeHasChanged_t)) : (VolumeHasChanged_t.Pack8)Marshal.PtrToStructure(p, typeof(VolumeHasChanged_t.Pack8)));
		}

		public static async Task<VolumeHasChanged_t?> GetResultAsync(SteamAPICall_t handle)
		{
			VolumeHasChanged_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(VolumeHasChanged_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, VolumeHasChanged_t.StructSize, 4002, ref flag) | flag))
					{
						nullable = new VolumeHasChanged_t?(VolumeHasChanged_t.Fill(intPtr));
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

		public static void Install(Action<VolumeHasChanged_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(VolumeHasChanged_t.OnClient), VolumeHasChanged_t.StructSize, 4002, false);
				VolumeHasChanged_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(VolumeHasChanged_t.OnServer), VolumeHasChanged_t.StructSize, 4002, true);
				VolumeHasChanged_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<VolumeHasChanged_t> action = VolumeHasChanged_t.actionClient;
			if (action != null)
			{
				action(VolumeHasChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<VolumeHasChanged_t> action = VolumeHasChanged_t.actionServer;
			if (action != null)
			{
				action(VolumeHasChanged_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal float NewVolume;

			public static implicit operator VolumeHasChanged_t(VolumeHasChanged_t.Pack8 d)
			{
				return new VolumeHasChanged_t()
				{
					NewVolume = d.NewVolume
				};
			}

			public static implicit operator Pack8(VolumeHasChanged_t d)
			{
				return new VolumeHasChanged_t.Pack8()
				{
					NewVolume = d.NewVolume
				};
			}
		}
	}
}