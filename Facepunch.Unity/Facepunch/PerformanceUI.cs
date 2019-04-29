using System;
using UnityEngine;
using UnityEngine.UI;

namespace Facepunch
{
	public class PerformanceUI : MonoBehaviour
	{
		public LayoutElement[] rainbow;

		public Text info;

		public Text fps;

		public Text ms;

		public Text mb;

		public Text gc;

		private UnityEngine.Canvas Canvas;

		private float updateTime;

		public PerformanceUI()
		{
		}

		private void Awake()
		{
			this.Canvas = base.GetComponent<UnityEngine.Canvas>();
			this.Canvas.enabled = false;
			if (!BuildInfo.Current.Valid)
			{
				this.info.transform.parent.gameObject.SetActive(false);
				return;
			}
			string changeId = BuildInfo.Current.Scm.ChangeId;
			if (changeId.Length > 8)
			{
				changeId = changeId.Substring(0, 8);
			}
			string str = string.Concat("<color=#bbbbbb>Build Date</color> <b>", BuildInfo.Current.BuildDate, "</b>");
			str = string.Concat(new string[] { str, "\n<color=#bbbbbb>Change</color> <b>", changeId, "</b> <color=#bbbbbb>by</color> <b>", BuildInfo.Current.Scm.Author, "</b>" });
			str = string.Concat(new string[] { str, "\n<color=#bbbbbb>Build</color> <b>", BuildInfo.Current.Build.Number, "</b> <color=#bbbbbb>on</color> <b>", BuildInfo.Current.Build.Node, "</b>" });
			this.info.text = str;
		}

		public static void SpawnPrefab()
		{
			UnityEngine.Object.DontDestroyOnLoad(UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Facepunch.Performance")));
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.F8))
			{
				this.Canvas.enabled = !this.Canvas.enabled;
			}
			if (!this.Canvas.enabled)
			{
				return;
			}
			if (this.updateTime - Time.realtimeSinceStartup > 0f)
			{
				return;
			}
			this.updateTime = Time.realtimeSinceStartup + 0.5f;
			if (Performance.FrameCountLastSecond < 30)
			{
				this.fps.color = Color.red;
			}
			else if (Performance.FrameCountLastSecond >= 50)
			{
				this.fps.color = Color.white;
			}
			else
			{
				this.fps.color = Color.yellow;
			}
			Text str = this.fps;
			int frameCountLastSecond = Performance.FrameCountLastSecond;
			str.text = frameCountLastSecond.ToString("0");
			this.ms.text = Performance.AvgFrameTimeLastSecond.ToString("0.00");
			Text text = this.mb;
			frameCountLastSecond = Performance.MemoryUsage;
			text.text = frameCountLastSecond.ToString("N0");
			Text str1 = this.gc;
			frameCountLastSecond = Performance.GarbageCollections;
			str1.text = frameCountLastSecond.ToString("N0");
			this.UpdateRainbow();
		}

		private void UpdateRainbow()
		{
			for (int i = 0; i < 6; i++)
			{
				this.rainbow[i].flexibleWidth = Performance.GetFrameFraction((FrameRateCategory)i) * 1000f;
			}
		}
	}
}