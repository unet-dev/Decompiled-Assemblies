using Apex.AI;
using Apex.LoadBalancing;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.AI.Components
{
	public sealed class LoadBalancedUtilityAIClient : UtilityAIClient, ILoadBalanced
	{
		private ILoadBalancedHandle _lbHandle;

		bool Apex.LoadBalancing.ILoadBalanced.repeat
		{
			get
			{
				return true;
			}
		}

		public float executionIntervalMax
		{
			get;
			set;
		}

		public float executionIntervalMin
		{
			get;
			set;
		}

		public float startDelayMax
		{
			get;
			set;
		}

		public float startDelayMin
		{
			get;
			set;
		}

		public LoadBalancedUtilityAIClient(Guid aiId, IContextProvider contextProvider) : this(aiId, contextProvider, 1f, 1f, 0f, 0f)
		{
		}

		public LoadBalancedUtilityAIClient(IUtilityAI ai, IContextProvider contextProvider) : this(ai, contextProvider, 1f, 1f, 0f, 0f)
		{
		}

		public LoadBalancedUtilityAIClient(Guid aiId, IContextProvider contextProvider, float executionIntervalMin, float executionIntervalMax, float startDelayMin, float startDelayMax) : base(aiId, contextProvider)
		{
			this.executionIntervalMin = executionIntervalMin;
			this.executionIntervalMax = executionIntervalMax;
			this.startDelayMin = startDelayMin;
			this.startDelayMax = startDelayMax;
		}

		public LoadBalancedUtilityAIClient(IUtilityAI ai, IContextProvider contextProvider, float executionIntervalMin, float executionIntervalMax, float startDelayMin, float startDelayMax) : base(ai, contextProvider)
		{
			this.executionIntervalMin = executionIntervalMin;
			this.executionIntervalMax = executionIntervalMax;
			this.startDelayMin = startDelayMin;
			this.startDelayMax = startDelayMax;
		}

		float? Apex.LoadBalancing.ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
		{
			base.Execute();
			if (this.executionIntervalMin == this.executionIntervalMax)
			{
				return null;
			}
			return new float?(UnityEngine.Random.Range(this.executionIntervalMin, this.executionIntervalMax));
		}

		protected override void OnPause()
		{
			if (this._lbHandle != null)
			{
				this._lbHandle.Pause();
			}
		}

		protected override void OnResume()
		{
			if (this._lbHandle != null)
			{
				this._lbHandle.Resume();
			}
		}

		protected override void OnStart()
		{
			float single = this.startDelayMin;
			if (single != this.startDelayMax)
			{
				single = UnityEngine.Random.Range(single, this.startDelayMax);
			}
			float single1 = this.executionIntervalMin;
			if (single1 != this.executionIntervalMax)
			{
				single1 = UnityEngine.Random.Range(single1, this.executionIntervalMax);
			}
			this._lbHandle = AILoadBalancer.aiLoadBalancer.Add(this, single1, single);
		}

		protected override void OnStop()
		{
			if (this._lbHandle != null)
			{
				this._lbHandle.Stop();
				this._lbHandle = null;
			}
		}
	}
}