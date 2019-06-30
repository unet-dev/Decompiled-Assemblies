using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTTPRequestHeadersReceived_t
	{
		internal uint Request;

		internal ulong ContextValue;

		internal readonly static int StructSize;

		private static Action<HTTPRequestHeadersReceived_t> actionClient;

		private static Action<HTTPRequestHeadersReceived_t> actionServer;

		static HTTPRequestHeadersReceived_t()
		{
			HTTPRequestHeadersReceived_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTTPRequestHeadersReceived_t) : typeof(HTTPRequestHeadersReceived_t.Pack8)));
		}

		internal static HTTPRequestHeadersReceived_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTTPRequestHeadersReceived_t)Marshal.PtrToStructure(p, typeof(HTTPRequestHeadersReceived_t)) : (HTTPRequestHeadersReceived_t.Pack8)Marshal.PtrToStructure(p, typeof(HTTPRequestHeadersReceived_t.Pack8)));
		}

		public static async Task<HTTPRequestHeadersReceived_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTTPRequestHeadersReceived_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTTPRequestHeadersReceived_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTTPRequestHeadersReceived_t.StructSize, 2102, ref flag) | flag))
					{
						nullable = new HTTPRequestHeadersReceived_t?(HTTPRequestHeadersReceived_t.Fill(intPtr));
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

		public static void Install(Action<HTTPRequestHeadersReceived_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTTPRequestHeadersReceived_t.OnClient), HTTPRequestHeadersReceived_t.StructSize, 2102, false);
				HTTPRequestHeadersReceived_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTTPRequestHeadersReceived_t.OnServer), HTTPRequestHeadersReceived_t.StructSize, 2102, true);
				HTTPRequestHeadersReceived_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTTPRequestHeadersReceived_t> action = HTTPRequestHeadersReceived_t.actionClient;
			if (action != null)
			{
				action(HTTPRequestHeadersReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTTPRequestHeadersReceived_t> action = HTTPRequestHeadersReceived_t.actionServer;
			if (action != null)
			{
				action(HTTPRequestHeadersReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint Request;

			internal ulong ContextValue;

			public static implicit operator HTTPRequestHeadersReceived_t(HTTPRequestHeadersReceived_t.Pack8 d)
			{
				HTTPRequestHeadersReceived_t hTTPRequestHeadersReceivedT = new HTTPRequestHeadersReceived_t()
				{
					Request = d.Request,
					ContextValue = d.ContextValue
				};
				return hTTPRequestHeadersReceivedT;
			}

			public static implicit operator Pack8(HTTPRequestHeadersReceived_t d)
			{
				HTTPRequestHeadersReceived_t.Pack8 pack8 = new HTTPRequestHeadersReceived_t.Pack8()
				{
					Request = d.Request,
					ContextValue = d.ContextValue
				};
				return pack8;
			}
		}
	}
}