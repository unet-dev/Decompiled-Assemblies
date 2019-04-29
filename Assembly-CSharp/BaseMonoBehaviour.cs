using ConVar;
using System;
using UnityEngine;

public abstract class BaseMonoBehaviour : FacepunchBehaviour
{
	protected BaseMonoBehaviour()
	{
	}

	public virtual string GetLogColor()
	{
		return "yellow";
	}

	public virtual bool IsDebugging()
	{
		return false;
	}

	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string str1 = string.Format(str, arg1);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[] { log.ToString().PadRight(10), this.ToString(), str1, this.GetLogColor() }), base.gameObject);
	}

	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str, object arg1, object arg2)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string str1 = string.Format(str, arg1, arg2);
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[] { log.ToString().PadRight(10), this.ToString(), str1, this.GetLogColor() }), base.gameObject);
	}

	public void LogEntry(BaseMonoBehaviour.LogEntryType log, int level, string str)
	{
		if (!this.IsDebugging() && Global.developer < level)
		{
			return;
		}
		string str1 = str;
		Debug.Log(string.Format("<color=white>{0}</color>[<color={3}>{1}</color>] {2}", new object[] { log.ToString().PadRight(10), this.ToString(), str1, this.GetLogColor() }), base.gameObject);
	}

	public enum LogEntryType
	{
		General,
		Network,
		Hierarchy,
		Serialization
	}
}