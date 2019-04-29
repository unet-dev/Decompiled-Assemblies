using Facepunch;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class TransformEx
	{
		public static Transform ActiveChild(this Transform transform, string name, bool bDisableOthers)
		{
			Transform transforms = null;
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				{
					transforms = child;
					child.gameObject.SetActive(true);
				}
				else if (bDisableOthers)
				{
					child.gameObject.SetActive(false);
				}
			}
			return transforms;
		}

		public static void AddAllChildren(this Transform transform, List<Transform> list)
		{
			list.Add(transform);
			for (int i = 0; i < transform.childCount; i++)
			{
				Transform child = transform.GetChild(i);
				if (child != null)
				{
					child.AddAllChildren(list);
				}
			}
		}

		public static GameObject CreateChild(this GameObject go)
		{
			GameObject gameObject = new GameObject();
			gameObject.transform.parent = go.transform;
			gameObject.Identity();
			return gameObject;
		}

		public static void DestroyAllChildren(this Transform transform, bool immediate = false)
		{
			List<GameObject> list = Pool.GetList<GameObject>();
			foreach (Transform transforms in transform)
			{
				if (transforms.CompareTag("persist"))
				{
					continue;
				}
				list.Add(transforms.gameObject);
			}
			if (!immediate)
			{
				foreach (GameObject gameObject in list)
				{
					GameManager.Destroy(gameObject, 0f);
				}
			}
			else
			{
				foreach (GameObject gameObject1 in list)
				{
					GameManager.DestroyImmediate(gameObject1, false);
				}
			}
			Pool.FreeList<GameObject>(ref list);
		}

		public static void DestroyChildren(this Transform transform)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				GameManager.Destroy(transform.GetChild(i).gameObject, 0f);
			}
		}

		public static bool DropToGround(this Transform transform, bool alignToNormal = false, float fRange = 100f)
		{
			Vector3 vector3;
			Vector3 vector31;
			if (!transform.GetGroundInfo(out vector3, out vector31, fRange))
			{
				return false;
			}
			transform.position = vector3;
			if (alignToNormal)
			{
				transform.rotation = Quaternion.LookRotation(transform.forward, vector31);
			}
			return true;
		}

		public static List<Transform> GetAllChildren(this Transform transform)
		{
			List<Transform> transforms = new List<Transform>();
			if (transform != null)
			{
				transform.AddAllChildren(transforms);
			}
			return transforms;
		}

		public static Bounds GetBounds(this Transform transform, bool includeRenderers = true, bool includeColliders = true, bool includeInactive = true)
		{
			int i;
			Bounds bound = new Bounds(Vector3.zero, Vector3.zero);
			if (includeRenderers)
			{
				MeshFilter[] componentsInChildren = transform.GetComponentsInChildren<MeshFilter>(includeInactive);
				for (i = 0; i < (int)componentsInChildren.Length; i++)
				{
					MeshFilter meshFilter = componentsInChildren[i];
					if (meshFilter.sharedMesh)
					{
						Matrix4x4 matrix4x4 = transform.worldToLocalMatrix * meshFilter.transform.localToWorldMatrix;
						Bounds bound1 = meshFilter.sharedMesh.bounds;
						bound.Encapsulate(bound1.Transform(matrix4x4));
					}
				}
				SkinnedMeshRenderer[] skinnedMeshRendererArray = transform.GetComponentsInChildren<SkinnedMeshRenderer>(includeInactive);
				for (i = 0; i < (int)skinnedMeshRendererArray.Length; i++)
				{
					SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRendererArray[i];
					if (skinnedMeshRenderer.sharedMesh)
					{
						Matrix4x4 matrix4x41 = transform.worldToLocalMatrix * skinnedMeshRenderer.transform.localToWorldMatrix;
						Bounds bound2 = skinnedMeshRenderer.sharedMesh.bounds;
						bound.Encapsulate(bound2.Transform(matrix4x41));
					}
				}
			}
			if (includeColliders)
			{
				MeshCollider[] meshColliderArray = transform.GetComponentsInChildren<MeshCollider>(includeInactive);
				for (i = 0; i < (int)meshColliderArray.Length; i++)
				{
					MeshCollider meshCollider = meshColliderArray[i];
					if (meshCollider.sharedMesh && !meshCollider.isTrigger)
					{
						Matrix4x4 matrix4x42 = transform.worldToLocalMatrix * meshCollider.transform.localToWorldMatrix;
						Bounds bound3 = meshCollider.sharedMesh.bounds;
						bound.Encapsulate(bound3.Transform(matrix4x42));
					}
				}
			}
			return bound;
		}

		public static List<Transform> GetChildren(this Transform transform)
		{
			return transform.Cast<Transform>().ToList<Transform>();
		}

		public static Transform[] GetChildrenWithTag(this Transform transform, string strTag)
		{
			return (
				from x in transform.GetAllChildren()
				where x.CompareTag(strTag)
				select x).ToArray<Transform>();
		}

		public static T GetComponentInChildrenIncludeDisabled<T>(this Transform transform)
		where T : Component
		{
			T t;
			List<T> list = Pool.GetList<T>();
			transform.GetComponentsInChildren<T>(true, list);
			t = (list.Count > 0 ? list[0] : default(T));
			Pool.FreeList<T>(ref list);
			return t;
		}

		public static bool GetGroundInfo(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfo(transform.position, out pos, out normal, range, transform);
		}

		public static bool GetGroundInfoTerrainOnly(this Transform transform, out Vector3 pos, out Vector3 normal, float range = 100f)
		{
			return TransformUtil.GetGroundInfoTerrainOnly(transform.position, out pos, out normal, range);
		}

		public static string GetRecursiveName(this Transform transform, string strEndName = "")
		{
			string recursiveName = transform.name;
			if (!string.IsNullOrEmpty(strEndName))
			{
				recursiveName = string.Concat(recursiveName, "/", strEndName);
			}
			if (transform.parent != null)
			{
				recursiveName = transform.parent.GetRecursiveName(recursiveName);
			}
			return recursiveName;
		}

		public static List<T> GetSiblings<T>(this Transform transform, bool includeSelf = false)
		{
			List<T> ts = new List<T>();
			if (transform.parent == null)
			{
				return ts;
			}
			for (int i = 0; i < transform.parent.childCount; i++)
			{
				Transform child = transform.parent.GetChild(i);
				if (includeSelf || !(child == transform))
				{
					T component = child.GetComponent<T>();
					if (component != null)
					{
						ts.Add(component);
					}
				}
			}
			return ts;
		}

		public static void Identity(this GameObject go)
		{
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;
		}

		public static GameObject InstantiateChild(this GameObject go, GameObject prefab)
		{
			GameObject gameObject = Instantiate.GameObject(prefab, null);
			gameObject.transform.SetParent(go.transform, false);
			gameObject.Identity();
			return gameObject;
		}

		public static void OrderChildren(this Transform tx, Func<Transform, object> selector)
		{
			foreach (Transform transforms in tx.Cast<Transform>().OrderBy<Transform, object>(selector))
			{
				transforms.SetAsLastSibling();
			}
		}

		public static void RemoveComponent<T>(this Transform transform)
		where T : Component
		{
			T component = transform.GetComponent<T>();
			if (component == null)
			{
				return;
			}
			GameManager.Destroy(component, 0f);
		}

		public static void RetireAllChildren(this Transform transform, GameManager gameManager)
		{
			List<GameObject> list = Pool.GetList<GameObject>();
			foreach (Transform transforms in transform)
			{
				if (transforms.CompareTag("persist"))
				{
					continue;
				}
				list.Add(transforms.gameObject);
			}
			foreach (GameObject gameObject in list)
			{
				gameManager.Retire(gameObject);
			}
			Pool.FreeList<GameObject>(ref list);
		}

		public static void SetChildrenActive(this Transform transform, bool b)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				transform.GetChild(i).gameObject.SetActive(b);
			}
		}

		public static void SetHierarchyGroup(this Transform transform, string strRoot, bool groupActive = true, bool persistant = false)
		{
			transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		public static void SetLayerRecursive(this GameObject go, int Layer)
		{
			if (go.layer != Layer)
			{
				go.layer = Layer;
			}
			for (int i = 0; i < go.transform.childCount; i++)
			{
				go.transform.GetChild(i).gameObject.SetLayerRecursive(Layer);
			}
		}

		public static Bounds WorkoutRenderBounds(this Transform tx)
		{
			Bounds bound = new Bounds(Vector3.zero, Vector3.zero);
			Renderer[] componentsInChildren = tx.GetComponentsInChildren<Renderer>();
			for (int i = 0; i < (int)componentsInChildren.Length; i++)
			{
				Renderer renderer = componentsInChildren[i];
				if (!(renderer is ParticleSystemRenderer))
				{
					if (bound.center != Vector3.zero)
					{
						bound.Encapsulate(renderer.bounds);
					}
					else
					{
						bound = renderer.bounds;
					}
				}
			}
			return bound;
		}
	}
}