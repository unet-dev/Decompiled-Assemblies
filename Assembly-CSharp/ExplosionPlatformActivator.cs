using System;
using UnityEngine;

public class ExplosionPlatformActivator : MonoBehaviour
{
	public GameObject Effect;

	public float TimeDelay;

	public float DefaultRepeatTime = 5f;

	public float NearRepeatTime = 3f;

	private float currentTime;

	private float currentRepeatTime;

	private bool canUpdate;

	public ExplosionPlatformActivator()
	{
	}

	private void Init()
	{
		this.canUpdate = true;
		this.Effect.SetActive(true);
	}

	private void OnTriggerEnter(Collider coll)
	{
		this.currentRepeatTime = this.NearRepeatTime;
	}

	private void OnTriggerExit(Collider other)
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
	}

	private void Start()
	{
		this.currentRepeatTime = this.DefaultRepeatTime;
		base.Invoke("Init", this.TimeDelay);
	}

	private void Update()
	{
		if (!this.canUpdate || this.Effect == null)
		{
			return;
		}
		this.currentTime += Time.deltaTime;
		if (this.currentTime > this.currentRepeatTime)
		{
			this.currentTime = 0f;
			this.Effect.SetActive(false);
			this.Effect.SetActive(true);
		}
	}
}