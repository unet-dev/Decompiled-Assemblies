using System;
using UnityEngine;
using UnityEngine.Rendering;

public class TreeLOD : LODComponent
{
	[Horizontal(1, 0)]
	public TreeLOD.State[] States;

	public TreeLOD()
	{
	}

	[Serializable]
	public class State
	{
		public float distance;

		public Renderer renderer;

		[NonSerialized]
		public MeshFilter filter;

		[NonSerialized]
		public ShadowCastingMode shadowMode;

		public State()
		{
		}
	}
}