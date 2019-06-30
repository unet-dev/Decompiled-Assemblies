using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TabToggle : MonoBehaviour
{
	public Transform TabHolder;

	public Transform ContentHolder;

	public bool FadeIn;

	public bool FadeOut;

	public TabToggle()
	{
	}

	public void Awake()
	{
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button component = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (component)
				{
					component.onClick.AddListener(() => this.SwitchTo(component));
				}
			}
		}
	}

	private void Hide(GameObject go)
	{
		if (!go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (!this.FadeOut || !component)
		{
			go.SetActive(false);
			return;
		}
		LeanTween.alphaCanvas(component, 0f, 0.1f).setOnComplete(() => go.SetActive(false));
	}

	private void Show(GameObject go)
	{
		if (go.activeSelf)
		{
			return;
		}
		CanvasGroup component = go.GetComponent<CanvasGroup>();
		if (this.FadeIn && component)
		{
			component.alpha = 0f;
			LeanTween.alphaCanvas(component, 1f, 0.1f);
		}
		go.SetActive(true);
	}

	public void SwitchTo(Button sourceTab)
	{
		string str = sourceTab.transform.name;
		if (this.TabHolder)
		{
			for (int i = 0; i < this.TabHolder.childCount; i++)
			{
				Button component = this.TabHolder.GetChild(i).GetComponent<Button>();
				if (component)
				{
					component.interactable = component.name != str;
				}
			}
		}
		if (this.ContentHolder)
		{
			for (int j = 0; j < this.ContentHolder.childCount; j++)
			{
				Transform child = this.ContentHolder.GetChild(j);
				if (child.name != str)
				{
					this.Hide(child.gameObject);
				}
				else
				{
					this.Show(child.gameObject);
				}
			}
		}
	}
}