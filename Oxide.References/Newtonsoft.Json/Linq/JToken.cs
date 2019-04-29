using Newtonsoft.Json;
using Newtonsoft.Json.Linq.JsonPath;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Newtonsoft.Json.Linq
{
	[Preserve]
	public abstract class JToken : IJEnumerable<JToken>, IEnumerable<JToken>, IEnumerable, IJsonLineInfo, ICloneable
	{
		private static JTokenEqualityComparer _equalityComparer;

		private JContainer _parent;

		private JToken _previous;

		private JToken _next;

		private object _annotations;

		private readonly static JTokenType[] BooleanTypes;

		private readonly static JTokenType[] NumberTypes;

		private readonly static JTokenType[] StringTypes;

		private readonly static JTokenType[] GuidTypes;

		private readonly static JTokenType[] TimeSpanTypes;

		private readonly static JTokenType[] UriTypes;

		private readonly static JTokenType[] CharTypes;

		private readonly static JTokenType[] DateTimeTypes;

		private readonly static JTokenType[] BytesTypes;

		public static JTokenEqualityComparer EqualityComparer
		{
			get
			{
				if (JToken._equalityComparer == null)
				{
					JToken._equalityComparer = new JTokenEqualityComparer();
				}
				return JToken._equalityComparer;
			}
		}

		public virtual JToken First
		{
			get
			{
				throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, this.GetType()));
			}
		}

		public abstract bool HasValues
		{
			get;
		}

		public virtual JToken this[object key]
		{
			get
			{
				throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, this.GetType()));
			}
			set
			{
				throw new InvalidOperationException("Cannot set child value on {0}.".FormatWith(CultureInfo.InvariantCulture, this.GetType()));
			}
		}

		public virtual JToken Last
		{
			get
			{
				throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, this.GetType()));
			}
		}

		int Newtonsoft.Json.IJsonLineInfo.LineNumber
		{
			get
			{
				JToken.LineInfoAnnotation lineInfoAnnotation = this.Annotation<JToken.LineInfoAnnotation>();
				if (lineInfoAnnotation == null)
				{
					return 0;
				}
				return lineInfoAnnotation.LineNumber;
			}
		}

		int Newtonsoft.Json.IJsonLineInfo.LinePosition
		{
			get
			{
				JToken.LineInfoAnnotation lineInfoAnnotation = this.Annotation<JToken.LineInfoAnnotation>();
				if (lineInfoAnnotation == null)
				{
					return 0;
				}
				return lineInfoAnnotation.LinePosition;
			}
		}

		IJEnumerable<JToken> Newtonsoft.Json.Linq.IJEnumerable<Newtonsoft.Json.Linq.JToken>.this[object key]
		{
			get
			{
				return this[key];
			}
		}

		public JToken Next
		{
			get
			{
				return this._next;
			}
			internal set
			{
				this._next = value;
			}
		}

		public JContainer Parent
		{
			[DebuggerStepThrough]
			get
			{
				return this._parent;
			}
			internal set
			{
				this._parent = value;
			}
		}

		public string Path
		{
			get
			{
				JsonPosition jsonPosition;
				if (this.Parent == null)
				{
					return string.Empty;
				}
				List<JsonPosition> jsonPositions = new List<JsonPosition>();
				JToken jTokens = null;
				for (JToken i = this; i != null; i = i.Parent)
				{
					switch (i.Type)
					{
						case JTokenType.Array:
						case JTokenType.Constructor:
						{
							if (jTokens == null)
							{
								break;
							}
							int num = ((IList<JToken>)i).IndexOf(jTokens);
							jsonPosition = new JsonPosition(JsonContainerType.Array)
							{
								Position = num
							};
							jsonPositions.Add(jsonPosition);
							break;
						}
						case JTokenType.Property:
						{
							JProperty jProperty = (JProperty)i;
							jsonPosition = new JsonPosition(JsonContainerType.Object)
							{
								PropertyName = jProperty.Name
							};
							jsonPositions.Add(jsonPosition);
							break;
						}
					}
					jTokens = i;
				}
				jsonPositions.Reverse();
				return JsonPosition.BuildPath(jsonPositions, null);
			}
		}

		public JToken Previous
		{
			get
			{
				return this._previous;
			}
			internal set
			{
				this._previous = value;
			}
		}

		public JToken Root
		{
			get
			{
				JContainer parent = this.Parent;
				if (parent == null)
				{
					return this;
				}
				while (parent.Parent != null)
				{
					parent = parent.Parent;
				}
				return parent;
			}
		}

		public abstract JTokenType Type
		{
			get;
		}

		static JToken()
		{
			JToken.BooleanTypes = new JTokenType[] { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean };
			JToken.NumberTypes = new JTokenType[] { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean };
			JToken.StringTypes = new JTokenType[] { JTokenType.Date, JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Boolean, JTokenType.Bytes, JTokenType.Guid, JTokenType.TimeSpan, JTokenType.Uri };
			JToken.GuidTypes = new JTokenType[] { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Guid, JTokenType.Bytes };
			JToken.TimeSpanTypes = new JTokenType[] { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.TimeSpan };
			JToken.UriTypes = new JTokenType[] { JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Uri };
			JToken.CharTypes = new JTokenType[] { JTokenType.Integer, JTokenType.Float, JTokenType.String, JTokenType.Comment, JTokenType.Raw };
			JToken.DateTimeTypes = new JTokenType[] { JTokenType.Date, JTokenType.String, JTokenType.Comment, JTokenType.Raw };
			JToken.BytesTypes = new JTokenType[] { JTokenType.Bytes, JTokenType.String, JTokenType.Comment, JTokenType.Raw, JTokenType.Integer };
		}

		internal JToken()
		{
		}

		public void AddAfterSelf(object content)
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			int num = this._parent.IndexOfItem(this);
			this._parent.AddInternal(num + 1, content, false);
		}

		public void AddAnnotation(object annotation)
		{
			// 
			// Current member / type: System.Void Newtonsoft.Json.Linq.JToken::AddAnnotation(System.Object)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.References.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void AddAnnotation(System.Object)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:line 214
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.set_Left(Expression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\Expressions\BinaryExpression.cs:line 241
			//    at ..(BinaryExpression ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:line 74
			//    at ..(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 97
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeTransformer.cs:line 276
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\FixBinaryExpressionsStep.cs:line 44
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\ExpressionDecompilerStep.cs:line 91
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		public void AddBeforeSelf(object content)
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			int num = this._parent.IndexOfItem(this);
			this._parent.AddInternal(num, content, false);
		}

		public IEnumerable<JToken> AfterSelf()
		{
			JToken i;
			if (this.Parent == null)
			{
				yield break;
			}
			for (i = this.Next; i != null; i = i.Next)
			{
				yield return i;
			}
			i = null;
		}

		public IEnumerable<JToken> Ancestors()
		{
			return this.GetAncestors(false);
		}

		public IEnumerable<JToken> AncestorsAndSelf()
		{
			return this.GetAncestors(true);
		}

		public T Annotation<T>()
		where T : class
		{
			if (this._annotations != null)
			{
				object[] objArray = this._annotations as object[];
				if (objArray == null)
				{
					return (T)(this._annotations as T);
				}
				for (int i = 0; i < (int)objArray.Length; i++)
				{
					object obj = objArray[i];
					if (obj == null)
					{
						break;
					}
					T t = (T)(obj as T);
					if (t != null)
					{
						return t;
					}
				}
			}
			return default(T);
		}

		public object Annotation(System.Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this._annotations != null)
			{
				object[] objArray = this._annotations as object[];
				if (objArray != null)
				{
					for (int i = 0; i < (int)objArray.Length; i++)
					{
						object obj = objArray[i];
						if (obj == null)
						{
							break;
						}
						if (type.IsInstanceOfType(obj))
						{
							return obj;
						}
					}
				}
				else if (type.IsInstanceOfType(this._annotations))
				{
					return this._annotations;
				}
			}
			return null;
		}

		public IEnumerable<T> Annotations<T>()
		where T : class
		{
			if (this._annotations == null)
			{
				yield break;
			}
			object[] objArray = this._annotations as object[];
			if (objArray == null)
			{
				T t = (T)(this._annotations as T);
				if (t == null)
				{
					yield break;
				}
				yield return t;
				yield break;
			}
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				object obj = objArray[i];
				if (obj == null)
				{
					break;
				}
				T t1 = (T)(obj as T);
				if (t1 != null)
				{
					yield return t1;
				}
			}
		}

		public IEnumerable<object> Annotations(System.Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this._annotations == null)
			{
				yield break;
			}
			object[] objArray = this._annotations as object[];
			if (objArray == null)
			{
				if (!type.IsInstanceOfType(this._annotations))
				{
					yield break;
				}
				yield return this._annotations;
				yield break;
			}
			for (int i = 0; i < (int)objArray.Length; i++)
			{
				object obj = objArray[i];
				if (obj == null)
				{
					break;
				}
				if (type.IsInstanceOfType(obj))
				{
					yield return obj;
				}
			}
		}

		public IEnumerable<JToken> BeforeSelf()
		{
			JToken i;
			for (i = this.Parent.First; i != this; i = i.Next)
			{
				yield return i;
			}
			i = null;
		}

		public virtual JEnumerable<JToken> Children()
		{
			return JEnumerable<JToken>.Empty;
		}

		public JEnumerable<T> Children<T>()
		where T : JToken
		{
			return new JEnumerable<T>(this.Children().OfType<T>());
		}

		internal abstract JToken CloneToken();

		public JsonReader CreateReader()
		{
			return new JTokenReader(this);
		}

		public JToken DeepClone()
		{
			return this.CloneToken();
		}

		internal abstract bool DeepEquals(JToken node);

		public static bool DeepEquals(JToken t1, JToken t2)
		{
			if (t1 == t2)
			{
				return true;
			}
			if (t1 == null || t2 == null)
			{
				return false;
			}
			return t1.DeepEquals(t2);
		}

		private static JValue EnsureValue(JToken value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (value is JProperty)
			{
				value = ((JProperty)value).Value;
			}
			return value as JValue;
		}

		public static JToken FromObject(object o)
		{
			return JToken.FromObjectInternal(o, JsonSerializer.CreateDefault());
		}

		public static JToken FromObject(object o, JsonSerializer jsonSerializer)
		{
			return JToken.FromObjectInternal(o, jsonSerializer);
		}

		internal static JToken FromObjectInternal(object o, JsonSerializer jsonSerializer)
		{
			JToken token;
			ValidationUtils.ArgumentNotNull(o, "o");
			ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
			using (JTokenWriter jTokenWriter = new JTokenWriter())
			{
				jsonSerializer.Serialize(jTokenWriter, o);
				token = jTokenWriter.Token;
			}
			return token;
		}

		internal IEnumerable<JToken> GetAncestors(bool self)
		{
			JToken parent;
			JToken i;
			if (self)
			{
				parent = this;
			}
			else
			{
				parent = this.Parent;
			}
			for (i = parent; i != null; i = i.Parent)
			{
				yield return i;
			}
			i = null;
		}

		internal abstract int GetDeepHashCode();

		private static string GetType(JToken token)
		{
			ValidationUtils.ArgumentNotNull(token, "token");
			if (token is JProperty)
			{
				token = ((JProperty)token).Value;
			}
			return token.Type.ToString();
		}

		public static JToken Load(JsonReader reader, JsonLoadSettings settings)
		{
			return JToken.ReadFrom(reader, settings);
		}

		public static JToken Load(JsonReader reader)
		{
			return JToken.Load(reader, null);
		}

		bool Newtonsoft.Json.IJsonLineInfo.HasLineInfo()
		{
			return this.Annotation<JToken.LineInfoAnnotation>() != null;
		}

		public static explicit operator Boolean(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.BooleanTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToBoolean(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator DateTimeOffset(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.DateTimeTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value is DateTimeOffset)
			{
				return (DateTimeOffset)jValue.Value;
			}
			if (jValue.Value is string)
			{
				return DateTimeOffset.Parse((string)jValue.Value, CultureInfo.InvariantCulture);
			}
			return new DateTimeOffset(Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<Boolean>(JToken value)
		{
			bool? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.BooleanTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Boolean.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new bool?(Convert.ToBoolean(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Int64(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToInt64(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator Nullable<DateTime>(JToken value)
		{
			DateTime? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.DateTimeTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value is DateTimeOffset)
			{
				return new DateTime?(((DateTimeOffset)jValue.Value).DateTime);
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new DateTime?(Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<DateTimeOffset>(JToken value)
		{
			DateTimeOffset? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.DateTimeTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to DateTimeOffset.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			if (jValue.Value is DateTimeOffset)
			{
				return (DateTimeOffset?)jValue.Value;
			}
			if (jValue.Value is string)
			{
				return new DateTimeOffset?(DateTimeOffset.Parse((string)jValue.Value, CultureInfo.InvariantCulture));
			}
			return new DateTimeOffset?(new DateTimeOffset(Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture)));
		}

		public static explicit operator Nullable<Decimal>(JToken value)
		{
			decimal? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new decimal?(Convert.ToDecimal(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<Double>(JToken value)
		{
			double? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new double?(Convert.ToDouble(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<Char>(JToken value)
		{
			char? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.CharTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new char?(Convert.ToChar(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Int32(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToInt32(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator Int16(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToInt16(jValue.Value, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static explicit operator UInt16(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToUInt16(jValue.Value, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static explicit operator Char(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.CharTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Char.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToChar(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator Byte(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToByte(jValue.Value, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static explicit operator SByte(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToSByte(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator Nullable<Int32>(JToken value)
		{
			int? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Int32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new int?(Convert.ToInt32(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<Int16>(JToken value)
		{
			short? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Int16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new short?(Convert.ToInt16(jValue.Value, CultureInfo.InvariantCulture));
		}

		[CLSCompliant(false)]
		public static explicit operator Nullable<UInt16>(JToken value)
		{
			ushort? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to UInt16.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new ushort?(Convert.ToUInt16(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<Byte>(JToken value)
		{
			byte? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Byte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new byte?(Convert.ToByte(jValue.Value, CultureInfo.InvariantCulture));
		}

		[CLSCompliant(false)]
		public static explicit operator Nullable<SByte>(JToken value)
		{
			sbyte? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to SByte.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new sbyte?(Convert.ToSByte(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator DateTime(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.DateTimeTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to DateTime.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (!(jValue.Value is DateTimeOffset))
			{
				return Convert.ToDateTime(jValue.Value, CultureInfo.InvariantCulture);
			}
			return ((DateTimeOffset)jValue.Value).DateTime;
		}

		public static explicit operator Nullable<Int64>(JToken value)
		{
			long? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Int64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new long?(Convert.ToInt64(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<Single>(JToken value)
		{
			float? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new float?(Convert.ToSingle(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Decimal(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Decimal.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToDecimal(jValue.Value, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static explicit operator Nullable<UInt32>(JToken value)
		{
			uint? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new uint?(Convert.ToUInt32(jValue.Value, CultureInfo.InvariantCulture));
		}

		[CLSCompliant(false)]
		public static explicit operator Nullable<UInt64>(JToken value)
		{
			ulong? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new ulong?(Convert.ToUInt64(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Double(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Double.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToDouble(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator Single(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Single.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToSingle(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator String(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.StringTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to String.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				return null;
			}
			if (jValue.Value is byte[])
			{
				return Convert.ToBase64String((byte[])jValue.Value);
			}
			return Convert.ToString(jValue.Value, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static explicit operator UInt32(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to UInt32.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToUInt32(jValue.Value, CultureInfo.InvariantCulture);
		}

		[CLSCompliant(false)]
		public static explicit operator UInt64(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.NumberTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to UInt64.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return Convert.ToUInt64(jValue.Value, CultureInfo.InvariantCulture);
		}

		public static explicit operator Byte[](JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.BytesTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value is string)
			{
				return Convert.FromBase64String(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
			}
			if (!(jValue.Value is byte[]))
			{
				throw new ArgumentException("Can not convert {0} to byte array.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			return (byte[])jValue.Value;
		}

		public static explicit operator Guid(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.GuidTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value is byte[])
			{
				return new Guid((byte[])jValue.Value);
			}
			if (jValue.Value is Guid)
			{
				return (Guid)jValue.Value;
			}
			return new Guid(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<Guid>(JToken value)
		{
			Guid? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.GuidTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Guid.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			if (jValue.Value is byte[])
			{
				return new Guid?(new Guid((byte[])jValue.Value));
			}
			return new Guid?((jValue.Value is Guid ? (Guid)jValue.Value : new Guid(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture))));
		}

		public static explicit operator TimeSpan(JToken value)
		{
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.TimeSpanTypes, false))
			{
				throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value is TimeSpan)
			{
				return (TimeSpan)jValue.Value;
			}
			return ConvertUtils.ParseTimeSpan(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static explicit operator Nullable<TimeSpan>(JToken value)
		{
			TimeSpan? nullable;
			if (value == null)
			{
				nullable = null;
				return nullable;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.TimeSpanTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to TimeSpan.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				nullable = null;
				return nullable;
			}
			return new TimeSpan?((jValue.Value is TimeSpan ? (TimeSpan)jValue.Value : ConvertUtils.ParseTimeSpan(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture))));
		}

		public static explicit operator Uri(JToken value)
		{
			if (value == null)
			{
				return null;
			}
			JValue jValue = JToken.EnsureValue(value);
			if (jValue == null || !JToken.ValidateToken(jValue, JToken.UriTypes, true))
			{
				throw new ArgumentException("Can not convert {0} to Uri.".FormatWith(CultureInfo.InvariantCulture, JToken.GetType(value)));
			}
			if (jValue.Value == null)
			{
				return null;
			}
			if (jValue.Value is Uri)
			{
				return (Uri)jValue.Value;
			}
			return new Uri(Convert.ToString(jValue.Value, CultureInfo.InvariantCulture));
		}

		public static implicit operator JToken(bool value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(DateTimeOffset value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(byte value)
		{
			return new JValue((long)((ulong)value));
		}

		public static implicit operator JToken(byte? value)
		{
			return new JValue((object)value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(sbyte value)
		{
			return new JValue((long)value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(sbyte? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(bool? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(long value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(DateTime? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(DateTimeOffset? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(decimal? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(double? value)
		{
			return new JValue((object)value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(short value)
		{
			return new JValue((long)value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(ushort value)
		{
			return new JValue((long)((ulong)value));
		}

		public static implicit operator JToken(int value)
		{
			return new JValue((long)value);
		}

		public static implicit operator JToken(int? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(DateTime value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(long? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(float? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(decimal value)
		{
			return new JValue(value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(short? value)
		{
			return new JValue((object)value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(ushort? value)
		{
			return new JValue((object)value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(uint? value)
		{
			return new JValue((object)value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(ulong? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(double value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(float value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(string value)
		{
			return new JValue(value);
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(uint value)
		{
			return new JValue((long)((ulong)value));
		}

		[CLSCompliant(false)]
		public static implicit operator JToken(ulong value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(byte[] value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(Uri value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(TimeSpan value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(TimeSpan? value)
		{
			return new JValue((object)value);
		}

		public static implicit operator JToken(Guid value)
		{
			return new JValue(value);
		}

		public static implicit operator JToken(Guid? value)
		{
			return new JValue((object)value);
		}

		public static JToken Parse(string json)
		{
			return JToken.Parse(json, null);
		}

		public static JToken Parse(string json, JsonLoadSettings settings)
		{
			JToken jTokens;
			using (JsonReader jsonTextReader = new JsonTextReader(new StringReader(json)))
			{
				JToken jTokens1 = JToken.Load(jsonTextReader, settings);
				if (jsonTextReader.Read() && jsonTextReader.TokenType != JsonToken.Comment)
				{
					throw JsonReaderException.Create(jsonTextReader, "Additional text found in JSON string after parsing content.");
				}
				jTokens = jTokens1;
			}
			return jTokens;
		}

		public static JToken ReadFrom(JsonReader reader)
		{
			return JToken.ReadFrom(reader, null);
		}

		public static JToken ReadFrom(JsonReader reader, JsonLoadSettings settings)
		{
			ValidationUtils.ArgumentNotNull(reader, "reader");
			if (reader.TokenType == JsonToken.None)
			{
				if ((settings == null || settings.CommentHandling != CommentHandling.Ignore ? !reader.Read() : !reader.ReadAndMoveToContent()))
				{
					throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader.");
				}
			}
			IJsonLineInfo jsonLineInfo = reader as IJsonLineInfo;
			switch (reader.TokenType)
			{
				case JsonToken.StartObject:
				{
					return JObject.Load(reader, settings);
				}
				case JsonToken.StartArray:
				{
					return JArray.Load(reader, settings);
				}
				case JsonToken.StartConstructor:
				{
					return JConstructor.Load(reader, settings);
				}
				case JsonToken.PropertyName:
				{
					return JProperty.Load(reader, settings);
				}
				case JsonToken.Comment:
				{
					JValue jValue = JValue.CreateComment(reader.Value.ToString());
					jValue.SetLineInfo(jsonLineInfo, settings);
					return jValue;
				}
				case JsonToken.Raw:
				case JsonToken.EndObject:
				case JsonToken.EndArray:
				case JsonToken.EndConstructor:
				{
					throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
				case JsonToken.Integer:
				case JsonToken.Float:
				case JsonToken.String:
				case JsonToken.Boolean:
				case JsonToken.Date:
				case JsonToken.Bytes:
				{
					JValue jValue1 = new JValue(reader.Value);
					jValue1.SetLineInfo(jsonLineInfo, settings);
					return jValue1;
				}
				case JsonToken.Null:
				{
					JValue jValue2 = JValue.CreateNull();
					jValue2.SetLineInfo(jsonLineInfo, settings);
					return jValue2;
				}
				case JsonToken.Undefined:
				{
					JValue jValue3 = JValue.CreateUndefined();
					jValue3.SetLineInfo(jsonLineInfo, settings);
					return jValue3;
				}
				default:
				{
					throw JsonReaderException.Create(reader, "Error reading JToken from JsonReader. Unexpected token: {0}".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
				}
			}
		}

		public void Remove()
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			this._parent.RemoveItem(this);
		}

		public void RemoveAnnotations<T>()
		where T : class
		{
			if (this._annotations != null)
			{
				object[] objArray = this._annotations as object[];
				if (objArray != null)
				{
					int num = 0;
					int num1 = 0;
					while (num < (int)objArray.Length)
					{
						object obj = objArray[num];
						if (obj == null)
						{
							break;
						}
						if (!(obj is T))
						{
							int num2 = num1;
							num1 = num2 + 1;
							objArray[num2] = obj;
						}
						num++;
					}
					if (num1 != 0)
					{
						while (num1 < num)
						{
							int num3 = num1;
							num1 = num3 + 1;
							objArray[num3] = null;
						}
						return;
					}
					this._annotations = null;
				}
				else if (this._annotations is T)
				{
					this._annotations = null;
					return;
				}
			}
		}

		public void RemoveAnnotations(System.Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			if (this._annotations != null)
			{
				object[] objArray = this._annotations as object[];
				if (objArray != null)
				{
					int num = 0;
					int num1 = 0;
					while (num < (int)objArray.Length)
					{
						object obj = objArray[num];
						if (obj == null)
						{
							break;
						}
						if (!type.IsInstanceOfType(obj))
						{
							int num2 = num1;
							num1 = num2 + 1;
							objArray[num2] = obj;
						}
						num++;
					}
					if (num1 != 0)
					{
						while (num1 < num)
						{
							int num3 = num1;
							num1 = num3 + 1;
							objArray[num3] = null;
						}
						return;
					}
					this._annotations = null;
				}
				else if (type.IsInstanceOfType(this._annotations))
				{
					this._annotations = null;
					return;
				}
			}
		}

		public void Replace(JToken value)
		{
			if (this._parent == null)
			{
				throw new InvalidOperationException("The parent is missing.");
			}
			this._parent.ReplaceItem(this, value);
		}

		public JToken SelectToken(string path)
		{
			return this.SelectToken(path, false);
		}

		public JToken SelectToken(string path, bool errorWhenNoMatch)
		{
			JToken jTokens = null;
			foreach (JToken jTokens1 in (new JPath(path)).Evaluate(this, errorWhenNoMatch))
			{
				if (jTokens != null)
				{
					throw new JsonException("Path returned multiple tokens.");
				}
				jTokens = jTokens1;
			}
			return jTokens;
		}

		public IEnumerable<JToken> SelectTokens(string path)
		{
			return this.SelectTokens(path, false);
		}

		public IEnumerable<JToken> SelectTokens(string path, bool errorWhenNoMatch)
		{
			return (new JPath(path)).Evaluate(this, errorWhenNoMatch);
		}

		internal void SetLineInfo(IJsonLineInfo lineInfo, JsonLoadSettings settings)
		{
			if (settings != null && settings.LineInfoHandling == LineInfoHandling.Load)
			{
				return;
			}
			if (lineInfo == null || !lineInfo.HasLineInfo())
			{
				return;
			}
			this.SetLineInfo(lineInfo.LineNumber, lineInfo.LinePosition);
		}

		internal void SetLineInfo(int lineNumber, int linePosition)
		{
			this.AddAnnotation(new JToken.LineInfoAnnotation(lineNumber, linePosition));
		}

		IEnumerator<JToken> System.Collections.Generic.IEnumerable<Newtonsoft.Json.Linq.JToken>.GetEnumerator()
		{
			return this.Children().GetEnumerator();
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return ((IEnumerable<JToken>)this).GetEnumerator();
		}

		object System.ICloneable.Clone()
		{
			return this.DeepClone();
		}

		public T ToObject<T>()
		{
			return (T)this.ToObject(typeof(T));
		}

		public object ToObject(System.Type objectType)
		{
			bool flag;
			object obj;
			if (JsonConvert.DefaultSettings == null)
			{
				PrimitiveTypeCode typeCode = ConvertUtils.GetTypeCode(objectType, out flag);
				if (flag)
				{
					if (this.Type == JTokenType.String)
					{
						try
						{
							obj = this.ToObject(objectType, JsonSerializer.CreateDefault());
						}
						catch (Exception exception1)
						{
							Exception exception = exception1;
							System.Type type = (objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType));
							throw new ArgumentException("Could not convert '{0}' to {1}.".FormatWith(CultureInfo.InvariantCulture, (string)this, type.Name), exception);
						}
						return obj;
					}
					if (this.Type == JTokenType.Integer)
					{
						return Enum.ToObject((objectType.IsEnum() ? objectType : Nullable.GetUnderlyingType(objectType)), ((JValue)this).Value);
					}
				}
				switch (typeCode)
				{
					case PrimitiveTypeCode.Char:
					{
						return (char)this;
					}
					case PrimitiveTypeCode.CharNullable:
					{
						return (char?)this;
					}
					case PrimitiveTypeCode.Boolean:
					{
						return (bool)this;
					}
					case PrimitiveTypeCode.BooleanNullable:
					{
						return (bool?)this;
					}
					case PrimitiveTypeCode.SByte:
					{
						return (sbyte?)this;
					}
					case PrimitiveTypeCode.SByteNullable:
					{
						return (sbyte)this;
					}
					case PrimitiveTypeCode.Int16:
					{
						return (short)this;
					}
					case PrimitiveTypeCode.Int16Nullable:
					{
						return (short?)this;
					}
					case PrimitiveTypeCode.UInt16:
					{
						return (ushort)this;
					}
					case PrimitiveTypeCode.UInt16Nullable:
					{
						return (ushort?)this;
					}
					case PrimitiveTypeCode.Int32:
					{
						return (int)this;
					}
					case PrimitiveTypeCode.Int32Nullable:
					{
						return (int?)this;
					}
					case PrimitiveTypeCode.Byte:
					{
						return (byte)this;
					}
					case PrimitiveTypeCode.ByteNullable:
					{
						return (byte?)this;
					}
					case PrimitiveTypeCode.UInt32:
					{
						return (uint)this;
					}
					case PrimitiveTypeCode.UInt32Nullable:
					{
						return (uint?)this;
					}
					case PrimitiveTypeCode.Int64:
					{
						return (long)this;
					}
					case PrimitiveTypeCode.Int64Nullable:
					{
						return (long?)this;
					}
					case PrimitiveTypeCode.UInt64:
					{
						return (ulong)this;
					}
					case PrimitiveTypeCode.UInt64Nullable:
					{
						return (ulong?)this;
					}
					case PrimitiveTypeCode.Single:
					{
						return (float)((float)this);
					}
					case PrimitiveTypeCode.SingleNullable:
					{
						return (float?)this;
					}
					case PrimitiveTypeCode.Double:
					{
						return (double)((double)this);
					}
					case PrimitiveTypeCode.DoubleNullable:
					{
						return (double?)this;
					}
					case PrimitiveTypeCode.DateTime:
					{
						return (DateTime)this;
					}
					case PrimitiveTypeCode.DateTimeNullable:
					{
						return (DateTime?)this;
					}
					case PrimitiveTypeCode.DateTimeOffset:
					{
						return (DateTimeOffset)this;
					}
					case PrimitiveTypeCode.DateTimeOffsetNullable:
					{
						return (DateTimeOffset?)this;
					}
					case PrimitiveTypeCode.Decimal:
					{
						return (decimal)this;
					}
					case PrimitiveTypeCode.DecimalNullable:
					{
						return (decimal?)this;
					}
					case PrimitiveTypeCode.Guid:
					{
						return (Guid)this;
					}
					case PrimitiveTypeCode.GuidNullable:
					{
						return (Guid?)this;
					}
					case PrimitiveTypeCode.TimeSpan:
					{
						return (TimeSpan)this;
					}
					case PrimitiveTypeCode.TimeSpanNullable:
					{
						return (TimeSpan?)this;
					}
					case PrimitiveTypeCode.Uri:
					{
						return (Uri)this;
					}
					case PrimitiveTypeCode.String:
					{
						return (string)this;
					}
				}
			}
			return this.ToObject(objectType, JsonSerializer.CreateDefault());
		}

		public T ToObject<T>(JsonSerializer jsonSerializer)
		{
			return (T)this.ToObject(typeof(T), jsonSerializer);
		}

		public object ToObject(System.Type objectType, JsonSerializer jsonSerializer)
		{
			object obj;
			ValidationUtils.ArgumentNotNull(jsonSerializer, "jsonSerializer");
			using (JTokenReader jTokenReader = new JTokenReader(this))
			{
				obj = jsonSerializer.Deserialize(jTokenReader, objectType);
			}
			return obj;
		}

		public override string ToString()
		{
			return this.ToString(Formatting.Indented, new JsonConverter[0]);
		}

		public string ToString(Formatting formatting, params JsonConverter[] converters)
		{
			string str;
			using (StringWriter stringWriter = new StringWriter(CultureInfo.InvariantCulture))
			{
				JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter)
				{
					Formatting = formatting
				};
				this.WriteTo(jsonTextWriter, converters);
				str = stringWriter.ToString();
			}
			return str;
		}

		private static bool ValidateToken(JToken o, JTokenType[] validTypes, bool nullable)
		{
			if (Array.IndexOf<JTokenType>(validTypes, o.Type) != -1)
			{
				return true;
			}
			if (!nullable)
			{
				return false;
			}
			if (o.Type == JTokenType.Null)
			{
				return true;
			}
			return o.Type == JTokenType.Undefined;
		}

		public virtual T Value<T>(object key)
		{
			JToken item = this[key];
			if (item != null)
			{
				return item.Convert<JToken, T>();
			}
			return default(T);
		}

		public virtual IEnumerable<T> Values<T>()
		{
			throw new InvalidOperationException("Cannot access child value on {0}.".FormatWith(CultureInfo.InvariantCulture, this.GetType()));
		}

		public abstract void WriteTo(JsonWriter writer, params JsonConverter[] converters);

		private class LineInfoAnnotation
		{
			internal readonly int LineNumber;

			internal readonly int LinePosition;

			public LineInfoAnnotation(int lineNumber, int linePosition)
			{
				this.LineNumber = lineNumber;
				this.LinePosition = linePosition;
			}
		}
	}
}