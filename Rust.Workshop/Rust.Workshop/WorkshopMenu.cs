using System;
using UnityEngine;

namespace Rust.Workshop
{
	internal class WorkshopMenu : MonoBehaviour
	{
		protected WorkshopItemEditor Editor
		{
			get
			{
				return base.GetComponentInParent<WorkshopItemEditor>();
			}
		}

		public WorkshopMenu()
		{
		}
	}
}