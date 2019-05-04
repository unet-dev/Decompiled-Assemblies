using Rust.Workshop;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConVar
{
	[Factory("graphics")]
	public class Graphics : ConsoleSystem
	{
		private const float MinShadowDistance = 40f;

		private const float MaxShadowDistance2Split = 180f;

		private const float MaxShadowDistance4Split = 800f;

		private static float _shadowdistance;

		[ClientVar(Saved=true)]
		public static int shadowmode;

		[ClientVar(Saved=true)]
		public static int shadowlights;

		private static int _shadowquality;

		[ClientVar(Saved=true)]
		public static bool grassshadows;

		[ClientVar(Saved=true)]
		public static bool contactshadows;

		[ClientVar(Saved=true)]
		public static float drawdistance;

		private static float _fov;

		[ClientVar]
		public static bool hud;

		[ClientVar(Saved=true)]
		public static bool chat;

		[ClientVar(Saved=true)]
		public static bool branding;

		[ClientVar(Saved=true)]
		public static int compass;

		[ClientVar(Saved=true)]
		public static bool dof;

		[ClientVar(Saved=true)]
		public static float dof_aper;

		[ClientVar(Saved=true)]
		public static float dof_blur;

		private static float _uiscale;

		private static int _anisotropic;

		private static int _parallax;

		[ClientVar(Saved=true)]
		public static int af
		{
			get
			{
				return ConVar.Graphics._anisotropic;
			}
			set
			{
				value = Mathf.Clamp(value, 1, 16);
				Texture.SetGlobalAnisotropicFilteringLimits(1, value);
				if (value <= 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Disable;
				}
				if (value > 1)
				{
					Texture.anisotropicFiltering = AnisotropicFiltering.Enable;
				}
				ConVar.Graphics._anisotropic = value;
			}
		}

		[ClientVar(Saved=true)]
		public static float fov
		{
			get
			{
				return ConVar.Graphics._fov;
			}
			set
			{
				ConVar.Graphics._fov = Mathf.Clamp(value, 70f, 90f);
			}
		}

		[ClientVar]
		public static bool itemskins
		{
			get
			{
				return WorkshopSkin.AllowApply;
			}
			set
			{
				WorkshopSkin.AllowApply = value;
			}
		}

		[ClientVar]
		public static float itemskintimeout
		{
			get
			{
				return WorkshopSkin.DownloadTimeout;
			}
			set
			{
				WorkshopSkin.DownloadTimeout = value;
			}
		}

		[ClientVar]
		public static bool itemskinunload
		{
			get
			{
				return WorkshopSkin.AllowUnload;
			}
			set
			{
				WorkshopSkin.AllowUnload = value;
			}
		}

		[ClientVar]
		public static float lodbias
		{
			get
			{
				return QualitySettings.lodBias;
			}
			set
			{
				QualitySettings.lodBias = Mathf.Clamp(value, 0.25f, 5f);
			}
		}

		[ClientVar(Saved=true)]
		public static int parallax
		{
			get
			{
				return ConVar.Graphics._parallax;
			}
			set
			{
				if (value == 1)
				{
					Shader.EnableKeyword("TERRAIN_PARALLAX_OFFSET");
					Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
				}
				else if (value == 2)
				{
					Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
					Shader.EnableKeyword("TERRAIN_PARALLAX_OCCLUSION");
				}
				else
				{
					Shader.DisableKeyword("TERRAIN_PARALLAX_OFFSET");
					Shader.DisableKeyword("TERRAIN_PARALLAX_OCCLUSION");
				}
				ConVar.Graphics._parallax = value;
			}
		}

		[ClientVar(Help="The currently selected quality level")]
		public static int quality
		{
			get
			{
				return QualitySettings.GetQualityLevel();
			}
			set
			{
				int num = ConVar.Graphics.shadowcascades;
				QualitySettings.SetQualityLevel(value, true);
				ConVar.Graphics.shadowcascades = num;
			}
		}

		[ClientVar(Saved=true)]
		public static int shaderlod
		{
			get
			{
				return Shader.globalMaximumLOD;
			}
			set
			{
				Shader.globalMaximumLOD = Mathf.Clamp(value, 100, 600);
			}
		}

		[ClientVar(Saved=true)]
		public static int shadowcascades
		{
			get
			{
				return QualitySettings.shadowCascades;
			}
			set
			{
				QualitySettings.shadowCascades = value;
				QualitySettings.shadowDistance = ConVar.Graphics.EnforceShadowDistanceBounds(ConVar.Graphics.shadowdistance);
			}
		}

		[ClientVar(Saved=true)]
		public static float shadowdistance
		{
			get
			{
				return ConVar.Graphics._shadowdistance;
			}
			set
			{
				ConVar.Graphics._shadowdistance = value;
				QualitySettings.shadowDistance = ConVar.Graphics.EnforceShadowDistanceBounds(ConVar.Graphics._shadowdistance);
			}
		}

		[ClientVar(Saved=true)]
		public static int shadowquality
		{
			get
			{
				return ConVar.Graphics._shadowquality;
			}
			set
			{
				ConVar.Graphics._shadowquality = Mathf.Clamp(value, 0, 2);
				ConVar.Graphics.shadowmode = ConVar.Graphics._shadowquality + 1;
				KeywordUtil.EnsureKeywordState("SHADOW_QUALITY_HIGH", (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore ? ConVar.Graphics._shadowquality >= 2 : false));
			}
		}

		[ClientVar(Saved=true)]
		public static float uiscale
		{
			get
			{
				return ConVar.Graphics._uiscale;
			}
			set
			{
				ConVar.Graphics._uiscale = Mathf.Clamp(value, 0.5f, 1f);
			}
		}

		static Graphics()
		{
			ConVar.Graphics._shadowdistance = 800f;
			ConVar.Graphics.shadowmode = 2;
			ConVar.Graphics.shadowlights = 1;
			ConVar.Graphics._shadowquality = 1;
			ConVar.Graphics.grassshadows = false;
			ConVar.Graphics.contactshadows = false;
			ConVar.Graphics.drawdistance = 2500f;
			ConVar.Graphics._fov = 75f;
			ConVar.Graphics.hud = true;
			ConVar.Graphics.chat = true;
			ConVar.Graphics.branding = true;
			ConVar.Graphics.compass = 1;
			ConVar.Graphics.dof = false;
			ConVar.Graphics.dof_aper = 12f;
			ConVar.Graphics.dof_blur = 1f;
			ConVar.Graphics._uiscale = 1f;
			ConVar.Graphics._anisotropic = 1;
			ConVar.Graphics._parallax = 0;
		}

		public Graphics()
		{
		}

		public static float EnforceShadowDistanceBounds(float distance)
		{
			if (QualitySettings.shadowCascades == 1)
			{
				distance = Mathf.Clamp(distance, 40f, 40f);
			}
			else if (QualitySettings.shadowCascades != 2)
			{
				distance = Mathf.Clamp(distance, 40f, 800f);
			}
			else
			{
				distance = Mathf.Clamp(distance, 40f, 180f);
			}
			return distance;
		}
	}
}