using GameAnalyticsSDK.Setup;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAnalyticsSDK.State
{
	internal static class GAState
	{
		private static Settings _settings;

		public static bool HasAvailableCustomDimensions01(string _dimension01)
		{
			if (GAState.ListContainsString(GAState._settings.CustomDimensions01, _dimension01))
			{
				return true;
			}
			return false;
		}

		public static bool HasAvailableCustomDimensions02(string _dimension02)
		{
			if (GAState.ListContainsString(GAState._settings.CustomDimensions02, _dimension02))
			{
				return true;
			}
			return false;
		}

		public static bool HasAvailableCustomDimensions03(string _dimension03)
		{
			if (GAState.ListContainsString(GAState._settings.CustomDimensions03, _dimension03))
			{
				return true;
			}
			return false;
		}

		public static bool HasAvailableResourceCurrency(string _currency)
		{
			if (GAState.ListContainsString(GAState._settings.ResourceCurrencies, _currency))
			{
				return true;
			}
			return false;
		}

		public static bool HasAvailableResourceItemType(string _itemType)
		{
			if (GAState.ListContainsString(GAState._settings.ResourceItemTypes, _itemType))
			{
				return true;
			}
			return false;
		}

		public static void Init()
		{
			try
			{
				GAState._settings = (Settings)Resources.Load("GameAnalytics/Settings", typeof(Settings));
			}
			catch (Exception exception)
			{
				Debug.Log(string.Concat("Could not get Settings during event validation \n", exception.ToString()));
			}
		}

		public static bool IsManualSessionHandlingEnabled()
		{
			return GAState._settings.UseManualSessionHandling;
		}

		private static bool ListContainsString(List<string> _list, string _string)
		{
			if (_list.Contains(_string))
			{
				return true;
			}
			return false;
		}
	}
}