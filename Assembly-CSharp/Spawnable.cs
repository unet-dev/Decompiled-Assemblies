using Facepunch;
using ProtoBuf;
using Rust;
using System;
using UnityEngine;

public class Spawnable : MonoBehaviour, IServerComponent
{
	[ReadOnly]
	public SpawnPopulation Population;

	internal uint PrefabID;

	internal bool SpawnIndividual;

	internal Vector3 SpawnPosition;

	internal Quaternion SpawnRotation;

	public Spawnable()
	{
	}

	private void Add()
	{
		this.SpawnPosition = base.transform.position;
		this.SpawnRotation = base.transform.rotation;
		if (SingletonComponent<SpawnHandler>.Instance)
		{
			if (this.Population != null)
			{
				SingletonComponent<SpawnHandler>.Instance.AddInstance(this);
				return;
			}
			if (Rust.Application.isLoading && !Rust.Application.isLoadingSave)
			{
				BaseEntity component = base.GetComponent<BaseEntity>();
				if (component != null && component.enableSaving && !component.syncPosition)
				{
					SingletonComponent<SpawnHandler>.Instance.AddRespawn(new SpawnIndividual(component.prefabID, this.SpawnPosition, this.SpawnRotation));
				}
			}
		}
	}

	internal void Load(BaseNetworkable.LoadInfo info)
	{
		if (info.msg.spawnable != null)
		{
			this.Population = FileSystem.Load<SpawnPopulation>(StringPool.Get(info.msg.spawnable.population), true);
		}
		this.Add();
	}

	protected void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Remove();
	}

	protected void OnEnable()
	{
		if (Rust.Application.isLoadingSave)
		{
			return;
		}
		this.Add();
	}

	protected void OnValidate()
	{
		this.Population = null;
	}

	private void Remove()
	{
		if (SingletonComponent<SpawnHandler>.Instance && this.Population != null)
		{
			SingletonComponent<SpawnHandler>.Instance.RemoveInstance(this);
		}
	}

	internal void Save(BaseNetworkable.SaveInfo info)
	{
		if (this.Population == null)
		{
			return;
		}
		info.msg.spawnable = Pool.Get<ProtoBuf.Spawnable>();
		info.msg.spawnable.population = this.Population.FilenameStringId;
	}
}