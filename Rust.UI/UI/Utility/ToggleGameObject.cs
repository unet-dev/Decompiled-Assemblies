using Rust;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Rust.UI.Utility
{
	[RequireComponent(typeof(Toggle))]
	internal class ToggleGameObject : MonoBehaviour
	{
		public GameObject Target;

		private Toggle component;

		public ToggleGameObject()
		{
		}

		public void OnDisable()
		{
			if (Rust.Application.isQuitting)
			{
				return;
			}
			this.component.onValueChanged.RemoveListener(new UnityAction<bool>(this.OnToggled));
		}

		public void OnEnable()
		{
			this.component = base.GetComponent<Toggle>();
			this.component.onValueChanged.AddListener(new UnityAction<bool>(this.OnToggled));
		}

		public void OnToggled(bool value)
		{
			this.Target.SetActive(value);
		}

		public void Update()
		{
			this.component.isOn = this.Target.activeSelf;
		}
	}
}