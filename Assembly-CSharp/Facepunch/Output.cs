using Facepunch.Math;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

namespace Facepunch
{
	public static class Output
	{
		public static bool installed;

		public static List<Output.Entry> HistoryOutput;

		static Output()
		{
			Output.installed = false;
			Output.HistoryOutput = new List<Output.Entry>();
		}

		public static void Install()
		{
			if (Output.installed)
			{
				return;
			}
			UnityEngine.Application.logMessageReceived += new UnityEngine.Application.LogCallback(Output.LogHandler);
			Output.installed = true;
		}

		internal static void LogHandler(string log, string stacktrace, LogType type)
		{
			if (Output.OnMessage == null)
			{
				return;
			}
			if (log.StartsWith("Kinematic body only supports Speculative Continuous collision detection"))
			{
				return;
			}
			if (log.StartsWith("Skipped frame because GfxDevice"))
			{
				return;
			}
			if (log.StartsWith("Your current multi-scene setup has inconsistent Lighting"))
			{
				return;
			}
			if (log.Contains("HandleD3DDeviceLost"))
			{
				return;
			}
			if (log.Contains("ResetD3DDevice"))
			{
				return;
			}
			if (log.Contains("dev->Reset"))
			{
				return;
			}
			if (log.Contains("D3Dwindow device not lost anymore"))
			{
				return;
			}
			if (log.Contains("D3D device reset"))
			{
				return;
			}
			if (log.Contains("group < 0xfff"))
			{
				return;
			}
			if (log.Contains("Mesh can not have more than 65000 vert"))
			{
				return;
			}
			if (log.Contains("Trying to add (Layout Rebuilder for)"))
			{
				return;
			}
			if (log.Contains("Coroutine continue failure"))
			{
				return;
			}
			if (log.Contains("No texture data available to upload"))
			{
				return;
			}
			if (log.Contains("Trying to reload asset from disk that is not"))
			{
				return;
			}
			if (log.Contains("Unable to find shaders used for the terrain engine."))
			{
				return;
			}
			if (log.Contains("Canvas element contains more than 65535 vertices"))
			{
				return;
			}
			if (log.Contains("RectTransform.set_anchorMin"))
			{
				return;
			}
			if (log.Contains("FMOD failed to initialize the output device"))
			{
				return;
			}
			if (log.Contains("Cannot create FMOD::Sound"))
			{
				return;
			}
			if (log.Contains("invalid utf-16 sequence"))
			{
				return;
			}
			if (log.Contains("missing surrogate tail"))
			{
				return;
			}
			if (log.Contains("Failed to create agent because it is not close enough to the Nav"))
			{
				return;
			}
			if (log.Contains("user-provided triangle mesh descriptor is invalid"))
			{
				return;
			}
			if (log.Contains("Releasing render texture that is set as"))
			{
				return;
			}
			Output.OnMessage(log, stacktrace, type);
			List<Output.Entry> historyOutput = Output.HistoryOutput;
			Output.Entry entry = new Output.Entry()
			{
				Message = log,
				Stacktrace = stacktrace,
				Type = type.ToString(),
				Time = Epoch.Current
			};
			historyOutput.Add(entry);
			while (Output.HistoryOutput.Count > 65536)
			{
				Output.HistoryOutput.RemoveAt(0);
			}
		}

		public static event Action<string, string, LogType> OnMessage;

		public struct Entry
		{
			public string Message;

			public string Stacktrace;

			public string Type;

			public int Time;
		}
	}
}