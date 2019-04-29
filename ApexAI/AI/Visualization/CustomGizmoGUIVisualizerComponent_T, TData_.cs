using Apex.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public abstract class CustomGizmoGUIVisualizerComponent<T, TData> : CustomVisualizerComponent<T, TData>
	where T : class
	{
		public bool drawGizmos;

		public bool drawGUI;

		protected CustomGizmoGUIVisualizerComponent()
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

		protected abstract void DrawGUI(TData data);

		private void DrawGUIData(IAIContext context)
		{
			TData tDatum;
			if (this._data.TryGetValue(context, out tDatum))
			{
				this.DrawGUI(tDatum);
			}
		}

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying || !this.drawGizmos || !base.enabled)
			{
				return;
			}
			base.DoDraw(new Action<IAIContext>(this.DrawGizmoData));
		}

		private void OnGUI()
		{
			if (!Application.isPlaying || !this.drawGUI || !base.enabled)
			{
				return;
			}
			base.DoDraw(new Action<IAIContext>(this.DrawGUIData));
		}
	}
}