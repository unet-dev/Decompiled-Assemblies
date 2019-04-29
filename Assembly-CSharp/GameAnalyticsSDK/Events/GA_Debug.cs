using GameAnalyticsSDK;
using GameAnalyticsSDK.Setup;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace GameAnalyticsSDK.Events
{
	public static class GA_Debug
	{
		public static int MaxErrorCount;

		private static int _errorCount;

		private static bool _showLogOnGUI;

		public static List<string> Messages;

		static GA_Debug()
		{
			GA_Debug.MaxErrorCount = 10;
			GA_Debug._errorCount = 0;
			GA_Debug._showLogOnGUI = false;
		}

		public static void EnabledLog()
		{
			GA_Debug._showLogOnGUI = true;
		}

		public static void HandleLog(string logString, string stackTrace, LogType type)
		{
			if (logString.StartsWith("[Physics.PhysX] RigidBody::setRigidBodyFlag"))
			{
				return;
			}
			if (GA_Debug._showLogOnGUI)
			{
				if (GA_Debug.Messages == null)
				{
					GA_Debug.Messages = new List<string>();
				}
				GA_Debug.Messages.Add(logString);
			}
			if (GameAnalytics.SettingsGA.SubmitErrors && GA_Debug._errorCount < GA_Debug.MaxErrorCount && type != LogType.Log)
			{
				if (string.IsNullOrEmpty(stackTrace))
				{
					stackTrace = (new StackTrace()).ToString();
				}
				GA_Debug._errorCount++;
				string str = logString.Replace('\"', '\'').Replace('\n', ' ').Replace('\r', ' ');
				string str1 = stackTrace.Replace('\"', '\'').Replace('\n', ' ').Replace('\r', ' ');
				string str2 = string.Concat(str, " ", str1);
				if (str2.Length > 8192)
				{
					str2 = str2.Substring(8192);
				}
				GA_Debug.SubmitError(str2, type);
			}
		}

		private static void SubmitError(string message, LogType type)
		{
			GAErrorSeverity gAErrorSeverity = GAErrorSeverity.Info;
			switch (type)
			{
				case LogType.Error:
				{
					gAErrorSeverity = GAErrorSeverity.Error;
					break;
				}
				case LogType.Assert:
				{
					gAErrorSeverity = GAErrorSeverity.Info;
					break;
				}
				case LogType.Warning:
				{
					gAErrorSeverity = GAErrorSeverity.Warning;
					break;
				}
				case LogType.Log:
				{
					gAErrorSeverity = GAErrorSeverity.Debug;
					break;
				}
				case LogType.Exception:
				{
					gAErrorSeverity = GAErrorSeverity.Critical;
					break;
				}
			}
			GA_Error.NewEvent(gAErrorSeverity, message, null);
		}
	}
}