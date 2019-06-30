using ConVar;
using Facepunch;
using Facepunch.Network.Raknet;
using Facepunch.Utility;
using Network;
using Oxide.Core;
using Rust;
using Rust.Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TMPro;
using UnityEngine;

public class Bootstrap : SingletonComponent<Bootstrap>
{
	internal static bool bootstrapInitRun;

	public static bool isErrored;

	public string messageString = "Loading...";

	public CanvasGroup BootstrapUiCanvas;

	public GameObject errorPanel;

	public TextMeshProUGUI errorText;

	public TextMeshProUGUI statusText;

	public static bool isPresent
	{
		get
		{
			if (Bootstrap.bootstrapInitRun)
			{
				return true;
			}
			if (UnityEngine.Object.FindObjectsOfType<GameSetup>().Count<GameSetup>() > 0)
			{
				return true;
			}
			return false;
		}
	}

	public static bool needsSetup
	{
		get
		{
			return !Bootstrap.bootstrapInitRun;
		}
	}

	static Bootstrap()
	{
	}

	public Bootstrap()
	{
	}

	private IEnumerator DedicatedServerStartup()
	{
		Bootstrap bootstrap = null;
		Rust.Application.isLoading = true;
		Bootstrap.WriteToLog("Skinnable Warmup");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		GameManifest.LoadAssets();
		Bootstrap.WriteToLog("Loading Scene");
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		UnityEngine.Physics.solverIterationCount = 3;
		QualitySettings.SetQualityLevel(0);
		UnityEngine.Object.DontDestroyOnLoad(bootstrap.gameObject);
		UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server_console.prefab", true));
		bootstrap.StartupShared();
		World.InitSize(ConVar.Server.worldsize);
		World.InitSeed(ConVar.Server.seed);
		World.InitSalt(ConVar.Server.salt);
		World.Url = ConVar.Server.levelurl;
		LevelManager.LoadLevel(ConVar.Server.level, true);
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return bootstrap.StartCoroutine(FileSystem_Warmup.Run(0.2f, new Action<string>(Bootstrap.WriteToLog), "Asset Warmup ({0}/{1})"));
		yield return bootstrap.StartCoroutine(Bootstrap.StartServer(!Facepunch.CommandLine.HasSwitch("-skipload"), "", false));
		if (!UnityEngine.Object.FindObjectOfType<Performance>())
		{
			UnityEngine.Object.DontDestroyOnLoad(GameManager.server.CreatePrefab("assets/bundled/prefabs/system/performance.prefab", true));
		}
		Facepunch.Pool.Clear();
		Rust.GC.Collect();
		Rust.Application.isLoading = false;
	}

	public void ExitGame()
	{
		UnityEngine.Debug.Log("Exiting due to Exit Game button on bootstrap error panel");
		UnityEngine.Application.Quit();
	}

	public static void Init_Config()
	{
		ConsoleNetwork.Init();
		ConsoleSystem.UpdateValuesFromCommandLine();
		ConsoleSystem.Run(ConsoleSystem.Option.Server, "server.readcfg", Array.Empty<object>());
		ServerUsers.Load();
	}

	public static void Init_Systems()
	{
		Facepunch.Application.Initialize(new Integration());
		Facepunch.Performance.GetMemoryUsage = () => SystemInfoEx.systemMemoryUsed;
	}

	public static void Init_Tier0()
	{
		Bootstrap.RunDefaults();
		GameSetup.RunOnce = true;
		Bootstrap.bootstrapInitRun = true;
		ConsoleSystem.Index.Initialize(ConsoleGen.All);
		UnityButtons.Register();
		Output.Install();
		Facepunch.Pool.ResizeBuffer<Networkable>(65536);
		Facepunch.Pool.ResizeBuffer<EntityLink>(65536);
		Facepunch.Pool.FillBuffer<Networkable>(2147483647);
		Facepunch.Pool.FillBuffer<EntityLink>(2147483647);
		Bootstrap.NetworkInit();
		string str = Facepunch.CommandLine.Full.Replace(Facepunch.CommandLine.GetSwitch("-rcon.password", Facepunch.CommandLine.GetSwitch("+rcon.password", "RCONPASSWORD")), "******");
		Bootstrap.WriteToLog(string.Concat("Command Line: ", str));
		Interface.Initialize();
	}

	public static IEnumerator LoadingUpdate(string str)
	{
		if (!SingletonComponent<Bootstrap>.Instance)
		{
			yield break;
		}
		SingletonComponent<Bootstrap>.Instance.messageString = str;
		yield return CoroutineEx.waitForEndOfFrame;
		yield return CoroutineEx.waitForEndOfFrame;
	}

	public static void NetworkInit()
	{
		Network.Net.sv = new Facepunch.Network.Raknet.Server();
	}

