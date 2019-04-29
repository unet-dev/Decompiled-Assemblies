using Apex.AI;
using Apex.AI.Components;
using System;

namespace Rust.Ai.HTN
{
	public class HTNUtilityAiClient : UtilityAIClient
	{
		public HTNUtilityAiClient(Guid aiId, IContextProvider contextProvider) : base(aiId, contextProvider)
		{
		}

		public HTNUtilityAiClient(IUtilityAI ai, IContextProvider contextProvider) : base(ai, contextProvider)
		{
		}

		public void Initialize()
		{
			base.Start();
		}

		public void Kill()
		{
			base.Stop();
		}

		protected override void OnPause()
		{
		}

		protected override void OnResume()
		{
		}

		protected override void OnStart()
		{
		}

		protected override void OnStop()
		{
		}
	}
}