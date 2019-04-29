using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rust.Workshop
{
	public class WorkshopViewmodelControls : MonoBehaviour
	{
		public Toggle Enabled;

		public Toggle Ironsights;

		public Toggle admire;

		public WorkshopViewmodelControls()
		{
		}

		private void Clear()
		{
			this.Ironsights.isOn = false;
		}

		internal void DoUpdate(GameObject ViewModel)
		{
			if (ViewModel == null)
			{
				base.gameObject.SetActive(false);
				return;
			}
			base.gameObject.SetActive(true);
			if (!this.Enabled.isOn)
			{
				ViewModel.SetActive(false);
				return;
			}
			ViewModel.SetActive(true);
			Camera.main.fieldOfView = 85f;
			ViewModel.SendMessage("SetIronsightsEnabled", this.Ironsights.isOn, SendMessageOptions.DontRequireReceiver);
			ViewModel.SendMessage("OnCameraPositionChanged", Camera.main, SendMessageOptions.DontRequireReceiver);
			if (this.admire.isOn)
			{
				ViewModel.SendMessage("TriggerAdmire", SendMessageOptions.DontRequireReceiver);
				this.admire.isOn = false;
			}
		}
	}
}