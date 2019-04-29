using System;
using System.Runtime.Serialization;

namespace Mono.Cecil
{
	[Serializable]
	public class ResolutionException : Exception
	{
		private readonly MemberReference member;

		public MemberReference Member
		{
			get
			{
				return this.member;
			}
		}

		public IMetadataScope Scope
		{
			get
			{
				TypeReference typeReference = this.member as TypeReference;
				if (typeReference != null)
				{
					return typeReference.Scope;
				}
				TypeReference declaringType = this.member.DeclaringType;
				if (declaringType == null)
				{
					throw new NotSupportedException();
				}
				return declaringType.Scope;
			}
		}

		public ResolutionException(MemberReference member) : base(string.Concat("Failed to resolve ", member.FullName))
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}
			this.member = member;
		}

		protected ResolutionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}