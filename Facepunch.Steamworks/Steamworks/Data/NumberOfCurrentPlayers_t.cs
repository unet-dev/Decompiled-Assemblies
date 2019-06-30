using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct NumberOfCurrentPlayers_t
	{
		internal byte Success;

		internal int CPlayers;

		internal readonly static int StructSize;

		private static Action<NumberOfCurrentPlayers_t> actionClient;

		private static Action<NumberOfCurrentPlayers_t> actionServer;

		static NumberOfCurrentPlayers_t()
		{
			NumberOfCurrentPlayers_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(NumberOfCurrentPlayers_t) : typeof(NumberOfCurrentPlayers_t.Pack8)));
		}

		internal static NumberOfCurrentPlayers_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (NumberOfCurrentPlayers_t)Marshal.PtrToStructure(p, typeof(NumberOfCurrentPlayers_t)) : (NumberOfCurrentPlayers_t.Pack8)Marshal.PtrToStructure(p, typeof(NumberOfCurrentPlayers_t.Pack8)));
		}

		public static async Task<NumberOfCurrentPlayers_t?> GetResultAsync(SteamAPICall_t handle)
		{
			NumberOfCurrentPlayers_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(NumberOfCurrentPlayers_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, NumberOfCurrentPlayers_t.StructSize, 1107, ref flag) | flag))
					{
						nullable = new NumberOfCurrentPlayers_t?(NumberOfCurrentPlayers_t.Fill(intPtr));
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

		public static void Install(Action<NumberOfCurrentPlayers_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(NumberOfCurrentPlayers_t.OnClient), NumberOfCurrentPlayers_t.StructSize, 1107, false);
				NumberOfCurrentPlayers_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(NumberOfCurrentPlayers_t.OnServer), NumberOfCurrentPlayers_t.StructSize, 1107, true);
				NumberOfCurrentPlayers_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<NumberOfCurrentPlayers_t> action = NumberOfCurrentPlayers_t.actionClient;
			if (action != null)
			{
				action(NumberOfCurrentPlayers_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<NumberOfCurrentPlayers_t> action = NumberOfCurrentPlayers_t.actionServer;
			if (action != null)
			{
				action(NumberOfCurrentPlayers_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal byte Success;

			internal int CPlayers;

			public static implicit operator NumberOfCurrentPlayers_t(NumberOfCurrentPlayers_t.Pack8 d)
			{
				NumberOfCurrentPlayers_t numberOfCurrentPlayersT = new NumberOfCurrentPlayers_t()
				{
					Success = d.Success,
					CPlayers = d.CPlayers
				};
				return numberOfCurrentPlayersT;
			}

			public static implicit operator Pack8(NumberOfCurrentPlayers_t d)
			{
				NumberOfCurrentPlayers_t.Pack8 pack8 = new NumberOfCurrentPlayers_t.Pack8()
				{
					Success = d.Success,
					CPlayers = d.CPlayers
				};
				return pack8;
			}
		}
	}
}