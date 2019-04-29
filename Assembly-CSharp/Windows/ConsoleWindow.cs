using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using UnityEngine;

namespace Windows
{
	[SuppressUnmanagedCodeSecurity]
	public class ConsoleWindow
	{
		private TextWriter oldOutput;

		private const int STD_INPUT_HANDLE = -10;

		private const int STD_OUTPUT_HANDLE = -11;

		public ConsoleWindow()
		{
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool AllocConsole();

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool AttachConsole(uint dwProcessId);

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		private static extern bool FreeConsole();

		[DllImport("kernel32.dll", CallingConvention=CallingConvention.StdCall, CharSet=CharSet.Auto, ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr GetStdHandle(int nStdHandle);

		public void Initialize()
		{
			ConsoleWindow.FreeConsole();
			if (!ConsoleWindow.AttachConsole(-1))
			{
				ConsoleWindow.AllocConsole();
			}
			this.oldOutput = Console.Out;
			try
			{
				Console.OutputEncoding = Encoding.UTF8;
				Console.SetOut(new StreamWriter(new FileStream(new SafeFileHandle(ConsoleWindow.GetStdHandle(-11), true), FileAccess.Write), Encoding.UTF8)
				{
					AutoFlush = true
				});
			}
			catch (Exception exception)
			{
				Debug.Log(string.Concat("Couldn't redirect output: ", exception.Message));
			}
		}

		[DllImport("kernel32.dll", CharSet=CharSet.None, ExactSpelling=false)]
		private static extern bool SetConsoleTitleA(string lpConsoleTitle);

		public void SetTitle(string strName)
		{
			ConsoleWindow.SetConsoleTitleA(strName);
		}

		public void Shutdown()
		{
			Console.SetOut(this.oldOutput);
			ConsoleWindow.FreeConsole();
		}
	}
}