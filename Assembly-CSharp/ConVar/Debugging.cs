using Facepunch;
using Facepunch.Unity;
using Network;
using Rust;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace ConVar
{
	[Factory("debug")]
	public class Debugging : ConsoleSystem
	{
		[ClientVar]
		[ServerVar]
		public static bool checktriggers;

		[ServerVar(Help="Do not damage any items")]
		public static bool disablecondition;

		[ClientVar]
		[ServerVar]
		public static bool callbacks;

		[ClientVar]
		[ServerVar]
		public static bool log
		{
			get
			{
				return Debug.unityLogger.logEnabled;
			}
			set
			{
				Debug.unityLogger.logEnabled = value;
			}
		}

		static Debugging()
		{
		}

		public Debugging()
		{
		}

		[ServerVar(Help="Break the current held object")]
		public static void breakheld(ConsoleSystem.Arg arg)
		{
			Item activeItem = arg.Player().GetActiveItem();
			if (activeItem == null)
			{
				return;
			}
			activeItem.LoseCondition(activeItem.condition * 2f);
		}

		[ServerVar(Help="Break all the items in your inventory whose name match the passed string")]
		public static void breakitem(ConsoleSystem.Arg arg)
		{
			string str = arg.GetString(0, "");
			foreach (Item item in arg.Player().inventory.containerMain.itemList)
			{
				if (!item.info.shortname.Contains(str, CompareOptions.IgnoreCase) || !item.hasCondition)
				{
					continue;
				}
				item.LoseCondition(item.condition * 2f);
			}
		}

		[ServerVar]
		public static void drink(ConsoleSystem.Arg arg)
		{
			arg.Player().metabolism.ApplyChange(MetabolismAttribute.Type.Hydration, (float)arg.GetInt(0, 1), (float)arg.GetInt(1, 1));
		}

		[ServerVar]
		public static void eat(ConsoleSystem.Arg arg)
		{
			arg.Player().metabolism.ApplyChange(MetabolismAttribute.Type.Calories, (float)arg.GetInt(0, 1), (float)arg.GetInt(1, 1));
		}

		[ServerVar(Help="Takes you in and out of your current network group, causing you to delete and then download all entities in your PVS again")]
		public static void flushgroup(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			if (basePlayer == null)
			{
				return;
			}
			basePlayer.net.SwitchGroup(BaseNetworkable.LimboNetworkGroup);
			basePlayer.UpdateNetworkGroup();
		}

		[ServerVar]
		public static void hurt(ConsoleSystem.Arg arg)
		{
			BasePlayer basePlayer = arg.Player();
			HitInfo hitInfo = new HitInfo(basePlayer, basePlayer, DamageType.Bullet, (float)arg.GetInt(0, 1));
			string str = arg.GetString(1, string.Empty);
			if (!string.IsNullOrEmpty(str))
			{
				hitInfo.HitBone = StringPool.Get(str);
			}
			basePlayer.OnAttacked(hitInfo);
		}

		[ServerVar(EditorOnly=true, Help="respawn all puzzles from their prefabs")]
		public static void puzzleprefabrespawn(ConsoleSystem.Arg arg)
		{
			foreach (BaseNetworkable list in BaseNetworkable.serverEntities.Where<BaseNetworkable>((BaseNetworkable x) => {
				if (!(x is IOEntity))
				{
					return false;
				}
				return PrefabAttribute.server.Find<Construction>(x.prefabID) == null;
			}).ToList<BaseNetworkable>())
			{
				list.Kill(BaseNetworkable.DestroyMode.None);
			}
			foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
			{
				GameObject gameObject = GameManager.server.FindPrefab(monument.gameObject.name);
				if (gameObject == null)
				{
					continue;
				}
				Dictionary<IOEntity, IOEntity> oEntities = new Dictionary<IOEntity, IOEntity>();
				IOEntity[] componentsInChildren = gameObject.GetComponentsInChildren<IOEntity>(true);
				for (int i = 0; i < (int)componentsInChildren.Length; i++)
				{
					IOEntity oEntity = componentsInChildren[i];
					Quaternion quaternion = monument.transform.rotation * oEntity.transform.rotation;
					Vector3 vector3 = monument.transform.TransformPoint(oEntity.transform.position);
					BaseEntity baseEntity = GameManager.server.CreateEntity(oEntity.PrefabName, vector3, quaternion, true);
					IOEntity oEntity1 = baseEntity as IOEntity;
					if (oEntity1 != null)
					{
						oEntities.Add(oEntity, oEntity1);
						DoorManipulator doorManipulator = baseEntity as DoorManipulator;
						if (doorManipulator != null)
						{
							List<Door> doors = Facepunch.Pool.GetList<Door>();
							Vis.Entities<Door>(baseEntity.transform.position, 10f, doors, -1, QueryTriggerInteraction.Collide);
							Door door = (
								from x in doors
								orderby x.Distance(baseEntity.transform.position)
								select x).FirstOrDefault<Door>();
							if (door != null)
							{
								doorManipulator.targetDoor = door;
							}
							Facepunch.Pool.FreeList<Door>(ref doors);
						}
						CardReader cardReader = baseEntity as CardReader;
						if (cardReader != null)
						{
							CardReader cardReader1 = oEntity as CardReader;
							if (cardReader1 != null)
							{
								cardReader.accessLevel = cardReader1.accessLevel;
								cardReader.accessDuration = cardReader1.accessDuration;
							}
						}
						TimerSwitch timerSwitch = baseEntity as TimerSwitch;
						if (timerSwitch != null)
						{
							TimerSwitch timerSwitch1 = oEntity as TimerSwitch;
							if (timerSwitch1 != null)
							{
								timerSwitch.timerLength = timerSwitch1.timerLength;
							}
						}
					}
				}
				foreach (KeyValuePair<IOEntity, IOEntity> keyValuePair in oEntities)
				{
					IOEntity key = keyValuePair.Key;
					IOEntity value = keyValuePair.Value;
					for (int j = 0; j < (int)key.outputs.Length; j++)
					{
						if (key.outputs[j].connectedTo.ioEnt != null)
						{
							value.outputs[j].connectedTo.ioEnt = oEntities[key.outputs[j].connectedTo.ioEnt];
							value.outputs[j].connectedToSlot = key.outputs[j].connectedToSlot;
						}
					}
				}
				foreach (IOEntity value1 in oEntities.Values)
				{
					value1.Spawn();
				}
			}
		}

		[ServerVar(Help="reset all puzzles")]
		public static void puzzlereset(ConsoleSystem.Arg arg)
		{
			if (arg.Player() == null)
			{
				return;
			}
			PuzzleReset[] puzzleResetArray = UnityEngine.Object.FindObjectsOfType<PuzzleReset>();
			Debug.Log("iterating...");
			PuzzleReset[] puzzleResetArray1 = puzzleResetArray;
			for (int i = 0; i < (int)puzzleResetArray1.Length; i++)
			{
				PuzzleReset puzzleReset = puzzleResetArray1[i];
				Debug.Log(string.Concat("resetting puzzle at :", puzzleReset.transform.position));
				puzzleReset.DoReset();
				puzzleReset.ResetTimer();
			}
		}

		[ClientVar]
		[ServerVar]
		public static void renderinfo(ConsoleSystem.Arg arg)
		{
			RenderInfo.GenerateReport();
		}

		[ClientVar]
		[ServerVar]
		public static void stall(ConsoleSystem.Arg arg)
		{
			float single = Mathf.Clamp(arg.GetFloat(0, 0f), 0f, 1f);
			arg.ReplyWith(string.Concat("Stalling for ", single, " seconds..."));
			Thread.Sleep(Mathf.RoundToInt(single * 1000f));
		}
	}
}