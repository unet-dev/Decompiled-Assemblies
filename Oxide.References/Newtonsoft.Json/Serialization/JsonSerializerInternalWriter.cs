using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal class JsonSerializerInternalWriter : JsonSerializerInternalBase
	{
		private Type _rootType;

		private int _rootLevel;

		private readonly List<object> _serializeStack = new List<object>();

		public JsonSerializerInternalWriter(JsonSerializer serializer) : base(serializer)
		{
		}

		private bool CalculatePropertyValues(JsonWriter writer, object value, JsonContainerContract contract, JsonProperty member, JsonProperty property, out JsonContract memberContract, out object memberValue)
		{
			Object valueOrDefault;
			Required? itemRequired;
			if (!property.Ignored && property.Readable && this.ShouldSerialize(writer, property, value) && this.IsSpecified(writer, property, value))
			{
				if (property.PropertyContract == null)
				{
					property.PropertyContract = this.Serializer._contractResolver.ResolveContract(property.PropertyType);
				}
				memberValue = property.ValueProvider.GetValue(value);
				memberContract = (property.PropertyContract.IsSealed ? property.PropertyContract : this.GetContractSafe(memberValue));
				if (this.ShouldWriteProperty(memberValue, property))
				{
					if (this.ShouldWriteReference(memberValue, property, memberContract, contract, member))
					{
						property.WritePropertyName(writer);
						this.WriteReference(writer, memberValue);
						return false;
					}
					if (!this.CheckForCircularReference(writer, memberValue, property, memberContract, contract, member))
					{
						return false;
					}
					if (memberValue == null)
					{
						JsonObjectContract jsonObjectContract = contract as JsonObjectContract;
						Required? nullable = property._required;
						if (nullable.HasValue)
						{
							valueOrDefault = nullable.GetValueOrDefault();
						}
						else
						{
							if (jsonObjectContract != null)
							{
								itemRequired = jsonObjectContract.ItemRequired;
							}
							else
							{
								itemRequired = null;
							}
							Required? nullable1 = itemRequired;
							if (nullable1.HasValue)
							{
								valueOrDefault = nullable1.GetValueOrDefault();
							}
							else
							{
								valueOrDefault = null;
							}
						}
						if (valueOrDefault == null)
						{
							throw JsonSerializationException.Create(null, writer.ContainerPath, "Cannot write a null value for property '{0}'. Property requires a value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName), null);
						}
						if (valueOrDefault == null)
						{
							throw JsonSerializationException.Create(null, writer.ContainerPath, "Cannot write a null value for property '{0}'. Property requires a non-null value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName), null);
						}
					}
					return true;
				}
			}
			memberContract = null;
			memberValue = null;
			return false;
		}

		private bool CheckForCircularReference(JsonWriter writer, object value, JsonProperty property, JsonContract contract, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			if (value == null || contract.ContractType == JsonContractType.Primitive || contract.ContractType == JsonContractType.String)
			{
				return true;
			}
			ReferenceLoopHandling? referenceLoopHandling = null;
			if (property != null)
			{
				referenceLoopHandling = property.ReferenceLoopHandling;
			}
			if (!referenceLoopHandling.HasValue && containerProperty != null)
			{
				referenceLoopHandling = containerProperty.ItemReferenceLoopHandling;
			}
			if (!referenceLoopHandling.HasValue && containerContract != null)
			{
				referenceLoopHandling = containerContract.ItemReferenceLoopHandling;
			}
			if ((this.Serializer._equalityComparer != null ? this._serializeStack.Contains<object>(value, this.Serializer._equalityComparer) : this._serializeStack.Contains(value)))
			{
				string str = "Self referencing loop detected";
				if (property != null)
				{
					str = string.Concat(str, " for property '{0}'".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
				}
				str = string.Concat(str, " with type '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType()));
				switch (referenceLoopHandling.GetValueOrDefault(this.Serializer._referenceLoopHandling))
				{
					case ReferenceLoopHandling.Error:
					{
						throw JsonSerializationException.Create(null, writer.ContainerPath, str, null);
					}
					case ReferenceLoopHandling.Ignore:
					{
						if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
						{
							this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, string.Concat(str, ". Skipping serializing self referenced value.")), null);
						}
						return false;
					}
					case ReferenceLoopHandling.Serialize:
					{
						if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
						{
							this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, string.Concat(str, ". Serializing self referenced value.")), null);
						}
						return true;
					}
				}
			}
			return true;
		}

		private JsonContract GetContractSafe(object value)
		{
			if (value == null)
			{
				return null;
			}
			return this.Serializer._contractResolver.ResolveContract(value.GetType());
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (this.InternalSerializer == null)
			{
				this.InternalSerializer = new JsonSerializerProxy(this);
			}
			return this.InternalSerializer;
		}

		private string GetPropertyName(JsonWriter writer, object name, JsonContract contract, out bool escape)
		{
			string str;
			if (contract.ContractType != JsonContractType.Primitive)
			{
				if (JsonSerializerInternalWriter.TryConvertToString(name, name.GetType(), out str))
				{
					escape = true;
					return str;
				}
				escape = true;
				return name.ToString();
			}
			JsonPrimitiveContract jsonPrimitiveContract = (JsonPrimitiveContract)contract;
			if (jsonPrimitiveContract.TypeCode == PrimitiveTypeCode.DateTime || jsonPrimitiveContract.TypeCode == PrimitiveTypeCode.DateTimeNullable)
			{
				DateTime dateTime = DateTimeUtils.EnsureDateTime((DateTime)name, writer.DateTimeZoneHandling);
				escape = false;
				StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture);
				DateTimeUtils.WriteDateTimeString(stringWriter, dateTime, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
				return stringWriter.ToString();
			}
			if (jsonPrimitiveContract.TypeCode != PrimitiveTypeCode.DateTimeOffset && jsonPrimitiveContract.TypeCode != PrimitiveTypeCode.DateTimeOffsetNullable)
			{
				escape = true;
				return Convert.ToString(name, CultureInfo.InvariantCulture);
			}
			escape = false;
			StringWriter stringWriter1 = new StringWriter(CultureInfo.InvariantCulture);
			DateTimeUtils.WriteDateTimeOffsetString(stringWriter1, (DateTimeOffset)name, writer.DateFormatHandling, writer.DateFormatString, writer.Culture);
			return stringWriter1.ToString();
		}

		private string GetReference(JsonWriter writer, object value)
		{
			string reference;
			try
			{
				reference = this.Serializer.GetReferenceResolver().GetReference(this, value);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw JsonSerializationException.Create(null, writer.ContainerPath, "Error writing object reference for '{0}'.".FormatWith(CultureInfo.InvariantCulture, value.GetType()), exception);
			}
			return reference;
		}

		private void HandleError(JsonWriter writer, int initialDepth)
		{
			base.ClearErrorContext();
			if (writer.WriteState == WriteState.Property)
			{
				writer.WriteNull();
			}
			while (writer.Top > initialDepth)
			{
				writer.WriteEnd();
			}
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(PreserveReferencesHandling value, PreserveReferencesHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasFlag(TypeNameHandling value, TypeNameHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool IsSpecified(JsonWriter writer, JsonProperty property, object target)
		{
			if (property.GetIsSpecified == null)
			{
				return true;
			}
			bool getIsSpecified = property.GetIsSpecified(target);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "IsSpecified result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, getIsSpecified)), null);
			}
			return getIsSpecified;
		}

		private void OnSerialized(JsonWriter writer, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Finished serializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnSerialized(value, this.Serializer._context);
		}

		private void OnSerializing(JsonWriter writer, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Started serializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnSerializing(value, this.Serializer._context);
		}

		private bool? ResolveIsReference(JsonContract contract, JsonProperty property, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			bool? isReference = null;
			if (property != null)
			{
				isReference = property.IsReference;
			}
			if (!isReference.HasValue && containerProperty != null)
			{
				isReference = containerProperty.ItemIsReference;
			}
			if (!isReference.HasValue && collectionContract != null)
			{
				isReference = collectionContract.ItemIsReference;
			}
			if (!isReference.HasValue)
			{
				isReference = contract.IsReference;
			}
			return isReference;
		}

		public void Serialize(JsonWriter jsonWriter, object value, Type objectType)
		{
			if (jsonWriter == null)
			{
				throw new ArgumentNullException("jsonWriter");
			}
			this._rootType = objectType;
			this._rootLevel = this._serializeStack.Count + 1;
			JsonContract contractSafe = this.GetContractSafe(value);
			try
			{
				try
				{
					if (!this.ShouldWriteReference(value, null, contractSafe, null, null))
					{
						this.SerializeValue(jsonWriter, value, contractSafe, null, null, null);
					}
					else
					{
						this.WriteReference(jsonWriter, value);
					}
				}
				catch (Exception exception)
				{
					if (!base.IsErrorHandled(null, contractSafe, null, null, jsonWriter.Path, exception))
					{
						base.ClearErrorContext();
						throw;
					}
					else
					{
						this.HandleError(jsonWriter, 0);
					}
				}
			}
			finally
			{
				this._rootType = null;
			}
		}

		private void SerializeConvertable(JsonWriter writer, JsonConverter converter, object value, JsonContract contract, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			if (this.ShouldWriteReference(value, null, contract, collectionContract, containerProperty))
			{
				this.WriteReference(writer, value);
				return;
			}
			if (!this.CheckForCircularReference(writer, value, null, contract, collectionContract, containerProperty))
			{
				return;
			}
			this._serializeStack.Add(value);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Started serializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, value.GetType(), converter.GetType())), null);
			}
			converter.WriteJson(writer, value, this.GetInternalSerializer());
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Finished serializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, value.GetType(), converter.GetType())), null);
			}
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
		}

		private void SerializeDictionary(JsonWriter writer, IDictionary values, JsonDictionaryContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			bool flag;
			object underlyingDictionary;
			IWrappedDictionary wrappedDictionaries = values as IWrappedDictionary;
			if (wrappedDictionaries != null)
			{
				underlyingDictionary = wrappedDictionaries.UnderlyingDictionary;
			}
			else
			{
				underlyingDictionary = values;
			}
			object obj = underlyingDictionary;
			this.OnSerializing(writer, contract, obj);
			this._serializeStack.Add(obj);
			this.WriteObjectStart(writer, obj, contract, member, collectionContract, containerProperty);
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.Serializer._contractResolver.ResolveContract(contract.DictionaryValueType ?? typeof(object));
			}
			if (contract.KeyContract == null)
			{
				contract.KeyContract = this.Serializer._contractResolver.ResolveContract(contract.DictionaryKeyType ?? typeof(object));
			}
			int top = writer.Top;
			IDictionaryEnumerator enumerator = values.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry entry = enumerator.Entry;
					string propertyName = this.GetPropertyName(writer, entry.Key, contract.KeyContract, out flag);
					propertyName = (contract.DictionaryKeyResolver != null ? contract.DictionaryKeyResolver(propertyName) : propertyName);
					try
					{
						object value = entry.Value;
						JsonContract finalItemContract = contract.FinalItemContract ?? this.GetContractSafe(value);
						if (this.ShouldWriteReference(value, null, finalItemContract, contract, member))
						{
							writer.WritePropertyName(propertyName, flag);
							this.WriteReference(writer, value);
						}
						else if (this.CheckForCircularReference(writer, value, null, finalItemContract, contract, member))
						{
							writer.WritePropertyName(propertyName, flag);
							this.SerializeValue(writer, value, finalItemContract, null, contract, member);
						}
						else
						{
							continue;
						}
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(obj, contract, propertyName, null, writer.ContainerPath, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(writer, top);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
				else
				{
				}
			}
			writer.WriteEndObject();
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, obj);
		}

		private void SerializeISerializable(JsonWriter writer, ISerializable value, JsonISerializableContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				string str = string.Concat("Type '{0}' implements ISerializable but cannot be serialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data.", Environment.NewLine, "To fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true.", Environment.NewLine);
				str = str.FormatWith(CultureInfo.InvariantCulture, value.GetType());
				throw JsonSerializationException.Create(null, writer.ContainerPath, str, null);
			}
			this.OnSerializing(writer, contract, value);
			this._serializeStack.Add(value);
			this.WriteObjectStart(writer, value, contract, member, collectionContract, containerProperty);
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new FormatterConverter());
			value.GetObjectData(serializationInfo, this.Serializer._context);
			SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
			while (enumerator.MoveNext())
			{
				SerializationEntry current = enumerator.Current;
				JsonContract contractSafe = this.GetContractSafe(current.Value);
				if (!this.ShouldWriteReference(current.Value, null, contractSafe, contract, member))
				{
					if (!this.CheckForCircularReference(writer, current.Value, null, contractSafe, contract, member))
					{
						continue;
					}
					writer.WritePropertyName(current.Name);
					this.SerializeValue(writer, current.Value, contractSafe, null, contract, member);
				}
				else
				{
					writer.WritePropertyName(current.Name);
					this.WriteReference(writer, current.Value);
				}
			}
			writer.WriteEndObject();
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, value);
		}

		private void SerializeList(JsonWriter writer, IEnumerable values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			object underlyingCollection;
			IWrappedCollection wrappedCollections = values as IWrappedCollection;
			if (wrappedCollections != null)
			{
				underlyingCollection = wrappedCollections.UnderlyingCollection;
			}
			else
			{
				underlyingCollection = values;
			}
			object obj = underlyingCollection;
			this.OnSerializing(writer, contract, obj);
			this._serializeStack.Add(obj);
			bool flag = this.WriteStartArray(writer, obj, contract, member, collectionContract, containerProperty);
			writer.WriteStartArray();
			int top = writer.Top;
			int num = 0;
			foreach (object value in values)
			{
				try
				{
					try
					{
						JsonContract finalItemContract = contract.FinalItemContract ?? this.GetContractSafe(value);
						if (this.ShouldWriteReference(value, null, finalItemContract, contract, member))
						{
							this.WriteReference(writer, value);
						}
						else if (this.CheckForCircularReference(writer, value, null, finalItemContract, contract, member))
						{
							this.SerializeValue(writer, value, finalItemContract, null, contract, member);
						}
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(obj, contract, num, null, writer.ContainerPath, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(writer, top);
						}
					}
				}
				finally
				{
					num++;
				}
			}
			writer.WriteEndArray();
			if (flag)
			{
				writer.WriteEndObject();
			}
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, obj);
		}

		private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			this.OnSerializing(writer, contract, values);
			this._serializeStack.Add(values);
			bool flag = this.WriteStartArray(writer, values, contract, member, collectionContract, containerProperty);
			this.SerializeMultidimensionalArray(writer, values, contract, member, writer.Top, new int[0]);
			if (flag)
			{
				writer.WriteEndObject();
			}
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, values);
		}

		private void SerializeMultidimensionalArray(JsonWriter writer, Array values, JsonArrayContract contract, JsonProperty member, int initialDepth, int[] indices)
		{
			int length = (int)indices.Length;
			int[] numArray = new int[length + 1];
			for (int i = 0; i < length; i++)
			{
				numArray[i] = indices[i];
			}
			writer.WriteStartArray();
			for (int j = values.GetLowerBound(length); j <= values.GetUpperBound(length); j++)
			{
				numArray[length] = j;
				if ((int)numArray.Length != values.Rank)
				{
					this.SerializeMultidimensionalArray(writer, values, contract, member, initialDepth + 1, numArray);
				}
				else
				{
					object value = values.GetValue(numArray);
					try
					{
						JsonContract finalItemContract = contract.FinalItemContract ?? this.GetContractSafe(value);
						if (this.ShouldWriteReference(value, null, finalItemContract, contract, member))
						{
							this.WriteReference(writer, value);
						}
						else if (this.CheckForCircularReference(writer, value, null, finalItemContract, contract, member))
						{
							this.SerializeValue(writer, value, finalItemContract, null, contract, member);
						}
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(values, contract, j, null, writer.ContainerPath, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(writer, initialDepth + 1);
						}
					}
				}
			}
			writer.WriteEndArray();
		}

		private void SerializeObject(JsonWriter writer, object value, JsonObjectContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			object obj;
			JsonContract jsonContract;
			bool flag;
			this.OnSerializing(writer, contract, value);
			this._serializeStack.Add(value);
			this.WriteObjectStart(writer, value, contract, member, collectionContract, containerProperty);
			int top = writer.Top;
			for (int i = 0; i < contract.Properties.Count; i++)
			{
				JsonProperty item = contract.Properties[i];
				try
				{
					if (this.CalculatePropertyValues(writer, value, contract, member, item, out jsonContract, out obj))
					{
						item.WritePropertyName(writer);
						this.SerializeValue(writer, obj, jsonContract, item, contract, member);
					}
				}
				catch (Exception exception)
				{
					if (!base.IsErrorHandled(value, contract, item.PropertyName, null, writer.ContainerPath, exception))
					{
						throw;
					}
					else
					{
						this.HandleError(writer, top);
					}
				}
			}
			if (contract.ExtensionDataGetter != null)
			{
				IEnumerable<KeyValuePair<object, object>> extensionDataGetter = contract.ExtensionDataGetter(value);
				if (extensionDataGetter != null)
				{
					foreach (KeyValuePair<object, object> keyValuePair in extensionDataGetter)
					{
						JsonContract contractSafe = this.GetContractSafe(keyValuePair.Key);
						JsonContract contractSafe1 = this.GetContractSafe(keyValuePair.Value);
						string propertyName = this.GetPropertyName(writer, keyValuePair.Key, contractSafe, out flag);
						if (!this.ShouldWriteReference(keyValuePair.Value, null, contractSafe1, contract, member))
						{
							if (!this.CheckForCircularReference(writer, keyValuePair.Value, null, contractSafe1, contract, member))
							{
								continue;
							}
							writer.WritePropertyName(propertyName);
							this.SerializeValue(writer, keyValuePair.Value, contractSafe1, null, contract, member);
						}
						else
						{
							writer.WritePropertyName(propertyName);
							this.WriteReference(writer, keyValuePair.Value);
						}
					}
				}
			}
			writer.WriteEndObject();
			this._serializeStack.RemoveAt(this._serializeStack.Count - 1);
			this.OnSerialized(writer, contract, value);
		}

		private void SerializePrimitive(JsonWriter writer, object value, JsonPrimitiveContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			if (contract.TypeCode != PrimitiveTypeCode.Bytes || !this.ShouldWriteType(TypeNameHandling.Objects, contract, member, containerContract, containerProperty))
			{
				JsonWriter.WriteValue(writer, contract.TypeCode, value);
				return;
			}
			writer.WriteStartObject();
			this.WriteTypeProperty(writer, contract.CreatedType);
			writer.WritePropertyName("$value", false);
			JsonWriter.WriteValue(writer, contract.TypeCode, value);
			writer.WriteEndObject();
		}

		private void SerializeString(JsonWriter writer, object value, JsonStringContract contract)
		{
			string str;
			this.OnSerializing(writer, contract, value);
			JsonSerializerInternalWriter.TryConvertToString(value, contract.UnderlyingType, out str);
			writer.WriteValue(str);
			this.OnSerialized(writer, contract, value);
		}

		private void SerializeValue(JsonWriter writer, object value, JsonContract valueContract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			object converter;
			IDictionary dictionaries;
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			if (member != null)
			{
				converter = member.Converter;
			}
			else
			{
				converter = null;
			}
			if (converter == null)
			{
				if (containerProperty != null)
				{
					converter = containerProperty.ItemConverter;
				}
				else
				{
					converter = null;
				}
				if (converter == null)
				{
					if (containerContract != null)
					{
						converter = containerContract.ItemConverter;
					}
					else
					{
						converter = null;
					}
					if (converter == null)
					{
						converter = valueContract.Converter ?? (this.Serializer.GetMatchingConverter(valueContract.UnderlyingType) ?? valueContract.InternalConverter);
					}
				}
			}
			JsonConverter jsonConverter = (JsonConverter)converter;
			if (jsonConverter != null && jsonConverter.CanWrite)
			{
				this.SerializeConvertable(writer, jsonConverter, value, valueContract, containerContract, containerProperty);
				return;
			}
			switch (valueContract.ContractType)
			{
				case JsonContractType.Object:
				{
					this.SerializeObject(writer, value, (JsonObjectContract)valueContract, member, containerContract, containerProperty);
					return;
				}
				case JsonContractType.Array:
				{
					JsonArrayContract jsonArrayContract = (JsonArrayContract)valueContract;
					if (!jsonArrayContract.IsMultidimensionalArray)
					{
						this.SerializeList(writer, (IEnumerable)value, jsonArrayContract, member, containerContract, containerProperty);
						return;
					}
					this.SerializeMultidimensionalArray(writer, (Array)value, jsonArrayContract, member, containerContract, containerProperty);
					return;
				}
				case JsonContractType.Primitive:
				{
					this.SerializePrimitive(writer, value, (JsonPrimitiveContract)valueContract, member, containerContract, containerProperty);
					return;
				}
				case JsonContractType.String:
				{
					this.SerializeString(writer, value, (JsonStringContract)valueContract);
					return;
				}
				case JsonContractType.Dictionary:
				{
					JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)valueContract;
					JsonWriter jsonWriter = writer;
					if (value is IDictionary)
					{
						dictionaries = (IDictionary)value;
					}
					else
					{
						dictionaries = jsonDictionaryContract.CreateWrapper(value);
					}
					this.SerializeDictionary(jsonWriter, dictionaries, jsonDictionaryContract, member, containerContract, containerProperty);
					return;
				}
				case JsonContractType.Dynamic:
				{
					return;
				}
				case JsonContractType.Serializable:
				{
					this.SerializeISerializable(writer, (ISerializable)value, (JsonISerializableContract)valueContract, member, containerContract, containerProperty);
					return;
				}
				case JsonContractType.Linq:
				{
					((JToken)value).WriteTo(writer, this.Serializer.Converters.ToArray<JsonConverter>());
					return;
				}
				default:
				{
					return;
				}
			}
		}

		private bool ShouldSerialize(JsonWriter writer, JsonProperty property, object target)
		{
			if (property.ShouldSerialize == null)
			{
				return true;
			}
			bool shouldSerialize = property.ShouldSerialize(target);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "ShouldSerialize result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, shouldSerialize)), null);
			}
			return shouldSerialize;
		}

		private bool ShouldWriteDynamicProperty(object memberValue)
		{
			if (this.Serializer._nullValueHandling == NullValueHandling.Ignore && memberValue == null)
			{
				return false;
			}
			if (this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Ignore) && (memberValue == null || MiscellaneousUtils.ValueEquals(memberValue, ReflectionUtils.GetDefaultValue(memberValue.GetType()))))
			{
				return false;
			}
			return true;
		}

		private bool ShouldWriteProperty(object memberValue, JsonProperty property)
		{
			if (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) == NullValueHandling.Ignore && memberValue == null)
			{
				return false;
			}
			if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) && MiscellaneousUtils.ValueEquals(memberValue, property.GetResolvedDefaultValue()))
			{
				return false;
			}
			return true;
		}

		private bool ShouldWriteReference(object value, JsonProperty property, JsonContract valueContract, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			if (value == null)
			{
				return false;
			}
			if (valueContract.ContractType == JsonContractType.Primitive || valueContract.ContractType == JsonContractType.String)
			{
				return false;
			}
			bool? nullable = this.ResolveIsReference(valueContract, property, collectionContract, containerProperty);
			if (!nullable.HasValue)
			{
				nullable = (valueContract.ContractType != JsonContractType.Array ? new bool?(this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects)) : new bool?(this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays)));
			}
			if (!nullable.GetValueOrDefault())
			{
				return false;
			}
			return this.Serializer.GetReferenceResolver().IsReferenced(this, value);
		}

		private bool ShouldWriteType(TypeNameHandling typeNameHandlingFlag, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			TypeNameHandling? nullable;
			TypeNameHandling? nullable1;
			TypeNameHandling? typeNameHandling;
			TypeNameHandling valueOrDefault;
			TypeNameHandling? itemTypeNameHandling;
			TypeNameHandling? itemTypeNameHandling1;
			if (member != null)
			{
				typeNameHandling = member.TypeNameHandling;
			}
			else
			{
				nullable = null;
				typeNameHandling = nullable;
			}
			TypeNameHandling? nullable2 = typeNameHandling;
			if (nullable2.HasValue)
			{
				valueOrDefault = nullable2.GetValueOrDefault();
			}
			else
			{
				if (containerProperty != null)
				{
					itemTypeNameHandling = containerProperty.ItemTypeNameHandling;
				}
				else
				{
					nullable1 = null;
					itemTypeNameHandling = nullable1;
				}
				nullable = itemTypeNameHandling;
				if (nullable.HasValue)
				{
					valueOrDefault = nullable.GetValueOrDefault();
				}
				else
				{
					if (containerContract != null)
					{
						itemTypeNameHandling1 = containerContract.ItemTypeNameHandling;
					}
					else
					{
						itemTypeNameHandling1 = null;
					}
					nullable1 = itemTypeNameHandling1;
					valueOrDefault = (nullable1.HasValue ? nullable1.GetValueOrDefault() : this.Serializer._typeNameHandling);
				}
			}
			TypeNameHandling typeNameHandling1 = valueOrDefault;
			if (this.HasFlag(typeNameHandling1, typeNameHandlingFlag))
			{
				return true;
			}
			if (this.HasFlag(typeNameHandling1, TypeNameHandling.Auto))
			{
				if (member != null)
				{
					if (contract.UnderlyingType != member.PropertyContract.CreatedType)
					{
						return true;
					}
				}
				else if (containerContract != null)
				{
					if (containerContract.ItemContract == null || contract.UnderlyingType != containerContract.ItemContract.CreatedType)
					{
						return true;
					}
				}
				else if (this._rootType != null && this._serializeStack.Count == this._rootLevel)
				{
					JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(this._rootType);
					if (contract.UnderlyingType != jsonContract.CreatedType)
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static bool TryConvertToString(object value, Type type, out string s)
		{
			TypeConverter converter = ConvertUtils.GetConverter(type);
			if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter) && converter.CanConvertTo(typeof(string)))
			{
				s = converter.ConvertToInvariantString(value);
				return true;
			}
			if (!(value is Type))
			{
				s = null;
				return false;
			}
			s = ((Type)value).AssemblyQualifiedName;
			return true;
		}

		private void WriteObjectStart(JsonWriter writer, object value, JsonContract contract, JsonProperty member, JsonContainerContract collectionContract, JsonProperty containerProperty)
		{
			writer.WriteStartObject();
			bool? nullable = this.ResolveIsReference(contract, member, collectionContract, containerProperty);
			if ((nullable.HasValue ? nullable.GetValueOrDefault() : this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Objects)) && (member == null || member.Writable))
			{
				this.WriteReferenceIdProperty(writer, contract.UnderlyingType, value);
			}
			if (this.ShouldWriteType(TypeNameHandling.Objects, contract, member, collectionContract, containerProperty))
			{
				this.WriteTypeProperty(writer, contract.UnderlyingType);
			}
		}

		private void WriteReference(JsonWriter writer, object value)
		{
			string reference = this.GetReference(writer, value);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(null, writer.Path, "Writing object reference to Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, reference, value.GetType())), null);
			}
			writer.WriteStartObject();
			writer.WritePropertyName("$ref", false);
			writer.WriteValue(reference);
			writer.WriteEndObject();
		}

		private void WriteReferenceIdProperty(JsonWriter writer, Type type, object value)
		{
			string reference = this.GetReference(writer, value);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "Writing object reference Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, reference, type)), null);
			}
			writer.WritePropertyName("$id", false);
			writer.WriteValue(reference);
		}

		private bool WriteStartArray(JsonWriter writer, object values, JsonArrayContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			bool flag;
			bool? nullable = this.ResolveIsReference(contract, member, containerContract, containerProperty);
			bool flag1 = (nullable.HasValue ? nullable.GetValueOrDefault() : this.HasFlag(this.Serializer._preserveReferencesHandling, PreserveReferencesHandling.Arrays));
			if (!flag1)
			{
				flag = false;
			}
			else
			{
				flag = (member == null ? true : member.Writable);
			}
			flag1 = flag;
			bool flag2 = this.ShouldWriteType(TypeNameHandling.Arrays, contract, member, containerContract, containerProperty);
			bool flag3 = flag1 | flag2;
			if (flag3)
			{
				writer.WriteStartObject();
				if (flag1)
				{
					this.WriteReferenceIdProperty(writer, contract.UnderlyingType, values);
				}
				if (flag2)
				{
					this.WriteTypeProperty(writer, values.GetType());
				}
				writer.WritePropertyName("$values", false);
			}
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.Serializer._contractResolver.ResolveContract(contract.CollectionItemType ?? typeof(object));
			}
			return flag3;
		}

		private void WriteTypeProperty(JsonWriter writer, Type type)
		{
			string typeName = ReflectionUtils.GetTypeName(type, this.Serializer._typeNameAssemblyFormat, this.Serializer._binder);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, writer.Path, "Writing type name '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, typeName, type)), null);
			}
			writer.WritePropertyName("$type", false);
			writer.WriteValue(typeName);
		}
	}
}