using Facepunch;
using GameAnalyticsSDK.Events;
using GameAnalyticsSDK.Net;
using GameAnalyticsSDK.Setup;
using GameAnalyticsSDK.State;
using GameAnalyticsSDK.Wrapper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using UnityEngine;

namespace GameAnalyticsSDK
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(GA_SpecialEvents))]
	public class GameAnalytics : MonoBehaviour
	{
		private static Settings _settings;

		private static GameAnalyticsSDK.GameAnalytics _instance;

		private static bool _hasInitializeBeenCalled;

		public static Settings SettingsGA
		{
			get
			{
				if (GameAnalyticsSDK.GameAnalytics._settings == null)
				{
					GameAnalyticsSDK.GameAnalytics.InitAPI();
				}
				return GameAnalyticsSDK.GameAnalytics._settings;
			}
			private set
			{
				GameAnalyticsSDK.GameAnalytics._settings = value;
			}
		}

		public GameAnalytics()
		{
		}

		public void Awake()
		{
			if (!UnityEngine.Application.isPlaying)
			{
				return;
			}
			if (GameAnalyticsSDK.GameAnalytics._instance != null)
			{
				Debug.LogWarning("Destroying duplicate GameAnalytics object - only one is allowed per scene!");
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			GameAnalyticsSDK.GameAnalytics._instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			UnityEngine.Application.logMessageReceived += new UnityEngine.Application.LogCallback(GA_Debug.HandleLog);
			GameAnalyticsSDK.GameAnalytics.InternalInitialize();
		}

		public static void CommandCenterUpdated()
		{
			if (GameAnalyticsSDK.GameAnalytics.OnCommandCenterUpdatedEvent != null)
			{
				GameAnalyticsSDK.GameAnalytics.OnCommandCenterUpdatedEvent();
			}
		}

		public static void EndSession()
		{
			GA_Wrapper.EndSession();
		}

		public static string GetCommandCenterValueAsString(string key)
		{
			return GameAnalyticsSDK.GameAnalytics.GetCommandCenterValueAsString(key, null);
		}

		public static string GetCommandCenterValueAsString(string key, string defaultValue)
		{
			return GA_Wrapper.GetCommandCenterValueAsString(key, defaultValue);
		}

		public static string GetConfigurationsContentAsString()
		{
			return GA_Wrapper.GetConfigurationsContentAsString();
		}

		private static int GetPlatformIndex()
		{
			int num = -1;
			RuntimePlatform runtimePlatform = UnityEngine.Application.platform;
			if (runtimePlatform == RuntimePlatform.IPhonePlayer)
			{
				num = (GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.Contains(runtimePlatform) ? GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.IndexOf(runtimePlatform) : GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.tvOS));
			}
			else if (runtimePlatform != RuntimePlatform.tvOS)
			{
				num = (runtimePlatform == RuntimePlatform.MetroPlayerARM || runtimePlatform == RuntimePlatform.MetroPlayerX64 || runtimePlatform == RuntimePlatform.MetroPlayerX86 || runtimePlatform == RuntimePlatform.MetroPlayerARM || runtimePlatform == RuntimePlatform.MetroPlayerX64 || runtimePlatform == RuntimePlatform.MetroPlayerX86 ? GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.MetroPlayerARM) : GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.IndexOf(runtimePlatform));
			}
			else
			{
				num = (GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.Contains(runtimePlatform) ? GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.IndexOf(runtimePlatform) : GameAnalyticsSDK.GameAnalytics.SettingsGA.Platforms.IndexOf(RuntimePlatform.IPhonePlayer));
			}
			return num;
		}

		private static string GetUnityVersion()
		{
			int num;
			string str = "";
			string[] strArrays = UnityEngine.Application.unityVersion.Split(new char[] { '.' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				if (!int.TryParse(strArrays[i], out num))
				{
					string[] strArrays1 = Regex.Split(strArrays[i], "[^\\d]+");
					if (strArrays1.Length != 0 && int.TryParse(strArrays1[0], out num))
					{
						str = string.Concat(str, ".", strArrays1[0]);
					}
				}
				else
				{
					str = (i != 0 ? string.Concat(str, ".", strArrays[i]) : strArrays[i]);
				}
			}
			return str;
		}

		private static void InitAPI()
		{
			try
			{
				GameAnalyticsSDK.GameAnalytics._settings = (Settings)Resources.Load("GameAnalytics/Settings", typeof(Settings));
				GAState.Init();
			}
			catch (Exception exception)
			{
				Debug.Log(string.Concat("Error getting Settings in InitAPI: ", exception.Message));
			}
		}

		public static void Initialize()
		{
			if (UnityEngine.Application.isEditor)
			{
				GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled = true;
				return;
			}
			GA_Wrapper.Initialize("ab258be05882d64cb4e285ac8e8110f2", "e9e34daf4b7aff6505eccfc99eef811136b4c96c");
			GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled = true;
		}

		private static void InternalInitialize()
		{
			if (!UnityEngine.Application.isPlaying)
			{
				return;
			}
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.InfoLogBuild)
			{
				GA_Setup.SetInfoLog(true);
			}
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.VerboseLogBuild)
			{
				GA_Setup.SetVerboseLog(true);
			}
			GA_Wrapper.SetUnitySdkVersion(string.Concat("unity ", Settings.VERSION));
			GA_Wrapper.SetUnityEngineVersion(string.Concat("unity ", GameAnalyticsSDK.GameAnalytics.GetUnityVersion()));
			GA_Wrapper.SetBuild(BuildInfo.Current.Scm.ChangeId);
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.CustomDimensions01.Count > 0)
			{
				GA_Setup.SetAvailableCustomDimensions01(GameAnalyticsSDK.GameAnalytics.SettingsGA.CustomDimensions01);
			}
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.CustomDimensions02.Count > 0)
			{
				GA_Setup.SetAvailableCustomDimensions02(GameAnalyticsSDK.GameAnalytics.SettingsGA.CustomDimensions02);
			}
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.CustomDimensions03.Count > 0)
			{
				GA_Setup.SetAvailableCustomDimensions03(GameAnalyticsSDK.GameAnalytics.SettingsGA.CustomDimensions03);
			}
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.ResourceItemTypes.Count > 0)
			{
				GA_Setup.SetAvailableResourceItemTypes(GameAnalyticsSDK.GameAnalytics.SettingsGA.ResourceItemTypes);
			}
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.ResourceCurrencies.Count > 0)
			{
				GA_Setup.SetAvailableResourceCurrencies(GameAnalyticsSDK.GameAnalytics.SettingsGA.ResourceCurrencies);
			}
			if (GameAnalyticsSDK.GameAnalytics.SettingsGA.UseManualSessionHandling)
			{
				GameAnalyticsSDK.GameAnalytics.SetEnabledManualSessionHandling(true);
			}
		}

		public static bool IsCommandCenterReady()
		{
			return GA_Wrapper.IsCommandCenterReady();
		}

		public static void NewBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Business.NewEvent(currency, amount, itemType, itemId, cartType, null);
		}

		public static void NewDesignEvent(string eventName)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Design.NewEvent(eventName, null);
		}

		public static void NewDesignEvent(string eventName, float eventValue)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Design.NewEvent(eventName, eventValue, null);
		}

		public static void NewErrorEvent(GAErrorSeverity severity, string message)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Error.NewEvent(severity, message, null);
		}

		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, null);
		}

		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, null);
		}

		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, progression03, null);
		}

		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, int score)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, score, null);
		}

		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, int score)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, score, null);
		}

		public static void NewProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Progression.NewEvent(progressionStatus, progression01, progression02, progression03, score, null);
		}

		public static void NewResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
		{
			if (!GameAnalyticsSDK.GameAnalytics._hasInitializeBeenCalled)
			{
				return;
			}
			GA_Resource.NewEvent(flowType, currency, amount, itemType, itemId, null);
		}

		private void OnApplicationQuit()
		{
			if (!GameAnalyticsSDK.GameAnalytics.SettingsGA.UseManualSessionHandling)
			{
				GameAnalyticsSDK.Net.GameAnalytics.OnStop();
			}
		}

		public void OnCommandCenterUpdated()
		{
			if (GameAnalyticsSDK.GameAnalytics.OnCommandCenterUpdatedEvent != null)
			{
				GameAnalyticsSDK.GameAnalytics.OnCommandCenterUpdatedEvent();
			}
		}

		private void OnDestroy()
		{
			if (!UnityEngine.Application.isPlaying)
			{
				return;
			}
			if (GameAnalyticsSDK.GameAnalytics._instance == this)
			{
				GameAnalyticsSDK.GameAnalytics._instance = null;
			}
		}

		public static void SetBirthYear(int birthYear)
		{
			GA_Setup.SetBirthYear(birthYear);
		}

		public static void SetBuildAllPlatforms(string build)
		{
			for (int i = 0; i < GameAnalyticsSDK.GameAnalytics.SettingsGA.Build.Count; i++)
			{
				GameAnalyticsSDK.GameAnalytics.SettingsGA.Build[i] = build;
			}
		}

		public static void SetCustomDimension01(string customDimension)
		{
			GA_Setup.SetCustomDimension01(customDimension);
		}

		public static void SetCustomDimension02(string customDimension)
		{
			GA_Setup.SetCustomDimension02(customDimension);
		}

		public static void SetCustomDimension03(string customDimension)
		{
			GA_Setup.SetCustomDimension03(customDimension);
		}

		public static void SetCustomId(string userId)
		{
			GA_Wrapper.SetCustomUserId(userId);
		}

		public static void SetEnabledManualSessionHandling(bool enabled)
		{
			GA_Wrapper.SetEnabledManualSessionHandling(enabled);
		}

		public static void SetFacebookId(string facebookId)
		{
			GA_Setup.SetFacebookId(facebookId);
		}

		public static void SetGender(GAGender gender)
		{
			GA_Setup.SetGender(gender);
		}

		public static void StartSession()
		{
			GA_Wrapper.StartSession();
		}

		public static event Action OnCommandCenterUpdatedEvent;
	}
}