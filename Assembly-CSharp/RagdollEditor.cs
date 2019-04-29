using System;
using UnityEngine;

public class RagdollEditor : SingletonComponent<RagdollEditor>
{
	private Vector3 view;

	private Rigidbody grabbedRigid;

	private Vector3 grabPos;

	private Vector3 grabOffset;

	public RagdollEditor()
	{
	}

	protected override void Awake()
	{
		base.Awake();
	}

	private void FixedUpdate()
	{
		if (Input.GetKey(KeyCode.Mouse0))
		{
			this.UpdateGrab();
		}
	}

	private void OnGUI()
	{
		GUI.Box(new Rect((float)Screen.width * 0.5f - 2f, (float)Screen.height * 0.5f - 2f, 4f, 4f), "");
	}

	private void StartGrab()
	{
		RaycastHit raycastHit;
		if (!Physics.Raycast(base.transform.position, base.transform.forward, out raycastHit, 100f))
		{
			return;
		}
		this.grabbedRigid = raycastHit.collider.GetComponent<Rigidbody>();
		if (this.grabbedRigid == null)
		{
			return;
		}
		Matrix4x4 matrix4x4 = this.grabbedRigid.transform.worldToLocalMatrix;
		this.grabPos = matrix4x4.MultiplyPoint(raycastHit.point);
		matrix4x4 = base.transform.worldToLocalMatrix;
		this.grabOffset = matrix4x4.MultiplyPoint(raycastHit.point);
	}

	private void StopGrab()
	{
		this.grabbedRigid = null;
	}

	private void Update()
	{
		Camera.main.fieldOfView = 75f;
		if (!Input.GetKey(KeyCode.Mouse1))
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			ref float axisRaw = ref this.view.y;
			axisRaw = axisRaw + Input.GetAxisRaw("Mouse X") * 3f;
			ref float singlePointer = ref this.view.x;
			singlePointer = singlePointer - Input.GetAxisRaw("Mouse Y") * 3f;
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
		Camera.main.transform.rotation = Quaternion.Euler(this.view);
		Vector3 vector3 = Vector3.zero;
		if (Input.GetKey(KeyCode.W))
		{
			vector3 += Vector3.forward;
		}
		if (Input.GetKey(KeyCode.S))
		{
			vector3 += Vector3.back;
		}
		if (Input.GetKey(KeyCode.A))
		{
			vector3 += Vector3.left;
		}
		if (Input.GetKey(KeyCode.D))
		{
			vector3 += Vector3.right;
		}
		Transform transforms = Camera.main.transform;
		transforms.position = transforms.position + ((base.transform.rotation * vector3) * 0.05f);
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			this.StartGrab();
		}
		if (Input.GetKeyUp(KeyCode.Mouse0))
		{
			this.StopGrab();
		}
	}

	private void UpdateGrab()
	{
		if (this.grabbedRigid == null)
		{
			return;
		}
		Vector3 vector3 = base.transform.TransformPoint(this.grabOffset);
		Vector3 vector31 = this.grabbedRigid.transform.TransformPoint(this.grabPos);
		Vector3 vector32 = vector3 - vector31;
		this.grabbedRigid.AddForceAtPosition(vector32 * 100f, vector31, ForceMode.Acceleration);
	}
}