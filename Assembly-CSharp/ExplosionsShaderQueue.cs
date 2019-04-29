using System;
using UnityEngine;

public class ExplosionsShaderQueue : MonoBehaviour
{
	public int AddQueue = 1;

	private Renderer rend;

	public ExplosionsShaderQueue()
	{
	}

	private void OnDisable()
	{
		if (this.rend != null)
		{
			this.rend.sharedMaterial.renderQueue = -1;
		}
	}

	private void SetProjectorQueue()
	{
		Material component = base.GetComponent<Projector>().material;
		component.renderQueue = component.renderQueue + this.AddQueue;
	}

	private void Start()
	{
		this.rend = base.GetComponent<Renderer>();
		if (this.rend == null)
		{
			base.Invoke("SetProjectorQueue", 0.1f);
			return;
		}
		Material addQueue = this.rend.sharedMaterial;
		addQueue.renderQueue = addQueue.renderQueue + this.AddQueue;
	}
}