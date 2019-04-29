using GameAnalyticsSDK.Wrapper;
using System;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
	public static class GA_Design
	{
		private static void CreateNewEvent(string eventName, float? eventValue, IDictionary<string, object> fields)
		{
			if (!eventValue.HasValue)
			{
				GA_Wrapper.AddDesignEvent(eventName, fields);
				return;
			}
			GA_Wrapper.AddDesignEvent(eventName, eventValue.Value, fields);
		}

		public static void NewEvent(string eventName, float eventValue, IDictionary<string, object> fields)
		{
			GA_Design.CreateNewEvent(eventName, new float?(eventValue), fields);
		}

		public static void NewEvent(string eventName, IDictionary<string, object> fields)
		{
			GA_Design.CreateNewEvent(eventName, null, fields);
		}
	}
}