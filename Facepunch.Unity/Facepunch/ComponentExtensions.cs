using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class ComponentExtensions
	{
		public static T Duplicate<T>(this T obj)
		where T : Component
		{
			return UnityEngine.Object.Instantiate<GameObject>(obj.gameObject, obj.transform.parent).GetComponent<T>();
		}

		public static T[] Duplicate<T>(this T obj, int amount, bool includeOriginalInArray = false)
		where T : Component
		{
			if (includeOriginalInArray)
			{
				amount++;
			}
			T[] tArray = new T[amount];
			for (int i = 0; i < amount; i++)
			{
				if (!includeOriginalInArray || i != 0)
				{
					tArray[i] = obj.Duplicate<T>();
				}
				else
				{
					tArray[i] = obj;
				}
			}
			return tArray;
		}

		public static bool GetComponent<T, U>(this T obj, out U value)
		where T : Component
		{
			value = obj.GetComponent<U>();
			return value != null;
		}

		public static bool GetComponentInChildren<T, U>(this T obj, out U value)
		where T : Component
		{
			value = obj.GetComponentInChildren<U>();
			return value != null;
		}

		public static bool GetComponentInParent<T, U>(this T obj, out U value)
		where T : Component
		{
			value = obj.GetComponentInParent<U>();
			return value != null;
		}

		public static Rect GetWorldRect<T>(this T obj)
		where T : Component
		{
			Vector3[] vector3Array = new Vector3[4];
			((RectTransform)obj.transform).GetWorldCorners(vector3Array);
			return new Rect(vector3Array[0], vector3Array[2] - vector3Array[0]);
		}

		public static void SetActive<T>(this T obj, bool active)
		where T : Component
		{
			obj.gameObject.SetActive(active);
		}

		public static Vector2 WorldToRectTransform<T>(this T obj, Vector2 worldPos)
		where T : Component
		{
			Rect worldRect = obj.GetWorldRect<T>();
			worldPos.x = (worldPos.x - worldRect.xMin) / worldRect.width;
			worldPos.y = (worldPos.y - worldRect.yMin) / worldRect.height;
			return worldPos;
		}
	}
}