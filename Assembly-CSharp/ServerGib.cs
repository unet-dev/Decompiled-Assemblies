using ProtoBuf;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ServerGib : BaseCombatEntity
{
	public GameObject _gibSource;

	public string _gibName;

	public PhysicMaterial physicsMaterial;

	private MeshCollider meshCollider;

	private Rigidbody rigidBody;

	public ServerGib()
	{
	}

	public override float BoundsPadding()
	{
		return 3f;
	}

	public static List<ServerGib> CreateGibs(string entityToCreatePath, GameObject creator, GameObject gibSource, Vector3 inheritVelocity, float spreadVelocity)
	{
		List<ServerGib> serverGibs = new List<ServerGib>();
		MeshRenderer[] componentsInChildren = gibSource.GetComponentsInChildren<MeshRenderer>(true);
		for (int i = 0; i < (int)componentsInChildren.Length; i++)
		{
			MeshRenderer meshRenderer = componentsInChildren[i];
			MeshFilter component = meshRenderer.GetComponent<MeshFilter>();
			Vector3 vector3 = meshRenderer.transform.localPosition;
			Vector3 vector31 = vector3.normalized;
			Matrix4x4 matrix4x4 = creator.transform.localToWorldMatrix;
			Vector3 vector32 = matrix4x4.MultiplyPoint(meshRenderer.transform.localPosition) + (vector31 * 0.5f);
			Quaternion quaternion = creator.transform.rotation * meshRenderer.transform.localRotation;
			BaseEntity baseEntity = GameManager.server.CreateEntity(entityToCreatePath, vector32, quaternion, true);
			if (baseEntity)
			{
				ServerGib serverGib = baseEntity.GetComponent<ServerGib>();
				serverGib.transform.position = vector32;
				serverGib.transform.rotation = quaternion;
				serverGib._gibName = meshRenderer.name;
				serverGib.PhysicsInit(component.sharedMesh);
				vector3 = meshRenderer.transform.localPosition;
				Vector3 vector33 = vector3.normalized * spreadVelocity;
				serverGib.rigidBody.velocity = inheritVelocity + vector33;
				Rigidbody rigidbody = serverGib.rigidBody;
				vector3 = Vector3Ex.Range(-1f, 1f);
				rigidbody.angularVelocity = vector3.normalized * 1f;
				serverGib.rigidBody.WakeUp();
				serverGib.Spawn();
				serverGibs.Add(serverGib);
			}
		}
		foreach (ServerGib serverGib1 in serverGibs)
		{
			foreach (ServerGib serverGib2 in serverGibs)
			{
				if (serverGib1 == serverGib2)
				{
					continue;
				}
				Physics.IgnoreCollision(serverGib2.GetCollider(), serverGib1.GetCollider(), true);
			}
		}
		return serverGibs;
	}

	public MeshCollider GetCollider()
	{
		return this.meshCollider;
	}

	public virtual void PhysicsInit(Mesh mesh)
	{
		this.meshCollider = base.gameObject.AddComponent<MeshCollider>();
		this.meshCollider.sharedMesh = mesh;
		this.meshCollider.convex = true;
		this.meshCollider.material = this.physicsMaterial;
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = true;
		Vector3 vector3 = this.meshCollider.bounds.size;
		float single = vector3.magnitude;
		vector3 = this.meshCollider.bounds.size;
		rigidbody.mass = Mathf.Clamp(single * vector3.magnitude * 20f, 10f, 2000f);
		rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
		if (base.isServer)
		{
			rigidbody.drag = 0.1f;
			rigidbody.angularDrag = 0.1f;
		}
		this.rigidBody = rigidbody;
		base.gameObject.layer = LayerMask.NameToLayer("Default");
		if (base.isClient)
		{
			rigidbody.isKinematic = true;
		}
	}

	public void RemoveMe()
	{
		base.Kill(BaseNetworkable.DestroyMode.None);
	}

	public override void Save(BaseNetworkable.SaveInfo info)
	{
		base.Save(info);
		if (!info.forDisk && this._gibName != "")
		{
			info.msg.servergib = new ProtoBuf.ServerGib()
			{
				gibName = this._gibName
			};
		}
	}

	public override void ServerInit()
	{
		base.ServerInit();
		base.Invoke(new Action(this.RemoveMe), 1800f);
	}
}