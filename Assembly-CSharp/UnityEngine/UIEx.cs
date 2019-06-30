using System;
using System.Runtime.CompilerServices;
using UnityEngine.UI;

namespace UnityEngine
{
	public static class UIEx
	{
		public static void CenterOnPosition(this ScrollRect scrollrect, Vector2 pos)
		{
			RectTransform rectTransform = scrollrect.transform as RectTransform;
			Vector2 vector2 = new Vector2(scrollrect.content.localScale.x, scrollrect.content.localScale.y);
			pos.x *= vector2.x;
			pos.y *= vector2.y;
			Rect rect = scrollrect.content.rect;
			float single = rect.width * vector2.x;
			rect = rectTransform.rect;
			float single1 = single - rect.width;
			rect = scrollrect.content.rect;
			float single2 = rect.height * vector2.y;
			rect = rectTransform.rect;
			Vector2 vector21 = new Vector2(single1, single2 - rect.height);
			pos.x = pos.x / vector21.x + scrollrect.content.pivot.x;
			pos.y = pos.y / vector21.y + scrollrect.content.pivot.y;
			if (scrollrect.movementType != ScrollRect.MovementType.Unrestricted)
			{
				pos.x = Mathf.Clamp(pos.x, 0f, 1f);
				pos.y = Mathf.Clamp(pos.y, 0f, 1f);
			}
			scrollrect.normalizedPosition = pos;
		}

		public static void RebuildHackUnity2019(this Image image)
		{
			Sprite sprite = image.sprite;
			image.sprite = null;
			image.sprite = sprite;
		}

		public static Vector2 Unpivot(this RectTransform rect, Vector2 localPos)
		{
			ref float singlePointer = ref localPos.x;
			float single = rect.pivot.x;
			Rect rect1 = rect.rect;
			singlePointer = singlePointer + single * rect1.width;
			ref float singlePointer1 = ref localPos.y;
			float single1 = rect.pivot.y;
			rect1 = rect.rect;
			singlePointer1 = singlePointer1 + single1 * rect1.height;
			return localPos;
		}
	}
}