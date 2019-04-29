using Apex.AI;
using Apex.AI.Components;
using Apex.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Apex.AI.Visualization
{
	public static class VisualizationManager
	{
		private static bool _visualizing;

		private static Dictionary<Type, ICustomVisualizer> _visualizerLookup;

		private static List<IContextProvider> _visualizedContextProviders;

		internal static bool isVisualizing
		{
			get
			{
				return VisualizationManager._visualizing;
			}
		}

		internal static IList<IContextProvider> visualizedContextProviders
		{
			get
			{
				if (VisualizationManager._visualizedContextProviders == null)
				{
					VisualizationManager._visualizedContextProviders = new List<IContextProvider>();
				}
				return VisualizationManager._visualizedContextProviders;
			}
		}

		internal static bool BeginVisualization()
		{
			if (VisualizationManager._visualizing)
			{
				return false;
			}
			VisualizationManager._visualizing = true;
			foreach (IUtilityAIClient allClient in AIManager.allClients)
			{
				if (allClient.ai is UtilityAIVisualizer)
				{
					continue;
				}
				allClient.ai = new UtilityAIVisualizer(allClient.ai);
			}
			return true;
		}

		private static IEnumerable<Type> GetDerived(Type forType)
		{
			return ApexReflection.GetRelevantTypes().Where<Type>((Type t) => {
				if (!t.IsClass || t.IsAbstract)
				{
					return false;
				}
				return forType.IsAssignableFrom(t);
			});
		}

		public static void RegisterVisualizer<TFor>(ICustomVisualizer visualizer, bool registerDerivedTypes = false)
		{
			VisualizationManager.RegisterVisualizer(typeof(TFor), visualizer, registerDerivedTypes);
		}

		public static void RegisterVisualizer(Type forType, ICustomVisualizer visualizer, bool registerDerivedTypes = false)
		{
			Ensure.ArgumentNotNull(visualizer, "visualizer");
			if (VisualizationManager._visualizerLookup == null)
			{
				VisualizationManager._visualizerLookup = new Dictionary<Type, ICustomVisualizer>();
			}
			if (!(forType.IsAbstract | registerDerivedTypes))
			{
				if (VisualizationManager._visualizerLookup.ContainsKey(forType))
				{
					Debug.LogWarning(string.Format("A visualizer for type {0} has already been registered, skipping {1}.", forType.Name, visualizer.GetType().Name));
					return;
				}
				VisualizationManager._visualizerLookup.Add(forType, visualizer);
			}
			else
			{
				foreach (Type derived in VisualizationManager.GetDerived(forType))
				{
					if (!VisualizationManager._visualizerLookup.ContainsKey(derived))
					{
						VisualizationManager._visualizerLookup.Add(derived, visualizer);
					}
					else
					{
						Debug.LogWarning(string.Format("A visualizer for type {0} has already been registered, skipping {1}.", derived.Name, visualizer.GetType().Name));
					}
				}
			}
		}

		internal static bool TryGetVisualizerFor(Type t, out ICustomVisualizer visualizer)
		{
			if (VisualizationManager._visualizerLookup == null)
			{
				visualizer = null;
				return false;
			}
			return VisualizationManager._visualizerLookup.TryGetValue(t, out visualizer);
		}

		public static void UnregisterVisualizer<TFor>(bool registeredDerivedTypes = false)
		{
			VisualizationManager.UnregisterVisualizer(typeof(TFor), registeredDerivedTypes);
		}

		public static void UnregisterVisualizer(Type forType, bool registeredDerivedTypes = false)
		{
			if (VisualizationManager._visualizerLookup == null)
			{
				return;
			}
			if (!(forType.IsAbstract | registeredDerivedTypes))
			{
				VisualizationManager._visualizerLookup.Remove(forType);
			}
			else
			{
				foreach (Type derived in VisualizationManager.GetDerived(forType))
				{
					VisualizationManager._visualizerLookup.Remove(derived);
				}
			}
		}

		internal static void UpdateSelectedGameObjects(GameObject[] selected)
		{
			if (!VisualizationManager._visualizing)
			{
				return;
			}
			IList<IContextProvider> contextProviders = VisualizationManager.visualizedContextProviders;
			contextProviders.Clear();
			if (selected == null)
			{
				return;
			}
			for (int i = 0; i < (int)selected.Length; i++)
			{
				IContextProvider component = selected[i].GetComponent(typeof(IContextProvider)) as IContextProvider;
				if (component != null)
				{
					contextProviders.Add(component);
				}
			}
		}
	}
}