using Apex.AI;
using Apex.Ai.HTN;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rust.Ai.HTN
{
	public abstract class BaseNpcContext : IHTNContext, IAIContext
	{
		public static List<Item> InventoryLookupCache;

		public abstract List<AnimalInfo> AnimalsInRange
		{
			get;
		}

		public abstract Dictionary<Guid, Stack<IEffect>> AppliedEffects
		{
			get;
			set;
		}

		public abstract Dictionary<Guid, Stack<IEffect>> AppliedExpectedEffects
		{
			get;
			set;
		}

		public abstract BaseNpcMemory BaseMemory
		{
			get;
		}

		public abstract Vector3 BodyPosition
		{
			get;
		}

		public abstract PrimitiveTaskSelector CurrentTask
		{
			get;
			set;
		}

		public abstract List<PrimitiveTaskSelector> DebugPlan
		{
			get;
		}

		public abstract int DecompositionScore
		{
			get;
			set;
		}

		public abstract List<NpcPlayerInfo> EnemyPlayersAudible
		{
			get;
		}

		public abstract List<NpcPlayerInfo> EnemyPlayersInLineOfSight
		{
			get;
		}

		public abstract List<NpcPlayerInfo> EnemyPlayersInRange
		{
			get;
		}

		public abstract Stack<PrimitiveTaskSelector> HtnPlan
		{
			get;
			set;
		}

		public abstract bool IsWorldStateDirty
		{
			get;
			set;
		}

		public abstract NpcOrientation OrientationType
		{
			get;
			set;
		}

		public abstract PlanResultType PlanResult
		{
			get;
			set;
		}

		public abstract PlanStateType PlanState
		{
			get;
			set;
		}

		public abstract List<NpcPlayerInfo> PlayersInRange
		{
			get;
		}

		public abstract List<NpcPlayerInfo> PlayersOutsideDetectionRange
		{
			get;
		}

		public abstract byte[] PreviousWorldState
		{
			get;
		}

		public abstract NpcPlayerInfo PrimaryEnemyPlayerAudible
		{
			get;
			set;
		}

		public abstract NpcPlayerInfo PrimaryEnemyPlayerInLineOfSight
		{
			get;
			set;
		}

		public abstract byte[] WorldState
		{
			get;
		}

		public abstract Stack<WorldStateInfo>[] WorldStateChanges
		{
			get;
		}

		static BaseNpcContext()
		{
			BaseNpcContext.InventoryLookupCache = new List<Item>(10);
		}

		protected BaseNpcContext()
		{
		}

		public abstract Vector3 GetDirectionAudibleTarget();

		public abstract Vector3 GetDirectionLastAttackedDir();

		public abstract Vector3 GetDirectionLookAround();

		public abstract Vector3 GetDirectionToAnimal();

		public abstract Vector3 GetDirectionToMemoryOfPrimaryEnemyPlayerTarget();

		public abstract Vector3 GetDirectionToPrimaryEnemyPlayerTargetBody();

		public abstract Vector3 GetDirectionToPrimaryEnemyPlayerTargetHead();

		public abstract byte GetFact(byte fact);

		public abstract NpcPlayerInfo GetPrimaryEnemyPlayerTarget();

		public byte GetWorldState(byte fact)
		{
			int num = fact;
			byte worldState = this.WorldState[num];
			if (this.WorldStateChanges[num].Count > 0)
			{
				worldState = this.WorldStateChanges[num].Peek().Value;
			}
			return worldState;
		}

		public abstract bool HasPrimaryEnemyPlayerTarget();

		public virtual void ResetState()
		{
		}

		public abstract void SetFact(byte fact, byte value, bool invokeChangedEvent = true, bool setAsDirty = true, bool checkValueDiff = true);

		public abstract void StartDomainDecomposition();
	}
}