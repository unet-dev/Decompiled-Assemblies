using Rust;
using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;

public class ErrorText : MonoBehaviour
{
	public TextMeshProUGUI text;

	public int maxLength = 1024;

	private Stopwatch stopwatch;

	public ErrorText()
	{
	}

	internal void CaptureLog(string error, string stacktrace, LogType type)
	{
		if (type != LogType.Error && type != LogType.Exception && type != LogType.Assert)
		{
			return;
		}
		if (this.text == null)
		{
			return;
		}
		TextMeshProUGUI textMeshProUGUI = this.text;
		textMeshProUGUI.text = string.Concat(new string[] { textMeshProUGUI.text, error, "\n", stacktrace, "\n\n" });
		if (this.text.text.Length > this.maxLength)
		{
			this.text.text = this.text.text.Substring(this.text.text.Length - this.maxLength, this.maxLength);
		}
		this.stopwatch = Stopwatch.StartNew();
	}

	public void OnDisable()
	{
		if (Rust.Application.isQuitting)
		{
			return;
		}
		Output.OnMessage -= new Action<string, string, LogType>(this.CaptureLog);
	}

	public void OnEnable()
	{
		Output.OnMessage += new Action<string, string, LogType>(this.CaptureLog);
	}

	protected void Update()
	{
		if (this.stopwatch != null && this.stopwatch.Elapsed.TotalSeconds > 30)
		{
			this.text.text = string.Empty;
			this.stopwatch = null;
		}
	}
}