using Oxide.Core;
using Oxide.Core.Configuration;
using Oxide.Core.Libraries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace Oxide.Core.Plugins.Watchers
{
	public sealed class FSWatcher : PluginChangeWatcher
	{
		private FileSystemWatcher watcher;

		private ICollection<string> watchedPlugins;

		private Dictionary<string, FSWatcher.QueuedChange> changeQueue;

		private Timer timers;

		public FSWatcher(string directory, string filter)
		{
			this.watchedPlugins = new HashSet<string>();
			this.changeQueue = new Dictionary<string, FSWatcher.QueuedChange>();
			this.timers = Interface.Oxide.GetLibrary<Timer>(null);
			if (Interface.Oxide.Config.Options.PluginWatchers)
			{
				this.LoadWatcher(directory, filter);
				return;
			}
			Interface.Oxide.LogWarning("Automatic plugin reloading and unloading has been disabled", Array.Empty<object>());
		}

		public void AddMapping(string name)
		{
			this.watchedPlugins.Add(name);
		}

		[PermissionSet(SecurityAction.Demand, Name="FullTrust")]
		private void LoadWatcher(string directory, string filter)
		{
			this.watcher = new FileSystemWatcher(directory, filter);
			this.watcher.Changed += new FileSystemEventHandler(this.watcher_Changed);
			this.watcher.Created += new FileSystemEventHandler(this.watcher_Changed);
			this.watcher.Deleted += new FileSystemEventHandler(this.watcher_Changed);
			this.watcher.Error += new ErrorEventHandler(this.watcher_Error);
			this.watcher.NotifyFilter = NotifyFilters.LastWrite;
			this.watcher.IncludeSubdirectories = true;
			this.watcher.EnableRaisingEvents = true;
			GC.KeepAlive(this.watcher);
		}

		public void RemoveMapping(string name)
		{
			this.watchedPlugins.Remove(name);
		}

		private void watcher_Changed(object sender, FileSystemEventArgs e)
		{
			FSWatcher.QueuedChange queuedChange1;
			Action action2 = null;
			FileSystemWatcher fileSystemWatcher = (FileSystemWatcher)sender;
			int length = e.FullPath.Length - fileSystemWatcher.Path.Length - Path.GetExtension(e.Name).Length - 1;
			string str = e.FullPath.Substring(fileSystemWatcher.Path.Length + 1, length);
			if (!this.changeQueue.TryGetValue(str, out queuedChange1))
			{
				queuedChange1 = new FSWatcher.QueuedChange();
				this.changeQueue[str] = queuedChange1;
			}
			Timer.TimerInstance timerInstance1 = queuedChange1.timer;
			if (timerInstance1 != null)
			{
				timerInstance1.Destroy();
			}
			else
			{
			}
			queuedChange1.timer = null;
			switch (e.ChangeType)
			{
				case WatcherChangeTypes.Created:
				{
					if (queuedChange1.type != WatcherChangeTypes.Deleted)
					{
						queuedChange1.type = WatcherChangeTypes.Created;
						goto case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted;
					}
					else
					{
						queuedChange1.type = WatcherChangeTypes.Changed;
						goto case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted;
					}
				}
				case WatcherChangeTypes.Deleted:
				{
					if (queuedChange1.type == WatcherChangeTypes.Created)
					{
						this.changeQueue.Remove(str);
						return;
					}
					queuedChange1.type = WatcherChangeTypes.Deleted;
					goto case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted;
				}
				case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted:
				{
					Interface.Oxide.NextTick(() => {
						Timer.TimerInstance timerInstance = queuedChange1.timer;
						if (timerInstance != null)
						{
							timerInstance.Destroy();
						}
						else
						{
						}
						FSWatcher.QueuedChange queuedChange = queuedChange1;
						Timer u003cu003e4_this = this.timers;
						Action u003cu003e9_1 = action2;
						if (u003cu003e9_1 == null)
						{
							Action action = () => {
								queuedChange1.timer = null;
								this.changeQueue.Remove(str);
								if (Regex.Match(str, "include\\\\", RegexOptions.IgnoreCase).Success)
								{
									if (queuedChange1.type == WatcherChangeTypes.Created || queuedChange1.type == WatcherChangeTypes.Changed)
									{
										base.FirePluginSourceChanged(str);
									}
									return;
								}
								switch (queuedChange1.type)
								{
									case WatcherChangeTypes.Created:
									{
										base.FirePluginAdded(str);
										return;
									}
									case WatcherChangeTypes.Deleted:
									{
										if (!this.watchedPlugins.Contains(str))
										{
											return;
										}
										base.FirePluginRemoved(str);
										return;
									}
									case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted:
									{
										return;
									}
									case WatcherChangeTypes.Changed:
									{
										if (this.watchedPlugins.Contains(str))
										{
											base.FirePluginSourceChanged(str);
											return;
										}
										base.FirePluginAdded(str);
										return;
									}
									default:
									{
										return;
									}
								}
							};
							Action action1 = action;
							action2 = action;
							u003cu003e9_1 = action1;
						}
						queuedChange.timer = u003cu003e4_this.Once(0.2f, u003cu003e9_1, null);
					});
					return;
				}
				case WatcherChangeTypes.Changed:
				{
					if (queuedChange1.type == WatcherChangeTypes.Created)
					{
						goto case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted;
					}
					queuedChange1.type = WatcherChangeTypes.Changed;
					goto case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted;
				}
				default:
				{
					goto case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted;
				}
			}
		}

		private void watcher_Error(object sender, ErrorEventArgs e)
		{
			Interface.Oxide.NextTick(() => {
				Interface.Oxide.LogError("FSWatcher error: {0}", new object[] { e.GetException() });
				RemoteLogger.Exception("FSWatcher error", e.GetException());
			});
		}

		private class QueuedChange
		{
			internal WatcherChangeTypes type;

			internal Timer.TimerInstance timer;

			public QueuedChange()
			{
			}
		}
	}
}