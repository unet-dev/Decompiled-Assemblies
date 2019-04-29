using ProtoBuf;
using System;
using System.Reflection;

namespace ProtoBuf.Meta
{
	internal abstract class AttributeMap
	{
		public abstract Type AttributeType
		{
			get;
		}

		public abstract object Target
		{
			get;
		}

		protected AttributeMap()
		{
		}

		public static AttributeMap[] Create(TypeModel model, Type type, bool inherit)
		{
			object[] customAttributes = type.GetCustomAttributes(inherit);
			AttributeMap[] reflectionAttributeMap = new AttributeMap[(int)customAttributes.Length];
			for (int i = 0; i < (int)customAttributes.Length; i++)
			{
				reflectionAttributeMap[i] = new AttributeMap.ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return reflectionAttributeMap;
		}

		public static AttributeMap[] Create(TypeModel model, MemberInfo member, bool inherit)
		{
			object[] customAttributes = member.GetCustomAttributes(inherit);
			AttributeMap[] reflectionAttributeMap = new AttributeMap[(int)customAttributes.Length];
			for (int i = 0; i < (int)customAttributes.Length; i++)
			{
				reflectionAttributeMap[i] = new AttributeMap.ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return reflectionAttributeMap;
		}

		public static AttributeMap[] Create(TypeModel model, Assembly assembly)
		{
			object[] customAttributes = assembly.GetCustomAttributes(false);
			AttributeMap[] reflectionAttributeMap = new AttributeMap[(int)customAttributes.Length];
			for (int i = 0; i < (int)customAttributes.Length; i++)
			{
				reflectionAttributeMap[i] = new AttributeMap.ReflectionAttributeMap((Attribute)customAttributes[i]);
			}
			return reflectionAttributeMap;
		}

		public abstract bool TryGet(string key, bool publicOnly, out object value);

		public bool TryGet(string key, out object value)
		{
			return this.TryGet(key, true, out value);
		}

		private sealed class ReflectionAttributeMap : AttributeMap
		{
			private readonly Attribute attribute;

			public override Type AttributeType
			{
				get
				{
					return this.attribute.GetType();
				}
			}

			public override object Target
			{
				get
				{
					return this.attribute;
				}
			}

			public ReflectionAttributeMap(Attribute attribute)
			{
				this.attribute = attribute;
			}

			public override bool TryGet(string key, bool publicOnly, out object value)
			{
				bool flag;
				MemberInfo[] instanceFieldsAndProperties = Helpers.GetInstanceFieldsAndProperties(this.attribute.GetType(), publicOnly);
				for (int i = 0; i < (int)instanceFieldsAndProperties.Length; i++)
				{
					MemberInfo memberInfo = instanceFieldsAndProperties[i];
					if (string.Equals(memberInfo.Name, key, StringComparison.OrdinalIgnoreCase))
					{
						PropertyInfo propertyInfo = memberInfo as PropertyInfo;
						if (propertyInfo == null)
						{
							FieldInfo fieldInfo = memberInfo as FieldInfo;
							if (fieldInfo == null)
							{
								throw new NotSupportedException(memberInfo.GetType().Name);
							}
							value = fieldInfo.GetValue(this.attribute);
							flag = true;
						}
						else
						{
							value = propertyInfo.GetValue(this.attribute, null);
							flag = true;
						}
						return flag;
					}
				}
				value = null;
				return false;
			}
		}
	}
}