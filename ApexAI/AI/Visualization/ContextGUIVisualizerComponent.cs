using Apex.AI;
using System;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public abstract class ContextGUIVisualizerComponent : ContextVisualizerComponent
	{
		protected ContextGUIVisualizerComponent()
		{
		}

		protected abstract void DrawGUI(IAIContext context);

		private void OnGUI()
		{
			if (!Application.isPlaying || !base.enabled)
			{
				return;
			}
			ContextGUIVisualizerComponent contextGUIVisualizerComponent = this;
			base.DoDraw(new Action<IAIContext>(contextGUIVisualizerComponent.DrawGUI));
		}
	}
}