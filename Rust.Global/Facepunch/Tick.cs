using System;
using System.Collections.Generic;
using UnityEngine;

namespace Facepunch
{
	public static class Tick
	{
		private static Tick.Entry.List Timed;

		private static Tick.Entry.List Update;

		private static Tick.Entry.List Late;

		private static List<UnityEngine.Object> RemoveList;

		static Tick()
		{
			Tick.Timed = new Tick.Entry.List();
			Tick.Update = new Tick.Entry.List();
			Tick.Late = new Tick.Entry.List();
			Tick.RemoveList = new List<UnityEngine.Object>(32);
		}

		public static void Add(UnityEngine.Object obj, Action action, string DebugName)
		{
			TickComponent.Init();
			Tick.Entry.List update = Tick.Update;
			Tick.Entry entry = new Tick.Entry()
			{
				TargetObject = obj,
				Function = action,
				DebugName = string.Format("{0} - {1}", DebugName, obj.name)
			};
			update.Add(entry);
		}

		public static void AddLateUpdate(UnityEngine.Object obj, Action action, string DebugName)
		{
			TickComponent.Init();
			Tick.Entry.List late = Tick.Late;
			Tick.Entry entry = new Tick.Entry()
			{
				TargetObject = obj,
				Function = action,
				DebugName = string.Format("{0} - {1}", DebugName, obj.name)
			};
			late.Add(entry);
		}

		public static void AddTimed(UnityEngine.Object obj, float minDelay, float maxDelay, Action action, string DebugName)
		{
			TickComponent.Init();
			Tick.Entry.List timed = Tick.Timed;
			Tick.Entry entry = new Tick.Entry()
			{
				TargetObject = obj,
				MinDelay = minDelay,
				RandDelay = maxDelay - minDelay,
				Function = action,
				DebugName = string.Format("{0} - {1}", DebugName, obj.name)
			};
			timed.Add(entry);
		}

		private static void Cleanup()
		{
			if (Tick.RemoveList.Count == 0)
			{
				return;
			}
			for (int i = 0; i < Tick.RemoveList.Count; i++)
			{
				UnityEngine.Object item = Tick.RemoveList[i];
				Tick.Timed.Remove(item);
				Tick.Update.Remove(item);
				Tick.Late.Remove(item);
			}
			Tick.RemoveList.Clear();
		}

		internal static void OnFrame()
		{
			Tick.Cleanup();
			Tick.Update.Tick();
			Tick.Cleanup();
			Tick.Timed.TickTimed();
		}

		internal static void OnFrameLate()
		{
			Tick.Cleanup();
			Tick.Late.Tick();
		}

		public static void RemoveAll(UnityEngine.Object obj)
		{
			Tick.RemoveList.Add(obj);
		}

		public struct Entry
		{
			public UnityEngine.Object TargetObject;

			public float MinDelay;

			public float RandDelay;

			public float NextCall;

			public Action Function;

			private bool Errored;

			public string DebugName;

			public class List : List<Tick.Entry>
			{
				public List()
				{
				}

				public void Remove(UnityEngine.Object obj)
				{
					for (int i = 0; i < base.Count; i++)
					{
						if (base[i].TargetObject == obj || base[i].Errored)
						{
							base.RemoveAt(i);
							i--;
						}
					}
				}

				internal void Tick()
				{
					int i = 0;
					try
					{
						for (i = 0; i < base.Count; i++)
						{
							base[i].Function();
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						Tick.Entry item = base[i];
						item.Errored = true;
						base[i] = item;
					}
				}

				internal void TickTimed()
				{
					float single = Time.time;
					int i = 0;
					try
					{
						for (i = 0; i < base.Count; i++)
						{
							Tick.Entry item = base[i];
							if (item.NextCall <= single)
							{
								item.Function();
								item.NextCall = single + item.MinDelay + item.RandDelay * UnityEngine.Random.Range(0f, 1f);
								base[i] = item;
							}
						}
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						Tick.Entry entry = base[i];
						entry.Errored = true;
						base[i] = entry;
					}
				}
			}
		}
	}
}