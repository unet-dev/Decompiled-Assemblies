using Facepunch.Steamworks;
using System;

namespace SteamNative
{
	internal class SteamHTTP : IDisposable
	{
		internal Platform.Interface platform;

		internal BaseSteamworks steamworks;

		public bool IsValid
		{
			get
			{
				if (this.platform == null)
				{
					return false;
				}
				return this.platform.IsValid;
			}
		}

		internal SteamHTTP(BaseSteamworks steamworks, IntPtr pointer)
		{
			this.steamworks = steamworks;
			if (Platform.IsWindows64)
			{
				this.platform = new Platform.Win64(pointer);
				return;
			}
			if (Platform.IsWindows32)
			{
				this.platform = new Platform.Win32(pointer);
				return;
			}
			if (Platform.IsLinux32)
			{
				this.platform = new Platform.Linux32(pointer);
				return;
			}
			if (Platform.IsLinux64)
			{
				this.platform = new Platform.Linux64(pointer);
				return;
			}
			if (Platform.IsOsx)
			{
				this.platform = new Platform.Mac(pointer);
			}
		}

		public HTTPCookieContainerHandle CreateCookieContainer(bool bAllowResponsesToModify)
		{
			return this.platform.ISteamHTTP_CreateCookieContainer(bAllowResponsesToModify);
		}

		public HTTPRequestHandle CreateHTTPRequest(HTTPMethod eHTTPRequestMethod, string pchAbsoluteURL)
		{
			return this.platform.ISteamHTTP_CreateHTTPRequest(eHTTPRequestMethod, pchAbsoluteURL);
		}

		public bool DeferHTTPRequest(HTTPRequestHandle hRequest)
		{
			return this.platform.ISteamHTTP_DeferHTTPRequest(hRequest.Value);
		}

		public virtual void Dispose()
		{
			if (this.platform != null)
			{
				this.platform.Dispose();
				this.platform = null;
			}
		}

		public bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, out float pflPercentOut)
		{
			return this.platform.ISteamHTTP_GetHTTPDownloadProgressPct(hRequest.Value, out pflPercentOut);
		}

		public bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, ref bool pbWasTimedOut)
		{
			return this.platform.ISteamHTTP_GetHTTPRequestWasTimedOut(hRequest.Value, ref pbWasTimedOut);
		}

		public bool GetHTTPResponseBodyData(HTTPRequestHandle hRequest, out byte pBodyDataBuffer, uint unBufferSize)
		{
			return this.platform.ISteamHTTP_GetHTTPResponseBodyData(hRequest.Value, out pBodyDataBuffer, unBufferSize);
		}

		public bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, out uint unBodySize)
		{
			return this.platform.ISteamHTTP_GetHTTPResponseBodySize(hRequest.Value, out unBodySize);
		}

		public bool GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, string pchHeaderName, out uint unResponseHeaderSize)
		{
			return this.platform.ISteamHTTP_GetHTTPResponseHeaderSize(hRequest.Value, pchHeaderName, out unResponseHeaderSize);
		}

		public bool GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, out byte pHeaderValueBuffer, uint unBufferSize)
		{
			return this.platform.ISteamHTTP_GetHTTPResponseHeaderValue(hRequest.Value, pchHeaderName, out pHeaderValueBuffer, unBufferSize);
		}

		public bool GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, out byte pBodyDataBuffer, uint unBufferSize)
		{
			return this.platform.ISteamHTTP_GetHTTPStreamingResponseBodyData(hRequest.Value, cOffset, out pBodyDataBuffer, unBufferSize);
		}

		public bool PrioritizeHTTPRequest(HTTPRequestHandle hRequest)
		{
			return this.platform.ISteamHTTP_PrioritizeHTTPRequest(hRequest.Value);
		}

		public bool ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer)
		{
			return this.platform.ISteamHTTP_ReleaseCookieContainer(hCookieContainer.Value);
		}

		public bool ReleaseHTTPRequest(HTTPRequestHandle hRequest)
		{
			return this.platform.ISteamHTTP_ReleaseHTTPRequest(hRequest.Value);
		}

		public bool SendHTTPRequest(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
		{
			return this.platform.ISteamHTTP_SendHTTPRequest(hRequest.Value, ref pCallHandle.Value);
		}

		public bool SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
		{
			return this.platform.ISteamHTTP_SendHTTPRequestAndStreamResponse(hRequest.Value, ref pCallHandle.Value);
		}

		public bool SetCookie(HTTPCookieContainerHandle hCookieContainer, string pchHost, string pchUrl, string pchCookie)
		{
			return this.platform.ISteamHTTP_SetCookie(hCookieContainer.Value, pchHost, pchUrl, pchCookie);
		}

		public bool SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS(hRequest.Value, unMilliseconds);
		}

		public bool SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestContextValue(hRequest.Value, ulContextValue);
		}

		public bool SetHTTPRequestCookieContainer(HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestCookieContainer(hRequest.Value, hCookieContainer.Value);
		}

		public bool SetHTTPRequestGetOrPostParameter(HTTPRequestHandle hRequest, string pchParamName, string pchParamValue)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestGetOrPostParameter(hRequest.Value, pchParamName, pchParamValue);
		}

		public bool SetHTTPRequestHeaderValue(HTTPRequestHandle hRequest, string pchHeaderName, string pchHeaderValue)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestHeaderValue(hRequest.Value, pchHeaderName, pchHeaderValue);
		}

		public bool SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestNetworkActivityTimeout(hRequest.Value, unTimeoutSeconds);
		}

		public bool SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, string pchContentType, out byte pubBody, uint unBodyLen)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestRawPostBody(hRequest.Value, pchContentType, out pubBody, unBodyLen);
		}

		public bool SetHTTPRequestRequiresVerifiedCertificate(HTTPRequestHandle hRequest, bool bRequireVerifiedCertificate)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate(hRequest.Value, bRequireVerifiedCertificate);
		}

		public bool SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, string pchUserAgentInfo)
		{
			return this.platform.ISteamHTTP_SetHTTPRequestUserAgentInfo(hRequest.Value, pchUserAgentInfo);
		}
	}
}