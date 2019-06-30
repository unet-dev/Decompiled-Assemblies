using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct ComputeNewPlayerCompatibilityResult_t
	{
		internal Steamworks.Result Result;

		internal int CPlayersThatDontLikeCandidate;

		internal int CPlayersThatCandidateDoesntLike;

		internal int CClanPlayersThatDontLikeCandidate;

		internal ulong SteamIDCandidate;

		internal readonly static int StructSize;

		private static Action<ComputeNewPlayerCompatibilityResult_t> actionClient;

		private static Action<ComputeNewPlayerCompatibilityResult_t> actionServer;

		static ComputeNewPlayerCompatibilityResult_t()
		{
			ComputeNewPlayerCompatibilityResult_t.StructSize = Marshal.SizeOf(typeof(ComputeNewPlayerCompatibilityResult_t));
		}

		internal static ComputeNewPlayerCompatibilityResult_t Fill(IntPtr p)
		{
			return (ComputeNewPlayerCompatibilityResult_t)Marshal.PtrToStructure(p, typeof(ComputeNewPlayerCompatibilityResult_t));
		}

		public static async Task<ComputeNewPlayerCompatibilityResult_t?> GetResultAsync(SteamAPICall_t handle)
		{
			ComputeNewPlayerCompatibilityResult_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(ComputeNewPlayerCompatibilityResult_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, ComputeNewPlayerCompatibilityResult_t.StructSize, 211, ref flag) | flag))
					{
						nullable = new ComputeNewPlayerCompatibilityResult_t?(ComputeNewPlayerCompatibilityResult_t.Fill(intPtr));
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

		public static void Install(Action<ComputeNewPlayerCompatibilityResult_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(ComputeNewPlayerCompatibilityResult_t.OnClient), ComputeNewPlayerCompatibilityResult_t.StructSize, 211, false);
				ComputeNewPlayerCompatibilityResult_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(ComputeNewPlayerCompatibilityResult_t.OnServer), ComputeNewPlayerCompatibilityResult_t.StructSize, 211, true);
				ComputeNewPlayerCompatibilityResult_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ComputeNewPlayerCompatibilityResult_t> action = ComputeNewPlayerCompatibilityResult_t.actionClient;
			if (action != null)
			{
				action(ComputeNewPlayerCompatibilityResult_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<ComputeNewPlayerCompatibilityResult_t> action = ComputeNewPlayerCompatibilityResult_t.actionServer;
			if (action != null)
			{
				action(ComputeNewPlayerCompatibilityResult_t.Fill(pvParam));
			}
			else
			{
			}
		}
	}
}