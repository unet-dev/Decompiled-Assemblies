using System;
using UnityEngine;

public class ImpostorAsset : ScriptableObject
{
	public ImpostorAsset.TextureEntry[] textures;

	public Vector2 size;

	public Vector2 pivot;

	public Mesh mesh;

	public ImpostorAsset()
	{
	}

	public Texture2D FindTexture(string name)
	{
		ImpostorAsset.TextureEntry[] textureEntryArray = this.textures;
		for (int i = 0; i < (int)textureEntryArray.Length; i++)
		{
			ImpostorAsset.TextureEntry textureEntry = textureEntryArray[i];
			if (textureEntry.name == name)
			{
				return textureEntry.texture;
			}
		}
		return null;
	}

	[Serializable]
	public class TextureEntry
	{
		public string name;

		public Texture2D texture;

		public TextureEntry(string name, Texture2D texture)
		{
			this.name = name;
			this.texture = texture;
		}
	}
}