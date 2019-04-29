using Rust;
using System;
using UnityEngine;

public class SubsurfaceProfile : ScriptableObject
{
	private static SubsurfaceProfileTexture profileTexture;

	public SubsurfaceProfileData Data = SubsurfaceProfileData.Default;

	private int id = -1;

	public int Id
	{
		get
		{
			return this.id;
		}
	}

	public static Texture2D Texture
	{
		get
		{
			if (SubsurfaceProfile.profileTexture == null)
			{
				return null;
			}
			return SubsurfaceProfile.profileTexture.Texture;
		}
	}

	static SubsurfaceProfile()
	{
		SubsurfaceProfile.profileTexture = new SubsurfaceProfileTexture();
	}

	public SubsurfaceProfile()
	{
	}

	private void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		SubsurfaceProfile.profileTexture.RemoveProfile(this.id);
	}

	private void OnEnable()
	{
		this.id = SubsurfaceProfile.profileTexture.AddProfile(this.Data, this);
	}

	public void Update()
	{
		SubsurfaceProfile.profileTexture.UpdateProfile(this.id, this.Data);
	}
}