using System;

namespace Apex.Serialization
{
	public sealed class StageAttribute : StageValue
	{
		internal StageAttribute(string name, string value, bool isText) : base(name, value, isText)
		{
		}
	}
}