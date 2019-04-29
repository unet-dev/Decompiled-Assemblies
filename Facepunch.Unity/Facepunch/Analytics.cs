using Facepunch.Models.Analytics;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Facepunch
{
	public class Analytics
	{
		private static Stat Stats;

		public static string LastMessage
		{
			get;
			internal set;
		}

		public static string LastResponse
		{
			get;
			internal set;
		}

		public static DateTime LastSend
		{
			get;
			internal set;
		}

		internal string ReportUrl
		{
			get;
			private set;
		}

		internal string SessionId
		{
			get;
			private set;
		}

		internal string Uid
		{
			get;
			set;
		}

		static Analytics()
		{
			Analytics.Stats = new Stat();
			Analytics.LastMessage = string.Empty;
			Analytics.LastResponse = string.Empty;
		}

		internal Analytics(string reportUrl)
		{
			this.Uid = Facepunch.Application.Integration.UserId;
			this.ReportUrl = reportUrl;
			this.SessionId = Guid.NewGuid().ToString();
			this.Send("start", new SessionStart()
			{
				Uid = this.Uid,
				Sid = this.SessionId,
				ChangeSet = BuildInfo.Current.Scm.ChangeId,
				Branch = BuildInfo.Current.Scm.Branch,
				Os = SystemInfo.operatingSystem,
				Bucket = Facepunch.Application.Integration.Bucket,
				Gpu = SystemInfo.graphicsDeviceName,
				Cpu = SystemInfo.processorType,
				CpuFrq = SystemInfo.processorFrequency,
				CpuCnt = SystemInfo.processorCount,
				Mem = SystemInfo.systemMemorySize,
				GpuMem = SystemInfo.graphicsMemorySize,
				Arch = (IntPtr.Size == 4 ? "x86" : "x64"),
				Width = Screen.width,
				Height = Screen.height,
				Fullscreen = Screen.fullScreen,
				RR = Screen.currentResolution.refreshRate
			}, false);
			Facepunch.Application.Controller.StartCoroutine(this.KeepAlive());
		}

		public static void ForceSendSessionUpdateDebug()
		{
			if (Facepunch.Application.Analytics == null)
			{
				return;
			}
			Facepunch.Application.Analytics.SendSessionUpdate(false);
		}

		public IEnumerator KeepAlive()
		{
			Analytics analytic = null;
			float single = 4f;
			while (true)
			{
				yield return new WaitForSeconds(60f * single);
				analytic.SendSessionUpdate(false);
				if (single < 32f)
				{
					single *= 2f;
				}
			}
		}

		internal void OnQuit()
		{
			this.SendSessionUpdate(false);
			this.Send("close", new SessionClose()
			{
				Uid = this.Uid,
				Sid = this.SessionId,
				FinalUpdate = new SessionUpdate()
				{
					Frames = Performance.CategorizedFrameCount,
					Mem = Performance.MemoryUsage,
					Gc = Performance.GarbageCollections
				}
			}, true);
		}

		public static void RecordAdd(string category, string name, double val)
		{
			Analytics.Stats.GetCategory(category).AddSum(name, val);
		}

		public static void RecordAverage(string category, string name, double val)
		{
			Analytics.Stats.GetCategory(category).AddAverage(name, val);
		}

		public static void RecordReplace(string category, string name, double val)
		{
			Analytics.Stats.GetCategory(category).AddReplace(name, val);
		}

		private void Send(string action, object obj, bool andWait = false)
		{
			string str = JsonConvert.SerializeObject(obj, Formatting.Indented);
			Dictionary<string, string> strs = new Dictionary<string, string>()
			{
				{ "data", str }
			};
			Analytics.LastMessage = str;
			Analytics.LastResponse = "";
			string str1 = this.ReportUrl.Replace("{action}", action);
			if (Facepunch.Application.Integration.DebugOutput)
			{
				UnityEngine.Debug.Log(string.Concat("[Analytics] Sending ", str1));
			}
			try
			{
				WebUtil.Post(str1, strs, andWait, null);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				UnityEngine.Debug.LogWarning(string.Concat("Analytic Exception: ", exception.Message));
				UnityEngine.Debug.LogWarning(exception.StackTrace);
			}
			Analytics.LastSend = DateTime.UtcNow;
		}

		private void SendSessionUpdate(bool andWait = false)
		{
			this.Send("update", new SessionUpdate()
			{
				Uid = this.Uid,
				Sid = this.SessionId,
				Frames = Performance.CategorizedFrameCount,
				Mem = Performance.MemoryUsage,
				Gc = Performance.GarbageCollections,
				StatText = (Analytics.Stats == null ? "" : JsonConvert.SerializeObject(Analytics.Stats))
			}, andWait);
		}
	}
}