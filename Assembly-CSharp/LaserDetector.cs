using System;
using System.Collections.Generic;
using UnityEngine;

public class LaserDetector : BaseDetector
{
	public LaserDetector()
	{
	}

	public override void OnObjects()
	{
		foreach (BaseEntity entityContent in this.myTrigger.entityContents)
		{
			if (!entityContent.IsVisible(base.transform.position + (base.transform.forward * 0.1f), 4f))
			{
				continue;
			}
			base.OnObjects();
			return;
		}
	}
}