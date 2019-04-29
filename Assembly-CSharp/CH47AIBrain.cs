using System;
using System.Collections.Generic;
using UnityEngine;

public class CH47AIBrain : BaseAIBrain<CH47HelicopterAIController>
{
	public const int CH47State_Idle = 1;

	public const int CH47State_Patrol = 2;

	public const int CH47State_Land = 3;

	public const int CH47State_Dropoff = 4;

	public const int CH47State_Orbit = 5;

	public const int CH47State_Retreat = 6;

	public const int CH47State_Egress = 7;

	private float age;

	public CH47AIBrain()
	{
	}

	public override void AIThink(float delta)
	{
		this.age += delta;
		base.AIThink(delta);
	}

	public void FixedUpdate()
	{
		if (base.baseEntity == null || base.baseEntity.isClient)
		{
			return;
		}
		this.AIThink(Time.fixedDeltaTime);
	}

	public override void InitializeAI()
	{
		base.InitializeAI();
		this.AIStates = new BaseAIBrain<CH47HelicopterAIController>.BasicAIState[8];
		this.AddState(new CH47AIBrain.IdleState(), 1);
		this.AddState(new CH47AIBrain.PatrolState(), 2);
		this.AddState(new CH47AIBrain.OrbitState(), 5);
		this.AddState(new CH47AIBrain.EgressState(), 7);
		this.AddState(new CH47AIBrain.DropCrate(), 4);
		this.AddState(new CH47AIBrain.LandState(), 3);
	}

	public void OnDrawGizmos()
	{
		BaseAIBrain<CH47HelicopterAIController>.BasicAIState currentState = base.GetCurrentState();
		if (currentState != null)
		{
			currentState.DrawGizmos();
		}
	}

	public class DropCrate : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
	{
		private float nextDropTime;

		public DropCrate()
		{
		}

		public bool CanDrop()
		{
			if (Time.time <= this.nextDropTime)
			{
				return false;
			}
			return this.brain.GetEntity().CanDropCrate();
		}

		public override bool CanInterrupt()
		{
			if (!base.CanInterrupt())
			{
				return false;
			}
			return !this.CanDrop();
		}

		public override float GetWeight()
		{
			if (!this.CanDrop())
			{
				return 0f;
			}
			CH47DropZone closest = CH47DropZone.GetClosest(this.brain.GetEntity().transform.position);
			if (closest == null || Vector3.Distance(closest.transform.position, this.brain.mainInterestPoint) > 200f)
			{
				return 0f;
			}
			if (base.IsInState())
			{
				return 10000f;
			}
			if (this.brain._currentState == 5 && this.brain.GetCurrentState().TimeInState() > 60f)
			{
				return 1000f;
			}
			return 0f;
		}

		public override void StateEnter()
		{
			this.brain.GetEntity().SetDropDoorOpen(true);
			this.brain.GetEntity().EnableFacingOverride(false);
			CH47DropZone closest = CH47DropZone.GetClosest(this.brain.GetEntity().transform.position);
			if (closest == null)
			{
				this.nextDropTime = Time.time + 60f;
			}
			this.brain.mainInterestPoint = closest.transform.position;
			this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
			base.StateEnter();
		}

		public override void StateLeave()
		{
			this.brain.GetEntity().SetDropDoorOpen(false);
			this.nextDropTime = Time.time + 60f;
			base.StateLeave();
		}

		public override void StateThink(float delta)
		{
			base.StateThink(delta);
			if (this.CanDrop() && Vector3Ex.Distance2D(this.brain.mainInterestPoint, this.brain.GetEntity().transform.position) < 5f && this.brain.GetEntity().rigidBody.velocity.magnitude < 5f)
			{
				this.brain.GetEntity().DropCrate();
				this.nextDropTime = Time.time + 120f;
			}
		}
	}

	public class EgressState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
	{
		private bool killing;

		public EgressState()
		{
		}

		public override bool CanInterrupt()
		{
			return false;
		}

