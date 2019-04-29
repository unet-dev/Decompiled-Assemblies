using Apex.AI;
using System;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public abstract class ContextGizmoGUIVisualizerComponent : ContextVisualizerComponent
	{
		public bool drawGizmos = true;

		public bool drawGUI = true;

		protected ContextGizmoGUIVisualizerComponent()
		{
		}

		protected abstract void DrawGizmos(IAIContext context);

		protected abstract void DrawGUI(IAIContext context);

		private void OnDrawGizmos()
		{
			if (!Application.isPlaying || !this.drawGizmos || !base.enabled)
			{
				return;
			}
			ContextGizmoGUIVisualizerComponent contextGizmoGUIVisualizerComponent = this;
			base.DoDraw(new Action<IAIContext>(contextGizmoGUIVisualizerComponent.DrawGizmos));
		}

		private void OnGUI()
		{
			if (!Application.isPlaying || !this.drawGUI || !base.enabled)
			{
				return;
			}
			ContextGizmoGUIVisualizerComponent contextGizmoGUIVisualizerComponent = this;
			base.DoDraw(new Action<IAIContext>(contextGizmoGUIVisualizerComponent.DrawGUI));
		}
	}
}