using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace Oxide.Core.Libraries
{
	public class WebRequests : Library
	{
		private readonly static Oxide.Core.Libraries.Covalence.Covalence covalence;

		public static float Timeout;

		public static bool AllowDecompression;

		private readonly Queue<WebRequests.WebRequest> queue = new Queue<WebRequests.WebRequest>();

		private readonly object syncroot = new object();

		private readonly Thread workerthread;

		private readonly AutoResetEvent workevent = new AutoResetEvent(false);

		private bool shutdown;

		private readonly int maxWorkerThreads;

		private readonly int maxCompletionPortThreads;

		static WebRequests()
		{
			WebRequests.covalence = Interface.Oxide.GetLibrary<Oxide.Core.Libraries.Covalence.Covalence>(null);
			WebRequests.Timeout = 30f;
			WebRequests.AllowDecompression = false;
		}

		public WebRequests()
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.ServerCertificateValidationCallback = (object argument0, X509Certificate argument1, X509Chain argument2, SslPolicyErrors argument3) => true;
			ServicePointManager.DefaultConnectionLimit = 200;
			ThreadPool.GetMaxThreads(out this.maxWorkerThreads, out this.maxCompletionPortThreads);
			this.maxCompletionPortThreads = (int)((double)this.maxCompletionPortThreads * 0.6);
			this.maxWorkerThreads = (int)((double)this.maxWorkerThreads * 0.75);
			this.workerthread = new Thread(new ThreadStart(this.Worker));
			this.workerthread.Start();
		}

		[LibraryFunction("Enqueue")]
		public void Enqueue(string url, string body, Action<int, string> callback, Plugin owner, RequestMethod method = 1, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			WebRequests.WebRequest webRequest = new WebRequests.WebRequest(url, callback, owner)
			{
				Method = method.ToString(),
				RequestHeaders = headers,
				Timeout = timeout,
				Body = body
			};
			lock (this.syncroot)
			{
				this.queue.Enqueue(webRequest);
			}
			this.workevent.Set();
		}

		[LibraryFunction("EnqueueGet")]
		[Obsolete("EnqueueGet is deprecated, use Enqueue instead")]
		public void EnqueueGet(string url, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			this.Enqueue(url, null, callback, owner, RequestMethod.GET, headers, timeout);
		}

		[LibraryFunction("EnqueuePost")]
		[Obsolete("EnqueuePost is deprecated, use Enqueue instead")]
		public void EnqueuePost(string url, string body, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			this.Enqueue(url, body, callback, owner, RequestMethod.POST, headers, timeout);
		}

		[LibraryFunction("EnqueuePut")]
		[Obsolete("EnqueuePut is deprecated, use Enqueue instead")]
		public void EnqueuePut(string url, string body, Action<int, string> callback, Plugin owner, Dictionary<string, string> headers = null, float timeout = 0f)
		{
			this.Enqueue(url, body, callback, owner, RequestMethod.PUT, headers, timeout);
		}

		public static string FormatWebException(Exception exception, string response)
		{
			if (!string.IsNullOrEmpty(response))
			{
				response = string.Concat(response, Environment.NewLine);
			}
			response = string.Concat(response, exception.Message);
			if (exception.InnerException != null)
			{
				response = WebRequests.FormatWebException(exception.InnerException, response);
			}
			return response;
		}

		[LibraryFunction("GetQueueLength")]
		public int GetQueueLength()
		{
			return this.queue.Count;
		}

		public override void Shutdown()
		{
			if (this.shutdown)
			{
				return;
			}
			this.shutdown = true;
			this.workevent.Set();
			Thread.Sleep(250);
			this.workerthread.Abort();
		}

		private void Worker()
		{
			int num;
			int num1;
			try
			{
				while (!this.shutdown)
				{
					ThreadPool.GetAvailableThreads(out num, out num1);
					if (num <= this.maxWorkerThreads || num1 <= this.maxCompletionPortThreads)
					{
						Thread.Sleep(100);
					}
					else
					{
						WebRequests.WebRequest webRequest = null;
						lock (this.syncroot)
						{
							if (this.queue.Count > 0)
							{
								webRequest = this.queue.Dequeue();
							}
						}
						if (webRequest == null)
						{
							this.workevent.WaitOne();
						}
						else
						{
							webRequest.Start();
						}
					}
				}
			}
			catch (Exception exception)
			{
				Interface.Oxide.LogException("WebRequests worker: ", exception);
			}
		}

		public class WebRequest
		{
			private HttpWebRequest request;

			private WaitHandle waitHandle;

			private RegisteredWaitHandle registeredWaitHandle;

			private Event.Callback<Plugin, PluginManager> removedFromManager;

			public string Body
			{
				get;
				set;
			}

			public Action<int, string> Callback
			{
				get;
			}

			public string Method
			{
				get;
				set;
			}

			public Plugin Owner
			{
				get;
				protected set;
			}

			public Dictionary<string, string> RequestHeaders
			{
				get;
				set;
			}

			public int ResponseCode
			{
				get;
				protected set;
			}

			public string ResponseText
			{
				get;
				protected set;
			}

			public float Timeout
			{
				get;
				set;
			}

			public string Url
			{
				get;
			}

			public WebRequest(string url, Action<int, string> callback, Plugin owner)
			{
				Event.Callback<Plugin, PluginManager> callback1;
				this.Url = url;
				this.Callback = callback;
				this.Owner = owner;
				Plugin plugin = this.Owner;
				if (plugin != null)
				{
					callback1 = plugin.OnRemovedFromManager.Add(new Action<Plugin, PluginManager>(this.owner_OnRemovedFromManager));
				}
				else
				{
					callback1 = null;
				}
				this.removedFromManager = callback1;
			}

			private void OnComplete()
			{
				Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
				RegisteredWaitHandle registeredWaitHandle = this.registeredWaitHandle;
				if (registeredWaitHandle != null)
				{
					registeredWaitHandle.Unregister(this.waitHandle);
				}
				else
				{
				}
				Interface.Oxide.NextTick(() => {
					if (this.request == null)
					{
						return;
					}
					this.request = null;
					Plugin owner = this.Owner;
					if (owner != null)
					{
						owner.TrackStart();
					}
					else
					{
					}
					try
					{
						this.Callback(this.ResponseCode, this.ResponseText);
					}
					catch (Exception exception1)
					{
						Exception exception = exception1;
						string str = "Web request callback raised an exception";
						if (this.Owner && this.Owner != null)
						{
							str = string.Concat(str, string.Format(" in '{0} v{1}' plugin", this.Owner.Name, this.Owner.Version));
						}
						Interface.Oxide.LogException(str, exception);
					}
					Plugin plugin = this.Owner;
					if (plugin != null)
					{
						plugin.TrackEnd();
					}
					else
					{
					}
					this.Owner = null;
				});
			}

			private void OnTimeout(object state, bool timedOut)
			{
				if (timedOut)
				{
					HttpWebRequest httpWebRequest = this.request;
					if (httpWebRequest != null)
					{
						httpWebRequest.Abort();
					}
					else
					{
					}
				}
				if (this.Owner == null)
				{
					return;
				}
				Event.Remove<Plugin, PluginManager>(ref this.removedFromManager);
				this.Owner = null;
			}

			private void owner_OnRemovedFromManager(Plugin sender, PluginManager manager)
			{
				if (this.request == null)
				{
					return;
				}
				HttpWebRequest httpWebRequest = this.request;
				this.request = null;
				httpWebRequest.Abort();
			}

			public void Start()
			{
				try
				{
					this.request = (HttpWebRequest)System.Net.WebRequest.Create(this.Url);
					this.request.Method = this.Method;
					this.request.Credentials = CredentialCache.DefaultCredentials;
					this.request.Proxy = null;
					this.request.KeepAlive = false;
					this.request.Timeout = (int)Math.Round((double)((this.Timeout.Equals(0f) ? WebRequests.Timeout : this.Timeout) * 1000f));
					this.request.AutomaticDecompression = (WebRequests.AllowDecompression ? DecompressionMethods.GZip | DecompressionMethods.Deflate : DecompressionMethods.None);
					this.request.ServicePoint.MaxIdleTime = this.request.Timeout;
					this.request.ServicePoint.Expect100Continue = ServicePointManager.Expect100Continue;
					this.request.ServicePoint.ConnectionLimit = ServicePointManager.DefaultConnectionLimit;
					if (!this.request.RequestUri.IsLoopback && Environment.OSVersion.Platform != PlatformID.Unix)
					{
						this.request.ServicePoint.BindIPEndPointDelegate = (ServicePoint servicePoint, IPEndPoint remoteEndPoint, int retryCount) => new IPEndPoint(WebRequests.covalence.Server.LocalAddress ?? WebRequests.covalence.Server.Address, 0);
					}
					byte[] bytes = new byte[0];
					if (this.Body != null)
					{
						bytes = Encoding.UTF8.GetBytes(this.Body);
						this.request.ContentLength = (long)((int)bytes.Length);
						this.request.ContentType = "application/x-www-form-urlencoded";
					}
					if (this.RequestHeaders != null)
					{
						this.request.SetRawHeaders(this.RequestHeaders);
					}
					if (bytes.Length == 0)
					{
						this.WaitForResponse();
					}
					else
					{
						this.request.BeginGetRequestStream((IAsyncResult result) => {
							if (this.request == null)
							{
								return;
							}
							try
							{
								using (Stream stream = this.request.EndGetRequestStream(result))
								{
									stream.Write(bytes, 0, (int)bytes.Length);
								}
							}
							catch (Exception exception)
							{
								this.ResponseText = WebRequests.FormatWebException(exception, this.ResponseText ?? string.Empty);
								HttpWebRequest u003cu003e4_this = this.request;
								if (u003cu003e4_this != null)
								{
									u003cu003e4_this.Abort();
								}
								else
								{
								}
								this.OnComplete();
								return;
							}
							this.WaitForResponse();
						}, null);
					}
				}
				catch (Exception exception2)
				{
					Exception exception1 = exception2;
					this.ResponseText = WebRequests.FormatWebException(exception1, this.ResponseText ?? string.Empty);
					string str = string.Concat("Web request produced exception (Url: ", this.Url, ")");
					if (this.Owner)
					{
						str = string.Concat(str, string.Format(" in '{0} v{1}' plugin", this.Owner.Name, this.Owner.Version));
					}
					Interface.Oxide.LogException(str, exception1);
					HttpWebRequest httpWebRequest = this.request;
					if (httpWebRequest != null)
					{
						httpWebRequest.Abort();
					}
					else
					{
					}
					this.OnComplete();
				}
			}

			private void WaitForResponse()
			{
				IAsyncResult asyncResult = this.request.BeginGetResponse((IAsyncResult res) => {
					try
					{
						using (HttpWebResponse httpWebResponse = (HttpWebResponse)this.request.EndGetResponse(res))
						{
							using (Stream responseStream = httpWebResponse.GetResponseStream())
							{
								using (StreamReader streamReader = new StreamReader(responseStream))
								{
									this.ResponseText = streamReader.ReadToEnd();
								}
							}
							this.ResponseCode = (int)httpWebResponse.StatusCode;
						}
					}
					catch (WebException webException1)
					{
						WebException webException = webException1;
						this.ResponseText = WebRequests.FormatWebException(webException, this.ResponseText ?? string.Empty);
						HttpWebResponse response = webException.Response as HttpWebResponse;
						if (response != null)
						{
							try
							{
								using (Stream stream = response.GetResponseStream())
								{
									using (StreamReader streamReader1 = new StreamReader(stream))
									{
										this.ResponseText = streamReader1.ReadToEnd();
									}
								}
							}
							catch (Exception exception)
							{
							}
							this.ResponseCode = (int)response.StatusCode;
						}
					}
					catch (Exception exception2)
					{
						Exception exception1 = exception2;
						this.ResponseText = WebRequests.FormatWebException(exception1, this.ResponseText ?? string.Empty);
						string str = string.Concat("Web request produced exception (Url: ", this.Url, ")");
						if (this.Owner)
						{
							str = string.Concat(str, string.Format(" in '{0} v{1}' plugin", this.Owner.Name, this.Owner.Version));
						}
						Interface.Oxide.LogException(str, exception1);
					}
					if (this.request == null)
					{
						return;
					}
					this.request.Abort();
					this.OnComplete();
				}, null);
				this.waitHandle = asyncResult.AsyncWaitHandle;
				this.registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(this.waitHandle, new WaitOrTimerCallback(this.OnTimeout), null, this.request.Timeout, true);
			}
		}
	}
}