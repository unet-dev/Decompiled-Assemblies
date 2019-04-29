using Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

[Factory("global")]
public class DiagnosticsConSys : ConsoleSystem
{
	public DiagnosticsConSys()
	{
	}

	[ClientVar]
	[ServerVar]
	public static void dump(ConsoleSystem.Arg args)
	{
		if (Directory.Exists("diagnostics"))
		{
			Directory.CreateDirectory("diagnostics");
		}
		int num = 1;
		while (Directory.Exists(string.Concat("diagnostics/", num)))
		{
			num++;
		}
		Directory.CreateDirectory(string.Concat("diagnostics/", num));
		string str = string.Concat("diagnostics/", num, "/");
		DiagnosticsConSys.DumpLODGroups(str);
		DiagnosticsConSys.DumpSystemInformation(str);
		DiagnosticsConSys.DumpGameObjects(str);
		DiagnosticsConSys.DumpObjects(str);
		DiagnosticsConSys.DumpEntities(str);
		DiagnosticsConSys.DumpNetwork(str);
		DiagnosticsConSys.DumpPhysics(str);
		DiagnosticsConSys.DumpAnimators(str);
	}

	private static void DumpAnimators(string targetFolder)
	{
		Animator[] animatorArray = UnityEngine.Object.FindObjectsOfType<Animator>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All animators");
		stringBuilder.AppendLine();
		Animator[] animatorArray1 = animatorArray;
		for (int i = 0; i < (int)animatorArray1.Length; i++)
		{
			Animator animator = animatorArray1[i];
			stringBuilder.AppendFormat("{1}\t{0}", animator.transform.GetRecursiveName(""), animator.enabled);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.Animators.List.txt"), stringBuilder.ToString());
		StringBuilder stringBuilder1 = new StringBuilder();
		stringBuilder1.AppendLine("All animators - grouped by object name");
		stringBuilder1.AppendLine();
		foreach (IGrouping<string, Animator> strs in 
			from x in (IEnumerable<Animator>)animatorArray
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<Animator>() descending
			select x)
		{
			stringBuilder1.AppendFormat("{1:N0}\t{0}", strs.First<Animator>().transform.GetRecursiveName(""), strs.Count<Animator>());
			stringBuilder1.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.Animators.Counts.txt"), stringBuilder1.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All animators - grouped by enabled/disabled");
		stringBuilder2.AppendLine();
		foreach (IGrouping<string, Animator> strs1 in 
			from x in (IEnumerable<Animator>)animatorArray
			group x by x.transform.GetRecursiveName((x.enabled ? "" : " (DISABLED)")) into x
			orderby x.Count<Animator>() descending
			select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", strs1.First<Animator>().transform.GetRecursiveName((strs1.First<Animator>().enabled ? "" : " (DISABLED)")), strs1.Count<Animator>());
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.Animators.Counts.Enabled.txt"), stringBuilder2.ToString());
	}

	private static void DumpColliders(string targetFolder)
	{
		Collider[] colliderArray = UnityEngine.Object.FindObjectsOfType<Collider>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Colliders");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Collider> strs in 
			from x in (IEnumerable<Collider>)colliderArray
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<Collider>() descending
			select x)
		{
			StringBuilder stringBuilder1 = stringBuilder;
			object[] key = new object[] { strs.Key, strs.Count<Collider>(), null, null };
			key[2] = strs.Count<Collider>((Collider x) => x.isTrigger);
			key[3] = strs.Count<Collider>((Collider x) => x.enabled);
			stringBuilder1.AppendFormat("{1:N0}\t{0} ({2:N0} triggers) ({3:N0} enabled)", key);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "Physics.Colliders.Objects.txt"), stringBuilder.ToString());
	}

