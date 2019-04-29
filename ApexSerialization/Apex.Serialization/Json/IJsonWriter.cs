using Apex.Serialization;
using System;

namespace Apex.Serialization.Json
{
	internal interface IJsonWriter
	{
		void WriteAttributeLabel(StageAttribute a);

		void WriteElementEnd();

		void WriteElementStart();

		void WriteLabel(StageItem l);

		void WriteListEnd();

		void WriteListStart();

		void WriteNull(StageNull n);

		void WriteSeparator();

		void WriteValue(StageValue v);
	}
}