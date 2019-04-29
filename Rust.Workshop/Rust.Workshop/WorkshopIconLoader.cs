using System;
using UnityEngine;

namespace Rust.Workshop
{
	public class WorkshopIconLoader : MonoBehaviour
	{
		public WorkshopIconLoader()
		{
		}

		public static Sprite Find(ulong workshopId, Sprite placeholder = null, Action callback = null)
		{
			Skin skin = WorkshopSkin.GetSkin(workshopId);
			if (!skin.IconLoaded)
			{
				if (callback != null)
				{
					skin.OnIconLoaded = callback;
				}
				if (placeholder != null)
				{
					return placeholder;
				}
			}
			return skin.sprite;
		}
	}
}