using Facepunch.Models;
using Newtonsoft.Json;
using System;
using System.Threading;
using UnityEngine;

namespace Facepunch
{
	public class BenchmarkData
	{
		public BenchmarkData.Result[] Results;

		public string UserID;

		public string BuildDate;

		public string BranchName;

		public string Changeset;

		public string BuildId;

		public Facepunch.Models.AppInfo AppInfo;

		public BenchmarkData()
		{
		}

		public static BenchmarkData New()
		{
			return new BenchmarkData()
			{
				BuildDate = BuildInfo.Current.BuildDate.ToString(),
				BranchName = BuildInfo.Current.Scm.Branch,
				Changeset = BuildInfo.Current.Scm.ChangeId,
				BuildId = BuildInfo.Current.Build.Id
			};
		}

		public string Upload()
		{
			if (string.IsNullOrEmpty(Facepunch.Application.Manifest.BenchmarkUrl))
			{
				return null;
			}
			string str = JsonConvert.SerializeObject(this, Formatting.Indented);
			WWWForm wWWForm = new WWWForm();
			wWWForm.AddField("data", str);
			WWW wWW = new WWW(Facepunch.Application.Manifest.BenchmarkUrl, wWWForm);
			while (!wWW.isDone)
			{
				Thread.Sleep(100);
			}
			if (wWW.text == null)
			{
				return null;
			}
			return wWW.text.Trim();
		}

		public class Result
		{
			public string Name;

			public int FrameCount;

			public float Min;

			public float Max;

			public float Avg;

			public float Seconds;

			public Result()
			{
			}
		}
	}
}