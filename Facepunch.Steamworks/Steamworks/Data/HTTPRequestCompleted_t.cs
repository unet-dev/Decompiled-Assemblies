using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTTPRequestCompleted_t
	{
		internal uint Request;

		internal ulong ContextValue;

		internal bool RequestSuccessful;

		internal HTTPStatusCode StatusCode;

		internal uint BodySize;

		internal readonly static int StructSize;

		private static Action<HTTPRequestCompleted_t> actionClient;

		private static Action<HTTPRequestCompleted_t> actionServer;

		static HTTPRequestCompleted_t()
		{
			HTTPRequestCompleted_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTTPRequestCompleted_t) : typeof(HTTPRequestCompleted_t.Pack8)));
		}

		internal static HTTPRequestCompleted_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTTPRequestCompleted_t)Marshal.PtrToStructure(p, typeof(HTTPRequestCompleted_t)) : (HTTPRequestCompleted_t.Pack8)Marshal.PtrToStructure(p, typeof(HTTPRequestCompleted_t.Pack8)));
		}

		public static async Task<HTTPRequestCompleted_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTTPRequestCompleted_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTTPRequestCompleted_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTTPRequestCompleted_t.StructSize, 2101, ref flag) | flag))
					{
						nullable = new HTTPRequestCompleted_t?(HTTPRequestCompleted_t.Fill(intPtr));
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

		public static void Install(Action<HTTPRequestCompleted_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTTPRequestCompleted_t.OnClient), HTTPRequestCompleted_t.StructSize, 2101, false);
				HTTPRequestCompleted_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTTPRequestCompleted_t.OnServer), HTTPRequestCompleted_t.StructSize, 2101, true);
				HTTPRequestCompleted_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTTPRequestCompleted_t> action = HTTPRequestCompleted_t.actionClient;
			if (action != null)
			{
				action(HTTPRequestCompleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTTPRequestCompleted_t> action = HTTPRequestCompleted_t.actionServer;
			if (action != null)
			{
				action(HTTPRequestCompleted_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint Request;

			internal ulong ContextValue;

			internal bool RequestSuccessful;

			internal HTTPStatusCode StatusCode;

			internal uint BodySize;

			public static implicit operator HTTPRequestCompleted_t(HTTPRequestCompleted_t.Pack8 d)
			{
				HTTPRequestCompleted_t hTTPRequestCompletedT = new HTTPRequestCompleted_t()
				{
					Request = d.Request,
					ContextValue = d.ContextValue,
					RequestSuccessful = d.RequestSuccessful,
					StatusCode = d.StatusCode,
					BodySize = d.BodySize
				};
				return hTTPRequestCompletedT;
			}

			public static implicit operator Pack8(HTTPRequestCompleted_t d)
			{
				HTTPRequestCompleted_t.Pack8 pack8 = new HTTPRequestCompleted_t.Pack8()
				{
					Request = d.Request,
					ContextValue = d.ContextValue,
					RequestSuccessful = d.RequestSuccessful,
					StatusCode = d.StatusCode,
					BodySize = d.BodySize
				};
				return pack8;
			}
		}
	}
}