using Mono.Unix.Native;
using System;

namespace Mono.Unix
{
	public sealed class UnixProcess
	{
		private int pid;

		public int ExitCode
		{
			get
			{
				if (!this.HasExited)
				{
					throw new InvalidOperationException(Locale.GetText("Process hasn't exited"));
				}
				return Syscall.WEXITSTATUS(this.GetProcessStatus());
			}
		}

		public bool HasExited
		{
			get
			{
				return Syscall.WIFEXITED(this.GetProcessStatus());
			}
		}

		public bool HasSignaled
		{
			get
			{
				return Syscall.WIFSIGNALED(this.GetProcessStatus());
			}
		}

		public bool HasStopped
		{
			get
			{
				return Syscall.WIFSTOPPED(this.GetProcessStatus());
			}
		}

		public int Id
		{
			get
			{
				return this.pid;
			}
		}

		public int ProcessGroupId
		{
			get
			{
				return Syscall.getpgid(this.pid);
			}
			set
			{
				UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.setpgid(this.pid, value));
			}
		}

		public int SessionId
		{
			get
			{
				int num = Syscall.getsid(this.pid);
				UnixMarshal.ThrowExceptionForLastErrorIf(num);
				return num;
			}
		}

		public Signum StopSignal
		{
			get
			{
				if (!this.HasStopped)
				{
					throw new InvalidOperationException(Locale.GetText("Process isn't stopped"));
				}
				return Syscall.WSTOPSIG(this.GetProcessStatus());
			}
		}

		public Signum TerminationSignal
		{
			get
			{
				if (!this.HasSignaled)
				{
					throw new InvalidOperationException(Locale.GetText("Process wasn't terminated by a signal"));
				}
				return Syscall.WTERMSIG(this.GetProcessStatus());
			}
		}

		internal UnixProcess(int pid)
		{
			this.pid = pid;
		}

		public static UnixProcess GetCurrentProcess()
		{
			return new UnixProcess(UnixProcess.GetCurrentProcessId());
		}

		public static int GetCurrentProcessId()
		{
			return Syscall.getpid();
		}

		private int GetProcessStatus()
		{
			int num;
			int num1 = Syscall.waitpid(this.pid, out num, WaitOptions.WNOHANG | WaitOptions.WUNTRACED);
			UnixMarshal.ThrowExceptionForLastErrorIf(num1);
			return num1;
		}

		public void Kill()
		{
			this.Signal(Signum.SIGKILL);
		}

		[CLSCompliant(false)]
		public void Signal(Signum signal)
		{
			UnixMarshal.ThrowExceptionForLastErrorIf(Syscall.kill(this.pid, signal));
		}

		public void WaitForExit()
		{
			int num;
			int num1;
			do
			{
				num1 = Syscall.waitpid(this.pid, out num, (WaitOptions)0);
			}
			while (UnixMarshal.ShouldRetrySyscall(num1));
			UnixMarshal.ThrowExceptionForLastErrorIf(num1);
		}
	}
}