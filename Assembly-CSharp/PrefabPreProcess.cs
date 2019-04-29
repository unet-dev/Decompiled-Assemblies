using Facepunch;
using Rust.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;
using VLB;

public class PrefabPreProcess : IPrefabProcessor
{
	public static Type[] clientsideOnlyTypes;

	public static Type[] serversideOnlyTypes;

	public bool isClientside;

	public bool isServerside;

	public bool isBundling;

	internal Dictionary<string, GameObject> prefabList = new Dictionary<string, GameObject>(StringComparer.OrdinalIgnoreCase);

	private List<Component> destroyList = new List<Component>();

	private List<GameObject> cleanupList = new List<GameObject>();

	static PrefabPreProcess()
	{
		PrefabPreProcess.clientsideOnlyTypes = new Type[] { typeof(IClientComponent), typeof(ImageEffectLayer), typeof(NGSS_Directional), typeof(VolumetricDustParticles), typeof(VolumetricLightBeam), typeof(Cloth), typeof(MeshFilter), typeof(Renderer), typeof(AudioLowPassFilter), typeof(AudioSource), typeof(AudioListener), typeof(ParticleSystemRenderer), typeof(ParticleSystem), typeof(ParticleEmitFromParentObject), typeof(Light), typeof(LODGroup), typeof(Animator), typeof(AnimationEvents), typeof(PlayerVoiceSpeaker), typeof(PlayerVoiceRecorder), typeof(ParticleScaler), typeof(PostEffectsBase), typeof(TOD_ImageEffect), typeof(TOD_Scattering), typeof(TOD_Rays), typeof(Tree), typeof(Projector), typeof(HttpImage), typeof(EventTrigger), typeof(StandaloneInputModule), typeof(UIBehaviour), typeof(Canvas), typeof(CanvasRenderer), typeof(CanvasGroup), typeof(GraphicRaycaster) };
		PrefabPreProcess.serversideOnlyTypes = new Type[] { typeof(IServerComponent), typeof(NavMeshObstacle) };
	}

	public PrefabPreProcess(bool clientside, bool serverside, bool bundling = false)
	{
		this.isClientside = clientside;
		this.isServerside = serverside;
		this.isBundling = bundling;
	}

	public void AddPrefab(string name, GameObject go)
	{
		go.SetActive(false);
		this.prefabList.Add(name, go);
	}

	private void DestroyComponents(Type t, GameObject go, bool client, bool server)
	{
		List<Component> components = new List<Component>();
		PrefabPreProcess.FindComponents(go.transform, components, t);
		components.Reverse();
		foreach (Component component in components)
		{
			RealmedRemove realmedRemove = component.GetComponent<RealmedRemove>();
			if (realmedRemove != null && !realmedRemove.ShouldDelete(component, client, server))
			{
				continue;
			}
			if (!component.gameObject.CompareTag("persist"))
			{
				this.NominateForDeletion(component.gameObject);
			}
			UnityEngine.Object.DestroyImmediate(component, true);
		}
	}

	private void DoCleanup(GameObject go)
	{
		if (go == null)
		{
			return;
		}
		if ((int)go.GetComponentsInChildren<Component>(true).Length > 1)
		{
			return;
		}
		Transform transforms = go.transform.parent;
		if (transforms == null)
		{
			return;
		}
		if (transforms.name.StartsWith("PrefabPreProcess - "))
		{
			return;
		}
		UnityEngine.Object.DestroyImmediate(go, true);
	}

	public GameObject Find(string strPrefab)
	{
		GameObject gameObject;
		if (!this.prefabList.TryGetValue(strPrefab, out gameObject))
		{
			return null;
		}
		if (gameObject != null)
		{
			return gameObject;
		}
		this.prefabList.Remove(strPrefab);
		return null;
	}

	public static List<T> FindComponents<T>(Transform transform)
	{
		List<T> ts = new List<T>();
		PrefabPreProcess.FindComponents<T>(transform, ts);
		return ts;
	}

	public static void FindComponents<T>(Transform transform, List<T> list)
	{
		list.AddRange(transform.GetComponents<T>());
		foreach (Transform transforms in transform)
		{
			if (PrefabPreProcess.ShouldExclude(transforms))
			{
				continue;
			}
			PrefabPreProcess.FindComponents<T>(transforms, list);
		}
	}

	public static List<Component> FindComponents(Transform transform, Type t)
	{
		List<Component> components = new List<Component>();
		PrefabPreProcess.FindComponents(transform, components, t);
		return components;
	}

	public static void FindComponents(Transform transform, List<Component> list, Type t)
	{
		list.AddRange(transform.GetComponents(t));
		foreach (Transform transforms in transform)
		{
			if (PrefabPreProcess.ShouldExclude(transforms))
			{
				continue;
			}
			PrefabPreProcess.FindComponents(transforms, list, t);
		}
	}

	public GameObject GetHierarchyGroup()
	{
		if (this.isClientside && this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Generic", false, true);
		}
		if (this.isServerside)
		{
			return HierarchyUtil.GetRoot("PrefabPreProcess - Server", false, true);
		}
		return HierarchyUtil.GetRoot("PrefabPreProcess - Client", false, true);
	}

	private static bool HasComponents<T>(Transform transform)
	{
		bool flag;
		if (transform.GetComponent<T>() != null)
		{
			return true;
		}
		IEnumerator enumerator = transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform current = (Transform)enumerator.Current;
				if (PrefabPreProcess.ShouldExclude(current) || !PrefabPreProcess.HasComponents<T>(current))
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return flag;
	}

