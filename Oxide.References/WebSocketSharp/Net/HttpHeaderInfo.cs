using System;

namespace WebSocketSharp.Net
{
	internal class HttpHeaderInfo
	{
		private string _name;

		private HttpHeaderType _type;

		internal bool IsMultiValueInRequest
		{
			get
			{
				return (this._type & HttpHeaderType.MultiValueInRequest) == HttpHeaderType.MultiValueInRequest;
			}
		}

		internal bool IsMultiValueInResponse
		{
			get
			{
				return (this._type & HttpHeaderType.MultiValueInResponse) == HttpHeaderType.MultiValueInResponse;
			}
		}

		public bool IsRequest
		{
			get
			{
				return (this._type & HttpHeaderType.Request) == HttpHeaderType.Request;
			}
		}

		public bool IsResponse
		{
			get
			{
				return (this._type & HttpHeaderType.Response) == HttpHeaderType.Response;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
		}

		public HttpHeaderType Type
		{
			get
			{
				return this._type;
			}
		}

		internal HttpHeaderInfo(string name, HttpHeaderType type)
		{
			this._name = name;
			this._type = type;
		}

		public bool IsMultiValue(bool response)
		{
			bool flag;
			if ((this._type & HttpHeaderType.MultiValue) == HttpHeaderType.MultiValue)
			{
				flag = (response ? this.IsResponse : this.IsRequest);
			}
			else
			{
				flag = (response ? this.IsMultiValueInResponse : this.IsMultiValueInRequest);
			}
			return flag;
		}

		public bool IsRestricted(bool response)
		{
			bool flag;
			if ((this._type & HttpHeaderType.Restricted) == HttpHeaderType.Restricted)
			{
				flag = (response ? this.IsResponse : this.IsRequest);
			}
			else
			{
				flag = false;
			}
			return flag;
		}
	}
}