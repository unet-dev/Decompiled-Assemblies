using Newtonsoft.Json.Shims;
using System;

namespace Newtonsoft.Json.Serialization
{
	[Preserve]
	internal struct ResolverContractKey
	{
		private readonly Type _resolverType;

		private readonly Type _contractType;

		public ResolverContractKey(Type resolverType, Type contractType)
		{
			this._resolverType = resolverType;
			this._contractType = contractType;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ResolverContractKey))
			{
				return false;
			}
			return this.Equals((ResolverContractKey)obj);
		}

		public bool Equals(ResolverContractKey other)
		{
			if (this._resolverType != other._resolverType)
			{
				return false;
			}
			return this._contractType == other._contractType;
		}

		public override int GetHashCode()
		{
			return this._resolverType.GetHashCode() ^ this._contractType.GetHashCode();
		}
	}
}