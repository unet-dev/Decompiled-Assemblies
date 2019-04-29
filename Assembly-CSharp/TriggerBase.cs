using ConVar;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerBase : BaseMonoBehaviour
{
	public LayerMask interestLayers;

	[NonSerialized]
	public HashSet<GameObject> contents;

	[NonSerialized]
	public HashSet<BaseEntity> entityContents;

	public TriggerBase()
	{
	}

	internal bool CheckEntity(BaseEntity ent)
	{
		if (ent == null)
		{
			return true;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return true;
		}
		Bounds bound = component.bounds;
		bound.Expand(1f);
		return bound.Contains(ent.ClosestPoint(base.transform.position));
	}

	internal virtual GameObject InterestedInObject(GameObject obj)
	{
		int num = 1 << (obj.layer & 31);
		if ((this.interestLayers.@value & num) != num)
		{
			return null;
		}
		return obj;
	}

	protected virtual void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		if (this.contents == null)
		{
			return;
		}
		GameObject[] array = this.contents.ToArray<GameObject>();
		for (int i = 0; i < (int)array.Length; i++)
		{
			this.OnTriggerExit(array[i]);
		}
		this.contents = null;
	}

	internal virtual void OnEmpty()
	{
		this.contents = null;
		this.entityContents = null;
	}

	internal virtual void OnEntityEnter(BaseEntity ent)
	{
		if (ent == null)
		{
			return;
		}
		if (this.entityContents == null)
		{
			this.entityContents = new HashSet<BaseEntity>();
		}
		Interface.CallHook("OnEntityEnter", this, ent);
		this.entityContents.Add(ent);
	}

	internal virtual void OnEntityLeave(BaseEntity ent)
	{
		if (this.entityContents == null)
		{
			return;
		}
		Interface.CallHook("OnEntityLeave", this, ent);
		this.entityContents.Remove(ent);
	}

	internal virtual void OnObjectAdded(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			this.OnEntityEnter(baseEntity);
			baseEntity.EnterTrigger(this);
		}
	}

	internal virtual void OnObjectRemoved(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		BaseEntity baseEntity = obj.ToBaseEntity();
		if (baseEntity)
		{
			this.OnEntityLeave(baseEntity);
			baseEntity.LeaveTrigger(this);
		}
	}

	internal virtual void OnObjects()
	{
	}

	public void OnTriggerEnter(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		using (TimeWarning timeWarning = TimeWarning.New("TriggerBase.OnTriggerEnter", 0.1f))
		{
			GameObject gameObject = this.InterestedInObject(collider.gameObject);
			if (gameObject != null)
			{
				if (this.contents == null)
				{
					this.contents = new HashSet<GameObject>();
				}
				if (!this.contents.Contains(gameObject))
				{
					int count = this.contents.Count;
					this.contents.Add(gameObject);
					this.OnObjectAdded(gameObject);
					if (count == 0 && this.contents.Count == 1)
					{
						this.OnObjects();
					}
				}
				else
				{
					return;
				}
			}
			else
			{
				return;
			}
		}
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	public void OnTriggerExit(Collider collider)
	{
		if (this == null)
		{
			return;
		}
		if (collider == null)
		{
			return;
		}
		GameObject gameObject = this.InterestedInObject(collider.gameObject);
		if (gameObject == null)
		{
			return;
		}
		this.OnTriggerExit(gameObject);
		if (Debugging.checktriggers)
		{
			this.RemoveInvalidEntities();
		}
	}

	private void OnTriggerExit(GameObject targetObj)
	{
		if (this.contents == null)
		{
			return;
		}
		if (!this.contents.Contains(targetObj))
		{
			return;
		}
		this.contents.Remove(targetObj);
		this.OnObjectRemoved(targetObj);
		if (this.contents == null || this.contents.Count == 0)
		{
			this.OnEmpty();
		}
	}

	public void RemoveEntity(BaseEntity obj)
	{
		this.OnTriggerExit(obj.gameObject);
	}

	internal void RemoveInvalidEntities()
	{
		if (this.entityContents == null)
		{
			return;
		}
		Collider component = base.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		Bounds bound = component.bounds;
		bound.Expand(1f);
		BaseEntity[] array = this.entityContents.ToArray<BaseEntity>();
		for (int i = 0; i < (int)array.Length; i++)
		{
			BaseEntity baseEntity = array[i];
			if (baseEntity == null)
			{
				Debug.LogWarning(string.Concat("Trigger ", this.ToString(), " contains destroyed entity."));
			}
			else if (!bound.Contains(baseEntity.ClosestPoint(base.transform.position)))
			{
				Debug.LogWarning(string.Concat("Trigger ", this.ToString(), " contains entity that is too far away: ", baseEntity.ToString()));
				this.RemoveEntity(baseEntity);
			}
		}
	}

	public void RemoveObject(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}
		Collider component = obj.GetComponent<Collider>();
		if (component == null)
		{
			return;
		}
		this.OnTriggerExit(component);
	}
}