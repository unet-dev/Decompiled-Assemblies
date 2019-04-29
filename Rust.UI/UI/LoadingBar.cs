using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI
{
	public class LoadingBar : MonoBehaviour
	{
		public CanvasGroup Canvas;

		public UnityEngine.UI.Text Label;

		public UnityEngine.UI.Text SubLabel;

		public Image ProgressImage;

		public bool Active
		{
			set
			{
				if (!this.Canvas)
				{
					return;
				}
				this.Canvas.alpha = (value ? 1f : 0f);
			}
		}

		public float Progress
		{
			set
			{
				if (!this.ProgressImage)
				{
					return;
				}
				this.ProgressImage.fillAmount = value;
			}
		}

		public string SubText
		{
			set
			{
				if (!this.SubLabel)
				{
					return;
				}
				this.SubLabel.text = value;
			}
		}

		public string Text
		{
			set
			{
				if (!this.Label)
				{
					return;
				}
				this.Label.text = value;
			}
		}

		public LoadingBar()
		{
		}
	}
}