		public override float GetWeight()
		{
			if (this.brain.GetEntity().OutOfCrates() && !this.brain.GetEntity().ShouldLand())
			{
				return 10000f;
			}
			CH47AIBrain component = this.brain.GetComponent<CH47AIBrain>();
			if (component == null)
			{
				return 0f;
			}
			if (component.age <= 600f)
			{
				return 0f;
			}
			return 10000f;
		}

		public override void StateEnter()
		{
			this.brain.GetEntity().EnableFacingOverride(false);
			Transform entity = this.brain.GetEntity().transform;
			Rigidbody rigidbody = this.brain.GetEntity().rigidBody;
			Vector3 vector3 = (rigidbody.velocity.magnitude < 0.1f ? entity.forward : rigidbody.velocity.normalized);
			Vector3 vector31 = Vector3.Cross(Vector3.Cross(entity.up, vector3), Vector3.up);
			this.brain.mainInterestPoint = entity.position + (vector31 * 8000f);
			this.brain.mainInterestPoint.y = 90f;
			this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
			base.StateEnter();
		}

		public override void StateLeave()
		{
			base.StateLeave();
		}

		public override void StateThink(float delta)
		{
			base.StateThink(delta);
			if (this.killing)
			{
				return;
			}
			this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
			if (base.TimeInState() > 300f)
			{
				this.brain.GetEntity().Invoke("DelayedKill", 2f);
				this.killing = true;
			}
		}
	}

	public class IdleState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
	{
		public IdleState()
		{
		}

		public override float GetWeight()
		{
			return 0.1f;
		}

		public override void StateEnter()
		{
			CH47HelicopterAIController entity = this.brain.GetEntity();
			Vector3 position = this.brain.GetEntity().GetPosition();
			Vector3 vector3 = this.brain.GetEntity().rigidBody.velocity;
			entity.SetMoveTarget(position + (vector3.normalized * 10f));
			base.StateEnter();
		}
	}

	public class LandState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
	{
		private float landedForSeconds;

		private float lastLandtime;

		private float landingHeight;

		private float nextDismountTime;

		public LandState()
		{
		}

		public override bool CanInterrupt()
		{
			return true;
		}

		public override float GetWeight()
		{
			if (!base.GetEntity().ShouldLand())
			{
				return 0f;
			}
			float single = Time.time - this.lastLandtime;
			if (base.IsInState() && this.landedForSeconds < 12f)
			{
				return 1000f;
			}
			if (!base.IsInState() && single > 10f)
			{
				return 9000f;
			}
			return 0f;
		}

		public override void StateEnter()
		{
			this.brain.mainInterestPoint = base.GetEntity().landingTarget;
			this.landingHeight = 15f;
			base.StateEnter();
		}

		public override void StateLeave()
		{
			this.brain.GetEntity().EnableFacingOverride(false);
			this.brain.GetEntity().SetAltitudeProtection(true);
			this.brain.GetEntity().SetMinHoverHeight(30f);
			this.landedForSeconds = 0f;
			base.StateLeave();
		}

