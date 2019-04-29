using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	public class JsonDictionaryContract : JsonContainerContract
	{
		private readonly Type _genericCollectionDefinitionType;

		private Type _genericWrapperType;

		private ObjectConstructor<object> _genericWrapperCreator;

		private Func<object> _genericTemporaryDictionaryCreator;

		private readonly ConstructorInfo _parameterizedConstructor;

		private ObjectConstructor<object> _overrideCreator;

		private ObjectConstructor<object> _parameterizedCreator;

		public Func<string, string> DictionaryKeyResolver
		{
			get;
			set;
		}

		public Type DictionaryKeyType
		{
			get;
			private set;
		}

		public Type DictionaryValueType
		{
			get;
			private set;
		}

		public bool HasParameterizedCreator
		{
			get;
			set;
		}

		internal bool HasParameterizedCreatorInternal
		{
			get
			{
				if (this.HasParameterizedCreator || this._parameterizedCreator != null)
				{
					return true;
				}
				return this._parameterizedConstructor != null;
			}
		}

		internal JsonContract KeyContract
		{
			get;
			set;
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
			}
		}

		internal ObjectConstructor<object> ParameterizedCreator
		{
			get
			{
				if (this._parameterizedCreator == null)
				{
					this._parameterizedCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(this._parameterizedConstructor);
				}
				return this._parameterizedCreator;
			}
		}

		[Obsolete("PropertyNameResolver is obsolete. Use DictionaryKeyResolver instead.")]
		public Func<string, string> PropertyNameResolver
		{
			get
			{
				return this.DictionaryKeyResolver;
			}
			set
			{
				this.DictionaryKeyResolver = value;
			}
		}

		internal bool ShouldCreateWrapper
		{
			get;
			private set;
		}

		public JsonDictionaryContract(Type underlyingType) : base(underlyingType)
		{
			Type genericArguments;
			Type type;
			Type type1;
			this.ContractType = JsonContractType.Dictionary;
			if (!ReflectionUtils.ImplementsGenericDefinition(underlyingType, typeof(IDictionary<,>), out this._genericCollectionDefinitionType))
			{
				ReflectionUtils.GetDictionaryKeyValueTypes(base.UnderlyingType, out genericArguments, out type);
				if (base.UnderlyingType == typeof(IDictionary))
				{
					base.CreatedType = typeof(Dictionary<object, object>);
				}
			}
			else
			{
				genericArguments = this._genericCollectionDefinitionType.GetGenericArguments()[0];
				type = this._genericCollectionDefinitionType.GetGenericArguments()[1];
				if (ReflectionUtils.IsGenericDefinition(base.UnderlyingType, typeof(IDictionary<,>)))
				{
					base.CreatedType = typeof(Dictionary<,>).MakeGenericType(new Type[] { genericArguments, type });
				}
			}
			if (genericArguments != null && type != null)
			{
				this._parameterizedConstructor = CollectionUtils.ResolveEnumerableCollectionConstructor(base.CreatedType, typeof(KeyValuePair<,>).MakeGenericType(new Type[] { genericArguments, type }), typeof(IDictionary<,>).MakeGenericType(new Type[] { genericArguments, type }));
			}
			this.ShouldCreateWrapper = !typeof(IDictionary).IsAssignableFrom(base.CreatedType);
			this.DictionaryKeyType = genericArguments;
			this.DictionaryValueType = type;
			if (this.DictionaryValueType != null && ReflectionUtils.IsNullableType(this.DictionaryValueType) && ReflectionUtils.InheritsGenericDefinition(base.CreatedType, typeof(Dictionary<,>), out type1))
			{
				this.ShouldCreateWrapper = true;
			}
		}

		internal IDictionary CreateTemporaryDictionary()
		{
			if (this._genericTemporaryDictionaryCreator == null)
			{
				Type type = typeof(Dictionary<,>).MakeGenericType(new Type[] { this.DictionaryKeyType ?? typeof(object), this.DictionaryValueType ?? typeof(object) });
				this._genericTemporaryDictionaryCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateDefaultConstructor<object>(type);
			}
			return (IDictionary)this._genericTemporaryDictionaryCreator();
		}

		internal IWrappedDictionary CreateWrapper(object dictionary)
		{
			if (this._genericWrapperCreator == null)
			{
				this._genericWrapperType = typeof(DictionaryWrapper<,>).MakeGenericType(new Type[] { this.DictionaryKeyType, this.DictionaryValueType });
				ConstructorInfo constructor = this._genericWrapperType.GetConstructor(new Type[] { this._genericCollectionDefinitionType });
				this._genericWrapperCreator = JsonTypeReflector.ReflectionDelegateFactory.CreateParameterizedConstructor(constructor);
			}
			return (IWrappedDictionary)this._genericWrapperCreator(new object[] { dictionary });
		}
	}
}