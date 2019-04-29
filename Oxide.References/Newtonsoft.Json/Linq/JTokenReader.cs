using Newtonsoft.Json;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public class JTokenReader : JsonReader, IJsonLineInfo
	{
		private readonly JToken _root;

		private string _initialPath;

		private JToken _parent;

		private JToken _current;

		public JToken CurrentToken
		{
			get
			{
				return this._current;
			}
		}

		int Newtonsoft.Json.IJsonLineInfo.LineNumber
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start)
				{
					return 0;
				}
				IJsonLineInfo jsonLineInfo = this._current;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LineNumber;
			}
		}

		int Newtonsoft.Json.IJsonLineInfo.LinePosition
		{
			get
			{
				if (base.CurrentState == JsonReader.State.Start)
				{
					return 0;
				}
				IJsonLineInfo jsonLineInfo = this._current;
				if (jsonLineInfo == null)
				{
					return 0;
				}
				return jsonLineInfo.LinePosition;
			}
		}

		public override string Path
		{
			get
			{
				string path = base.Path;
				if (this._initialPath == null)
				{
					this._initialPath = this._root.Path;
				}
				if (!string.IsNullOrEmpty(this._initialPath))
				{
					if (string.IsNullOrEmpty(path))
					{
						return this._initialPath;
					}
					path = (!path.StartsWith('[') ? string.Concat(this._initialPath, ".", path) : string.Concat(this._initialPath, path));
				}
				return path;
			}
		}

		public JTokenReader(JToken token)
		{
			ValidationUtils.ArgumentNotNull(token, "token");
			this._root = token;
		}

		internal JTokenReader(JToken token, string initialPath) : this(token)
		{
			this._initialPath = initialPath;
		}

		private JsonToken? GetEndToken(JContainer c)
		{
			switch (c.Type)
			{
				case JTokenType.Object:
				{
					return new JsonToken?(JsonToken.EndObject);
				}
				case JTokenType.Array:
				{
					return new JsonToken?(JsonToken.EndArray);
				}
				case JTokenType.Constructor:
				{
					return new JsonToken?(JsonToken.EndConstructor);
				}
				case JTokenType.Property:
				{
					return null;
				}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", c.Type, "Unexpected JContainer type.");
		}

		bool Newtonsoft.Json.IJsonLineInfo.HasLineInfo()
		{
			if (base.CurrentState == JsonReader.State.Start)
			{
				return false;
			}
			IJsonLineInfo jsonLineInfo = this._current;
			if (jsonLineInfo == null)
			{
				return false;
			}
			return jsonLineInfo.HasLineInfo();
		}

		public override bool Read()
		{
			if (base.CurrentState == JsonReader.State.Start)
			{
				this._current = this._root;
				this.SetToken(this._current);
				return true;
			}
			if (this._current == null)
			{
				return false;
			}
			JContainer jContainers = this._current as JContainer;
			if (jContainers != null && this._parent != jContainers)
			{
				return this.ReadInto(jContainers);
			}
			return this.ReadOver(this._current);
		}

		private bool ReadInto(JContainer c)
		{
			JToken first = c.First;
			if (first == null)
			{
				return this.SetEnd(c);
			}
			this.SetToken(first);
			this._current = first;
			this._parent = c;
			return true;
		}

		private bool ReadOver(JToken t)
		{
			if (t == this._root)
			{
				return this.ReadToEnd();
			}
			JToken next = t.Next;
			if (next != null && next != t && t != t.Parent.Last)
			{
				this._current = next;
				this.SetToken(this._current);
				return true;
			}
			if (t.Parent == null)
			{
				return this.ReadToEnd();
			}
			return this.SetEnd(t.Parent);
		}

		private bool ReadToEnd()
		{
			this._current = null;
			base.SetToken(JsonToken.None);
			return false;
		}

		private string SafeToString(object value)
		{
			if (value == null)
			{
				return null;
			}
			return value.ToString();
		}

		private bool SetEnd(JContainer c)
		{
			JsonToken? endToken = this.GetEndToken(c);
			if (!endToken.HasValue)
			{
				return this.ReadOver(c);
			}
			base.SetToken(endToken.GetValueOrDefault());
			this._current = c;
			this._parent = c;
			return true;
		}

		private void SetToken(JToken token)
		{
			switch (token.Type)
			{
				case JTokenType.Object:
				{
					base.SetToken(JsonToken.StartObject);
					return;
				}
				case JTokenType.Array:
				{
					base.SetToken(JsonToken.StartArray);
					return;
				}
				case JTokenType.Constructor:
				{
					base.SetToken(JsonToken.StartConstructor, ((JConstructor)token).Name);
					return;
				}
				case JTokenType.Property:
				{
					base.SetToken(JsonToken.PropertyName, ((JProperty)token).Name);
					return;
				}
				case JTokenType.Comment:
				{
					base.SetToken(JsonToken.Comment, ((JValue)token).Value);
					return;
				}
				case JTokenType.Integer:
				{
					base.SetToken(JsonToken.Integer, ((JValue)token).Value);
					return;
				}
				case JTokenType.Float:
				{
					base.SetToken(JsonToken.Float, ((JValue)token).Value);
					return;
				}
				case JTokenType.String:
				{
					base.SetToken(JsonToken.String, ((JValue)token).Value);
					return;
				}
				case JTokenType.Boolean:
				{
					base.SetToken(JsonToken.Boolean, ((JValue)token).Value);
					return;
				}
				case JTokenType.Null:
				{
					base.SetToken(JsonToken.Null, ((JValue)token).Value);
					return;
				}
				case JTokenType.Undefined:
				{
					base.SetToken(JsonToken.Undefined, ((JValue)token).Value);
					return;
				}
				case JTokenType.Date:
				{
					base.SetToken(JsonToken.Date, ((JValue)token).Value);
					return;
				}
				case JTokenType.Raw:
				{
					base.SetToken(JsonToken.Raw, ((JValue)token).Value);
					return;
				}
				case JTokenType.Bytes:
				{
					base.SetToken(JsonToken.Bytes, ((JValue)token).Value);
					return;
				}
				case JTokenType.Guid:
				{
					base.SetToken(JsonToken.String, this.SafeToString(((JValue)token).Value));
					return;
				}
				case JTokenType.Uri:
				{
					object value = ((JValue)token).Value;
					if (value is Uri)
					{
						base.SetToken(JsonToken.String, ((Uri)value).OriginalString);
						return;
					}
					base.SetToken(JsonToken.String, this.SafeToString(value));
					return;
				}
				case JTokenType.TimeSpan:
				{
					base.SetToken(JsonToken.String, this.SafeToString(((JValue)token).Value));
					return;
				}
			}
			throw MiscellaneousUtils.CreateArgumentOutOfRangeException("Type", token.Type, "Unexpected JTokenType.");
		}
	}
}