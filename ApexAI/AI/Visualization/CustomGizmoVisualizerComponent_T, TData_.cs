using Apex.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public abstract class CustomGizmoVisualizerComponent<T, TData> : CustomVisualizerComponent<T, TData>
	where T : class
	{
		protected CustomGizmoVisualizerComponent()
		{
		}

		private void DrawGizmoData(IAIContext context)
		{
			TData tDatum;
			if (this._data.TryGetValue(context, out tDatum))
			{
				this.DrawGizmos(tDatum);
			}
		}

		protected abstract void DrawGizmos(TData data);

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying || !base.enabled)
			{
				return;
			}
			base.DoDraw(new Action<IAIContext>(this.DrawGizmoData));
		}
	}
}