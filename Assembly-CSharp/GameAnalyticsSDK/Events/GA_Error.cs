using GameAnalyticsSDK;
using GameAnalyticsSDK.Wrapper;
using System;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
	public static class GA_Error
	{
		private static void CreateNewEvent(GAErrorSeverity severity, string message, IDictionary<string, object> fields)
		{
			GA_Wrapper.AddErrorEvent(severity, message, fields);
		}

		public static void NewEvent(GAErrorSeverity severity, string message, IDictionary<string, object> fields)
		{
			GA_Error.CreateNewEvent(severity, message, fields);
		}
	}
}