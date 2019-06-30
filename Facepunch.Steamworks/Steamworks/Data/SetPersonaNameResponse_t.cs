using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SetPersonaNameResponse_t
	{
		internal bool Success;

		internal bool LocalSuccess;

		internal Steamworks.Result Result;

		internal readonly static int StructSize;

		private static Action<SetPersonaNameResponse_t> actionClient;

		private static Action<SetPersonaNameResponse_t> actionServer;

		static SetPersonaNameResponse_t()
		{
			SetPersonaNameResponse_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(SetPersonaNameResponse_t) : typeof(SetPersonaNameResponse_t.Pack8)));
		}

		internal static SetPersonaNameResponse_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (SetPersonaNameResponse_t)Marshal.PtrToStructure(p, typeof(SetPersonaNameResponse_t)) : (SetPersonaNameResponse_t.Pack8)Marshal.PtrToStructure(p, typeof(SetPersonaNameResponse_t.Pack8)));
		}

		public static async Task<SetPersonaNameResponse_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SetPersonaNameResponse_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SetPersonaNameResponse_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SetPersonaNameResponse_t.StructSize, 347, ref flag) | flag))
					{
						nullable = new SetPersonaNameResponse_t?(SetPersonaNameResponse_t.Fill(intPtr));
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

		public static void Install(Action<SetPersonaNameResponse_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SetPersonaNameResponse_t.OnClient), SetPersonaNameResponse_t.StructSize, 347, false);
				SetPersonaNameResponse_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SetPersonaNameResponse_t.OnServer), SetPersonaNameResponse_t.StructSize, 347, true);
				SetPersonaNameResponse_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SetPersonaNameResponse_t> action = SetPersonaNameResponse_t.actionClient;
			if (action != null)
			{
				action(SetPersonaNameResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SetPersonaNameResponse_t> action = SetPersonaNameResponse_t.actionServer;
			if (action != null)
			{
				action(SetPersonaNameResponse_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal bool Success;

			internal bool LocalSuccess;

			internal Steamworks.Result Result;

			public static implicit operator SetPersonaNameResponse_t(SetPersonaNameResponse_t.Pack8 d)
			{
				SetPersonaNameResponse_t setPersonaNameResponseT = new SetPersonaNameResponse_t()
				{
					Success = d.Success,
					LocalSuccess = d.LocalSuccess,
					Result = d.Result
				};
				return setPersonaNameResponseT;
			}

			public static implicit operator Pack8(SetPersonaNameResponse_t d)
			{
				SetPersonaNameResponse_t.Pack8 pack8 = new SetPersonaNameResponse_t.Pack8()
				{
					Success = d.Success,
					LocalSuccess = d.LocalSuccess,
					Result = d.Result
				};
				return pack8;
			}
		}
	}
}