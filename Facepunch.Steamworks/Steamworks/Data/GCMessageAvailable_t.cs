using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GCMessageAvailable_t
	{
		internal uint MessageSize;

		internal readonly static int StructSize;

		private static Action<GCMessageAvailable_t> actionClient;

		private static Action<GCMessageAvailable_t> actionServer;

		static GCMessageAvailable_t()
		{
			GCMessageAvailable_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GCMessageAvailable_t) : typeof(GCMessageAvailable_t.Pack8)));
		}

		internal static GCMessageAvailable_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GCMessageAvailable_t)Marshal.PtrToStructure(p, typeof(GCMessageAvailable_t)) : (GCMessageAvailable_t.Pack8)Marshal.PtrToStructure(p, typeof(GCMessageAvailable_t.Pack8)));
		}

		public static async Task<GCMessageAvailable_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GCMessageAvailable_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GCMessageAvailable_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GCMessageAvailable_t.StructSize, 1701, ref flag) | flag))
					{
						nullable = new GCMessageAvailable_t?(GCMessageAvailable_t.Fill(intPtr));
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

		public static void Install(Action<GCMessageAvailable_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GCMessageAvailable_t.OnClient), GCMessageAvailable_t.StructSize, 1701, false);
				GCMessageAvailable_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GCMessageAvailable_t.OnServer), GCMessageAvailable_t.StructSize, 1701, true);
				GCMessageAvailable_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GCMessageAvailable_t> action = GCMessageAvailable_t.actionClient;
			if (action != null)
			{
				action(GCMessageAvailable_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GCMessageAvailable_t> action = GCMessageAvailable_t.actionServer;
			if (action != null)
			{
				action(GCMessageAvailable_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint MessageSize;

			public static implicit operator GCMessageAvailable_t(GCMessageAvailable_t.Pack8 d)
			{
				return new GCMessageAvailable_t()
				{
					MessageSize = d.MessageSize
				};
			}

			public static implicit operator Pack8(GCMessageAvailable_t d)
			{
				return new GCMessageAvailable_t.Pack8()
				{
					MessageSize = d.MessageSize
				};
			}
		}
	}
}