using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseArcadeGame : BaseMonoBehaviour
{
	public static List<BaseArcadeGame> globalActiveGames;

	public Camera cameraToRender;

	public RenderTexture renderTexture;

	public Texture2D distantTexture;

	public Transform center;

	public int frameRate = 30;

	public Dictionary<uint, ArcadeEntity> activeArcadeEntities = new Dictionary<uint, ArcadeEntity>();

	public Sprite[] spriteManifest;

	public ArcadeEntity[] entityManifest;

	public bool clientside;

	public bool clientsideInput = true;

	public const int spriteIndexInvisible = 1555;

	public GameObject arcadeEntityPrefab;

	public BaseArcadeMachine ownerMachine;

	public static int gameOffsetIndex;

	private bool isAuthorative;

	public Canvas canvas;

	static BaseArcadeGame()
	{
		BaseArcadeGame.globalActiveGames = new List<BaseArcadeGame>();
		BaseArcadeGame.gameOffsetIndex = 0;
	}

	public BaseArcadeGame()
	{
	}

	public BasePlayer GetHostPlayer()
	{
		if (!this.ownerMachine)
		{
			return null;
		}
		return this.ownerMachine.GetDriver();
	}
}