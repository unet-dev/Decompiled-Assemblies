using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Mono.Remoting.Channels.Unix
{
	internal class SimpleBinder : SerializationBinder
	{
		public static SimpleBinder Instance;

		static SimpleBinder()
		{
			SimpleBinder.Instance = new SimpleBinder();
		}

		public SimpleBinder()
		{
		}

		public override Type BindToType(string assemblyName, string typeName)
		{
			Assembly assembly;
			Type type;
			if (assemblyName.IndexOf(',') != -1)
			{
				try
				{
					assembly = Assembly.Load(assemblyName);
					if (assembly != null)
					{
						Type type1 = assembly.GetType(typeName);
						if (type1 == null)
						{
							assembly = Assembly.LoadWithPartialName(assemblyName);
							if (assembly == null)
							{
								return null;
							}
							return assembly.GetType(typeName, true);
						}
						else
						{
							type = type1;
						}
					}
					else
					{
						type = null;
					}
				}
				catch
				{
					assembly = Assembly.LoadWithPartialName(assemblyName);
					if (assembly == null)
					{
						return null;
					}
					return assembly.GetType(typeName, true);
				}
				return type;
			}
			assembly = Assembly.LoadWithPartialName(assemblyName);
			if (assembly == null)
			{
				return null;
			}
			return assembly.GetType(typeName, true);
		}
	}
}