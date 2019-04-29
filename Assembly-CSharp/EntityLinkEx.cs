using Facepunch;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class EntityLinkEx
{
	public static void AddLinks(this List<EntityLink> links, BaseEntity entity, Socket_Base[] sockets)
	{
		for (int i = 0; i < (int)sockets.Length; i++)
		{
			Socket_Base socketBase = sockets[i];
			EntityLink entityLink = Pool.Get<EntityLink>();
			entityLink.Setup(entity, socketBase);
			links.Add(entityLink);
		}
	}

	public static void ClearLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			links[i].Clear();
		}
	}

	public static void FreeLinks(this List<EntityLink> links)
	{
		for (int i = 0; i < links.Count; i++)
		{
			EntityLink item = links[i];
			item.Clear();
			Pool.Free<EntityLink>(ref item);
		}
		links.Clear();
	}
}