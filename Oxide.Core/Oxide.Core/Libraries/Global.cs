using Oxide.Core;
using System;

namespace Oxide.Core.Libraries
{
	public class Global : Library
	{
		public override bool IsGlobal
		{
			get
			{
				return true;
			}
		}

		public Global()
		{
		}

		[LibraryFunction("V")]
		public VersionNumber MakeVersion(ushort major, ushort minor, ushort patch)
		{
			return new VersionNumber((int)major, (int)minor, (int)patch);
		}

		[LibraryFunction("new")]
		public object New(Type type, object[] args)
		{
			if (args == null)
			{
				return Activator.CreateInstance(type);
			}
			return Activator.CreateInstance(type, args);
		}
	}
}