using Apex.AI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public abstract class CustomGUIVisualizerComponent<T, TData> : CustomVisualizerComponent<T, TData>
	where T : class
	{
		protected CustomGUIVisualizerComponent()
		{
		}

		protected abstract void DrawGUI(TData data);

		private void DrawGUIData(IAIContext context)
		{
			TData tDatum;
			if (this._data.TryGetValue(context, out tDatum))
			{
				this.DrawGUI(tDatum);
			}
		}

		private void OnGUI()
		{
			if (!Application.isPlaying || !base.enabled)
			{
				return;
			}
			base.DoDraw(new Action<IAIContext>(this.DrawGUIData));
		}
	}
}