using System;
using UnityEngine;

public class HolosightReticlePositioning : MonoBehaviour
{
	public IronsightAimPoint aimPoint;

	public RectTransform rectTransform
	{
		get
		{
			return base.transform as RectTransform;
		}
	}

	public HolosightReticlePositioning()
	{
	}

	private void Update()
	{
		if (MainCamera.isValid)
		{
			this.UpdatePosition(MainCamera.mainCamera);
		}
	}

	private void UpdatePosition(Camera cam)
	{
		Vector3 vector3 = this.aimPoint.targetPoint.transform.position;
		Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, vector3);
		RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform.parent as RectTransform, screenPoint, cam, out screenPoint);
		ref float singlePointer = ref screenPoint.x;
		Rect rect = (this.rectTransform.parent as RectTransform).rect;
		singlePointer = singlePointer / (rect.width * 0.5f);
		ref float singlePointer1 = ref screenPoint.y;
		rect = (this.rectTransform.parent as RectTransform).rect;
		singlePointer1 = singlePointer1 / (rect.height * 0.5f);
		this.rectTransform.anchoredPosition = screenPoint;
	}
}