using System;
using UnityEngine;

public class AICoverPoint : BaseMonoBehaviour
{
	public float coverDot = 0.5f;

	private BaseEntity currentUser;

	public AICoverPoint()
	{
	}

	public void ClearUsed()
	{
		this.currentUser = null;
	}

	public bool InUse()
	{
		return this.currentUser != null;
	}

	public bool IsUsedBy(BaseEntity user)
	{
		if (!this.InUse())
		{
			return false;
		}
		if (user == null)
		{
			return false;
		}
		return user == this.currentUser;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Vector3 vector3 = base.transform.position + (Vector3.up * 1f);
		Gizmos.DrawCube(base.transform.position + (Vector3.up * 0.125f), new Vector3(0.5f, 0.25f, 0.5f));
		Gizmos.DrawLine(base.transform.position, vector3);
		Vector3 vector31 = base.transform.forward + ((base.transform.right * this.coverDot) * 1f);
		Vector3 vector32 = vector31.normalized;
		vector31 = base.transform.forward + ((-base.transform.right * this.coverDot) * 1f);
		Vector3 vector33 = vector31.normalized;
		Gizmos.DrawLine(vector3, vector3 + (vector32 * 1f));
		Gizmos.DrawLine(vector3, vector3 + (vector33 * 1f));
	}

	public void SetUsedBy(BaseEntity user, float duration = 5f)
	{
		this.currentUser = user;
		base.Invoke(new Action(this.ClearUsed), duration);
	}
}