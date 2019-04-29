using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class ObjectWorkQueue<T>
{
	protected Queue<T> queue;

	protected HashSet<T> containerTest;

	public string queueName;

	public long warnTime;

	public long totalProcessed;

	public double totalTime;

	public int queueProcessedLast;

	public double lastMS;

	public int hashsetMaxLength;

	public int queueLength
	{
		get
		{
			return this.queue.Count;
		}
	}

	public ObjectWorkQueue()
	{
		this.queueName = this.GetType().FullName;
	}

	public void Add(T entity)
	{
		if (this.Contains(entity))
		{
			return;
		}
		if (!this.ShouldAdd(entity))
		{
			return;
		}
		this.queue.Enqueue(entity);
		this.containerTest.Add(entity);
		this.hashsetMaxLength = Mathf.Max(this.containerTest.Count, this.hashsetMaxLength);
	}

	public void Clear()
	{
		this.queue.Clear();
		this.queue = new Queue<T>();
		this.containerTest.Clear();
		if (this.hashsetMaxLength > 256)
		{
			this.containerTest = new HashSet<T>();
			this.hashsetMaxLength = 0;
		}
	}

	public bool Contains(T entity)
	{
		return this.containerTest.Contains(entity);
	}

	public string Info()
	{
		return string.Format("{0:n0}, lastMS: {1:n0}, tot: {2:n0}, totMS: {3:n0} ", new object[] { this.queueLength, this.queueProcessedLast, this.totalProcessed, this.totalTime });
	}

	protected abstract void RunJob(T entity);

	public void RunQueue(double maximumMilliseconds)
	{
		if (this.queue.Count == 0)
		{
			return;
		}
		this.SortQueue();
		using (TimeWarning timeWarning = TimeWarning.New(this.queueName, this.warnTime))
		{
			Stopwatch stopwatch = Stopwatch.StartNew();
			this.queueProcessedLast = 0;
			do
			{
				if (this.queue.Count <= 0)
				{
					break;
				}
				this.queueProcessedLast++;
				this.totalProcessed += (long)1;
				T t = this.queue.Dequeue();
				this.containerTest.Remove(t);
				if (t == null)
				{
					continue;
				}
				this.RunJob(t);
			}
			while (stopwatch.Elapsed.TotalMilliseconds < maximumMilliseconds);
			this.lastMS = stopwatch.Elapsed.TotalMilliseconds;
			this.totalTime += this.lastMS;
		}
		if (this.queue.Count == 0)
		{
			this.Clear();
		}
	}

	protected virtual bool ShouldAdd(T entity)
	{
		return true;
	}

	protected virtual void SortQueue()
	{
	}
}