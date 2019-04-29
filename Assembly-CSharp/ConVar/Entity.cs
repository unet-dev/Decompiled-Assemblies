using Network;
using Network.Visibility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace ConVar
{
	[Factory("entity")]
	public class Entity : ConsoleSystem
	{
		public Entity()
		{
		}

		[ServerVar]
		public static void debug_toggle(ConsoleSystem.Arg args)
		{
			int num = args.GetInt(0, 0);
			if (num == 0)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find((uint)num) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.SetFlag(BaseEntity.Flags.Debugging, !baseEntity.IsDebugging(), false, true);
			if (baseEntity.IsDebugging())
			{
				baseEntity.OnDebugStart();
			}
			ConsoleSystem.Arg arg = args;
			object[] d = new object[] { "Debugging for ", baseEntity.net.ID, " ", null };
			d[3] = (baseEntity.IsDebugging() ? "enabled" : "disabled");
			arg.ReplyWith(string.Concat(d));
		}

		[ServerVar(Help="Destroy all entities created by this user")]
		public static int DeleteBy(ulong SteamId)
		{
			if (SteamId == 0)
			{
				return 0;
			}
			int num = 0;
			foreach (BaseEntity serverEntity in BaseNetworkable.serverEntities)
			{
				if (serverEntity == null || serverEntity.OwnerID != SteamId)
				{
					continue;
				}
				serverEntity.Invoke(new Action(serverEntity.KillMessage), (float)num * 0.2f);
				num++;
			}
			return num;
		}

		[ClientVar]
		[ServerVar]
		public static void find_entity(ConsoleSystem.Arg args)
		{
			string str = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => {
				if (string.IsNullOrEmpty(str))
				{
					return true;
				}
				return info.entity.PrefabName.Contains(str);
			});
			args.ReplyWith(entityTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void find_group(ConsoleSystem.Arg args)
		{
			uint num = args.GetUInt(0, 0);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.groupID == num);
			args.ReplyWith(entityTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void find_id(ConsoleSystem.Arg args)
		{
			uint num = args.GetUInt(0, 0);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == num);
			args.ReplyWith(entityTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void find_parent(ConsoleSystem.Arg args)
		{
			uint num = args.GetUInt(0, 0);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.parentID == num);
			args.ReplyWith(entityTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void find_radius(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (basePlayer == null)
			{
				return;
			}
			uint num = args.GetUInt(0, 10);
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => Vector3.Distance(info.entity.transform.position, basePlayer.transform.position) <= (float)((float)num));
			args.ReplyWith(entityTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void find_self(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			if (basePlayer == null)
			{
				return;
			}
			if (basePlayer.net == null)
			{
				return;
			}
			uint d = basePlayer.net.ID;
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => info.entityID == d);
			args.ReplyWith(entityTable.ToString());
		}

		[ClientVar]
		[ServerVar]
		public static void find_status(ConsoleSystem.Arg args)
		{
			string str = args.GetString(0, "");
			TextTable entityTable = Entity.GetEntityTable((Entity.EntityInfo info) => {
				if (string.IsNullOrEmpty(str))
				{
					return true;
				}
				return info.status.Contains(str);
			});
			args.ReplyWith(entityTable.ToString());
		}

		private static TextTable GetEntityTable(Func<Entity.EntityInfo, bool> filter)
		{
			TextTable textTable = new TextTable();
			textTable.AddColumn("realm");
			textTable.AddColumn("entity");
			textTable.AddColumn("group");
			textTable.AddColumn("parent");
			textTable.AddColumn("name");
			textTable.AddColumn("position");
			textTable.AddColumn("local");
			textTable.AddColumn("rotation");
			textTable.AddColumn("local");
			textTable.AddColumn("status");
			textTable.AddColumn("invokes");
			foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
			{
				if (serverEntity == null)
				{
					continue;
				}
				Entity.EntityInfo entityInfo = new Entity.EntityInfo(serverEntity);
				if (!filter(entityInfo))
				{
					continue;
				}
				string[] str = new string[] { "sv", entityInfo.entityID.ToString(), entityInfo.groupID.ToString(), entityInfo.parentID.ToString(), entityInfo.entity.ShortPrefabName, null, null, null, null, null, null };
				Vector3 vector3 = entityInfo.entity.transform.position;
				str[5] = vector3.ToString();
				vector3 = entityInfo.entity.transform.localPosition;
				str[6] = vector3.ToString();
				Quaternion quaternion = entityInfo.entity.transform.rotation;
				vector3 = quaternion.eulerAngles;
				str[7] = vector3.ToString();
				quaternion = entityInfo.entity.transform.localRotation;
				vector3 = quaternion.eulerAngles;
				str[8] = vector3.ToString();
				str[9] = entityInfo.status;
				str[10] = entityInfo.entity.InvokeString();
				textTable.AddRow(str);
			}
			return textTable;
		}

		[ServerVar]
		public static void nudge(int entID)
		{
			if (entID == 0)
			{
				return;
			}
			BaseEntity baseEntity = BaseNetworkable.serverEntities.Find((uint)entID) as BaseEntity;
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.BroadcastMessage("DebugNudge", SendMessageOptions.DontRequireReceiver);
		}

		[ServerVar]
		public static void spawnlootfrom(ConsoleSystem.Arg args)
		{
			BasePlayer basePlayer = args.Player();
			string str = args.GetString(0, string.Empty);
			int num = args.GetInt(1, 1);
			Vector3 vector3 = args.GetVector3(1, (basePlayer ? basePlayer.CenterPoint() : Vector3.zero));
			if (string.IsNullOrEmpty(str))
			{
				return;
			}
			GameManager gameManager = GameManager.server;
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str, vector3, quaternion, true);
			if (baseEntity == null)
			{
				return;
			}
			baseEntity.Spawn();
			basePlayer.ChatMessage(string.Concat(new object[] { "Contents of ", str, " spawned ", num, " times" }));
			LootContainer component = baseEntity.GetComponent<LootContainer>();
			if (component != null)
			{
				for (int i = 0; i < num * component.maxDefinitionsToSpawn; i++)
				{
					component.lootDefinition.SpawnIntoContainer(basePlayer.inventory.containerMain);
				}
			}
			baseEntity.Kill(BaseNetworkable.DestroyMode.None);
		}

		[ServerVar(Name="spawn")]
		public static string svspawn(string name, Vector3 pos)
		{
			if (string.IsNullOrEmpty(name))
			{
				return "No entity name provided";
			}
			string[] array = (
				from x in GameManifest.Current.entities
				where Path.GetFileNameWithoutExtension(x).Contains(name, CompareOptions.IgnoreCase)
				select x.ToLower()).ToArray<string>();
			if (array.Length == 0)
			{
				return "Entity type not found";
			}
			if ((int)array.Length > 1)
			{
				string str = array.FirstOrDefault<string>((string x) => string.Compare(Path.GetFileNameWithoutExtension(x), name, StringComparison.OrdinalIgnoreCase) == 0);
				if (str == null)
				{
					return string.Concat("Unknown entity - could be:\n\n", string.Join("\n", array.Select<string, string>(new Func<string, string>(Path.GetFileNameWithoutExtension)).ToArray<string>()));
				}
				array[0] = str;
			}
			GameManager gameManager = GameManager.server;
			string str1 = array[0];
			Quaternion quaternion = new Quaternion();
			BaseEntity baseEntity = gameManager.CreateEntity(str1, pos, quaternion, true);
			if (baseEntity == null)
			{
				return string.Concat("Couldn't spawn ", name);
			}
			baseEntity.Spawn();
			return string.Concat(new object[] { "spawned ", baseEntity, " at ", pos });
		}

		[ServerVar(Name="spawnitem")]
		public static string svspawnitem(string name, Vector3 pos)
		{
			if (string.IsNullOrEmpty(name))
			{
				return "No entity name provided";
			}
			string[] array = (
				from x in ItemManager.itemList
				select x.shortname into x
				where x.Contains(name, CompareOptions.IgnoreCase)
				select x).ToArray<string>();
			if (array.Length == 0)
			{
				return "Entity type not found";
			}
			if ((int)array.Length > 1)
			{
				string str = array.FirstOrDefault<string>((string x) => string.Compare(x, name, StringComparison.OrdinalIgnoreCase) == 0);
				if (str == null)
				{
					return string.Concat("Unknown entity - could be:\n\n", string.Join("\n", array));
				}
				array[0] = str;
			}
			Item item = ItemManager.CreateByName(array[0], 1, (ulong)0);
			if (item == null)
			{
				return string.Concat("Couldn't spawn ", name);
			}
			Quaternion quaternion = new Quaternion();
			item.CreateWorldObject(pos, quaternion, null, 0);
			return string.Concat(new object[] { "spawned ", item, " at ", pos });
		}

		private struct EntityInfo
		{
			public BaseNetworkable entity;

			public uint entityID;

			public uint groupID;

			public uint parentID;

			public string status;

			public EntityInfo(BaseNetworkable src)
			{
				BaseEntity parentEntity;
				uint d;
				uint num;
				uint num1;
				this.entity = src;
				BaseEntity baseEntity = this.entity as BaseEntity;
				if (baseEntity != null)
				{
					parentEntity = baseEntity.GetParentEntity();
				}
				else
				{
					parentEntity = null;
				}
				BaseEntity baseEntity1 = parentEntity;
				if (!(this.entity != null) || this.entity.net == null)
				{
					d = 0;
				}
				else
				{
					d = this.entity.net.ID;
				}
				this.entityID = d;
				if (!(this.entity != null) || this.entity.net == null || this.entity.net.@group == null)
				{
					num = 0;
				}
				else
				{
					num = this.entity.net.@group.ID;
				}
				this.groupID = num;
				if (baseEntity != null)
				{
					num1 = baseEntity.parentEntity.uid;
				}
				else
				{
					num1 = 0;
				}
				this.parentID = num1;
				if (!(baseEntity != null) || baseEntity.parentEntity.uid == 0)
				{
					this.status = string.Empty;
					return;
				}
				if (baseEntity1 == null)
				{
					this.status = "orphan";
					return;
				}
				this.status = "child";
			}
		}
	}
}