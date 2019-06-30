using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct RemoveUGCDependencyResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal Steamworks.Data.PublishedFileId ChildPublishedFileId;

		internal readonly static int StructSize;

		private static Action<RemoveUGCDependencyResult_t> actionClient;

		private static Action<RemoveUGCDependencyResult_t> actionServer;

		static RemoveUGCDependencyResult_t()
		{
			RemoveUGCDependencyResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(RemoveUGCDependencyResult_t) : typeof(RemoveUGCDependencyResult_t.Pack8)));
		}

		internal static RemoveUGCDependencyResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (RemoveUGCDependencyResult_t)Marshal.PtrToStructure(p, typeof(RemoveUGCDependencyResult_t)) : (RemoveUGCDependencyResult_t.Pack8)Marshal.PtrToStructure(p, typeof(RemoveUGCDependencyResult_t.Pack8)));
		}

		public static async Task<RemoveUGCDependencyResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			RemoveUGCDependencyResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(RemoveUGCDependencyResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, RemoveUGCDependencyResult_t.StructSize, 3413, ref flag) | flag))
					{
						nullable = new RemoveUGCDependencyResult_t?(RemoveUGCDependencyResult_t.Fill(intPtr));
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

		public static void Install(Action<RemoveUGCDependencyResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(RemoveUGCDependencyResult_t.OnClient), RemoveUGCDependencyResult_t.StructSize, 3413, false);
				RemoveUGCDependencyResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(RemoveUGCDependencyResult_t.OnServer), RemoveUGCDependencyResult_t.StructSize, 3413, true);
				RemoveUGCDependencyResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoveUGCDependencyResult_t> action = RemoveUGCDependencyResult_t.actionClient;
			if (action != null)
			{
				action(RemoveUGCDependencyResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<RemoveUGCDependencyResult_t> action = RemoveUGCDependencyResult_t.actionServer;
			if (action != null)
			{
				action(RemoveUGCDependencyResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal Steamworks.Data.PublishedFileId ChildPublishedFileId;

			public static implicit operator RemoveUGCDependencyResult_t(RemoveUGCDependencyResult_t.Pack8 d)
			{
				RemoveUGCDependencyResult_t removeUGCDependencyResultT = new RemoveUGCDependencyResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					ChildPublishedFileId = d.ChildPublishedFileId
				};
				return removeUGCDependencyResultT;
			}

			public static implicit operator Pack8(RemoveUGCDependencyResult_t d)
			{
				RemoveUGCDependencyResult_t.Pack8 pack8 = new RemoveUGCDependencyResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					ChildPublishedFileId = d.ChildPublishedFileId
				};
				return pack8;
			}
		}
	}
}