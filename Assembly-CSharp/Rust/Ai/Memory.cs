using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai
{
	public class Memory
	{
		public List<BaseEntity> Visible = new List<BaseEntity>();

		public List<Memory.SeenInfo> All = new List<Memory.SeenInfo>();

		public List<Memory.ExtendedInfo> AllExtended = new List<Memory.ExtendedInfo>();

		public Memory()
		{
		}

		public void AddDanger(Vector3 position, float amount)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (Mathf.Approximately(this.All[i].Position.x, position.x) && Mathf.Approximately(this.All[i].Position.y, position.y) && Mathf.Approximately(this.All[i].Position.z, position.z))
				{
					Memory.SeenInfo item = this.All[i];
					item.Danger = amount;
					this.All[i] = item;
					return;
				}
			}
			List<Memory.SeenInfo> all = this.All;
			Memory.SeenInfo seenInfo = new Memory.SeenInfo()
			{
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = amount
			};
			all.Add(seenInfo);
		}

		internal void Forget(float maxSecondsOld)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				float timestamp = Time.realtimeSinceStartup - this.All[i].Timestamp;
				if (timestamp > maxSecondsOld)
				{
					if (this.All[i].Entity != null)
					{
						this.Visible.Remove(this.All[i].Entity);
						int num = 0;
						while (num < this.AllExtended.Count)
						{
							if (this.AllExtended[num].Entity != this.All[i].Entity)
							{
								num++;
							}
							else
							{
								this.AllExtended.RemoveAt(num);
								break;
							}
						}
					}
					this.All.RemoveAt(i);
					i--;
				}
				else if (timestamp > 0f)
				{
					float single = timestamp / maxSecondsOld;
					if (this.All[i].Danger > 0f)
					{
						Memory.SeenInfo item = this.All[i];
						item.Danger -= single;
						this.All[i] = item;
					}
					if (timestamp >= 1f)
					{
						int num1 = 0;
						while (num1 < this.AllExtended.Count)
						{
							if (this.AllExtended[num1].Entity != this.All[i].Entity)
							{
								num1++;
							}
							else
							{
								Memory.ExtendedInfo extendedInfo = this.AllExtended[num1];
								extendedInfo.LineOfSight = 0;
								this.AllExtended[num1] = extendedInfo;
								break;
							}
						}
					}
				}
			}
			for (int j = 0; j < this.Visible.Count; j++)
			{
				if (this.Visible[j] == null)
				{
					this.Visible.RemoveAt(j);
					j--;
				}
			}
			for (int k = 0; k < this.AllExtended.Count; k++)
			{
				if (this.AllExtended[k].Entity == null)
				{
					this.AllExtended.RemoveAt(k);
					k--;
				}
			}
		}

		public Memory.ExtendedInfo GetExtendedInfo(BaseEntity entity)
		{
			Memory.ExtendedInfo extendedInfo;
			List<Memory.ExtendedInfo>.Enumerator enumerator = this.AllExtended.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Memory.ExtendedInfo current = enumerator.Current;
					if (current.Entity != entity)
					{
						continue;
					}
					extendedInfo = current;
					return extendedInfo;
				}
				return new Memory.ExtendedInfo();
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return extendedInfo;
		}

		public Memory.SeenInfo GetInfo(BaseEntity entity)
		{
			Memory.SeenInfo seenInfo;
			List<Memory.SeenInfo>.Enumerator enumerator = this.All.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Memory.SeenInfo current = enumerator.Current;
					if (current.Entity != entity)
					{
						continue;
					}
					seenInfo = current;
					return seenInfo;
				}
				return new Memory.SeenInfo();
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return seenInfo;
		}

		public Memory.SeenInfo GetInfo(Vector3 position)
		{
			Memory.SeenInfo seenInfo;
			List<Memory.SeenInfo>.Enumerator enumerator = this.All.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Memory.SeenInfo current = enumerator.Current;
					if ((current.Position - position).sqrMagnitude >= 1f)
					{
						continue;
					}
					seenInfo = current;
					return seenInfo;
				}
				return new Memory.SeenInfo();
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return seenInfo;
		}

		public Memory.SeenInfo Update(BaseEntity entity, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			return this.Update(entity, entity.ServerPosition, score, direction, dot, distanceSqr, lineOfSight, updateLastHurtUsTime, lastHurtUsTime, out extendedInfo);
		}

		public Memory.SeenInfo Update(BaseEntity entity, Vector3 position, float score, Vector3 direction, float dot, float distanceSqr, byte lineOfSight, bool updateLastHurtUsTime, float lastHurtUsTime, out Memory.ExtendedInfo extendedInfo)
		{
			Memory.ExtendedInfo extendedInfo1;
			extendedInfo = new Memory.ExtendedInfo();
			bool flag = false;
			int num = 0;
			while (num < this.AllExtended.Count)
			{
				if (this.AllExtended[num].Entity != entity)
				{
					num++;
				}
				else
				{
					Memory.ExtendedInfo item = this.AllExtended[num];
					item.Direction = direction;
					item.Dot = dot;
					item.DistanceSqr = distanceSqr;
					item.LineOfSight = lineOfSight;
					if (updateLastHurtUsTime)
					{
						item.LastHurtUsTime = lastHurtUsTime;
					}
					this.AllExtended[num] = item;
					extendedInfo = item;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (!updateLastHurtUsTime)
				{
					extendedInfo1 = new Memory.ExtendedInfo()
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight
					};
					Memory.ExtendedInfo extendedInfo2 = extendedInfo1;
					this.AllExtended.Add(extendedInfo2);
					extendedInfo = extendedInfo2;
				}
				else
				{
					extendedInfo1 = new Memory.ExtendedInfo()
					{
						Entity = entity,
						Direction = direction,
						Dot = dot,
						DistanceSqr = distanceSqr,
						LineOfSight = lineOfSight,
						LastHurtUsTime = lastHurtUsTime
					};
					Memory.ExtendedInfo extendedInfo3 = extendedInfo1;
					this.AllExtended.Add(extendedInfo3);
					extendedInfo = extendedInfo3;
				}
			}
			return this.Update(entity, position, score);
		}

		public Memory.SeenInfo Update(BaseEntity ent, float danger = 0f)
		{
			return this.Update(ent, ent.ServerPosition, danger);
		}

		public Memory.SeenInfo Update(BaseEntity ent, Vector3 position, float danger = 0f)
		{
			for (int i = 0; i < this.All.Count; i++)
			{
				if (this.All[i].Entity == ent)
				{
					Memory.SeenInfo item = this.All[i];
					item.Position = position;
					item.Timestamp = Time.realtimeSinceStartup;
					item.Danger += danger;
					this.All[i] = item;
					return item;
				}
			}
			Memory.SeenInfo seenInfo = new Memory.SeenInfo()
			{
				Entity = ent,
				Position = position,
				Timestamp = Time.realtimeSinceStartup,
				Danger = danger
			};
			Memory.SeenInfo seenInfo1 = seenInfo;
			this.All.Add(seenInfo1);
			this.Visible.Add(ent);
			return seenInfo1;
		}

		public struct ExtendedInfo
		{
			public BaseEntity Entity;

			public Vector3 Direction;

			public float Dot;

			public float DistanceSqr;

			public byte LineOfSight;

			public float LastHurtUsTime;
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