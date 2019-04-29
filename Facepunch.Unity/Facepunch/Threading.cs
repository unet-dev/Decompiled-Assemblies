using System;
using System.Collections.Generic;
using System.Threading;

namespace Facepunch
{
	public static class Threading
	{
		private static int mainThread;

		private static List<Action> actions;

		public static bool IsMainThread
		{
			get
			{
				return Thread.CurrentThread.ManagedThreadId == Threading.mainThread;
			}
		}

		static Threading()
		{
			Threading.actions = new List<Action>();
		}

		public static void QueueOnMainThread(Action action)
		{
			if (Threading.IsMainThread)
			{
				action();
				return;
			}
			lock (Threading.actions)
			{
				Threading.actions.Add(action);
			}
		}

		internal static void RunQueuedFunctionsOnMainThread()
		{
			Threading.mainThread = Thread.CurrentThread.ManagedThreadId;
			lock (Threading.actions)
			{
				foreach (Action action in Threading.actions)
				{
					action();
				}
				Threading.actions.Clear();
			}
		}
	}
}