using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal class JsonSerializerInternalReader : JsonSerializerInternalBase
	{
		public JsonSerializerInternalReader(JsonSerializer serializer) : base(serializer)
		{
		}

		private void AddReference(JsonReader reader, string id, object value)
		{
			try
			{
				if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
				{
					this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Read object reference Id '{0}' for {1}.".FormatWith(CultureInfo.InvariantCulture, id, value.GetType())), null);
				}
				this.Serializer.GetReferenceResolver().AddReference(this, id, value);
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw JsonSerializationException.Create(reader, "Error reading object reference '{0}'.".FormatWith(CultureInfo.InvariantCulture, id), exception);
			}
		}

		private bool CalculatePropertyDetails(JsonProperty property, ref JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, object target, out bool useExistingValue, out object currentValue, out JsonContract propertyContract, out bool gottenCurrentValue)
		{
			currentValue = null;
			useExistingValue = false;
			propertyContract = null;
			gottenCurrentValue = false;
			if (property.Ignored)
			{
				return true;
			}
			JsonToken tokenType = reader.TokenType;
			if (property.PropertyContract == null)
			{
				property.PropertyContract = this.GetContractSafe(property.PropertyType);
			}
			if (property.ObjectCreationHandling.GetValueOrDefault(this.Serializer._objectCreationHandling) != ObjectCreationHandling.Replace && (tokenType == JsonToken.StartArray || tokenType == JsonToken.StartObject) && property.Readable)
			{
				currentValue = property.ValueProvider.GetValue(target);
				gottenCurrentValue = true;
				if (currentValue != null)
				{
					propertyContract = this.GetContractSafe(currentValue.GetType());
					useExistingValue = (propertyContract.IsReadOnlyOrFixedSize ? false : !propertyContract.UnderlyingType.IsValueType());
				}
			}
			if (!property.Writable && !useExistingValue)
			{
				return true;
			}
			if (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) == NullValueHandling.Ignore && tokenType == JsonToken.Null)
			{
				return true;
			}
			if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) && !this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) && JsonTokenUtils.IsPrimitiveToken(tokenType) && MiscellaneousUtils.ValueEquals(reader.Value, property.GetResolvedDefaultValue()))
			{
				return true;
			}
			if (currentValue != null)
			{
				propertyContract = this.GetContractSafe(currentValue.GetType());
				if (propertyContract != property.PropertyContract)
				{
					propertyConverter = this.GetConverter(propertyContract, property.MemberConverter, containerContract, containerProperty);
				}
			}
			else
			{
				propertyContract = property.PropertyContract;
			}
			return false;
		}

		private bool CheckPropertyName(JsonReader reader, string memberName)
		{
			if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.ReadAhead || !(memberName == "$id") && !(memberName == "$ref") && !(memberName == "$type") && !(memberName == "$values"))
			{
				return false;
			}
			reader.Skip();
			return true;
		}

		private static bool CoerceEmptyStringToNull(Type objectType, JsonContract contract, string s)
		{
			if (!string.IsNullOrEmpty(s) || objectType == null || objectType == typeof(string) || objectType == typeof(object) || contract == null)
			{
				return false;
			}
			return contract.IsNullable;
		}

		private object CreateISerializable(JsonReader reader, JsonISerializableContract contract, JsonProperty member, string id)
		{
			Type underlyingType = contract.UnderlyingType;
			if (!JsonTypeReflector.FullyTrusted)
			{
				string str = string.Concat("Type '{0}' implements ISerializable but cannot be deserialized using the ISerializable interface because the current application is not fully trusted and ISerializable can expose secure data.", Environment.NewLine, "To fix this error either change the environment to be fully trusted, change the application to not deserialize the type, add JsonObjectAttribute to the type or change the JsonSerializer setting ContractResolver to use a new DefaultContractResolver with IgnoreSerializableInterface set to true.", Environment.NewLine);
				str = str.FormatWith(CultureInfo.InvariantCulture, underlyingType);
				throw JsonSerializationException.Create(reader, str);
			}
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Deserializing {0} using ISerializable constructor.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			SerializationInfo serializationInfo = new SerializationInfo(contract.UnderlyingType, new JsonFormatterConverter(this, contract, member));
			bool flag = false;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					string str1 = reader.Value.ToString();
					if (!reader.Read())
					{
						throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, str1));
					}
					serializationInfo.AddValue(str1, JToken.ReadFrom(reader));
				}
				else
				{
					if (tokenType == JsonToken.Comment)
					{
						continue;
					}
					if (tokenType != JsonToken.EndObject)
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					flag = true;
				}
			}
			while (!flag && reader.Read());
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, serializationInfo, "Unexpected end when deserializing object.");
			}
			if (contract.ISerializableCreator == null)
			{
				throw JsonSerializationException.Create(reader, "ISerializable type '{0}' does not have a valid constructor. To correctly implement ISerializable a constructor that takes SerializationInfo and StreamingContext parameters should be present.".FormatWith(CultureInfo.InvariantCulture, underlyingType));
			}
			object serializableCreator = contract.ISerializableCreator(new object[] { serializationInfo, this.Serializer._context });
			if (id != null)
			{
				this.AddReference(reader, id, serializableCreator);
			}
			this.OnDeserializing(reader, contract, serializableCreator);
			this.OnDeserialized(reader, contract, serializableCreator);
			return serializableCreator;
		}

		internal object CreateISerializableItem(JToken token, Type type, JsonISerializableContract contract, JsonProperty member)
		{
			object obj;
			JsonContract contractSafe = this.GetContractSafe(type);
			JsonConverter converter = this.GetConverter(contractSafe, null, contract, member);
			JsonReader jsonReader = token.CreateReader();
			jsonReader.ReadAndAssert();
			obj = (converter == null || !converter.CanRead ? this.CreateValueInternal(jsonReader, type, contractSafe, null, contract, member, null) : this.DeserializeConvertable(converter, jsonReader, type, null));
			return obj;
		}

		private JToken CreateJObject(JsonReader reader)
		{
			JToken token;
			ValidationUtils.ArgumentNotNull(reader, "reader");
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteStartObject();
				do
				{
					if (reader.TokenType != JsonToken.PropertyName)
					{
						if (reader.TokenType == JsonToken.Comment)
						{
							continue;
						}
						jTokenWriter.WriteEndObject();
						token = jTokenWriter.Token;
						return token;
					}
					else
					{
						string value = (string)reader.Value;
						if (!reader.ReadAndMoveToContent())
						{
							break;
						}
						if (this.CheckPropertyName(reader, value))
						{
							continue;
						}
						jTokenWriter.WritePropertyName(value);
						jTokenWriter.WriteToken(reader, true, true, false);
					}
				}
				while (reader.Read());
				throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
			}
			return token;
		}

		private JToken CreateJToken(JsonReader reader, JsonContract contract)
		{
			JToken token;
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (contract != null)
			{
				if (contract.UnderlyingType == typeof(JRaw))
				{
					return JRaw.Create(reader);
				}
				if (reader.TokenType == JsonToken.Null && contract.UnderlyingType != typeof(JValue) && contract.UnderlyingType != typeof(JToken))
				{
					return null;
				}
			}
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jTokenWriter.WriteToken(reader);
				token = jTokenWriter.Token;
			}
			return token;
		}

		private object CreateList(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, object existingValue, string id)
		{
			object obj;
			bool flag;
			IList lists;
			if (this.HasNoDefinedType(contract))
			{
				return this.CreateJToken(reader, contract);
			}
			JsonArrayContract jsonArrayContract = this.EnsureArrayContract(reader, objectType, contract);
			if (existingValue != null)
			{
				if (!jsonArrayContract.CanDeserialize)
				{
					throw JsonSerializationException.Create(reader, "Cannot populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.CreatedType));
				}
				if (jsonArrayContract.ShouldCreateWrapper)
				{
					lists = jsonArrayContract.CreateWrapper(existingValue);
				}
				else
				{
					lists = (IList)existingValue;
				}
				obj = this.PopulateList(lists, reader, jsonArrayContract, member, id);
			}
			else
			{
				IList multidimensionalArray = this.CreateNewList(reader, jsonArrayContract, out flag);
				if (flag)
				{
					if (id != null)
					{
						throw JsonSerializationException.Create(reader, "Cannot preserve reference to array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
					if (contract.OnSerializingCallbacks.Count > 0)
					{
						throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
					if (contract.OnErrorCallbacks.Count > 0)
					{
						throw JsonSerializationException.Create(reader, "Cannot call OnError on an array or readonly list, or list created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
					if (!jsonArrayContract.HasParameterizedCreatorInternal && !jsonArrayContract.IsArray)
					{
						throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size list: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
					}
				}
				if (jsonArrayContract.IsMultidimensionalArray)
				{
					this.PopulateMultidimensionalArray(multidimensionalArray, reader, jsonArrayContract, member, id);
				}
				else
				{
					this.PopulateList(multidimensionalArray, reader, jsonArrayContract, member, id);
				}
				if (flag)
				{
					if (!jsonArrayContract.IsMultidimensionalArray)
					{
						if (!jsonArrayContract.IsArray)
						{
							return (jsonArrayContract.OverrideCreator ?? jsonArrayContract.ParameterizedCreator)(new object[] { multidimensionalArray });
						}
						Array arrays = Array.CreateInstance(jsonArrayContract.CollectionItemType, multidimensionalArray.Count);
						multidimensionalArray.CopyTo(arrays, 0);
						multidimensionalArray = arrays;
					}
					else
					{
						multidimensionalArray = CollectionUtils.ToMultidimensionalArray(multidimensionalArray, jsonArrayContract.CollectionItemType, contract.CreatedType.GetArrayRank());
					}
				}
				else if (multidimensionalArray is IWrappedCollection)
				{
					return ((IWrappedCollection)multidimensionalArray).UnderlyingCollection;
				}
				obj = multidimensionalArray;
			}
			return obj;
		}

		private IDictionary CreateNewDictionary(JsonReader reader, JsonDictionaryContract contract, out bool createdFromNonDefaultCreator)
		{
			if (contract.OverrideCreator != null)
			{
				if (contract.HasParameterizedCreator)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryDictionary();
				}
				createdFromNonDefaultCreator = false;
				return (IDictionary)contract.OverrideCreator(new object[0]);
			}
			if (contract.IsReadOnlyOrFixedSize)
			{
				createdFromNonDefaultCreator = true;
				return contract.CreateTemporaryDictionary();
			}
			if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				object defaultCreator = contract.DefaultCreator();
				if (contract.ShouldCreateWrapper)
				{
					defaultCreator = contract.CreateWrapper(defaultCreator);
				}
				createdFromNonDefaultCreator = false;
				return (IDictionary)defaultCreator;
			}
			if (!contract.HasParameterizedCreatorInternal)
			{
				if (contract.IsInstantiable)
				{
					throw JsonSerializationException.Create(reader, "Unable to find a default constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			createdFromNonDefaultCreator = true;
			return contract.CreateTemporaryDictionary();
		}

		private IList CreateNewList(JsonReader reader, JsonArrayContract contract, out bool createdFromNonDefaultCreator)
		{
			if (!contract.CanDeserialize)
			{
				throw JsonSerializationException.Create(reader, "Cannot create and populate list type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.CreatedType));
			}
			if (contract.OverrideCreator != null)
			{
				if (contract.HasParameterizedCreator)
				{
					createdFromNonDefaultCreator = true;
					return contract.CreateTemporaryCollection();
				}
				createdFromNonDefaultCreator = false;
				return (IList)contract.OverrideCreator(new object[0]);
			}
			if (contract.IsReadOnlyOrFixedSize)
			{
				createdFromNonDefaultCreator = true;
				IList lists = contract.CreateTemporaryCollection();
				if (contract.ShouldCreateWrapper)
				{
					lists = contract.CreateWrapper(lists);
				}
				return lists;
			}
			if (contract.DefaultCreator != null && (!contract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor))
			{
				object defaultCreator = contract.DefaultCreator();
				if (contract.ShouldCreateWrapper)
				{
					defaultCreator = contract.CreateWrapper(defaultCreator);
				}
				createdFromNonDefaultCreator = false;
				return (IList)defaultCreator;
			}
			if (!contract.HasParameterizedCreatorInternal)
			{
				if (contract.IsInstantiable)
				{
					throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
				}
				throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
			}
			createdFromNonDefaultCreator = true;
			return contract.CreateTemporaryCollection();
		}

		public object CreateNewObject(JsonReader reader, JsonObjectContract objectContract, JsonProperty containerMember, JsonProperty containerProperty, string id, out bool createdFromNonDefaultCreator)
		{
			object overrideCreator = null;
			if (objectContract.OverrideCreator != null)
			{
				if (objectContract.CreatorParameters.Count > 0)
				{
					createdFromNonDefaultCreator = true;
					return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.OverrideCreator, id);
				}
				overrideCreator = objectContract.OverrideCreator(new object[0]);
			}
			else if (objectContract.DefaultCreator != null && (!objectContract.DefaultCreatorNonPublic || this.Serializer._constructorHandling == ConstructorHandling.AllowNonPublicDefaultConstructor || objectContract.ParameterizedCreator == null))
			{
				overrideCreator = objectContract.DefaultCreator();
			}
			else if (objectContract.ParameterizedCreator != null)
			{
				createdFromNonDefaultCreator = true;
				return this.CreateObjectUsingCreatorWithParameters(reader, objectContract, containerMember, objectContract.ParameterizedCreator, id);
			}
			if (overrideCreator == null)
			{
				if (objectContract.IsInstantiable)
				{
					throw JsonSerializationException.Create(reader, "Unable to find a constructor to use for type {0}. A class should either have a default constructor, one constructor with arguments or a constructor marked with the JsonConstructor attribute.".FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
				}
				throw JsonSerializationException.Create(reader, "Could not create an instance of type {0}. Type is an interface or abstract class and cannot be instantiated.".FormatWith(CultureInfo.InvariantCulture, objectContract.UnderlyingType));
			}
			createdFromNonDefaultCreator = false;
			return overrideCreator;
		}

		private object CreateObject(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue)
		{
			string str;
			string str1;
			object obj;
			object obj1;
			object obj2;
			object obj3;
			bool flag;
			IDictionary dictionaries;
			Type type = objectType;
			if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.Ignore)
			{
				reader.ReadAndAssert();
				str = null;
			}
			else if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.ReadAhead)
			{
				reader.ReadAndAssert();
				if (this.ReadMetadataProperties(reader, ref type, ref contract, member, containerContract, containerMember, existingValue, out obj1, out str))
				{
					return obj1;
				}
			}
			else
			{
				JTokenReader culture = reader as JTokenReader;
				if (culture == null)
				{
					culture = (JTokenReader)JToken.ReadFrom(reader).CreateReader();
					culture.Culture = reader.Culture;
					culture.DateFormatString = reader.DateFormatString;
					culture.DateParseHandling = reader.DateParseHandling;
					culture.DateTimeZoneHandling = reader.DateTimeZoneHandling;
					culture.FloatParseHandling = reader.FloatParseHandling;
					culture.SupportMultipleContent = reader.SupportMultipleContent;
					culture.ReadAndAssert();
					reader = culture;
				}
				if (this.ReadMetadataPropertiesToken(culture, ref type, ref contract, member, containerContract, containerMember, existingValue, out obj, out str))
				{
					return obj;
				}
			}
			if (this.HasNoDefinedType(contract))
			{
				return this.CreateJObject(reader);
			}
			switch (contract.ContractType)
			{
				case JsonContractType.Object:
				{
					bool flag1 = false;
					JsonObjectContract jsonObjectContract = (JsonObjectContract)contract;
					obj2 = (existingValue == null || type != objectType && !type.IsAssignableFrom(existingValue.GetType()) ? this.CreateNewObject(reader, jsonObjectContract, member, containerMember, str, out flag1) : existingValue);
					if (flag1)
					{
						return obj2;
					}
					return this.PopulateObject(obj2, reader, jsonObjectContract, member, str);
				}
				case JsonContractType.Array:
				case JsonContractType.String:
				case JsonContractType.Dynamic:
				{
					str1 = string.Concat("Cannot deserialize the current JSON object (e.g. {{\"name\":\"value\"}}) into type '{0}' because the type requires a {1} to deserialize correctly.", Environment.NewLine, "To fix this error either change the JSON to a {1} or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer, not a collection type like an array or List<T>) that can be deserialized from a JSON object. JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object.", Environment.NewLine);
					str1 = str1.FormatWith(CultureInfo.InvariantCulture, type, this.GetExpectedDescription(contract));
					throw JsonSerializationException.Create(reader, str1);
				}
				case JsonContractType.Primitive:
				{
					JsonPrimitiveContract jsonPrimitiveContract = (JsonPrimitiveContract)contract;
					if (this.Serializer.MetadataPropertyHandling == MetadataPropertyHandling.Ignore || reader.TokenType != JsonToken.PropertyName || !string.Equals(reader.Value.ToString(), "$value", StringComparison.Ordinal))
					{
						str1 = string.Concat("Cannot deserialize the current JSON object (e.g. {{\"name\":\"value\"}}) into type '{0}' because the type requires a {1} to deserialize correctly.", Environment.NewLine, "To fix this error either change the JSON to a {1} or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer, not a collection type like an array or List<T>) that can be deserialized from a JSON object. JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object.", Environment.NewLine);
						str1 = str1.FormatWith(CultureInfo.InvariantCulture, type, this.GetExpectedDescription(contract));
						throw JsonSerializationException.Create(reader, str1);
					}
					reader.ReadAndAssert();
					if (reader.TokenType == JsonToken.StartObject)
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected token when deserializing primitive value: ", reader.TokenType));
					}
					object obj4 = this.CreateValueInternal(reader, type, jsonPrimitiveContract, member, null, null, existingValue);
					reader.ReadAndAssert();
					return obj4;
				}
				case JsonContractType.Dictionary:
				{
					JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)contract;
					if (existingValue != null)
					{
						if (jsonDictionaryContract.ShouldCreateWrapper)
						{
							dictionaries = jsonDictionaryContract.CreateWrapper(existingValue);
						}
						else
						{
							dictionaries = (IDictionary)existingValue;
						}
						obj3 = this.PopulateDictionary(dictionaries, reader, jsonDictionaryContract, member, str);
					}
					else
					{
						IDictionary dictionaries1 = this.CreateNewDictionary(reader, jsonDictionaryContract, out flag);
						if (flag)
						{
							if (str != null)
							{
								throw JsonSerializationException.Create(reader, "Cannot preserve reference to readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
							}
							if (contract.OnSerializingCallbacks.Count > 0)
							{
								throw JsonSerializationException.Create(reader, "Cannot call OnSerializing on readonly dictionary, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
							}
							if (contract.OnErrorCallbacks.Count > 0)
							{
								throw JsonSerializationException.Create(reader, "Cannot call OnError on readonly list, or dictionary created from a non-default constructor: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
							}
							if (!jsonDictionaryContract.HasParameterizedCreatorInternal)
							{
								throw JsonSerializationException.Create(reader, "Cannot deserialize readonly or fixed size dictionary: {0}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType));
							}
						}
						this.PopulateDictionary(dictionaries1, reader, jsonDictionaryContract, member, str);
						if (flag)
						{
							return (jsonDictionaryContract.OverrideCreator ?? jsonDictionaryContract.ParameterizedCreator)(new object[] { dictionaries1 });
						}
						if (dictionaries1 is IWrappedDictionary)
						{
							return ((IWrappedDictionary)dictionaries1).UnderlyingDictionary;
						}
						obj3 = dictionaries1;
					}
					return obj3;
				}
				case JsonContractType.Serializable:
				{
					return this.CreateISerializable(reader, (JsonISerializableContract)contract, member, str);
				}
				default:
				{
					str1 = string.Concat("Cannot deserialize the current JSON object (e.g. {{\"name\":\"value\"}}) into type '{0}' because the type requires a {1} to deserialize correctly.", Environment.NewLine, "To fix this error either change the JSON to a {1} or change the deserialized type so that it is a normal .NET type (e.g. not a primitive type like integer, not a collection type like an array or List<T>) that can be deserialized from a JSON object. JsonObjectAttribute can also be added to the type to force it to deserialize from a JSON object.", Environment.NewLine);
					str1 = str1.FormatWith(CultureInfo.InvariantCulture, type, this.GetExpectedDescription(contract));
					throw JsonSerializationException.Create(reader, str1);
				}
			}
		}

		private object CreateObjectUsingCreatorWithParameters(JsonReader reader, JsonObjectContract contract, JsonProperty containerProperty, ObjectConstructor<object> creator, string id)
		{
			JsonSerializerInternalReader.PropertyPresence propertyPresence;
			JsonSerializerInternalReader.PropertyPresence? presence;
			IDictionary dictionaries;
			IDictionary dictionaries1;
			ValidationUtils.ArgumentNotNull(creator, "creator");
			bool flag = (contract.HasRequiredOrDefaultValueProperties ? true : this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate));
			Type underlyingType = contract.UnderlyingType;
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				string str = string.Join(", ", (
					from p in contract.CreatorParameters
					select p.PropertyName).ToArray<string>());
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Deserializing {0} using creator with parameters: {1}.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType, str)), null);
			}
			List<JsonSerializerInternalReader.CreatorPropertyContext> creatorPropertyContexts = this.ResolvePropertyAndCreatorValues(contract, containerProperty, reader, underlyingType);
			if (flag)
			{
				foreach (JsonProperty property in contract.Properties)
				{
					if (!creatorPropertyContexts.All<JsonSerializerInternalReader.CreatorPropertyContext>((JsonSerializerInternalReader.CreatorPropertyContext p) => p.Property != property))
					{
						continue;
					}
					creatorPropertyContexts.Add(new JsonSerializerInternalReader.CreatorPropertyContext()
					{
						Property = property,
						Name = property.PropertyName,
						Presence = new JsonSerializerInternalReader.PropertyPresence?(JsonSerializerInternalReader.PropertyPresence.None)
					});
				}
			}
			object[] value = new object[contract.CreatorParameters.Count];
			foreach (JsonSerializerInternalReader.CreatorPropertyContext nullable in creatorPropertyContexts)
			{
				if (flag && nullable.Property != null && !nullable.Presence.HasValue)
				{
					object obj = nullable.Value;
					if (obj == null)
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Null;
					}
					else if (!(obj is string))
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Value;
					}
					else
					{
						propertyPresence = (JsonSerializerInternalReader.CoerceEmptyStringToNull(nullable.Property.PropertyType, nullable.Property.PropertyContract, (string)obj) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value);
					}
					nullable.Presence = new JsonSerializerInternalReader.PropertyPresence?(propertyPresence);
				}
				JsonProperty constructorProperty = nullable.ConstructorProperty;
				if (constructorProperty == null && nullable.Property != null)
				{
					constructorProperty = contract.CreatorParameters.ForgivingCaseSensitiveFind<JsonProperty>((JsonProperty p) => p.PropertyName, nullable.Property.UnderlyingName);
				}
				if (constructorProperty == null || constructorProperty.Ignored)
				{
					continue;
				}
				if (flag)
				{
					presence = nullable.Presence;
					if ((presence.GetValueOrDefault() == JsonSerializerInternalReader.PropertyPresence.None ? !presence.HasValue : true))
					{
						presence = nullable.Presence;
						if ((presence.GetValueOrDefault() == JsonSerializerInternalReader.PropertyPresence.Null ? !presence.HasValue : true))
						{
							goto Label0;
						}
					}
					if (constructorProperty.PropertyContract == null)
					{
						constructorProperty.PropertyContract = this.GetContractSafe(constructorProperty.PropertyType);
					}
					if (this.HasFlag(constructorProperty.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate))
					{
						nullable.Value = this.EnsureType(reader, constructorProperty.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, constructorProperty.PropertyContract, constructorProperty.PropertyType);
					}
				}
			Label0:
				int num = contract.CreatorParameters.IndexOf(constructorProperty);
				value[num] = nullable.Value;
				nullable.Used = true;
			}
			object obj1 = creator(value);
			if (id != null)
			{
				this.AddReference(reader, id, obj1);
			}
			this.OnDeserializing(reader, contract, obj1);
			foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext in creatorPropertyContexts)
			{
				if (creatorPropertyContext.Used || creatorPropertyContext.Property == null || creatorPropertyContext.Property.Ignored)
				{
					continue;
				}
				presence = creatorPropertyContext.Presence;
				if ((presence.GetValueOrDefault() == JsonSerializerInternalReader.PropertyPresence.None ? presence.HasValue : false))
				{
					continue;
				}
				JsonProperty jsonProperty = creatorPropertyContext.Property;
				object value1 = creatorPropertyContext.Value;
				if (!this.ShouldSetPropertyValue(jsonProperty, value1))
				{
					if (jsonProperty.Writable || value1 == null)
					{
						continue;
					}
					JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(jsonProperty.PropertyType);
					if (jsonContract.ContractType == JsonContractType.Array)
					{
						JsonArrayContract jsonArrayContract = (JsonArrayContract)jsonContract;
						object value2 = jsonProperty.ValueProvider.GetValue(obj1);
						if (value2 != null)
						{
							IWrappedCollection wrappedCollections = jsonArrayContract.CreateWrapper(value2);
							foreach (object obj2 in jsonArrayContract.CreateWrapper(value1))
							{
								wrappedCollections.Add(obj2);
							}
						}
					}
					else if (jsonContract.ContractType == JsonContractType.Dictionary)
					{
						JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)jsonContract;
						object value3 = jsonProperty.ValueProvider.GetValue(obj1);
						if (value3 != null)
						{
							if (jsonDictionaryContract.ShouldCreateWrapper)
							{
								dictionaries = jsonDictionaryContract.CreateWrapper(value3);
							}
							else
							{
								dictionaries = (IDictionary)value3;
							}
							IDictionary dictionaries2 = dictionaries;
							if (jsonDictionaryContract.ShouldCreateWrapper)
							{
								dictionaries1 = jsonDictionaryContract.CreateWrapper(value1);
							}
							else
							{
								dictionaries1 = (IDictionary)value1;
							}
							IDictionaryEnumerator enumerator = dictionaries1.GetEnumerator();
							try
							{
								while (enumerator.MoveNext())
								{
									DictionaryEntry entry = enumerator.Entry;
									dictionaries2.Add(entry.Key, entry.Value);
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
						}
					}
					creatorPropertyContext.Used = true;
				}
				else
				{
					jsonProperty.ValueProvider.SetValue(obj1, value1);
					creatorPropertyContext.Used = true;
				}
			}
			if (contract.ExtensionDataSetter != null)
			{
				foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext1 in creatorPropertyContexts)
				{
					if (creatorPropertyContext1.Used)
					{
						continue;
					}
					contract.ExtensionDataSetter(obj1, creatorPropertyContext1.Name, creatorPropertyContext1.Value);
				}
			}
			if (flag)
			{
				foreach (JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext2 in creatorPropertyContexts)
				{
					if (creatorPropertyContext2.Property == null)
					{
						continue;
					}
					this.EndProcessProperty(obj1, reader, contract, reader.Depth, creatorPropertyContext2.Property, creatorPropertyContext2.Presence.GetValueOrDefault(), !creatorPropertyContext2.Used);
				}
			}
			this.OnDeserialized(reader, contract, obj1);
			return obj1;
		}

		private object CreateValueInternal(JsonReader reader, Type objectType, JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue)
		{
			if (contract != null && contract.ContractType == JsonContractType.Linq)
			{
				return this.CreateJToken(reader, contract);
			}
			do
			{
				switch (reader.TokenType)
				{
					case JsonToken.StartObject:
					{
						return this.CreateObject(reader, objectType, contract, member, containerContract, containerMember, existingValue);
					}
					case JsonToken.StartArray:
					{
						return this.CreateList(reader, objectType, contract, member, existingValue, null);
					}
					case JsonToken.StartConstructor:
					{
						string str = reader.Value.ToString();
						return this.EnsureType(reader, str, CultureInfo.InvariantCulture, contract, objectType);
					}
					case JsonToken.PropertyName:
					case JsonToken.EndObject:
					case JsonToken.EndArray:
					case JsonToken.EndConstructor:
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected token while deserializing object: ", reader.TokenType));
					}
					case JsonToken.Comment:
					{
						continue;
					}
					case JsonToken.Raw:
					{
						return new JRaw((string)reader.Value);
					}
					case JsonToken.Integer:
					case JsonToken.Float:
					case JsonToken.Boolean:
					case JsonToken.Date:
					case JsonToken.Bytes:
					{
						return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
					}
					case JsonToken.String:
					{
						string value = (string)reader.Value;
						if (JsonSerializerInternalReader.CoerceEmptyStringToNull(objectType, contract, value))
						{
							return null;
						}
						if (objectType == typeof(byte[]))
						{
							return Convert.FromBase64String(value);
						}
						return this.EnsureType(reader, value, CultureInfo.InvariantCulture, contract, objectType);
					}
					case JsonToken.Null:
					case JsonToken.Undefined:
					{
						if (objectType == typeof(DBNull))
						{
							return DBNull.Value;
						}
						return this.EnsureType(reader, reader.Value, CultureInfo.InvariantCulture, contract, objectType);
					}
					default:
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected token while deserializing object: ", reader.TokenType));
					}
				}
			}
			while (reader.Read());
			throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
		}

		public object Deserialize(JsonReader reader, Type objectType, bool checkAdditionalContent)
		{
			object obj;
			object obj1;
			if (reader == null)
			{
				throw new ArgumentNullException("reader");
			}
			JsonContract contractSafe = this.GetContractSafe(objectType);
			try
			{
				JsonConverter converter = this.GetConverter(contractSafe, null, null, null);
				if (reader.TokenType != JsonToken.None || this.ReadForType(reader, contractSafe, converter != null))
				{
					obj = (converter == null || !converter.CanRead ? this.CreateValueInternal(reader, objectType, contractSafe, null, null, null, null) : this.DeserializeConvertable(converter, reader, objectType, null));
					if (checkAdditionalContent && reader.Read() && reader.TokenType != JsonToken.Comment)
					{
						throw new JsonSerializationException("Additional text found in JSON string after finishing deserializing object.");
					}
					obj1 = obj;
				}
				else
				{
					if (contractSafe != null && !contractSafe.IsNullable)
					{
						throw JsonSerializationException.Create(reader, "No JSON content found and type '{0}' is not nullable.".FormatWith(CultureInfo.InvariantCulture, contractSafe.UnderlyingType));
					}
					obj1 = null;
				}
			}
			catch (Exception exception)
			{
				if (!base.IsErrorHandled(null, contractSafe, null, reader as IJsonLineInfo, reader.Path, exception))
				{
					base.ClearErrorContext();
					throw;
				}
				else
				{
					this.HandleError(reader, false, 0);
					obj1 = null;
				}
			}
			return obj1;
		}

		private object DeserializeConvertable(JsonConverter converter, JsonReader reader, Type objectType, object existingValue)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, objectType, converter.GetType())), null);
			}
			object obj = converter.ReadJson(reader, objectType, existingValue, this.GetInternalSerializer());
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0} with converter {1}.".FormatWith(CultureInfo.InvariantCulture, objectType, converter.GetType())), null);
			}
			return obj;
		}

		private void EndProcessProperty(object newObject, JsonReader reader, JsonObjectContract contract, int initialDepth, JsonProperty property, JsonSerializerInternalReader.PropertyPresence presence, bool setDefaultValue)
		{
			Required valueOrDefault;
			if (presence == JsonSerializerInternalReader.PropertyPresence.None || presence == JsonSerializerInternalReader.PropertyPresence.Null)
			{
				try
				{
					Required? nullable = property._required;
					if (nullable.HasValue)
					{
						valueOrDefault = nullable.GetValueOrDefault();
					}
					else
					{
						Required? itemRequired = contract.ItemRequired;
						valueOrDefault = (itemRequired.HasValue ? itemRequired.GetValueOrDefault() : Required.Default);
					}
					Required required = valueOrDefault;
					if (presence == JsonSerializerInternalReader.PropertyPresence.None)
					{
						if (required == Required.AllowNull || required == Required.Always)
						{
							throw JsonSerializationException.Create(reader, "Required property '{0}' not found in JSON.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
						}
						if (setDefaultValue && !property.Ignored)
						{
							if (property.PropertyContract == null)
							{
								property.PropertyContract = this.GetContractSafe(property.PropertyType);
							}
							if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) && property.Writable)
							{
								property.ValueProvider.SetValue(newObject, this.EnsureType(reader, property.GetResolvedDefaultValue(), CultureInfo.InvariantCulture, property.PropertyContract, property.PropertyType));
							}
						}
					}
					else if (presence == JsonSerializerInternalReader.PropertyPresence.Null)
					{
						if (required == Required.Always)
						{
							throw JsonSerializationException.Create(reader, "Required property '{0}' expects a value but got null.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
						}
						if (required == Required.DisallowNull)
						{
							throw JsonSerializationException.Create(reader, "Required property '{0}' expects a non-null value.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName));
						}
					}
				}
				catch (Exception exception)
				{
					if (!base.IsErrorHandled(newObject, contract, property.PropertyName, reader as IJsonLineInfo, reader.Path, exception))
					{
						throw;
					}
					else
					{
						this.HandleError(reader, true, initialDepth);
					}
				}
			}
		}

		private JsonArrayContract EnsureArrayContract(JsonReader reader, Type objectType, JsonContract contract)
		{
			if (contract == null)
			{
				throw JsonSerializationException.Create(reader, "Could not resolve type '{0}' to a JsonContract.".FormatWith(CultureInfo.InvariantCulture, objectType));
			}
			JsonArrayContract jsonArrayContract = contract as JsonArrayContract;
			if (jsonArrayContract == null)
			{
				string str = string.Concat("Cannot deserialize the current JSON array (e.g. [1,2,3]) into type '{0}' because the type requires a {1} to deserialize correctly.", Environment.NewLine, "To fix this error either change the JSON to a {1} or change the deserialized type to an array or a type that implements a collection interface (e.g. ICollection, IList) like List<T> that can be deserialized from a JSON array. JsonArrayAttribute can also be added to the type to force it to deserialize from a JSON array.", Environment.NewLine);
				str = str.FormatWith(CultureInfo.InvariantCulture, objectType, this.GetExpectedDescription(contract));
				throw JsonSerializationException.Create(reader, str);
			}
			return jsonArrayContract;
		}

		private object EnsureType(JsonReader reader, object value, CultureInfo culture, JsonContract contract, Type targetType)
		{
			object obj;
			if (targetType == null)
			{
				return value;
			}
			if (ReflectionUtils.GetObjectType(value) == targetType)
			{
				return value;
			}
			if (value == null && contract.IsNullable)
			{
				return null;
			}
			try
			{
				if (!contract.IsConvertable)
				{
					obj = ConvertUtils.ConvertOrCast(value, culture, contract.NonNullableUnderlyingType);
				}
				else
				{
					JsonPrimitiveContract jsonPrimitiveContract = (JsonPrimitiveContract)contract;
					if (contract.IsEnum)
					{
						if (value is string)
						{
							obj = Enum.Parse(contract.NonNullableUnderlyingType, value.ToString(), true);
							return obj;
						}
						else if (ConvertUtils.IsInteger(jsonPrimitiveContract.TypeCode))
						{
							obj = Enum.ToObject(contract.NonNullableUnderlyingType, value);
							return obj;
						}
					}
					obj = Convert.ChangeType(value, contract.NonNullableUnderlyingType, culture);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				throw JsonSerializationException.Create(reader, "Error converting value {0} to type '{1}'.".FormatWith(CultureInfo.InvariantCulture, MiscellaneousUtils.FormatValueForPrint(value), targetType), exception);
			}
			return obj;
		}

		private JsonContract GetContractSafe(Type type)
		{
			if (type == null)
			{
				return null;
			}
			return this.Serializer._contractResolver.ResolveContract(type);
		}

		private JsonConverter GetConverter(JsonContract contract, JsonConverter memberConverter, JsonContainerContract containerContract, JsonProperty containerProperty)
		{
			JsonConverter itemConverter = null;
			if (memberConverter != null)
			{
				itemConverter = memberConverter;
			}
			else if (containerProperty != null && containerProperty.ItemConverter != null)
			{
				itemConverter = containerProperty.ItemConverter;
			}
			else if (containerContract != null && containerContract.ItemConverter != null)
			{
				itemConverter = containerContract.ItemConverter;
			}
			else if (contract != null)
			{
				if (contract.Converter == null)
				{
					JsonConverter matchingConverter = this.Serializer.GetMatchingConverter(contract.UnderlyingType);
					JsonConverter jsonConverter = matchingConverter;
					if (matchingConverter != null)
					{
						itemConverter = jsonConverter;
					}
					else if (contract.InternalConverter != null)
					{
						itemConverter = contract.InternalConverter;
					}
				}
				else
				{
					itemConverter = contract.Converter;
				}
			}
			return itemConverter;
		}

		internal string GetExpectedDescription(JsonContract contract)
		{
			switch (contract.ContractType)
			{
				case JsonContractType.Object:
				case JsonContractType.Dictionary:
				case JsonContractType.Serializable:
				{
					return "JSON object (e.g. {\"name\":\"value\"})";
				}
				case JsonContractType.Array:
				{
					return "JSON array (e.g. [1,2,3])";
				}
				case JsonContractType.Primitive:
				{
					return "JSON primitive value (e.g. string, number, boolean, null)";
				}
				case JsonContractType.String:
				{
					return "JSON string value";
				}
				case JsonContractType.Dynamic:
				{
					throw new ArgumentOutOfRangeException();
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		private JsonSerializerProxy GetInternalSerializer()
		{
			if (this.InternalSerializer == null)
			{
				this.InternalSerializer = new JsonSerializerProxy(this);
			}
			return this.InternalSerializer;
		}

		private void HandleError(JsonReader reader, bool readPastError, int initialDepth)
		{
			base.ClearErrorContext();
			if (readPastError)
			{
				reader.Skip();
				while (reader.Depth > initialDepth + 1 && reader.Read())
				{
				}
			}
		}

		private bool HasFlag(DefaultValueHandling value, DefaultValueHandling flag)
		{
			return (value & flag) == flag;
		}

		private bool HasNoDefinedType(JsonContract contract)
		{
			if (contract == null || contract.UnderlyingType == typeof(object))
			{
				return true;
			}
			return contract.ContractType == JsonContractType.Linq;
		}

		private void OnDeserialized(JsonReader reader, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Finished deserializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnDeserialized(value, this.Serializer._context);
		}

		private void OnDeserializing(JsonReader reader, JsonContract contract, object value)
		{
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
			{
				this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Started deserializing {0}".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType)), null);
			}
			contract.InvokeOnDeserializing(value, this.Serializer._context);
		}

		public void Populate(JsonReader reader, object target)
		{
			IDictionary dictionaries;
			string str;
			IList lists;
			ValidationUtils.ArgumentNotNull(target, "target");
			Type type = target.GetType();
			JsonContract jsonContract = this.Serializer._contractResolver.ResolveContract(type);
			if (!reader.MoveToContent())
			{
				throw JsonSerializationException.Create(reader, "No JSON content found.");
			}
			if (reader.TokenType == JsonToken.StartArray)
			{
				if (jsonContract.ContractType != JsonContractType.Array)
				{
					throw JsonSerializationException.Create(reader, "Cannot populate JSON array onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
				}
				JsonArrayContract jsonArrayContract = (JsonArrayContract)jsonContract;
				if (jsonArrayContract.ShouldCreateWrapper)
				{
					lists = jsonArrayContract.CreateWrapper(target);
				}
				else
				{
					lists = (IList)target;
				}
				this.PopulateList(lists, reader, jsonArrayContract, null, null);
				return;
			}
			if (reader.TokenType != JsonToken.StartObject)
			{
				throw JsonSerializationException.Create(reader, "Unexpected initial token '{0}' when populating object. Expected JSON object or array.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
			}
			reader.ReadAndAssert();
			string str1 = null;
			if (this.Serializer.MetadataPropertyHandling != MetadataPropertyHandling.Ignore && reader.TokenType == JsonToken.PropertyName && string.Equals(reader.Value.ToString(), "$id", StringComparison.Ordinal))
			{
				reader.ReadAndAssert();
				if (reader.Value != null)
				{
					str = reader.Value.ToString();
				}
				else
				{
					str = null;
				}
				str1 = str;
				reader.ReadAndAssert();
			}
			if (jsonContract.ContractType != JsonContractType.Dictionary)
			{
				if (jsonContract.ContractType != JsonContractType.Object)
				{
					throw JsonSerializationException.Create(reader, "Cannot populate JSON object onto type '{0}'.".FormatWith(CultureInfo.InvariantCulture, type));
				}
				this.PopulateObject(target, reader, (JsonObjectContract)jsonContract, null, str1);
				return;
			}
			JsonDictionaryContract jsonDictionaryContract = (JsonDictionaryContract)jsonContract;
			if (jsonDictionaryContract.ShouldCreateWrapper)
			{
				dictionaries = jsonDictionaryContract.CreateWrapper(target);
			}
			else
			{
				dictionaries = (IDictionary)target;
			}
			this.PopulateDictionary(dictionaries, reader, jsonDictionaryContract, null, str1);
		}

		private object PopulateDictionary(IDictionary dictionary, JsonReader reader, JsonDictionaryContract contract, JsonProperty containerProperty, string id)
		{
			object obj;
			DateTime dateTime;
			DateTimeOffset dateTimeOffset;
			object underlyingDictionary;
			IWrappedDictionary wrappedDictionaries = dictionary as IWrappedDictionary;
			if (wrappedDictionaries != null)
			{
				underlyingDictionary = wrappedDictionaries.UnderlyingDictionary;
			}
			else
			{
				underlyingDictionary = dictionary;
			}
			object obj1 = underlyingDictionary;
			if (id != null)
			{
				this.AddReference(reader, id, obj1);
			}
			this.OnDeserializing(reader, contract, obj1);
			int depth = reader.Depth;
			if (contract.KeyContract == null)
			{
				contract.KeyContract = this.GetContractSafe(contract.DictionaryKeyType);
			}
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.GetContractSafe(contract.DictionaryValueType);
			}
			JsonConverter itemConverter = contract.ItemConverter ?? this.GetConverter(contract.ItemContract, null, contract, containerProperty);
			PrimitiveTypeCode primitiveTypeCode = (contract.KeyContract is JsonPrimitiveContract ? ((JsonPrimitiveContract)contract.KeyContract).TypeCode : PrimitiveTypeCode.Empty);
			bool flag = false;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					object value = reader.Value;
					if (this.CheckPropertyName(reader, value.ToString()))
					{
						continue;
					}
					try
					{
						try
						{
							switch (primitiveTypeCode)
							{
								case PrimitiveTypeCode.DateTime:
								case PrimitiveTypeCode.DateTimeNullable:
								{
									if (!DateTimeUtils.TryParseDateTime(value.ToString(), reader.DateTimeZoneHandling, reader.DateFormatString, reader.Culture, out dateTime))
									{
										value = this.EnsureType(reader, value, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
										break;
									}
									else
									{
										value = dateTime;
										break;
									}
								}
								case PrimitiveTypeCode.DateTimeOffset:
								case PrimitiveTypeCode.DateTimeOffsetNullable:
								{
									if (!DateTimeUtils.TryParseDateTimeOffset(value.ToString(), reader.DateFormatString, reader.Culture, out dateTimeOffset))
									{
										value = this.EnsureType(reader, value, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
										break;
									}
									else
									{
										value = dateTimeOffset;
										break;
									}
								}
								default:
								{
									value = this.EnsureType(reader, value, CultureInfo.InvariantCulture, contract.KeyContract, contract.DictionaryKeyType);
									break;
								}
							}
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							throw JsonSerializationException.Create(reader, "Could not convert string '{0}' to dictionary key type '{1}'. Create a TypeConverter to convert from the string to the key type object.".FormatWith(CultureInfo.InvariantCulture, reader.Value, contract.DictionaryKeyType), exception);
						}
						if (!this.ReadForType(reader, contract.ItemContract, itemConverter != null))
						{
							throw JsonSerializationException.Create(reader, "Unexpected end when deserializing object.");
						}
						obj = (itemConverter == null || !itemConverter.CanRead ? this.CreateValueInternal(reader, contract.DictionaryValueType, contract.ItemContract, null, contract, containerProperty, null) : this.DeserializeConvertable(itemConverter, reader, contract.DictionaryValueType, null));
						dictionary[value] = obj;
					}
					catch (Exception exception2)
					{
						if (!base.IsErrorHandled(obj1, contract, value, reader as IJsonLineInfo, reader.Path, exception2))
						{
							throw;
						}
						else
						{
							this.HandleError(reader, true, depth);
						}
					}
				}
				else
				{
					if (tokenType == JsonToken.Comment)
					{
						continue;
					}
					if (tokenType != JsonToken.EndObject)
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					flag = true;
				}
			}
			while (!flag && reader.Read());
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, obj1, "Unexpected end when deserializing object.");
			}
			this.OnDeserialized(reader, contract, obj1);
			return obj1;
		}

		private object PopulateList(IList list, JsonReader reader, JsonArrayContract contract, JsonProperty containerProperty, string id)
		{
			object obj;
			object underlyingCollection;
			IWrappedCollection wrappedCollections = list as IWrappedCollection;
			if (wrappedCollections != null)
			{
				underlyingCollection = wrappedCollections.UnderlyingCollection;
			}
			else
			{
				underlyingCollection = list;
			}
			object obj1 = underlyingCollection;
			if (id != null)
			{
				this.AddReference(reader, id, obj1);
			}
			if (list.IsFixedSize)
			{
				reader.Skip();
				return obj1;
			}
			this.OnDeserializing(reader, contract, obj1);
			int depth = reader.Depth;
			if (contract.ItemContract == null)
			{
				contract.ItemContract = this.GetContractSafe(contract.CollectionItemType);
			}
			JsonConverter converter = this.GetConverter(contract.ItemContract, null, contract, containerProperty);
			int? nullable = null;
			bool flag = false;
			do
			{
				try
				{
					if (!this.ReadForType(reader, contract.ItemContract, converter != null))
					{
						break;
					}
					else if (reader.TokenType != JsonToken.EndArray)
					{
						obj = (converter == null || !converter.CanRead ? this.CreateValueInternal(reader, contract.CollectionItemType, contract.ItemContract, null, contract, containerProperty, null) : this.DeserializeConvertable(converter, reader, contract.CollectionItemType, null));
						list.Add(obj);
					}
					else
					{
						flag = true;
					}
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					JsonPosition position = reader.GetPosition(depth);
					if (!base.IsErrorHandled(obj1, contract, position.Position, reader as IJsonLineInfo, reader.Path, exception))
					{
						throw;
					}
					else
					{
						this.HandleError(reader, true, depth);
						if (nullable.HasValue)
						{
							int? nullable1 = nullable;
							int num = position.Position;
							if ((nullable1.GetValueOrDefault() == num ? nullable1.HasValue : false))
							{
								throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", exception);
							}
						}
						nullable = new int?(position.Position);
					}
				}
			}
			while (!flag);
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, obj1, "Unexpected end when deserializing array.");
			}
			this.OnDeserialized(reader, contract, obj1);
			return obj1;
		}

		private object PopulateMultidimensionalArray(IList list, JsonReader reader, JsonArrayContract contract, JsonProperty containerProperty, string id)
		{
			JsonToken tokenType;
			object obj;
			int arrayRank = contract.UnderlyingType.GetArrayRank();
			if (id != null)
			{
				this.AddReference(reader, id, list);
			}
			this.OnDeserializing(reader, contract, list);
			JsonContract contractSafe = this.GetContractSafe(contract.CollectionItemType);
			JsonConverter converter = this.GetConverter(contractSafe, null, contract, containerProperty);
			int? nullable = null;
			Stack<IList> lists = new Stack<IList>();
			lists.Push(list);
			IList lists1 = list;
			bool flag = false;
			do
			{
				int depth = reader.Depth;
				if (lists.Count != arrayRank)
				{
					if (!reader.Read())
					{
						break;
					}
					tokenType = reader.TokenType;
					if (tokenType == JsonToken.StartArray)
					{
						IList objs = new List<object>();
						lists1.Add(objs);
						lists.Push(objs);
						lists1 = objs;
					}
					else
					{
						if (tokenType == JsonToken.Comment)
						{
							continue;
						}
						if (tokenType != JsonToken.EndArray)
						{
							throw JsonSerializationException.Create(reader, string.Concat("Unexpected token when deserializing multidimensional array: ", reader.TokenType));
						}
						lists.Pop();
						if (lists.Count <= 0)
						{
							flag = true;
						}
						else
						{
							lists1 = lists.Peek();
						}
					}
				}
				else
				{
					try
					{
						if (!this.ReadForType(reader, contractSafe, converter != null))
						{
							break;
						}
						else
						{
							tokenType = reader.TokenType;
							if (tokenType != JsonToken.EndArray)
							{
								obj = (converter == null || !converter.CanRead ? this.CreateValueInternal(reader, contract.CollectionItemType, contractSafe, null, contract, containerProperty, null) : this.DeserializeConvertable(converter, reader, contract.CollectionItemType, null));
								lists1.Add(obj);
							}
							else
							{
								lists.Pop();
								lists1 = lists.Peek();
								nullable = null;
							}
						}
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						JsonPosition position = reader.GetPosition(depth);
						if (!base.IsErrorHandled(list, contract, position.Position, reader as IJsonLineInfo, reader.Path, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(reader, true, depth);
							if (nullable.HasValue)
							{
								int? nullable1 = nullable;
								int num = position.Position;
								if ((nullable1.GetValueOrDefault() == num ? nullable1.HasValue : false))
								{
									throw JsonSerializationException.Create(reader, "Infinite loop detected from error handling.", exception);
								}
							}
							nullable = new int?(position.Position);
						}
					}
				}
			}
			while (!flag);
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, list, "Unexpected end when deserializing array.");
			}
			this.OnDeserialized(reader, contract, list);
			return list;
		}

		private object PopulateObject(object newObject, JsonReader reader, JsonObjectContract contract, JsonProperty member, string id)
		{
			Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> dictionary;
			this.OnDeserializing(reader, contract, newObject);
			if (contract.HasRequiredOrDefaultValueProperties || this.HasFlag(this.Serializer._defaultValueHandling, DefaultValueHandling.Populate))
			{
				dictionary = contract.Properties.ToDictionary<JsonProperty, JsonProperty, JsonSerializerInternalReader.PropertyPresence>((JsonProperty m) => m, (JsonProperty m) => JsonSerializerInternalReader.PropertyPresence.None);
			}
			else
			{
				dictionary = null;
			}
			Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> jsonProperties = dictionary;
			if (id != null)
			{
				this.AddReference(reader, id, newObject);
			}
			int depth = reader.Depth;
			bool flag = false;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					string str = reader.Value.ToString();
					if (this.CheckPropertyName(reader, str))
					{
						continue;
					}
					try
					{
						JsonProperty closestMatchProperty = contract.Properties.GetClosestMatchProperty(str);
						if (closestMatchProperty == null)
						{
							if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
							{
								this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}".FormatWith(CultureInfo.InvariantCulture, str, contract.UnderlyingType)), null);
							}
							if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
							{
								throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, str, contract.UnderlyingType.Name));
							}
							if (reader.Read())
							{
								this.SetExtensionData(contract, member, reader, str, newObject);
							}
						}
						else if (!closestMatchProperty.Ignored && this.ShouldDeserialize(reader, closestMatchProperty, newObject))
						{
							if (closestMatchProperty.PropertyContract == null)
							{
								closestMatchProperty.PropertyContract = this.GetContractSafe(closestMatchProperty.PropertyType);
							}
							JsonConverter converter = this.GetConverter(closestMatchProperty.PropertyContract, closestMatchProperty.MemberConverter, contract, member);
							if (!this.ReadForType(reader, closestMatchProperty.PropertyContract, converter != null))
							{
								throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, str));
							}
							this.SetPropertyPresence(reader, closestMatchProperty, jsonProperties);
							if (!this.SetPropertyValue(closestMatchProperty, converter, contract, member, reader, newObject))
							{
								this.SetExtensionData(contract, member, reader, str, newObject);
							}
						}
						else if (reader.Read())
						{
							this.SetPropertyPresence(reader, closestMatchProperty, jsonProperties);
							this.SetExtensionData(contract, member, reader, str, newObject);
						}
						else
						{
							continue;
						}
					}
					catch (Exception exception)
					{
						if (!base.IsErrorHandled(newObject, contract, str, reader as IJsonLineInfo, reader.Path, exception))
						{
							throw;
						}
						else
						{
							this.HandleError(reader, true, depth);
						}
					}
				}
				else
				{
					if (tokenType == JsonToken.Comment)
					{
						continue;
					}
					if (tokenType != JsonToken.EndObject)
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					flag = true;
				}
			}
			while (!flag && reader.Read());
			if (!flag)
			{
				this.ThrowUnexpectedEndException(reader, contract, newObject, "Unexpected end when deserializing object.");
			}
			if (jsonProperties != null)
			{
				foreach (KeyValuePair<JsonProperty, JsonSerializerInternalReader.PropertyPresence> keyValuePair in jsonProperties)
				{
					JsonProperty key = keyValuePair.Key;
					JsonSerializerInternalReader.PropertyPresence value = keyValuePair.Value;
					this.EndProcessProperty(newObject, reader, contract, depth, key, value, true);
				}
			}
			this.OnDeserialized(reader, contract, newObject);
			return newObject;
		}

		private object ReadExtensionDataValue(JsonObjectContract contract, JsonProperty member, JsonReader reader)
		{
			object obj;
			if (!contract.ExtensionDataIsJToken)
			{
				obj = this.CreateValueInternal(reader, null, null, null, contract, member, null);
			}
			else
			{
				obj = JToken.ReadFrom(reader);
			}
			return obj;
		}

		private bool ReadForType(JsonReader reader, JsonContract contract, bool hasConverter)
		{
			if (hasConverter)
			{
				return reader.Read();
			}
			switch ((contract != null ? contract.InternalReadType : ReadType.Read))
			{
				case ReadType.Read:
				{
					return reader.ReadAndMoveToContent();
				}
				case ReadType.ReadAsInt32:
				{
					reader.ReadAsInt32();
					break;
				}
				case ReadType.ReadAsBytes:
				{
					reader.ReadAsBytes();
					break;
				}
				case ReadType.ReadAsString:
				{
					reader.ReadAsString();
					break;
				}
				case ReadType.ReadAsDecimal:
				{
					reader.ReadAsDecimal();
					break;
				}
				case ReadType.ReadAsDateTime:
				{
					reader.ReadAsDateTime();
					break;
				}
				case ReadType.ReadAsDateTimeOffset:
				{
					reader.ReadAsDateTimeOffset();
					break;
				}
				case ReadType.ReadAsDouble:
				{
					reader.ReadAsDouble();
					break;
				}
				case ReadType.ReadAsBoolean:
				{
					reader.ReadAsBoolean();
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException();
				}
			}
			return reader.TokenType != JsonToken.None;
		}

		private bool ReadMetadataProperties(JsonReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue, out object newValue, out string id)
		{
			bool flag;
			string str;
			string str1;
			id = null;
			newValue = null;
			if (reader.TokenType == JsonToken.PropertyName)
			{
				string str2 = reader.Value.ToString();
				if (str2.Length > 0 && str2[0] == '$')
				{
					do
					{
						str2 = reader.Value.ToString();
						if (string.Equals(str2, "$ref", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							if (reader.TokenType != JsonToken.String && reader.TokenType != JsonToken.Null)
							{
								throw JsonSerializationException.Create(reader, "JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
							}
							if (reader.Value != null)
							{
								str1 = reader.Value.ToString();
							}
							else
							{
								str1 = null;
							}
							string str3 = str1;
							reader.ReadAndAssert();
							if (str3 != null)
							{
								if (reader.TokenType == JsonToken.PropertyName)
								{
									throw JsonSerializationException.Create(reader, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"));
								}
								newValue = this.Serializer.GetReferenceResolver().ResolveReference(this, str3);
								if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
								{
									this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, str3, newValue.GetType())), null);
								}
								return true;
							}
							flag = true;
						}
						else if (string.Equals(str2, "$type", StringComparison.Ordinal))
						{
							reader.ReadAndAssert();
							string str4 = reader.Value.ToString();
							this.ResolveTypeName(reader, ref objectType, ref contract, member, containerContract, containerMember, str4);
							reader.ReadAndAssert();
							flag = true;
						}
						else if (!string.Equals(str2, "$id", StringComparison.Ordinal))
						{
							if (string.Equals(str2, "$values", StringComparison.Ordinal))
							{
								reader.ReadAndAssert();
								object obj = this.CreateList(reader, objectType, contract, member, existingValue, id);
								reader.ReadAndAssert();
								newValue = obj;
								return true;
							}
							flag = false;
						}
						else
						{
							reader.ReadAndAssert();
							if (reader.Value != null)
							{
								str = reader.Value.ToString();
							}
							else
							{
								str = null;
							}
							id = str;
							reader.ReadAndAssert();
							flag = true;
						}
					}
					while (flag && reader.TokenType == JsonToken.PropertyName);
				}
			}
			return false;
		}

		private bool ReadMetadataPropertiesToken(JTokenReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, object existingValue, out object newValue, out string id)
		{
			id = null;
			newValue = null;
			if (reader.TokenType == JsonToken.StartObject)
			{
				JObject currentToken = (JObject)reader.CurrentToken;
				JToken item = currentToken["$ref"];
				if (item != null)
				{
					if (item.Type != JTokenType.String && item.Type != JTokenType.Null)
					{
						throw JsonSerializationException.Create(item, item.Path, "JSON reference {0} property must have a string or null value.".FormatWith(CultureInfo.InvariantCulture, "$ref"), null);
					}
					JToken parent = item.Parent;
					JToken next = null;
					if (parent.Next != null)
					{
						next = parent.Next;
					}
					else if (parent.Previous != null)
					{
						next = parent.Previous;
					}
					string str = (string)item;
					if (str != null)
					{
						if (next != null)
						{
							throw JsonSerializationException.Create(next, next.Path, "Additional content found in JSON reference object. A JSON reference object should only have a {0} property.".FormatWith(CultureInfo.InvariantCulture, "$ref"), null);
						}
						newValue = this.Serializer.GetReferenceResolver().ResolveReference(this, str);
						if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Info)
						{
							this.TraceWriter.Trace(TraceLevel.Info, JsonPosition.FormatMessage(reader, reader.Path, "Resolved object reference '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, str, newValue.GetType())), null);
						}
						reader.Skip();
						return true;
					}
				}
				JToken jTokens = currentToken["$type"];
				if (jTokens != null)
				{
					string str1 = (string)jTokens;
					JsonReader jsonReader = jTokens.CreateReader();
					jsonReader.ReadAndAssert();
					this.ResolveTypeName(jsonReader, ref objectType, ref contract, member, containerContract, containerMember, str1);
					if (currentToken["$value"] != null)
					{
						while (true)
						{
							reader.ReadAndAssert();
							if (reader.TokenType == JsonToken.PropertyName && (string)reader.Value == "$value")
							{
								break;
							}
							reader.ReadAndAssert();
							reader.Skip();
						}
						return false;
					}
				}
				JToken item1 = currentToken["$id"];
				if (item1 != null)
				{
					id = (string)item1;
				}
				JToken jTokens1 = currentToken["$values"];
				if (jTokens1 != null)
				{
					JsonReader jsonReader1 = jTokens1.CreateReader();
					jsonReader1.ReadAndAssert();
					newValue = this.CreateList(jsonReader1, objectType, contract, member, existingValue, id);
					reader.Skip();
					return true;
				}
			}
			reader.ReadAndAssert();
			return false;
		}

		private List<JsonSerializerInternalReader.CreatorPropertyContext> ResolvePropertyAndCreatorValues(JsonObjectContract contract, JsonProperty containerProperty, JsonReader reader, Type objectType)
		{
			List<JsonSerializerInternalReader.CreatorPropertyContext> creatorPropertyContexts = new List<JsonSerializerInternalReader.CreatorPropertyContext>();
			bool flag = false;
			do
			{
				JsonToken tokenType = reader.TokenType;
				if (tokenType == JsonToken.PropertyName)
				{
					string str = reader.Value.ToString();
					JsonSerializerInternalReader.CreatorPropertyContext creatorPropertyContext = new JsonSerializerInternalReader.CreatorPropertyContext()
					{
						Name = reader.Value.ToString(),
						ConstructorProperty = contract.CreatorParameters.GetClosestMatchProperty(str),
						Property = contract.Properties.GetClosestMatchProperty(str)
					};
					creatorPropertyContexts.Add(creatorPropertyContext);
					JsonProperty constructorProperty = creatorPropertyContext.ConstructorProperty ?? creatorPropertyContext.Property;
					if (constructorProperty == null || constructorProperty.Ignored)
					{
						if (!reader.Read())
						{
							throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, str));
						}
						if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
						{
							this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Could not find member '{0}' on {1}.".FormatWith(CultureInfo.InvariantCulture, str, contract.UnderlyingType)), null);
						}
						if (this.Serializer._missingMemberHandling == MissingMemberHandling.Error)
						{
							throw JsonSerializationException.Create(reader, "Could not find member '{0}' on object of type '{1}'".FormatWith(CultureInfo.InvariantCulture, str, objectType.Name));
						}
						if (contract.ExtensionDataSetter == null)
						{
							reader.Skip();
						}
						else
						{
							creatorPropertyContext.Value = this.ReadExtensionDataValue(contract, containerProperty, reader);
						}
					}
					else
					{
						if (constructorProperty.PropertyContract == null)
						{
							constructorProperty.PropertyContract = this.GetContractSafe(constructorProperty.PropertyType);
						}
						JsonConverter converter = this.GetConverter(constructorProperty.PropertyContract, constructorProperty.MemberConverter, contract, containerProperty);
						if (!this.ReadForType(reader, constructorProperty.PropertyContract, converter != null))
						{
							throw JsonSerializationException.Create(reader, "Unexpected end when setting {0}'s value.".FormatWith(CultureInfo.InvariantCulture, str));
						}
						if (converter == null || !converter.CanRead)
						{
							creatorPropertyContext.Value = this.CreateValueInternal(reader, constructorProperty.PropertyType, constructorProperty.PropertyContract, constructorProperty, contract, containerProperty, null);
						}
						else
						{
							creatorPropertyContext.Value = this.DeserializeConvertable(converter, reader, constructorProperty.PropertyType, null);
						}
					}
				}
				else
				{
					if (tokenType == JsonToken.Comment)
					{
						continue;
					}
					if (tokenType != JsonToken.EndObject)
					{
						throw JsonSerializationException.Create(reader, string.Concat("Unexpected token when deserializing object: ", reader.TokenType));
					}
					flag = true;
				}
			}
			while (!flag && reader.Read());
			return creatorPropertyContexts;
		}

		private void ResolveTypeName(JsonReader reader, ref Type objectType, ref JsonContract contract, JsonProperty member, JsonContainerContract containerContract, JsonProperty containerMember, string qualifiedTypeName)
		{
			TypeNameHandling? nullable;
			TypeNameHandling? nullable1;
			string str;
			string str1;
			Type type;
			TypeNameHandling? typeNameHandling;
			bool valueOrDefault;
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
				if (containerContract != null)
				{
					itemTypeNameHandling = containerContract.ItemTypeNameHandling;
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
					if (containerMember != null)
					{
						itemTypeNameHandling1 = containerMember.ItemTypeNameHandling;
					}
					else
					{
						itemTypeNameHandling1 = null;
					}
					nullable1 = itemTypeNameHandling1;
					valueOrDefault = (nullable1.HasValue ? nullable1.GetValueOrDefault() : this.Serializer._typeNameHandling);
				}
			}
			if (valueOrDefault)
			{
				ReflectionUtils.SplitFullyQualifiedTypeName(qualifiedTypeName, out str, out str1);
				try
				{
					type = this.Serializer._binder.BindToType(str1, str);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					throw JsonSerializationException.Create(reader, "Error resolving type specified in JSON '{0}'.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName), exception);
				}
				if (type == null)
				{
					throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' was not resolved.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName));
				}
				if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
				{
					this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "Resolved type '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, qualifiedTypeName, type)), null);
				}
				if (objectType != null && !objectType.IsAssignableFrom(type))
				{
					throw JsonSerializationException.Create(reader, "Type specified in JSON '{0}' is not compatible with '{1}'.".FormatWith(CultureInfo.InvariantCulture, type.AssemblyQualifiedName, objectType.AssemblyQualifiedName));
				}
				objectType = type;
				contract = this.GetContractSafe(type);
			}
		}

		private void SetExtensionData(JsonObjectContract contract, JsonProperty member, JsonReader reader, string memberName, object o)
		{
			if (contract.ExtensionDataSetter == null)
			{
				reader.Skip();
			}
			else
			{
				try
				{
					object obj = this.ReadExtensionDataValue(contract, member, reader);
					contract.ExtensionDataSetter(o, memberName, obj);
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					throw JsonSerializationException.Create(reader, "Error setting value in extension data for type '{0}'.".FormatWith(CultureInfo.InvariantCulture, contract.UnderlyingType), exception);
				}
			}
		}

		private void SetPropertyPresence(JsonReader reader, JsonProperty property, Dictionary<JsonProperty, JsonSerializerInternalReader.PropertyPresence> requiredProperties)
		{
			JsonSerializerInternalReader.PropertyPresence propertyPresence;
			if (property != null && requiredProperties != null)
			{
				switch (reader.TokenType)
				{
					case JsonToken.String:
					{
						propertyPresence = (JsonSerializerInternalReader.CoerceEmptyStringToNull(property.PropertyType, property.PropertyContract, (string)reader.Value) ? JsonSerializerInternalReader.PropertyPresence.Null : JsonSerializerInternalReader.PropertyPresence.Value);
						break;
					}
					case JsonToken.Boolean:
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Value;
						break;
					}
					case JsonToken.Null:
					case JsonToken.Undefined:
					{
						propertyPresence = JsonSerializerInternalReader.PropertyPresence.Null;
						break;
					}
					default:
					{
						goto case JsonToken.Boolean;
					}
				}
				requiredProperties[property] = propertyPresence;
			}
		}

		private bool SetPropertyValue(JsonProperty property, JsonConverter propertyConverter, JsonContainerContract containerContract, JsonProperty containerProperty, JsonReader reader, object target)
		{
			object value;
			bool flag;
			JsonContract jsonContract;
			bool flag1;
			object obj;
			if (this.CalculatePropertyDetails(property, ref propertyConverter, containerContract, containerProperty, reader, target, out flag, out value, out jsonContract, out flag1))
			{
				return false;
			}
			if (propertyConverter == null || !propertyConverter.CanRead)
			{
				obj = this.CreateValueInternal(reader, property.PropertyType, jsonContract, property, containerContract, containerProperty, (flag ? value : null));
			}
			else
			{
				if (!flag1 && target != null && property.Readable)
				{
					value = property.ValueProvider.GetValue(target);
				}
				obj = this.DeserializeConvertable(propertyConverter, reader, property.PropertyType, value);
			}
			if (flag && obj == value || !this.ShouldSetPropertyValue(property, obj))
			{
				return flag;
			}
			property.ValueProvider.SetValue(target, obj);
			if (property.SetIsSpecified != null)
			{
				if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
				{
					this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(reader as IJsonLineInfo, reader.Path, "IsSpecified for property '{0}' on {1} set to true.".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType)), null);
				}
				property.SetIsSpecified(target, true);
			}
			return true;
		}

		private bool ShouldDeserialize(JsonReader reader, JsonProperty property, object target)
		{
			if (property.ShouldDeserialize == null)
			{
				return true;
			}
			bool shouldDeserialize = property.ShouldDeserialize(target);
			if (this.TraceWriter != null && this.TraceWriter.LevelFilter >= TraceLevel.Verbose)
			{
				this.TraceWriter.Trace(TraceLevel.Verbose, JsonPosition.FormatMessage(null, reader.Path, "ShouldDeserialize result for property '{0}' on {1}: {2}".FormatWith(CultureInfo.InvariantCulture, property.PropertyName, property.DeclaringType, shouldDeserialize)), null);
			}
			return shouldDeserialize;
		}

		private bool ShouldSetPropertyValue(JsonProperty property, object value)
		{
			if (property.NullValueHandling.GetValueOrDefault(this.Serializer._nullValueHandling) == NullValueHandling.Ignore && value == null)
			{
				return false;
			}
			if (this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Ignore) && !this.HasFlag(property.DefaultValueHandling.GetValueOrDefault(this.Serializer._defaultValueHandling), DefaultValueHandling.Populate) && MiscellaneousUtils.ValueEquals(value, property.GetResolvedDefaultValue()))
			{
				return false;
			}
			if (!property.Writable)
			{
				return false;
			}
			return true;
		}

		private void ThrowUnexpectedEndException(JsonReader reader, JsonContract contract, object currentObject, string message)
		{
			try
			{
				throw JsonSerializationException.Create(reader, message);
			}
			catch (Exception exception)
			{
				if (!base.IsErrorHandled(currentObject, contract, null, reader as IJsonLineInfo, reader.Path, exception))
				{
					throw;
				}
				else
				{
					this.HandleError(reader, false, 0);
				}
			}
		}

		internal class CreatorPropertyContext
		{
			public string Name;

			public JsonProperty Property;

			public JsonProperty ConstructorProperty;

			public JsonSerializerInternalReader.PropertyPresence? Presence;

			public object Value;

			public bool Used;

			public CreatorPropertyContext()
			{
			}
		}

		internal enum PropertyPresence
		{
			None,
			Null,
			Value
		}
	}
}