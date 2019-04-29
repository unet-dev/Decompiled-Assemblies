using Apex;
using System;
using System.Runtime.CompilerServices;

namespace Apex.AI
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple=true, Inherited=true)]
	public sealed class MemberDependencyAttribute : Attribute
	{
		public CompareOperator compare
		{
			get;
			private set;
		}

		public string dependsOn
		{
			get;
			private set;
		}

		public bool isMask
		{
			get;
			private set;
		}

		public MaskMatch match
		{
			get;
			private set;
		}

		public ValueType @value
		{
			get;
			private set;
		}

		public MemberDependencyAttribute(string dependsOn, int value, MaskMatch match)
		{
			this.dependsOn = dependsOn;
			this.@value = value;
			this.match = match;
			this.isMask = true;
		}

		public MemberDependencyAttribute(string dependsOn, int value, CompareOperator compare)
		{
			this.dependsOn = dependsOn;
			this.@value = value;
			this.compare = compare;
		}

		public MemberDependencyAttribute(string dependsOn, float value, CompareOperator compare)
		{
			this.dependsOn = dependsOn;
			this.@value = value;
			this.compare = compare;
		}

		public MemberDependencyAttribute(string dependsOn, bool value)
		{
			this.dependsOn = dependsOn;
			this.@value = value;
			this.compare = CompareOperator.Equals;
		}
	}
}