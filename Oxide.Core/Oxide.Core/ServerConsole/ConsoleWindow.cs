using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Oxide.Core.ServerConsole
{
	public class ConsoleWindow
	{
		private const uint ATTACH_PARENT_PROCESS = 4294967295;

		private const int STD_OUTPUT_HANDLE = -11;

		private TextWriter oldOutput;

		private Encoding oldEncoding;

		public ConsoleWindow()
		{
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool AllocConsole();

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool AttachConsole(uint dwProcessId);

		public static bool Check(bool force = false)
		{
			if (Environment.OSVersion.Platform > PlatformID.Win32NT)
			{
				return false;
			}
			IntPtr moduleHandle = ConsoleWindow.GetModuleHandle("ntdll.dll");
			if (moduleHandle == IntPtr.Zero)
			{
				return false;
			}
			if (ConsoleWindow.GetProcAddress(moduleHandle, "wine_get_version") != IntPtr.Zero)
			{
				return false;
			}
			if (force)
			{
				return true;
			}
			return ConsoleWindow.GetConsoleWindow() == IntPtr.Zero;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool FreeConsole();

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern IntPtr GetConsoleWindow();

		[DllImport("kernel32.dll", CharSet=CharSet.Auto, ExactSpelling=false)]
		private static extern IntPtr GetModuleHandle(string lpModuleName);

		[DllImport("kernel32.dll", CharSet=CharSet.Ansi, ExactSpelling=true, SetLastError=true)]
		private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		public bool Initialize()
		{
			Stream fileStream;
			if (!ConsoleWindow.AttachConsole(-1))
			{
				ConsoleWindow.AllocConsole();
			}
			if (ConsoleWindow.GetConsoleWindow() == IntPtr.Zero)
			{
				ConsoleWindow.FreeConsole();
				return false;
			}
			this.oldOutput = Console.Out;
			this.oldEncoding = Console.OutputEncoding;
			UTF8Encoding uTF8Encoding = new UTF8Encoding(false);
			ConsoleWindow.SetConsoleOutputCP((uint)uTF8Encoding.CodePage);
			Console.OutputEncoding = uTF8Encoding;
			try
			{
				fileStream = new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), FileAccess.Write);
			}
			catch (Exception exception)
			{
				fileStream = Console.OpenStandardOutput();
			}
			Console.SetOut(new StreamWriter(fileStream, uTF8Encoding)
			{
				AutoFlush = true
			});
			return true;
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetConsoleOutputCP(uint wCodePageId);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetConsoleTitle(string lpConsoleTitle);

		public void SetTitle(string title)
		{
			if (title != null)
			{
				ConsoleWindow.SetConsoleTitle(title);
			}
		}

		public void Shutdown()
		{
			if (this.oldOutput != null)
			{
				Console.SetOut(this.oldOutput);
			}
			if (this.oldEncoding != null)
			{
				ConsoleWindow.SetConsoleOutputCP((uint)this.oldEncoding.CodePage);
				Console.OutputEncoding = this.oldEncoding;
			}
			ConsoleWindow.FreeConsole();
		}
	}
}