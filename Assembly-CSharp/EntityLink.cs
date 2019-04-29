using Facepunch;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityLink : Pool.IPooled
{
	public BaseEntity owner;

	public Socket_Base socket;

	public List<EntityLink> connections = new List<EntityLink>(8);

	public int capacity = 2147483647;

	public string name
	{
		get
		{
			return this.socket.socketName;
		}
	}

	public EntityLink()
	{
	}

	public void Add(EntityLink entity)
	{
		this.connections.Add(entity);
	}

	public bool CanConnect(EntityLink link)
	{
		if (this.IsOccupied())
		{
			return false;
		}
		if (link == null)
		{
			return false;
		}
		if (link.IsOccupied())
		{
			return false;
		}
		return this.socket.CanConnect(this.owner.transform.position, this.owner.transform.rotation, link.socket, link.owner.transform.position, link.owner.transform.rotation);
	}

	public void Clear()
	{
		for (int i = 0; i < this.connections.Count; i++)
		{
			this.connections[i].Remove(this);
		}
		this.connections.Clear();
	}

	public bool Contains(EntityLink entity)
	{
		return this.connections.Contains(entity);
	}

	public void EnterPool()
	{
		this.owner = null;
		this.socket = null;
		this.capacity = 2147483647;
	}

	public bool IsEmpty()
	{
		return this.connections.Count == 0;
	}

	public bool IsFemale()
	{
		return this.socket.female;
	}

	public bool IsMale()
	{
		return this.socket.male;
	}

	public bool IsOccupied()
	{
		return this.connections.Count >= this.capacity;
	}

	public void LeavePool()
	{
	}

	public void Remove(EntityLink entity)
	{
		this.connections.Remove(entity);
	}

	public void Setup(BaseEntity owner, Socket_Base socket)
	{
		this.owner = owner;
		this.socket = socket;
		if (socket.monogamous)
		{
			this.capacity = 1;
		}
	}
}