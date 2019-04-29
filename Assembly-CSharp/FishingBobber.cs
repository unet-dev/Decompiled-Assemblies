using System;
using UnityEngine;

public class FishingBobber : BaseCombatEntity
{
	public Transform centerOfMass;

	public Rigidbody myRigidBody;

	public FishingBobber()
	{
	}

	public override void ServerInit()
	{
		this.myRigidBody.centerOfMass = this.centerOfMass.localPosition;
		base.ServerInit();
	}
}