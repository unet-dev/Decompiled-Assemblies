using System;
using UnityEngine;

public class UISleepingScreen : SingletonComponent<UISleepingScreen>, IUIScreen
{
	protected CanvasGroup canvasGroup;

	public UISleepingScreen()
	{
	}

	protected override void Awake()
	{
		base.Awake();
		this.canvasGroup = base.GetComponent<CanvasGroup>();
	}

	public void SetVisible(bool b)
	{
		this.canvasGroup.alpha = (b ? 1f : 0f);
	}
}