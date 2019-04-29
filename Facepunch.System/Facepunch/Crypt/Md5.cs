using System;
using System.Security.Cryptography;
using System.Text;

namespace Facepunch.Crypt
{
	public class Md5
	{
		public Md5()
		{
		}

		public static string Calculate(string input)
		{
			return Md5.Calculate(Encoding.ASCII.GetBytes(input));
		}

		public static string Calculate(byte[] input)
		{
			byte[] numArray = MD5.Create().ComputeHash(input);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < (int)numArray.Length; i++)
			{
				stringBuilder.Append(numArray[i].ToString("x2"));
			}
			return stringBuilder.ToString();
		}
	}
}