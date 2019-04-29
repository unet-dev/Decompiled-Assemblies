using System;
using UnityEngine;

public class LifeInfographicStat : MonoBehaviour
{
	public LifeInfographicStat.DataType dataSource;

	public LifeInfographicStat()
	{
	}

	public enum DataType
	{
		None,
		AliveTime_Short,
		SleepingTime_Short,
		KillerName,
		KillerWeapon,
		AliveTime_Long
	}
}