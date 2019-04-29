using Rust.Workshop;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop.Editor
{
	public class FloatRow : MaterialRow
	{
		public Button Reset;

		public UnityEngine.UI.Slider Slider;

		private float Default;

		public bool IsDefault
		{
			get
			{
				return this.Default == this.Slider.@value;
			}
		}

		public FloatRow()
		{
		}

		public void OnChanged()
		{
			base.Editor.SetFloat(this.ParamName, this.Slider.@value);
		}

		public override void Read(Material source, Material def)
		{
			float num = source.GetFloat(this.ParamName);
			this.Slider.@value = num;
			this.Default = def.GetFloat(this.ParamName);
			base.Editor.SetFloat(this.ParamName, num);
		}

		public void ResetToDefault()
		{
			this.Slider.@value = this.Default;
			base.Editor.SetFloat(this.ParamName, this.Default);
		}

		public void Update()
		{
			this.Reset.gameObject.SetActive(!this.IsDefault);
		}
	}
}