using Mono.Unix.Native;
using System;
using System.Text;

namespace Mono.Unix
{
	public sealed class UnixPath
	{
		public readonly static char DirectorySeparatorChar;

		public readonly static char AltDirectorySeparatorChar;

		public readonly static char PathSeparator;

		public readonly static char VolumeSeparatorChar;

		private readonly static char[] _InvalidPathChars;

		static UnixPath()
		{
			UnixPath.DirectorySeparatorChar = '/';
			UnixPath.AltDirectorySeparatorChar = '/';
			UnixPath.PathSeparator = ':';
			UnixPath.VolumeSeparatorChar = '/';
			UnixPath._InvalidPathChars = new char[0];
		}

		private UnixPath()
		{
		}

		private static string _GetFullPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			if (!UnixPath.IsPathRooted(path))
			{
				path = string.Concat(UnixDirectoryInfo.GetCurrentDirectory(), UnixPath.DirectorySeparatorChar, path);
			}
			return path;
		}

		internal static void CheckPath(string path)
		{
			if (path == null)
			{
				throw new ArgumentNullException();
			}
			if (path.Length == 0)
			{
				throw new ArgumentException("Path cannot contain a zero-length string", "path");
			}
			if (path.IndexOfAny(UnixPath._InvalidPathChars) != -1)
			{
				throw new ArgumentException("Invalid characters in path.", "path");
			}
		}

		public static string Combine(string path1, params string[] paths)
		{
			if (path1 == null)
			{
				throw new ArgumentNullException("path1");
			}
			if (paths == null)
			{
				throw new ArgumentNullException("paths");
			}
			if (path1.IndexOfAny(UnixPath._InvalidPathChars) != -1)
			{
				throw new ArgumentException("Illegal characters in path", "path1");
			}
			int length = path1.Length;
			int num = -1;
			for (int i = 0; i < (int)paths.Length; i++)
			{
				if (paths[i] == null)
				{
					throw new ArgumentNullException(string.Concat("paths[", i, "]"));
				}
				if (paths[i].IndexOfAny(UnixPath._InvalidPathChars) != -1)
				{
					throw new ArgumentException("Illegal characters in path", string.Concat("paths[", i, "]"));
				}
				if (UnixPath.IsPathRooted(paths[i]))
				{
					length = 0;
					num = i;
				}
				length = length + paths[i].Length + 1;
			}
			StringBuilder stringBuilder = new StringBuilder(length);
			if (num == -1)
			{
				stringBuilder.Append(path1);
				num = 0;
			}
			for (int j = num; j < (int)paths.Length; j++)
			{
				UnixPath.Combine(stringBuilder, paths[j]);
			}
			return stringBuilder.ToString();
		}

		private static void Combine(StringBuilder path, string part)
		{
			if (path.Length > 0 && part.Length > 0)
			{
				char chars = path[path.Length - 1];
				if (chars != UnixPath.DirectorySeparatorChar && chars != UnixPath.AltDirectorySeparatorChar && chars != UnixPath.VolumeSeparatorChar)
				{
					path.Append(UnixPath.DirectorySeparatorChar);
				}
			}
			path.Append(part);
		}

		public static string GetCanonicalPath(string path)
		{
			string[] strArrays;
			int num;
			UnixPath.GetPathComponents(path, out strArrays, out num);
			string str = string.Join("/", strArrays, 0, num);
			return (!UnixPath.IsPathRooted(path) ? str : string.Concat("/", str));
		}

		public static string GetCompleteRealPath(string path)
		{
			string[] strArrays;
			int num;
			if (path == null)
			{
				throw new ArgumentNullException("path");
			}
			UnixPath.GetPathComponents(path, out strArrays, out num);
			StringBuilder stringBuilder = new StringBuilder();
			if ((int)strArrays.Length > 0)
			{
				string str = string.Concat((!UnixPath.IsPathRooted(path) ? string.Empty : "/"), strArrays[0]);
				stringBuilder.Append(UnixPath.GetRealPath(str));
			}
			for (int i = 1; i < num; i++)
			{
				stringBuilder.Append("/").Append(strArrays[i]);
				string realPath = UnixPath.GetRealPath(stringBuilder.ToString());
				stringBuilder.Remove(0, stringBuilder.Length);
				stringBuilder.Append(realPath);
			}
			return stringBuilder.ToString();
		}

