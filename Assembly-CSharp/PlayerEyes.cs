using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerEyes : EntityComponent<BasePlayer>
{
	public readonly static Vector3 EyeOffset;

	public readonly static Vector3 DuckOffset;

	public Vector3 thirdPersonSleepingOffset = new Vector3(0.43f, 1.25f, 0.7f);

	public LazyAimProperties defaultLazyAim;

	private Vector3 viewOffset = Vector3.zero;

	public Quaternion bodyRotation
	{
		get;
		set;
	}

	public Vector3 center
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector3 = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity);
				if (vector3 != Vector3.zero)
				{
					return vector3;
				}
			}
			return base.transform.position + (base.transform.up * (PlayerEyes.EyeOffset.y + PlayerEyes.DuckOffset.y));
		}
	}

	public Quaternion headRotation
	{
		get;
		set;
	}

	public Vector3 offset
	{
		get
		{
			return base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y);
		}
	}

	public Quaternion parentRotation
	{
		get
		{
			if (base.transform.parent == null)
			{
				return Quaternion.identity;
			}
			Quaternion quaternion = base.transform.parent.rotation;
			return Quaternion.Euler(0f, quaternion.eulerAngles.y, 0f);
		}
	}

	public Vector3 position
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector3 = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity);
				if (vector3 != Vector3.zero)
				{
					return vector3;
				}
			}
			return base.transform.position + (base.transform.up * (PlayerEyes.EyeOffset.y + this.viewOffset.y));
		}
	}

	public Quaternion rotation
	{
		get
		{
			return this.parentRotation * this.bodyRotation;
		}
		set
		{
			this.bodyRotation = Quaternion.Inverse(this.parentRotation) * value;
		}
	}

	public Quaternion rotationLook
	{
		get;
		set;
	}

	public Vector3 worldCrouchedPosition
	{
		get
		{
			return this.worldStandingPosition + PlayerEyes.DuckOffset;
		}
	}

	public Vector3 worldMountedPosition
	{
		get
		{
			if (base.baseEntity && base.baseEntity.isMounted)
			{
				Vector3 vector3 = base.baseEntity.GetMounted().EyePositionForPlayer(base.baseEntity);
				if (vector3 != Vector3.zero)
				{
					return vector3;
				}
			}
			return this.worldStandingPosition;
		}
	}

	public Vector3 worldStandingPosition
	{
		get
		{
			return base.transform.position + PlayerEyes.EyeOffset;
		}
	}

	static PlayerEyes()
	{
		PlayerEyes.EyeOffset = new Vector3(0f, 1.5f, 0f);
		PlayerEyes.DuckOffset = new Vector3(0f, -0.6f, 0f);
	}

	public PlayerEyes()
	{
	}

	public Vector3 BodyForward()
	{
		return this.rotation * Vector3.forward;
	}

	public Ray BodyRay()
	{
		return new Ray(this.position, this.BodyForward());
	}

	public Vector3 BodyRight()
	{
		return this.rotation * Vector3.right;
	}

	public Vector3 BodyUp()
	{
		return this.rotation * Vector3.up;
	}

	public Vector3 HeadForward()
	{
		return (this.rotation * this.headRotation) * Vector3.forward;
	}

	public Ray HeadRay()
	{
		return new Ray(this.position, this.HeadForward());
	}

	public Vector3 HeadRight()
	{
		return (this.rotation * this.headRotation) * Vector3.right;
	}

	public Vector3 HeadUp()
	{
		return (this.rotation * this.headRotation) * Vector3.up;
	}

	public Vector3 MovementForward()
	{
		Quaternion quaternion = this.rotation;
		return Quaternion.Euler(new Vector3(0f, quaternion.eulerAngles.y, 0f)) * Vector3.forward;
	}

	public Vector3 MovementRight()
	{
		Quaternion quaternion = this.rotation;
		return Quaternion.Euler(new Vector3(0f, quaternion.eulerAngles.y, 0f)) * Vector3.right;
	}

	public void NetworkUpdate(Quaternion rot)
	{
		this.viewOffset = (base.baseEntity.IsDucked() ? PlayerEyes.DuckOffset : Vector3.zero);
		this.bodyRotation = rot;
		this.headRotation = Quaternion.identity;
	}
}