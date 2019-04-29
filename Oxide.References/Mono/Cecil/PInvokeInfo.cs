using System;

namespace Mono.Cecil
{
	public sealed class PInvokeInfo
	{
		private ushort attributes;

		private string entry_point;

		private ModuleReference module;

		public PInvokeAttributes Attributes
		{
			get
			{
				return (PInvokeAttributes)this.attributes;
			}
			set
			{
				this.attributes = (ushort)value;
			}
		}

		public string EntryPoint
		{
			get
			{
				return this.entry_point;
			}
			set
			{
				this.entry_point = value;
			}
		}

		public bool IsBestFitDisabled
		{
			get
			{
				return this.attributes.GetMaskedAttributes(48, 32);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(48, 32, value);
			}
		}

		public bool IsBestFitEnabled
		{
			get
			{
				return this.attributes.GetMaskedAttributes(48, 16);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(48, 16, value);
			}
		}

		public bool IsCallConvCdecl
		{
			get
			{
				return this.attributes.GetMaskedAttributes(1792, 512);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(1792, 512, value);
			}
		}

		public bool IsCallConvFastcall
		{
			get
			{
				return this.attributes.GetMaskedAttributes(1792, 1280);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(1792, 1280, value);
			}
		}

		public bool IsCallConvStdCall
		{
			get
			{
				return this.attributes.GetMaskedAttributes(1792, 768);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(1792, 768, value);
			}
		}

		public bool IsCallConvThiscall
		{
			get
			{
				return this.attributes.GetMaskedAttributes(1792, 1024);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(1792, 1024, value);
			}
		}

		public bool IsCallConvWinapi
		{
			get
			{
				return this.attributes.GetMaskedAttributes(1792, 256);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(1792, 256, value);
			}
		}

		public bool IsCharSetAnsi
		{
			get
			{
				return this.attributes.GetMaskedAttributes(6, 2);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(6, 2, value);
			}
		}

		public bool IsCharSetAuto
		{
			get
			{
				return this.attributes.GetMaskedAttributes(6, 6);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(6, 6, value);
			}
		}

		public bool IsCharSetNotSpec
		{
			get
			{
				return this.attributes.GetMaskedAttributes(6, 0);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(6, 0, value);
			}
		}

		public bool IsCharSetUnicode
		{
			get
			{
				return this.attributes.GetMaskedAttributes(6, 4);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(6, 4, value);
			}
		}

		public bool IsNoMangle
		{
			get
			{
				return this.attributes.GetAttributes(1);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(1, value);
			}
		}

		public bool IsThrowOnUnmappableCharDisabled
		{
			get
			{
				return this.attributes.GetMaskedAttributes(12288, 8192);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(12288, 8192, value);
			}
		}

		public bool IsThrowOnUnmappableCharEnabled
		{
			get
			{
				return this.attributes.GetMaskedAttributes(12288, 4096);
			}
			set
			{
				this.attributes = this.attributes.SetMaskedAttributes(12288, 4096, value);
			}
		}

		public ModuleReference Module
		{
			get
			{
				return this.module;
			}
			set
			{
				this.module = value;
			}
		}

		public bool SupportsLastError
		{
			get
			{
				return this.attributes.GetAttributes(64);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(64, value);
			}
		}

		public PInvokeInfo(PInvokeAttributes attributes, string entryPoint, ModuleReference module)
		{
			this.attributes = (ushort)attributes;
			this.entry_point = entryPoint;
			this.module = module;
		}
	}
}