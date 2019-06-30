using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct EndGameResultCallback_t
	{
		internal Steamworks.Result Result;

		internal ulong UllUniqueGameID;

		internal readonly static int StructSize;

		private static Action<EndGameResultCallback_t> actionClient;

		private static Action<EndGameResultCallback_t> actionServer;

		static EndGameResultCallback_t()
		{
			EndGameResultCallback_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(EndGameResultCallback_t) : typeof(EndGameResultCallback_t.Pack8)));
		}

		internal static EndGameResultCallback_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (EndGameResultCallback_t)Marshal.PtrToStructure(p, typeof(EndGameResultCallback_t)) : (EndGameResultCallback_t.Pack8)Marshal.PtrToStructure(p, typeof(EndGameResultCallback_t.Pack8)));
		}

		public static async Task<EndGameResultCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			EndGameResultCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(EndGameResultCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, EndGameResultCallback_t.StructSize, 5215, ref flag) | flag))
					{
						nullable = new EndGameResultCallback_t?(EndGameResultCallback_t.Fill(intPtr));
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

		public static void Install(Action<EndGameResultCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(EndGameResultCallback_t.OnClient), EndGameResultCallback_t.StructSize, 5215, false);
				EndGameResultCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(EndGameResultCallback_t.OnServer), EndGameResultCallback_t.StructSize, 5215, true);
				EndGameResultCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<EndGameResultCallback_t> action = EndGameResultCallback_t.actionClient;
			if (action != null)
			{
				action(EndGameResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<EndGameResultCallback_t> action = EndGameResultCallback_t.actionServer;
			if (action != null)
			{
				action(EndGameResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong UllUniqueGameID;

			public static implicit operator EndGameResultCallback_t(EndGameResultCallback_t.Pack8 d)
			{
				EndGameResultCallback_t endGameResultCallbackT = new EndGameResultCallback_t()
				{
					Result = d.Result,
					UllUniqueGameID = d.UllUniqueGameID
				};
				return endGameResultCallbackT;
			}

			public static implicit operator Pack8(EndGameResultCallback_t d)
			{
				EndGameResultCallback_t.Pack8 pack8 = new EndGameResultCallback_t.Pack8()
				{
					Result = d.Result,
					UllUniqueGameID = d.UllUniqueGameID
				};
				return pack8;
			}
		}
	}
}