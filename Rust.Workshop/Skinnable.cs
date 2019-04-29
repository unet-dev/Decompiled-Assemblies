using Rust.Workshop;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName="Rust/Skinnable")]
public class Skinnable : ScriptableObject
{
	public string Name;

	public string ItemName;

	public GameObject EntityPrefab;

	public string EntityPrefabName;

	public GameObject ViewmodelPrefab;

	public string ViewmodelPrefabName;

	public Mesh[] MeshDownloads;

	public Rust.Workshop.Category Category;

	public Skinnable.Group[] Groups;

	public static Skinnable[] All;

	[NonSerialized]
	private Material[] _sourceMaterials;

	public Material[] SourceMaterials
	{
		get
		{
			if (this._sourceMaterials == null)
			{
				this._sourceMaterials = new Material[(int)this.Groups.Length];
				for (int i = 0; i < (int)this.Groups.Length; i++)
				{
					this._sourceMaterials[i] = this.Groups[i].Material;
				}
			}
			return this._sourceMaterials;
		}
	}

	public Skinnable()
	{
	}

	public static Skinnable FindForEntity(string entityName)
	{
		return Skinnable.All.FirstOrDefault<Skinnable>((Skinnable x) => string.Compare(x.EntityPrefabName, entityName, StringComparison.OrdinalIgnoreCase) == 0);
	}

	public static Skinnable FindForItem(string itemType)
	{
		return Skinnable.All.FirstOrDefault<Skinnable>((Skinnable x) => {
			if (string.Compare(x.ItemName, itemType, StringComparison.OrdinalIgnoreCase) == 0)
			{
				return true;
			}
			return string.Compare(x.Name, itemType, StringComparison.OrdinalIgnoreCase) == 0;
		});
	}

	[Serializable]
	public class Group
	{
		public string Name;

		public Material Material;

		public int MaxTextureSize;

		public Group()
		{
		}
	}
}