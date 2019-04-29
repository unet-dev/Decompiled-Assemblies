using Apex.AI;
using System;

namespace Apex.AI.Components
{
	public interface IUtilityAIClient
	{
		IUtilityAI ai
		{
			get;
			set;
		}

		UtilityAIClientState state
		{
			get;
		}

		void Execute();

		void Pause();

		void Resume();

		void Start();

		void Stop();
	}
}