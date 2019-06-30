using Facepunch.Extend;
using Facepunch.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Facepunch
{
	public class ExceptionReporter : MonoBehaviour
	{
		private readonly static Stopwatch LastReportTime;

		private static int _reportsSentCounter;

		public static bool Disabled
		{
			get;
			set;
		}

		private static Dictionary<string, string> Headers
		{
			get
			{
				string str = Epoch.Current.ToString();
				Dictionary<string, string> strs = new Dictionary<string, string>()
				{
					{ "X-Sentry-Auth", string.Concat(new string[] { "Sentry sentry_version=5, sentry_client=FacepunchER/1.0, sentry_timestamp=", str, ", sentry_key=", ExceptionReporter.PublicKey, ", sentry_secret=", ExceptionReporter.SecretKey }) }
				};
				return strs;
			}
		}

		public static string Host
		{
			get;
			set;
		}

		public static string ProjectId
		{
			get;
			set;
		}

		public static string PublicKey
		{
			get;
			set;
		}

		public static string SecretKey
		{
			get;
			set;
		}

		static ExceptionReporter()
		{
			ExceptionReporter.LastReportTime = Stopwatch.StartNew();
		}

		public ExceptionReporter()
		{
		}

		public static void InitializeFromUrl(string url)
		{
			string[] strArrays = url.Replace("https://", "").Split(new char[] { '/', ':', '@' });
			ExceptionReporter.PublicKey = strArrays[0];
			ExceptionReporter.SecretKey = strArrays[1];
			ExceptionReporter.Host = strArrays[2];
			ExceptionReporter.ProjectId = strArrays[3];
		}

		internal static void InstallHooks()
		{
			UnityEngine.Application.logMessageReceived += new UnityEngine.Application.LogCallback(ExceptionReporter.OnLogMessage);
		}

		private static void OnLogMessage(string message, string stackTrace, LogType type)
		{
			if (ExceptionReporter.Disabled)
			{
				return;
			}
			if (!Facepunch.Application.Integration.ShouldReportException(message, stackTrace, type))
			{
				return;
			}
			if (ExceptionReporter.LastReportTime.Elapsed.TotalSeconds > 60)
			{
				ExceptionReporter.LastReportTime.Reset();
				ExceptionReporter.LastReportTime.Start();
				ExceptionReporter._reportsSentCounter = 0;
			}
			if (ExceptionReporter._reportsSentCounter >= 5)
			{
				return;
			}
			ExceptionReporter.LastReportTime.Reset();
			ExceptionReporter.LastReportTime.Start();
			ExceptionReporter._reportsSentCounter++;
			if (type == LogType.Exception && message.Contains("NullReferenceException"))
			{
				string[] strArrays = stackTrace.Split(new char[] { '\n' });
				message = string.Concat("NullReferenceException: ", strArrays[0]);
			}
			ExceptionReporter.SendReport(message, stackTrace);
		}

		public static void SendReport(string exception, string stacktrace)
		{
			if (string.IsNullOrEmpty(ExceptionReporter.Host))
			{
				return;
			}
			if (string.IsNullOrEmpty(ExceptionReporter.ProjectId))
			{
				return;
			}
			if (string.IsNullOrEmpty(ExceptionReporter.PublicKey))
			{
				return;
			}
			if (string.IsNullOrEmpty(ExceptionReporter.SecretKey))
			{
				return;
			}
			Report report = new Report()
			{
				release = BuildInfo.Current.Scm.ChangeId,
				message = exception.Truncate(250, ""),
				platform = "csharp",
				event_id = Guid.NewGuid().ToString("N"),
				stacktrace = new Report.StackTrace(stacktrace),
				tags = new Dictionary<string, string>()
				{
					{ "memory", string.Concat(Mathf.RoundToInt((float)(SystemInfo.systemMemorySize / 1024)), "gb") },
					{ "operatingSystem", SystemInfo.operatingSystem }
				}
			};
			report.tags.Add("batteryStatus", SystemInfo.batteryStatus.ToString());
			report.tags.Add("deviceModel", SystemInfo.deviceModel);
			report.tags.Add("processorType", SystemInfo.processorType);
			report.tags.Add("graphicsDeviceName", SystemInfo.graphicsDeviceName);
			report.tags.Add("graphicsMemorySize", string.Concat(Mathf.RoundToInt((float)(SystemInfo.graphicsMemorySize / 1024)), "gb"));
			report.tags.Add("architecture", (IntPtr.Size == 4 ? "x86" : "x64"));
			report.tags.Add("scene", SceneManager.GetSceneAt(0).name);
			report.tags.Add("qualitylevel", QualitySettings.GetQualityLevel().ToString());
			string str = JsonConvert.SerializeObject(report);
			WWW wWW = new WWW(string.Concat(new string[] { "https://", ExceptionReporter.Host, "/api/", ExceptionReporter.ProjectId, "/store/" }), Encoding.ASCII.GetBytes(str), ExceptionReporter.Headers);
		}
	}
}