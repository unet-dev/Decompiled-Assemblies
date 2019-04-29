using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIRoot : MonoBehaviour
{
	private GraphicRaycaster[] graphicRaycasters;

	public Canvas overlayCanvas;

	protected UIRoot()
	{
	}

	protected virtual void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	protected abstract void Refresh();

	protected virtual void Start()
	{
		this.graphicRaycasters = base.GetComponentsInChildren<GraphicRaycaster>(true);
	}

	private void ToggleRaycasters(bool state)
	{
		for (int i = 0; i < (int)this.graphicRaycasters.Length; i++)
		{
			GraphicRaycaster graphicRaycaster = this.graphicRaycasters[i];
			if (graphicRaycaster.enabled != state)
			{
				graphicRaycaster.enabled = state;
			}
		}
	}

	protected void Update()
	{
		this.Refresh();
	}
}