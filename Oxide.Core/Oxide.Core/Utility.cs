using Newtonsoft.Json;
using Oxide.Core.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Oxide.Core
{
	public class Utility
	{
		public Utility()
		{
		}

		public static string CleanPath(string path)
		{
			if (path == null)
			{
				return null;
			}
			return path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
		}

		public static T ConvertFromJson<T>(string jsonstr)
		{
			return JsonConvert.DeserializeObject<T>(jsonstr);
		}

		public static string ConvertToJson(object obj, bool indented = false)
		{
			return JsonConvert.SerializeObject(obj, (indented ? Formatting.Indented : Formatting.None));
		}

		public static void DatafileToProto<T>(string name, bool deleteAfter = true)
		{
			DataFileSystem dataFileSystem = Interface.Oxide.DataFileSystem;
			if (!dataFileSystem.ExistsDatafile(name))
			{
				return;
			}
			if (ProtoStorage.Exists(new string[] { name }))
			{
				Interface.Oxide.LogWarning("Failed to import JSON file: {0} already exists.", new object[] { name });
				return;
			}
			try
			{
				ProtoStorage.Save<T>(dataFileSystem.ReadObject<T>(name), new string[] { name });
				if (deleteAfter)
				{
					File.Delete(dataFileSystem.GetFile(name).Filename);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Interface.Oxide.LogException(string.Concat("Failed to convert datafile to proto storage: ", name), exception);
			}
		}

		public static string FormatBytes(double bytes)
		{
			string str;
			if (bytes > 1048576)
			{
				str = "mb";
				bytes /= 1048576;
			}
			else if (bytes <= 1024)
			{
				str = "b";
			}
			else
			{
				str = "kb";
				bytes /= 1024;
			}
			return string.Format("{0:0}{1}", bytes, str);
		}

		public static string GetDirectoryName(string name)
		{
			string str;
			try
			{
				name = name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
				str = name.Substring(0, name.LastIndexOf(Path.DirectorySeparatorChar));
			}
			catch
			{
				str = null;
			}
			return str;
		}

		public static string GetFileNameWithoutExtension(string value)
		{
			int length = value.Length - 1;
			int num = length;
			while (num >= 1)
			{
				if (value[num] != '.')
				{
					num--;
				}
				else
				{
					length = num - 1;
					break;
				}
			}
			int num1 = 0;
			int num2 = length - 1;
			while (num2 >= 0)
			{
				char chr = value[num2];
				if (chr == '/' || chr == '\\')
				{
					num1 = num2 + 1;
					break;
				}
				else
				{
					num2--;
				}
			}
			return value.Substring(num1, length - num1 + 1);
		}

		public static IPAddress GetLocalIP()
		{
			IPAddress address;
			UnicastIPAddressInformation unicastIPAddressInformation = null;
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			for (int i = 0; i < (int)allNetworkInterfaces.Length; i++)
			{
				NetworkInterface networkInterface = allNetworkInterfaces[i];
				if (networkInterface.OperationalStatus == OperationalStatus.Up)
				{
					IPInterfaceProperties pProperties = networkInterface.GetIPProperties();
					if (pProperties.GatewayAddresses.Count != 0 && !pProperties.GatewayAddresses[0].Address.Equals(IPAddress.Parse("0.0.0.0")))
					{
						using (IEnumerator<UnicastIPAddressInformation> enumerator = pProperties.UnicastAddresses.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								UnicastIPAddressInformation current = enumerator.Current;
								if (current.Address.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(current.Address))
								{
									continue;
								}
								if (!current.IsDnsEligible)
								{
									if (unicastIPAddressInformation != null)
									{
										continue;
									}
									unicastIPAddressInformation = current;
								}
								else if (current.PrefixOrigin == PrefixOrigin.Dhcp)
								{
									address = current.Address;
									return address;
								}
								else
								{
									if (unicastIPAddressInformation != null && unicastIPAddressInformation.IsDnsEligible)
									{
										continue;
									}
									unicastIPAddressInformation = current;
								}
							}
							goto Label0;
						}
						return address;
					}
				}
			Label0:
			}
			if (unicastIPAddressInformation == null)
			{
				return null;
			}
			return unicastIPAddressInformation.Address;
		}

		public static int GetNumbers(string input)
		{
			int num;
			int.TryParse(Regex.Replace(input, "[^.0-9]", ""), out num);
			return num;
		}

		public static bool IsLocalIP(string ipAddress)
		{
			string[] strArrays = ipAddress.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
			int[] numArray = new int[] { int.Parse(strArrays[0]), int.Parse(strArrays[1]), int.Parse(strArrays[2]), int.Parse(strArrays[3]) };
			if (numArray[0] == 0 || numArray[0] == 10 || numArray[0] == 127 || numArray[0] == 192 && numArray[1] == 168)
			{
				return true;
			}
			if (numArray[0] != 172 || numArray[1] < 16)
			{
				return false;
			}
			return numArray[1] <= 31;
		}

		public static void PrintCallStack()
		{
			Interface.Oxide.LogDebug("CallStack:{0}{1}", new object[] { Environment.NewLine, new StackTrace(1, true) });
		}

		public static bool ValidateIPv4(string ipAddress)
		{
			if (string.IsNullOrEmpty(ipAddress.Trim()))
			{
				return false;
			}
			string[] strArrays = ipAddress.Replace("\"", string.Empty).Trim().Split(new char[] { '.' });
			if ((int)strArrays.Length != 4)
			{
				return false;
			}
			return strArrays.All<string>((string r) => {
				byte num;
				return byte.TryParse(r, out num);
			});
		}
	}
}