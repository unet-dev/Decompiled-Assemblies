using Rust.Workshop;
using System;
using UnityEngine;

namespace Rust.Workshop.Editor
{
	public class MaterialRow : MonoBehaviour
	{
		public string ParamName;

		protected WorkshopItemEditor Editor
		{
			get
			{
				return base.GetComponentInParent<WorkshopItemEditor>();
			}
		}

		public MaterialRow()
		{
		}

		public virtual void Read(Material source, Material def)
		{
		}
	}
}