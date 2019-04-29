using ConVar;
using System;
using System.Linq;
using UnityEngine;

public class Poolable : MonoBehaviour, IClientComponent, IPrefabPostProcess
{
	[HideInInspector]
	public uint prefabID;

	[HideInInspector]
	public Behaviour[] behaviours;

	[HideInInspector]
	public Rigidbody[] rigidbodies;

	[HideInInspector]
	public Collider[] colliders;

	[HideInInspector]
	public LODGroup[] lodgroups;

	[HideInInspector]
	public Renderer[] renderers;

	[HideInInspector]
	public ParticleSystem[] particles;

	[HideInInspector]
	public bool[] behaviourStates;

	[HideInInspector]
	public bool[] rigidbodyStates;

	[HideInInspector]
	public bool[] colliderStates;

	[HideInInspector]
	public bool[] lodgroupStates;

	[HideInInspector]
	public bool[] rendererStates;

	public int ClientCount
	{
		get
		{
			if (base.GetComponent<LootPanel>() != null)
			{
				return 1;
			}
			if (base.GetComponent<DecorComponent>() != null)
			{
				return 100;
			}
			if (base.GetComponent<BuildingBlock>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Door>() != null)
			{
				return 100;
			}
			if (base.GetComponent<Projectile>() != null)
			{
				return 100;
			}
			return 10;
		}
	}

	public int ServerCount
	{
		get
		{
			return 0;
		}
	}

	public Poolable()
	{
	}

	public void EnterPool()
	{
		if (base.transform.parent != null)
		{
			base.transform.SetParent(null, false);
		}
		if (Pool.mode > 1)
		{
			this.SetBehaviourEnabled(false);
			this.SetComponentEnabled(false);
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
		}
		else if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
			return;
		}
	}

	public void Initialize(uint id)
	{
		this.prefabID = id;
		this.behaviours = base.gameObject.GetComponentsInChildren(typeof(Behaviour), true).OfType<Behaviour>().ToArray<Behaviour>();
		this.rigidbodies = base.gameObject.GetComponentsInChildren<Rigidbody>(true);
		this.colliders = base.gameObject.GetComponentsInChildren<Collider>(true);
		this.lodgroups = base.gameObject.GetComponentsInChildren<LODGroup>(true);
		this.renderers = base.gameObject.GetComponentsInChildren<Renderer>(true);
		this.particles = base.gameObject.GetComponentsInChildren<ParticleSystem>(true);
		this.behaviourStates = new bool[(int)this.behaviours.Length];
		this.rigidbodyStates = new bool[(int)this.rigidbodies.Length];
		this.colliderStates = new bool[(int)this.colliders.Length];
		this.lodgroupStates = new bool[(int)this.lodgroups.Length];
		this.rendererStates = new bool[(int)this.renderers.Length];
	}

	public void LeavePool()
	{
		if (Pool.mode > 1)
		{
			this.SetComponentEnabled(true);
		}
	}

	public void PostProcess(IPrefabProcessor preProcess, GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		if (bundling)
		{
			return;
		}
		this.Initialize(StringPool.Get(name));
	}

	public void SetBehaviourEnabled(bool state)
	{
		try
		{
			if (state)
			{
				for (int i = 0; i < (int)this.particles.Length; i++)
				{
					ParticleSystem particleSystem = this.particles[i];
					if (particleSystem.playOnAwake)
					{
						particleSystem.Play();
					}
				}
				for (int j = 0; j < (int)this.behaviours.Length; j++)
				{
					this.behaviours[j].enabled = this.behaviourStates[j];
				}
			}
			else
			{
				for (int k = 0; k < (int)this.behaviours.Length; k++)
				{
					Behaviour behaviour = this.behaviours[k];
					this.behaviourStates[k] = behaviour.enabled;
					behaviour.enabled = false;
				}
				for (int l = 0; l < (int)this.particles.Length; l++)
				{
					ParticleSystem particleSystem1 = this.particles[l];
					particleSystem1.Stop();
					particleSystem1.Clear();
				}
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			Debug.LogError(string.Concat(new string[] { "Pooling error: ", base.name, " (", exception.Message, ")" }));
		}
	}

	public void SetComponentEnabled(bool state)
	{
		try
		{
			if (state)
			{
				for (int i = 0; i < (int)this.renderers.Length; i++)
				{
					this.renderers[i].enabled = this.rendererStates[i];
				}
				for (int j = 0; j < (int)this.lodgroups.Length; j++)
				{
					this.lodgroups[j].enabled = this.lodgroupStates[j];
				}
				for (int k = 0; k < (int)this.colliders.Length; k++)
				{
					this.colliders[k].enabled = this.colliderStates[k];
				}
				for (int l = 0; l < (int)this.rigidbodies.Length; l++)
				{
					Rigidbody rigidbody = this.rigidbodies[l];
					rigidbody.isKinematic = this.rigidbodyStates[l];
					rigidbody.detectCollisions = true;
				}
			}
			else
			{
				for (int m = 0; m < (int)this.renderers.Length; m++)
				{
					Renderer renderer = this.renderers[m];
					this.rendererStates[m] = renderer.enabled;
					renderer.enabled = false;
				}
				for (int n = 0; n < (int)this.lodgroups.Length; n++)
				{
					LODGroup lODGroup = this.lodgroups[n];
					this.lodgroupStates[n] = lODGroup.enabled;
					lODGroup.enabled = false;
				}
				for (int o = 0; o < (int)this.colliders.Length; o++)
				{
					Collider collider = this.colliders[o];
					this.colliderStates[o] = collider.enabled;
					collider.enabled = false;
				}
				for (int p = 0; p < (int)this.rigidbodies.Length; p++)
				{
					Rigidbody rigidbody1 = this.rigidbodies[p];
					this.rigidbodyStates[p] = rigidbody1.isKinematic;
					rigidbody1.isKinematic = true;
					rigidbody1.detectCollisions = false;
				}
			}
		}
		catch (Exception exception1)
		{
			Exception exception = exception1;
			Debug.LogError(string.Concat(new string[] { "Pooling error: ", base.name, " (", exception.Message, ")" }));
		}
	}
}