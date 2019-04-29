using Oxide.Core;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Net.WebSockets;
using WebSocketSharp.Server;

namespace Facepunch.Rcon
{
	public class Listener
	{
		public string Password;

		public int Port;

		public string Address;

		public string SslCertificate;

		public string SslCertificatePassword;

		public Action<IPEndPoint, string, string> OnMessage;

		private WebSocketServer server;

		public Listener()
		{
		}

		public void BroadcastMessage(string str)
		{
			if (this.server != null)
			{
				this.server.WebSocketServices.Broadcast(str);
			}
		}

		public void SendMessage(string target, string str)
		{
			if (this.server != null)
			{
				foreach (WebSocketServiceHost host in this.server.WebSocketServices.Hosts)
				{
					host.Sessions.SendTo(str, target);
				}
			}
		}

		public void Shutdown()
		{
			if (this.server != null)
			{
				this.server.Stop();
				this.server = null;
			}
		}

		public void Start()
		{
			this.Shutdown();
			bool flag = (string.IsNullOrEmpty(this.SslCertificate) ? false : !string.IsNullOrEmpty(this.SslCertificatePassword));
			if (this.Address == null)
			{
				this.server = new WebSocketServer(this.Port, flag);
			}
			else
			{
				IPAddress any = IPAddress.Any;
				if (!IPAddress.TryParse(this.Address, out any))
				{
					any = IPAddress.Any;
				}
				this.server = new WebSocketServer(any, this.Port, flag);
			}
			if (!string.IsNullOrEmpty(this.SslCertificate) && !string.IsNullOrEmpty(this.SslCertificatePassword))
			{
				X509Certificate2 x509Certificate2 = new X509Certificate2(this.SslCertificate, this.SslCertificatePassword);
				this.server.SslConfiguration.ServerCertificate = x509Certificate2;
			}
			this.server.AddWebSocketService<Listener.RConBehaviour>(string.Concat("/", this.Password), () => new Listener.RConBehaviour()
			{
				IgnoreExtensions = true,
				Parent = this
			});
			this.server.WaitTime = TimeSpan.FromSeconds(5);
			this.server.Start();
		}

		public class RConBehaviour : WebSocketBehavior
		{
			public Listener Parent;

			public RConBehaviour()
			{
			}

			protected override void OnError(ErrorEventArgs e)
			{
				Debug.Log(string.Concat("Rcon Error: ", e.Exception));
				Debug.Log(e.Message);
			}

			protected override void OnMessage(MessageEventArgs e)
			{
				if (this.Parent.OnMessage != null)
				{
					if (Interface.CallHook("IOnRconCommand", this.Context.UserEndPoint, e.Data) != null)
					{
						return;
					}
					this.Parent.OnMessage(base.Context.UserEndPoint, base.ID, e.Data);
				}
			}
		}
	}
}