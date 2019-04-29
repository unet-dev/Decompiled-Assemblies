using Oxide.Core;
using System;

namespace Oxide.Core.Logging
{
	public class CallbackLogger : Logger
	{
		private NativeDebugCallback callback;

		public CallbackLogger(NativeDebugCallback callback) : base(true)
		{
			this.callback = callback;
		}

		protected override void ProcessMessage(Logger.LogMessage message)
		{
			NativeDebugCallback nativeDebugCallback = this.callback;
			if (nativeDebugCallback == null)
			{
				return;
			}
			nativeDebugCallback(message.LogfileMessage);
		}
	}
}