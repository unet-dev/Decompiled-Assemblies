using System;
using UnityEngine;

public class WearableHolsterOffset : MonoBehaviour
{
	public WearableHolsterOffset.offsetInfo[] Offsets;

	public WearableHolsterOffset()
	{
	}

	[Serializable]
	public class offsetInfo
	{
		public HeldEntity.HolsterInfo.HolsterSlot type;

		public Vector3 offset;

		public Vector3 rotationOffset;

		public int priority;

		public offsetInfo()
		{
		}
	}
}