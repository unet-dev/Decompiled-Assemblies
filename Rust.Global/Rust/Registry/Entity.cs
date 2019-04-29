using Rust;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Registry
{
	public static class Entity
	{
		private static Dictionary<GameObject, IEntity> _dict;

		static Entity()
		{
			Entity._dict = new Dictionary<GameObject, IEntity>();
		}

		public static IEntity Get(GameObject obj)
		{
			IEntity entity;
			if (Entity._dict.TryGetValue(obj, out entity))
			{
				return entity;
			}
			return null;
		}

		public static void Register(GameObject obj, IEntity entity)
		{
			Entity._dict[obj] = entity;
		}

		public static void Unregister(GameObject obj)
		{
			Entity._dict.Remove(obj);
		}
	}
}