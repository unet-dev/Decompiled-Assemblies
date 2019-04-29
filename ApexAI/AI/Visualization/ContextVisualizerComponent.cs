using Apex.AI;
using Apex.AI.Components;
using Apex.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public abstract class ContextVisualizerComponent : MonoBehaviour
	{
		[HideInInspector]
		[SerializeField]
		internal string relevantAIId;

		[HideInInspector]
		[SerializeField]
		internal SceneVisualizationMode mode;

		private Guid _relevantAIGuid;

		protected ContextVisualizerComponent()
		{
		}

		protected virtual void Awake()
		{
			this._relevantAIGuid = (string.IsNullOrEmpty(this.relevantAIId) ? Guid.Empty : new Guid(this.relevantAIId));
		}

		protected void DoDraw(Action<IAIContext> drawer)
		{
			switch (this.mode)
			{
				case SceneVisualizationMode.SingleSelectedGameObject:
				{
					IList<IContextProvider> contextProviders = VisualizationManager.visualizedContextProviders;
					if (contextProviders.Count <= 0)
					{
						break;
					}
					IAIContext context = contextProviders[0].GetContext(this._relevantAIGuid);
					if (context == null)
					{
						break;
					}
					drawer(context);
					return;
				}
				case SceneVisualizationMode.AllSelectedGameObjects:
				{
					IList<IContextProvider> contextProviders1 = VisualizationManager.visualizedContextProviders;
					int count = contextProviders1.Count;
					for (int i = 0; i < count; i++)
					{
						IAIContext aIContext = contextProviders1[i].GetContext(this._relevantAIGuid);
						if (aIContext != null)
						{
							drawer(aIContext);
						}
					}
					return;
				}
				case SceneVisualizationMode.Custom:
				{
					List<IAIContext> buffer = ListBufferPool.GetBuffer<IAIContext>(4);
					this.GetContextsToVisualize(buffer, this._relevantAIGuid);
					int num = buffer.Count;
					for (int j = 0; j < num; j++)
					{
						drawer(buffer[j]);
					}
					ListBufferPool.ReturnBuffer<IAIContext>(buffer);
					break;
				}
				default:
				{
					return;
				}
			}
		}

		protected virtual void GetContextsToVisualize(List<IAIContext> contextsBuffer, Guid relevantAIId)
		{
		}

		private void OnEnable()
		{
		}
	}
}