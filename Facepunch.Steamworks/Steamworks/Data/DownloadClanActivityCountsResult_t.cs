using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct DownloadClanActivityCountsResult_t
	{
		internal bool Success;

		internal readonly static int StructSize;

		private static Action<DownloadClanActivityCountsResult_t> actionClient;

		private static Action<DownloadClanActivityCountsResult_t> actionServer;

		static DownloadClanActivityCountsResult_t()
		{
			DownloadClanActivityCountsResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(DownloadClanActivityCountsResult_t) : typeof(DownloadClanActivityCountsResult_t.Pack8)));
		}

		internal static DownloadClanActivityCountsResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (DownloadClanActivityCountsResult_t)Marshal.PtrToStructure(p, typeof(DownloadClanActivityCountsResult_t)) : (DownloadClanActivityCountsResult_t.Pack8)Marshal.PtrToStructure(p, typeof(DownloadClanActivityCountsResult_t.Pack8)));
		}

		public static async Task<DownloadClanActivityCountsResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			DownloadClanActivityCountsResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(DownloadClanActivityCountsResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, DownloadClanActivityCountsResult_t.StructSize, 341, ref flag) | flag))
					{
						nullable = new DownloadClanActivityCountsResult_t?(DownloadClanActivityCountsResult_t.Fill(intPtr));
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

		public static void Install(Action<DownloadClanActivityCountsResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(DownloadClanActivityCountsResult_t.OnClient), DownloadClanActivityCountsResult_t.StructSize, 341, false);
				DownloadClanActivityCountsResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(DownloadClanActivityCountsResult_t.OnServer), DownloadClanActivityCountsResult_t.StructSize, 341, true);
				DownloadClanActivityCountsResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DownloadClanActivityCountsResult_t> action = DownloadClanActivityCountsResult_t.actionClient;
			if (action != null)
			{
				action(DownloadClanActivityCountsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<DownloadClanActivityCountsResult_t> action = DownloadClanActivityCountsResult_t.actionServer;
			if (action != null)
			{
				action(DownloadClanActivityCountsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal bool Success;

			public static implicit operator DownloadClanActivityCountsResult_t(DownloadClanActivityCountsResult_t.Pack8 d)
			{
				return new DownloadClanActivityCountsResult_t()
				{
					Success = d.Success
				};
			}

			public static implicit operator Pack8(DownloadClanActivityCountsResult_t d)
			{
				return new DownloadClanActivityCountsResult_t.Pack8()
				{
					Success = d.Success
				};
			}
		}
	}
}