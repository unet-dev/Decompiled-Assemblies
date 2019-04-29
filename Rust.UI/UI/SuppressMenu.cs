using System;

namespace Rust.UI
{
	public class SuppressMenu : ListComponent<SuppressMenu>
	{
		public static bool Any
		{
			get
			{
				return ListComponent<SuppressMenu>.InstanceList.Count > 0;
			}
		}

		public SuppressMenu()
		{
		}
	}
}