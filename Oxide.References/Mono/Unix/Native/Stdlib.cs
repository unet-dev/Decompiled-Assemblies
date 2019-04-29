using Mono.Unix;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Mono.Unix.Native
{
	public class Stdlib
	{
		internal const string LIBC = "msvcrt";

		internal const string MPH = "MonoPosixHelper";

		private readonly static IntPtr _SIG_DFL;

		private readonly static IntPtr _SIG_ERR;

		private readonly static IntPtr _SIG_IGN;

		[CLSCompliant(false)]
		public readonly static SignalHandler SIG_DFL;

		[CLSCompliant(false)]
		public readonly static SignalHandler SIG_ERR;

		[CLSCompliant(false)]
		public readonly static SignalHandler SIG_IGN;

		private readonly static SignalHandler[] registered_signals;

		[CLSCompliant(false)]
		public readonly static int _IOFBF;

		[CLSCompliant(false)]
		public readonly static int _IOLBF;

		[CLSCompliant(false)]
		public readonly static int _IONBF;

		[CLSCompliant(false)]
		public readonly static int BUFSIZ;

		[CLSCompliant(false)]
		public readonly static int EOF;

		[CLSCompliant(false)]
		public readonly static int FOPEN_MAX;

		[CLSCompliant(false)]
		public readonly static int FILENAME_MAX;

		[CLSCompliant(false)]
		public readonly static int L_tmpnam;

		public readonly static IntPtr stderr;

		public readonly static IntPtr stdin;

		public readonly static IntPtr stdout;

		[CLSCompliant(false)]
		public readonly static int TMP_MAX;

		private static object tmpnam_lock;

		[CLSCompliant(false)]
		public readonly static int EXIT_FAILURE;

		[CLSCompliant(false)]
		public readonly static int EXIT_SUCCESS;

		[CLSCompliant(false)]
		public readonly static int MB_CUR_MAX;

		[CLSCompliant(false)]
		public readonly static int RAND_MAX;

		private static object strerror_lock;

		static Stdlib()
		{
			Stdlib._SIG_DFL = Stdlib.GetDefaultSignal();
			Stdlib._SIG_ERR = Stdlib.GetErrorSignal();
			Stdlib._SIG_IGN = Stdlib.GetIgnoreSignal();
			Stdlib.SIG_DFL = new SignalHandler(Stdlib._DefaultHandler);
			Stdlib.SIG_ERR = new SignalHandler(Stdlib._ErrorHandler);
			Stdlib.SIG_IGN = new SignalHandler(Stdlib._IgnoreHandler);
			Stdlib._IOFBF = Stdlib.GetFullyBuffered();
			Stdlib._IOLBF = Stdlib.GetLineBuffered();
			Stdlib._IONBF = Stdlib.GetNonBuffered();
			Stdlib.BUFSIZ = Stdlib.GetBufferSize();
			Stdlib.EOF = Stdlib.GetEOF();
			Stdlib.FOPEN_MAX = Stdlib.GetFopenMax();
			Stdlib.FILENAME_MAX = Stdlib.GetFilenameMax();
			Stdlib.L_tmpnam = Stdlib.GetTmpnamLength();
			Stdlib.stderr = Stdlib.GetStandardError();
			Stdlib.stdin = Stdlib.GetStandardInput();
			Stdlib.stdout = Stdlib.GetStandardOutput();
			Stdlib.TMP_MAX = Stdlib.GetTmpMax();
			Stdlib.tmpnam_lock = new object();
			Stdlib.EXIT_FAILURE = Stdlib.GetExitFailure();
			Stdlib.EXIT_SUCCESS = Stdlib.GetExitSuccess();
			Stdlib.MB_CUR_MAX = Stdlib.GetMbCurMax();
			Stdlib.RAND_MAX = Stdlib.GetRandMax();
			Stdlib.strerror_lock = new object();
			Array values = Enum.GetValues(typeof(Signum));
			Stdlib.registered_signals = new SignalHandler[(int)values.GetValue(values.Length - 1)];
		}

		internal Stdlib()
		{
		}

		private static void _DefaultHandler(int signum)
		{
			Console.Error.WriteLine(string.Concat("Default handler invoked for signum ", signum, ".  Don't do that."));
		}

		private static void _ErrorHandler(int signum)
		{
			Console.Error.WriteLine(string.Concat("Error handler invoked for signum ", signum, ".  Don't do that."));
		}

		[CLSCompliant(false)]
		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void _Exit(int status);

		private static void _IgnoreHandler(int signum)
		{
			Console.Error.WriteLine(string.Concat("Ignore handler invoked for signum ", signum, ".  Don't do that."));
		}

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void abort();

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_calloc", ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr calloc(ulong nmemb, ulong size);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_clearerr", ExactSpelling=false, SetLastError=true)]
		public static extern int clearerr(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_CreateFilePosition", ExactSpelling=false)]
		internal static extern IntPtr CreateFilePosition();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_DumpFilePosition", ExactSpelling=false)]
		internal static extern int DumpFilePosition(StringBuilder buf, HandleRef handle, int len);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void exit(int status);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fclose(IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int feof(IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int ferror(IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fflush(IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fgetc(IntPtr stream);

		public static int fgetpos(IntPtr stream, FilePosition pos)
		{
			return Stdlib.sys_fgetpos(stream, pos.Handle);
		}

		public static StringBuilder fgets(StringBuilder sb, int size, IntPtr stream)
		{
			if (Stdlib.sys_fgets(sb, size, stream) == IntPtr.Zero)
			{
				return null;
			}
			return sb;
		}

		public static StringBuilder fgets(StringBuilder sb, IntPtr stream)
		{
			return Stdlib.fgets(sb, sb.Capacity, stream);
		}

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr fopen(string path, string mode);

		public static int fprintf(IntPtr stream, string message)
		{
			return Stdlib.sys_fprintf(stream, "%s", message);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse fprintf (IntPtr, string) instead.")]
		public static int fprintf(IntPtr stream, string format, params object[] parameters)
		{
			object[] objArray = new object[checked((int)parameters.Length + 2)];
			objArray[0] = stream;
			objArray[1] = format;
			Array.Copy(parameters, 0, objArray, 2, (int)parameters.Length);
			return (int)XPrintfFunctions.fprintf(objArray);
		}

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fputc(int c, IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int fputs(string s, IntPtr stream);

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_fread", ExactSpelling=false, SetLastError=true)]
		public static extern ulong fread(IntPtr ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public static unsafe ulong fread(void* ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			return Stdlib.fread((IntPtr)ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fread(byte[] ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			if (size * nmemb > (long)((int)ptr.Length))
			{
				throw new ArgumentOutOfRangeException("nmemb");
			}
			return Stdlib.sys_fread(ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fread(byte[] ptr, IntPtr stream)
		{
			return Stdlib.fread(ptr, (ulong)1, (ulong)((int)ptr.Length), stream);
		}

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void free(IntPtr ptr);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr freopen(string path, string mode, IntPtr stream);

		[CLSCompliant(false)]
		public static int fseek(IntPtr stream, long offset, SeekFlags origin)
		{
			return Stdlib.sys_fseek(stream, offset, NativeConvert.FromSeekFlags(origin));
		}

		public static int fsetpos(IntPtr stream, FilePosition pos)
		{
			return Stdlib.sys_fsetpos(stream, pos.Handle);
		}

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_ftell", ExactSpelling=false, SetLastError=true)]
		public static extern long ftell(IntPtr stream);

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_fwrite", ExactSpelling=false, SetLastError=true)]
		public static extern ulong fwrite(IntPtr ptr, ulong size, ulong nmemb, IntPtr stream);

		[CLSCompliant(false)]
		public static unsafe ulong fwrite(void* ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			return Stdlib.fwrite((IntPtr)ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fwrite(byte[] ptr, ulong size, ulong nmemb, IntPtr stream)
		{
			if (size * nmemb > (long)((int)ptr.Length))
			{
				throw new ArgumentOutOfRangeException("nmemb");
			}
			return Stdlib.sys_fwrite(ptr, size, nmemb, stream);
		}

		[CLSCompliant(false)]
		public static ulong fwrite(byte[] ptr, IntPtr stream)
		{
			return Stdlib.fwrite(ptr, (ulong)1, (ulong)((int)ptr.Length), stream);
		}

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_BUFSIZ", ExactSpelling=false)]
		private static extern int GetBufferSize();

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getc(IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int getchar();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_SIG_DFL", ExactSpelling=false)]
		private static extern IntPtr GetDefaultSignal();

		public static string getenv(string name)
		{
			return UnixMarshal.PtrToString(Stdlib.sys_getenv(name));
		}

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_EOF", ExactSpelling=false)]
		private static extern int GetEOF();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_SIG_ERR", ExactSpelling=false)]
		private static extern IntPtr GetErrorSignal();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_EXIT_FAILURE", ExactSpelling=false)]
		private static extern int GetExitFailure();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_EXIT_SUCCESS", ExactSpelling=false)]
		private static extern int GetExitSuccess();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_FILENAME_MAX", ExactSpelling=false)]
		private static extern int GetFilenameMax();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_FOPEN_MAX", ExactSpelling=false)]
		private static extern int GetFopenMax();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib__IOFBF", ExactSpelling=false)]
		private static extern int GetFullyBuffered();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_SIG_IGN", ExactSpelling=false)]
		private static extern IntPtr GetIgnoreSignal();

		public static Errno GetLastError()
		{
			return NativeConvert.ToErrno(Marshal.GetLastWin32Error());
		}

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib__IOLBF", ExactSpelling=false)]
		private static extern int GetLineBuffered();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_MB_CUR_MAX", ExactSpelling=false)]
		private static extern int GetMbCurMax();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib__IONBF", ExactSpelling=false)]
		private static extern int GetNonBuffered();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_RAND_MAX", ExactSpelling=false)]
		private static extern int GetRandMax();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_stderr", ExactSpelling=false)]
		private static extern IntPtr GetStandardError();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_stdin", ExactSpelling=false)]
		private static extern IntPtr GetStandardInput();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_stdout", ExactSpelling=false)]
		private static extern IntPtr GetStandardOutput();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_TMP_MAX", ExactSpelling=false)]
		private static extern int GetTmpMax();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_L_tmpnam", ExactSpelling=false)]
		private static extern int GetTmpnamLength();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_InvokeSignalHandler", ExactSpelling=false)]
		internal static extern void InvokeSignalHandler(int signum, IntPtr handler);

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_malloc", ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr malloc(ulong size);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_perror", ExactSpelling=false, SetLastError=true)]
		private static extern int perror(string s, int err);

		public static int perror(string s)
		{
			return Stdlib.perror(s, Marshal.GetLastWin32Error());
		}

		public static int printf(string message)
		{
			return Stdlib.sys_printf("%s", message);
		}

		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse printf (string) instead.")]
		public static int printf(string format, params object[] parameters)
		{
			object[] objArray = new object[checked((int)parameters.Length + 1)];
			objArray[0] = format;
			Array.Copy(parameters, 0, objArray, 1, (int)parameters.Length);
			return (int)XPrintfFunctions.printf(objArray);
		}

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int putc(int c, IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int putchar(int c);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int puts(string s);

		[CLSCompliant(false)]
		public static int raise(Signum sig)
		{
			return Stdlib.sys_raise(NativeConvert.FromSignum(sig));
		}

		public static int raise(RealTimeSignum rts)
		{
			return Stdlib.sys_raise(NativeConvert.FromRealTimeSignum(rts));
		}

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern int rand();

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_realloc", ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr realloc(IntPtr ptr, ulong size);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int @remove(string filename);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int rename(string oldpath, string newpath);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_rewind", ExactSpelling=false, SetLastError=true)]
		public static extern int rewind(IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_setbuf", ExactSpelling=false, SetLastError=true)]
		public static extern int setbuf(IntPtr stream, IntPtr buf);

		[CLSCompliant(false)]
		public static unsafe int setbuf(IntPtr stream, byte* buf)
		{
			return Stdlib.setbuf(stream, (IntPtr)buf);
		}

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_SetLastError", ExactSpelling=false)]
		private static extern void SetLastError(int error);

		protected static void SetLastError(Errno error)
		{
			Stdlib.SetLastError(NativeConvert.FromErrno(error));
		}

		public static int SetSignalAction(Signum signal, SignalAction action)
		{
			return Stdlib.SetSignalAction(NativeConvert.FromSignum(signal), action);
		}

		public static int SetSignalAction(RealTimeSignum rts, SignalAction action)
		{
			return Stdlib.SetSignalAction(NativeConvert.FromRealTimeSignum(rts), action);
		}

		private static int SetSignalAction(int signum, SignalAction action)
		{
			IntPtr zero = IntPtr.Zero;
			switch (action)
			{
				case SignalAction.Default:
				{
					zero = Stdlib._SIG_DFL;
					break;
				}
				case SignalAction.Ignore:
				{
					zero = Stdlib._SIG_IGN;
					break;
				}
				case SignalAction.Error:
				{
					zero = Stdlib._SIG_ERR;
					break;
				}
				default:
				{
					throw new ArgumentException("Invalid action value.", "action");
				}
			}
			if (Stdlib.sys_signal(signum, zero) == Stdlib._SIG_ERR)
			{
				return -1;
			}
			return 0;
		}

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_setvbuf", ExactSpelling=false, SetLastError=true)]
		public static extern int setvbuf(IntPtr stream, IntPtr buf, int mode, ulong size);

		[CLSCompliant(false)]
		public static unsafe int setvbuf(IntPtr stream, byte* buf, int mode, ulong size)
		{
			return Stdlib.setvbuf(stream, (IntPtr)buf, mode, size);
		}

		[CLSCompliant(false)]
		[Obsolete("This is not safe; use Mono.Unix.UnixSignal for signal delivery or SetSignalAction()")]
		public static SignalHandler signal(Signum signum, SignalHandler handler)
		{
			// 
			// Current member / type: Mono.Unix.Native.SignalHandler Mono.Unix.Native.Stdlib::signal(Mono.Unix.Native.Signum,Mono.Unix.Native.SignalHandler)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.References.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: Mono.Unix.Native.SignalHandler signal(Mono.Unix.Native.Signum,Mono.Unix.Native.SignalHandler)
			// 
			// Object reference not set to an instance of an object.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 93
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 24
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 19
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		[CLSCompliant(false)]
		public static int snprintf(StringBuilder s, ulong n, string message)
		{
			if (n > (long)s.Capacity)
			{
				throw new ArgumentOutOfRangeException("n", "n must be <= s.Capacity");
			}
			return Stdlib.sys_snprintf(s, n, "%s", message);
		}

		public static int snprintf(StringBuilder s, string message)
		{
			return Stdlib.sys_snprintf(s, (ulong)s.Capacity, "%s", message);
		}

		[CLSCompliant(false)]
		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse snprintf (StringBuilder, string) instead.")]
		public static int snprintf(StringBuilder s, ulong n, string format, params object[] parameters)
		{
			if (n > (long)s.Capacity)
			{
				throw new ArgumentOutOfRangeException("n", "n must be <= s.Capacity");
			}
			object[] objArray = new object[checked((int)parameters.Length + 3)];
			objArray[0] = s;
			objArray[1] = n;
			objArray[2] = format;
			Array.Copy(parameters, 0, objArray, 3, (int)parameters.Length);
			return (int)XPrintfFunctions.snprintf(objArray);
		}

		[CLSCompliant(false)]
		[Obsolete("Not necessarily portable due to cdecl restrictions.\nUse snprintf (StringBuilder, string) instead.")]
		public static int snprintf(StringBuilder s, string format, params object[] parameters)
		{
			object[] capacity = new object[checked((int)parameters.Length + 3)];
			capacity[0] = s;
			capacity[1] = (ulong)((long)s.Capacity);
			capacity[2] = format;
			Array.Copy(parameters, 0, capacity, 3, (int)parameters.Length);
			return (int)XPrintfFunctions.snprintf(capacity);
		}

		[CLSCompliant(false)]
		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false)]
		public static extern void srand(uint seed);

		[CLSCompliant(false)]
		public static string strerror(Errno errnum)
		{
			string str;
			int num = NativeConvert.FromErrno(errnum);
			object strerrorLock = Stdlib.strerror_lock;
			Monitor.Enter(strerrorLock);
			try
			{
				str = UnixMarshal.PtrToString(Stdlib.sys_strerror(num));
			}
			finally
			{
				Monitor.Exit(strerrorLock);
			}
			return str;
		}

		[CLSCompliant(false)]
		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_strlen", ExactSpelling=false, SetLastError=true)]
		public static extern ulong strlen(IntPtr s);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_fgetpos", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_fgetpos(IntPtr stream, HandleRef pos);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="fgets", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_fgets(StringBuilder sb, int size, IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="fprintf", ExactSpelling=false)]
		private static extern int sys_fprintf(IntPtr stream, string format, string message);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_fread", ExactSpelling=false, SetLastError=true)]
		private static extern ulong sys_fread([Out] byte[] ptr, ulong size, ulong nmemb, IntPtr stream);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_fseek", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_fseek(IntPtr stream, long offset, int origin);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_fsetpos", ExactSpelling=false, SetLastError=true)]
		private static extern int sys_fsetpos(IntPtr stream, HandleRef pos);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_fwrite", ExactSpelling=false, SetLastError=true)]
		private static extern ulong sys_fwrite(byte[] ptr, ulong size, ulong nmemb, IntPtr stream);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="getenv", ExactSpelling=false)]
		private static extern IntPtr sys_getenv(string name);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="printf", ExactSpelling=false)]
		private static extern int sys_printf(string format, string message);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="raise", ExactSpelling=false)]
		private static extern int sys_raise(int sig);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="signal", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_signal(int signum, SignalHandler handler);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="signal", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_signal(int signum, IntPtr handler);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_Stdlib_snprintf", ExactSpelling=false)]
		private static extern int sys_snprintf(StringBuilder s, ulong n, string format, string message);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="strerror", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_strerror(int errnum);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="tmpnam", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr sys_tmpnam(StringBuilder s);

		[CLSCompliant(false)]
		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int system(string @string);

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern IntPtr tmpfile();

		[Obsolete("Syscall.mkstemp() should be preferred.")]
		public static string tmpnam(StringBuilder s)
		{
			// 
			// Current member / type: System.String Mono.Unix.Native.Stdlib::tmpnam(System.Text.StringBuilder)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.References.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.String tmpnam(System.Text.StringBuilder)
			// 
			// Object reference not set to an instance of an object.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 93
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 24
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 19
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		[Obsolete("Syscall.mkstemp() should be preferred.")]
		public static string tmpnam()
		{
			string str;
			object tmpnamLock = Stdlib.tmpnam_lock;
			Monitor.Enter(tmpnamLock);
			try
			{
				str = UnixMarshal.PtrToString(Stdlib.sys_tmpnam(null));
			}
			finally
			{
				Monitor.Exit(tmpnamLock);
			}
			return str;
		}

		private static SignalHandler TranslateHandler(IntPtr handler)
		{
			if (handler == Stdlib._SIG_DFL)
			{
				return Stdlib.SIG_DFL;
			}
			if (handler == Stdlib._SIG_ERR)
			{
				return Stdlib.SIG_ERR;
			}
			if (handler == Stdlib._SIG_IGN)
			{
				return Stdlib.SIG_IGN;
			}
			return (SignalHandler)Marshal.GetDelegateForFunctionPointer(handler, typeof(SignalHandler));
		}

		[DllImport("msvcrt", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, ExactSpelling=false, SetLastError=true)]
		public static extern int ungetc(int c, IntPtr stream);
	}
}