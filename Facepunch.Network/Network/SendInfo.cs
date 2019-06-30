using System;
using System.Collections.Generic;

namespace Network
{
	public struct SendInfo
	{
		public SendMethod method;

		public sbyte channel;

		public Priority priority;

		public IEnumerable<Connection> connections;

		public Connection connection;

		public SendInfo(IEnumerable<Connection> connections)
		{
			this = new SendInfo()
			{
				channel = 0,
				method = SendMethod.Reliable,
				priority = Priority.Medium,
				connections = connections
			};
		}

		public SendInfo(Connection connection)
		{
			this = new SendInfo()
			{
				channel = 0,
				method = SendMethod.Reliable,
				priority = Priority.Medium,
				connection = connection
			};
		}
	}
}