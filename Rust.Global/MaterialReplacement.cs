using Facepunch;
using Facepunch.Extend;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class MaterialReplacement : MonoBehaviour
{
	private bool initialized;

	public Material[] Default;

	public Material[] Override;

	public UnityEngine.Renderer Renderer;

	public MaterialReplacement()
	{
	}

	private void Init()
	{
		if (this.initialized)
		{
			return;
		}
		this.initialized = true;
		this.Renderer = base.GetComponent<UnityEngine.Renderer>();
		this.Default = this.Renderer.sharedMaterials;
		this.Override = new Material[(int)this.Default.Length];
		Array.Copy(this.Default, this.Override, (int)this.Default.Length);
	}

	private static bool MaterialsContainAny(Material[] source, Material[] find)
	{
		for (int i = 0; i < (int)source.Length; i++)
		{
			if (find.Contains<Material>(source[i]))
			{
				return true;
			}
		}
		return false;
	}

	public static void Prepare(GameObject go)
	{
		List<UnityEngine.Renderer> list = Pool.GetList<UnityEngine.Renderer>();
		go.GetComponentsInChildren<UnityEngine.Renderer>(true, list);
		foreach (UnityEngine.Renderer renderer in list)
		{
			if (renderer is ParticleSystemRenderer)
			{
				continue;
			}
			renderer.transform.GetOrAddComponent<MaterialReplacement>().Init();
		}
		Pool.FreeList<UnityEngine.Renderer>(ref list);
	}

	private void Replace(Material mat)
	{
		if (this.Renderer)
		{
			for (int i = 0; i < (int)this.Override.Length; i++)
			{
				this.Override[i] = mat;
			}
			this.Renderer.sharedMaterials = this.Override;
		}
	}

	private void Replace(Material find, Material replace)
	{
		if (this.Renderer)
		{
			for (int i = 0; i < (int)this.Default.Length; i++)
			{
				if (find == this.Default[i])
				{
					this.Override[i] = replace;
				}
			}
			this.Renderer.sharedMaterials = this.Override;
		}
	}

	private void Replace(Material[] find, Material[] replace)
	{
		if (this.Renderer)
		{
			int num = Mathf.Min((int)find.Length, (int)replace.Length);
			for (int i = 0; i < (int)this.Default.Length; i++)
			{
				for (int j = 0; j < num; j++)
				{
					if (find[j] == this.Default[i])
					{
						this.Override[i] = replace[j];
					}
				}
			}
			this.Renderer.sharedMaterials = this.Override;
		}
	}

	public static void ReplaceRecursive(GameObject go, Material mat)
	{
		List<UnityEngine.Renderer> list = Pool.GetList<UnityEngine.Renderer>();
		go.transform.GetComponentsInChildren<UnityEngine.Renderer>(true, list);
		foreach (UnityEngine.Renderer renderer in list)
		{
			if (renderer is ParticleSystemRenderer)
			{
				continue;
			}
			MaterialReplacement orAddComponent = renderer.transform.GetOrAddComponent<MaterialReplacement>();
			orAddComponent.Init();
			orAddComponent.Replace(mat);
		}
		Pool.FreeList<UnityEngine.Renderer>(ref list);
	}

	public static void ReplaceRecursive(GameObject obj, Material[] find, Material[] replace)
	{
		Assert.AreEqual((int)find.Length, (int)replace.Length);
		List<UnityEngine.Renderer> list = Pool.GetList<UnityEngine.Renderer>();
		obj.GetComponentsInChildren<UnityEngine.Renderer>(true, list);
		foreach (UnityEngine.Renderer renderer in list)
		{
			if (renderer is ParticleSystemRenderer || !MaterialReplacement.MaterialsContainAny(renderer.sharedMaterials, find))
			{
				continue;
			}
			MaterialReplacement orAddComponent = renderer.transform.GetOrAddComponent<MaterialReplacement>();
			orAddComponent.Init();
			orAddComponent.Revert();
			orAddComponent.Replace(find, replace);
		}
		Pool.FreeList<UnityEngine.Renderer>(ref list);
	}

	public static void Reset(GameObject go)
	{
		List<MaterialReplacement> list = Pool.GetList<MaterialReplacement>();
		go.GetComponentsInChildren<MaterialReplacement>(true, list);
		foreach (MaterialReplacement materialReplacement in list)
		{
			materialReplacement.Revert();
		}
		Pool.FreeList<MaterialReplacement>(ref list);
	}

	private void Revert()
	{
		if (this.Renderer)
		{
			this.Renderer.sharedMaterials = this.Default;
			Array.Copy(this.Default, this.Override, (int)this.Default.Length);
		}
	}
}