using ProtoBuf;
using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ProtoBuf.Meta
{
	public class CallbackSet
	{
		private readonly MetaType metaType;

		private MethodInfo beforeSerialize;

		private MethodInfo afterSerialize;

		private MethodInfo beforeDeserialize;

		private MethodInfo afterDeserialize;

		public MethodInfo AfterDeserialize
		{
			get
			{
				return this.afterDeserialize;
			}
			set
			{
				this.afterDeserialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		public MethodInfo AfterSerialize
		{
			get
			{
				return this.afterSerialize;
			}
			set
			{
				this.afterSerialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		public MethodInfo BeforeDeserialize
		{
			get
			{
				return this.beforeDeserialize;
			}
			set
			{
				this.beforeDeserialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		public MethodInfo BeforeSerialize
		{
			get
			{
				return this.beforeSerialize;
			}
			set
			{
				this.beforeSerialize = this.SanityCheckCallback(this.metaType.Model, value);
			}
		}

		internal MethodInfo this[TypeModel.CallbackType callbackType]
		{
			get
			{
				switch (callbackType)
				{
					case TypeModel.CallbackType.BeforeSerialize:
					{
						return this.beforeSerialize;
					}
					case TypeModel.CallbackType.AfterSerialize:
					{
						return this.afterSerialize;
					}
					case TypeModel.CallbackType.BeforeDeserialize:
					{
						return this.beforeDeserialize;
					}
					case TypeModel.CallbackType.AfterDeserialize:
					{
						return this.afterDeserialize;
					}
				}
				throw new ArgumentException(string.Concat("Callback type not supported: ", callbackType.ToString()), "callbackType");
			}
		}

		public bool NonTrivial
		{
			get
			{
				if (this.beforeSerialize != null || this.beforeDeserialize != null || this.afterSerialize != null)
				{
					return true;
				}
				return this.afterDeserialize != null;
			}
		}

		internal CallbackSet(MetaType metaType)
		{
			if (metaType == null)
			{
				throw new ArgumentNullException("metaType");
			}
			this.metaType = metaType;
		}

		internal static bool CheckCallbackParameters(TypeModel model, MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			for (int i = 0; i < (int)parameters.Length; i++)
			{
				Type parameterType = parameters[i].ParameterType;
				if (parameterType != model.MapType(typeof(SerializationContext)) && parameterType != model.MapType(typeof(Type)) && parameterType != model.MapType(typeof(StreamingContext)))
				{
					return false;
				}
			}
			return true;
		}

		internal static Exception CreateInvalidCallbackSignature(MethodInfo method)
		{
			return new NotSupportedException(string.Concat("Invalid callback signature in ", method.DeclaringType.FullName, ".", method.Name));
		}

		private MethodInfo SanityCheckCallback(TypeModel model, MethodInfo callback)
		{
			this.metaType.ThrowIfFrozen();
			if (callback == null)
			{
				return callback;
			}
			if (callback.IsStatic)
			{
				throw new ArgumentException("Callbacks cannot be static", "callback");
			}
			if (callback.ReturnType != model.MapType(typeof(void)) || !CallbackSet.CheckCallbackParameters(model, callback))
			{
				throw CallbackSet.CreateInvalidCallbackSignature(callback);
			}
			return callback;
		}
	}
}