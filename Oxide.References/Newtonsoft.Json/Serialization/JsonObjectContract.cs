using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonObjectContract : JsonContainerContract
	{
		internal bool ExtensionDataIsJToken;

		private bool? _hasRequiredOrDefaultValueProperties;

		private ConstructorInfo _parametrizedConstructor;

		private ConstructorInfo _overrideConstructor;

		private ObjectConstructor<object> _overrideCreator;

		private ObjectConstructor<object> _parameterizedCreator;

		private JsonPropertyCollection _creatorParameters;

		private Type _extensionDataValueType;

		[Obsolete("ConstructorParameters is obsolete. Use CreatorParameters instead.")]
		public JsonPropertyCollection ConstructorParameters
		{
			get
			{
				return this.CreatorParameters;
			}
		}

		public JsonPropertyCollection CreatorParameters
		{
			get
			{
				if (this._creatorParameters == null)
				{
					this._creatorParameters = new JsonPropertyCollection(base.UnderlyingType);
				}
				return this._creatorParameters;
			}
		}

		public Newtonsoft.Json.Serialization.ExtensionDataGetter ExtensionDataGetter
		{
			get;
			set;
		}

		public Newtonsoft.Json.Serialization.ExtensionDataSetter ExtensionDataSetter
		{
			get;
			set;
		}

		public Type ExtensionDataValueType
		{
			get
			{
				return this._extensionDataValueType;
			}
			set
			{
				this._extensionDataValueType = value;
				this.ExtensionDataIsJToken = (value == null ? false : typeof(JToken).IsAssignableFrom(value));
			}
		}

		internal bool HasRequiredOrDefaultValueProperties
		{
			get
			{
				DefaultValueHandling? nullable;
				bool flag;
				if (!this._hasRequiredOrDefaultValueProperties.HasValue)
				{
					this._hasRequiredOrDefaultValueProperties = new bool?(false);
					if (this.ItemRequired.GetValueOrDefault(Required.Default) == Required.Default)
					{
						using (IEnumerator<JsonProperty> enumerator = this.Properties.GetEnumerator())
						{
							do
							{
								if (enumerator.MoveNext())
								{
									JsonProperty current = enumerator.Current;
									if (current.Required != Required.Default)
									{
										break;
									}
									DefaultValueHandling? defaultValueHandling = current.DefaultValueHandling;
									if (defaultValueHandling.HasValue)
									{
										nullable = new DefaultValueHandling?(defaultValueHandling.GetValueOrDefault() & DefaultValueHandling.Populate);
									}
									else
									{
										nullable = null;
									}
									DefaultValueHandling? nullable1 = nullable;
									flag = (nullable1.GetValueOrDefault() == DefaultValueHandling.Populate ? nullable1.HasValue : false);
								}
								else
								{
									return this._hasRequiredOrDefaultValueProperties.GetValueOrDefault();
								}
							}
							while (!flag);
							this._hasRequiredOrDefaultValueProperties = new bool?(true);
						}
					}
					else
					{
						this._hasRequiredOrDefaultValueProperties = new bool?(true);
					}
				}
				return this._hasRequiredOrDefaultValueProperties.GetValueOrDefault();
			}
		}

		public Required? ItemRequired
		{
			get;
			set;
		}

		public Newtonsoft.Json.MemberSerialization MemberSerialization
		{
			get;
			set;
		}

		[Obsolete("OverrideConstructor is obsolete. Use OverrideCreator instead.")]
		public ConstructorInfo OverrideConstructor
		{
			get
			{
				return this._overrideConstructor;
			}
			set
			{
				ObjectConstructor<object> objectConstructor;
				this._overrideConstructor = value;
				if (value != null)
				{
					objectConstructor = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value);
				}
				else
				{
					objectConstructor = null;
				}
				this._overrideCreator = objectConstructor;
			}
		}

		public ObjectConstructor<object> OverrideCreator
		{
			get
			{
				return this._overrideCreator;
			}
			set
			{
				this._overrideCreator = value;
				this._overrideConstructor = null;
			}
		}

		internal ObjectConstructor<object> ParameterizedCreator
		{
			get
			{
				return this._parameterizedCreator;
			}
		}

		[Obsolete("ParametrizedConstructor is obsolete. Use OverrideCreator instead.")]
		public ConstructorInfo ParametrizedConstructor
		{
			get
			{
				return this._parametrizedConstructor;
			}
			set
			{
				ObjectConstructor<object> objectConstructor;
				this._parametrizedConstructor = value;
				if (value != null)
				{
					objectConstructor = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(value);
				}
				else
				{
					objectConstructor = null;
				}
				this._parameterizedCreator = objectConstructor;
			}
		}

		public JsonPropertyCollection Properties
		{
			get;
			private set;
		}

		public JsonObjectContract(Type underlyingType) : base(underlyingType)
		{
			this.ContractType = JsonContractType.Object;
			this.Properties = new JsonPropertyCollection(base.UnderlyingType);
		}

		internal object GetUninitializedObject()
		{
			if (!JsonTypeReflector.FullyTrusted)
			{
				throw new JsonException("Insufficient permissions. Creating an uninitialized '{0}' type requires full trust.".FormatWith(CultureInfo.InvariantCulture, this.NonNullableUnderlyingType));
			}
			return FormatterServices.GetUninitializedObject(this.NonNullableUnderlyingType);
		}
	}
}