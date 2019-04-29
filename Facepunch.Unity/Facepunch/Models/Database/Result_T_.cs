using System;

namespace Facepunch.Models.Database
{
	public class Result<T>
	{
		public bool Running;

		public bool Success;

		public Result<T>.Entry[] Entries;

		public Result()
		{
		}

		public class Entry
		{
			public T Content;

			public string Id;

			public DateTime Created;

			public DateTime Updated;

			public string AuthorId;

			public string AuthorName;

			public string AuthType;

			public Entry()
			{
			}
		}
	}
}