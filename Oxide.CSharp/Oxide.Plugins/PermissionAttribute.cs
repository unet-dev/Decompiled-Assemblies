using System;
using System.Runtime.CompilerServices;

namespace Oxide.Plugins
{
	[AttributeUsage(AttributeTargets.Method)]
	public class PermissionAttribute : Attribute
	{
		public string[] Permission
		{
			get;
		}

		public PermissionAttribute(string permission)
		{
			this.Permission = new string[] { permission };
		}
	}
}