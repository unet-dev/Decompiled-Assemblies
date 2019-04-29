using System;
using UnityEngine;

public struct SpawnIndividual
{
	public uint PrefabID;

	public Vector3 Position;

	public Quaternion Rotation;

	public SpawnIndividual(uint prefabID, Vector3 position, Quaternion rotation)
	{
		this.PrefabID = prefabID;
		this.Position = position;
		this.Rotation = rotation;
	}
}