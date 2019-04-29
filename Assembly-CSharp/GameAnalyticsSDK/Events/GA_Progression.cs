using GameAnalyticsSDK;
using GameAnalyticsSDK.Wrapper;
using System;
using System.Collections.Generic;

namespace GameAnalyticsSDK.Events
{
	public static class GA_Progression
	{
		private static void CreateEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int? score, IDictionary<string, object> fields)
		{
			if (!score.HasValue)
			{
				GA_Wrapper.AddProgressionEvent(progressionStatus, progression01, progression02, progression03, fields);
				return;
			}
			GA_Wrapper.AddProgressionEventWithScore(progressionStatus, progression01, progression02, progression03, score.Value, fields);
		}

		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, null, null, null, fields);
		}

		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, null, null, fields);
		}

		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, progression03, null, fields);
		}

		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, int score, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, null, null, new int?(score), fields);
		}

		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, int score, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, null, new int?(score), fields);
		}

		public static void NewEvent(GAProgressionStatus progressionStatus, string progression01, string progression02, string progression03, int score, IDictionary<string, object> fields)
		{
			GA_Progression.CreateEvent(progressionStatus, progression01, progression02, progression03, new int?(score), fields);
		}
	}
}