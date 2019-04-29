using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

public static class FileEx
{
	public static void Backup(DirectoryInfo parent, params string[] names)
	{
		for (int i = 0; i < (int)names.Length; i++)
		{
			names[i] = Path.Combine(parent.FullName, names[i]);
		}
		FileEx.Backup(names);
	}

	public static void Backup(params string[] names)
	{
		for (int i = (int)names.Length - 2; i >= 0; i--)
		{
			FileInfo fileInfo = new FileInfo(names[i]);
			FileInfo fileInfo1 = new FileInfo(names[i + 1]);
			if (fileInfo.Exists)
			{
				if (!fileInfo1.Exists)
				{
					if (!fileInfo1.Directory.Exists)
					{
						fileInfo1.Directory.Create();
					}
					fileInfo.MoveToSafe(fileInfo1.FullName, 10);
				}
				else
				{
					TimeSpan now = DateTime.Now - fileInfo1.LastWriteTime;
					if (now.TotalHours >= (double)((i == 0 ? 0 : 1 << (i - 1 & 31))))
					{
						fileInfo1.Delete();
						fileInfo.MoveToSafe(fileInfo1.FullName, 10);
					}
				}
			}
		}
	}

	public static bool MoveToSafe(this FileInfo parent, string target, int retries = 10)
	{
		for (int i = 0; i < retries; i++)
		{
			try
			{
				parent.MoveTo(target);
				return true;
			}
			catch (Exception exception)
			{
				Thread.Sleep(5);
			}
		}
		return false;
	}
}