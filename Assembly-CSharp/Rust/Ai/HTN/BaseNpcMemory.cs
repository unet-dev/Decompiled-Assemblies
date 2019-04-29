using Oxide.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN
{
	[Serializable]
	public class BaseNpcMemory
	{
		[ReadOnly]
		public bool HasTargetDestination;

		[ReadOnly]
		public Vector3 TargetDestination;

		[ReadOnly]
		private readonly List<BaseNpcMemory.FailedDestinationInfo> _failedDestinationMemory = new List<BaseNpcMemory.FailedDestinationInfo>(10);

		[ReadOnly]
		public BaseNpcMemory.EnemyPlayerInfo PrimaryKnownEnemyPlayer;

		[ReadOnly]
		public List<BaseNpcMemory.EnemyPlayerInfo> KnownEnemyPlayers = new List<BaseNpcMemory.EnemyPlayerInfo>(10);

		[ReadOnly]
		public List<BaseNpcMemory.EntityOfInterestInfo> KnownEntitiesOfInterest = new List<BaseNpcMemory.EntityOfInterestInfo>(10);

		[ReadOnly]
		public List<BaseNpcMemory.EntityOfInterestInfo> KnownTimedExplosives = new List<BaseNpcMemory.EntityOfInterestInfo>(10);

		[ReadOnly]
		public AnimalInfo PrimaryKnownAnimal;

		[ReadOnly]
		public Vector3 LastClosestEdgeNormal;

		[NonSerialized]
		public BaseNpcContext NpcContext;

		public virtual BaseNpcDefinition Definition
		{
			get
			{
				return null;
			}
		}

		public BaseNpcMemory(BaseNpcContext context)
		{
			this.NpcContext = context;
		}

		public void AddFailedDestination(Vector3 destination)
		{
			for (int i = 0; i < this._failedDestinationMemory.Count; i++)
			{
				BaseNpcMemory.FailedDestinationInfo item = this._failedDestinationMemory[i];
				if ((item.Destination - destination).sqrMagnitude <= 0.1f)
				{
					item.Time = Time.time;
					this._failedDestinationMemory[i] = item;
					return;
				}
			}
			List<BaseNpcMemory.FailedDestinationInfo> failedDestinationInfos = this._failedDestinationMemory;
			BaseNpcMemory.FailedDestinationInfo failedDestinationInfo = new BaseNpcMemory.FailedDestinationInfo()
			{
				Time = Time.time,
				Destination = destination
			};
			failedDestinationInfos.Add(failedDestinationInfo);
		}

		public void Forget(float memoryTimeout)
		{
			float single = Time.time;
			for (int i = 0; i < this._failedDestinationMemory.Count; i++)
			{
				if (single - this._failedDestinationMemory[i].Time > memoryTimeout)
				{
					this._failedDestinationMemory.RemoveAt(i);
					i--;
				}
			}
			for (int j = 0; j < this.KnownEnemyPlayers.Count; j++)
			{
				BaseNpcMemory.EnemyPlayerInfo item = this.KnownEnemyPlayers[j];
				float time = single - item.Time;
				if (time > memoryTimeout)
				{
					this.KnownEnemyPlayers.RemoveAt(j);
					j--;
					if (item.PlayerInfo.Player != null)
					{
						this.OnForget(item.PlayerInfo.Player);
						if (this.PrimaryKnownEnemyPlayer.PlayerInfo.Player == item.PlayerInfo.Player)
						{
							this.ForgetPrimiaryEnemyPlayer();
						}
					}
				}
				else if (this.PrimaryKnownEnemyPlayer.PlayerInfo.Player == item.PlayerInfo.Player)
				{
					ref float audibleScore = ref this.PrimaryKnownEnemyPlayer.PlayerInfo.AudibleScore;
					audibleScore = audibleScore * (1f - time / memoryTimeout);
					ref float visibilityScore = ref this.PrimaryKnownEnemyPlayer.PlayerInfo.VisibilityScore;
					visibilityScore = visibilityScore * (1f - time / memoryTimeout);
				}
			}
			for (int k = 0; k < this.KnownEntitiesOfInterest.Count; k++)
			{
				if (single - this.KnownEntitiesOfInterest[k].Time > memoryTimeout)
				{
					this.KnownEntitiesOfInterest.RemoveAt(k);
					k--;
				}
			}
			if (this.PrimaryKnownAnimal.Animal != null && single - this.PrimaryKnownAnimal.Time > memoryTimeout)
			{
				this.PrimaryKnownAnimal.Animal = null;
			}
		}

		public void ForgetPrimiaryAnimal()
		{
			this.PrimaryKnownAnimal.Animal = null;
		}

		public void ForgetPrimiaryEnemyPlayer()
		{
			this.PrimaryKnownEnemyPlayer.PlayerInfo.Player = null;
		}

		public bool IsValid(Vector3 destination)
		{
			bool flag;
			List<BaseNpcMemory.FailedDestinationInfo>.Enumerator enumerator = this._failedDestinationMemory.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if ((enumerator.Current.Destination - destination).sqrMagnitude > 0.1f)
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		protected virtual void OnForget(BasePlayer player)
		{
		}

		protected virtual void OnSetPrimaryKnownEnemyPlayer(ref BaseNpcMemory.EnemyPlayerInfo info)
		{
			this.PrimaryKnownEnemyPlayer = info;
		}

		public void RememberEnemyPlayer(IHTNAgent npc, ref NpcPlayerInfo info, float time, float uncertainty = 0f, string debugStr = "ENEMY!")
		{
			Vector3 bodyPosition;
			if (info.Player == null || info.Player.transform == null || info.Player.IsDestroyed || info.Player.IsDead() || info.Player.IsWounded())
			{
				return;
			}
			if (Mathf.Approximately(info.SqrDistance, 0f))
			{
				bodyPosition = npc.BodyPosition - info.Player.transform.position;
				info.SqrDistance = bodyPosition.sqrMagnitude;
			}
			for (int i = 0; i < this.KnownEnemyPlayers.Count; i++)
			{
				BaseNpcMemory.EnemyPlayerInfo item = this.KnownEnemyPlayers[i];
				if (item.PlayerInfo.Player == info.Player)
				{
					item.PlayerInfo = info;
					if (uncertainty >= 0.05f)
					{
						Vector2 vector2 = UnityEngine.Random.insideUnitCircle * uncertainty;
						item.LastKnownLocalPosition = info.Player.transform.localPosition + new Vector3(vector2.x, 0f, vector2.y);
						bodyPosition = item.LastKnownPosition - this.NpcContext.BodyPosition;
						item.LastKnownLocalHeading = bodyPosition.normalized;
						item.BodyVisibleWhenLastNoticed = info.BodyVisible;
						item.HeadVisibleWhenLastNoticed = info.HeadVisible;
					}
					else
					{
						item.LastKnownLocalPosition = info.Player.transform.localPosition;
						bodyPosition = info.Player.GetLocalVelocity();
						item.LastKnownLocalHeading = bodyPosition.normalized;
						item.OurLastLocalPositionWhenLastSeen = npc.transform.localPosition;
						item.BodyVisibleWhenLastNoticed = info.BodyVisible;
						item.HeadVisibleWhenLastNoticed = info.HeadVisible;
					}
					item.Time = time;
					this.KnownEnemyPlayers[i] = item;
					if (this.PrimaryKnownEnemyPlayer.PlayerInfo.Player == info.Player)
					{
						this.PrimaryKnownEnemyPlayer = item;
					}
					return;
				}
			}
			List<BaseNpcMemory.EnemyPlayerInfo> knownEnemyPlayers = this.KnownEnemyPlayers;
			BaseNpcMemory.EnemyPlayerInfo enemyPlayerInfo = new BaseNpcMemory.EnemyPlayerInfo()
			{
				PlayerInfo = info,
				LastKnownLocalPosition = info.Player.transform.localPosition,
				Time = time
			};
			knownEnemyPlayers.Add(enemyPlayerInfo);
		}

		public void RememberEntityOfInterest(IHTNAgent npc, BaseEntity entityOfInterest, float time, string debugStr)
		{
			TimedExplosive timedExplosive = entityOfInterest as TimedExplosive;
			if (timedExplosive != null)
			{
				this.RememberTimedExplosives(npc, timedExplosive, time, "EXPLOSIVE!");
			}
			bool flag = false;
			for (int i = 0; i < this.KnownEntitiesOfInterest.Count; i++)
			{
				BaseNpcMemory.EntityOfInterestInfo item = this.KnownEntitiesOfInterest[i];
				if (item.Entity == null)
				{
					this.KnownEntitiesOfInterest.RemoveAt(i);
					i--;
				}
				else if (item.Entity == entityOfInterest)
				{
					item.Time = time;
					this.KnownEntitiesOfInterest[i] = item;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				List<BaseNpcMemory.EntityOfInterestInfo> knownEntitiesOfInterest = this.KnownEntitiesOfInterest;
				BaseNpcMemory.EntityOfInterestInfo entityOfInterestInfo = new BaseNpcMemory.EntityOfInterestInfo()
				{
					Entity = entityOfInterest,
					Time = time
				};
				knownEntitiesOfInterest.Add(entityOfInterestInfo);
			}
		}

		public void RememberPrimaryAnimal(BaseNpc animal)
		{
			if (Interface.CallHook("OnNpcPlayerTarget", this, animal) != null)
			{
				return;
			}
			for (int i = 0; i < this.NpcContext.AnimalsInRange.Count; i++)
			{
				AnimalInfo item = this.NpcContext.AnimalsInRange[i];
				if (item.Animal == animal)
				{
					this.PrimaryKnownAnimal = item;
					return;
				}
			}
		}

		public void RememberPrimaryEnemyPlayer(BasePlayer primaryTarget)
		{
			for (int i = 0; i < this.KnownEnemyPlayers.Count; i++)
			{
				BaseNpcMemory.EnemyPlayerInfo item = this.KnownEnemyPlayers[i];
				if (item.PlayerInfo.Player == primaryTarget)
				{
					this.OnSetPrimaryKnownEnemyPlayer(ref item);
					return;
				}
			}
		}

		public void RememberTimedExplosives(IHTNAgent npc, TimedExplosive explosive, float time, string debugStr)
		{
			bool flag = false;
			for (int i = 0; i < this.KnownTimedExplosives.Count; i++)
			{
				BaseNpcMemory.EntityOfInterestInfo item = this.KnownTimedExplosives[i];
				if (item.Entity == null)
				{
					this.KnownTimedExplosives.RemoveAt(i);
					i--;
				}
				else if (item.Entity == explosive)
				{
					item.Time = time;
					this.KnownTimedExplosives[i] = item;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				List<BaseNpcMemory.EntityOfInterestInfo> knownTimedExplosives = this.KnownTimedExplosives;
				BaseNpcMemory.EntityOfInterestInfo entityOfInterestInfo = new BaseNpcMemory.EntityOfInterestInfo()
				{
					Entity = explosive,
					Time = time
				};
				knownTimedExplosives.Add(entityOfInterestInfo);
			}
		}

		public virtual void ResetState()
		{
			this.HasTargetDestination = false;
			List<BaseNpcMemory.FailedDestinationInfo> failedDestinationInfos = this._failedDestinationMemory;
			if (failedDestinationInfos != null)
			{
				failedDestinationInfos.Clear();
			}
			else
			{
			}
			this.PrimaryKnownEnemyPlayer.PlayerInfo.Player = null;
			List<BaseNpcMemory.EnemyPlayerInfo> knownEnemyPlayers = this.KnownEnemyPlayers;
			if (knownEnemyPlayers != null)
			{
				knownEnemyPlayers.Clear();
			}
			else
			{
			}
			List<BaseNpcMemory.EntityOfInterestInfo> knownEntitiesOfInterest = this.KnownEntitiesOfInterest;
			if (knownEntitiesOfInterest != null)
			{
				knownEntitiesOfInterest.Clear();
			}
			else
			{
			}
			this.PrimaryKnownAnimal.Animal = null;
			this.LastClosestEdgeNormal = Vector3.zero;
		}

		public virtual bool ShouldRemoveOnPlayerForgetTimeout(float time, NpcPlayerInfo player)
		{
			float forgetInRangeTime;
			if (player.Player == null || player.Player.transform == null || player.Player.IsDestroyed || player.Player.IsDead() || player.Player.IsWounded())
			{
				return true;
			}
			float single = time;
			float single1 = player.Time;
			BaseNpcDefinition definition = this.Definition;
			if (definition != null)
			{
				forgetInRangeTime = definition.Memory.ForgetInRangeTime;
			}
			else
			{
				forgetInRangeTime = 0f;
			}
			if (single > single1 + forgetInRangeTime)
			{
				return true;
			}
			return false;
		}

		[Serializable]
		public struct EnemyPlayerInfo
		{
			public float Time;

			public NpcPlayerInfo PlayerInfo;

			public Vector3 LastKnownLocalPosition;

			public Vector3 LastKnownLocalHeading;

			public Vector3 OurLastLocalPositionWhenLastSeen;

			public bool BodyVisibleWhenLastNoticed;

			public bool HeadVisibleWhenLastNoticed;

			public Vector3 LastKnownHeading
			{
				get
				{
					if (this.PlayerInfo.Player != null)
					{
						BaseEntity parentEntity = this.PlayerInfo.Player.GetParentEntity();
						if (parentEntity != null)
						{
							return parentEntity.transform.TransformDirection(this.LastKnownLocalHeading);
						}
					}
					return this.LastKnownLocalHeading;
				}
			}

			public Vector3 LastKnownPosition
			{
				get
				{
					if (this.PlayerInfo.Player != null)
					{
						BaseEntity parentEntity = this.PlayerInfo.Player.GetParentEntity();
						if (parentEntity != null)
						{
							return parentEntity.transform.TransformPoint(this.LastKnownLocalPosition);
						}
					}
					return this.LastKnownLocalPosition;
				}
			}

			public Vector3 OurLastPositionWhenLastSeen
			{
				get
				{
					if (this.PlayerInfo.Player != null)
					{
						BaseEntity parentEntity = this.PlayerInfo.Player.GetParentEntity();
						if (parentEntity != null)
						{
							return parentEntity.transform.TransformPoint(this.OurLastLocalPositionWhenLastSeen);
						}
					}
					return this.OurLastLocalPositionWhenLastSeen;
				}
			}
		}

		[Serializable]
		public struct EntityOfInterestInfo
		{
			public float Time;

			public BaseEntity Entity;
		}

		[Serializable]
		private struct FailedDestinationInfo
		{
			public float Time;

			public Vector3 Destination;
		}
	}
}