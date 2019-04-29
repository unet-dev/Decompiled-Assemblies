using Apex.Serialization;
using System;
using UnityEngine;

namespace Rust.Ai
{
	public sealed class PrintDebug : BaseAction
	{
		[ApexSerialization]
		private string debugMessage;

		public PrintDebug()
		{
		}

		public override void DoExecute(BaseContext c)
		{
			Debug.Log(this.debugMessage);
		}
	}
}