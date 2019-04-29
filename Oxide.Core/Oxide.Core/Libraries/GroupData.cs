using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	[ProtoContract(ImplicitFields=ImplicitFields.AllFields)]
	public class GroupData
	{
		public string ParentGroup { get; set; } = string.Empty;

		public HashSet<string> Perms { get; set; } = new HashSet<string>();

		public int Rank
		{
			get;
			set;
		}

		public string Title { get; set; } = string.Empty;

		public GroupData()
		{
		}
	}
}