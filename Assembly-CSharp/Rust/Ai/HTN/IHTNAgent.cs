using System;
using UnityEngine;

namespace Rust.Ai.HTN
{
	public interface IHTNAgent
	{
		BaseNpcDefinition AiDefinition
		{
			get;
		}

		HTNDomain AiDomain
		{
			get;
		}

		BaseEntity Body
		{
			get;
		}

		Vector3 BodyPosition
		{
			get;
		}

		Vector3 estimatedVelocity
		{
			get;
		}

		Vector3 EyePosition
		{
			get;
		}

		Quaternion EyeRotation
		{
			get;
		}

		BaseNpc.AiStatistics.FamilyEnum Family
		{
			get;
		}

		float healthFraction
		{
			get;
		}

		bool IsDestroyed
		{
			get;
		}

		bool IsDormant
		{
			get;
			set;
		}

		BaseEntity MainTarget
		{
			get;
		}

		Transform transform
		{
			get;
		}
	}
}