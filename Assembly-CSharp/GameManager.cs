using ConVar;
using Facepunch;
using Rust;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
	public static GameManager server;

	internal PrefabPreProcess preProcessed;

	internal PrefabPoolCollection pool;

	private bool Clientside;

	private bool Serverside;

	static GameManager()
	{
		GameManager.server = new GameManager(false, true);
	}

	public GameManager(bool clientside, bool serverside)
	{
		this.Clientside = clientside;
		this.Serverside = serverside;
		this.preProcessed = new PrefabPreProcess(clientside, serverside, false);
		this.pool = new PrefabPoolCollection();
	}

	public BaseEntity CreateEntity(string strPrefab, Vector3 pos = null, Quaternion rot = null, bool startActive = true)
	{
		if (string.IsNullOrEmpty(strPrefab))
		{
			return null;
		}
		GameObject gameObject = this.CreatePrefab(strPrefab, pos, rot, startActive);
		if (gameObject == null)
		{
			return null;
		}
		BaseEntity component = gameObject.GetComponent<BaseEntity>();
		if (component)
		{
			return component;
		}
		Debug.LogError(string.Concat("CreateEntity called on a prefab that isn't an entity! ", strPrefab));
		UnityEngine.Object.Destroy(gameObject);
		return null;
	}

	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, Vector3 scale, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject)
		{
			gameObject.transform.localScale = scale;
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	public GameObject CreatePrefab(string strPrefab, Vector3 pos, Quaternion rot, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, pos, rot);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	public GameObject CreatePrefab(string strPrefab, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, Vector3.zero, Quaternion.identity);
		if (gameObject && active)
		{
			gameObject.AwakeFromInstantiate();
		}
		return gameObject;
	}

	public GameObject CreatePrefab(string strPrefab, Transform parent, bool active = true)
	{
		GameObject gameObject = this.Instantiate(strPrefab, parent.position, parent.rotation);
		if (gameObject)
		{
			gameObject.transform.SetParent(parent, false);
			gameObject.Identity();
			if (active)
			{
				gameObject.AwakeFromInstantiate();
			}
		}
		return gameObject;
	}

	public static void Destroy(Component component, float delay = 0f)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError(string.Concat("Trying to destroy an entity without killing it first: ", component.name));
		}
		UnityEngine.Object.Destroy(component, delay);
	}

	public static void Destroy(GameObject instance, float delay = 0f)
	{
		if (!instance)
		{
			return;
		}
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError(string.Concat("Trying to destroy an entity without killing it first: ", instance.name));
		}
		UnityEngine.Object.Destroy(instance, delay);
	}

	public static void DestroyImmediate(Component component, bool allowDestroyingAssets = false)
	{
		if ((component as BaseEntity).IsValid())
		{
			Debug.LogError(string.Concat("Trying to destroy an entity without killing it first: ", component.name));
		}
		UnityEngine.Object.DestroyImmediate(component, allowDestroyingAssets);
	}

	public static void DestroyImmediate(GameObject instance, bool allowDestroyingAssets = false)
	{
		if (instance.GetComponent<BaseEntity>().IsValid())
		{
			Debug.LogError(string.Concat("Trying to destroy an entity without killing it first: ", instance.name));
		}
		UnityEngine.Object.DestroyImmediate(instance, allowDestroyingAssets);
	}

	public GameObject FindPrefab(uint prefabID)
	{
		string str = StringPool.Get(prefabID);
		if (string.IsNullOrEmpty(str))
		{
			return null;
		}
		return this.FindPrefab(str);
	}

	public GameObject FindPrefab(BaseEntity ent)
	{
		if (ent == null)
		{
			return null;
		}
		return this.FindPrefab(ent.PrefabName);
	}

	public GameObject FindPrefab(string strPrefab)
	{
		GameObject gameObject = this.preProcessed.Find(strPrefab);
		if (gameObject != null)
		{
			return gameObject;
		}
		gameObject = FileSystem.LoadPrefab(strPrefab);
		if (gameObject == null)
		{
			return null;
		}
		this.preProcessed.Process(strPrefab, gameObject);
		gameObject = this.preProcessed.Find(strPrefab);
		return gameObject;
	}

	private GameObject Instantiate(string strPrefab, Vector3 pos, Quaternion rot)
	{
		if (!strPrefab.IsLower())
		{
			Debug.LogWarning(string.Concat("Converting prefab name to lowercase: ", strPrefab));
			strPrefab = strPrefab.ToLower();
		}
		GameObject gameObject = this.FindPrefab(strPrefab);
		if (!gameObject)
		{
			Debug.LogError(string.Concat("Couldn't find prefab \"", strPrefab, "\""));
			return null;
		}
		GameObject gameObject1 = this.pool.Pop(StringPool.Get(strPrefab), pos, rot);
		if (gameObject1 != null)
		{
			gameObject1.transform.localScale = gameObject.transform.localScale;
		}
		else
		{
			gameObject1 = Instantiate.GameObject(gameObject, pos, rot);
			gameObject1.name = strPrefab;
		}
		if (!this.Clientside && this.Serverside && gameObject1.transform.parent == null)
		{
			SceneManager.MoveGameObjectToScene(gameObject1, Rust.Server.EntityScene);
		}
		return gameObject1;
	}

	public void Reset()
	{
		this.pool.Clear();
	}

	public void Retire(GameObject instance)
	{
		if (!instance)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("GameManager.Retire", 0.1f))
		{
			if (instance.GetComponent<BaseEntity>().IsValid())
			{
				Debug.LogError(string.Concat("Trying to retire an entity without killing it first: ", instance.name));
			}
			if (Rust.Application.isQuitting || !ConVar.Pool.enabled || !instance.SupportsPooling())
			{
				UnityEngine.Object.Destroy(instance);
			}
			else
			{
				this.pool.Push(instance);
			}
		}
	}
}