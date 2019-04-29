using Newtonsoft.Json.Shims;
using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple=false)]
	[Preserve]
	public sealed class JsonConverterAttribute : Attribute
	{
		private readonly Type _converterType;

		public object[] ConverterParameters
		{
			get;
			private set;
		}

		public Type ConverterType
		{
			get
			{
				return this._converterType;
			}
		}

		public JsonConverterAttribute(Type converterType)
		{
			if (converterType == null)
			{
				throw new ArgumentNullException("converterType");
			}
			this._converterType = converterType;
		}

		public JsonConverterAttribute(Type converterType, params object[] converterParameters) : this(converterType)
		{
			this.ConverterParameters = converterParameters;
		}
	}
}