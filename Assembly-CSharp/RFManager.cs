using System;
using System.Collections.Generic;
using UnityEngine;

public class RFManager
{
	public static Dictionary<int, List<IRFObject>> _listeners;

	public static Dictionary<int, List<IRFObject>> _broadcasters;

	public static int minFreq;

	public static int maxFreq;

	private static int reserveRangeMin;

	private static int reserveRangeMax;

	public static string reserveString;

	static RFManager()
	{
		RFManager._listeners = new Dictionary<int, List<IRFObject>>();
		RFManager._broadcasters = new Dictionary<int, List<IRFObject>>();
		RFManager.minFreq = 1;
		RFManager.maxFreq = 9999;
		RFManager.reserveRangeMin = 4760;
		RFManager.reserveRangeMax = 4790;
		RFManager.reserveString = string.Concat(new object[] { "Channels ", RFManager.reserveRangeMin, " to ", RFManager.reserveRangeMax, " are restricted." });
	}

	public RFManager()
	{
	}

	public static void AddBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			return;
		}
		broadcasterList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	public static void AddListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			Debug.Log("adding same listener twice");
			return;
		}
		listenList.Add(obj);
		RFManager.MarkFrequencyDirty(frequency);
	}

	public static void ChangeFrequency(int oldFrequency, int newFrequency, IRFObject obj, bool isListener, bool isOn = true)
	{
		newFrequency = RFManager.ClampFrequency(newFrequency);
		if (!isListener)
		{
			RFManager.RemoveBroadcaster(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddBroadcaster(newFrequency, obj);
			}
		}
		else
		{
			RFManager.RemoveListener(oldFrequency, obj);
			if (isOn)
			{
				RFManager.AddListener(newFrequency, obj);
				return;
			}
		}
	}

	public static int ClampFrequency(int freq)
	{
		return Mathf.Clamp(freq, RFManager.minFreq, RFManager.maxFreq);
	}

	public static List<IRFObject> GetBroadcasterList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> rFObjects = null;
		if (!RFManager._broadcasters.TryGetValue(frequency, out rFObjects))
		{
			rFObjects = new List<IRFObject>();
			RFManager._broadcasters.Add(frequency, rFObjects);
		}
		return rFObjects;
	}

	public static List<IRFObject> GetListenList(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> rFObjects = null;
		if (!RFManager._listeners.TryGetValue(frequency, out rFObjects))
		{
			rFObjects = new List<IRFObject>();
			RFManager._listeners.Add(frequency, rFObjects);
		}
		return rFObjects;
	}

	public static bool IsReserved(int frequency)
	{
		if (frequency >= RFManager.reserveRangeMin && frequency <= RFManager.reserveRangeMax)
		{
			return true;
		}
		return false;
	}

	public static void MarkFrequencyDirty(int frequency)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		bool count = broadcasterList.Count > 0;
		bool flag = false;
		bool flag1 = false;
		foreach (IRFObject rFObject in listenList)
		{
			if (rFObject.IsValidEntityReference<IRFObject>())
			{
				if (count)
				{
					count = false;
					foreach (IRFObject rFObject1 in broadcasterList)
					{
						if (rFObject1.IsValidEntityReference<IRFObject>())
						{
							if (Vector3.Distance(rFObject1.GetPosition(), rFObject.GetPosition()) > rFObject1.GetMaxRange())
							{
								continue;
							}
							count = true;
							goto Label0;
						}
						else
						{
							flag1 = true;
						}
					}
				}
			Label0:
				rFObject.RFSignalUpdate(count);
			}
			else
			{
				flag = true;
			}
		}
		if (flag)
		{
			Debug.LogWarning(string.Concat("Found null entries in the RF listener list for frequency ", frequency, "... cleaning up."));
			for (int i = listenList.Count - 1; i >= 0; i--)
			{
				if (listenList[i] == null)
				{
					listenList.RemoveAt(i);
				}
			}
		}
		if (flag1)
		{
			Debug.LogWarning(string.Concat("Found null entries in the RF broadcaster list for frequency ", frequency, "... cleaning up."));
			for (int j = broadcasterList.Count - 1; j >= 0; j--)
			{
				if (broadcasterList[j] == null)
				{
					broadcasterList.RemoveAt(j);
				}
			}
		}
	}

	public static void RemoveBroadcaster(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> broadcasterList = RFManager.GetBroadcasterList(frequency);
		if (broadcasterList.Contains(obj))
		{
			broadcasterList.Remove(obj);
		}
		RFManager.MarkFrequencyDirty(frequency);
	}

	public static void RemoveListener(int frequency, IRFObject obj)
	{
		frequency = RFManager.ClampFrequency(frequency);
		List<IRFObject> listenList = RFManager.GetListenList(frequency);
		if (listenList.Contains(obj))
		{
			listenList.Remove(obj);
		}
		obj.RFSignalUpdate(false);
	}

	public static void ReserveErrorPrint(BasePlayer player)
	{
		player.ChatMessage(RFManager.reserveString);
	}
}