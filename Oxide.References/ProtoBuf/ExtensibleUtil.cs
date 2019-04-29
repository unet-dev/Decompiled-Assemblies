using ProtoBuf.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace ProtoBuf
{
	internal static class ExtensibleUtil
	{
		internal static void AppendExtendValue(TypeModel model, IExtensible instance, int tag, DataFormat format, object value)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			IExtension extensionObject = instance.GetExtensionObject(true);
			if (extensionObject == null)
			{
				throw new InvalidOperationException("No extension object available; appended data would be lost.");
			}
			bool flag = false;
			Stream stream = extensionObject.BeginAppend();
			try
			{
				using (ProtoWriter protoWriter = new ProtoWriter(stream, model, null))
				{
					model.TrySerializeAuxiliaryType(protoWriter, null, format, tag, value, false);
					protoWriter.Close();
				}
				flag = true;
			}
			finally
			{
				extensionObject.EndAppend(stream, flag);
			}
		}

		internal static IEnumerable<TValue> GetExtendedValues<TValue>(IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			foreach (TValue extendedValue in ExtensibleUtil.GetExtendedValues(RuntimeTypeModel.Default, typeof(TValue), instance, tag, format, singleton, allowDefinedTag))
			{
				yield return extendedValue;
			}
		}

		internal static IEnumerable GetExtendedValues(TypeModel model, Type type, IExtensible instance, int tag, DataFormat format, bool singleton, bool allowDefinedTag)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			if (tag <= 0)
			{
				throw new ArgumentOutOfRangeException("tag");
			}
			IExtension extensionObject = instance.GetExtensionObject(false);
			if (extensionObject != null)
			{
				Stream stream = extensionObject.BeginQuery();
				object obj = null;
				ProtoReader protoReader = null;
				try
				{
					SerializationContext serializationContext = new SerializationContext();
					protoReader = ProtoReader.Create(stream, model, serializationContext, -1);
					while (model.TryDeserializeAuxiliaryType(protoReader, format, tag, type, ref obj, true, false, false, false) && obj != null)
					{
						if (singleton)
						{
							continue;
						}
						yield return obj;
						obj = null;
					}
					if (!singleton || obj == null)
					{
						goto Label0;
					}
					yield return obj;
				}
				finally
				{
					ProtoReader.Recycle(protoReader);
					extensionObject.EndQuery(stream);
				}
			}
		Label0:
			yield break;
		}
	}
}