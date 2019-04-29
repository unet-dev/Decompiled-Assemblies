using Rust.Workshop;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop.Editor
{
	public class ColorRow : MaterialRow
	{
		public Button Reset;

		public Slider Red;

		public Slider Green;

		public Slider Blue;

		private Color Default;

		public bool IsDefault
		{
			get
			{
				if (this.Default.r != this.Red.@value || this.Green.@value != this.Default.g)
				{
					return false;
				}
				return this.Blue.@value == this.Default.b;
			}
		}

		public ColorRow()
		{
		}

		public void OnChanged()
		{
			base.Editor.SetColor(this.ParamName, new Color(this.Red.@value, this.Green.@value, this.Blue.@value));
		}

		public override void Read(Material source, Material def)
		{
			Color color = source.GetColor(this.ParamName);
			this.Red.@value = color.r;
			this.Green.@value = color.g;
			this.Blue.@value = color.b;
			this.Default = def.GetColor(this.ParamName);
			base.Editor.SetColor(this.ParamName, color);
		}

		public void ResetToDefault()
		{
			this.Red.@value = this.Default.r;
			this.Green.@value = this.Default.g;
			this.Blue.@value = this.Default.b;
			base.Editor.SetColor(this.ParamName, this.Default);
		}

		public void Update()
		{
			this.Reset.gameObject.SetActive(!this.IsDefault);
		}
	}
}