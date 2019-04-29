using System;
using System.Runtime.CompilerServices;

namespace Apex.Serialization
{
	public static class SerializationExtensions
	{
		public static void AddAttribute(this StageElement parent, string name, object value, bool onlyIfNotNull = true)
		{
			if (onlyIfNotNull && value == null)
			{
				return;
			}
			parent.Add(SerializationMaster.ToStageAttribute(name, value));
		}

		public static void AddTextAttribute(this StageElement parent, string name, string value, bool onlyIfNotNullOrEmpty = true)
		{
			if (onlyIfNotNullOrEmpty && string.IsNullOrEmpty(value))
			{
				return;
			}
			parent.Add(SerializationMaster.ToStageAttribute(name, value));
		}

		public static void AddTextValue(this StageContainer parent, string name, string value, bool onlyIfNotNullOrEmpty = true)
		{
			if (onlyIfNotNullOrEmpty && string.IsNullOrEmpty(value))
			{
				return;
			}
			parent.Add(SerializationMaster.ToStageValue(name, value));
		}

		public static void AddValue(this StageContainer parent, string name, object value, bool onlyIfNotNull = true)
		{
			if (onlyIfNotNull && value == null)
			{
				return;
			}
			parent.Add(SerializationMaster.Stage(name, value));
		}

		public static T AttributeValue<T>(this StageElement element, string attributeName)
		{
			if (element != null)
			{
				StageAttribute stageAttribute = element.Attribute(attributeName);
				if (stageAttribute != null)
				{
					return SerializationMaster.FromString<T>(stageAttribute.@value);
				}
			}
			throw new ArgumentException(string.Concat("No attribute by that name was found: ", attributeName));
		}

		public static T AttributeValueOrDefault<T>(this StageElement element, string attributeName, T defaultValue = null)
		{
			if (element == null)
			{
				return defaultValue;
			}
			StageAttribute stageAttribute = element.Attribute(attributeName);
			if (stageAttribute == null)
			{
				return defaultValue;
			}
			return SerializationMaster.FromString<T>(stageAttribute.@value);
		}

		public static void SetAttribute(this StageElement parent, string name, object value)
		{
			bool flag = value == null;
			StageAttribute str = parent.Attribute(name);
			if (str != null)
			{
				if (flag)
				{
					str.Remove();
					return;
				}
				if (str.isText && !(value is string))
				{
					throw new InvalidOperationException("Use SetTextAttribute to set text attributes.");
				}
				str.@value = SerializationMaster.ToString(value);
			}
			else if (!flag)
			{
				parent.Add(SerializationMaster.ToStageAttribute(name, value));
				return;
			}
		}

		public static void SetTextAttribute(this StageElement parent, string name, string value, bool removeIfEmpty = true)
		{
			bool flag;
			if (value == null)
			{
				flag = true;
			}
			else
			{
				flag = (!removeIfEmpty ? false : string.IsNullOrEmpty(value));
			}
			bool flag1 = flag;
			StageAttribute str = parent.Attribute(name);
			if (str != null)
			{
				if (flag1)
				{
					str.Remove();
					return;
				}
				if (!str.isText)
				{
					throw new InvalidOperationException("Cannot set a text value on a non-text attribute.");
				}
				str.@value = SerializationMaster.ToString(value);
			}
			else if (!flag1)
			{
				parent.Add(SerializationMaster.ToStageAttribute(name, value));
				return;
			}
		}

		public static void SetTextValue(this StageElement parent, string name, string value, bool removeIfNullOrEmpty = true)
		{
			bool flag = (!removeIfNullOrEmpty ? false : string.IsNullOrEmpty(value));
			StageItem stageItem = parent.Item(name);
			if (stageItem == null)
			{
				if (!flag)
				{
					parent.Add(SerializationMaster.ToStageValue(name, value));
				}
				return;
			}
			if (stageItem is StageAttribute)
			{
				throw new InvalidOperationException("Use SetTextAttribute to set text attributes.");
			}
			StageNull stageNull = stageItem as StageNull;
			if (stageItem != null)
			{
				if (value != null)
				{
					stageNull.Remove();
					parent.Add(SerializationMaster.ToStageValue(name, value));
				}
				return;
			}
			StageValue str = stageItem as StageValue;
			if (stageItem == null)
			{
				throw new InvalidOperationException("Only value elements can be set using this method.");
			}
			if (flag)
			{
				stageItem.Remove();
				return;
			}
			if (value == null)
			{
				stageItem.Remove();
				parent.Add(new StageNull(name));
				return;
			}
			if (!str.isText)
			{
				throw new InvalidOperationException("Cannot set a text value on a non-text value item.");
			}
			str.@value = SerializationMaster.ToString(value);
		}

		public static void SetValue(this StageElement parent, string name, object value, bool removeIfNull = true)
		{
			bool flag = (!removeIfNull ? false : value == null);
			StageItem stageItem = parent.Item(name);
			if (stageItem == null)
			{
				if (!flag)
				{
					parent.Add(SerializationMaster.ToStageValue(name, value));
				}
				return;
			}
			if (stageItem is StageAttribute)
			{
				throw new InvalidOperationException("Use SetTextAttribute to set text attributes.");
			}
			StageNull stageNull = stageItem as StageNull;
			if (stageItem != null)
			{
				if (value != null)
				{
					stageNull.Remove();
					parent.Add(SerializationMaster.ToStageValue(name, value));
				}
				return;
			}
			StageValue str = stageItem as StageValue;
			if (stageItem == null)
			{
				throw new InvalidOperationException("Only value elements can be set using this method.");
			}
			if (flag)
			{
				stageItem.Remove();
				return;
			}
			if (value == null)
			{
				stageItem.Remove();
				parent.Add(new StageNull(name));
				return;
			}
			if (str.isText && !(value is string))
			{
				throw new InvalidOperationException("Use SetTextValue to set text values.");
			}
			str.@value = SerializationMaster.ToString(value);
		}

		public static T Value<T>(this StageElement element, string itemName)
		{
			if (element != null)
			{
				StageItem stageItem = element.Item(itemName);
				if (stageItem != null)
				{
					return stageItem.ValueOrDefault<T>(default(T));
				}
			}
			throw new ArgumentException(string.Concat("No item by that name was found: ", itemName));
		}

		public static T ValueOrDefault<T>(this StageItem item, T defaultValue = null)
		{
			if (item == null || item is StageNull)
			{
				return defaultValue;
			}
			if (item is StageContainer)
			{
				return SerializationMaster.UnstageAndInitialize<T>(item);
			}
			return SerializationMaster.FromString<T>(((StageValue)item).@value);
		}

		public static T ValueOrDefault<T>(this StageElement element, string itemName, T defaultValue = null)
		{
			if (element == null)
			{
				return defaultValue;
			}
			return element.Item(itemName).ValueOrDefault<T>(defaultValue);
		}
	}
}