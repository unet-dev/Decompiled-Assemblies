using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TextureColorPicker : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDragHandler
{
	public Texture2D texture;

	public TextureColorPicker.onColorSelectedEvent onColorSelected = new TextureColorPicker.onColorSelectedEvent();

	public TextureColorPicker()
	{
	}

	public virtual void OnDrag(PointerEventData eventData)
	{
		RectTransform rectTransform = base.transform as RectTransform;
		Vector2 vector2 = new Vector2();
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out vector2))
		{
			ref float singlePointer = ref vector2.x;
			Rect rect = rectTransform.rect;
			singlePointer = singlePointer + rect.width * 0.5f;
			ref float singlePointer1 = ref vector2.y;
			rect = rectTransform.rect;
			singlePointer1 = singlePointer1 + rect.height * 0.5f;
			ref float singlePointer2 = ref vector2.x;
			rect = rectTransform.rect;
			singlePointer2 /= rect.width;
			ref float singlePointer3 = ref vector2.y;
			rect = rectTransform.rect;
			singlePointer3 /= rect.height;
			Color pixel = this.texture.GetPixel((int)(vector2.x * (float)this.texture.width), (int)(vector2.y * (float)this.texture.height));
			this.onColorSelected.Invoke(pixel);
		}
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		this.OnDrag(eventData);
	}

	[Serializable]
	public class onColorSelectedEvent : UnityEvent<Color>
	{
		public onColorSelectedEvent()
		{
		}
	}
}