using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

public static class DirectoryEx
{
	public static void Backup(DirectoryInfo parent, params string[] names)
	{
		for (int i = 0; i < (int)names.Length; i++)
		{
			names[i] = Path.Combine(parent.FullName, names[i]);
		}
		DirectoryEx.Backup(names);
	}

	public static void Backup(params string[] names)
	{
		for (int i = (int)names.Length - 2; i >= 0; i--)
		{
			DirectoryInfo directoryInfo = new DirectoryInfo(names[i]);
			DirectoryInfo directoryInfo1 = new DirectoryInfo(names[i + 1]);
			if (directoryInfo.Exists)
			{
				if (!directoryInfo1.Exists)
				{
					if (!directoryInfo1.Parent.Exists)
					{
						directoryInfo1.Parent.Create();
					}
					directoryInfo.MoveToSafe(directoryInfo1.FullName, 10);
				}
				else
				{
					TimeSpan now = DateTime.Now - directoryInfo1.LastWriteTime;
					if (now.TotalHours >= (double)((i == 0 ? 0 : 1 << (i - 1 & 31))))
					{
						directoryInfo1.Delete(true);
						directoryInfo.MoveToSafe(directoryInfo1.FullName, 10);
					}
				}
			}
		}
	}

	public static void CopyAll(string sourceDirectory, string targetDirectory)
	{
		DirectoryEx.CopyAll(new DirectoryInfo(sourceDirectory), new DirectoryInfo(targetDirectory));
	}

	public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
	{
		int i;
		if (source.FullName.ToLower() == target.FullName.ToLower())
		{
			return;
		}
		if (!source.Exists)
		{
			return;
		}
		if (!target.Exists)
		{
			target.Create();
		}
		FileInfo[] files = source.GetFiles();
		for (i = 0; i < (int)files.Length; i++)
		{
			FileInfo fileInfo = files[i];
			FileInfo creationTime = new FileInfo(Path.Combine(target.FullName, fileInfo.Name));
			fileInfo.CopyTo(creationTime.FullName, true);
			creationTime.CreationTime = fileInfo.CreationTime;
			creationTime.LastAccessTime = fileInfo.LastAccessTime;
			creationTime.LastWriteTime = fileInfo.LastWriteTime;
		}
		DirectoryInfo[] directories = source.GetDirectories();
		for (i = 0; i < (int)directories.Length; i++)
		{
			DirectoryInfo directoryInfo = directories[i];
			DirectoryInfo lastAccessTime = target.CreateSubdirectory(directoryInfo.Name);
			DirectoryEx.CopyAll(directoryInfo, lastAccessTime);
			lastAccessTime.CreationTime = directoryInfo.CreationTime;
			lastAccessTime.LastAccessTime = directoryInfo.LastAccessTime;
			lastAccessTime.LastWriteTime = directoryInfo.LastWriteTime;
		}
	}

	public static bool MoveToSafe(this DirectoryInfo parent, string target, int retries = 10)
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