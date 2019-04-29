using Rust.Ai.HTN;
using System;
using UnityEngine;

public abstract class BaseNpcDefinition : Definition<BaseNpcDefinition>
{
	[Header("Domain")]
	public HTNDomain Domain;

	[Header("Base Stats")]
	public BaseNpcDefinition.InfoStats Info;

	public BaseNpcDefinition.VitalStats Vitals;

	public BaseNpcDefinition.MovementStats Movement;

	public BaseNpcDefinition.SensoryStats Sensory;

	public BaseNpcDefinition.MemoryStats Memory;

	public BaseNpcDefinition.EngagementStats Engagement;

	protected BaseNpcDefinition()
	{
	}

	public virtual void Loadout(HTNPlayer target)
	{
	}

	public virtual void Loadout(HTNAnimal target)
	{
	}

	public virtual BaseCorpse OnCreateCorpse(HTNPlayer target)
	{
		return null;
	}

	public virtual BaseCorpse OnCreateCorpse(HTNAnimal target)
	{
		return null;
	}

	public virtual void OnlyLoadoutWeapons(HTNPlayer target)
	{
	}

	public virtual void StartVoices(HTNPlayer target)
	{
	}

	public virtual void StartVoices(HTNAnimal target)
	{
	}

	public virtual void StopVoices(HTNPlayer target)
	{
	}

	public virtual void StopVoices(HTNAnimal target)
	{
	}

	[Serializable]
	public class EngagementStats
	{
		public float CloseRange;

		public float MediumRange;

		public float LongRange;

		public float AggroRange;

		public float DeaggroRange;

		public float Hostility;

		public float Defensiveness;

		public float SqrAggroRange
		{
			get
			{
				return this.AggroRange * this.AggroRange;
			}
		}

		public float SqrCloseRange
		{
			get
			{
				return this.CloseRange * this.CloseRange;
			}
		}

		public float SqrDeaggroRange
		{
			get
			{
				return this.DeaggroRange * this.DeaggroRange;
			}
		}

		public float SqrLongRange
		{
			get
			{
				return this.LongRange * this.LongRange;
			}
		}

		public float SqrMediumRange
		{
			get
			{
				return this.MediumRange * this.MediumRange;
			}
		}

		public EngagementStats()
		{
		}

		public float CenterOfCloseRange()
		{
			return this.CloseRange * 0.5f;
		}

		public float CenterOfCloseRangeFirearm(AttackEntity ent)
		{
			return this.CloseRangeFirearm(ent) * 0.5f;
		}

		public float CenterOfMediumRange()
		{
			float mediumRange = this.MediumRange - this.CloseRange;
			return this.MediumRange - mediumRange * 0.5f;
		}

		public float CenterOfMediumRangeFirearm(AttackEntity ent)
		{
			float single = this.MediumRangeFirearm(ent);
			float single1 = single - this.CloseRangeFirearm(ent);
			return single - single1 * 0.5f;
		}

		public float CloseRangeFirearm(AttackEntity ent)
		{
			if (!ent)
			{
				return this.CloseRange;
			}
			return this.CloseRange + ent.CloseRangeAddition;
		}

		public float LongRangeFirearm(AttackEntity ent)
		{
			if (!ent)
			{
				return this.LongRange;
			}
			return this.LongRange + ent.LongRangeAddition;
		}

		public float MediumRangeFirearm(AttackEntity ent)
		{
			if (!ent)
			{
				return this.MediumRange;
			}
			return this.MediumRange + ent.MediumRangeAddition;
		}

		public float SqrCenterOfCloseRange()
		{
			float single = this.CenterOfCloseRange();
			return single * single;
		}

		public float SqrCenterOfCloseRangeFirearm(AttackEntity ent)
		{
			float single = this.CenterOfCloseRangeFirearm(ent);
			return single * single;
		}

		public float SqrCenterOfMediumRange()
		{
			float single = this.CenterOfMediumRange();
			return single * single;
		}

		public float SqrCenterOfMediumRangeFirearm(AttackEntity ent)
		{
			float single = this.CenterOfMediumRangeFirearm(ent);
			return single * single;
		}

		public float SqrCloseRangeFirearm(AttackEntity ent)
		{
			float single = this.CloseRangeFirearm(ent);
			return single * single;
		}

		public float SqrLongRangeFirearm(AttackEntity ent)
		{
			float single = this.LongRangeFirearm(ent);
			return single * single;
		}

		public float SqrMediumRangeFirearm(AttackEntity ent)
		{
			float single = this.MediumRangeFirearm(ent);
			return single * single;
		}
	}

	public enum Family
	{
		Player,
		Scientist,
		Murderer,
		Horse,
		Deer,
		Boar,
		Wolf,
		Bear,
		Chicken,
		Zombie
	}

	[Serializable]
	public class InfoStats
	{
		public BaseNpcDefinition.Family Family;

		public BaseNpcDefinition.Family[] Predators;

		public BaseNpcDefinition.Family[] Prey;

		public InfoStats()
		{
		}

		public BaseNpc.AiStatistics.FamilyEnum ToFamily(BaseNpcDefinition.Family family)
		{
			switch (family)
			{
				case BaseNpcDefinition.Family.Scientist:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Scientist;
				}
				case BaseNpcDefinition.Family.Murderer:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Murderer;
				}
				case BaseNpcDefinition.Family.Horse:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Horse;
				}
				case BaseNpcDefinition.Family.Deer:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Deer;
				}
				case BaseNpcDefinition.Family.Boar:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Boar;
				}
				case BaseNpcDefinition.Family.Wolf:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Wolf;
				}
				case BaseNpcDefinition.Family.Bear:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Bear;
				}
				case BaseNpcDefinition.Family.Chicken:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Chicken;
				}
				case BaseNpcDefinition.Family.Zombie:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Zombie;
				}
				default:
				{
					return BaseNpc.AiStatistics.FamilyEnum.Player;
				}
			}
		}
	}

	[Serializable]
	public class MemoryStats
	{
		public float ForgetTime;

		public float ForgetInRangeTime;

		public float NoSeeReturnToSpawnTime;

		public float ForgetAnimalInRangeTime;

		public MemoryStats()
		{
		}
	}

	[Serializable]
	public class MovementStats
	{
		public float DuckSpeed;

		public float WalkSpeed;

		public float RunSpeed;

		public float TurnSpeed;

		public float Acceleration;

		public MovementStats()
		{
		}
	}

	[Serializable]
	public class RoamStats
	{
		public float MaxRoamRange;

		public float MinRoamDelay;

		public float MaxRoamDelay;

		public float SqrMaxRoamRange
		{
			get
			{
				return this.MaxRoamRange * this.MaxRoamRange;
			}
		}

		public RoamStats()
		{
		}
	}

	[Serializable]
	public class SensoryStats
	{
		public float VisionRange;

		public float HearingRange;

		[Range(0f, 360f)]
		public float FieldOfView;

		private const float Inv180 = 0.00555555569f;

		public float FieldOfViewRadians
		{
			get
			{
				return (this.FieldOfView - 180f) * -0.00555555569f - 0.1f;
			}
		}

		public float SqrHearingRange
		{
			get
			{
				return this.HearingRange * this.HearingRange;
			}
		}

		public float SqrVisionRange
		{
			get
			{
				return this.VisionRange * this.VisionRange;
			}
		}

		public SensoryStats()
		{
		}
	}

	[Serializable]
	public class VitalStats
	{
		public float HP;

		public VitalStats()
		{
		}
	}
}