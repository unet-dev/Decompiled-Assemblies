using System;
using System.Runtime.CompilerServices;

namespace ObjectStream.Data
{
	[Serializable]
	internal class CompilerMessage
	{
		public object Data
		{
			get;
			set;
		}

		public object ExtraData
		{
			get;
			set;
		}

		public int Id
		{
			get;
			set;
		}

		public CompilerMessageType Type
		{
			get;
			set;
		}

		public CompilerMessage()
		{
		}
	}
}