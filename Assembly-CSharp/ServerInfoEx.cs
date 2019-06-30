using Steamworks.Data;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

internal struct ServerInfoEx
{
	public ServerInfo info;

	public string[] Tags;

	public int Born;

	public ServerInfoEx(ServerInfo server)
	{
		this.info = server;
		this.Tags = this.info.Tags;
		this.Born = 0;
		string str = this.Tags.FirstOrDefault<string>((string x) => x.StartsWith("born"));
		if (str != null)
		{
			int.TryParse(str.Substring(4), out this.Born);
		}
	}
}