	private static bool HasComponents(Transform transform, Type t)
	{
		bool flag;
		if (transform.GetComponent(t) != null)
		{
			return true;
		}
		IEnumerator enumerator = transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				Transform current = (Transform)enumerator.Current;
				if (PrefabPreProcess.ShouldExclude(current) || !PrefabPreProcess.HasComponents(current, t))
				{
					continue;
				}
				flag = true;
				return flag;
			}
			return false;
		}
		finally
		{
			IDisposable disposable = enumerator as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
		}
		return flag;
	}

	public bool NeedsProcessing(GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return false;
		}
		if (PrefabPreProcess.HasComponents<IPrefabPreProcess>(go.transform))
		{
			return true;
		}
		if (PrefabPreProcess.HasComponents<IPrefabPostProcess>(go.transform))
		{
			return true;
		}
		if (PrefabPreProcess.HasComponents<IEditorComponent>(go.transform))
		{
			return true;
		}
		if (!this.isClientside)
		{
			if (PrefabPreProcess.clientsideOnlyTypes.Any<Type>((Type type) => PrefabPreProcess.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (PrefabPreProcess.HasComponents<IClientComponentEx>(go.transform))
			{
				return true;
			}
		}
		if (!this.isServerside)
		{
			if (PrefabPreProcess.serversideOnlyTypes.Any<Type>((Type type) => PrefabPreProcess.HasComponents(go.transform, type)))
			{
				return true;
			}
			if (PrefabPreProcess.HasComponents<IServerComponentEx>(go.transform))
			{
				return true;
			}
		}
		return false;
	}

	public void NominateForDeletion(GameObject gameObj)
	{
		this.cleanupList.Add(gameObj);
	}

	public void Process(string name, GameObject go)
	{
		if (go.CompareTag("NoPreProcessing"))
		{
			return;
		}
		GameObject hierarchyGroup = this.GetHierarchyGroup();
		GameObject gameObject = go;
		go = Instantiate.GameObject(gameObject, hierarchyGroup.transform);
		go.name = gameObject.name;
		if (this.NeedsProcessing(go))
		{
			this.ProcessObject(name, go, true);
		}
		this.AddPrefab(name, go);
	}

	public void ProcessObject(string name, GameObject go, bool resetLocalTransform = true)
	{
		Type[] typeArray;
		int i;
		if (!this.isClientside)
		{
			typeArray = PrefabPreProcess.clientsideOnlyTypes;
			for (i = 0; i < (int)typeArray.Length; i++)
			{
				Type type = typeArray[i];
				this.DestroyComponents(type, go, this.isClientside, this.isServerside);
			}
			foreach (IClientComponentEx clientComponentEx in PrefabPreProcess.FindComponents<IClientComponentEx>(go.transform))
			{
				clientComponentEx.PreClientComponentCull(this);
			}
		}
		if (!this.isServerside)
		{
			typeArray = PrefabPreProcess.serversideOnlyTypes;
			for (i = 0; i < (int)typeArray.Length; i++)
			{
				Type type1 = typeArray[i];
				this.DestroyComponents(type1, go, this.isClientside, this.isServerside);
			}
			foreach (IServerComponentEx serverComponentEx in PrefabPreProcess.FindComponents<IServerComponentEx>(go.transform))
			{
				serverComponentEx.PreServerComponentCull(this);
			}
		}
		this.DestroyComponents(typeof(IEditorComponent), go, this.isClientside, this.isServerside);
		if (resetLocalTransform)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
		}
		List<Transform> transforms = PrefabPreProcess.FindComponents<Transform>(go.transform);
		transforms.Reverse();
		foreach (IPrefabPreProcess prefabPreProcess in PrefabPreProcess.FindComponents<IPrefabPreProcess>(go.transform))
		{
			prefabPreProcess.PreProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
		foreach (Transform transforms1 in transforms)
		{
			if (!transforms1 || !transforms1.gameObject)
			{
				continue;
			}
			if (this.isServerside && transforms1.gameObject.CompareTag("Server Cull"))
			{
				this.RemoveComponents(transforms1.gameObject);
				this.NominateForDeletion(transforms1.gameObject);
			}
			if (!this.isClientside)
			{
				continue;
			}
			if (!(transforms1.gameObject.CompareTag("Client Cull") | (transforms1 == go.transform ? false : transforms1.gameObject.GetComponent<BaseEntity>() != null)))
			{
				continue;
			}
			this.RemoveComponents(transforms1.gameObject);
			this.NominateForDeletion(transforms1.gameObject);
		}
		this.RunCleanupQueue();
		foreach (IPrefabPostProcess prefabPostProcess in PrefabPreProcess.FindComponents<IPrefabPostProcess>(go.transform))
		{
			prefabPostProcess.PostProcess(this, go, name, this.isServerside, this.isClientside, this.isBundling);
		}
	}

	public void RemoveComponent(Component c)
	{
		if (c == null)
		{
			return;
		}
		this.destroyList.Add(c);
	}

	public void RemoveComponents(GameObject gameObj)
	{
		Component[] components = gameObj.GetComponents<Component>();
		for (int i = 0; i < (int)components.Length; i++)
		{
			Component component = components[i];
			if (!(component is Transform))
			{
				this.destroyList.Add(component);
			}
		}
	}

	private void RunCleanupQueue()
	{
		foreach (Component component in this.destroyList)
		{
			UnityEngine.Object.DestroyImmediate(component, true);
		}
		this.destroyList.Clear();
		foreach (GameObject gameObject in this.cleanupList)
		{
			this.DoCleanup(gameObject);
		}
		this.cleanupList.Clear();
	}

	private static bool ShouldExclude(Transform transform)
	{
		if (transform.GetComponent<BaseEntity>() != null)
		{
			return true;
		}
		return false;
	}
}