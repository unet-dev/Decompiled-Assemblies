using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct AddAppDependencyResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId AppID;

		internal readonly static int StructSize;

		private static Action<AddAppDependencyResult_t> actionClient;

		private static Action<AddAppDependencyResult_t> actionServer;

		static AddAppDependencyResult_t()
		{
			AddAppDependencyResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(AddAppDependencyResult_t) : typeof(AddAppDependencyResult_t.Pack8)));
		}

		internal static AddAppDependencyResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (AddAppDependencyResult_t)Marshal.PtrToStructure(p, typeof(AddAppDependencyResult_t)) : (AddAppDependencyResult_t.Pack8)Marshal.PtrToStructure(p, typeof(AddAppDependencyResult_t.Pack8)));
		}

		public static async Task<AddAppDependencyResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			AddAppDependencyResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(AddAppDependencyResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, AddAppDependencyResult_t.StructSize, 3414, ref flag) | flag))
					{
						nullable = new AddAppDependencyResult_t?(AddAppDependencyResult_t.Fill(intPtr));
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

		public static void Install(Action<AddAppDependencyResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(AddAppDependencyResult_t.OnClient), AddAppDependencyResult_t.StructSize, 3414, false);
				AddAppDependencyResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(AddAppDependencyResult_t.OnServer), AddAppDependencyResult_t.StructSize, 3414, true);
				AddAppDependencyResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AddAppDependencyResult_t> action = AddAppDependencyResult_t.actionClient;
			if (action != null)
			{
				action(AddAppDependencyResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<AddAppDependencyResult_t> action = AddAppDependencyResult_t.actionServer;
			if (action != null)
			{
				action(AddAppDependencyResult_t.Fill(pvParam));
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

			public static implicit operator AddAppDependencyResult_t(AddAppDependencyResult_t.Pack8 d)
			{
				AddAppDependencyResult_t addAppDependencyResultT = new AddAppDependencyResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					AppID = d.AppID
				};
				return addAppDependencyResultT;
			}

			public static implicit operator Pack8(AddAppDependencyResult_t d)
			{
				AddAppDependencyResult_t.Pack8 pack8 = new AddAppDependencyResult_t.Pack8()
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