		public static string GetDirectoryName(string path)
		{
			UnixPath.CheckPath(path);
			int num = path.LastIndexOf(UnixPath.DirectorySeparatorChar);
			if (num > 0)
			{
				return path.Substring(0, num);
			}
			if (num == 0)
			{
				return "/";
			}
			return string.Empty;
		}

		public static string GetFileName(string path)
		{
			if (path == null || path.Length == 0)
			{
				return path;
			}
			int num = path.LastIndexOf(UnixPath.DirectorySeparatorChar);
			if (num < 0)
			{
				return path;
			}
			return path.Substring(num + 1);
		}

		public static string GetFullPath(string path)
		{
			path = UnixPath._GetFullPath(path);
			return UnixPath.GetCanonicalPath(path);
		}

		public static char[] GetInvalidPathChars()
		{
			return (char[])UnixPath._InvalidPathChars.Clone();
		}

		private static void GetPathComponents(string path, out string[] components, out int lastIndex)
		{
			string[] strArrays = path.Split(new char[] { UnixPath.DirectorySeparatorChar });
			int num = 0;
			for (int i = 0; i < (int)strArrays.Length; i++)
			{
				if (!(strArrays[i] == ".") && !(strArrays[i] == string.Empty))
				{
					if (strArrays[i] != "..")
					{
						int num1 = num;
						num = num1 + 1;
						strArrays[num1] = strArrays[i];
					}
					else if (num == 0)
					{
						num++;
					}
					else
					{
						num--;
					}
				}
			}
			components = strArrays;
			lastIndex = num;
		}

		public static string GetPathRoot(string path)
		{
			if (path == null)
			{
				return null;
			}
			if (!UnixPath.IsPathRooted(path))
			{
				return string.Empty;
			}
			return "/";
		}

		public static string GetRealPath(string path)
		{
			while (true)
			{
				string str = UnixPath.ReadSymbolicLink(path);
				if (str == null)
				{
					break;
				}
				if (!UnixPath.IsPathRooted(str))
				{
					path = string.Concat(UnixPath.GetDirectoryName(path), UnixPath.DirectorySeparatorChar, str);
					path = UnixPath.GetCanonicalPath(path);
				}
				else
				{
					path = str;
				}
			}
			return path;
		}

		public static bool IsPathRooted(string path)
		{
			if (path == null || path.Length == 0)
			{
				return false;
			}
			return path[0] == UnixPath.DirectorySeparatorChar;
		}

		public static string ReadLink(string path)
		{
			Errno errno;
			path = UnixPath.ReadSymbolicLink(path, out errno);
			if ((int)errno != 0)
			{
				UnixMarshal.ThrowExceptionForError(errno);
			}
			return path;
		}

		internal static string ReadSymbolicLink(string path)
		{
			int num;
			StringBuilder stringBuilder = new StringBuilder(256);
			while (true)
			{
				num = Syscall.readlink(path, stringBuilder);
				if (num >= 0)
				{
					if (num != stringBuilder.Capacity)
					{
						break;
					}
					StringBuilder capacity = stringBuilder;
					capacity.Capacity = capacity.Capacity * 2;
				}
				else
				{
					Errno lastError = Stdlib.GetLastError();
					Errno errno = lastError;
					if (lastError == Errno.EINVAL)
					{
						return null;
					}
					UnixMarshal.ThrowExceptionForError(errno);
				}
			}
			return stringBuilder.ToString(0, num);
		}

		private static string ReadSymbolicLink(string path, out Errno errno)
		{
			int num;
			errno = (Errno)0;
			StringBuilder stringBuilder = new StringBuilder(256);
			while (true)
			{
				num = Syscall.readlink(path, stringBuilder);
				if (num < 0)
				{
					errno = Stdlib.GetLastError();
					return null;
				}
				if (num != stringBuilder.Capacity)
				{
					break;
				}
				StringBuilder capacity = stringBuilder;
				capacity.Capacity = capacity.Capacity * 2;
			}
			return stringBuilder.ToString(0, num);
		}

		public static string TryReadLink(string path)
		{
			Errno errno;
			return UnixPath.ReadSymbolicLink(path, out errno);
		}
	}
}