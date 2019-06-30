using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RegisterActivationCodeResponse_t
	{
		internal RegisterActivationCodeResult Result;

		internal uint PackageRegistered;

		internal readonly static int StructSize;

		private static Action<RegisterActivationCodeResponse_t> actionClient;

		private static Action<RegisterActivationCodeResponse_t> actionServer;

		static RegisterActivationCodeResponse_t()
		{
			RegisterActivationCodeResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RegisterActivationCodeResponse_t) : typeof(RegisterActivationCodeResponse_t.Pack8)));
		}

		internal static RegisterActivationCodeResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RegisterActivationCodeResponse_t)Marshal.PtrToStructure(p, typeof(RegisterActivationCodeResponse_t)) : (RegisterActivationCodeResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(RegisterActivationCodeResponse_t.Pack8)));
		}

		public static async Task<RegisterActivationCodeResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RegisterActivationCodeResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RegisterActivationCodeResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RegisterActivationCodeResponse_t.StructSize, 1008, ref flag) | flag))
					{
						nullable = new RegisterActivationCodeResponse_t?(RegisterActivationCodeResponse_t.Fill(intPtr));
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

		public static void Install(Action<RegisterActivationCodeResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RegisterActivationCodeResponse_t.OnClient), RegisterActivationCodeResponse_t.StructSize, 1008, false);
				RegisterActivationCodeResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RegisterActivationCodeResponse_t.OnServer), RegisterActivationCodeResponse_t.StructSize, 1008, true);
				RegisterActivationCodeResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RegisterActivationCodeResponse_t> action = RegisterActivationCodeResponse_t.actionClient;
			if (action != null)
			{
				action(RegisterActivationCodeResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RegisterActivationCodeResponse_t> action = RegisterActivationCodeResponse_t.actionServer;
			if (action != null)
			{
				action(RegisterActivationCodeResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal RegisterActivationCodeResult Result;

			internal uint PackageRegistered;

			public static implicit operator RegisterActivationCodeResponse_t(RegisterActivationCodeResponse_t.Pack8 d)
			{
				RegisterActivationCodeResponse_t registerActivationCodeResponseT = new RegisterActivationCodeResponse_t()
				{
					Result = d.Result,
					PackageRegistered = d.PackageRegistered
				};
				return registerActivationCodeResponseT;
			}

			public static implicit operator Pack8(RegisterActivationCodeResponse_t d)
			{
				RegisterActivationCodeResponse_t.Pack8 pack8 = new RegisterActivationCodeResponse_t.Pack8()
				{
					Result = d.Result,
					PackageRegistered = d.PackageRegistered
				};
				return pack8;
			}
		}
	}
}