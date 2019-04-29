using System;
using UnityEngine;

public class LeavesBlowing : MonoBehaviour
{
	public ParticleSystem m_psLeaves;

	public float m_flSwirl;

	public float m_flSpeed;

	public float m_flEmissionRate;

	public LeavesBlowing()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		if (this.m_psLeaves != null)
		{
			this.m_psLeaves.startSpeed = this.m_flSpeed;
			ParticleSystem mPsLeaves = this.m_psLeaves;
			mPsLeaves.startSpeed = mPsLeaves.startSpeed + Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psLeaves.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}
}