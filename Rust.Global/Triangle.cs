using System;
using UnityEngine;

public struct Triangle
{
	public Vector3 point0;

	public Vector3 point1;

	public Vector3 point2;

	public Vector3 Center
	{
		get
		{
			return ((this.point0 + this.point1) + this.point2) / 3f;
		}
	}

	public Vector3 Normal
	{
		get
		{
			Vector3 vector3 = this.point1 - this.point0;
			Vector3 vector31 = this.point2 - this.point0;
			return Vector3.Cross(vector3, vector31).normalized;
		}
	}

	public Triangle(Vector3 point0, Vector3 point1, Vector3 point2)
	{
		this.point0 = point0;
		this.point1 = point1;
		this.point2 = point2;
	}

	public Vector3 ClosestPoint(Vector3 pos)
	{
		Vector3 vector3 = this.point0 - pos;
		Vector3 vector31 = this.point1 - this.point0;
		Vector3 vector32 = this.point2 - this.point0;
		float single = Vector3.Dot(vector31, vector31);
		float single1 = Vector3.Dot(vector31, vector32);
		float single2 = Vector3.Dot(vector32, vector32);
		float single3 = Vector3.Dot(vector31, vector3);
		float single4 = Vector3.Dot(vector32, vector3);
		float single5 = single * single2 - single1 * single1;
		float single6 = single1 * single4 - single2 * single3;
		float single7 = single1 * single3 - single * single4;
		if (single6 + single7 < single5)
		{
			if (single6 >= 0f)
			{
				if (single7 >= 0f)
				{
					float single8 = 1f / single5;
					single6 *= single8;
					single7 *= single8;
				}
				else
				{
					single6 = Mathf.Clamp01(-single3 / single);
					single7 = 0f;
				}
			}
			else if (single7 >= 0f)
			{
				single6 = 0f;
				single7 = Mathf.Clamp01(-single4 / single2);
			}
			else if (single3 >= 0f)
			{
				single6 = 0f;
				single7 = Mathf.Clamp01(-single4 / single2);
			}
			else
			{
				single6 = Mathf.Clamp01(-single3 / single);
				single7 = 0f;
			}
		}
		else if (single6 < 0f)
		{
			float single9 = single1 + single3;
			float single10 = single2 + single4;
			if (single10 <= single9)
			{
				single7 = Mathf.Clamp01(-single4 / single2);
				single6 = 0f;
			}
			else
			{
				float single11 = single - 2f * single1 + single2;
				single6 = Mathf.Clamp01((single10 - single9) / single11);
				single7 = 1f - single6;
			}
		}
		else if (single7 >= 0f)
		{
			float single12 = single - 2f * single1 + single2;
			single6 = Mathf.Clamp01((single2 + single4 - single1 - single3) / single12);
			single7 = 1f - single6;
		}
		else if (single + single3 <= single1 + single4)
		{
			single6 = Mathf.Clamp01(-single4 / single2);
			single7 = 0f;
		}
		else
		{
			float single13 = single - 2f * single1 + single2;
			single6 = Mathf.Clamp01((single2 + single4 - single1 - single3) / single13);
			single7 = 1f - single6;
		}
		return (this.point0 + (single6 * vector31)) + (single7 * vector32);
	}

	public float Distance(Vector3 pos)
	{
		return (pos - this.ClosestPoint(pos)).magnitude;
	}

	private bool LineTest(Vector3 a, Vector3 b, Ray ray, float radius, out RaycastHit hit, float maxDistance)
	{
		if (!(new Line(this.point0, this.point2)).Trace(ray, radius, out hit, maxDistance))
		{
			return false;
		}
		hit.normal = this.Normal;
		return true;
	}

	public float SqrDistance(Vector3 pos)
	{
		return (pos - this.ClosestPoint(pos)).sqrMagnitude;
	}

	public bool Trace(Ray ray, float radius, out RaycastHit hit, float maxDistance = Single.PositiveInfinity)
	{
		hit = new RaycastHit();
		Vector3 vector3 = this.point1 - this.point0;
		Vector3 vector31 = this.point2 - this.point0;
		Vector3 vector32 = Vector3.Cross(ray.direction, vector31);
		float single = Vector3.Dot(vector3, vector32);
		if (single > -Mathf.Epsilon && single < Mathf.Epsilon)
		{
			return false;
		}
		float single1 = 1f / single;
		Vector3 vector33 = ray.origin - this.point0;
		float single2 = Vector3.Dot(vector33, vector32) * single1;
		if (single2 < 0f)
		{
			return this.LineTest(this.point0, this.point2, ray, radius, out hit, maxDistance);
		}
		if (single2 > 1f)
		{
			return this.LineTest(this.point1, this.point2, ray, radius, out hit, maxDistance);
		}
		Vector3 vector34 = Vector3.Cross(vector33, vector3);
		float single3 = Vector3.Dot(ray.direction, vector34) * single1;
		if (single3 < 0f)
		{
			return this.LineTest(this.point0, this.point1, ray, radius, out hit, maxDistance);
		}
		if (single2 + single3 > 1f)
		{
			return this.LineTest(this.point1, this.point2, ray, radius, out hit, maxDistance);
		}
		float single4 = Vector3.Dot(vector31, vector34) * single1;
		if (single4 < 0f || single4 > maxDistance)
		{
			return false;
		}
		Vector3 vector35 = ray.origin + (single4 * ray.direction);
		hit.point = vector35;
		hit.distance = single4;
		Vector3 vector36 = Vector3.Cross(vector3, vector31);
		hit.normal = vector36.normalized;
		return true;
	}
}