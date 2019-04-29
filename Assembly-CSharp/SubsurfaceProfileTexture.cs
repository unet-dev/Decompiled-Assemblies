using System;
using System.Collections.Generic;
using UnityEngine;

public class SubsurfaceProfileTexture
{
	public const int SUBSURFACE_RADIUS_SCALE = 1024;

	public const int SUBSURFACE_KERNEL_SIZE = 3;

	private List<SubsurfaceProfileTexture.SubsurfaceProfileEntry> entries = new List<SubsurfaceProfileTexture.SubsurfaceProfileEntry>(16);

	private Texture2D texture;

	public Texture2D Texture
	{
		get
		{
			if (this.texture != null)
			{
				return this.texture;
			}
			return this.CreateTexture();
		}
	}

	public SubsurfaceProfileTexture()
	{
		this.AddProfile(SubsurfaceProfileData.Default, null);
	}

	public int AddProfile(SubsurfaceProfileData data, SubsurfaceProfile profile)
	{
		int subsurfaceProfileEntry = -1;
		int num = 0;
		while (num < this.entries.Count)
		{
			if (this.entries[num].profile != profile)
			{
				num++;
			}
			else
			{
				subsurfaceProfileEntry = num;
				this.entries[subsurfaceProfileEntry] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile);
				break;
			}
		}
		if (subsurfaceProfileEntry < 0)
		{
			subsurfaceProfileEntry = this.entries.Count;
			this.entries.Add(new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, profile));
		}
		this.ReleaseTexture();
		return subsurfaceProfileEntry;
	}

	private void CheckReleaseTexture()
	{
		int num = 0;
		for (int i = 0; i < this.entries.Count; i++)
		{
			num = num + (this.entries[i].profile == null ? 1 : 0);
		}
		if (this.entries.Count == num)
		{
			this.ReleaseTexture();
		}
	}

	public static Color ColorClamp(Color color, float min = 0f, float max = 1f)
	{
		Color color1 = new Color();
		color1.r = Mathf.Clamp(color.r, min, max);
		color1.g = Mathf.Clamp(color.g, min, max);
		color1.b = Mathf.Clamp(color.b, min, max);
		color1.a = Mathf.Clamp(color.a, min, max);
		return color1;
	}

	private Texture2D CreateTexture()
	{
		if (this.entries.Count <= 0)
		{
			return null;
		}
		int num = 32;
		int num1 = Mathf.Max(this.entries.Count, 64);
		this.ReleaseTexture();
		this.texture = new Texture2D(num, num1, TextureFormat.RGBAHalf, false, true)
		{
			name = "SubsurfaceProfiles",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Bilinear
		};
		Color[] pixels = this.texture.GetPixels(0);
		for (int i = 0; i < (int)pixels.Length; i++)
		{
			pixels[i] = Color.clear;
		}
		Color[] subsurfaceColor = new Color[num];
		for (int j = 0; j < this.entries.Count; j++)
		{
			SubsurfaceProfileData item = this.entries[j].data;
			item.SubsurfaceColor = SubsurfaceProfileTexture.ColorClamp(item.SubsurfaceColor, 0f, 1f);
			item.FalloffColor = SubsurfaceProfileTexture.ColorClamp(item.FalloffColor, 0.009f, 1f);
			subsurfaceColor[0] = item.SubsurfaceColor;
			subsurfaceColor[0].a = 0f;
			SeparableSSS.CalculateKernel(subsurfaceColor, 1, 13, item.SubsurfaceColor, item.FalloffColor);
			SeparableSSS.CalculateKernel(subsurfaceColor, 14, 9, item.SubsurfaceColor, item.FalloffColor);
			SeparableSSS.CalculateKernel(subsurfaceColor, 23, 6, item.SubsurfaceColor, item.FalloffColor);
			int num2 = num * (num1 - j - 1);
			for (int k = 0; k < 29; k++)
			{
				Color color = subsurfaceColor[k] * new Color(1f, 1f, 1f, 0.333333343f);
				ref float scatterRadius = ref color.a;
				scatterRadius = scatterRadius * (item.ScatterRadius / 1024f);
				pixels[num2 + k] = color;
			}
		}
		this.texture.SetPixels(pixels, 0);
		this.texture.Apply(false, false);
		return this.texture;
	}

	public int FindEntryIndex(SubsurfaceProfile profile)
	{
		for (int i = 0; i < this.entries.Count; i++)
		{
			if (this.entries[i].profile == profile)
			{
				return i;
			}
		}
		return -1;
	}

	private void ReleaseTexture()
	{
		if (this.texture != null)
		{
			UnityEngine.Object.DestroyImmediate(this.texture);
			this.texture = null;
		}
	}

	public void RemoveProfile(int id)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(SubsurfaceProfileData.Invalid, null);
			this.CheckReleaseTexture();
		}
	}

	public void UpdateProfile(int id, SubsurfaceProfileData data)
	{
		if (id >= 0)
		{
			this.entries[id] = new SubsurfaceProfileTexture.SubsurfaceProfileEntry(data, this.entries[id].profile);
			this.ReleaseTexture();
		}
	}

	private struct SubsurfaceProfileEntry
	{
		public SubsurfaceProfileData data;

		public SubsurfaceProfile profile;

		public SubsurfaceProfileEntry(SubsurfaceProfileData data, SubsurfaceProfile profile)
		{
			this.data = data;
			this.profile = profile;
		}
	}
}