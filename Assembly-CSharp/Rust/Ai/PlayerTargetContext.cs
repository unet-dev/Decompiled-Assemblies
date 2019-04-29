using Apex.AI;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public class PlayerTargetContext : IAIContext
	{
		public IAIAgent Self;

		public int CurrentOptionsIndex;

		public int PlayerCount;

		public BasePlayer[] Players;

		public Vector3[] Direction;

		public float[] Dot;

		public float[] DistanceSqr;

		public byte[] LineOfSight;

		public BasePlayer Target;

		public float Score;

		public int Index;

		public Vector3 LastKnownPosition;

		public PlayerTargetContext()
		{
		}

		public void Refresh(IAIAgent self, BasePlayer[] players, int playerCount)
		{
			this.Self = self;
			this.Players = players;
			this.PlayerCount = playerCount;
			this.Target = null;
			this.Score = 0f;
			this.Index = -1;
			this.LastKnownPosition = Vector3.zero;
		}
	}
}