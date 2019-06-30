using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct FileDetailsResult_t
	{
		internal Steamworks.Result Result;

		internal ulong FileSize;

		internal byte[] FileSHA;

		internal uint Flags;

		internal readonly static int StructSize;

		private static Action<FileDetailsResult_t> actionClient;

		private static Action<FileDetailsResult_t> actionServer;

		static FileDetailsResult_t()
		{
			FileDetailsResult_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(FileDetailsResult_t) : typeof(FileDetailsResult_t.Pack8)));
		}

		internal static FileDetailsResult_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (FileDetailsResult_t)Marshal.PtrToStructure(p, typeof(FileDetailsResult_t)) : (FileDetailsResult_t.Pack8)Marshal.PtrToStructure(p, typeof(FileDetailsResult_t.Pack8)));
		}

		public static async Task<FileDetailsResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			FileDetailsResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(FileDetailsResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, FileDetailsResult_t.StructSize, 1023, ref flag) | flag))
					{
						nullable = new FileDetailsResult_t?(FileDetailsResult_t.Fill(intPtr));
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

		public static void Install(Action<FileDetailsResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(FileDetailsResult_t.OnClient), FileDetailsResult_t.StructSize, 1023, false);
				FileDetailsResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(FileDetailsResult_t.OnServer), FileDetailsResult_t.StructSize, 1023, true);
				FileDetailsResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FileDetailsResult_t> action = FileDetailsResult_t.actionClient;
			if (action != null)
			{
				action(FileDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<FileDetailsResult_t> action = FileDetailsResult_t.actionServer;
			if (action != null)
			{
				action(FileDetailsResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal Steamworks.Result Result;

			internal ulong FileSize;

			internal byte[] FileSHA;

			internal uint Flags;

			public static implicit operator FileDetailsResult_t(FileDetailsResult_t.Pack8 d)
			{
				FileDetailsResult_t fileDetailsResultT = new FileDetailsResult_t()
				{
					Result = d.Result,
					FileSize = d.FileSize,
					FileSHA = d.FileSHA,
					Flags = d.Flags
				};
				return fileDetailsResultT;
			}

			public static implicit operator Pack8(FileDetailsResult_t d)
			{
				FileDetailsResult_t.Pack8 pack8 = new FileDetailsResult_t.Pack8()
				{
					Result = d.Result,
					FileSize = d.FileSize,
					FileSHA = d.FileSHA,
					Flags = d.Flags
				};
				return pack8;
			}
		}
	}
}