using System;
using UnityEngine;

public class MeshLOD : LODComponent, IBatchingHandler
{
	[Horizontal(1, 0)]
	public MeshLOD.State[] States;

	public MeshLOD()
	{
	}

	[Serializable]
	public class State
	{
		public float distance;

		public Mesh mesh;

		public State()
		{
		}
	}
}