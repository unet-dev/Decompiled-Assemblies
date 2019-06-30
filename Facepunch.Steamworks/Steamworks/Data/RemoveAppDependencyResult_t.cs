using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoveAppDependencyResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<RemoveAppDependencyResult_t> actionClient;

		private static Action<RemoveAppDependencyResult_t> actionServer;

		static RemoveAppDependencyResult_t()
		{
			RemoveAppDependencyResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoveAppDependencyResult_t) : typeof(RemoveAppDependencyResult_t.Pack8)));
		}

		internal static RemoveAppDependencyResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoveAppDependencyResult_t)Marshal.PtrToStructure(p, typeof(RemoveAppDependencyResult_t)) : (RemoveAppDependencyResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoveAppDependencyResult_t.Pack8)));
		}

		public static async Task<RemoveAppDependencyResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoveAppDependencyResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoveAppDependencyResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoveAppDependencyResult_t.StructSize, 3415, ref flag) | flag))
					{
						nullable = new RemoveAppDependencyResult_t?(RemoveAppDependencyResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoveAppDependencyResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoveAppDependencyResult_t.OnClient), RemoveAppDependencyResult_t.StructSize, 3415, false);
				RemoveAppDependencyResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoveAppDependencyResult_t.OnServer), RemoveAppDependencyResult_t.StructSize, 3415, true);
				RemoveAppDependencyResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoveAppDependencyResult_t> action = RemoveAppDependencyResult_t.actionClient;
			if (action != null)
			{
				action(RemoveAppDependencyResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoveAppDependencyResult_t> action = RemoveAppDependencyResult_t.actionServer;
			if (action != null)
			{
				action(RemoveAppDependencyResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal AppId AppID;

			public static implicit operator RemoveAppDependencyResult_t(RemoveAppDependencyResult_t.Pack8 d)
			{
				RemoveAppDependencyResult_t removeAppDependencyResultT = new RemoveAppDependencyResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return removeAppDependencyResultT;
			}

			public static implicit operator Pack8(RemoveAppDependencyResult_t d)
			{
				RemoveAppDependencyResult_t.Pack8 pack8 = new RemoveAppDependencyResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return pack8;
			}
		}
	}
}