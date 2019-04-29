using System;
using System.Security.Cryptography;

namespace Mono.Security.Cryptography
{
	internal static class CryptoConvert
	{
		public static RSA FromCapiKeyBlob(byte[] blob)
		{
			return CryptoConvert.FromCapiKeyBlob(blob, 0);
		}

		public static RSA FromCapiKeyBlob(byte[] blob, int offset)
		{
			if (blob == null)
			{
				throw new ArgumentNullException("blob");
			}
			if (offset >= (int)blob.Length)
			{
				throw new ArgumentException("blob is too small.");
			}
			byte num = blob[offset];
			if (num != 0)
			{
				if (num == 6)
				{
					return CryptoConvert.FromCapiPublicKeyBlob(blob, offset);
				}
				if (num == 7)
				{
					return CryptoConvert.FromCapiPrivateKeyBlob(blob, offset);
				}
			}
			else if (blob[offset + 12] == 6)
			{
				return CryptoConvert.FromCapiPublicKeyBlob(blob, offset + 12);
			}
			throw new CryptographicException("Unknown blob format.");
		}

		private static RSA FromCapiPrivateKeyBlob(byte[] blob, int offset)
		{
			RSAParameters rSAParameter = new RSAParameters();
			try
			{
				if (blob[offset] != 7 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || CryptoConvert.ToUInt32LE(blob, offset + 8) != 843141970)
				{
					throw new CryptographicException("Invalid blob header");
				}
				int int32LE = CryptoConvert.ToInt32LE(blob, offset + 12);
				byte[] numArray = new byte[4];
				Buffer.BlockCopy(blob, offset + 16, numArray, 0, 4);
				Array.Reverse(numArray);
				rSAParameter.Exponent = CryptoConvert.Trim(numArray);
				int num = offset + 20;
				int num1 = int32LE >> 3;
				rSAParameter.Modulus = new byte[num1];
				Buffer.BlockCopy(blob, num, rSAParameter.Modulus, 0, num1);
				Array.Reverse(rSAParameter.Modulus);
				num += num1;
				int num2 = num1 >> 1;
				rSAParameter.P = new byte[num2];
				Buffer.BlockCopy(blob, num, rSAParameter.P, 0, num2);
				Array.Reverse(rSAParameter.P);
				num += num2;
				rSAParameter.Q = new byte[num2];
				Buffer.BlockCopy(blob, num, rSAParameter.Q, 0, num2);
				Array.Reverse(rSAParameter.Q);
				num += num2;
				rSAParameter.DP = new byte[num2];
				Buffer.BlockCopy(blob, num, rSAParameter.DP, 0, num2);
				Array.Reverse(rSAParameter.DP);
				num += num2;
				rSAParameter.DQ = new byte[num2];
				Buffer.BlockCopy(blob, num, rSAParameter.DQ, 0, num2);
				Array.Reverse(rSAParameter.DQ);
				num += num2;
				rSAParameter.InverseQ = new byte[num2];
				Buffer.BlockCopy(blob, num, rSAParameter.InverseQ, 0, num2);
				Array.Reverse(rSAParameter.InverseQ);
				num += num2;
				rSAParameter.D = new byte[num1];
				if (num + num1 + offset <= (int)blob.Length)
				{
					Buffer.BlockCopy(blob, num, rSAParameter.D, 0, num1);
					Array.Reverse(rSAParameter.D);
				}
			}
			catch (Exception exception)
			{
				throw new CryptographicException("Invalid blob.", exception);
			}
			RSA rSACryptoServiceProvider = null;
			try
			{
				rSACryptoServiceProvider = RSA.Create();
				rSACryptoServiceProvider.ImportParameters(rSAParameter);
			}
			catch (CryptographicException cryptographicException)
			{
				bool flag = false;
				try
				{
					rSACryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
					{
						Flags = CspProviderFlags.UseMachineKeyStore
					});
					rSACryptoServiceProvider.ImportParameters(rSAParameter);
				}
				catch
				{
					flag = true;
				}
				if (flag)
				{
					throw;
				}
			}
			return rSACryptoServiceProvider;
		}

		private static RSA FromCapiPublicKeyBlob(byte[] blob, int offset)
		{
			RSA rSA;
			try
			{
				if (blob[offset] != 6 || blob[offset + 1] != 2 || blob[offset + 2] != 0 || blob[offset + 3] != 0 || CryptoConvert.ToUInt32LE(blob, offset + 8) != 826364754)
				{
					throw new CryptographicException("Invalid blob header");
				}
				int int32LE = CryptoConvert.ToInt32LE(blob, offset + 12);
				RSAParameters rSAParameter = new RSAParameters()
				{
					Exponent = new byte[] { blob[offset + 18], blob[offset + 17], blob[offset + 16] }
				};
				int num = offset + 20;
				int num1 = int32LE >> 3;
				rSAParameter.Modulus = new byte[num1];
				Buffer.BlockCopy(blob, num, rSAParameter.Modulus, 0, num1);
				Array.Reverse(rSAParameter.Modulus);
				RSA rSACryptoServiceProvider = null;
				try
				{
					rSACryptoServiceProvider = RSA.Create();
					rSACryptoServiceProvider.ImportParameters(rSAParameter);
				}
				catch (CryptographicException cryptographicException)
				{
					rSACryptoServiceProvider = new RSACryptoServiceProvider(new CspParameters()
					{
						Flags = CspProviderFlags.UseMachineKeyStore
					});
					rSACryptoServiceProvider.ImportParameters(rSAParameter);
				}
				rSA = rSACryptoServiceProvider;
			}
			catch (Exception exception)
			{
				throw new CryptographicException("Invalid blob.", exception);
			}
			return rSA;
		}

		private static int ToInt32LE(byte[] bytes, int offset)
		{
			return bytes[offset + 3] << 24 | bytes[offset + 2] << 16 | bytes[offset + 1] << 8 | bytes[offset];
		}

		private static uint ToUInt32LE(byte[] bytes, int offset)
		{
			return (uint)(bytes[offset + 3] << 24 | bytes[offset + 2] << 16 | bytes[offset + 1] << 8 | bytes[offset]);
		}

		private static byte[] Trim(byte[] array)
		{
			for (int i = 0; i < (int)array.Length; i++)
			{
				if (array[i] != 0)
				{
					byte[] numArray = new byte[(int)array.Length - i];
					Buffer.BlockCopy(array, i, numArray, 0, (int)numArray.Length);
					return numArray;
				}
			}
			return null;
		}
	}
}