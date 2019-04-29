using System;

namespace WebSocketSharp
{
	public class ErrorEventArgs : EventArgs
	{
		private System.Exception _exception;

		private string _message;

		public System.Exception Exception
		{
			get
			{
				return this._exception;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		internal ErrorEventArgs(string message) : this(message, null)
		{
		}

		internal ErrorEventArgs(string message, System.Exception exception)
		{
			this._message = message;
			this._exception = exception;
		}
	}
}