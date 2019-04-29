using System;
using UnityEngine;

namespace ConVar
{
	[Factory("fps")]
	public class FPS : ConsoleSystem
	{
		private static int m_graph;

		[ClientVar]
		public static int graph
		{
			get
			{
				return FPS.m_graph;
			}
			set
			{
				FPS.m_graph = value;
				if (!MainCamera.mainCamera)
				{
					return;
				}
				FPSGraph component = MainCamera.mainCamera.GetComponent<FPSGraph>();
				if (!component)
				{
					return;
				}
				component.Refresh();
			}
		}

		[ClientVar(Saved=true)]
		[ServerVar(Saved=true)]
		public static int limit
		{
			get
			{
				return Application.targetFrameRate;
			}
			set
			{
				Application.targetFrameRate = value;
			}
		}

		public FPS()
		{
		}
	}
}