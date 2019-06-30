using Steamworks;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	internal struct HTTPRequestDataReceived_t
	{
		internal uint Request;

		internal ulong ContextValue;

		internal uint COffset;

		internal uint CBytesReceived;

		internal readonly static int StructSize;

		private static Action<HTTPRequestDataReceived_t> actionClient;

		private static Action<HTTPRequestDataReceived_t> actionServer;

		static HTTPRequestDataReceived_t()
		{
			HTTPRequestDataReceived_t.StructSize = Marshal.SizeOf((Config.PackSmall ? typeof(HTTPRequestDataReceived_t) : typeof(HTTPRequestDataReceived_t.Pack8)));
		}

		internal static HTTPRequestDataReceived_t Fill(IntPtr p)
		{
			return (Config.PackSmall ? (HTTPRequestDataReceived_t)Marshal.PtrToStructure(p, typeof(HTTPRequestDataReceived_t)) : (HTTPRequestDataReceived_t.Pack8)Marshal.PtrToStructure(p, typeof(HTTPRequestDataReceived_t.Pack8)));
		}

		public static async Task<HTTPRequestDataReceived_t?> GetResultAsync(SteamAPICall_t handle)
		{
			HTTPRequestDataReceived_t? nullable;
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
				IntPtr intPtr = Marshal.AllocHGlobal(HTTPRequestDataReceived_t.StructSize);
				try
				{
					if (!(!SteamUtils.Internal.GetAPICallResult(handle, intPtr, HTTPRequestDataReceived_t.StructSize, 2103, ref flag) | flag))
					{
						nullable = new HTTPRequestDataReceived_t?(HTTPRequestDataReceived_t.Fill(intPtr));
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

		public static void Install(Action<HTTPRequestDataReceived_t> action, bool server = false)
		{
			if (!server)
			{
				Event.Register(new Callback.Run(HTTPRequestDataReceived_t.OnClient), HTTPRequestDataReceived_t.StructSize, 2103, false);
				HTTPRequestDataReceived_t.actionClient = action;
			}
			else
			{
				Event.Register(new Callback.Run(HTTPRequestDataReceived_t.OnServer), HTTPRequestDataReceived_t.StructSize, 2103, true);
				HTTPRequestDataReceived_t.actionServer = action;
			}
		}

		[MonoPInvokeCallback]
		private static void OnClient(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTTPRequestDataReceived_t> action = HTTPRequestDataReceived_t.actionClient;
			if (action != null)
			{
				action(HTTPRequestDataReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		[MonoPInvokeCallback]
		private static void OnServer(IntPtr thisptr, IntPtr pvParam)
		{
			Action<HTTPRequestDataReceived_t> action = HTTPRequestDataReceived_t.actionServer;
			if (action != null)
			{
				action(HTTPRequestDataReceived_t.Fill(pvParam));
			}
			else
			{
			}
		}

		public struct Pack8
		{
			internal uint Request;

			internal ulong ContextValue;

			internal uint COffset;

			internal uint CBytesReceived;

			public static implicit operator HTTPRequestDataReceived_t(HTTPRequestDataReceived_t.Pack8 d)
			{
				HTTPRequestDataReceived_t hTTPRequestDataReceivedT = new HTTPRequestDataReceived_t()
				{
					Request = d.Request,
					ContextValue = d.ContextValue,
					COffset = d.COffset,
					CBytesReceived = d.CBytesReceived
				};
				return hTTPRequestDataReceivedT;
			}

			public static implicit operator Pack8(HTTPRequestDataReceived_t d)
			{
				HTTPRequestDataReceived_t.Pack8 pack8 = new HTTPRequestDataReceived_t.Pack8()
				{
					Request = d.Request,
					ContextValue = d.ContextValue,
					COffset = d.COffset,
					CBytesReceived = d.CBytesReceived
				};
				return pack8;
			}
		}
	}
}