using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct CheckFileSignature_t
	{
		internal Steamworks.CheckFileSignature CheckFileSignature;

		internal readonly static int StructSize;

		private static Action<CheckFileSignature_t> actionClient;

		private static Action<CheckFileSignature_t> actionServer;

		static CheckFileSignature_t()
		{
			CheckFileSignature_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(CheckFileSignature_t) : typeof(CheckFileSignature_t.Pack8)));
		}

		internal static CheckFileSignature_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (CheckFileSignature_t)Marshal.PtrToStructure(p, typeof(CheckFileSignature_t)) : (CheckFileSignature_t.Pack8)Marshal.PtrToStructure(p, typeof(CheckFileSignature_t.Pack8)));
		}

		public static async Task<CheckFileSignature_t?> GetResultAsync(SteamAPICall_t handle)
		{
			CheckFileSignature_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(CheckFileSignature_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, CheckFileSignature_t.StructSize, 705, ref flag) | flag))
					{
						nullable = new CheckFileSignature_t?(CheckFileSignature_t.Fill(intPtr));
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

		public static void Install(Action<CheckFileSignature_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(CheckFileSignature_t.OnClient), CheckFileSignature_t.StructSize, 705, false);
				CheckFileSignature_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(CheckFileSignature_t.OnServer), CheckFileSignature_t.StructSize, 705, true);
				CheckFileSignature_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<CheckFileSignature_t> action = CheckFileSignature_t.actionClient;
			if (action != null)
			{
				action(CheckFileSignature_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<CheckFileSignature_t> action = CheckFileSignature_t.actionServer;
			if (action != null)
			{
				action(CheckFileSignature_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.CheckFileSignature CheckFileSignature;

			public static implicit operator CheckFileSignature_t(CheckFileSignature_t.Pack8 d)
			{
				return new CheckFileSignature_t()
				{
					CheckFileSignature = d.CheckFileSignature
				};
			}

			public static implicit operator Pack8(CheckFileSignature_t d)
			{
				return new CheckFileSignature_t.Pack8()
				{
					CheckFileSignature = d.CheckFileSignature
				};
			}
		}
	}
}