using System;

public class CoverageQueryFlare : SimpleFlare
{
	private CoverageQueries.Query query;

	public CoverageQueries.RadiusSpace coverageRadiusSpace;

	public float coverageRadius = 0.01f;

	public CoverageQueryFlare()
	{
	}
}