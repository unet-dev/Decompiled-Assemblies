using System;
using UnityEngine;

public class Sandstorm : MonoBehaviour
{
	public ParticleSystem m_psSandStorm;

	public float m_flSpeed;

	public float m_flSwirl;

	public float m_flEmissionRate;

	public Sandstorm()
	{
	}

	private void Start()
	{
	}

	private void Update()
	{
		base.transform.RotateAround(base.transform.position, Vector3.up, Time.deltaTime * this.m_flSwirl);
		Vector3 vector3 = base.transform.eulerAngles;
		vector3.x = -7f + Mathf.Sin(Time.time * 2.5f) * 7f;
		base.transform.eulerAngles = vector3;
		if (this.m_psSandStorm != null)
		{
			this.m_psSandStorm.startSpeed = this.m_flSpeed;
			ParticleSystem mPsSandStorm = this.m_psSandStorm;
			mPsSandStorm.startSpeed = mPsSandStorm.startSpeed + Mathf.Sin(Time.time * 0.4f) * (this.m_flSpeed * 0.75f);
			this.m_psSandStorm.emissionRate = this.m_flEmissionRate + Mathf.Sin(Time.time * 1f) * (this.m_flEmissionRate * 0.3f);
		}
	}
}