using Mono.Unix;
using Mono.Unix.Native;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Threading;

namespace Mono.Remoting.Channels.Unix
{
	public class UnixServerChannel : IChannel, IChannelReceiver
	{
		private string path;

		private string name = "unix";

		private int priority = 1;

		private bool supressChannelData;

		private Thread server_thread;

		private UnixListener listener;

		private UnixServerTransportSink sink;

		private ChannelDataStore channel_data;

		private int _maxConcurrentConnections = 100;

		private ArrayList _activeConnections = new ArrayList();

		public object ChannelData
		{
			get
			{
				if (this.supressChannelData)
				{
					return null;
				}
				return this.channel_data;
			}
		}

		public string ChannelName
		{
			get
			{
				return this.name;
			}
		}

		public int ChannelPriority
		{
			get
			{
				return this.priority;
			}
		}

		public UnixServerChannel(string path)
		{
			this.path = path;
			this.Init(null);
		}

		public UnixServerChannel(IDictionary properties, IServerChannelSinkProvider serverSinkProvider)
		{
			int num;
			IDictionaryEnumerator enumerator = properties.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry current = (DictionaryEntry)enumerator.Current;
					string key = (string)current.Key;
					if (key == null)
					{
						continue;
					}
					if (UnixServerChannel.<>f__switch$map2 == null)
					{
						Dictionary<string, int> strs = new Dictionary<string, int>(3)
						{
							{ "path", 0 },
							{ "priority", 1 },
							{ "supressChannelData", 2 }
						};
						UnixServerChannel.<>f__switch$map2 = strs;
					}
					if (!UnixServerChannel.<>f__switch$map2.TryGetValue(key, out num))
					{
						continue;
					}
					switch (num)
					{
						case 0:
						{
							this.path = current.Value as string;
							continue;
						}
						case 1:
						{
							this.priority = Convert.ToInt32(current.Value);
							continue;
						}
						case 2:
						{
							this.supressChannelData = Convert.ToBoolean(current.Value);
							continue;
						}
						default:
						{
							continue;
						}
					}
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable == null)
				{
				}
				disposable.Dispose();
			}
			this.Init(serverSinkProvider);
		}

		public UnixServerChannel(string name, string path, IServerChannelSinkProvider serverSinkProvider)
		{
			this.name = name;
			this.path = path;
			this.Init(serverSinkProvider);
		}

		public UnixServerChannel(string name, string path)
		{
			this.name = name;
			this.path = path;
			this.Init(null);
		}

		internal void CreateListenerConnection(Socket client)
		{
			ArrayList arrayLists = this._activeConnections;
			Monitor.Enter(arrayLists);
			try
			{
				if (this._activeConnections.Count >= this._maxConcurrentConnections)
				{
					Monitor.Wait(this._activeConnections);
				}
				if (this.server_thread != null)
				{
					ClientConnection clientConnection = new ClientConnection(this, client, this.sink);
					Thread thread = new Thread(new ThreadStart(clientConnection.ProcessMessages));
					thread.Start();
					thread.IsBackground = true;
					this._activeConnections.Add(thread);
				}
			}
			finally
			{
				Monitor.Exit(arrayLists);
			}
		}

		public string GetChannelUri()
		{
			return string.Concat("unix://", this.path);
		}

		public string[] GetUrlsForUri(string uri)
		{
			if (!uri.StartsWith("/"))
			{
				uri = string.Concat("/", uri);
			}
			string[] channelUris = this.channel_data.ChannelUris;
			string[] strArrays = new string[(int)channelUris.Length];
			for (int i = 0; i < (int)channelUris.Length; i++)
			{
				strArrays[i] = string.Concat(channelUris[i], "?", uri);
			}
			return strArrays;
		}

		private void Init(IServerChannelSinkProvider serverSinkProvider)
		{
			if (serverSinkProvider == null)
			{
				serverSinkProvider = new UnixBinaryServerFormatterSinkProvider();
			}
			this.channel_data = new ChannelDataStore(null);
			for (IServerChannelSinkProvider i = serverSinkProvider; i != null; i = i.Next)
			{
				i.GetChannelData(this.channel_data);
			}
			this.sink = new UnixServerTransportSink(ChannelServices.CreateServerChannelSinkChain(serverSinkProvider, this));
			this.StartListening(null);
		}

		public string Parse(string url, out string objectURI)
		{
			return UnixChannel.ParseUnixURL(url, out objectURI);
		}

		internal void ReleaseConnection(Thread thread)
		{
			ArrayList arrayLists = this._activeConnections;
			Monitor.Enter(arrayLists);
			try
			{
				this._activeConnections.Remove(thread);
				Monitor.Pulse(this._activeConnections);
			}
			finally
			{
				Monitor.Exit(arrayLists);
			}
		}

		public void StartListening(object data)
		{
			this.listener = new UnixListener(this.path);
			Syscall.chmod(this.path, FilePermissions.DEFFILEMODE);
			if (this.server_thread == null)
			{
				this.listener.Start();
				string[] channelUri = new string[1];
				channelUri = new string[] { this.GetChannelUri() };
				this.channel_data.ChannelUris = channelUri;
				this.server_thread = new Thread(new ThreadStart(this.WaitForConnections))
				{
					IsBackground = true
				};
				this.server_thread.Start();
			}
		}

		public void StopListening(object data)
		{
			// 
			// Current member / type: System.Void Mono.Remoting.Channels.Unix.UnixServerChannel::StopListening(System.Object)
			// File path: D:\GameServers\Rust\RustDedicated_Data\Managed\Oxide.References.dll
			// 
			// Product version: 2019.1.118.0
			// Exception in: System.Void StopListening(System.Object)
			// 
			// Object reference not set to an instance of an object.
			//    at ..() in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 93
			//    at ..( ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 24
			//    at ..Visit(ICodeNode ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Ast\BaseCodeVisitor.cs:line 69
			//    at ..(DecompilationContext ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Steps\RebuildLockStatements.cs:line 19
			//    at ..(MethodBody ,  , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 88
			//    at ..(MethodBody , ILanguage ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.( , ILanguage , MethodBody , DecompilationContext& ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 95
			//    at Telerik.JustDecompiler.Decompiler.Extensions.(MethodBody , ILanguage , DecompilationContext& ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\Extensions.cs:line 58
			//    at ..(ILanguage , MethodDefinition ,  ) in C:\DeveloperTooling_JD_Agent1\_work\15\s\OpenSource\Cecil.Decompiler\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private void WaitForConnections()
		{
			try
			{
				while (true)
				{
					this.CreateListenerConnection(this.listener.AcceptSocket());
				}
			}
			catch
			{
			}
		}
	}
}