	public static void RunDefaults()
	{
		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
		UnityEngine.Application.targetFrameRate = 256;
		UnityEngine.Time.fixedDeltaTime = 0.0625f;
		UnityEngine.Time.maximumDeltaTime = 0.125f;
	}

	private IEnumerator Start()
	{
		Bootstrap bootstrap = null;
		int num;
		Bootstrap.WriteToLog("Bootstrap Startup");
		ExceptionReporter.InitializeFromUrl("https://83df169465e84da091c1a3cd2fbffeee:3671b903f9a840ecb68411cf946ab9b6@sentry.io/51080");
		num = (!Facepunch.Utility.CommandLine.Full.Contains("-official") ? 0 : (int)Facepunch.Utility.CommandLine.Full.Contains("+official"));
		ExceptionReporter.Disabled = num == 0;
		Bootstrap.WriteToLog(SystemInfoGeneralText.currentInfo);
		UnityEngine.Texture.SetGlobalAnisotropicFilteringLimits(1, 16);
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("Loading Bundles"));
		FileSystem.Backend = new AssetBundleBackend("Bundles/Bundles");
		if (FileSystem.Backend.isError)
		{
			bootstrap.ThrowError(FileSystem.Backend.loadingError);
		}
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("Loading Game Manifest"));
		GameManifest.Load();
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("DONE!"));
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("Running Self Check"));
		SelfCheck.Run();
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Tier0"));
		Bootstrap.Init_Tier0();
		ConsoleSystem.UpdateValuesFromCommandLine();
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Systems"));
		Bootstrap.Init_Systems();
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("Bootstrap Config"));
		Bootstrap.Init_Config();
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return bootstrap.StartCoroutine(Bootstrap.LoadingUpdate("Loading Items"));
		ItemManager.Initialize();
		if (Bootstrap.isErrored)
		{
			yield break;
		}
		yield return bootstrap.StartCoroutine(bootstrap.DedicatedServerStartup());
		GameManager.Destroy(bootstrap.gameObject, 0f);
	}

	public static IEnumerator StartServer(bool doLoad, string saveFileOverride, bool allowOutOfDateSaves)
	{
		float single = UnityEngine.Time.timeScale;
		if (ConVar.Time.pausewhileloading)
		{
			UnityEngine.Time.timeScale = 0f;
		}
		RCon.Initialize();
		BaseEntity.Query.Server = new BaseEntity.Query.EntityTree(8096f);
		if (SingletonComponent<WorldSetup>.Instance)
		{
			yield return SingletonComponent<WorldSetup>.Instance.StartCoroutine(SingletonComponent<WorldSetup>.Instance.InitCoroutine());
		}
		if (SingletonComponent<DynamicNavMesh>.Instance && SingletonComponent<DynamicNavMesh>.Instance.enabled && !AiManager.nav_disable)
		{
			yield return SingletonComponent<DynamicNavMesh>.Instance.StartCoroutine(SingletonComponent<DynamicNavMesh>.Instance.UpdateNavMeshAndWait());
		}
		if (SingletonComponent<AiManager>.Instance && SingletonComponent<AiManager>.Instance.enabled)
		{
			SingletonComponent<AiManager>.Instance.Initialize();
			if (!AiManager.nav_disable && AI.npc_enable && TerrainMeta.Path != null)
			{
				foreach (MonumentInfo monument in TerrainMeta.Path.Monuments)
				{
					if (!monument.HasNavmesh)
					{
						continue;
					}
					yield return monument.StartCoroutine(monument.GetMonumentNavMesh().UpdateNavMeshAndWait());
				}
			}
		}
		GameObject gameObject = GameManager.server.CreatePrefab("assets/bundled/prefabs/system/server.prefab", true);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		ServerMgr component = gameObject.GetComponent<ServerMgr>();
		component.Initialize(doLoad, saveFileOverride, allowOutOfDateSaves, false);
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		ColliderGrid.RefreshAll();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityLinks();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntitySupports();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.InitializeEntityConditionals();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		ColliderGrid.RefreshAll();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		SaveRestore.GetSaveCache();
		yield return CoroutineEx.waitForSecondsRealtime(0.1f);
		component.OpenConnection();
		if (ConVar.Time.pausewhileloading)
		{
			UnityEngine.Time.timeScale = single;
		}
		Bootstrap.WriteToLog("Server startup complete");
	}

	private void StartupShared()
	{
		Interface.CallHook("InitLogging");
		ItemManager.Initialize();
	}

	public void ThrowError(string error)
	{
		UnityEngine.Debug.Log(string.Concat("ThrowError: ", error));
		this.errorPanel.SetActive(true);
		this.errorText.text = error;
		Bootstrap.isErrored = true;
	}

	public static void WriteToLog(string str)
	{
		DebugEx.Log(str, StackTraceLogType.None);
	}
}