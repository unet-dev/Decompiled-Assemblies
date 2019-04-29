using System;
using UnityEngine;

public class ExplosionsFPS : MonoBehaviour
{
	private readonly GUIStyle guiStyleHeader = new GUIStyle();

	private float timeleft;

	private float fps;

	private int frames;

	public ExplosionsFPS()
	{
	}

	private void Awake()
	{
		this.guiStyleHeader.fontSize = 14;
		this.guiStyleHeader.normal.textColor = new Color(1f, 1f, 1f);
	}

	private void OnGUI()
	{
		GUI.Label(new Rect(0f, 0f, 30f, 30f), string.Concat("FPS: ", (int)this.fps), this.guiStyleHeader);
	}

	private void Update()
	{
		this.timeleft -= Time.deltaTime;
		this.frames++;
		if ((double)this.timeleft <= 0)
		{
			this.fps = (float)this.frames;
			this.timeleft = 1f;
			this.frames = 0;
		}
	}
}