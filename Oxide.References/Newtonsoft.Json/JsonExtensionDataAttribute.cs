using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=false)]
	[Preserve]
	public class JsonExtensionDataAttribute : Attribute
	{
		public bool ReadData
		{
			get;
			set;
		}

		public bool WriteData
		{
			get;
			set;
		}

		public JsonExtensionDataAttribute()
		{
			this.WriteData = true;
			this.ReadData = true;
		}
	}
}