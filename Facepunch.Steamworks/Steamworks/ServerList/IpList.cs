using Steamworks.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.ServerList
{
	public class IpList : Internet
	{
		public List<string> Ips = new List<string>();

		private bool wantsCancel;

		public IpList(IEnumerable<string> list)
		{
			this.Ips.AddRange(list);
		}

		public IpList(params string[] list)
		{
			this.Ips.AddRange(list);
		}

		public override void Cancel()
		{
			this.wantsCancel = true;
		}

		public override async Task<bool> RunQueryAsync(float timeoutSeconds = 10f)
		{
			IpList.<RunQueryAsync>d__4 variable = null;
			AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder = AsyncTaskMethodBuilder<bool>.Create();
			asyncTaskMethodBuilder.Start<IpList.<RunQueryAsync>d__4>(ref variable);
			return asyncTaskMethodBuilder.Task;
		}
	}
}