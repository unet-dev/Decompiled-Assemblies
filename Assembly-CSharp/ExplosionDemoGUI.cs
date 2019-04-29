using System;
using UnityEngine;

public class ExplosionDemoGUI : MonoBehaviour
{
	public GameObject[] Prefabs;

	public float reactivateTime = 4f;

	public Light Sun;

	private int currentNomber;

	private GameObject currentInstance;

	private GUIStyle guiStyleHeader = new GUIStyle();

	private float sunIntensity;

	private float dpiScale;

	public ExplosionDemoGUI()
	{
	}

	private void ChangeCurrent(int delta)
	{
		this.currentNomber += delta;
		if (this.currentNomber > (int)this.Prefabs.Length - 1)
		{
			this.currentNomber = 0;
		}
		else if (this.currentNomber < 0)
		{
			this.currentNomber = (int)this.Prefabs.Length - 1;
		}
		if (this.currentInstance != null)
		{
			UnityEngine.Object.Destroy(this.currentInstance);
		}
		GameObject prefabs = this.Prefabs[this.currentNomber];
		Vector3 vector3 = base.transform.position;
		Quaternion quaternion = new Quaternion();
		this.currentInstance = UnityEngine.Object.Instantiate<GameObject>(prefabs, vector3, quaternion);
		this.currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = this.reactivateTime;
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "PREVIOUS EFFECT"))
		{
			this.ChangeCurrent(-1);
		}
		if (GUI.Button(new Rect(160f * this.dpiScale, 15f * this.dpiScale, 135f * this.dpiScale, 37f * this.dpiScale), "NEXT EFFECT"))
		{
			this.ChangeCurrent(1);
		}
		this.sunIntensity = GUI.HorizontalSlider(new Rect(10f * this.dpiScale, 70f * this.dpiScale, 285f * this.dpiScale, 15f * this.dpiScale), this.sunIntensity, 0f, 0.6f);
		this.Sun.intensity = this.sunIntensity;
		GUI.Label(new Rect(300f * this.dpiScale, 70f * this.dpiScale, 30f * this.dpiScale, 30f * this.dpiScale), "SUN INTENSITY", this.guiStyleHeader);
		GUI.Label(new Rect(400f * this.dpiScale, 15f * this.dpiScale, 100f * this.dpiScale, 20f * this.dpiScale), string.Concat("Prefab name is \"", this.Prefabs[this.currentNomber].name, "\"  \r\nHold any mouse button that would move the camera"), this.guiStyleHeader);
	}

	private void Start()
	{
		if (Screen.dpi < 1f)
		{
			this.dpiScale = 1f;
		}
		if (Screen.dpi >= 200f)
		{
			this.dpiScale = Screen.dpi / 200f;
		}
		else
		{
			this.dpiScale = 1f;
		}
		this.guiStyleHeader.fontSize = (int)(15f * this.dpiScale);
		this.guiStyleHeader.normal.textColor = new Color(0.15f, 0.15f, 0.15f);
		GameObject prefabs = this.Prefabs[this.currentNomber];
		Vector3 vector3 = base.transform.position;
		Quaternion quaternion = new Quaternion();
		this.currentInstance = UnityEngine.Object.Instantiate<GameObject>(prefabs, vector3, quaternion);
		this.currentInstance.AddComponent<ExplosionDemoReactivator>().TimeDelayToReactivate = this.reactivateTime;
		this.sunIntensity = this.Sun.intensity;
	}
}