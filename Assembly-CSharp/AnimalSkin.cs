using System;
using UnityEngine;

public class AnimalSkin : MonoBehaviour, IClientComponent
{
	public SkinnedMeshRenderer[] animalMesh;

	public AnimalMultiSkin[] animalSkins;

	private Model model;

	public bool dontRandomizeOnStart;

	public AnimalSkin()
	{
	}

	public void ChangeSkin(int iSkin)
	{
		if (this.animalSkins.Length == 0)
		{
			return;
		}
		iSkin = Mathf.Clamp(iSkin, 0, (int)this.animalSkins.Length - 1);
		SkinnedMeshRenderer[] skinnedMeshRendererArray = this.animalMesh;
		for (int i = 0; i < (int)skinnedMeshRendererArray.Length; i++)
		{
			SkinnedMeshRenderer skinnedMeshRenderer = skinnedMeshRendererArray[i];
			Material[] materialArray = skinnedMeshRenderer.sharedMaterials;
			if (materialArray != null)
			{
				for (int j = 0; j < (int)materialArray.Length; j++)
				{
					materialArray[j] = this.animalSkins[iSkin].multiSkin[j];
				}
				skinnedMeshRenderer.sharedMaterials = materialArray;
			}
		}
		if (this.model != null)
		{
			this.model.skin = iSkin;
		}
	}

	private void Start()
	{
		this.model = base.gameObject.GetComponent<Model>();
		if (!this.dontRandomizeOnStart)
		{
			int num = Mathf.FloorToInt((float)UnityEngine.Random.Range(0, (int)this.animalSkins.Length));
			this.ChangeSkin(num);
		}
	}
}