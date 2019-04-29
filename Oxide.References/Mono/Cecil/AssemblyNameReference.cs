using Mono;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Mono.Cecil
{
	public class AssemblyNameReference : IMetadataScope, IMetadataTokenProvider
	{
		private string name;

		private string culture;

		private System.Version version;

		private uint attributes;

		private byte[] public_key;

		private byte[] public_key_token;

		private AssemblyHashAlgorithm hash_algorithm;

		private byte[] hash;

		internal Mono.Cecil.MetadataToken token;

		private string full_name;

		public AssemblyAttributes Attributes
		{
			get
			{
				return (AssemblyAttributes)this.attributes;
			}
			set
			{
				this.attributes = (uint)value;
			}
		}

		public string Culture
		{
			get
			{
				return this.culture;
			}
			set
			{
				this.culture = value;
				this.full_name = null;
			}
		}

		public string FullName
		{
			get
			{
				if (this.full_name != null)
				{
					return this.full_name;
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(this.name);
				if (this.version != null)
				{
					stringBuilder.Append(", ");
					stringBuilder.Append("Version=");
					stringBuilder.Append(this.version.ToString());
				}
				stringBuilder.Append(", ");
				stringBuilder.Append("Culture=");
				stringBuilder.Append((string.IsNullOrEmpty(this.culture) ? "neutral" : this.culture));
				stringBuilder.Append(", ");
				stringBuilder.Append("PublicKeyToken=");
				byte[] publicKeyToken = this.PublicKeyToken;
				if (publicKeyToken.IsNullOrEmpty<byte>() || publicKeyToken.Length == 0)
				{
					stringBuilder.Append("null");
				}
				else
				{
					for (int i = 0; i < (int)publicKeyToken.Length; i++)
					{
						stringBuilder.Append(publicKeyToken[i].ToString("x2"));
					}
				}
				string str = stringBuilder.ToString();
				string str1 = str;
				this.full_name = str;
				return str1;
			}
		}

		public virtual byte[] Hash
		{
			get
			{
				return this.hash;
			}
			set
			{
				this.hash = value;
			}
		}

		public AssemblyHashAlgorithm HashAlgorithm
		{
			get
			{
				return this.hash_algorithm;
			}
			set
			{
				this.hash_algorithm = value;
			}
		}

		public bool HasPublicKey
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

		public bool IsRetargetable
		{
			get
			{
				return this.attributes.GetAttributes(256);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(256, value);
			}
		}

		public bool IsSideBySideCompatible
		{
			get
			{
				return this.attributes.GetAttributes(0);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(0, value);
			}
		}

		public bool IsWindowsRuntime
		{
			get
			{
				return this.attributes.GetAttributes(512);
			}
			set
			{
				this.attributes = this.attributes.SetAttributes(512, value);
			}
		}

		public virtual Mono.Cecil.MetadataScopeType MetadataScopeType
		{
			get
			{
				return Mono.Cecil.MetadataScopeType.AssemblyNameReference;
			}
		}

		public Mono.Cecil.MetadataToken MetadataToken
		{
			get
			{
				return this.token;
			}
			set
			{
				this.token = value;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				this.name = value;
				this.full_name = null;
			}
		}

		public byte[] PublicKey
		{
			get
			{
				Object publicKey = this.public_key;
				if (publicKey == null)
				{
					publicKey = Empty<byte>.Array;
				}
				return publicKey;
			}
			set
			{
				this.public_key = value;
				this.HasPublicKey = !this.public_key.IsNullOrEmpty<byte>();
				this.public_key_token = Empty<byte>.Array;
				this.full_name = null;
			}
		}

		public byte[] PublicKeyToken
		{
			get
			{
				if (this.public_key_token.IsNullOrEmpty<byte>() && !this.public_key.IsNullOrEmpty<byte>())
				{
					byte[] numArray = this.HashPublicKey();
					byte[] numArray1 = new byte[8];
					Array.Copy(numArray, (int)numArray.Length - 8, numArray1, 0, 8);
					Array.Reverse(numArray1, 0, 8);
					this.public_key_token = numArray1;
				}
				Object publicKeyToken = this.public_key_token;
				if (publicKeyToken == null)
				{
					publicKeyToken = Empty<byte>.Array;
				}
				return publicKeyToken;
			}
			set
			{
				this.public_key_token = value;
				this.full_name = null;
			}
		}

		public System.Version Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
				this.full_name = null;
			}
		}

		internal AssemblyNameReference()
		{
		}

		public AssemblyNameReference(string name, System.Version version)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			this.name = name;
			this.version = version;
			this.hash_algorithm = AssemblyHashAlgorithm.None;
			this.token = new Mono.Cecil.MetadataToken(Mono.Cecil.TokenType.AssemblyRef);
		}

		private byte[] HashPublicKey()
		{
			System.Security.Cryptography.HashAlgorithm hashAlgorithm;
			byte[] numArray;
			if (this.hash_algorithm != AssemblyHashAlgorithm.Reserved)
			{
				hashAlgorithm = SHA1.Create();
			}
			else
			{
				hashAlgorithm = MD5.Create();
			}
			using (hashAlgorithm)
			{
				numArray = hashAlgorithm.ComputeHash(this.public_key);
			}
			return numArray;
		}

		public static AssemblyNameReference Parse(string fullName)
		{
			if (fullName == null)
			{
				throw new ArgumentNullException("fullName");
			}
			if (fullName.Length == 0)
			{
				throw new ArgumentException("Name can not be empty");
			}
			AssemblyNameReference assemblyNameReference = new AssemblyNameReference();
			string[] strArrays = fullName.Split(new char[] { ',' });
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				string str = strArrays[i].Trim();
				if (i != 0)
				{
					string[] strArrays1 = str.Split(new char[] { '=' });
					if ((int)strArrays1.Length != 2)
					{
						throw new ArgumentException("Malformed name");
					}
					string lowerInvariant = strArrays1[0].ToLowerInvariant();
					if (lowerInvariant == "version")
					{
						assemblyNameReference.Version = new System.Version(strArrays1[1]);
					}
					else if (lowerInvariant == "culture")
					{
						assemblyNameReference.Culture = strArrays1[1];
					}
					else if (lowerInvariant == "publickeytoken")
					{
						string str1 = strArrays1[1];
						if (str1 != "null")
						{
							assemblyNameReference.PublicKeyToken = new byte[str1.Length / 2];
							for (int j = 0; j < (int)assemblyNameReference.PublicKeyToken.Length; j++)
							{
								assemblyNameReference.PublicKeyToken[j] = byte.Parse(str1.Substring(j * 2, 2), NumberStyles.HexNumber);
							}
						}
					}
				}
				else
				{
					assemblyNameReference.Name = str;
				}
			}
			return assemblyNameReference;
		}

		public override string ToString()
		{
			return this.FullName;
		}
	}
}