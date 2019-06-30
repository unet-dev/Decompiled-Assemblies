using System;
using UnityEngine;

public class ChippyBoss : SpriteArcadeEntity
{
	public Vector2 roamDistance;

	public float animationSpeed = 0.5f;

	public Sprite[] animationFrames;

	public ArcadeEntity bulletTest;

	public SpriteRenderer flashRenderer;

	public ChippyBoss.BossDamagePoint[] damagePoints;

	public ChippyBoss()
	{
	}

	[Serializable]
	public class BossDamagePoint
	{
		public BoxCollider hitBox;

		public float health;

		public ArcadeEntityController damagePrefab;

		public ArcadeEntityController damageInstance;

		public bool destroyed;

		public BossDamagePoint()
		{
		}
	}
}