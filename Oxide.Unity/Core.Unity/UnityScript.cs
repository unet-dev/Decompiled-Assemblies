using Oxide.Core;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oxide.Core.Unity
{
	public class UnityScript : MonoBehaviour
	{
		private OxideMod oxideMod;

		public static GameObject Instance
		{
			get;
			private set;
		}

		public UnityScript()
		{
		}

		private void Awake()
		{
			this.oxideMod = Interface.get_Oxide();
			EventInfo @event = typeof(Application).GetEvent("logMessageReceived");
			if (@event != null)
			{
				Delegate @delegate = Delegate.CreateDelegate(@event.EventHandlerType, this, "LogMessageReceived");
				@event.GetAddMethod().Invoke(null, new object[] { @delegate });
				return;
			}
			FieldInfo field = typeof(Application).GetField("s_LogCallback", BindingFlags.Static | BindingFlags.NonPublic);
			Application.LogCallback logCallback1 = (field != null ? field.GetValue(null) : null) as Application.LogCallback;
			if (logCallback1 == null)
			{
				Interface.get_Oxide().LogWarning("No Unity application log callback is registered", new object[0]);
			}
			Application.RegisterLogCallback((string message, string stack_trace, LogType type) => {
				Application.LogCallback logCallback = logCallback1;
				if (logCallback != null)
				{
					logCallback(message, stack_trace, type);
				}
				else
				{
				}
				this.LogMessageReceived(message, stack_trace, type);
			});
		}

		public static void Create()
		{
			UnityScript.Instance = new GameObject("Oxide.Core.Unity");
			UnityEngine.Object.DontDestroyOnLoad(UnityScript.Instance);
			UnityScript.Instance.AddComponent<UnityScript>();
		}

		private void LogMessageReceived(string message, string stackTrace, LogType type)
		{
			if (type == LogType.Exception)
			{
				RemoteLogger.Exception(message, stackTrace);
			}
		}

		private void OnApplicationQuit()
		{
			if (!this.oxideMod.get_IsShuttingDown())
			{
				Interface.Call("OnServerShutdown", new object[0]);
				Interface.get_Oxide().OnShutdown();
			}
		}

		private void OnDestroy()
		{
			if (this.oxideMod.get_IsShuttingDown())
			{
				return;
			}
			this.oxideMod.LogWarning("The Oxide Unity Script was destroyed (creating a new instance)", new object[0]);
			this.oxideMod.NextTick(new Action(UnityScript.Create));
		}

		private void Update()
		{
			this.oxideMod.OnFrame(Time.deltaTime);
		}
	}
}