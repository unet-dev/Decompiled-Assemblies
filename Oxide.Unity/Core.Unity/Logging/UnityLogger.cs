using Oxide.Core;
using Oxide.Core.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Oxide.Core.Unity.Logging
{
	public sealed class UnityLogger : Oxide.Core.Logging.Logger
	{
		private readonly Thread mainThread = Thread.CurrentThread;

		public UnityLogger() : base(true)
		{
		}

		protected override void ProcessMessage(Oxide.Core.Logging.Logger.LogMessage message)
		{
			if (Thread.CurrentThread != this.mainThread)
			{
				Interface.get_Oxide().NextTick(() => this.ProcessMessage(message));
				return;
			}
			Oxide.Core.Logging.LogType type = message.Type;
			if (type == 1)
			{
				Debug.LogError(message.ConsoleMessage);
				return;
			}
			if (type == 3)
			{
				Debug.LogWarning(message.ConsoleMessage);
				return;
			}
			Debug.Log(message.ConsoleMessage);
		}
	}
}