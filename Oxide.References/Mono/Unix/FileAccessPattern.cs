using System;

namespace Mono.Unix
{
	public enum FileAccessPattern
	{
		Normal = 0,
		Random = 1,
		Sequential = 2,
		PreLoad = 3,
		FlushCache = 4,
		NoReuse = 5
	}
}