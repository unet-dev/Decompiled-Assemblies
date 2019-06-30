using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SteamAPICallCompleted_t
	{
		internal ulong AsyncCall;

		internal int Callback;

		internal uint ParamCount;

		internal readonly static int StructSize;

		private static Action<SteamAPICallCompleted_t> actionClient;

		private static Action<SteamAPICallCompleted_t> actionServer;

		static SteamAPICallCompleted_t()
		{
			SteamAPICallCompleted_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SteamAPICallCompleted_t) : typeof(SteamAPICallCompleted_t.Pack8)));
		}

		internal static SteamAPICallCompleted_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SteamAPICallCompleted_t)Marshal.PtrToStructure(p, typeof(SteamAPICallCompleted_t)) : (SteamAPICallCompleted_t.Pack8)Marshal.PtrToStructure(p, typeof(SteamAPICallCompleted_t.Pack8)));
		}

		public static async Task<SteamAPICallCompleted_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SteamAPICallCompleted_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SteamAPICallCompleted_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SteamAPICallCompleted_t.StructSize, 703, ref flag) | flag))
					{
						nullable = new SteamAPICallCompleted_t?(SteamAPICallCompleted_t.Fill(intPtr));
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

		public static void Install(Action<SteamAPICallCompleted_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Steamworks.Callback.Run(SteamAPICallCompleted_t.OnClient), SteamAPICallCompleted_t.StructSize, 703, false);
				SteamAPICallCompleted_t.actionClient = action;
			}
			else
			{
				Event.Register(new Steamworks.Callback.Run(SteamAPICallCompleted_t.OnServer), SteamAPICallCompleted_t.StructSize, 703, true);
				SteamAPICallCompleted_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamAPICallCompleted_t> action = SteamAPICallCompleted_t.actionClient;
			if (action != null)
			{
				action(SteamAPICallCompleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SteamAPICallCompleted_t> action = SteamAPICallCompleted_t.actionServer;
			if (action != null)
			{
				action(SteamAPICallCompleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal ulong AsyncCall;

			internal int Callback;

			internal uint ParamCount;

			public static implicit operator SteamAPICallCompleted_t(SteamAPICallCompleted_t.Pack8 d)
			{
				SteamAPICallCompleted_t steamAPICallCompletedT = new SteamAPICallCompleted_t()
				{
					AsyncCall = d.AsyncCall,
					Callback = d.Callback,
					ParamCount = d.ParamCount
				};
				return steamAPICallCompletedT;
			}

			public static implicit operator Pack8(SteamAPICallCompleted_t d)
			{
				SteamAPICallCompleted_t.Pack8 pack8 = new SteamAPICallCompleted_t.Pack8()
				{
					AsyncCall = d.AsyncCall,
					Callback = d.Callback,
					ParamCount = d.ParamCount
				};
				return pack8;
			}
		}
	}
}