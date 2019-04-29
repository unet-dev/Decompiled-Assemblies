using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class Parallel
	{
		public static int MaxThreads;

		static Parallel()
		{
		}

		public static void Call(Action<int, int> action)
		{
			Parallel.Call(Environment.ProcessorCount, action);
		}

		public static void Call(int threads, Action<int, int> action)
		{
			int num = threads;
			if (Parallel.MaxThreads > 0)
			{
				num = Mathf.Min(num, Parallel.MaxThreads);
			}
			Action<int> action1 = (int thread_id) => action(thread_id, num);
			IAsyncResult[] asyncResultArray = new IAsyncResult[num];
			for (int i = 0; i < num; i++)
			{
				asyncResultArray[i] = action1.BeginInvoke(i, null, null);
			}
			for (int j = 0; j < num; j++)
			{
				action1.EndInvoke(asyncResultArray[j]);
			}
		}

		public static IEnumerator Coroutine(Action action)
		{
			TimeWarning.BeginSample("Parallel.Coroutine");
			IAsyncResult asyncResult = action.BeginInvoke(null, null);
			while (!asyncResult.IsCompleted)
			{
				TimeWarning.EndSample();
				yield return null;
				TimeWarning.BeginSample("Parallel.Coroutine");
			}
			action.EndInvoke(asyncResult);
			TimeWarning.EndSample();
		}

		public static void For(int fromInclusive, int toExclusive, Action<int> action)
		{
			Parallel.For(fromInclusive, toExclusive, Environment.ProcessorCount, action);
		}

		public static void For(int fromInclusive, int toExclusive, int threads, Action<int> action)
		{
			if (Parallel.MaxThreads > 0)
			{
				threads = Mathf.Min(threads, Parallel.MaxThreads);
			}
			int num1 = toExclusive - fromInclusive;
			int num2 = num1 / threads;
			if (threads * num2 < num1)
			{
				num2++;
			}
			Action<int> action1 = (int thread_id) => {
				int threadId = thread_id * num2;
				int num = Mathf.Min(threadId + num2, toExclusive);
				for (int i = threadId; i < num; i++)
				{
					action(i);
				}
			};
			IAsyncResult[] asyncResultArray = new IAsyncResult[threads];
			for (int i1 = 0; i1 < threads; i1++)
			{
				asyncResultArray[i1] = action1.BeginInvoke(i1, null, null);
			}
			for (int j = 0; j < threads; j++)
			{
				action1.EndInvoke(asyncResultArray[j]);
			}
		}

		public static void ForEach<T>(IList<T> data, Action<T> action)
		{
			Parallel.ForEach<T>(data, Environment.ProcessorCount, action);
		}

		public static void ForEach<T>(IList<T> data, int threads, Action<T> action)
		{
			Parallel.For(0, data.Count, threads, (int i) => action(data[i]));
		}
	}
}