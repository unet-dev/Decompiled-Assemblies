using ConVar;
using Network;
using System;
using UnityEngine;
using UnityEngine.Assertions;

public class RHIB : MotorRowboat
{
	public GameObject steeringWheel;

	[ServerVar(Help="Population active on the server")]
	public static float rhibpopulation;

	private float targetGasPedal;

	static RHIB()
	{
		RHIB.rhibpopulation = 1f;
	}

	public RHIB()
	{
	}

	public override void DriverInput(InputState inputState, BasePlayer player)
	{
		base.DriverInput(inputState, player);
		if (inputState.IsDown(BUTTON.FORWARD))
		{
			this.targetGasPedal = 1f;
		}
		else if (!inputState.IsDown(BUTTON.BACKWARD))
		{
			this.targetGasPedal = 0f;
		}
		else
		{
			this.targetGasPedal = -0.5f;
		}
		if (inputState.IsDown(BUTTON.LEFT))
		{
			this.steering = 1f;
			return;
		}
		if (inputState.IsDown(BUTTON.RIGHT))
		{
			this.steering = -1f;
			return;
		}
		this.steering = 0f;
	}

	public override bool EngineOn()
	{
		return base.EngineOn();
	}

	public override bool HasFuel(bool forceCheck = false)
	{
		return base.HasFuel(forceCheck);
	}

	public override bool OnRpcMessage(BasePlayer player, uint rpc, Message msg)
	{
		bool flag;
		using (TimeWarning timeWarning = TimeWarning.New("RHIB.OnRpcMessage", 0.1f))
		{
			if (rpc != 1382282393 || !(player != null))
			{
				return base.OnRpcMessage(player, rpc, msg);
			}
			else
			{
				Assert.IsTrue(player.isServer, "SV_RPC Message is using a clientside player!");
				if (Global.developer > 2)
				{
					Debug.Log(string.Concat("SV_RPCMessage: ", player, " - Server_Release "));
				}
				using (TimeWarning timeWarning1 = TimeWarning.New("Server_Release", 0.1f))
				{
					try
					{
						using (TimeWarning timeWarning2 = TimeWarning.New("Call", 0.1f))
						{
							BaseEntity.RPCMessage rPCMessage = new BaseEntity.RPCMessage()
							{
								connection = msg.connection,
								player = player,
								read = msg.read
							};
							this.Server_Release(rPCMessage);
						}
					}
					catch (Exception exception)
					{
						player.Kick("RPC Error in Server_Release");
						Debug.LogException(exception);
					}
				}
				flag = true;
			}
		}
		return flag;
	}

	[RPC_Server]
	public void Server_Release(BaseEntity.RPCMessage msg)
	{
		Debug.Log("Rhib server release!");
		if (base.GetParentEntity() == null)
		{
			return;
		}
		base.SetParent(null, true, true);
		this.myRigidBody.isKinematic = false;
	}

	public override bool UseFuel(float seconds)
	{
		return base.UseFuel(seconds);
	}

	public override void VehicleFixedUpdate()
	{
		this.gasPedal = Mathf.MoveTowards(this.gasPedal, this.targetGasPedal, UnityEngine.Time.fixedDeltaTime * 1f);
		base.VehicleFixedUpdate();
	}
}