		public override void StateThink(float delta)
		{
			bool flag;
			Vector3 entity = this.brain.GetEntity().transform.position;
			CH47LandingZone closest = CH47LandingZone.GetClosest(this.brain.GetEntity().landingTarget);
			if (!closest)
			{
				return;
			}
			float single = this.brain.GetEntity().rigidBody.velocity.magnitude;
			Vector3.Distance(closest.transform.position, entity);
			float single1 = Vector3Ex.Distance2D(closest.transform.position, entity);
			Mathf.InverseLerp(1f, 20f, single1);
			bool flag1 = single1 < 100f;
			bool flag2 = (single1 <= 15f ? false : entity.y < closest.transform.position.y + 10f);
			this.brain.GetEntity().EnableFacingOverride(flag1);
			this.brain.GetEntity().SetAltitudeProtection(flag2);
			flag = (Mathf.Abs(closest.transform.position.y - entity.y) >= 3f || single1 > 5f ? false : single < 1f);
			if (flag)
			{
				this.landedForSeconds += delta;
				if (this.lastLandtime == 0f)
				{
					this.lastLandtime = Time.time;
				}
			}
			float single2 = 1f - Mathf.InverseLerp(0f, 7f, single1);
			this.landingHeight = this.landingHeight - 4f * single2 * Time.deltaTime;
			if (this.landingHeight < -5f)
			{
				this.landingHeight = -5f;
			}
			this.brain.GetEntity().SetAimDirection(closest.transform.forward);
			this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint + new Vector3(0f, this.landingHeight, 0f));
			if (flag)
			{
				if (this.landedForSeconds > 1f && Time.time > this.nextDismountTime)
				{
					BaseVehicle.MountPointInfo[] mountPointInfoArray = this.brain.GetEntity().mountPoints;
					int num = 0;
					while (num < (int)mountPointInfoArray.Length)
					{
						BaseVehicle.MountPointInfo mountPointInfo = mountPointInfoArray[num];
						if (!mountPointInfo.mountable || !mountPointInfo.mountable.IsMounted())
						{
							num++;
						}
						else
						{
							this.nextDismountTime = Time.time + 0.5f;
							mountPointInfo.mountable.DismountAllPlayers();
							break;
						}
					}
				}
				if (this.landedForSeconds > 8f)
				{
					this.brain.GetComponent<CH47AIBrain>().age = Single.PositiveInfinity;
				}
			}
		}
	}

	public class OrbitState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
	{
		public OrbitState()
		{
		}

		public override bool CanInterrupt()
		{
			return base.CanInterrupt();
		}

		public Vector3 GetOrbitCenter()
		{
			return this.brain.mainInterestPoint;
		}

		public override float GetWeight()
		{
			if (base.IsInState())
			{
				float single = 1f - Mathf.InverseLerp(120f, 180f, base.TimeInState());
				return 5f * single;
			}
			if (this.brain._currentState == 2 && Vector3Ex.Distance2D(this.brain.mainInterestPoint, this.brain.GetEntity().GetPosition()) <= CH47AIBrain.PatrolState.patrolApproachDist * 1.1f)
			{
				return 5f;
			}
			return 0f;
		}

		public override void StateEnter()
		{
			this.brain.GetEntity().EnableFacingOverride(true);
			this.brain.GetEntity().InitiateAnger();
			base.StateEnter();
		}

		public override void StateLeave()
		{
			this.brain.GetEntity().EnableFacingOverride(false);
			this.brain.GetEntity().CancelAnger();
			base.StateLeave();
		}

		public override void StateThink(float delta)
		{
			Vector3 orbitCenter = this.GetOrbitCenter();
			CH47HelicopterAIController entity = this.brain.GetEntity();
			Vector3 position = entity.GetPosition();
			Vector3 vector3 = Vector3Ex.Direction2D(orbitCenter, position);
			Vector3 vector31 = Vector3.Cross(Vector3.up, vector3);
			float single = (Vector3.Dot(Vector3.Cross(entity.transform.right, Vector3.up), vector31) < 0f ? -1f : 1f);
			float single1 = 75f;
			Vector3 vector32 = -vector3 + ((vector31 * single) * 0.6f);
			Vector3 vector33 = orbitCenter + (vector32.normalized * single1);
			entity.SetMoveTarget(vector33);
			entity.SetAimDirection(Vector3Ex.Direction2D(vector33, position));
			base.StateThink(delta);
		}
	}

	public class PatrolState : BaseAIBrain<CH47HelicopterAIController>.BasicAIState
	{
		public List<Vector3> visitedPoints;

		public static float patrolApproachDist;

		public bool testing;

		static PatrolState()
		{
			CH47AIBrain.PatrolState.patrolApproachDist = 75f;
		}

		public PatrolState()
		{
		}

		public bool AtPatrolDestination()
		{
			return Vector3Ex.Distance2D(this.brain.mainInterestPoint, this.brain.GetEntity().GetPosition()) < CH47AIBrain.PatrolState.patrolApproachDist;
		}

		public override bool CanInterrupt()
		{
			if (!base.CanInterrupt())
			{
				return false;
			}
			return this.AtPatrolDestination();
		}

		public Vector3 GetRandomPatrolPoint()
		{
			Vector3 vector3 = Vector3.zero;
			MonumentInfo randomValidMonumentInfo = null;
			if (TerrainMeta.Path != null && TerrainMeta.Path.Monuments != null && TerrainMeta.Path.Monuments.Count > 0)
			{
				int count = TerrainMeta.Path.Monuments.Count;
				int num = UnityEngine.Random.Range(0, count);
				for (int i = 0; i < count; i++)
				{
					int num1 = i + num;
					if (num1 >= count)
					{
						num1 -= count;
					}
					MonumentInfo item = TerrainMeta.Path.Monuments[num1];
					if (item.Type != MonumentType.Cave && item.Type != MonumentType.WaterWell && item.Tier != MonumentTier.Tier0 && (int)(item.Tier & MonumentTier.Tier0) <= 0)
					{
						bool flag = false;
						foreach (Vector3 visitedPoint in this.visitedPoints)
						{
							if (Vector3Ex.Distance2D(item.transform.position, visitedPoint) >= 100f)
							{
								continue;
							}
							flag = true;
							goto Label0;
						}
					Label0:
						if (!flag)
						{
							randomValidMonumentInfo = item;
							break;
						}
					}
				}
				if (randomValidMonumentInfo == null)
				{
					this.visitedPoints.Clear();
					randomValidMonumentInfo = this.GetRandomValidMonumentInfo();
				}
			}
			if (randomValidMonumentInfo == null)
			{
				float size = TerrainMeta.Size.x;
				float single = 30f;
				vector3 = Vector3Ex.Range(-1f, 1f);
				vector3.y = 0f;
				vector3.Normalize();
				vector3 = vector3 * (size * UnityEngine.Random.Range(0f, 0.75f));
				vector3.y = single;
			}
			else
			{
				this.visitedPoints.Add(randomValidMonumentInfo.transform.position);
				vector3 = randomValidMonumentInfo.transform.position;
			}
			return vector3;
		}

		public MonumentInfo GetRandomValidMonumentInfo()
		{
			int count = TerrainMeta.Path.Monuments.Count;
			int num = UnityEngine.Random.Range(0, count);
			for (int i = 0; i < count; i++)
			{
				int num1 = i + num;
				if (num1 >= count)
				{
					num1 -= count;
				}
				MonumentInfo item = TerrainMeta.Path.Monuments[num1];
				if (item.Type != MonumentType.Cave && item.Type != MonumentType.WaterWell && item.Tier != MonumentTier.Tier0)
				{
					return item;
				}
			}
			return null;
		}

		public override float GetWeight()
		{
			if (base.IsInState())
			{
				if (this.AtPatrolDestination() && base.TimeInState() > 2f)
				{
					return 0f;
				}
				return 3f;
			}
			float single = Mathf.InverseLerp(30f, 120f, base.TimeSinceState()) * 5f;
			return 1f + single;
		}

		public override void StateEnter()
		{
			RaycastHit raycastHit;
			base.StateEnter();
			Vector3 randomPatrolPoint = this.GetRandomPatrolPoint();
			this.brain.mainInterestPoint = randomPatrolPoint;
			float single = Mathf.Max(TerrainMeta.WaterMap.GetHeight(randomPatrolPoint), TerrainMeta.HeightMap.GetHeight(randomPatrolPoint));
			float single1 = single;
			if (Physics.SphereCast(randomPatrolPoint + new Vector3(0f, 200f, 0f), 20f, Vector3.down, out raycastHit, 300f, 1218511105))
			{
				single1 = Mathf.Max(raycastHit.point.y, single);
			}
			this.brain.mainInterestPoint.y = single1 + 30f;
		}

		public override void StateThink(float delta)
		{
			base.StateThink(delta);
			this.brain.GetEntity().SetMoveTarget(this.brain.mainInterestPoint);
		}
	}
}