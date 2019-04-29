using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
	[RequireComponent(typeof(Toggle))]
	internal class ForceWeather : MonoBehaviour
	{
		private Toggle component;

		public bool Rain;

		public bool Fog;

		public bool Wind;

		public bool Clouds;

		public ForceWeather()
		{
		}

		public void OnEnable()
		{
			this.component = base.GetComponent<Toggle>();
		}

		public void Update()
		{
			object obj;
			object obj1;
			object obj2;
			object obj3;
			if (SingletonComponent<Climate>.Instance == null)
			{
				return;
			}
			if (this.Rain)
			{
				float rain = SingletonComponent<Climate>.Instance.Overrides.Rain;
				if (this.component.isOn)
				{
					obj3 = 1;
				}
				else
				{
					obj3 = null;
				}
				SingletonComponent<Climate>.Instance.Overrides.Rain = Mathf.MoveTowards(rain, (float)obj3, Time.deltaTime / 2f);
			}
			if (this.Fog)
			{
				float fog = SingletonComponent<Climate>.Instance.Overrides.Fog;
				if (this.component.isOn)
				{
					obj2 = 1;
				}
				else
				{
					obj2 = null;
				}
				SingletonComponent<Climate>.Instance.Overrides.Fog = Mathf.MoveTowards(fog, (float)obj2, Time.deltaTime / 2f);
			}
			if (this.Wind)
			{
				float wind = SingletonComponent<Climate>.Instance.Overrides.Wind;
				if (this.component.isOn)
				{
					obj1 = 1;
				}
				else
				{
					obj1 = null;
				}
				SingletonComponent<Climate>.Instance.Overrides.Wind = Mathf.MoveTowards(wind, (float)obj1, Time.deltaTime / 2f);
			}
			if (this.Clouds)
			{
				float clouds = SingletonComponent<Climate>.Instance.Overrides.Clouds;
				if (this.component.isOn)
				{
					obj = 1;
				}
				else
				{
					obj = null;
				}
				SingletonComponent<Climate>.Instance.Overrides.Clouds = Mathf.MoveTowards(clouds, (float)obj, Time.deltaTime / 2f);
			}
		}
	}
}