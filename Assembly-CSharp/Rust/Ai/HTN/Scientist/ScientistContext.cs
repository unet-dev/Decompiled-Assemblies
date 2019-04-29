using Apex.Ai.HTN;
using Rust.Ai;
using Rust.Ai.HTN;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Rust.Ai.HTN.Scientist
{
	[Serializable]
	public class ScientistContext : BaseNpcContext, IDisposable
	{
		[ReadOnly]
		[SerializeField]
		public bool _isWorldStateDirty;

		[SerializeField]
		private byte[] _worldState;

		[ReadOnly]
		[SerializeField]
		private byte[] _previousWorldState;

		[ReadOnly]
		[SerializeField]
		private int _decompositionScore;

		[ReadOnly]
		[SerializeField]
		private List<PrimitiveTaskSelector> _debugPlan;

		private static Stack<WorldStateInfo>[] _worldStateChanges;

		public ScientistContext.WorldStateChangedEvent OnWorldStateChangedEvent;

		[ReadOnly]
		public bool HasVisitedLastKnownEnemyPlayerLocation;

		[ReadOnly]
		public HTNPlayer Body;

		[ReadOnly]
		public ScientistDomain Domain;

		[ReadOnly]
		public ScientistMemory Memory;

		[ReadOnly]
		public AiLocationManager Location;

		[Header("Cover")]
		[ReadOnly]
		public CoverPointVolume CoverVolume;

		[ReadOnly]
		public List<CoverPoint> CoverPoints = new List<CoverPoint>();

		[ReadOnly]
		public CoverPoint BestFlankCover;

		[ReadOnly]
		public CoverPoint BestAdvanceCover;

		[ReadOnly]
		public CoverPoint BestRetreatCover;

		[ReadOnly]
		public CoverPoint ReservedCoverPoint;

		[ReadOnly]
		public float ReservedCoverTime;

		public override List<AnimalInfo> AnimalsInRange { get; } = new List<AnimalInfo>();

		public override Dictionary<Guid, Stack<IEffect>> AppliedEffects { get; set; } = new Dictionary<Guid, Stack<IEffect>>();

		public override Dictionary<Guid, Stack<IEffect>> AppliedExpectedEffects { get; set; } = new Dictionary<Guid, Stack<IEffect>>();

		public override BaseNpcMemory BaseMemory
		{
			get
			{
				return this.Memory;
			}
		}

		public override Vector3 BodyPosition
		{
			get
			{
				return this.Body.transform.position;
			}
		}

		public CoverPoint ClosestCover
		{
			get
			{
				if (this.CoverPoints.Count <= 0)
				{
					return null;
				}
				return this.CoverPoints[0];
			}
		}

		public override PrimitiveTaskSelector CurrentTask
		{
			get;
			set;
		}

		public override List<PrimitiveTaskSelector> DebugPlan
		{
			get
			{
				return this._debugPlan;
			}
		}

		public override int DecompositionScore
		{
			get
			{
				return this._decompositionScore;
			}
			set
			{
				this._decompositionScore = value;
			}
		}

		public override List<NpcPlayerInfo> EnemyPlayersAudible { get; } = new List<NpcPlayerInfo>();

		public override List<NpcPlayerInfo> EnemyPlayersInLineOfSight { get; } = new List<NpcPlayerInfo>();

		public override List<NpcPlayerInfo> EnemyPlayersInRange { get; } = new List<NpcPlayerInfo>();

		public override Stack<PrimitiveTaskSelector> HtnPlan { get; set; } = new Stack<PrimitiveTaskSelector>();

		public override bool IsWorldStateDirty
		{
			get
			{
				return this._isWorldStateDirty;
			}
			set
			{
				this._isWorldStateDirty = value;
			}
		}

		public override NpcOrientation OrientationType
		{
			get;
			set;
		}

		public override PlanResultType PlanResult
		{
			get;
			set;
		}

		public override PlanStateType PlanState
		{
			get;
			set;
		}

		public override List<NpcPlayerInfo> PlayersInRange { get; } = new List<NpcPlayerInfo>();

		public override List<NpcPlayerInfo> PlayersOutsideDetectionRange { get; } = new List<NpcPlayerInfo>();

		public override byte[] PreviousWorldState
		{
			get
			{
				return this._previousWorldState;
			}
		}

		public override NpcPlayerInfo PrimaryEnemyPlayerAudible
		{
			get;
			set;
		}

		public override NpcPlayerInfo PrimaryEnemyPlayerInLineOfSight
		{
			get;
			set;
		}

		public override byte[] WorldState
		{
			get
			{
				return this._worldState;
			}
		}

		public override Stack<WorldStateInfo>[] WorldStateChanges
		{
			get
			{
				return ScientistContext._worldStateChanges;
			}
		}

		public ScientistContext(HTNPlayer body, ScientistDomain domain)
		{
			int length = Enum.GetValues(typeof(Facts)).Length;
			if (this._worldState == null || (int)this._worldState.Length != length)
			{
				this._worldState = new byte[length];
				this._previousWorldState = new byte[length];
				if (ScientistContext._worldStateChanges == null)
				{
					ScientistContext._worldStateChanges = new Stack<WorldStateInfo>[length];
					for (int i = 0; i < length; i++)
					{
						ScientistContext._worldStateChanges[i] = new Stack<WorldStateInfo>(5);
					}
				}
			}
			this._decompositionScore = 2147483647;
			this.Body = body;
			this.Domain = domain;
			this.PlanState = PlanStateType.NoPlan;
			if (this.Memory == null || this.Memory.ScientistContext != this)
			{
				this.Memory = new ScientistMemory(this);
			}
			if (this.Location == null)
			{
				float single = Single.MaxValue;
				foreach (AiLocationManager manager in AiLocationManager.Managers)
				{
					Vector3 vector3 = manager.transform.position - this.Body.transform.position;
					float single1 = vector3.sqrMagnitude;
					if (single1 >= single)
					{
						continue;
					}
					single = single1;
					this.Location = manager;
				}
			}
		}

		public void Dispose()
		{
		}

		public override Vector3 GetDirectionAudibleTarget()
		{
			NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo();
			foreach (NpcPlayerInfo enemyPlayersAudible in this.EnemyPlayersAudible)
			{
				if (enemyPlayersAudible.AudibleScore <= npcPlayerInfo.AudibleScore)
				{
					continue;
				}
				npcPlayerInfo = enemyPlayersAudible;
			}
			if (npcPlayerInfo.Player == null)
			{
				return this.Body.transform.forward;
			}
			Vector3 vector3 = npcPlayerInfo.Player.CenterPoint();
			Vector3 vector31 = this.Body.CenterPoint();
			Vector3 vector32 = (vector3 - vector31).normalized;
			return this.Domain.ModifyFirearmAim(vector32, vector3, vector31, 1f);
		}

		public override Vector3 GetDirectionLastAttackedDir()
		{
			if (this.Body.lastAttacker != null)
			{
				return this.Body.LastAttackedDir;
			}
			return this.Body.transform.forward;
		}

		public override Vector3 GetDirectionLookAround()
		{
			BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = this.Memory.PrimaryKnownEnemyPlayer;
			if (primaryKnownEnemyPlayer.PlayerInfo.Player == null)
			{
				return this.Body.transform.forward;
			}
			Vector3 vector3 = primaryKnownEnemyPlayer.PlayerInfo.Player.CenterPoint() - this.Body.CenterPoint();
			return vector3.normalized;
		}

		public override Vector3 GetDirectionToAnimal()
		{
			AnimalInfo primaryKnownAnimal = this.Memory.PrimaryKnownAnimal;
			if (primaryKnownAnimal.Animal == null)
			{
				return this.Body.transform.forward;
			}
			Vector3 vector3 = Vector3.zero;
			Vector3 vector31 = primaryKnownAnimal.Animal.CenterPoint() + vector3;
			Vector3 vector32 = this.Body.CenterPoint();
			Vector3 vector33 = (vector31 - vector32).normalized;
			return this.Domain.ModifyFirearmAim(vector33, vector31, vector32, 1f);
		}

		public override Vector3 GetDirectionToMemoryOfPrimaryEnemyPlayerTarget()
		{
			BaseNpcMemory.EnemyPlayerInfo primaryKnownEnemyPlayer = this.Memory.PrimaryKnownEnemyPlayer;
			if (primaryKnownEnemyPlayer.PlayerInfo.Player == null)
			{
				return this.Body.transform.forward;
			}
			Vector3 vector3 = this.Body.CenterPoint();
			float body = vector3.y - this.Body.transform.position.y;
			Vector3 lastKnownPosition = (primaryKnownEnemyPlayer.LastKnownPosition + (primaryKnownEnemyPlayer.PlayerInfo.Player.transform.up * body)) - vector3;
			if (lastKnownPosition.sqrMagnitude < 2f)
			{
				return primaryKnownEnemyPlayer.LastKnownHeading;
			}
			return lastKnownPosition.normalized;
		}

		public override Vector3 GetDirectionToPrimaryEnemyPlayerTargetBody()
		{
			NpcPlayerInfo primaryEnemyPlayerTarget = this.GetPrimaryEnemyPlayerTarget();
			if (primaryEnemyPlayerTarget.Player == null)
			{
				return this.Body.transform.forward;
			}
			Vector3 duckOffset = Vector3.zero;
			if (primaryEnemyPlayerTarget.Player.IsDucked())
			{
				duckOffset = PlayerEyes.DuckOffset;
			}
			if (primaryEnemyPlayerTarget.Player.IsSleeping())
			{
				duckOffset = Vector3.down;
			}
			Vector3 vector3 = primaryEnemyPlayerTarget.Player.CenterPoint() + duckOffset;
			Vector3 vector31 = this.Body.CenterPoint();
			Vector3 vector32 = (vector3 - vector31).normalized;
			return this.Domain.ModifyFirearmAim(vector32, vector3, vector31, 1f);
		}

		public override Vector3 GetDirectionToPrimaryEnemyPlayerTargetHead()
		{
			NpcPlayerInfo primaryEnemyPlayerTarget = this.GetPrimaryEnemyPlayerTarget();
			if (primaryEnemyPlayerTarget.Player == null)
			{
				return this.Body.transform.forward;
			}
			Vector3 player = primaryEnemyPlayerTarget.Player.eyes.position;
			Vector3 vector3 = this.Body.CenterPoint();
			Vector3 vector31 = (player - vector3).normalized;
			return this.Domain.ModifyFirearmAim(vector31, player, vector3, 1f);
		}

		public byte GetFact(Facts fact)
		{
			return this._worldState[(int)fact];
		}

		public override byte GetFact(byte fact)
		{
			return this.GetFact((Facts)fact);
		}

		public byte GetPreviousFact(Facts fact)
		{
			return this._previousWorldState[(int)fact];
		}

		public override NpcPlayerInfo GetPrimaryEnemyPlayerTarget()
		{
			if (this.PrimaryEnemyPlayerInLineOfSight.Player != null)
			{
				return this.PrimaryEnemyPlayerInLineOfSight;
			}
			return new NpcPlayerInfo();
		}

		public byte GetWorldState(Facts fact)
		{
			return base.GetWorldState((byte)fact);
		}

		public override bool HasPrimaryEnemyPlayerTarget()
		{
			return this.GetPrimaryEnemyPlayerTarget().Player != null;
		}

		public void IncrementFact(Facts fact, int value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, this.GetFact(fact) + value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void IncrementFact(Facts fact, byte value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (int)(this.GetFact(fact) + value), invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public bool IsBodyAlive()
		{
			if (!(this.Body != null) || !(this.Body.transform != null) || this.Body.IsDestroyed || this.Body.IsDead())
			{
				return false;
			}
			return !this.Body.IsWounded();
		}

		public bool IsFact(Facts fact)
		{
			return this.GetFact(fact) > 0;
		}

		public byte PeekFactChangeDuringPlanning(Facts fact)
		{
			return this.PeekFactChangeDuringPlanning((byte)fact);
		}

		public byte PeekFactChangeDuringPlanning(byte fact)
		{
			int num = fact;
			if (ScientistContext._worldStateChanges[num].Count <= 0)
			{
				return (byte)0;
			}
			return ScientistContext._worldStateChanges[num].Peek().Value;
		}

		public void PopFactChangeDuringPlanning(Facts fact)
		{
			this.PopFactChangeDuringPlanning((byte)fact);
		}

		public void PopFactChangeDuringPlanning(byte fact)
		{
			int num = fact;
			if (ScientistContext._worldStateChanges[num].Count > 0)
			{
				ScientistContext._worldStateChanges[num].Pop();
			}
		}

		public void PushFactChangeDuringPlanning(Facts fact, CoverState value, bool temporary)
		{
			this.PushFactChangeDuringPlanning((byte)fact, (byte)value, temporary);
		}

		public void PushFactChangeDuringPlanning(Facts fact, ItemType value, bool temporary)
		{
			this.PushFactChangeDuringPlanning((byte)fact, (byte)value, temporary);
		}

		public void PushFactChangeDuringPlanning(Facts fact, FirearmOrders value, bool temporary)
		{
			this.PushFactChangeDuringPlanning((byte)fact, (byte)value, temporary);
		}

		public void PushFactChangeDuringPlanning(Facts fact, HealthState value, bool temporary)
		{
			this.PushFactChangeDuringPlanning((byte)fact, (byte)value, temporary);
		}

		public void PushFactChangeDuringPlanning(Facts fact, bool value, bool temporary)
		{
			object obj;
			Facts fact1 = fact;
			if (value)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			this.PushFactChangeDuringPlanning((byte)fact1, (byte)obj, temporary);
		}

		public void PushFactChangeDuringPlanning(Facts fact, int value, bool temporary)
		{
			this.PushFactChangeDuringPlanning((byte)fact, (byte)value, temporary);
		}

		public void PushFactChangeDuringPlanning(Facts fact, byte value, bool temporary)
		{
			this.PushFactChangeDuringPlanning((byte)fact, value, temporary);
		}

		public void PushFactChangeDuringPlanning(byte fact, byte value, bool temporary)
		{
			Stack<WorldStateInfo> worldStateInfos = ScientistContext._worldStateChanges[fact];
			WorldStateInfo worldStateInfo = new WorldStateInfo()
			{
				Value = value,
				Temporary = temporary
			};
			worldStateInfos.Push(worldStateInfo);
		}

		public void ReserveCoverPoint(CoverPoint coverPoint)
		{
			if (coverPoint == null || coverPoint.IsValidFor(this.Body))
			{
				if (this.ReservedCoverPoint != null && this.ReservedCoverPoint != coverPoint)
				{
					this.ReservedCoverPoint.ReservedFor = null;
				}
				this.ReservedCoverPoint = coverPoint;
				if (this.ReservedCoverPoint != null)
				{
					this.ReservedCoverPoint.ReservedFor = this.Body;
					this.ReservedCoverTime = Time.time;
				}
			}
		}

		public override void ResetState()
		{
			base.ResetState();
			this.Memory.ResetState();
			this.IsWorldStateDirty = false;
			this.PlanState = PlanStateType.NoPlan;
			this.PlanResult = PlanResultType.NoPlan;
			this.HtnPlan.Clear();
			this.AppliedEffects.Clear();
			this.AppliedExpectedEffects.Clear();
			this.DecompositionScore = 2147483647;
			this.CurrentTask = null;
			this.HasVisitedLastKnownEnemyPlayerLocation = false;
			this.OrientationType = NpcOrientation.Heading;
			this.PlayersInRange.Clear();
			this.EnemyPlayersInRange.Clear();
			this.EnemyPlayersAudible.Clear();
			this.EnemyPlayersInLineOfSight.Clear();
			NpcPlayerInfo npcPlayerInfo = new NpcPlayerInfo();
			this.PrimaryEnemyPlayerAudible = npcPlayerInfo;
			npcPlayerInfo = new NpcPlayerInfo();
			this.PrimaryEnemyPlayerInLineOfSight = npcPlayerInfo;
			this.Location = null;
			this.CoverPoints.Clear();
			for (int i = 0; i < (int)this._worldState.Length; i++)
			{
				this._worldState[i] = 0;
				this._previousWorldState[i] = 0;
			}
		}

		public void SetFact(Facts fact, EnemyRange value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, CoverState value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, CoverTactic value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, AmmoState value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, HealthState value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, ItemType value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, FireTactic value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, FirearmOrders value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, bool value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			object obj;
			Facts fact1 = fact;
			if (value)
			{
				obj = 1;
			}
			else
			{
				obj = null;
			}
			this.SetFact(fact1, (byte)obj, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, int value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact(fact, (byte)value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public void SetFact(Facts fact, byte value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			int num = (int)fact;
			if (checkValueDiff && this._worldState[num] == value)
			{
				return;
			}
			if (setAsDirty)
			{
				this.IsWorldStateDirty = true;
			}
			this._previousWorldState[num] = this._worldState[num];
			this._worldState[num] = value;
			if (invokeChangedEvent)
			{
				ScientistContext.WorldStateChangedEvent onWorldStateChangedEvent = this.OnWorldStateChangedEvent;
				if (onWorldStateChangedEvent == null)
				{
					return;
				}
				onWorldStateChangedEvent(this, fact, this._previousWorldState[num], value);
			}
		}

		public override void SetFact(byte fact, byte value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true)
		{
			this.SetFact((Facts)fact, value, invokeChangedEvent, setAsDirty, checkValueDiff);
		}

		public override void StartDomainDecomposition()
		{
		}

		public delegate void WorldStateChangedEvent(ScientistContext context, Facts fact, byte oldValue, byte newValue);
	}
}