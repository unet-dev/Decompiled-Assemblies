using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class WaterBody : MonoBehaviour
{
	public WaterBodyType Type = WaterBodyType.Lake;

	public UnityEngine.Renderer Renderer;

	public Collider[] Triggers;

	public UnityEngine.Transform Transform
	{
		get;
		private set;
	}

	public WaterBody()
	{
	}

	private void Awake()
	{
		this.Transform = base.transform;
	}

	private void OnDisable()
	{
		WaterSystem.UnregisterBody(this);
	}

	private void OnEnable()
	{
		WaterSystem.RegisterBody(this);
	}
}