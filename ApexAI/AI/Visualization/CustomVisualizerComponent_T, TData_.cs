using Apex.AI;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Apex.AI.Visualization
{
	public abstract class CustomVisualizerComponent<T, TData> : ContextVisualizerComponent, ICustomVisualizer
	where T : class
	{
		protected Dictionary<IAIContext, TData> _data;

		protected bool registerForDerivedTypes
		{
			get;
			set;
		}

		protected CustomVisualizerComponent()
		{
		}

		void Apex.AI.Visualization.ICustomVisualizer.EntityUpdate(object aiEntity, IAIContext context, Guid aiId)
		{
			this._data[context] = this.GetDataForVisualization((T)(aiEntity as T), context, aiId);
		}

		protected override void Awake()
		{
			base.Awake();
			this._data = new Dictionary<IAIContext, TData>();
		}

		protected abstract TData GetDataForVisualization(T aiEntity, IAIContext context, Guid aiId);

		protected virtual void OnDisable()
		{
			VisualizationManager.UnregisterVisualizer<T>(this.registerForDerivedTypes);
		}

		protected virtual void OnEnable()
		{
			VisualizationManager.RegisterVisualizer<T>(this, this.registerForDerivedTypes);
		}
	}
}