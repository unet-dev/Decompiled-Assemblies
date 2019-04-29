using System;
using UnityEngine;

public class Socket_Base : PrefabAttribute
{
	public bool male = true;

	public bool maleDummy;

	public bool female;

	public bool femaleDummy;

	public bool monogamous;

	[NonSerialized]
	public Vector3 position;

	[NonSerialized]
	public Quaternion rotation;

	public Vector3 selectSize = new Vector3(2f, 0.1f, 2f);

	public Vector3 selectCenter = new Vector3(0f, 0f, 1f);

	[HideInInspector]
	public string socketName;

	[NonSerialized]
	public SocketMod[] socketMods;

	public Socket_Base()
	{
	}

	protected override void AttributeSetup(GameObject rootObj, string name, bool serverside, bool clientside, bool bundling)
	{
		base.AttributeSetup(rootObj, name, serverside, clientside, bundling);
		this.position = base.transform.position;
		this.rotation = base.transform.rotation;
		this.socketMods = base.GetComponentsInChildren<SocketMod>(true);
		SocketMod[] socketModArray = this.socketMods;
		for (int i = 0; i < (int)socketModArray.Length; i++)
		{
			socketModArray[i].baseSocket = this;
		}
	}

	public virtual bool CanConnect(Vector3 position, Quaternion rotation, Socket_Base socket, Vector3 socketPosition, Quaternion socketRotation)
	{
		return this.IsCompatible(socket);
	}

	public virtual bool CheckSocketMods(Construction.Placement placement)
	{
		int i;
		SocketMod[] socketModArray = this.socketMods;
		for (i = 0; i < (int)socketModArray.Length; i++)
		{
			socketModArray[i].ModifyPlacement(placement);
		}
		socketModArray = this.socketMods;
		for (i = 0; i < (int)socketModArray.Length; i++)
		{
			SocketMod socketMod = socketModArray[i];
			if (!socketMod.DoCheck(placement))
			{
				if (socketMod.FailedPhrase.IsValid())
				{
					Construction.lastPlacementError = string.Concat("Failed Check: (", socketMod.FailedPhrase.translated, ")");
				}
				return false;
			}
		}
		return true;
	}

	public virtual Construction.Placement DoPlacement(Construction.Target target)
	{
		Quaternion quaternion = Quaternion.LookRotation(target.normal, Vector3.up) * Quaternion.Euler(target.rotation);
		Vector3 vector3 = target.position;
		vector3 = vector3 - (quaternion * this.position);
		return new Construction.Placement()
		{
			rotation = quaternion,
			position = vector3
		};
	}

	protected override Type GetIndexedType()
	{
		return typeof(Socket_Base);
	}

	public OBB GetSelectBounds(Vector3 position, Quaternion rotation)
	{
		return new OBB(position + (rotation * this.worldPosition), Vector3.one, rotation * this.worldRotation, new Bounds(this.selectCenter, this.selectSize));
	}

	public Vector3 GetSelectPivot(Vector3 position, Quaternion rotation)
	{
		return position + (rotation * this.worldPosition);
	}

	public virtual bool IsCompatible(Socket_Base socket)
	{
		if (socket == null)
		{
			return false;
		}
		if (!socket.male && !this.male)
		{
			return false;
		}
		if (!socket.female && !this.female)
		{
			return false;
		}
		return socket.GetType() == base.GetType();
	}

	public virtual bool TestTarget(Construction.Target target)
	{
		return target.socket != null;
	}
}