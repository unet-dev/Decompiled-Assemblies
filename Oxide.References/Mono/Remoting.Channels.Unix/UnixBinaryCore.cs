using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Mono.Remoting.Channels.Unix
{
	internal class UnixBinaryCore
	{
		private BinaryFormatter _serializationFormatter;

		private BinaryFormatter _deserializationFormatter;

		private bool _includeVersions = true;

		private bool _strictBinding;

		private IDictionary _properties;

		public static UnixBinaryCore DefaultInstance;

		public BinaryFormatter Deserializer
		{
			get
			{
				return this._deserializationFormatter;
			}
		}

		public IDictionary Properties
		{
			get
			{
				return this._properties;
			}
		}

		public BinaryFormatter Serializer
		{
			get
			{
				return this._serializationFormatter;
			}
		}

		static UnixBinaryCore()
		{
			UnixBinaryCore.DefaultInstance = new UnixBinaryCore();
		}

		public UnixBinaryCore(object owner, IDictionary properties, string[] allowedProperties)
		{
			int num;
			this._properties = properties;
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry current = (DictionaryEntry)enumerator.Current;
					string key = (string)current.Key;
					if (Array.IndexOf<string>(allowedProperties, key) == -1)
					{
						throw new RemotingException(string.Concat(owner.GetType().Name, " does not recognize '", key, "' configuration property"));
					}
					string str = key;
					if (str == null)
					{
						continue;
					}
					if (UnixBinaryCore.<>f__switch$map0 == null)
					{
						Dictionary<string, int> strs = new Dictionary<string, int>(2)
						{
							{ "includeVersions", 0 },
							{ "strictBinding", 1 }
						};
						UnixBinaryCore.<>f__switch$map0 = strs;
					}
					if (!UnixBinaryCore.<>f__switch$map0.TryGetValue(str, out num))
					{
						continue;
					}
					if (num == 0)
					{
						this._includeVersions = Convert.ToBoolean(current.Value);
					}
					else if (num == 1)
					{
						this._strictBinding = Convert.ToBoolean(current.Value);
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
			this.Init();
		}

		public UnixBinaryCore()
		{
			this._properties = new Hashtable();
			this.Init();
		}

		public void Init()
		{
			RemotingSurrogateSelector remotingSurrogateSelector = new RemotingSurrogateSelector();
			StreamingContext streamingContext = new StreamingContext(StreamingContextStates.Remoting, null);
			this._serializationFormatter = new BinaryFormatter(remotingSurrogateSelector, streamingContext);
			this._deserializationFormatter = new BinaryFormatter(null, streamingContext);
			if (!this._includeVersions)
			{
				this._serializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
				this._deserializationFormatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
			}
			if (!this._strictBinding)
			{
				this._serializationFormatter.Binder = SimpleBinder.Instance;
				this._deserializationFormatter.Binder = SimpleBinder.Instance;
			}
		}
	}
}