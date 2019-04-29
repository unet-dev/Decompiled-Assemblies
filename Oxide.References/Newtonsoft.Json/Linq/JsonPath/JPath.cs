using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Shims;
using Newtonsoft.Json.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[Preserve]
	internal class JPath
	{
		private readonly string _expression;

		private int _currentIndex;

		public List<PathFilter> Filters
		{
			get;
			private set;
		}

		public JPath(string expression)
		{
			ValidationUtils.ArgumentNotNull(expression, "expression");
			this._expression = expression;
			this.Filters = new List<PathFilter>();
			this.ParseMain();
		}

		private void EatWhitespace()
		{
			while (this._currentIndex < this._expression.Length && this._expression[this._currentIndex] == ' ')
			{
				this._currentIndex++;
			}
		}

		private void EnsureLength(string message)
		{
			if (this._currentIndex >= this._expression.Length)
			{
				throw new JsonException(message);
			}
		}

		internal IEnumerable<JToken> Evaluate(JToken t, bool errorWhenNoMatch)
		{
			return JPath.Evaluate(this.Filters, t, errorWhenNoMatch);
		}

		internal static IEnumerable<JToken> Evaluate(List<PathFilter> filters, JToken t, bool errorWhenNoMatch)
		{
			IEnumerable<JToken> jTokens = new JToken[] { t };
			foreach (PathFilter filter in filters)
			{
				jTokens = filter.ExecuteFilter(jTokens, errorWhenNoMatch);
			}
			return jTokens;
		}

		private bool Match(string s)
		{
			int num = this._currentIndex;
			string str = s;
			for (int i = 0; i < str.Length; i++)
			{
				char chr = str[i];
				if (num >= this._expression.Length || this._expression[num] != chr)
				{
					return false;
				}
				num++;
			}
			this._currentIndex = num;
			return true;
		}

		private PathFilter ParseArrayIndexer(char indexerCloseChar)
		{
			int? nullable;
			int num = this._currentIndex;
			int? nullable1 = null;
			List<int> nums = null;
			int num1 = 0;
			int? nullable2 = null;
			int? nullable3 = null;
			int? nullable4 = null;
			while (this._currentIndex < this._expression.Length)
			{
				char chr = this._expression[this._currentIndex];
				if (chr != ' ')
				{
					if (chr == indexerCloseChar)
					{
						nullable = nullable1;
						int num2 = (nullable.HasValue ? nullable.GetValueOrDefault() : this._currentIndex) - num;
						if (nums != null)
						{
							if (num2 == 0)
							{
								throw new JsonException("Array index expected.");
							}
							int num3 = Convert.ToInt32(this._expression.Substring(num, num2), CultureInfo.InvariantCulture);
							nums.Add(num3);
							return new ArrayMultipleIndexFilter()
							{
								Indexes = nums
							};
						}
						if (num1 <= 0)
						{
							if (num2 == 0)
							{
								throw new JsonException("Array index expected.");
							}
							int num4 = Convert.ToInt32(this._expression.Substring(num, num2), CultureInfo.InvariantCulture);
							return new ArrayIndexFilter()
							{
								Index = new int?(num4)
							};
						}
						if (num2 > 0)
						{
							int num5 = Convert.ToInt32(this._expression.Substring(num, num2), CultureInfo.InvariantCulture);
							if (num1 != 1)
							{
								nullable4 = new int?(num5);
							}
							else
							{
								nullable3 = new int?(num5);
							}
						}
						return new ArraySliceFilter()
						{
							Start = nullable2,
							End = nullable3,
							Step = nullable4
						};
					}
					if (chr != ',')
					{
						if (chr == '*')
						{
							this._currentIndex++;
							this.EnsureLength("Path ended with open indexer.");
							this.EatWhitespace();
							if (this._expression[this._currentIndex] != indexerCloseChar)
							{
								throw new JsonException(string.Concat("Unexpected character while parsing path indexer: ", chr.ToString()));
							}
							return new ArrayIndexFilter();
						}
						if (chr != ':')
						{
							if (!char.IsDigit(chr) && chr != '-')
							{
								throw new JsonException(string.Concat("Unexpected character while parsing path indexer: ", chr.ToString()));
							}
							if (nullable1.HasValue)
							{
								throw new JsonException(string.Concat("Unexpected character while parsing path indexer: ", chr.ToString()));
							}
							this._currentIndex++;
						}
						else
						{
							nullable = nullable1;
							int num6 = (nullable.HasValue ? nullable.GetValueOrDefault() : this._currentIndex) - num;
							if (num6 > 0)
							{
								int num7 = Convert.ToInt32(this._expression.Substring(num, num6), CultureInfo.InvariantCulture);
								if (num1 == 0)
								{
									nullable2 = new int?(num7);
								}
								else if (num1 != 1)
								{
									nullable4 = new int?(num7);
								}
								else
								{
									nullable3 = new int?(num7);
								}
							}
							num1++;
							this._currentIndex++;
							this.EatWhitespace();
							num = this._currentIndex;
							nullable1 = null;
						}
					}
					else
					{
						nullable = nullable1;
						int num8 = (nullable.HasValue ? nullable.GetValueOrDefault() : this._currentIndex) - num;
						if (num8 == 0)
						{
							throw new JsonException("Array index expected.");
						}
						if (nums == null)
						{
							nums = new List<int>();
						}
						string str = this._expression.Substring(num, num8);
						nums.Add(Convert.ToInt32(str, CultureInfo.InvariantCulture));
						this._currentIndex++;
						this.EatWhitespace();
						num = this._currentIndex;
						nullable1 = null;
					}
				}
				else
				{
					nullable1 = new int?(this._currentIndex);
					this.EatWhitespace();
				}
			}
			throw new JsonException("Path ended with open indexer.");
		}

		private QueryExpression ParseExpression()
		{
			QueryOperator queryOperator;
			JValue jValue;
			QueryExpression queryExpression = null;
			CompositeExpression compositeExpression = null;
			while (this._currentIndex < this._expression.Length)
			{
				this.EatWhitespace();
				if (this._expression[this._currentIndex] != '@')
				{
					char chr = this._expression[this._currentIndex];
					throw new JsonException(string.Concat("Unexpected character while parsing path query: ", chr.ToString()));
				}
				this._currentIndex++;
				List<PathFilter> pathFilters = new List<PathFilter>();
				if (this.ParsePath(pathFilters, this._currentIndex, true))
				{
					throw new JsonException("Path ended with open query.");
				}
				this.EatWhitespace();
				this.EnsureLength("Path ended with open query.");
				object obj = null;
				if (this._expression[this._currentIndex] == ')' || this._expression[this._currentIndex] == '|' || this._expression[this._currentIndex] == '&')
				{
					queryOperator = QueryOperator.Exists;
				}
				else
				{
					queryOperator = this.ParseOperator();
					this.EatWhitespace();
					this.EnsureLength("Path ended with open query.");
					obj = this.ParseValue();
					this.EatWhitespace();
					this.EnsureLength("Path ended with open query.");
				}
				BooleanQueryExpression booleanQueryExpression = new BooleanQueryExpression()
				{
					Path = pathFilters,
					Operator = queryOperator
				};
				if (queryOperator != QueryOperator.Exists)
				{
					jValue = new JValue(obj);
				}
				else
				{
					jValue = null;
				}
				booleanQueryExpression.Value = jValue;
				BooleanQueryExpression booleanQueryExpression1 = booleanQueryExpression;
				if (this._expression[this._currentIndex] == ')')
				{
					if (compositeExpression == null)
					{
						return booleanQueryExpression1;
					}
					compositeExpression.Expressions.Add(booleanQueryExpression1);
					return queryExpression;
				}
				if (this._expression[this._currentIndex] == '&' && this.Match("&&"))
				{
					if (compositeExpression == null || compositeExpression.Operator != QueryOperator.And)
					{
						CompositeExpression compositeExpression1 = new CompositeExpression()
						{
							Operator = QueryOperator.And
						};
						if (compositeExpression != null)
						{
							compositeExpression.Expressions.Add(compositeExpression1);
						}
						compositeExpression = compositeExpression1;
						if (queryExpression == null)
						{
							queryExpression = compositeExpression;
						}
					}
					compositeExpression.Expressions.Add(booleanQueryExpression1);
				}
				if (this._expression[this._currentIndex] != '|' || !this.Match("||"))
				{
					continue;
				}
				if (compositeExpression == null || compositeExpression.Operator != QueryOperator.Or)
				{
					CompositeExpression compositeExpression2 = new CompositeExpression()
					{
						Operator = QueryOperator.Or
					};
					if (compositeExpression != null)
					{
						compositeExpression.Expressions.Add(compositeExpression2);
					}
					compositeExpression = compositeExpression2;
					if (queryExpression == null)
					{
						queryExpression = compositeExpression;
					}
				}
				compositeExpression.Expressions.Add(booleanQueryExpression1);
			}
			throw new JsonException("Path ended with open query.");
		}

		private PathFilter ParseIndexer(char indexerOpenChar)
		{
			this._currentIndex++;
			char chr = (indexerOpenChar == '[' ? ']' : ')');
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] == '\'')
			{
				return this.ParseQuotedField(chr);
			}
			if (this._expression[this._currentIndex] == '?')
			{
				return this.ParseQuery(chr);
			}
			return this.ParseArrayIndexer(chr);
		}

		private void ParseMain()
		{
			int num = this._currentIndex;
			this.EatWhitespace();
			if (this._expression.Length == this._currentIndex)
			{
				return;
			}
			if (this._expression[this._currentIndex] == '$')
			{
				if (this._expression.Length == 1)
				{
					return;
				}
				char chr = this._expression[this._currentIndex + 1];
				if (chr == '.' || chr == '[')
				{
					this._currentIndex++;
					num = this._currentIndex;
				}
			}
			if (!this.ParsePath(this.Filters, num, false))
			{
				int num1 = this._currentIndex;
				this.EatWhitespace();
				if (this._currentIndex < this._expression.Length)
				{
					char chr1 = this._expression[num1];
					throw new JsonException(string.Concat("Unexpected character while parsing path: ", chr1.ToString()));
				}
			}
		}

		private QueryOperator ParseOperator()
		{
			if (this._currentIndex + 1 >= this._expression.Length)
			{
				throw new JsonException("Path ended with open query.");
			}
			if (this.Match("=="))
			{
				return QueryOperator.Equals;
			}
			if (this.Match("!=") || this.Match("<>"))
			{
				return QueryOperator.NotEquals;
			}
			if (this.Match("<="))
			{
				return QueryOperator.LessThanOrEquals;
			}
			if (this.Match("<"))
			{
				return QueryOperator.LessThan;
			}
			if (this.Match(">="))
			{
				return QueryOperator.GreaterThanOrEquals;
			}
			if (!this.Match(">"))
			{
				throw new JsonException("Could not read query operator.");
			}
			return QueryOperator.GreaterThan;
		}

		private bool ParsePath(List<PathFilter> filters, int currentPartStartIndex, bool query)
		{
			char chr;
			PathFilter scanFilter;
			PathFilter fieldFilter;
			PathFilter pathFilter;
			bool flag = false;
			bool flag1 = false;
			bool flag2 = false;
			bool flag3 = false;
		Label2:
			while (this._currentIndex < this._expression.Length && !flag3)
			{
				chr = this._expression[this._currentIndex];
				if (chr <= ')')
				{
					if (chr == ' ')
					{
						if (this._currentIndex >= this._expression.Length)
						{
							continue;
						}
						flag3 = true;
						continue;
					}
					else
					{
						if (chr == '(')
						{
							goto Label0;
						}
						if (chr == ')')
						{
							goto Label1;
						}
					}
				}
				else if (chr == '.')
				{
					if (this._currentIndex > currentPartStartIndex)
					{
						string str = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
						if (str == "*")
						{
							str = null;
						}
						if (flag)
						{
							fieldFilter = new ScanFilter();
							((ScanFilter)fieldFilter).Name = str;
						}
						else
						{
							fieldFilter = new FieldFilter();
							((FieldFilter)fieldFilter).Name = str;
						}
						filters.Add(fieldFilter);
						flag = false;
					}
					if (this._currentIndex + 1 < this._expression.Length && this._expression[this._currentIndex + 1] == '.')
					{
						flag = true;
						this._currentIndex++;
					}
					this._currentIndex++;
					currentPartStartIndex = this._currentIndex;
					flag1 = false;
					flag2 = true;
					continue;
				}
				else
				{
					if (chr == '[')
					{
						goto Label0;
					}
					if (chr == ']')
					{
						goto Label1;
					}
				}
				if (!query || chr != '=' && chr != '<' && chr != '!' && chr != '>' && chr != '|' && chr != '&')
				{
					if (flag1)
					{
						throw new JsonException(string.Concat("Unexpected character following indexer: ", chr.ToString()));
					}
					this._currentIndex++;
				}
				else
				{
					flag3 = true;
				}
			}
			bool length = this._currentIndex == this._expression.Length;
			if (this._currentIndex > currentPartStartIndex)
			{
				string str1 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex).TrimEnd(new char[0]);
				if (str1 == "*")
				{
					str1 = null;
				}
				if (flag)
				{
					scanFilter = new ScanFilter();
					((ScanFilter)scanFilter).Name = str1;
				}
				else
				{
					scanFilter = new FieldFilter();
					((FieldFilter)scanFilter).Name = str1;
				}
				filters.Add(scanFilter);
			}
			else if (flag2 && length | query)
			{
				throw new JsonException("Unexpected end while parsing path.");
			}
			return length;
		Label0:
			if (this._currentIndex > currentPartStartIndex)
			{
				string str2 = this._expression.Substring(currentPartStartIndex, this._currentIndex - currentPartStartIndex);
				if (str2 == "*")
				{
					str2 = null;
				}
				if (flag)
				{
					pathFilter = new ScanFilter();
					((ScanFilter)pathFilter).Name = str2;
				}
				else
				{
					pathFilter = new FieldFilter();
					((FieldFilter)pathFilter).Name = str2;
				}
				filters.Add(pathFilter);
				flag = false;
			}
			filters.Add(this.ParseIndexer(chr));
			this._currentIndex++;
			currentPartStartIndex = this._currentIndex;
			flag1 = true;
			flag2 = false;
			goto Label2;
		Label1:
			flag3 = true;
			goto Label2;
		}

		private PathFilter ParseQuery(char indexerCloseChar)
		{
			char chr;
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			if (this._expression[this._currentIndex] != '(')
			{
				chr = this._expression[this._currentIndex];
				throw new JsonException(string.Concat("Unexpected character while parsing path indexer: ", chr.ToString()));
			}
			this._currentIndex++;
			QueryExpression queryExpression = this.ParseExpression();
			this._currentIndex++;
			this.EnsureLength("Path ended with open indexer.");
			this.EatWhitespace();
			if (this._expression[this._currentIndex] != indexerCloseChar)
			{
				chr = this._expression[this._currentIndex];
				throw new JsonException(string.Concat("Unexpected character while parsing path indexer: ", chr.ToString()));
			}
			return new QueryFilter()
			{
				Expression = queryExpression
			};
		}

		private PathFilter ParseQuotedField(char indexerCloseChar)
		{
			List<string> strs = null;
			while (this._currentIndex < this._expression.Length)
			{
				string str = this.ReadQuotedString();
				this.EatWhitespace();
				this.EnsureLength("Path ended with open indexer.");
				if (this._expression[this._currentIndex] == indexerCloseChar)
				{
					if (strs == null)
					{
						return new FieldFilter()
						{
							Name = str
						};
					}
					strs.Add(str);
					return new FieldMultipleFilter()
					{
						Names = strs
					};
				}
				if (this._expression[this._currentIndex] != ',')
				{
					char chr = this._expression[this._currentIndex];
					throw new JsonException(string.Concat("Unexpected character while parsing path indexer: ", chr.ToString()));
				}
				this._currentIndex++;
				this.EatWhitespace();
				if (strs == null)
				{
					strs = new List<string>();
				}
				strs.Add(str);
			}
			throw new JsonException("Path ended with open indexer.");
		}

		private object ParseValue()
		{
			double num;
			long num1;
			char chr = this._expression[this._currentIndex];
			if (chr == '\'')
			{
				return this.ReadQuotedString();
			}
			if (char.IsDigit(chr) || chr == '-')
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(chr);
				this._currentIndex++;
				while (this._currentIndex < this._expression.Length)
				{
					chr = this._expression[this._currentIndex];
					if (chr == ' ' || chr == ')')
					{
						string str = stringBuilder.ToString();
						if (str.IndexOfAny(new char[] { '.', 'E', 'e' }) != -1)
						{
							if (!double.TryParse(str, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.Integer | NumberStyles.Float, CultureInfo.InvariantCulture, out num))
							{
								throw new JsonException("Could not read query value.");
							}
							return num;
						}
						if (!long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out num1))
						{
							throw new JsonException("Could not read query value.");
						}
						return num1;
					}
					stringBuilder.Append(chr);
					this._currentIndex++;
				}
			}
			else if (chr == 't')
			{
				if (this.Match("true"))
				{
					return true;
				}
			}
			else if (chr == 'f')
			{
				if (this.Match("false"))
				{
					return false;
				}
			}
			else if (chr == 'n' && this.Match("null"))
			{
				return null;
			}
			throw new JsonException("Could not read query value.");
		}

		private string ReadQuotedString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this._currentIndex++;
			while (this._currentIndex < this._expression.Length)
			{
				char chr = this._expression[this._currentIndex];
				if (chr != '\\' || this._currentIndex + 1 >= this._expression.Length)
				{
					if (chr == '\'')
					{
						this._currentIndex++;
						return stringBuilder.ToString();
					}
					this._currentIndex++;
					stringBuilder.Append(chr);
				}
				else
				{
					this._currentIndex++;
					if (this._expression[this._currentIndex] != '\'')
					{
						if (this._expression[this._currentIndex] != '\\')
						{
							char chr1 = this._expression[this._currentIndex];
							throw new JsonException(string.Concat("Unknown escape chracter: \\", chr1.ToString()));
						}
						stringBuilder.Append('\\');
					}
					else
					{
						stringBuilder.Append('\'');
					}
					this._currentIndex++;
				}
			}
			throw new JsonException("Path ended with an open string.");
		}
	}
}