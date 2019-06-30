using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct IPCFailure_t
	{
		internal byte FailureType;

		internal readonly static int StructSize;

		private static Action<IPCFailure_t> actionClient;

		private static Action<IPCFailure_t> actionServer;

		static IPCFailure_t()
		{
			IPCFailure_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(IPCFailure_t) : typeof(IPCFailure_t.Pack8)));
		}

		internal static IPCFailure_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (IPCFailure_t)Marshal.PtrToStructure(p, typeof(IPCFailure_t)) : (IPCFailure_t.Pack8)Marshal.PtrToStructure(p, typeof(IPCFailure_t.Pack8)));
		}

		public static async Task<IPCFailure_t?> GetResultAsync(SteamAPICall_t handle)
		{
			IPCFailure_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(IPCFailure_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, IPCFailure_t.StructSize, 117, ref flag) | flag))
					{
						nullable = new IPCFailure_t?(IPCFailure_t.Fill(intPtr));
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

		public static void Install(Action<IPCFailure_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(IPCFailure_t.OnClient), IPCFailure_t.StructSize, 117, false);
				IPCFailure_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(IPCFailure_t.OnServer), IPCFailure_t.StructSize, 117, true);
				IPCFailure_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<IPCFailure_t> action = IPCFailure_t.actionClient;
			if (action != null)
			{
				action(IPCFailure_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<IPCFailure_t> action = IPCFailure_t.actionServer;
			if (action != null)
			{
				action(IPCFailure_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal byte FailureType;

			public static implicit operator IPCFailure_t(IPCFailure_t.Pack8 d)
			{
				return new IPCFailure_t()
				{
					FailureType = d.FailureType
				};
			}

			public static implicit operator Pack8(IPCFailure_t d)
			{
				return new IPCFailure_t.Pack8()
				{
					FailureType = d.FailureType
				};
			}
		}
	}
}