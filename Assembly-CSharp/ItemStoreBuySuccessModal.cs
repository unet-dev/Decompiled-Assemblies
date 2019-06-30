using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ItemStoreBuySuccessModal : MonoBehaviour
{
	public ItemStoreBuySuccessModal()
	{
	}

	public void Hide()
	{
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 0f, 0.2f).setOnComplete(() => base.gameObject.SetActive(false));
	}

	public void Show(ulong orderId)
	{
		base.gameObject.SetActive(true);
		base.GetComponent<CanvasGroup>().alpha = 0f;
		LeanTween.alphaCanvas(base.GetComponent<CanvasGroup>(), 1f, 0.1f);
	}
}