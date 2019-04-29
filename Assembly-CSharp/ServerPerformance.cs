using Facepunch;
using Rust;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Profiling;

public class ServerPerformance : BaseMonoBehaviour
{
	public static ulong deaths;

	public static ulong spawns;

	public static ulong position_changes;

	private string fileName;

	private int lastFrame;

	static ServerPerformance()
	{
	}

	public ServerPerformance()
	{
	}

	public static void ComponentReport(string filename, string Title, UnityEngine.Object[] objects)
	{
		File.AppendAllText(filename, string.Concat("\r\n\r\n", Title, ":\r\n\r\n"));
		foreach (IGrouping<string, UnityEngine.Object> strs in 
			from x in (IEnumerable<UnityEngine.Object>)objects
			group x by ServerPerformance.WorkoutPrefabName((x as Component).gameObject) into x
			orderby x.Count<UnityEngine.Object>() descending
			select x)
		{
			File.AppendAllText(filename, string.Concat(new object[] { "\t", ServerPerformance.WorkoutPrefabName((strs.ElementAt<UnityEngine.Object>(0) as Component).gameObject), " - ", strs.Count<UnityEngine.Object>(), "\r\n" }));
		}
		File.AppendAllText(filename, string.Concat("\r\nTotal: ", objects.Count<UnityEngine.Object>(), "\r\n\r\n\r\n"));
	}

	public static void DoReport()
	{
		DateTime now = DateTime.Now;
		string str = string.Concat("report.", now.ToString(), ".txt");
		str = str.Replace('\\', '-');
		str = str.Replace('/', '-');
		str = str.Replace(' ', '\u005F');
		str = str.Replace(':', '.');
		now = DateTime.Now;
		File.WriteAllText(str, string.Concat("Report Generated ", now.ToString(), "\r\n"));
		ServerPerformance.ComponentReport(str, "All Objects", UnityEngine.Object.FindObjectsOfType<Transform>());
		ServerPerformance.ComponentReport(str, "Entities", UnityEngine.Object.FindObjectsOfType<BaseEntity>());
		ServerPerformance.ComponentReport(str, "Rigidbodies", UnityEngine.Object.FindObjectsOfType<Rigidbody>());
		string str1 = str;
		UnityEngine.Object[] array = (
			from x in (IEnumerable<Collider>)UnityEngine.Object.FindObjectsOfType<Collider>()
			where !x.enabled
			select x).ToArray<Collider>();
		ServerPerformance.ComponentReport(str1, "Disabled Colliders", array);
		string str2 = str;
		array = (
			from x in (IEnumerable<Collider>)UnityEngine.Object.FindObjectsOfType<Collider>()
			where x.enabled
			select x).ToArray<Collider>();
		ServerPerformance.ComponentReport(str2, "Enabled Colliders", array);
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			SingletonComponent<SpawnHandler>.Instance.DumpReport(str);
		}
	}

	private void Start()
	{
		if (!Profiler.supported)
		{
			return;
		}
		if (!CommandLine.HasSwitch("-perf"))
		{
			return;
		}
		DateTime now = DateTime.Now;
		this.fileName = string.Concat("perfdata.", now.ToString(), ".txt");
		this.fileName = this.fileName.Replace('\\', '-');
		this.fileName = this.fileName.Replace('/', '-');
		this.fileName = this.fileName.Replace(' ', '\u005F');
		this.fileName = this.fileName.Replace(':', '.');
		this.lastFrame = Time.frameCount;
		File.WriteAllText(this.fileName, "MemMono,MemUnity,Frame,PlayerCount,Sleepers,CollidersDisabled,BehavioursDisabled,GameObjects,Colliders,RigidBodies,BuildingBlocks,nwSend,nwRcv,cnInit,cnApp,cnRej,deaths,spawns,poschange\r\n");
		base.InvokeRepeating(new Action(this.WriteLine), 1f, 60f);
	}

	public static string WorkoutPrefabName(GameObject obj)
	{
		if (obj == null)
		{
			return "null";
		}
		string str = (obj.activeSelf ? "" : " (inactive)");
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			return string.Concat(baseEntity.PrefabName, str);
		}
		return string.Concat(obj.name, str);
	}

	private void WriteLine()
	{
		Rust.GC.Collect();
		uint monoUsedSize = Profiler.GetMonoUsedSize();
		uint num = Profiler.usedHeapSize;
		int count = BasePlayer.activePlayerList.Count;
		int count1 = BasePlayer.sleepingPlayerList.Count;
		int length = (int)UnityEngine.Object.FindObjectsOfType<GameObject>().Length;
		int num1 = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		int num5 = 0;
		int num6 = Time.frameCount - this.lastFrame;
		File.AppendAllText(this.fileName, string.Concat(new object[] { monoUsedSize, ",", num, ",", num6, ",", count, ",", count1, ",", NetworkSleep.totalCollidersDisabled, ",", NetworkSleep.totalBehavioursDisabled, ",", length, ",", (int)UnityEngine.Object.FindObjectsOfType<Collider>().Length, ",", (int)UnityEngine.Object.FindObjectsOfType<Rigidbody>().Length, ",", (int)UnityEngine.Object.FindObjectsOfType<BuildingBlock>().Length, ",", num1, ",", num2, ",", num3, ",", num4, ",", num5, ",", ServerPerformance.deaths, ",", ServerPerformance.spawns, ",", ServerPerformance.position_changes, "\r\n" }));
		this.lastFrame = Time.frameCount;
		ServerPerformance.deaths = (ulong)0;
		ServerPerformance.spawns = (ulong)0;
		ServerPerformance.position_changes = (ulong)0;
	}
}