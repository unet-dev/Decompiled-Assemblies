using System;

namespace WebSocketSharp.Net
{
	[Flags]
	internal enum HttpHeaderType
	{
		Unspecified = 0,
		Request = 1,
		Response = 2,
		Restricted = 4,
		MultiValue = 8,
		MultiValueInRequest = 16,
		MultiValueInResponse = 32
	}
}