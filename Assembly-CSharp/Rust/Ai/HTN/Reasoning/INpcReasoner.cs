using Rust.Ai.HTN;
using System;

namespace Rust.Ai.HTN.Reasoning
{
	public interface INpcReasoner
	{
		float LastTickTime
		{
			get;
			set;
		}

		float TickFrequency
		{
			get;
			set;
		}

		void Tick(IHTNAgent npc, float deltaTime, float time);
	}
}