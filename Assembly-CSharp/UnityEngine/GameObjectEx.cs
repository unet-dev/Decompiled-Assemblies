using Rust;
using Rust.Registry;
using System;
using System.Runtime.CompilerServices;

namespace UnityEngine
{
	public static class GameObjectEx
	{
		private static IEntity GetEntityFromComponent(GameObject go)
		{
			IEntity i;
			Transform transforms = go.transform;
			for (i = transforms.GetComponent<IEntity>(); i == null && transforms.parent != null; i = transforms.GetComponent<IEntity>())
			{
				transforms = transforms.parent;
			}
			if (i != null && !i.IsDestroyed)
			{
				return i;
			}
			return null;
		}

		private static IEntity GetEntityFromRegistry(GameObject go)
		{
			IEntity i;
			Transform transforms = go.transform;
			for (i = Entity.Get(transforms.gameObject); i == null && transforms.parent != null; i = Entity.Get(transforms.gameObject))
			{
				transforms = transforms.parent;
			}
			if (i != null && !i.IsDestroyed)
			{
				return i;
			}
			return null;
		}

		public static void SetHierarchyGroup(this GameObject obj, string strRoot, bool groupActive = true, bool persistant = false)
		{
			obj.transform.SetParent(HierarchyUtil.GetRoot(strRoot, groupActive, persistant).transform, true);
		}

		public static BaseEntity ToBaseEntity(this GameObject go)
		{
			IEntity entityFromRegistry = GameObjectEx.GetEntityFromRegistry(go);
			if (entityFromRegistry == null && !go.transform.gameObject.activeSelf)
			{
				entityFromRegistry = GameObjectEx.GetEntityFromComponent(go);
			}
			return entityFromRegistry as BaseEntity;
		}
	}
}