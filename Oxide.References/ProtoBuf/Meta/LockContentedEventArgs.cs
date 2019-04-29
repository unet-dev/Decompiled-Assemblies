using System;

namespace ProtoBuf.Meta
{
	public sealed class LockContentedEventArgs : EventArgs
	{
		private readonly string ownerStackTrace;

		public string OwnerStackTrace
		{
			get
			{
				return this.ownerStackTrace;
			}
		}

		internal LockContentedEventArgs(string ownerStackTrace)
		{
			this.ownerStackTrace = ownerStackTrace;
		}
	}
}