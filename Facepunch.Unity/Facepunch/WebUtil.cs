using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public static class WebUtil
	{
		private static IEnumerator DownloadStringCoroutine(WWW www, Action<string> result)
		{
			yield return www;
			if (www.error == null && result != null)
			{
				result(www.text);
			}
			www.Dispose();
		}

		internal static string Escape(string type)
		{
			return WWW.EscapeURL(type);
		}

		internal static void Get(string url, Action<string> result)
		{
			if (url.Contains("https://localhost"))
			{
				url = url.Replace("https://localhost", "http://localhost");
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				UnityEngine.Debug.Log(string.Concat("[Get] \"", url, "\""));
			}
			WWW wWW = new WWW(url);
			Facepunch.Application.Controller.StartCoroutine(WebUtil.DownloadStringCoroutine(wWW, result));
		}

		internal static void Post(string url, Dictionary<string, string> data, bool wait, Action<string> result)
		{
			if (url.Contains("https://localhost"))
			{
				url = url.Replace("https://localhost", "http://localhost");
			}
			WWWForm wWWForm = new WWWForm();
			foreach (KeyValuePair<string, string> datum in data)
			{
				wWWForm.AddField(datum.Key, datum.Value);
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				UnityEngine.Debug.Log(string.Concat("[Post] \"", url, "\""));
			}
			WWW wWW = new WWW(url, wWWForm);
			if (!wait)
			{
				Facepunch.Application.Controller.StartCoroutine(WebUtil.PostValuesCoroutine(wWW, result));
				return;
			}
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (!wWW.isDone && stopwatch.Elapsed.Seconds <= 5)
			{
			}
			wWW.Dispose();
		}

		private static IEnumerator PostValuesCoroutine(WWW www, Action<string> result)
		{
			yield return www;
			if (www.error == null && result != null)
			{
				result(www.text);
			}
			if (Facepunch.Application.Integration.DebugOutput)
			{
				if (www.error == null)
				{
					UnityEngine.Debug.Log(string.Concat("[Post] Response: \"", www.text, "\""));
				}
				else
				{
					string[] strArrays = new string[] { "[Post] Error: \"", www.error, "\" - \"", www.text, "\" " };
					UnityEngine.Debug.LogWarning(string.Concat(strArrays));
				}
			}
			www.Dispose();
		}
	}
}