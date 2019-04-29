using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Oxide.Core.Libraries
{
	[ProtoContract(ImplicitFields=ImplicitFields.AllFields)]
	public class UserData
	{
		public HashSet<string> Groups { get; set; } = new HashSet<string>();

		public string LastSeenNickname { get; set; } = "Unnamed";

		public HashSet<string> Perms { get; set; } = new HashSet<string>();

		public UserData()
		{
		}
	}
}