using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace ObjectStream.IO
{
	public class BindChanger : SerializationBinder
	{
		public BindChanger()
		{
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			return Type.GetType(string.Format("{0}, {1}", typeName, Assembly.GetExecutingAssembly().FullName));
		}
	}
}