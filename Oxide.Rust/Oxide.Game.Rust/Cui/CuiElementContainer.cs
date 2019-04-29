using System;
using System.Collections.Generic;

namespace Oxide.Game.Rust.Cui
{
	public class CuiElementContainer : List<CuiElement>
	{
		public CuiElementContainer()
		{
		}

		public string Add(CuiButton button, string parent = "Hud", string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = CuiHelper.GetGuid();
			}
			CuiElement cuiElement = new CuiElement()
			{
				Name = name,
				Parent = parent,
				FadeOut = button.FadeOut
			};
			cuiElement.Components.Add(button.Button);
			cuiElement.Components.Add(button.RectTransform);
			base.Add(cuiElement);
			if (!string.IsNullOrEmpty(button.Text.Text))
			{
				CuiElement cuiElement1 = new CuiElement()
				{
					Parent = name,
					FadeOut = button.FadeOut
				};
				cuiElement1.Components.Add(button.Text);
				cuiElement1.Components.Add(new CuiRectTransformComponent());
				base.Add(cuiElement1);
			}
			return name;
		}

		public string Add(CuiLabel label, string parent = "Hud", string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = CuiHelper.GetGuid();
			}
			CuiElement cuiElement = new CuiElement()
			{
				Name = name,
				Parent = parent,
				FadeOut = label.FadeOut
			};
			cuiElement.Components.Add(label.Text);
			cuiElement.Components.Add(label.RectTransform);
			base.Add(cuiElement);
			return name;
		}

		public string Add(CuiPanel panel, string parent = "Hud", string name = null)
		{
			if (string.IsNullOrEmpty(name))
			{
				name = CuiHelper.GetGuid();
			}
			CuiElement cuiElement = new CuiElement()
			{
				Name = name,
				Parent = parent,
				FadeOut = panel.FadeOut
			};
			if (panel.Image != null)
			{
				cuiElement.Components.Add(panel.Image);
			}
			if (panel.RawImage != null)
			{
				cuiElement.Components.Add(panel.RawImage);
			}
			cuiElement.Components.Add(panel.RectTransform);
			if (panel.CursorEnabled)
			{
				cuiElement.Components.Add(new CuiNeedsCursorComponent());
			}
			base.Add(cuiElement);
			return name;
		}

		public string ToJson()
		{
			return this.ToString();
		}

		public override string ToString()
		{
			return CuiHelper.ToJson(this, false);
		}
	}
}