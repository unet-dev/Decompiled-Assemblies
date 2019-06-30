using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct AssociateWithClanResult_t
	{
		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<AssociateWithClanResult_t> actionClient;

		private static Action<AssociateWithClanResult_t> actionServer;

		static AssociateWithClanResult_t()
		{
			AssociateWithClanResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(AssociateWithClanResult_t) : typeof(AssociateWithClanResult_t.Pack8)));
		}

		internal static AssociateWithClanResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (AssociateWithClanResult_t)Marshal.PtrToStructure(p, typeof(AssociateWithClanResult_t)) : (AssociateWithClanResult_t.Pack8)Marshal.PtrToStructure(p, typeof(AssociateWithClanResult_t.Pack8)));
		}

		public static async Task<AssociateWithClanResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			AssociateWithClanResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(AssociateWithClanResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, AssociateWithClanResult_t.StructSize, 210, ref flag) | flag))
					{
						nullable = new AssociateWithClanResult_t?(AssociateWithClanResult_t.Fill(intPtr));
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

		public static void Install(Action<AssociateWithClanResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(AssociateWithClanResult_t.OnClient), AssociateWithClanResult_t.StructSize, 210, false);
				AssociateWithClanResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(AssociateWithClanResult_t.OnServer), AssociateWithClanResult_t.StructSize, 210, true);
				AssociateWithClanResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AssociateWithClanResult_t> action = AssociateWithClanResult_t.actionClient;
			if (action != null)
			{
				action(AssociateWithClanResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AssociateWithClanResult_t> action = AssociateWithClanResult_t.actionServer;
			if (action != null)
			{
				action(AssociateWithClanResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			public static implicit operator AssociateWithClanResult_t(AssociateWithClanResult_t.Pack8 d)
			{
				return new AssociateWithClanResult_t()
				{
					Result = d.Result
				};
			}

			public static implicit operator Pack8(AssociateWithClanResult_t d)
			{
				return new AssociateWithClanResult_t.Pack8()
				{
					Result = d.Result
				};
			}
		}
	}
}