	private static void DumpEntities(string targetFolder)
	{
		uint d;
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All entities");
		stringBuilder.AppendLine();
		foreach (BaseNetworkable serverEntity in BaseNetworkable.serverEntities)
		{
			StringBuilder stringBuilder1 = stringBuilder;
			string prefabName = serverEntity.PrefabName;
			if (serverEntity.net != null)
			{
				d = serverEntity.net.ID;
			}
			else
			{
				d = 0;
			}
			stringBuilder1.AppendFormat("{1}\t{0}", prefabName, d);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.Entity.SV.List.txt"), stringBuilder.ToString());
		StringBuilder stringBuilder2 = new StringBuilder();
		stringBuilder2.AppendLine("All entities");
		stringBuilder2.AppendLine();
		foreach (IGrouping<uint, BaseNetworkable> nums in 
			from x in BaseNetworkable.serverEntities
			group x by x.prefabID into x
			orderby x.Count<BaseNetworkable>() descending
			select x)
		{
			stringBuilder2.AppendFormat("{1:N0}\t{0}", nums.First<BaseNetworkable>().PrefabName, nums.Count<BaseNetworkable>());
			stringBuilder2.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.Entity.SV.Counts.txt"), stringBuilder2.ToString());
		StringBuilder stringBuilder3 = new StringBuilder();
		stringBuilder3.AppendLine("Saved entities");
		stringBuilder3.AppendLine();
		foreach (IGrouping<uint, BaseEntity> nums1 in 
			from x in BaseEntity.saveList
			group x by x.prefabID into x
			orderby x.Count<BaseEntity>() descending
			select x)
		{
			stringBuilder3.AppendFormat("{1:N0}\t{0}", nums1.First<BaseEntity>().PrefabName, nums1.Count<BaseEntity>());
			stringBuilder3.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.Entity.SV.Savelist.Counts.txt"), stringBuilder3.ToString());
	}

	private static void DumpGameObjectRecursive(StringBuilder str, Transform tx, int indent, bool includeComponents = false)
	{
		if (tx == null)
		{
			return;
		}
		for (int i = 0; i < indent; i++)
		{
			str.Append(" ");
		}
		str.AppendFormat("{0} {1:N0}", tx.name, (int)tx.GetComponents<Component>().Length - 1);
		str.AppendLine();
		if (includeComponents)
		{
			Component[] components = tx.GetComponents<Component>();
			for (int j = 0; j < (int)components.Length; j++)
			{
				Component component = components[j];
				if (!(component is Transform))
				{
					for (int k = 0; k < indent + 1; k++)
					{
						str.Append(" ");
					}
					str.AppendFormat("[c] {0}", (component == null ? "NULL" : component.GetType().ToString()));
					str.AppendLine();
				}
			}
		}
		for (int l = 0; l < tx.childCount; l++)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(str, tx.GetChild(l), indent + 2, includeComponents);
		}
	}

	private static void DumpGameObjects(string targetFolder)
	{
		int i;
		Transform[] rootObjects = TransformUtil.GetRootObjects();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects");
		stringBuilder.AppendLine();
		Transform[] transformArrays = rootObjects;
		for (i = 0; i < (int)transformArrays.Length; i++)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, transformArrays[i], 0, false);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "GameObject.Hierarchy.txt"), stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active game objects including components");
		stringBuilder.AppendLine();
		transformArrays = rootObjects;
		for (i = 0; i < (int)transformArrays.Length; i++)
		{
			DiagnosticsConSys.DumpGameObjectRecursive(stringBuilder, transformArrays[i], 0, true);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "GameObject.Hierarchy.Components.txt"), stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects excluding children");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, Transform> strs in 
			from x in (IEnumerable<Transform>)rootObjects
			group x by x.name into x
			orderby x.Count<Transform>() descending
			select x)
		{
			Transform transforms = strs.First<Transform>();
			stringBuilder.AppendFormat("{1:N0}\t{0}", transforms.name, strs.Count<Transform>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "GameObject.Count.txt"), stringBuilder.ToString());
		stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Root gameobjects, grouped by name, ordered by the total number of objects including children");
		stringBuilder.AppendLine();
		foreach (KeyValuePair<Transform, int> keyValuePair in 
			from x in (IEnumerable<Transform>)rootObjects
			group x by x.name into x
			select new KeyValuePair<Transform, int>(x.First<Transform>(), x.Sum<Transform>((Transform y) => y.GetAllChildren().Count)) into x
			orderby x.Value descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", keyValuePair.Key.name, keyValuePair.Value);
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "GameObject.Count.Children.txt"), stringBuilder.ToString());
	}

	private static void DumpLODGroups(string targetFolder)
	{
		DiagnosticsConSys.DumpLODGroupTotals(targetFolder);
	}

	private static void DumpLODGroupTotals(string targetFolder)
	{
		LODGroup[] lODGroupArray = UnityEngine.Object.FindObjectsOfType<LODGroup>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("LODGroups");
		stringBuilder.AppendLine();
		foreach (IGrouping<string, LODGroup> strs in 
			from x in (IEnumerable<LODGroup>)lODGroupArray
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<LODGroup>() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", strs.Key, strs.Count<LODGroup>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "LODGroups.Objects.txt"), stringBuilder.ToString());
	}

	private static void DumpNetwork(string targetFolder)
	{
		if (Net.sv.IsConnected())
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Server Network Statistics");
			stringBuilder.AppendLine();
			stringBuilder.Append(Net.sv.GetDebug(null).Replace("\n", "\r\n"));
			stringBuilder.AppendLine();
			foreach (BasePlayer basePlayer in BasePlayer.activePlayerList)
			{
				stringBuilder.AppendLine(string.Concat("Name: ", basePlayer.displayName));
				stringBuilder.AppendLine(string.Concat("SteamID: ", basePlayer.userID));
				stringBuilder.Append((basePlayer.net == null ? "INVALID - NET IS NULL" : Net.sv.GetDebug(basePlayer.net.connection).Replace("\n", "\r\n")));
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
				stringBuilder.AppendLine();
			}
			DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "Network.Server.txt"), stringBuilder.ToString());
		}
	}

	private static void DumpObjects(string targetFolder)
	{
		UnityEngine.Object[] objArray = UnityEngine.Object.FindObjectsOfType<UnityEngine.Object>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("All active UnityEngine.Object, ordered by count");
		stringBuilder.AppendLine();
		foreach (IGrouping<Type, UnityEngine.Object> types in 
			from x in (IEnumerable<UnityEngine.Object>)objArray
			group x by x.GetType() into x
			orderby x.Count<UnityEngine.Object>() descending
			select x)
		{
			stringBuilder.AppendFormat("{1:N0}\t{0}", types.First<UnityEngine.Object>().GetType().Name, types.Count<UnityEngine.Object>());
			stringBuilder.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.Object.Count.txt"), stringBuilder.ToString());
		StringBuilder stringBuilder1 = new StringBuilder();
		stringBuilder1.AppendLine("All active UnityEngine.ScriptableObject, ordered by count");
		stringBuilder1.AppendLine();
		foreach (IGrouping<Type, UnityEngine.Object> types1 in 
			from x in (IEnumerable<UnityEngine.Object>)objArray
			where x is ScriptableObject
			group x by x.GetType() into x
			orderby x.Count<UnityEngine.Object>() descending
			select x)
		{
			stringBuilder1.AppendFormat("{1:N0}\t{0}", types1.First<UnityEngine.Object>().GetType().Name, types1.Count<UnityEngine.Object>());
			stringBuilder1.AppendLine();
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "UnityEngine.ScriptableObject.Count.txt"), stringBuilder1.ToString());
	}

	private static void DumpPhysics(string targetFolder)
	{
		DiagnosticsConSys.DumpTotals(targetFolder);
		DiagnosticsConSys.DumpColliders(targetFolder);
		DiagnosticsConSys.DumpRigidBodies(targetFolder);
	}

	private static void DumpRigidBodies(string targetFolder)
	{
		Rigidbody[] rigidbodyArray = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("RigidBody");
		stringBuilder.AppendLine();
		StringBuilder stringBuilder1 = new StringBuilder();
		stringBuilder1.AppendLine("RigidBody");
		stringBuilder1.AppendLine();
		foreach (IGrouping<string, Rigidbody> strs in 
			from x in (IEnumerable<Rigidbody>)rigidbodyArray
			group x by x.transform.GetRecursiveName("") into x
			orderby x.Count<Rigidbody>() descending
			select x)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			object[] key = new object[] { strs.Key, strs.Count<Rigidbody>(), null, null, null };
			key[2] = strs.Count<Rigidbody>((Rigidbody x) => !x.IsSleeping());
			key[3] = strs.Count<Rigidbody>((Rigidbody x) => x.isKinematic);
			key[4] = strs.Count<Rigidbody>((Rigidbody x) => x.collisionDetectionMode != CollisionDetectionMode.Discrete);
			stringBuilder2.AppendFormat("{1:N0}\t{0} ({2:N0} awake) ({3:N0} kinematic) ({4:N0} non-discrete)", key);
			stringBuilder.AppendLine();
			foreach (Rigidbody rigidbody in strs)
			{
				StringBuilder stringBuilder3 = stringBuilder1;
				object[] objArray = new object[] { strs.Key, null, null, null };
				objArray[1] = (rigidbody.isKinematic ? " KIN" : "");
				objArray[2] = (rigidbody.IsSleeping() ? " SLEEP" : "");
				objArray[3] = (rigidbody.useGravity ? " GRAVITY" : "");
				stringBuilder3.AppendFormat("{0} -{1}{2}{3}", objArray);
				stringBuilder1.AppendLine();
				stringBuilder1.AppendFormat("Mass: {0}\tVelocity: {1}\tsleepThreshold: {2}", rigidbody.mass, rigidbody.velocity, rigidbody.sleepThreshold);
				stringBuilder1.AppendLine();
				stringBuilder1.AppendLine();
			}
		}
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "Physics.RigidBody.Objects.txt"), stringBuilder.ToString());
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "Physics.RigidBody.All.txt"), stringBuilder1.ToString());
	}

	private static void DumpSystemInformation(string targetFolder)
	{
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "System.Info.txt"), SystemInfoGeneralText.currentInfo);
	}

	private static void DumpTotals(string targetFolder)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine("Physics Information");
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<Collider>().Count<Collider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Active Colliders:\t{0:N0}", (
			from x in (IEnumerable<Collider>)UnityEngine.Object.FindObjectsOfType<Collider>()
			where x.enabled
			select x).Count<Collider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Batched Colliders:\t{0:N0}", (SingletonComponent<ColliderGrid>.Instance ? SingletonComponent<ColliderGrid>.Instance.BatchedMeshCount() : 0));
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Total RigidBodys:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<Rigidbody>().Count<Rigidbody>());
		stringBuilder.AppendLine();
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Mesh Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<MeshCollider>().Count<MeshCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Box Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<BoxCollider>().Count<BoxCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Sphere Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<SphereCollider>().Count<SphereCollider>());
		stringBuilder.AppendLine();
		stringBuilder.AppendFormat("Capsule Colliders:\t{0:N0}", UnityEngine.Object.FindObjectsOfType<CapsuleCollider>().Count<CapsuleCollider>());
		stringBuilder.AppendLine();
		DiagnosticsConSys.WriteTextToFile(string.Concat(targetFolder, "Physics.txt"), stringBuilder.ToString());
	}

	private static void WriteTextToFile(string file, string text)
	{
		File.WriteAllText(file, text);
	}
}