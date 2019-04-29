using Kino;
using Smaa;
using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.CinematicEffects;
using UnityStandardAssets.ImageEffects;

[ExecuteInEditMode]
public class MainCamera : SingletonComponent<MainCamera>
{
	public static Camera mainCamera;

	public UnityStandardAssets.ImageEffects.DepthOfField dof;

	public AmplifyOcclusionEffect ssao;

	public Kino.Motion motionBlur;

	public TOD_Rays shafts;

	public TonemappingColorGrading tonemappingColorGrading;

	public FXAA fxaa;

	public SMAA smaa;

	public PostProcessLayer post;

	public CC_SharpenAndVignette sharpenAndVignette;

	public SEScreenSpaceShadows contactShadows;

	public VisualizeTexelDensity visualizeTexelDensity;

	public EnvironmentVolumePropertiesCollection environmentVolumeProperties;

	public static Vector3 forward
	{
		get
		{
			return MainCamera.mainCamera.transform.forward;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.forward = value;
			}
		}
	}

	public static bool isValid
	{
		get
		{
			if (MainCamera.mainCamera == null)
			{
				return false;
			}
			return MainCamera.mainCamera.enabled;
		}
	}

	public static Vector3 position
	{
		get
		{
			return MainCamera.mainCamera.transform.position;
		}
		set
		{
			MainCamera.mainCamera.transform.position = value;
		}
	}

	public static UnityEngine.Ray Ray
	{
		get
		{
			return new UnityEngine.Ray(MainCamera.position, MainCamera.forward);
		}
	}

	public static RaycastHit Raycast
	{
		get
		{
			RaycastHit raycastHit;
			Physics.Raycast(MainCamera.Ray, out raycastHit, 1024f, 229731073);
			return raycastHit;
		}
	}

	public static Vector3 right
	{
		get
		{
			return MainCamera.mainCamera.transform.right;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.right = value;
			}
		}
	}

	public static Quaternion rotation
	{
		get
		{
			return MainCamera.mainCamera.transform.rotation;
		}
	}

	public static Vector3 up
	{
		get
		{
			return MainCamera.mainCamera.transform.up;
		}
		set
		{
			if (value.sqrMagnitude > 0f)
			{
				MainCamera.mainCamera.transform.up = value;
			}
		}
	}

	public MainCamera()
	{
	}
}