using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.PE;
using Mono.Collections.Generic;
using System;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Rocks
{
	public static class ILParser
	{
		private static ILParser.ParseContext CreateContext(MethodDefinition method, IILVisitor visitor)
		{
			CodeReader codeReader = method.Module.Read<MethodDefinition, CodeReader>(method, (MethodDefinition _, MetadataReader reader) => new CodeReader(reader.image.MetadataSection, reader));
			return new ILParser.ParseContext()
			{
				Code = codeReader,
				Metadata = codeReader.reader,
				Visitor = visitor
			};
		}

		private static VariableDefinition GetVariable(ILParser.ParseContext context, int index)
		{
			return context.Variables[index];
		}

		public static void Parse(MethodDefinition method, IILVisitor visitor)
		{
			if (method == null)
			{
				throw new ArgumentNullException("method");
			}
			if (visitor == null)
			{
				throw new ArgumentNullException("visitor");
			}
			if (!method.HasBody || !method.HasImage)
			{
				throw new ArgumentException();
			}
			ILParser.ParseContext parseContext = ILParser.CreateContext(method, visitor);
			CodeReader code = parseContext.Code;
			code.MoveTo(method.RVA);
			byte num = code.ReadByte();
			int num1 = num & 3;
			if (num1 == 2)
			{
				ILParser.ParseCode(num >> 2, parseContext);
				return;
			}
			if (num1 != 3)
			{
				throw new NotSupportedException();
			}
			code.position--;
			ILParser.ParseFatMethod(parseContext);
		}

		private static void ParseCode(int code_size, ILParser.ParseContext context)
		{
			CodeReader code = context.Code;
			MetadataReader metadata = context.Metadata;
			IILVisitor visitor = context.Visitor;
			int codeSize = code.position + code_size;
			while (code.position < codeSize)
			{
				byte num = code.ReadByte();
				OpCode opCode = (num != 254 ? OpCodes.OneByteOpCode[num] : OpCodes.TwoBytesOpCode[code.ReadByte()]);
				switch (opCode.OperandType)
				{
					case OperandType.InlineBrTarget:
					{
						visitor.OnInlineBranch(opCode, code.ReadInt32());
						continue;
					}
					case OperandType.InlineField:
					case OperandType.InlineMethod:
					case OperandType.InlineTok:
					case OperandType.InlineType:
					{
						IMetadataTokenProvider metadataTokenProvider = metadata.LookupToken(code.ReadToken());
						Mono.Cecil.TokenType tokenType = metadataTokenProvider.MetadataToken.TokenType;
						if (tokenType > Mono.Cecil.TokenType.Field)
						{
							if (tokenType > Mono.Cecil.TokenType.MemberRef)
							{
								if (tokenType == Mono.Cecil.TokenType.TypeSpec)
								{
									goto Label0;
								}
								if (tokenType != Mono.Cecil.TokenType.MethodSpec)
								{
									continue;
								}
							}
							else if (tokenType != Mono.Cecil.TokenType.Method)
							{
								if (tokenType == Mono.Cecil.TokenType.MemberRef)
								{
									FieldReference fieldReference = metadataTokenProvider as FieldReference;
									if (fieldReference == null)
									{
										MethodReference methodReference = metadataTokenProvider as MethodReference;
										if (methodReference == null)
										{
											throw new InvalidOperationException();
										}
										visitor.OnInlineMethod(opCode, methodReference);
										continue;
									}
									else
									{
										visitor.OnInlineField(opCode, fieldReference);
										continue;
									}
								}
								else
								{
									continue;
								}
							}
							visitor.OnInlineMethod(opCode, (MethodReference)metadataTokenProvider);
							continue;
						}
						else if (tokenType != Mono.Cecil.TokenType.TypeRef && tokenType != Mono.Cecil.TokenType.TypeDef)
						{
							if (tokenType == Mono.Cecil.TokenType.Field)
							{
								visitor.OnInlineField(opCode, (FieldReference)metadataTokenProvider);
								continue;
							}
							else
							{
								continue;
							}
						}
					Label0:
						visitor.OnInlineType(opCode, (TypeReference)metadataTokenProvider);
						continue;
					}
					case OperandType.InlineI:
					{
						visitor.OnInlineInt32(opCode, code.ReadInt32());
						continue;
					}
					case OperandType.InlineI8:
					{
						visitor.OnInlineInt64(opCode, code.ReadInt64());
						continue;
					}
					case OperandType.InlineNone:
					{
						visitor.OnInlineNone(opCode);
						continue;
					}
					case OperandType.InlineR:
					{
						visitor.OnInlineDouble(opCode, code.ReadDouble());
						continue;
					}
					case OperandType.InlineSig:
					{
						visitor.OnInlineSignature(opCode, code.GetCallSite(code.ReadToken()));
						continue;
					}
					case OperandType.InlineString:
					{
						visitor.OnInlineString(opCode, code.GetString(code.ReadToken()));
						continue;
					}
					case OperandType.InlineSwitch:
					{
						int num1 = code.ReadInt32();
						int[] numArray = new int[num1];
						for (int i = 0; i < num1; i++)
						{
							numArray[i] = code.ReadInt32();
						}
						visitor.OnInlineSwitch(opCode, numArray);
						continue;
					}
					case OperandType.InlineVar:
					{
						visitor.OnInlineVariable(opCode, ILParser.GetVariable(context, code.ReadInt16()));
						continue;
					}
					case OperandType.InlineArg:
					{
						visitor.OnInlineArgument(opCode, code.GetParameter(code.ReadInt16()));
						continue;
					}
					case OperandType.ShortInlineBrTarget:
					{
						visitor.OnInlineBranch(opCode, code.ReadSByte());
						continue;
					}
					case OperandType.ShortInlineI:
					{
						if (opCode != OpCodes.Ldc_I4_S)
						{
							visitor.OnInlineByte(opCode, code.ReadByte());
							continue;
						}
						else
						{
							visitor.OnInlineSByte(opCode, code.ReadSByte());
							continue;
						}
					}
					case OperandType.ShortInlineR:
					{
						visitor.OnInlineSingle(opCode, code.ReadSingle());
						continue;
					}
					case OperandType.ShortInlineVar:
					{
						visitor.OnInlineVariable(opCode, ILParser.GetVariable(context, (int)code.ReadByte()));
						continue;
					}
					case OperandType.ShortInlineArg:
					{
						visitor.OnInlineArgument(opCode, code.GetParameter((int)code.ReadByte()));
						continue;
					}
					default:
					{
						continue;
					}
				}
			}
		}

		private static void ParseFatMethod(ILParser.ParseContext context)
		{
			CodeReader code = context.Code;
			code.Advance(4);
			int num = code.ReadInt32();
			MetadataToken metadataToken = code.ReadToken();
			if (metadataToken != MetadataToken.Zero)
			{
				context.Variables = code.ReadVariables(metadataToken);
			}
			ILParser.ParseCode(num, context);
		}

		private class ParseContext
		{
			public CodeReader Code
			{
				get;
				set;
			}

			public MetadataReader Metadata
			{
				get;
				set;
			}

			public Collection<VariableDefinition> Variables
			{
				get;
				set;
			}

			public IILVisitor Visitor
			{
				get;
				set;
			}

			public ParseContext()
			{
			}
		}
	}
}