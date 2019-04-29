using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using System;
using System.Globalization;
using System.IO;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JRaw : JValue
	{
		public JRaw(JRaw other) : base(other)
		{
		}

		public JRaw(object rawJson) : base(rawJson, JTokenType.Raw)
		{
		}

		internal override JToken CloneToken()
		{
			return new JRaw(this);
		}

		public static JRaw Create(JsonReader reader)
		{
			JRaw jRaw;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				using (JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter))
				{
					jsonTextWriter.WriteToken(reader);
					jRaw = new JRaw(stringWriter.ToString());
				}
			}
			return jRaw;
		}
	}
}