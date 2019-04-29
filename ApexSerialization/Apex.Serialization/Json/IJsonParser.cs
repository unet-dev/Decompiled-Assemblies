using Apex.Serialization;
using System;

namespace Apex.Serialization.Json
{
	internal interface IJsonParser
	{
		StageElement Parse(string json);
	}
}