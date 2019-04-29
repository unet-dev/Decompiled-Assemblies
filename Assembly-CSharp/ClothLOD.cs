using System;
using UnityEngine;

public class ClothLOD : FacepunchBehaviour
{
	[ServerVar(Help="distance cloth will simulate until")]
	public static float clothLODDist;

	public Cloth cloth;

	static ClothLOD()
	{
		ClothLOD.clothLODDist = 20f;
	}

	public ClothLOD()
	{
	}
}