using Rust.Ai.HTN;
using System;

namespace Rust.Ai.HTN.Sensors
{
	public interface INpcSensor
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