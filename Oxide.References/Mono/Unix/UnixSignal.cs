using Mono.Unix.Native;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Mono.Unix
{
	public class UnixSignal : WaitHandle
	{
		private int signum;

		private IntPtr signal_info;

		public int Count
		{
			get
			{
				return (*(this.Info)).count;
			}
			set
			{
				Interlocked.Exchange((*(this.Info)).count, value);
			}
		}

		private UnixSignal.SignalInfo* Info
		{
			get
			{
				this.AssertValid();
				return (void*)this.signal_info;
			}
		}

		public bool IsRealTimeSignal
		{
			get
			{
				this.AssertValid();
				int sIGRTMIN = UnixSignal.GetSIGRTMIN();
				if (sIGRTMIN == -1)
				{
					return false;
				}
				return this.signum >= sIGRTMIN;
			}
		}

		public bool IsSet
		{
			get
			{
				return this.Count > 0;
			}
		}

		public Mono.Unix.Native.RealTimeSignum RealTimeSignum
		{
			get
			{
				if (!this.IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is not a RealTimeSignum");
				}
				return NativeConvert.ToRealTimeSignum(this.signum - UnixSignal.GetSIGRTMIN());
			}
		}

		public Mono.Unix.Native.Signum Signum
		{
			get
			{
				if (this.IsRealTimeSignal)
				{
					throw new InvalidOperationException("This signal is a RealTimeSignum");
				}
				return NativeConvert.ToSignum(this.signum);
			}
		}

		public UnixSignal(Mono.Unix.Native.Signum signum)
		{
			this.signum = NativeConvert.FromSignum(signum);
			this.signal_info = UnixSignal.install(this.signum);
			if (this.signal_info == IntPtr.Zero)
			{
				throw new ArgumentException("Unable to handle signal", "signum");
			}
		}

		public UnixSignal(Mono.Unix.Native.RealTimeSignum rtsig)
		{
			this.signum = NativeConvert.FromRealTimeSignum(rtsig);
			this.signal_info = UnixSignal.install(this.signum);
			Errno lastError = Stdlib.GetLastError();
			if (this.signal_info == IntPtr.Zero)
			{
				if (lastError != Errno.EADDRINUSE)
				{
					throw new ArgumentException("Unable to handle signal", "signum");
				}
				throw new ArgumentException("Signal registered outside of Mono.Posix", "signum");
			}
		}

		private void AssertValid()
		{
			if (this.signal_info == IntPtr.Zero)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (this.signal_info == IntPtr.Zero)
			{
				return;
			}
			UnixSignal.uninstall(this.signal_info);
			this.signal_info = IntPtr.Zero;
		}

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_SIGRTMAX", ExactSpelling=false)]
		internal static extern int GetSIGRTMAX();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Posix_SIGRTMIN", ExactSpelling=false)]
		internal static extern int GetSIGRTMIN();

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Unix_UnixSignal_install", ExactSpelling=false, SetLastError=true)]
		private static extern IntPtr install(int signum);

		public bool Reset()
		{
			int num = Interlocked.Exchange((*(this.Info)).count, 0);
			return num != 0;
		}

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Unix_UnixSignal_uninstall", ExactSpelling=false)]
		private static extern int uninstall(IntPtr info);

		[DllImport("MonoPosixHelper", CallingConvention=CallingConvention.Cdecl, CharSet=CharSet.None, EntryPoint="Mono_Unix_UnixSignal_WaitAny", ExactSpelling=false)]
		private static extern int WaitAny(IntPtr[] infos, int count, int timeout, UnixSignal.Mono_Posix_RuntimeIsShuttingDown shutting_down);

		public static int WaitAny(UnixSignal[] signals)
		{
			return UnixSignal.WaitAny(signals, -1);
		}

		public static int WaitAny(UnixSignal[] signals, TimeSpan timeout)
		{
			long totalMilliseconds = (long)timeout.TotalMilliseconds;
			if (totalMilliseconds < (long)-1 || totalMilliseconds > (long)2147483647)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return UnixSignal.WaitAny(signals, (int)totalMilliseconds);
		}

		public static int WaitAny(UnixSignal[] signals, int millisecondsTimeout)
		{
			if (signals == null)
			{
				throw new ArgumentNullException("signals");
			}
			if (millisecondsTimeout < -1)
			{
				throw new ArgumentOutOfRangeException("millisecondsTimeout");
			}
			IntPtr[] signalInfo = new IntPtr[(int)signals.Length];
			for (int i = 0; i < (int)signals.Length; i++)
			{
				signalInfo[i] = signals[i].signal_info;
				if (signalInfo[i] == IntPtr.Zero)
				{
					throw new InvalidOperationException("Disposed UnixSignal");
				}
			}
			return UnixSignal.WaitAny(signalInfo, (int)signalInfo.Length, millisecondsTimeout, () => (!Environment.HasShutdownStarted ? 0 : 1));
		}

		public override bool WaitOne()
		{
			return this.WaitOne(-1, false);
		}

		public override bool WaitOne(TimeSpan timeout, bool exitContext)
		{
			long totalMilliseconds = (long)timeout.TotalMilliseconds;
			if (totalMilliseconds < (long)-1 || totalMilliseconds > (long)2147483647)
			{
				throw new ArgumentOutOfRangeException("timeout");
			}
			return this.WaitOne((int)totalMilliseconds, exitContext);
		}

		public override bool WaitOne(int millisecondsTimeout, bool exitContext)
		{
			this.AssertValid();
			if (exitContext)
			{
				throw new InvalidOperationException("exitContext is not supported");
			}
			return UnixSignal.WaitAny(new UnixSignal[] { this }, millisecondsTimeout) == 0;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		private delegate int Mono_Posix_RuntimeIsShuttingDown();

		[Map]
		private struct SignalInfo
		{
			public int signum;

			public int count;

			public int read_fd;

			public int write_fd;

			public int have_handler;

			public int pipecnt;

			public IntPtr handler;
		}
	}
}