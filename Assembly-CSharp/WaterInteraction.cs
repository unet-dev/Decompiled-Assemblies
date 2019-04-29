using System;
using System.Runtime.CompilerServices;
using UnityEngine;

[ExecuteInEditMode]
public class WaterInteraction : MonoBehaviour
{
	[SerializeField]
	private Texture2D texture;

	[Range(0f, 1f)]
	public float Displacement = 1f;

	[Range(0f, 1f)]
	public float Disturbance = 0.5f;

	private Transform cachedTransform;

	public WaterDynamics.Image Image
	{
		get;
		private set;
	}

	public Vector2 Position { get; private set; } = Vector2.zero;

	public float Rotation
	{
		get;
		private set;
	}

	public Vector2 Scale { get; private set; } = Vector2.one;

	public Texture2D Texture
	{
		get
		{
			return this.texture;
		}
		set
		{
			this.texture = value;
			this.CheckRegister();
		}
	}

	public WaterInteraction()
	{
	}

	public void CheckRegister()
	{
		if (!base.enabled || this.texture == null)
		{
			this.Unregister();
			return;
		}
		if (this.Image == null || this.Image.texture != this.texture)
		{
			this.Register();
		}
	}

	protected void OnDisable()
	{
		this.Unregister();
	}

	protected void OnEnable()
	{
		this.CheckRegister();
		this.UpdateTransform();
	}

	private void Register()
	{
		this.UpdateImage();
		WaterDynamics.RegisterInteraction(this);
	}

	private void Unregister()
	{
		if (this.Image != null)
		{
			WaterDynamics.UnregisterInteraction(this);
			this.Image = null;
		}
	}

	private void UpdateImage()
	{
		this.Image = new WaterDynamics.Image(this.texture);
	}

	public void UpdateTransform()
	{
		this.cachedTransform = (this.cachedTransform != null ? this.cachedTransform : base.transform);
		if (this.cachedTransform.hasChanged)
		{
			Vector3 vector3 = this.cachedTransform.position;
			Vector3 vector31 = this.cachedTransform.lossyScale;
			this.Position = new Vector2(vector3.x, vector3.z);
			this.Scale = new Vector2(vector31.x, vector31.z);
			this.Rotation = this.cachedTransform.rotation.eulerAngles.y;
			this.cachedTransform.hasChanged = false;
		}
	}
}