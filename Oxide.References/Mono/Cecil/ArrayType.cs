using Mono.Cecil.Metadata;
using Mono.Collections.Generic;
using System;
using System.Text;

namespace Mono.Cecil
{
	public sealed class ArrayType : TypeSpecification
	{
		private Collection<ArrayDimension> dimensions;

		public Collection<ArrayDimension> Dimensions
		{
			get
			{
				if (this.dimensions != null)
				{
					return this.dimensions;
				}
				this.dimensions = new Collection<ArrayDimension>()
				{
					new ArrayDimension()
				};
				return this.dimensions;
			}
		}

		public override string FullName
		{
			get
			{
				return string.Concat(base.FullName, this.Suffix);
			}
		}

		public override bool IsArray
		{
			get
			{
				return true;
			}
		}

		public override bool IsValueType
		{
			get
			{
				return false;
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public bool IsVector
		{
			get
			{
				if (this.dimensions == null)
				{
					return true;
				}
				if (this.dimensions.Count > 1)
				{
					return false;
				}
				return !this.dimensions[0].IsSized;
			}
		}

		public override string Name
		{
			get
			{
				return string.Concat(base.Name, this.Suffix);
			}
		}

		public int Rank
		{
			get
			{
				if (this.dimensions == null)
				{
					return 1;
				}
				return this.dimensions.Count;
			}
		}

		private string Suffix
		{
			get
			{
				if (this.IsVector)
				{
					return "[]";
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("[");
				for (int i = 0; i < this.dimensions.Count; i++)
				{
					if (i > 0)
					{
						stringBuilder.Append(",");
					}
					ArrayDimension item = this.dimensions[i];
					stringBuilder.Append(item.ToString());
				}
				stringBuilder.Append("]");
				return stringBuilder.ToString();
			}
		}

		public ArrayType(TypeReference type) : base(type)
		{
			Mixin.CheckType(type);
			this.etype = Mono.Cecil.Metadata.ElementType.Array;
		}

		public ArrayType(TypeReference type, int rank) : this(type)
		{
			Mixin.CheckType(type);
			if (rank == 1)
			{
				return;
			}
			this.dimensions = new Collection<ArrayDimension>(rank);
			for (int i = 0; i < rank; i++)
			{
				this.dimensions.Add(new ArrayDimension());
			}
			this.etype = Mono.Cecil.Metadata.ElementType.Array;
		}
	}
}