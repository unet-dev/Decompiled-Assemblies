using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : SingletonComponent<UIHUD>, IUIScreen
{
	public UIChat chatPanel;

	public HudElement Hunger;

	public HudElement Thirst;

	public HudElement Health;

	public HudElement PendingHealth;

	public HudElement VehicleHealth;

	public HudElement AnimalStamina;

	public HudElement AnimalStaminaMax;

	public RawImage compassStrip;

	public CanvasGroup compassGroup;

	public RectTransform vitalsRect;

	public UIHUD()
	{
	}
}