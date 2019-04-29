using System;
using System.Diagnostics;
using UnityEngine;

public class EntityDebug : EntityComponent<BaseEntity>
{
	internal Stopwatch stopwatch = Stopwatch.StartNew();

	public EntityDebug()
	{
	}

	private void Update()
	{
		if (!base.baseEntity.IsValid() || !base.baseEntity.IsDebugging())
		{
			base.enabled = false;
			return;
		}
		if (this.stopwatch.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		bool flag = base.baseEntity.isClient;
		if (base.baseEntity.isServer)
		{
			base.baseEntity.DebugServer(1, (float)this.stopwatch.Elapsed.TotalSeconds);
		}
		this.stopwatch.Reset();
		this.stopwatch.Start();
	}
}