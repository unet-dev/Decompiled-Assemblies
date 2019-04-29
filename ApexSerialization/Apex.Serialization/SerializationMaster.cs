using Apex;
using Apex.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Apex.Serialization
{
	public static class SerializationMaster
	{
		private readonly static Dictionary<Type, IValueConverter> _typeConverters;

		private readonly static Dictionary<Type, IStager> _typeStagers;

		private readonly static Type _enumType;

		private readonly static Type _stringType;

		private static ISerializer _serializer;

		private static bool _isInitialized;

		[ThreadStatic]
		private static ICollection<IInitializeAfterDeserialization> _initBuffer;

		[ThreadStatic]
		private static ICollection<IInitializeAfterDeserialization> _requiresInit;

		static SerializationMaster()
		{
			SerializationMaster._typeConverters = new Dictionary<Type, IValueConverter>();
			SerializationMaster._typeStagers = new Dictionary<Type, IStager>();
			SerializationMaster._enumType = typeof(Enum);
			SerializationMaster._stringType = typeof(string);
		}

		public static bool ConverterExists(Type forType)
		{
			SerializationMaster.EnsureInit();
			return SerializationMaster._typeConverters.ContainsKey(forType);
		}

		public static T Deserialize<T>(string data)
		{
			SerializationMaster.EnsureInit();
			StageItem stageItem = SerializationMaster._serializer.Deserialize(data);
			if (stageItem != null)
			{
				return SerializationMaster.UnstageAndInitialize<T>(stageItem);
			}
			return default(T);
		}

		public static T Deserialize<T>(string data, ICollection<IInitializeAfterDeserialization> requiresInit)
		{
			SerializationMaster.EnsureInit();
			StageItem stageItem = SerializationMaster._serializer.Deserialize(data);
			if (stageItem == null)
			{
				return default(T);
			}
			return SerializationMaster.Unstage<T>(stageItem, requiresInit);
		}

		public static StageElement Deserialize(string data)
		{
			SerializationMaster.EnsureInit();
			return SerializationMaster._serializer.Deserialize(data) as StageElement;
		}

		private static void EnsureInit()
		{
			if (!SerializationMaster._isInitialized)
			{
				lock (SerializationMaster._typeConverters)
				{
					if (!SerializationMaster._isInitialized)
					{
						SerializationMaster.PopulateKnownSerializers();
						SerializationMaster._isInitialized = true;
					}
				}
			}
		}

		public static T FromString<T>(string value)
		{
			if (value == null)
			{
				return default(T);
			}
			Type type = typeof(T);
			IValueConverter converter = SerializationMaster.GetConverter(type);
			if (converter == null)
			{
				throw new ArgumentException(string.Concat("No converter was found for type ", type.Name));
			}
			return (T)converter.FromString(value, type);
		}

		private static IValueConverter GetConverter(Type forType)
		{
			SerializationMaster.EnsureInit();
			if (forType.IsGenericType)
			{
				forType = forType.GetGenericTypeDefinition();
			}
			else if (forType.IsEnum)
			{
				forType = SerializationMaster._enumType;
			}
			IValueConverter valueConverter = null;
			SerializationMaster._typeConverters.TryGetValue(forType, out valueConverter);
			return valueConverter;
		}

		public static IEnumerable<FieldInfo> GetSerializedFields(Type t)
		{
			return 
				from f in (IEnumerable<FieldInfo>)t.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				let attrib = f.GetAttribute<ApexSerializationAttribute>(false)
				where attrib != null
				select f;
		}

		public static IEnumerable<PropertyInfo> GetSerializedProperties(Type t)
		{
			return (
				from p in (IEnumerable<PropertyInfo>)t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				select new { p = p, attrib = p.GetAttribute<ApexSerializationAttribute>(true) }).Where((argument0) => {
				if (argument0.attrib == null || !argument0.p.CanRead)
				{
					return false;
				}
				return argument0.p.CanWrite;
			}).Select((argument1) => argument1.p);
		}

		private static IStager GetStager(Type forType)
		{
			SerializationMaster.EnsureInit();
			if (forType.IsGenericType)
			{
				forType = forType.GetGenericTypeDefinition();
			}
			else if (forType.IsArray)
			{
				forType = typeof(Array);
			}
			IStager stager = null;
			SerializationMaster._typeStagers.TryGetValue(forType, out stager);
			return stager;
		}

		private static void PopulateKnownSerializers()
		{
			object obj;
			Type[] typeArray;
			int i;
			foreach (Type type in ApexReflection.GetRelevantTypes().Where<Type>((Type t) => {
				if (!typeof(IValueConverter).IsAssignableFrom(t) && !typeof(IStager).IsAssignableFrom(t) && !typeof(ISerializer).IsAssignableFrom(t) || t.IsAbstract)
				{
					return false;
				}
				return t.GetConstructor(Type.EmptyTypes) != null;
			}))
			{
				FieldInfo field = type.GetField("instance", BindingFlags.Static | BindingFlags.Public);
				obj = (field == null ? Activator.CreateInstance(type) : field.GetValue(null));
				IValueConverter valueConverter = obj as IValueConverter;
				if (valueConverter != null)
				{
					if (valueConverter.handledTypes == null)
					{
						continue;
					}
					typeArray = valueConverter.handledTypes;
					for (i = 0; i < (int)typeArray.Length; i++)
					{
						Type type1 = typeArray[i];
						if (!SerializationMaster._typeConverters.ContainsKey(type1) || type.IsDefined<SerializationOverrideAttribute>(false))
						{
							SerializationMaster._typeConverters[type1] = valueConverter;
						}
					}
				}
				else if (!(obj is ISerializer) || SerializationMaster._serializer != null && !type.IsDefined<SerializationOverrideAttribute>(false))
				{
					IStager stager = obj as IStager;
					if (stager.handledTypes == null)
					{
						continue;
					}
					typeArray = stager.handledTypes;
					for (i = 0; i < (int)typeArray.Length; i++)
					{
						Type type2 = typeArray[i];
						if (!SerializationMaster._typeStagers.ContainsKey(type2) || type.IsDefined<SerializationOverrideAttribute>(false))
						{
							SerializationMaster._typeStagers[type2] = stager;
						}
					}
				}
				else
				{
					SerializationMaster._serializer = (ISerializer)obj;
				}
			}
		}

		private static object ReflectIn(StageElement element)
		{
			object obj;
			object obj1;
			object obj2;
			string str = element.AttributeValueOrDefault<string>("type", null);
			if (str == null)
			{
				throw new SerializationException("Invalid structure detected, missing type info.");
			}
			Type type = Type.GetType(str, true);
			try
			{
				obj = Activator.CreateInstance(type, true);
			}
			catch (MissingMethodException missingMethodException1)
			{
				MissingMethodException missingMethodException = missingMethodException1;
				throw new SerializationException(string.Format("Unable to create type {0}, ensure it has a parameterless constructor", type.Name), missingMethodException);
			}
			IInitializeAfterDeserialization initializeAfterDeserialization = obj as IInitializeAfterDeserialization;
			if (initializeAfterDeserialization != null)
			{
				if (SerializationMaster._requiresInit == null)
				{
					throw new InvalidOperationException("An entity requires initialization but was unable to register, call UnstageAndInitialize instead.");
				}
				SerializationMaster._requiresInit.Add(initializeAfterDeserialization);
			}
			foreach (PropertyInfo serializedProperty in SerializationMaster.GetSerializedProperties(type))
			{
				StageItem stageItem = element.Item(serializedProperty.Name);
				if (stageItem == null || !SerializationMaster.TryUnstage(stageItem, serializedProperty.PropertyType, out obj1))
				{
					continue;
				}
				serializedProperty.SetValue(obj, obj1, null);
			}
			foreach (FieldInfo serializedField in SerializationMaster.GetSerializedFields(type))
			{
				StageItem stageItem1 = element.Item(serializedField.Name);
				if (stageItem1 == null || !SerializationMaster.TryUnstage(stageItem1, serializedField.FieldType, out obj2))
				{
					continue;
				}
				serializedField.SetValue(obj, obj2);
			}
			return obj;
		}

		private static StageElement ReflectOut(string elementName, object item)
		{
			StageItem stageItem;
			StageItem stageItem1;
			Type type = item.GetType();
			string[] strArrays = type.AssemblyQualifiedName.Split(new char[] { ',' });
			StageElement stageElement = new StageElement(elementName, new StageItem[] { new StageAttribute("type", string.Concat(strArrays[0], ",", strArrays[1]), true) });
			foreach (SerializationMaster.AIPropInfo aIPropInfo in (
				from p in (IEnumerable<PropertyInfo>)type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				select new { p = p, attrib = p.GetAttribute<ApexSerializationAttribute>(true) }).Where((argument0) => {
				if (argument0.attrib == null || !argument0.p.CanRead)
				{
					return false;
				}
				return argument0.p.CanWrite;
			}).Select((argument1) => new SerializationMaster.AIPropInfo()
			{
				prop = argument1.p,
				defaultValue = argument1.attrib.defaultValue
			}))
			{
				object value = aIPropInfo.prop.GetValue(item, null);
				if (value == null || value.Equals(aIPropInfo.defaultValue) || !SerializationMaster.TryStage(aIPropInfo.prop.Name, value, out stageItem))
				{
					continue;
				}
				stageElement.Add(stageItem);
			}
			foreach (SerializationMaster.AIFieldInfo aIFieldInfo in 
				from f in (IEnumerable<FieldInfo>)type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
				let attrib = f.GetAttribute<ApexSerializationAttribute>(false)
				where attrib != null
				select new SerializationMaster.AIFieldInfo()
				{
					field = f,
					defaultValue = attrib.defaultValue
				})
			{
				object obj = aIFieldInfo.field.GetValue(item);
				if (obj == null || obj.Equals(aIFieldInfo.defaultValue) || !SerializationMaster.TryStage(aIFieldInfo.field.Name, obj, out stageItem1))
				{
					continue;
				}
				stageElement.Add(stageItem1);
			}
			return stageElement;
		}

		public static string Serialize<T>(T item, bool pretty = false)
		{
			SerializationMaster.EnsureInit();
			StageItem stageItem = SerializationMaster.Stage(typeof(T).Name, item);
			if (stageItem == null)
			{
				return string.Empty;
			}
			return SerializationMaster._serializer.Serialize(stageItem, pretty);
		}

		public static string Serialize(StageElement item, bool pretty = false)
		{
			SerializationMaster.EnsureInit();
			return SerializationMaster._serializer.Serialize(item, pretty);
		}

		public static StageItem Stage(string name, object value)
		{
			if (value == null)
			{
				return new StageNull(name);
			}
			Type type = value.GetType();
			IPrepareForSerialization prepareForSerialization = value as IPrepareForSerialization;
			if (prepareForSerialization != null)
			{
				prepareForSerialization.Prepare();
			}
			IStager stager = SerializationMaster.GetStager(type);
			if (stager != null)
			{
				return stager.StageValue(name, value);
			}
			IValueConverter converter = SerializationMaster.GetConverter(type);
			if (converter == null)
			{
				return SerializationMaster.ReflectOut(name, value);
			}
			return new StageValue(name, converter.ToString(value), type == SerializationMaster._stringType);
		}

		public static bool StagerExists(Type forType)
		{
			SerializationMaster.EnsureInit();
			return SerializationMaster._typeStagers.ContainsKey(forType);
		}

		public static StageItem ToStageAttribute(string name, object value)
		{
			if (value == null)
			{
				return null;
			}
			return new StageAttribute(name, SerializationMaster.ToString(value), value is string);
		}

		public static StageItem ToStageValue(string name, object value)
		{
			if (value == null)
			{
				return new StageNull(name);
			}
			return new StageValue(name, SerializationMaster.ToString(value), value is string);
		}

		public static string ToString(object value)
		{
			if (value == null)
			{
				return null;
			}
			IValueConverter converter = SerializationMaster.GetConverter(value.GetType());
			if (converter == null)
			{
				throw new ArgumentException(string.Concat("No converter was found for type ", value.GetType()));
			}
			return converter.ToString(value);
		}

		private static bool TryStage(string name, object value, out StageItem result)
		{
			result = SerializationMaster.Stage(name, value);
			return result != null;
		}

		private static bool TryUnstage(StageItem item, Type targetType, out object value)
		{
			value = SerializationMaster.Unstage(item, targetType);
			return value != null;
		}

		public static T Unstage<T>(StageItem item, ICollection<IInitializeAfterDeserialization> requiresInit)
		{
			T t;
			if (SerializationMaster._requiresInit != null)
			{
				throw new InvalidOperationException("Generic overloads of Unstage cannot be called during a nested unstage operation.");
			}
			SerializationMaster._requiresInit = requiresInit;
			try
			{
				object obj = SerializationMaster.Unstage(item, typeof(T));
				if (obj != null)
				{
					t = (T)obj;
				}
				else
				{
					t = default(T);
					t = t;
				}
			}
			finally
			{
				SerializationMaster._requiresInit = null;
			}
			return t;
		}

		public static T Unstage<T>(StageItem item)
		{
			return (T)SerializationMaster.Unstage(item, typeof(T));
		}

		public static object Unstage(StageItem item, Type targetType)
		{
			if (item is StageNull)
			{
				return null;
			}
			IStager stager = SerializationMaster.GetStager(targetType);
			if (stager != null)
			{
				return stager.UnstageValue(item, targetType);
			}
			StageValue stageValue = item as StageValue;
			if (stageValue == null)
			{
				StageElement stageElement = item as StageElement;
				if (stageElement == null)
				{
					throw new SerializationException(string.Concat("Unable to unstage, the element is not supported: ", targetType.Name));
				}
				return SerializationMaster.ReflectIn(stageElement);
			}
			IValueConverter converter = SerializationMaster.GetConverter(targetType);
			if (converter == null)
			{
				throw new SerializationException(string.Concat("Unable to unstage, no converter or stager was found for type: ", targetType.Name));
			}
			return converter.FromString(stageValue.@value, targetType);
		}

		public static T UnstageAndInitialize<T>(StageItem item)
		{
			if (SerializationMaster._initBuffer != null)
			{
				SerializationMaster._initBuffer.Clear();
			}
			else
			{
				SerializationMaster._initBuffer = new List<IInitializeAfterDeserialization>();
			}
			T t = SerializationMaster.Unstage<T>(item, SerializationMaster._initBuffer);
			if (SerializationMaster._initBuffer.Count > 0)
			{
				foreach (IInitializeAfterDeserialization initializeAfterDeserialization in SerializationMaster._initBuffer)
				{
					initializeAfterDeserialization.Initialize(t);
				}
				SerializationMaster._initBuffer.Clear();
			}
			return t;
		}

		private struct AIFieldInfo
		{
			internal FieldInfo field;

			internal object defaultValue;
		}

		private struct AIPropInfo
		{
			internal PropertyInfo prop;

			internal object defaultValue;
		}
	}
}