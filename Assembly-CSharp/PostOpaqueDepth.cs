using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(CommandBufferManager))]
public class PostOpaqueDepth : MonoBehaviour
{
	public RenderTexture postOpaqueDepth;

	public RenderTexture PostOpaque
	{
		get
		{
			return this.postOpaqueDepth;
		}
	}

	public PostOpaqueDepth()
	{
	}
}