using System;
using UnityEngine;
using UnityEngine.UI;

public class GameStat : MonoBehaviour
{
	public float refreshTime = 5f;

	public Text title;

	public Text globalStat;

	public Text localStat;

	private long globalValue;

	private long localValue;

	private long oldGlobalValue;

	private long oldLocalValue;

	private float secondsSinceRefresh;

	private float secondsUntilUpdate;

	private float secondsUntilChange;

	public GameStat.Stat[] stats;

	public GameStat()
	{
	}

	[Serializable]
	public struct Stat
	{
		public string statName;

		public string statTitle;
	}
}