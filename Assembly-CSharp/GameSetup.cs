using ConVar;
using Network;
using Rust;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetup : MonoBehaviour
{
	public static bool RunOnce;

	public bool startServer = true;

	public string clientConnectCommand = "client.connect 127.0.0.1:28015";

	public bool loadMenu = true;

	public bool loadLevel;

	public string loadLevelScene = "";

	public bool loadSave;

	public string loadSaveFile = "";

	static GameSetup()
	{
	}

	public GameSetup()
	{
	}

	protected void Awake()
	{
		if (GameSetup.RunOnce)
		{
			GameManager.Destroy(base.gameObject, 0f);
			return;
		}
		GameManifest.Load();
		GameManifest.LoadAssets();
		GameSetup.RunOnce = true;
		if (Bootstrap.needsSetup)
		{
			Bootstrap.Init_Tier0();
			Bootstrap.Init_Systems();
			Bootstrap.Init_Config();
		}
		base.StartCoroutine(this.DoGameSetup());
	}

	private IEnumerator DoGameSetup()
	{
		GameSetup gameSetup = null;
		Rust.Application.isLoading = true;
		TerrainMeta.InitNoTerrain();
		ItemManager.Initialize();
		LevelManager.CurrentLevelName = SceneManager.GetActiveScene().name;
		if (gameSetup.loadLevel && !string.IsNullOrEmpty(gameSetup.loadLevelScene))
		{
			Network.Net.sv.Reset();
			ConVar.Server.level = gameSetup.loadLevelScene;
			LoadingScreen.Update("LOADING SCENE");
			UnityEngine.Application.LoadLevelAdditive(gameSetup.loadLevelScene);
			LoadingScreen.Update(string.Concat(gameSetup.loadLevelScene.ToUpper(), " LOADED"));
		}
		if (gameSetup.startServer)
		{
			yield return gameSetup.StartCoroutine(gameSetup.StartServer());
		}
		yield return null;
		Rust.Application.isLoading = false;
	}

	private IEnumerator StartServer()
	{
		GameSetup gameSetup = null;
		ConVar.GC.collect();
		ConVar.GC.unload();
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return gameSetup.StartCoroutine(Bootstrap.StartServer(gameSetup.loadSave, gameSetup.loadSaveFile, true));
	}
}