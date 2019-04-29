using GameAnalyticsSDK;
using GameAnalyticsSDK.Setup;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameAnalyticsSDK.Events
{
	public class GA_SpecialEvents : MonoBehaviour
	{
		private static int _frameCountAvg;

		private static float _lastUpdateAvg;

		private int _frameCountCrit;

		private float _lastUpdateCrit;

		private static int _criticalFpsCount;

		static GA_SpecialEvents()
		{
		}

		public GA_SpecialEvents()
		{
		}

		public void CheckCriticalFPS()
		{
			if (GameAnalytics.SettingsGA.SubmitFpsCritical)
			{
				float single = Time.time - this._lastUpdateCrit;
				if (single >= 1f)
				{
					float single1 = (float)this._frameCountCrit / single;
					this._lastUpdateCrit = Time.time;
					this._frameCountCrit = 0;
					if (single1 <= (float)GameAnalytics.SettingsGA.FpsCriticalThreshold)
					{
						GA_SpecialEvents._criticalFpsCount++;
					}
				}
			}
		}

		private IEnumerator CheckCriticalFPSRoutine()
		{
			GA_SpecialEvents gASpecialEvent = null;
			while (Application.isPlaying && GameAnalytics.SettingsGA.SubmitFpsCritical)
			{
				yield return new WaitForSeconds((float)GameAnalytics.SettingsGA.FpsCirticalSubmitInterval);
				gASpecialEvent.CheckCriticalFPS();
			}
		}

		public void Start()
		{
			base.StartCoroutine(this.SubmitFPSRoutine());
			base.StartCoroutine(this.CheckCriticalFPSRoutine());
		}

		public static void SubmitFPS()
		{
			if (GameAnalytics.SettingsGA.SubmitFpsAverage)
			{
				float single = Time.time - GA_SpecialEvents._lastUpdateAvg;
				if (single > 1f)
				{
					float single1 = (float)GA_SpecialEvents._frameCountAvg / single;
					GA_SpecialEvents._lastUpdateAvg = Time.time;
					GA_SpecialEvents._frameCountAvg = 0;
					if (single1 > 0f)
					{
						GameAnalytics.NewDesignEvent("GA:AverageFPS", (float)((int)single1));
					}
				}
			}
			if (GameAnalytics.SettingsGA.SubmitFpsCritical && GA_SpecialEvents._criticalFpsCount > 0)
			{
				GameAnalytics.NewDesignEvent("GA:CriticalFPS", (float)GA_SpecialEvents._criticalFpsCount);
				GA_SpecialEvents._criticalFpsCount = 0;
			}
		}

		private IEnumerator SubmitFPSRoutine()
		{
			while (Application.isPlaying && GameAnalytics.SettingsGA.SubmitFpsAverage)
			{
				yield return new WaitForSeconds(30f);
				GA_SpecialEvents.SubmitFPS();
			}
		}

		public void Update()
		{
			if (GameAnalytics.SettingsGA.SubmitFpsAverage)
			{
				GA_SpecialEvents._frameCountAvg++;
			}
			if (GameAnalytics.SettingsGA.SubmitFpsCritical)
			{
				this._frameCountCrit++;
			}
		}
	}
}