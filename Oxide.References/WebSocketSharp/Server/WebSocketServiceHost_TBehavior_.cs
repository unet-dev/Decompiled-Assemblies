using System;
using WebSocketSharp;

namespace WebSocketSharp.Server
{
	internal class WebSocketServiceHost<TBehavior> : WebSocketServiceHost
	where TBehavior : WebSocketBehavior
	{
		private Func<TBehavior> _initializer;

		private Logger _logger;

		private string _path;

		private WebSocketSessionManager _sessions;

		public override bool KeepClean
		{
			get
			{
				return this._sessions.KeepClean;
			}
			set
			{
				string str = this._sessions.State.CheckIfAvailable(true, false, false);
				if (str == null)
				{
					this._sessions.KeepClean = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		public override string Path
		{
			get
			{
				return this._path;
			}
		}

		public override WebSocketSessionManager Sessions
		{
			get
			{
				return this._sessions;
			}
		}

		public override System.Type Type
		{
			get
			{
				return typeof(TBehavior);
			}
		}

		public override TimeSpan WaitTime
		{
			get
			{
				return this._sessions.WaitTime;
			}
			set
			{
				string str = this._sessions.State.CheckIfAvailable(true, false, false) ?? value.CheckIfValidWaitTime();
				if (str == null)
				{
					this._sessions.WaitTime = value;
				}
				else
				{
					this._logger.Error(str);
				}
			}
		}

		internal WebSocketServiceHost(string path, Func<TBehavior> initializer, Logger logger)
		{
			this._path = path;
			this._initializer = initializer;
			this._logger = logger;
			this._sessions = new WebSocketSessionManager(logger);
		}

		protected override WebSocketBehavior CreateSession()
		{
			return this._initializer();
		}
	}
}