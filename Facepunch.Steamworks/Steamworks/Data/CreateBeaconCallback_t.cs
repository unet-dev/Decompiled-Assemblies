using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct CreateBeaconCallback_t
	{
		internal Steamworks.Result Result;

		internal ulong BeaconID;

		internal readonly static int StructSize;

		private static Action<CreateBeaconCallback_t> actionClient;

		private static Action<CreateBeaconCallback_t> actionServer;

		static CreateBeaconCallback_t()
		{
			CreateBeaconCallback_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(CreateBeaconCallback_t) : typeof(CreateBeaconCallback_t.Pack8)));
		}

		internal static CreateBeaconCallback_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (CreateBeaconCallback_t)Marshal.PtrToStructure(p, typeof(CreateBeaconCallback_t)) : (CreateBeaconCallback_t.Pack8)Marshal.PtrToStructure(p, typeof(CreateBeaconCallback_t.Pack8)));
		}

		public static async Task<CreateBeaconCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			CreateBeaconCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(CreateBeaconCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, CreateBeaconCallback_t.StructSize, 5302, ref flag) | flag))
					{
						nullable = new CreateBeaconCallback_t?(CreateBeaconCallback_t.Fill(intPtr));
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

		public static void Install(Action<CreateBeaconCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(CreateBeaconCallback_t.OnClient), CreateBeaconCallback_t.StructSize, 5302, false);
				CreateBeaconCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(CreateBeaconCallback_t.OnServer), CreateBeaconCallback_t.StructSize, 5302, true);
				CreateBeaconCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<CreateBeaconCallback_t> action = CreateBeaconCallback_t.actionClient;
			if (action != null)
			{
				action(CreateBeaconCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<CreateBeaconCallback_t> action = CreateBeaconCallback_t.actionServer;
			if (action != null)
			{
				action(CreateBeaconCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong BeaconID;

			public static implicit operator CreateBeaconCallback_t(CreateBeaconCallback_t.Pack8 d)
			{
				CreateBeaconCallback_t createBeaconCallbackT = new CreateBeaconCallback_t()
				{
					Result = d.Result,
					BeaconID = d.BeaconID
				};
				return createBeaconCallbackT;
			}

			public static implicit operator Pack8(CreateBeaconCallback_t d)
			{
				CreateBeaconCallback_t.Pack8 pack8 = new CreateBeaconCallback_t.Pack8()
				{
					Result = d.Result,
					BeaconID = d.BeaconID
				};
				return pack8;
			}
		}
	}
}