using System;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace WebSocketSharp.Net
{
	public class ClientSslConfiguration : SslConfiguration
	{
		private X509CertificateCollection _certs;

		private string _host;

		public X509CertificateCollection ClientCertificates
		{
			get
			{
				return this._certs;
			}
			set
			{
				this._certs = value;
			}
		}

		public LocalCertificateSelectionCallback ClientCertificateSelectionCallback
		{
			get
			{
				return base.CertificateSelectionCallback;
			}
			set
			{
				base.CertificateSelectionCallback = value;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				return base.CertificateValidationCallback;
			}
			set
			{
				base.CertificateValidationCallback = value;
			}
		}

		public string TargetHost
		{
			get
			{
				return this._host;
			}
			set
			{
				this._host = value;
			}
		}

		public ClientSslConfiguration(string targetHost) : this(targetHost, null, SslProtocols.Default, false)
		{
		}

		public ClientSslConfiguration(string targetHost, X509CertificateCollection clientCertificates, SslProtocols enabledSslProtocols, bool checkCertificateRevocation) : base(enabledSslProtocols, checkCertificateRevocation)
		{
			this._host = targetHost;
			this._certs = clientCertificates;
		}
	}
}