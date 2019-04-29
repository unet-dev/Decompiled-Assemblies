using Newtonsoft.Json.Shims;
using System;
using System.ComponentModel;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JPropertyDescriptor : PropertyDescriptor
	{
		public override Type ComponentType
		{
			get
			{
				return typeof(JObject);
			}
		}

		public override bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		protected override int NameHashCode
		{
			get
			{
				return base.NameHashCode;
			}
		}

		public override Type PropertyType
		{
			get
			{
				return typeof(object);
			}
		}

		public JPropertyDescriptor(string name) : base(name, null)
		{
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		private static JObject CastInstance(object instance)
		{
			return (JObject)instance;
		}

		public override object GetValue(object component)
		{
			return JPropertyDescriptor.CastInstance(component)[this.Name];
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			JToken jValue;
			if (value is JToken)
			{
				jValue = (JToken)value;
			}
			else
			{
				jValue = new JValue(value);
			}
			JPropertyDescriptor.CastInstance(component)[this.Name] = jValue;
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}