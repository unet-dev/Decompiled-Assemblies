using GameAnalyticsSDK;
using GameAnalyticsSDK.Net;
using GameAnalyticsSDK.State;
using GameAnalyticsSDK.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameAnalyticsSDK.Wrapper
{
	public class GA_Wrapper
	{
		private readonly static GA_Wrapper.UnityCommandCenterListener unityCommandCenterListener;

		static GA_Wrapper()
		{
			GA_Wrapper.unityCommandCenterListener = new GA_Wrapper.UnityCommandCenterListener();
		}

		public GA_Wrapper()
		{
		}

		private static void addBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, string fields)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddBusinessEvent(currency, amount, itemType, itemId, cartType);
		}

		public static void AddBusinessEvent(string currency, int amount, string itemType, string itemId, string cartType, IDictionary<string, object> fields)
		{
			string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addBusinessEvent(currency, amount, itemType, itemId, cartType, jsonString);
		}

		private static void addDesignEvent(string eventId, string fields)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddDesignEvent(eventId, null);
		}

		public static void AddDesignEvent(string eventID, float eventValue, IDictionary<string, object> fields)
		{
			GA_Wrapper.addDesignEventWithValue(eventID, eventValue, GA_Wrapper.DictionaryToJsonString(fields));
		}

		public static void AddDesignEvent(string eventID, IDictionary<string, object> fields)
		{
			GA_Wrapper.addDesignEvent(eventID, GA_Wrapper.DictionaryToJsonString(fields));
		}

		private static void addDesignEventWithValue(string eventId, float value, string fields)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddDesignEvent(eventId, (double)value);
		}

		private static void addErrorEvent(int severity, string message, string fields)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddErrorEvent((EGAErrorSeverity)severity, message);
		}

		public static void AddErrorEvent(GAErrorSeverity severity, string message, IDictionary<string, object> fields)
		{
			GA_Wrapper.addErrorEvent((int)severity, message, GA_Wrapper.DictionaryToJsonString(fields));
		}

		private static void addProgressionEvent(int progressionStatus, string progression01, string progression02, string progression03, string fields)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddProgressionEvent((EGAProgressionStatus)progressionStatus, progression01, progression02, progression03);
		}

		public static void AddProgressionEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, IDictionary<string, object> fields)
		{
			string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addProgressionEvent((int)progressionStatus, progression01, progression02, progression03, jsonString);
		}

		private static void addProgressionEventWithScore(int progressionStatus, string progression01, string progression02, string progression03, int score, string fields)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddProgressionEvent((EGAProgressionStatus)progressionStatus, progression01, progression02, progression03, (double)score);
		}

		public static void AddProgressionEventWithScore(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score, IDictionary<string, object> fields)
		{
			string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addProgressionEventWithScore((int)progressionStatus, progression01, progression02, progression03, score, jsonString);
		}

		private static void addResourceEvent(int flowType, string currency, float amount, string itemType, string itemId, string fields)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddResourceEvent((EGAResourceFlowType)flowType, currency, amount, itemType, itemId);
		}

		public static void AddResourceEvent(GAResourceFlowType flowType, string currency, float amount, string itemType, string itemId, IDictionary<string, object> fields)
		{
			string jsonString = GA_Wrapper.DictionaryToJsonString(fields);
			GA_Wrapper.addResourceEvent((int)flowType, currency, amount, itemType, itemId, jsonString);
		}

		private static void configureAvailableCustomDimensions01(string list)
		{
			IList<object> objs = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayLists = new ArrayList();
			foreach (object obj in objs)
			{
				arrayLists.Add(obj);
			}
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableCustomDimensions01((string[])arrayLists.ToArray(typeof(string)));
		}

		private static void configureAvailableCustomDimensions02(string list)
		{
			IList<object> objs = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayLists = new ArrayList();
			foreach (object obj in objs)
			{
				arrayLists.Add(obj);
			}
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableCustomDimensions02((string[])arrayLists.ToArray(typeof(string)));
		}

		private static void configureAvailableCustomDimensions03(string list)
		{
			IList<object> objs = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayLists = new ArrayList();
			foreach (object obj in objs)
			{
				arrayLists.Add(obj);
			}
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableCustomDimensions03((string[])arrayLists.ToArray(typeof(string)));
		}

		private static void configureAvailableResourceCurrencies(string list)
		{
			IList<object> objs = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayLists = new ArrayList();
			foreach (object obj in objs)
			{
				arrayLists.Add(obj);
			}
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableResourceCurrencies((string[])arrayLists.ToArray(typeof(string)));
		}

		private static void configureAvailableResourceItemTypes(string list)
		{
			IList<object> objs = GA_MiniJSON.Deserialize(list) as IList<object>;
			ArrayList arrayLists = new ArrayList();
			foreach (object obj in objs)
			{
				arrayLists.Add(obj);
			}
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureAvailableResourceItemTypes((string[])arrayLists.ToArray(typeof(string)));
		}

		private static void configureBuild(string build)
		{
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureBuild(build);
		}

		private static void configureGameEngineVersion(string unityEngineVersion)
		{
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureGameEngineVersion(unityEngineVersion);
		}

		private static void configureSdkGameEngineVersion(string unitySdkVersion)
		{
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureSdkGameEngineVersion(unitySdkVersion);
		}

		private static void configureUserId(string userId)
		{
			GameAnalyticsSDK.Net.GameAnalytics.ConfigureUserId(userId);
		}

		private static string DictionaryToJsonString(IDictionary<string, object> dict)
		{
			Hashtable hashtables = new Hashtable();
			if (dict != null)
			{
				foreach (KeyValuePair<string, object> keyValuePair in dict)
				{
					hashtables.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return GA_MiniJSON.Serialize(hashtables);
		}

		public static void EndSession()
		{
			if (GAState.IsManualSessionHandlingEnabled())
			{
				GA_Wrapper.gameAnalyticsEndSession();
				return;
			}
			Debug.Log("Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
		}

		private static void gameAnalyticsEndSession()
		{
			GameAnalyticsSDK.Net.GameAnalytics.EndSession();
		}

		private static void gameAnalyticsStartSession()
		{
			GameAnalyticsSDK.Net.GameAnalytics.StartSession();
		}

		private static string getCommandCenterValueAsString(string key, string defaultValue)
		{
			return GameAnalyticsSDK.Net.GameAnalytics.GetCommandCenterValueAsString(key, defaultValue);
		}

		public static string GetCommandCenterValueAsString(string key, string defaultValue)
		{
			return GA_Wrapper.getCommandCenterValueAsString(key, defaultValue);
		}

		private static string getConfigurationsContentAsString()
		{
			return GameAnalyticsSDK.Net.GameAnalytics.GetConfigurationsAsString();
		}

		public static string GetConfigurationsContentAsString()
		{
			return GA_Wrapper.getConfigurationsContentAsString();
		}

		private static void initialize(string gamekey, string gamesecret)
		{
			GameAnalyticsSDK.Net.GameAnalytics.AddCommandCenterListener(GA_Wrapper.unityCommandCenterListener);
			GameAnalyticsSDK.Net.GameAnalytics.Initialize(gamekey, gamesecret);
		}

		public static void Initialize(string gamekey, string gamesecret)
		{
			GA_Wrapper.initialize(gamekey, gamesecret);
		}

		private static bool isCommandCenterReady()
		{
			return GameAnalyticsSDK.Net.GameAnalytics.IsCommandCenterReady();
		}

		public static bool IsCommandCenterReady()
		{
			return GA_Wrapper.isCommandCenterReady();
		}

		public static void SetAvailableCustomDimensions01(string list)
		{
			GA_Wrapper.configureAvailableCustomDimensions01(list);
		}

		public static void SetAvailableCustomDimensions02(string list)
		{
			GA_Wrapper.configureAvailableCustomDimensions02(list);
		}

		public static void SetAvailableCustomDimensions03(string list)
		{
			GA_Wrapper.configureAvailableCustomDimensions03(list);
		}

		public static void SetAvailableResourceCurrencies(string list)
		{
			GA_Wrapper.configureAvailableResourceCurrencies(list);
		}

		public static void SetAvailableResourceItemTypes(string list)
		{
			GA_Wrapper.configureAvailableResourceItemTypes(list);
		}

		private static void setBirthYear(int birthYear)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetBirthYear(birthYear);
		}

		public static void SetBirthYear(int birthYear)
		{
			GA_Wrapper.setBirthYear(birthYear);
		}

		public static void SetBuild(string build)
		{
			GA_Wrapper.configureBuild(build);
		}

		private static void setCustomDimension01(string customDimension)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetCustomDimension01(customDimension);
		}

		public static void SetCustomDimension01(string customDimension)
		{
			GA_Wrapper.setCustomDimension01(customDimension);
		}

		private static void setCustomDimension02(string customDimension)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetCustomDimension02(customDimension);
		}

		public static void SetCustomDimension02(string customDimension)
		{
			GA_Wrapper.setCustomDimension02(customDimension);
		}

		private static void setCustomDimension03(string customDimension)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetCustomDimension03(customDimension);
		}

		public static void SetCustomDimension03(string customDimension)
		{
			GA_Wrapper.setCustomDimension03(customDimension);
		}

		public static void SetCustomUserId(string userId)
		{
			GA_Wrapper.configureUserId(userId);
		}

		private static void setEnabledInfoLog(bool enabled)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetEnabledInfoLog(enabled);
		}

		public static void SetEnabledManualSessionHandling(bool enabled)
		{
			GA_Wrapper.setManualSessionHandling(enabled);
		}

		private static void setEnabledVerboseLog(bool enabled)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetEnabledVerboseLog(enabled);
		}

		private static void setFacebookId(string facebookId)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetFacebookId(facebookId);
		}

		public static void SetFacebookId(string facebookId)
		{
			GA_Wrapper.setFacebookId(facebookId);
		}

		private static void setGender(string gender)
		{
			if (gender == "male")
			{
				GameAnalyticsSDK.Net.GameAnalytics.SetGender(EGAGender.Male);
				return;
			}
			if (gender != "female")
			{
				return;
			}
			GameAnalyticsSDK.Net.GameAnalytics.SetGender(EGAGender.Female);
		}

		public static void SetGender(string gender)
		{
			GA_Wrapper.setGender(gender);
		}

		public static void SetInfoLog(bool enabled)
		{
			GA_Wrapper.setEnabledInfoLog(enabled);
		}

		private static void setManualSessionHandling(bool enabled)
		{
			GameAnalyticsSDK.Net.GameAnalytics.SetEnabledManualSessionHandling(enabled);
		}

		public static void SetUnityEngineVersion(string unityEngineVersion)
		{
			GA_Wrapper.configureGameEngineVersion(unityEngineVersion);
		}

		public static void SetUnitySdkVersion(string unitySdkVersion)
		{
			GA_Wrapper.configureSdkGameEngineVersion(unitySdkVersion);
		}

		public static void SetVerboseLog(bool enabled)
		{
			GA_Wrapper.setEnabledVerboseLog(enabled);
		}

		public static void StartSession()
		{
			if (GAState.IsManualSessionHandlingEnabled())
			{
				GA_Wrapper.gameAnalyticsStartSession();
				return;
			}
			Debug.Log("Manual session handling is not enabled. \nPlease check the \"Use manual session handling\" option in the \"Advanced\" section of the Settings object.");
		}

		private class UnityCommandCenterListener : ICommandCenterListener
		{
			public UnityCommandCenterListener()
			{
			}

			public void OnCommandCenterUpdated()
			{
				GameAnalyticsSDK.GameAnalytics.CommandCenterUpdated();
			}
		}
	}
}