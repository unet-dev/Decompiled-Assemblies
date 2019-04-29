using System;
using WebSocketSharp;
using WebSocketSharp.Net.WebSockets;

namespace WebSocketSharp.Server
{
	public abstract class WebSocketServiceHost
	{
		public abstract bool KeepClean
		{
			get;
			set;
		}

		public abstract string Path
		{
			get;
		}

		public abstract WebSocketSessionManager Sessions
		{
			get;
		}

		internal ServerState State
		{
			get
			{
				return this.Sessions.State;
			}
		}

		public abstract System.Type Type
		{
			get;
		}

		public abstract TimeSpan WaitTime
		{
			get;
			set;
		}

		protected WebSocketServiceHost()
		{
		}

		protected abstract WebSocketBehavior CreateSession();

		internal void Start()
		{
			this.Sessions.Start();
		}

		internal void StartSession(WebSocketContext context)
		{
			this.CreateSession().Start(context, this.Sessions);
		}

		internal void Stop(ushort code, string reason)
		{
			byte[] array;
			CloseEventArgs closeEventArg = new CloseEventArgs(code, reason);
			bool flag = !code.IsReserved();
			if (flag)
			{
				array = WebSocketFrame.CreateCloseFrame(closeEventArg.PayloadData, false).ToArray();
			}
			else
			{
				array = null;
			}
			this.Sessions.Stop(closeEventArg, array, flag);
		}
	}
}