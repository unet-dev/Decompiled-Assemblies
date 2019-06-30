using System;
using UnityEngine;

public class ArcadeEntityController : BaseMonoBehaviour
{
	public BaseArcadeGame parentGame;

	public ArcadeEntity arcadeEntity;

	public ArcadeEntity sourceEntity;

	public Vector3 heading
	{
		get
		{
			return this.arcadeEntity.heading;
		}
		set
		{
			this.arcadeEntity.heading = value;
		}
	}

	public Vector3 positionLocal
	{
		get
		{
			return this.arcadeEntity.transform.localPosition;
		}
		set
		{
			this.arcadeEntity.transform.localPosition = value;
		}
	}

	public Vector3 positionWorld
	{
		get
		{
			return this.arcadeEntity.transform.position;
		}
		set
		{
			this.arcadeEntity.transform.position = value;
		}
	}

	public ArcadeEntityController()
	{
	}
}