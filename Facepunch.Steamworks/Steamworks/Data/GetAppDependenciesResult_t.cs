using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct GetAppDependenciesResult_t
	{
		internal Steamworks.Result Result;

		internal Steamworks.Data.PublishedFileId PublishedFileId;

		internal AppId[] GAppIDs;

		internal uint NumAppDependencies;

		internal uint TotalNumAppDependencies;

		internal readonly static int StructSize;

		private static Action<GetAppDependenciesResult_t> actionClient;

		private static Action<GetAppDependenciesResult_t> actionServer;

		static GetAppDependenciesResult_t()
		{
			GetAppDependenciesResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(GetAppDependenciesResult_t) : typeof(GetAppDependenciesResult_t.Pack8)));
		}

		internal static GetAppDependenciesResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (GetAppDependenciesResult_t)Marshal.PtrToStructure(p, typeof(GetAppDependenciesResult_t)) : (GetAppDependenciesResult_t.Pack8)Marshal.PtrToStructure(p, typeof(GetAppDependenciesResult_t.Pack8)));
		}

		public static async Task<GetAppDependenciesResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			GetAppDependenciesResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(GetAppDependenciesResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, GetAppDependenciesResult_t.StructSize, 3416, ref flag) | flag))
					{
						nullable = new GetAppDependenciesResult_t?(GetAppDependenciesResult_t.Fill(intPtr));
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

		public static void Install(Action<GetAppDependenciesResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(GetAppDependenciesResult_t.OnClient), GetAppDependenciesResult_t.StructSize, 3416, false);
				GetAppDependenciesResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(GetAppDependenciesResult_t.OnServer), GetAppDependenciesResult_t.StructSize, 3416, true);
				GetAppDependenciesResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetAppDependenciesResult_t> action = GetAppDependenciesResult_t.actionClient;
			if (action != null)
			{
				action(GetAppDependenciesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<GetAppDependenciesResult_t> action = GetAppDependenciesResult_t.actionServer;
			if (action != null)
			{
				action(GetAppDependenciesResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal Steamworks.Data.PublishedFileId PublishedFileId;

			internal AppId[] GAppIDs;

			internal uint NumAppDependencies;

			internal uint TotalNumAppDependencies;

			public static implicit operator GetAppDependenciesResult_t(GetAppDependenciesResult_t.Pack8 d)
			{
				GetAppDependenciesResult_t getAppDependenciesResultT = new GetAppDependenciesResult_t()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					GAppIDs = d.GAppIDs,
					NumAppDependencies = d.NumAppDependencies,
					TotalNumAppDependencies = d.TotalNumAppDependencies
				};
				return getAppDependenciesResultT;
			}

			public static implicit operator Pack8(GetAppDependenciesResult_t d)
			{
				GetAppDependenciesResult_t.Pack8 pack8 = new GetAppDependenciesResult_t.Pack8()
				{
					Result = d.Result,
					PublishedFileId = d.PublishedFileId,
					GAppIDs = d.GAppIDs,
					NumAppDependencies = d.NumAppDependencies,
					TotalNumAppDependencies = d.TotalNumAppDependencies
				};
				return pack8;
			}
		}
	}
}