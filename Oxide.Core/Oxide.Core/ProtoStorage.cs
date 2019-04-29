using ProtoBuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Oxide.Core
{
	public class ProtoStorage
	{
		public ProtoStorage()
		{
		}

		public static bool Exists(params string[] subPaths)
		{
			return File.Exists(ProtoStorage.GetFileDataPath(ProtoStorage.GetFileName(subPaths)));
		}

		public static string GetFileDataPath(string name)
		{
			return Path.Combine(Interface.Oxide.DataDirectory, name);
		}

		public static string GetFileName(params string[] subPaths)
		{
			char directorySeparatorChar = Path.DirectorySeparatorChar;
			return string.Concat(string.Join(directorySeparatorChar.ToString(), subPaths).Replace("..", ""), ".data");
		}

		public static IEnumerable<string> GetFiles(string subDirectory)
		{
			string fileDataPath = ProtoStorage.GetFileDataPath(subDirectory.Replace("..", ""));
			if (!Directory.Exists(fileDataPath))
			{
				yield break;
			}
			string[] files = Directory.GetFiles(fileDataPath, "*.data");
			for (int i = 0; i < (int)files.Length; i++)
			{
				yield return Utility.GetFileNameWithoutExtension(files[i]);
			}
			files = null;
		}

		public static T Load<T>(params string[] subPaths)
		{
			T t;
			string fileName = ProtoStorage.GetFileName(subPaths);
			string fileDataPath = ProtoStorage.GetFileDataPath(fileName);
			try
			{
				if (File.Exists(fileDataPath))
				{
					using (FileStream fileStream = File.OpenRead(fileDataPath))
					{
						t = Serializer.Deserialize<T>(fileStream);
					}
					return t;
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Interface.Oxide.LogException(string.Concat("Failed to load protobuf data from ", fileName), exception);
			}
			return default(T);
		}

		public static void Save<T>(T data, params string[] subPaths)
		{
			string fileName = ProtoStorage.GetFileName(subPaths);
			string fileDataPath = ProtoStorage.GetFileDataPath(fileName);
			string directoryName = Path.GetDirectoryName(fileDataPath);
			try
			{
				if (directoryName != null && !Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				using (FileStream fileStream = File.Open(fileDataPath, FileMode.Create))
				{
					Serializer.Serialize<T>(fileStream, data);
				}
			}
			catch (Exception exception1)
			{
				Exception exception = exception1;
				Interface.Oxide.LogException(string.Concat("Failed to save protobuf data to ", fileName), exception);
			}
		}
	}
}