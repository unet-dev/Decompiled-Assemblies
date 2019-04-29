using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

public class FPSText : MonoBehaviour
{
	public Text text;

	private Stopwatch fpsTimer = Stopwatch.StartNew();

	public FPSText()
	{
	}

	protected void Update()
	{
		if (this.fpsTimer.Elapsed.TotalSeconds < 0.5)
		{
			return;
		}
		this.text.enabled = true;
		this.fpsTimer.Reset();
		this.fpsTimer.Start();
		string str = string.Concat(Performance.current.frameRate, " FPS");
		this.text.text = str;
	}
}