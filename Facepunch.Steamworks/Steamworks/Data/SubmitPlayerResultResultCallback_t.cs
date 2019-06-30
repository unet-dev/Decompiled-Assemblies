using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct SubmitPlayerResultResultCallback_t
	{
		internal Steamworks.Result Result;

		internal ulong UllUniqueGameID;

		internal ulong SteamIDPlayer;

		internal readonly static int StructSize;

		private static Action<SubmitPlayerResultResultCallback_t> actionClient;

		private static Action<SubmitPlayerResultResultCallback_t> actionServer;

		static SubmitPlayerResultResultCallback_t()
		{
			SubmitPlayerResultResultCallback_t.StructSize = Marshal.SizeOf(typeof(SubmitPlayerResultResultCallback_t));
		}

		internal static SubmitPlayerResultResultCallback_t Fill(IntPtr p)
		{
			return (SubmitPlayerResultResultCallback_t)Marshal.PtrToStructure(p, typeof(SubmitPlayerResultResultCallback_t));
		}

		public static async Task<SubmitPlayerResultResultCallback_t?> GetResultAsync(SteamAPICall_t handle)
		{
			SubmitPlayerResultResultCallback_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(SubmitPlayerResultResultCallback_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, SubmitPlayerResultResultCallback_t.StructSize, 5214, ref flag) | flag))
					{
						nullable = new SubmitPlayerResultResultCallback_t?(SubmitPlayerResultResultCallback_t.Fill(intPtr));
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

		public static void Install(Action<SubmitPlayerResultResultCallback_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(SubmitPlayerResultResultCallback_t.OnClient), SubmitPlayerResultResultCallback_t.StructSize, 5214, false);
				SubmitPlayerResultResultCallback_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(SubmitPlayerResultResultCallback_t.OnServer), SubmitPlayerResultResultCallback_t.StructSize, 5214, true);
				SubmitPlayerResultResultCallback_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SubmitPlayerResultResultCallback_t> action = SubmitPlayerResultResultCallback_t.actionClient;
			if (action != null)
			{
				action(SubmitPlayerResultResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<SubmitPlayerResultResultCallback_t> action = SubmitPlayerResultResultCallback_t.actionServer;
			if (action != null)
			{
				action(SubmitPlayerResultResultCallback_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}