using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.AI
{
	public class SimpleAIMemory
	{
		public List<BaseEntity> Visible = new List<BaseEntity>();

		public List<SimpleAIMemory.SeenInfo> All = new List<SimpleAIMemory.SeenInfo>();

		public SimpleAIMemory()
		{
		}

		public void AddDanger(Vector3 position, float amount)
		{
			List<SimpleAIMemory.SeenInfo> all = this.All;
			SimpleAIMemory.SeenInfo seenInfo = new SimpleAIMemory.SeenInfo()
			{
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = amount
			};
			all.Add(seenInfo);
		}

		internal void Forget(float secondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (Time.realtimeSinceStartup - this.All[i].Timestamp > secondsOld)
				{
					if (this.All[i].Entity != null)
					{
						this.Visible.Remove(this.All[i].Entity);
					}
					this.All.RemoveAt(i);
					i--;
				}
			}
		}

		public void Update(BaseEntity ent)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					SimpleAIMemory.SeenInfo item = this.All[i];
					item.Position = ent.transform.position;
					item.Timestamp = Mathf.Max(Time.realtimeSinceStartup, item.Timestamp);
					this.All[i] = item;
					return;
				}
			}
			List<SimpleAIMemory.SeenInfo> all = this.All;
			SimpleAIMemory.SeenInfo seenInfo = new SimpleAIMemory.SeenInfo()
			{
				Entity = ent,
				Position = ent.transform.position,
				Timestamp = Time.realtimeSinceStartup
			};
			all.Add(seenInfo);
			this.Visible.Add(ent);
		}

		public struct SeenInfo
		{
			public BaseEntity Entity;

			public Vector3 Position;

			public float Timestamp;

			public float Danger;
		}
	}
}