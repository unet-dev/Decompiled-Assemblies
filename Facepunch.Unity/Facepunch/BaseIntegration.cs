using Facepunch.Models;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Facepunch
{
	public abstract class BaseIntegration
	{
		public virtual string ApiUrl
		{
			get
			{
				return "https://api.facepunch.com/api/";
			}
		}

		public virtual Facepunch.Models.Auth Auth
		{
			get
			{
				return new Facepunch.Models.Auth()
				{
					Id = this.UserId,
					Name = this.UserName,
					Type = "none",
					Ticket = "none"
				};
			}
		}

		public virtual string Bucket
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual bool DebugOutput
		{
			get
			{
				return UnityEngine.Application.isEditor;
			}
		}

		public virtual string LevelName
		{
			get
			{
				return SceneManager.GetActiveScene().name;
			}
		}

		public virtual bool LocalApi
		{
			get
			{
				return false;
			}
		}

		public virtual int MinutesPlayed
		{
			get
			{
				return 0;
			}
		}

		public abstract string PublicKey
		{
			get;
		}

		public virtual bool RestrictEditorFunctionality
		{
			get
			{
				return true;
			}
		}

		public virtual string ServerAddress
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string ServerName
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string UserId
		{
			get
			{
				return string.Empty;
			}
		}

		public virtual string UserName
		{
			get
			{
				return string.Empty;
			}
		}

		protected BaseIntegration()
		{
		}

		public virtual void OnManifestFile(Facepunch.Models.Manifest manifest)
		{
		}

		public virtual bool ShouldReportException(string message, string stackTrace, LogType type)
		{
			if (message.StartsWith("[Physics.PhysX] RigidBody::setRigidBodyFlag"))
			{
				return false;
			}
			if (type == LogType.Exception)
			{
				return true;
			}
			if (type == LogType.Error)
			{
				return true;
			}
			return false;
		}
	}
}