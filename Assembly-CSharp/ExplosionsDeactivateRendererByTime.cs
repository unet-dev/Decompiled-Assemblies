using System;
using UnityEngine;

public class ExplosionsDeactivateRendererByTime : MonoBehaviour
{
	public float TimeDelay = 1f;

	private Renderer rend;

	public ExplosionsDeactivateRendererByTime()
	{
	}

	private void Awake()
	{
		this.rend = base.GetComponent<Renderer>();
	}

	private void DeactivateRenderer()
	{
		this.rend.enabled = false;
	}

	private void OnEnable()
	{
		this.rend.enabled = true;
		base.Invoke("DeactivateRenderer", this.TimeDelay);
	}
}