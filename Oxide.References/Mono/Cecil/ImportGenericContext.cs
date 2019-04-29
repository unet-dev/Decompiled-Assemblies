using Mono.Collections.Generic;
using System;

namespace Mono.Cecil
{
	internal struct ImportGenericContext
	{
		private Collection<IGenericParameterProvider> stack;

		public bool IsEmpty
		{
			get
			{
				return this.stack == null;
			}
		}

		public ImportGenericContext(IGenericParameterProvider provider)
		{
			this.stack = null;
			this.Push(provider);
		}

		private static TypeReference GenericTypeFor(IGenericParameterProvider context)
		{
			TypeReference typeReference = context as TypeReference;
			if (typeReference != null)
			{
				return typeReference.GetElementType();
			}
			MethodReference methodReference = context as MethodReference;
			if (methodReference == null)
			{
				throw new InvalidOperationException();
			}
			return methodReference.DeclaringType.GetElementType();
		}

		public TypeReference MethodParameter(string method, int position)
		{
			for (int i = this.stack.Count - 1; i >= 0; i--)
			{
				MethodReference item = this.stack[i] as MethodReference;
				if (item != null && !(method != this.NormalizeMethodName(item)))
				{
					return item.GenericParameters[position];
				}
			}
			throw new InvalidOperationException();
		}

		public string NormalizeMethodName(MethodReference method)
		{
			return string.Concat(method.DeclaringType.GetElementType().FullName, ".", method.Name);
		}

		public void Pop()
		{
			this.stack.RemoveAt(this.stack.Count - 1);
		}

		public void Push(IGenericParameterProvider provider)
		{
			if (this.stack != null)
			{
				this.stack.Add(provider);
				return;
			}
			this.stack = new Collection<IGenericParameterProvider>(1)
			{
				provider
			};
		}

		public TypeReference TypeParameter(string type, int position)
		{
			for (int i = this.stack.Count - 1; i >= 0; i--)
			{
				TypeReference typeReference = ImportGenericContext.GenericTypeFor(this.stack[i]);
				if (typeReference.FullName == type)
				{
					return typeReference.GenericParameters[position];
				}
			}
			throw new InvalidOperationException();
		}
	}
}