using System;
using System.Collections.Generic;
using UnityEngine;

public class AIThinkManager : BaseMonoBehaviour
{
	public static ListHashSet<IThinker> _processQueue;

	public static ListHashSet<IThinker> _removalQueue;

	[Help("How many miliseconds to budget for processing AI entities per server frame")]
	[ServerVar]
	public static float framebudgetms;

	private static int lastIndex;

	static AIThinkManager()
	{
		AIThinkManager._processQueue = new ListHashSet<IThinker>(8);
		AIThinkManager._removalQueue = new ListHashSet<IThinker>(8);
		AIThinkManager.framebudgetms = 2.5f;
		AIThinkManager.lastIndex = 0;
	}

	public AIThinkManager()
	{
	}

	public static void Add(IThinker toAdd)
	{
		AIThinkManager._processQueue.Add(toAdd);
	}

	public static void ProcessQueue()
	{
		float single = Time.realtimeSinceStartup;
		float single1 = (float)AIThinkManager.framebudgetms / 1000f;
		if (AIThinkManager._removalQueue.Count > 0)
		{
			foreach (IThinker thinker in AIThinkManager._removalQueue)
			{
				AIThinkManager._processQueue.Remove(thinker);
			}
			AIThinkManager._removalQueue.Clear();
		}
		while (AIThinkManager.lastIndex < AIThinkManager._processQueue.Count && Time.realtimeSinceStartup < single + single1)
		{
			IThinker item = AIThinkManager._processQueue[AIThinkManager.lastIndex];
			if (item != null)
			{
				item.TryThink();
			}
			AIThinkManager.lastIndex++;
		}
		if (AIThinkManager.lastIndex == AIThinkManager._processQueue.Count)
		{
			AIThinkManager.lastIndex = 0;
		}
	}

	public static void Remove(IThinker toRemove)
	{
		AIThinkManager._removalQueue.Add(toRemove);
	}
}