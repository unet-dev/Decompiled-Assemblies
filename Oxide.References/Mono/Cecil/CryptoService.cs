using Mono;
using Mono.Cecil.PE;
using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;

namespace Mono.Cecil
{
	internal static class CryptoService
	{
		public static byte[] ComputeHash(string file)
		{
			if (!File.Exists(file))
			{
				return Empty<byte>.Array;
			}
			SHA1Managed sHA1Managed = new SHA1Managed();
			using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				byte[] numArray = new byte[8192];
				using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sHA1Managed, CryptoStreamMode.Write))
				{
					CryptoService.CopyStreamChunk(fileStream, cryptoStream, numArray, (int)fileStream.Length);
				}
			}
			return sHA1Managed.Hash;
		}

		private static void CopyStreamChunk(Stream stream, Stream dest_stream, byte[] buffer, int length)
		{
			while (length > 0)
			{
				int num = stream.Read(buffer, 0, System.Math.Min((int)buffer.Length, length));
				dest_stream.Write(buffer, 0, num);
				length -= num;
			}
		}

		private static byte[] CreateStrongName(StrongNameKeyPair key_pair, byte[] hash)
		{
			byte[] numArray;
			using (RSA rSA = key_pair.CreateRSA())
			{
				RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSA);
				rSAPKCS1SignatureFormatter.SetHashAlgorithm("SHA1");
				byte[] numArray1 = rSAPKCS1SignatureFormatter.CreateSignature(hash);
				Array.Reverse((Array)numArray1);
				numArray = numArray1;
			}
			return numArray;
		}

		private static byte[] HashStream(Stream stream, ImageWriter writer, out int strong_name_pointer)
		{
			Section section = writer.text;
			int headerSize = (int)writer.GetHeaderSize();
			int pointerToRawData = (int)section.PointerToRawData;
			DataDirectory strongNameSignatureDirectory = writer.GetStrongNameSignatureDirectory();
			if (strongNameSignatureDirectory.Size == 0)
			{
				throw new InvalidOperationException();
			}
			strong_name_pointer = (int)((long)pointerToRawData + (ulong)(strongNameSignatureDirectory.VirtualAddress - section.VirtualAddress));
			int size = (int)strongNameSignatureDirectory.Size;
			SHA1Managed sHA1Managed = new SHA1Managed();
			byte[] numArray = new byte[8192];
			using (CryptoStream cryptoStream = new CryptoStream(Stream.Null, sHA1Managed, CryptoStreamMode.Write))
			{
				stream.Seek((long)0, SeekOrigin.Begin);
				CryptoService.CopyStreamChunk(stream, cryptoStream, numArray, headerSize);
				stream.Seek((long)pointerToRawData, SeekOrigin.Begin);
				CryptoService.CopyStreamChunk(stream, cryptoStream, numArray, strong_name_pointer - pointerToRawData);
				stream.Seek((long)size, SeekOrigin.Current);
				CryptoService.CopyStreamChunk(stream, cryptoStream, numArray, (int)(stream.Length - (long)(strong_name_pointer + size)));
			}
			return sHA1Managed.Hash;
		}

		private static void PatchStrongName(Stream stream, int strong_name_pointer, byte[] strong_name)
		{
			stream.Seek((long)strong_name_pointer, SeekOrigin.Begin);
			stream.Write(strong_name, 0, (int)strong_name.Length);
		}

		public static void StrongName(Stream stream, ImageWriter writer, StrongNameKeyPair key_pair)
		{
			int num;
			byte[] numArray = CryptoService.CreateStrongName(key_pair, CryptoService.HashStream(stream, writer, out num));
			CryptoService.PatchStrongName(stream, num, numArray);
		}
	}
}