using System;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;

namespace Facepunch
{
	internal static class Mono
	{
		internal static void FixHttpsValidation()
		{
			ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => {
				bool flag = true;
				if (sslPolicyErrors != SslPolicyErrors.None)
				{
					for (int i = 0; i < (int)chain.ChainStatus.Length; i++)
					{
						if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
						{
							chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
							chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
							chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
							chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
							if (!chain.Build((X509Certificate2)certificate))
							{
								flag = false;
								break;
							}
						}
					}
				}
				return flag;
			};
		}
	}
}