using Apex.AI;
using Rust.Ai;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public interface IAIAgent
{
	bool AgencyUpdateRequired
	{
		get;
		set;
	}

	int AgentTypeIndex
	{
		get;
		set;
	}

	Vector3 AttackPosition
	{
		get;
	}

	BaseEntity AttackTarget
	{
		get;
		set;
	}

	Memory.SeenInfo AttackTargetMemory
	{
		get;
		set;
	}

	float AttackTargetVisibleFor
	{
		get;
	}

	bool AutoBraking
	{
		get;
		set;
	}

	BaseCombatEntity CombatTarget
	{
		get;
	}

	Vector3 CrouchedAttackPosition
	{
		get;
	}

	Vector3 CurrentAimAngles
	{
		get;
	}

	float currentBehaviorDuration
	{
		get;
	}

	BaseNpc.Behaviour CurrentBehaviour
	{
		get;
		set;
	}

	Vector3 Destination
	{
		get;
		set;
	}

	BaseCombatEntity Entity
	{
		get;
	}

	BaseEntity FoodTarget
	{
		get;
		set;
	}

	float GetAttackCost
	{
		get;
	}

	Vector3 GetAttackOffset
	{
		get;
	}

	float GetAttackRange
	{
		get;
	}

	float GetAttackRate
	{
		get;
	}

	float GetEnergy
	{
		get;
	}

	float GetLastStuckTime
	{
		get;
	}

	NavMeshAgent GetNavAgent
	{
		get;
	}

	float GetSleep
	{
		get;
	}

	float GetStamina
	{
		get;
	}

	BaseNpc.AiStatistics GetStats
	{
		get;
	}

	float GetStuckDuration
	{
		get;
	}

	bool HasPath
	{
		get;
	}

	bool IsDormant
	{
		get;
		set;
	}

	bool IsOnOffmeshLinkAndReachedNewCoord
	{
		get;
		set;
	}

	bool IsStopped
	{
		get;
		set;
	}

	bool IsStuck
	{
		get;
	}

	Vector3 SpawnPosition
	{
		get;
		set;
	}

	float TargetSpeed
	{
		get;
		set;
	}

	float TimeAtDestination
	{
		get;
	}

	bool AttackReady();

	bool BusyTimerActive();

	void Eat();

	float FearLevel(BaseEntity ent);

	float GetActiveAggressionRangeSqr();

	IAIContext GetContext(Guid aiId);

	byte GetFact(BaseNpc.Facts fact);

	byte GetFact(NPCPlayerApex.Facts fact);

	float GetWantsToAttack(BaseEntity target);

	bool IsNavRunning();

	void Pause();

	List<NavPointSample> RequestNavPointSamplesInCircle(NavPointSampler.SampleCount sampleCount, float radius, NavPointSampler.SampleFeatures features = 0);

	List<NavPointSample> RequestNavPointSamplesInCircleWaterDepthOnly(NavPointSampler.SampleCount sampleCount, float radius, float waterDepth);

	void Resume();

	void SetBusyFor(float dur);

	void SetFact(BaseNpc.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true);

	void SetFact(NPCPlayerApex.Facts fact, byte value, bool triggerCallback = true, bool onlyTriggerCallbackOnDiffValue = true);

	void SetTargetPathStatus(float pendingDelay = 0.05f);

	void StartAttack();

	void StartAttack(AttackOperator.AttackType type, BaseCombatEntity target);

	void StopMoving();

	int TopologyPreference();

	float ToSpeed(BaseNpc.SpeedEnum speed);

	float ToSpeed(NPCPlayerApex.SpeedEnum speed);

	void UpdateDestination(Vector3 newDestination);

	void UpdateDestination(Transform tx);

	bool WantsToEat(BaseEntity eatable);
}