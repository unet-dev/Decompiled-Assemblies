using System;
using System.Collections.Generic;
using UnityEngine;

public class BasePathNode : MonoBehaviour
{
	public List<BasePathNode> linked;

	public float maxVelocityOnApproach = -1f;

	public bool straightaway;

	public BasePathNode()
	{
	}

	public void OnDrawGizmosSelected()
	{
	}
}