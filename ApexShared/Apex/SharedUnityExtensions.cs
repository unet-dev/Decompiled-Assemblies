using Apex.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex
{
	public static class SharedUnityExtensions
	{
		private readonly static Plane _xzPlane;

		static SharedUnityExtensions()
		{
			SharedUnityExtensions._xzPlane = new Plane(Vector3.up, Vector3.zero);
		}

		public static T AddComponentSafe<T>(this GameObject target, Action<T> configurator)
		where T : Component
		{
			target.SetActive(false);
			T t = target.AddComponent<T>();
			configurator(t);
			target.SetActive(true);
			return t;
		}

		public static bool AddIfMissing<T>(this GameObject target, bool entireScene, out T component)
		where T : Component
		{
			if (!entireScene)
			{
				component = target.GetComponent<T>();
			}
			else
			{
				component = ComponentHelper.FindFirstComponentInScene<T>();
			}
			if (component != null)
			{
				return false;
			}
			component = target.AddComponent<T>();
			return true;
		}

		public static bool AddIfMissing<T>(this GameObject target, bool entireScene)
		where T : Component
		{
			T t;
			return target.AddIfMissing<T>(entireScene, out t);
		}

		public static bool AddIfMissing<T>(this GameObject target, out T component)
		where T : Component
		{
			return target.AddIfMissing<T>(false, out component);
		}

		public static bool AddIfMissing<T>(this GameObject target)
		where T : Component
		{
			T t;
			return target.AddIfMissing<T>(false, out t);
		}

		public static bool Approximately(this Vector3 me, Vector3 other, float allowedDifference)
		{
			float single = me.x - other.x;
			if (single < -allowedDifference || single > allowedDifference)
			{
				return false;
			}
			float single1 = me.y - other.y;
			if (single1 < -allowedDifference || single1 > allowedDifference)
			{
				return false;
			}
			float single2 = me.z - other.z;
			if (single2 < -allowedDifference)
			{
				return false;
			}
			return single2 <= allowedDifference;
		}

		public static T As<T>(this Component c, bool searchParent = false, bool required = false)
		where T : class
		{
			if (c.Equals(null))
			{
				return default(T);
			}
			return c.gameObject.As<T>(searchParent, required);
		}

		public static T As<T>(this IGameObjectComponent c, bool searchParent = false, bool required = false)
		where T : class
		{
			if (c.Equals(null))
			{
				return default(T);
			}
			return c.gameObject.As<T>(searchParent, required);
		}

		public static T As<T>(this GameObject go, bool searchParent = false, bool required = false)
		where T : class
		{
			if (go.Equals(null))
			{
				return default(T);
			}
			T component = (T)(go.GetComponent(typeof(T)) as T);
			if (component == null & searchParent && go.transform.parent != null)
			{
				return go.transform.parent.gameObject.As<T>(false, required);
			}
			if (component == null & required)
			{
				throw new MissingComponentException(string.Format("Game object {0} does not have a component of type {1}.", go.name, typeof(T).Name));
			}
			return component;
		}

		public static bool Contains(this Rect rect, Rect other)
		{
			if (!rect.Contains(other.min))
			{
				return false;
			}
			return rect.Contains(other.max);
		}

		public static Bounds DeltaSize(this Bounds b, Vector3 delta)
		{
			b.size = b.size + delta;
			return b;
		}

		public static Bounds DeltaSize(this Bounds b, float dx, float dy, float dz)
		{
			Vector3 vector3 = b.size;
			vector3.x += dx;
			vector3.y += dy;
			vector3.z += dz;
			b.size = vector3;
			return b;
		}

		public static Vector3 DirToXZ(this Vector3 from, Vector3 to)
		{
			return new Vector3(to.x - from.x, 0f, to.z - from.z);
		}

		public static Collider GetColliderAtPosition(this Camera cam, Vector3 screenPos, LayerMask layerMask, float maxDistance = 1000f)
		{
			RaycastHit raycastHit;
			if (!Physics.Raycast(cam.ScreenPointToRay(screenPos), out raycastHit, maxDistance, layerMask))
			{
				return null;
			}
			return raycastHit.collider;
		}

		public static Bounds Intersection(this Bounds a, Bounds b)
		{
			Vector3 vector3 = new Vector3(Mathf.Max(a.min.x, b.min.x), Mathf.Max(a.min.y, b.min.y), Mathf.Max(a.min.z, b.min.z));
			Vector3 vector31 = new Vector3(Mathf.Min(a.max.x, b.max.x), Mathf.Min(a.max.y, b.max.y), Mathf.Min(a.max.z, b.max.z));
			Bounds bound = new Bounds();
			bound.SetMinMax(vector3, vector31);
			return bound;
		}

		public static Bounds Merge(this Bounds b, Bounds other)
		{
			Bounds bound = new Bounds()
			{
				min = new Vector3(Mathf.Min(b.min.x, other.min.x), Mathf.Min(b.min.y, other.min.y), Mathf.Min(b.min.z, other.min.z)),
				max = new Vector3(Mathf.Max(b.max.x, other.max.x), Mathf.Max(b.max.y, other.max.y), Mathf.Max(b.max.z, other.max.z))
			};
			return bound;
		}

		public static bool NukeSingle<T>(this GameObject go)
		where T : Component
		{
			T component = go.GetComponent<T>();
			if (component == null)
			{
				return false;
			}
			if (!Application.isPlaying)
			{
				UnityEngine.Object.DestroyImmediate(component, true);
			}
			else
			{
				UnityEngine.Object.Destroy(component);
			}
			return true;
		}

		public static Vector3 OnlyXZ(this Vector3 v)
		{
			v.y = 0f;
			return v;
		}

		public static bool Overlaps(this Bounds a, Bounds b)
		{
			if (b.max.x <= a.min.x || b.min.x >= a.max.x)
			{
				return false;
			}
			if (b.max.z <= a.min.z)
			{
				return false;
			}
			return b.min.z < a.max.z;
		}

		public static Rect Round(this Rect rect)
		{
			rect.xMin = Mathf.Round(rect.xMin);
			rect.xMax = Mathf.Round(rect.xMax);
			rect.yMin = Mathf.Round(rect.yMin);
			rect.yMax = Mathf.Round(rect.yMax);
			return rect;
		}

		public static Vector2 Round(this Vector2 v)
		{
			v.x = Mathf.Round(v.x);
			v.y = Mathf.Round(v.y);
			return v;
		}

		public static Vector3 Round(this Vector3 v)
		{
			v.x = Mathf.Round(v.x);
			v.y = Mathf.Round(v.y);
			v.z = Mathf.Round(v.z);
			return v;
		}

		public static Vector3 ScreenToGroundPoint(this Camera cam, Vector3 screenPos)
		{
			float single;
			Ray ray = cam.ScreenPointToRay(screenPos);
			if (!SharedUnityExtensions._xzPlane.Raycast(ray, out single))
			{
				return Vector3.zero;
			}
			return ray.GetPoint(single);
		}

		public static Vector3 ScreenToGroundPoint(this Camera cam, Vector3 screenPos, float groundHeight)
		{
			float single;
			Ray ray = cam.ScreenPointToRay(screenPos);
			Plane plane = new Plane(Vector3.up, new Vector3(0f, groundHeight, 0f));
			if (!plane.Raycast(ray, out single))
			{
				return Vector3.zero;
			}
			return ray.GetPoint(single);
		}

		public static bool ScreenToLayerHit(this Camera cam, Vector3 screenPos, LayerMask layerMask, float maxDistance, out RaycastHit hit)
		{
			return Physics.Raycast(cam.ScreenPointToRay(screenPos), out hit, maxDistance, layerMask);
		}

		public static void SelfAndDescendants(this GameObject root, List<GameObject> collector)
		{
			collector.Add(root);
			Transform transforms = root.transform;
			int num = transforms.childCount;
			for (int i = 0; i < num; i++)
			{
				transforms.GetChild(i).gameObject.SelfAndDescendants(collector);
			}
		}

		public static Bounds Translate(this Bounds b, Vector3 translation)
		{
			b.center = b.center + translation;
			return b;
		}

		public static Bounds Translate(this Bounds b, float x, float y, float z)
		{
			Vector3 vector3 = b.center;
			vector3.x += x;
			vector3.y += y;
			vector3.z += z;
			b.center = vector3;
			return b;
		}

		public static void WarnIfMultipleInstances(this MonoBehaviour component)
		{
			Type type = component.GetType();
			if ((int)component.GetComponents(type).Length > 1)
			{
				Debug.LogWarning(string.Format("GameObject '{0}' defines multiple instances of '{1}' which is not recommended.", component.gameObject.name, type.Name));
			}
		}

		public static void WarnIfMultipleInstances<TInterface>(this MonoBehaviour component)
		where TInterface : class
		{
			int num = 0;
			MonoBehaviour[] components = component.GetComponents<MonoBehaviour>();
			for (int i = 0; i < (int)components.Length; i++)
			{
				if ((TInterface)(components[i] as TInterface) != null)
				{
					num++;
				}
			}
			if (num > 1)
			{
				Debug.LogWarning(string.Format("GameObject '{0}' defines multiple component implementing '{1}' which is not recommended.", component.gameObject.name, typeof(TInterface).Name));
			}
		}
	}
}