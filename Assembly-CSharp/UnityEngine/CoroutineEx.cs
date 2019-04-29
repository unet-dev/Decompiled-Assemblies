using System;
using System.Collections.Generic;

namespace UnityEngine
{
	public static class CoroutineEx
	{
		public static WaitForEndOfFrame waitForEndOfFrame;

		public static WaitForFixedUpdate waitForFixedUpdate;

		private static Dictionary<float, WaitForSeconds> waitForSecondsBuffer;

		static CoroutineEx()
		{
			CoroutineEx.waitForEndOfFrame = new WaitForEndOfFrame();
			CoroutineEx.waitForFixedUpdate = new WaitForFixedUpdate();
			CoroutineEx.waitForSecondsBuffer = new Dictionary<float, WaitForSeconds>();
		}

		public static WaitForSeconds waitForSeconds(float seconds)
		{
			WaitForSeconds waitForSecond;
			if (!CoroutineEx.waitForSecondsBuffer.TryGetValue(seconds, out waitForSecond))
			{
				waitForSecond = new WaitForSeconds(seconds);
				CoroutineEx.waitForSecondsBuffer.Add(seconds, waitForSecond);
			}
			return waitForSecond;
		}

		public static WaitForSecondsRealtime waitForSecondsRealtime(float seconds)
		{
			return new WaitForSecondsRealtime(seconds);
		}
	}
}