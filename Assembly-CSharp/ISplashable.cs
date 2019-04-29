using System;

public interface ISplashable
{
	int DoSplash(ItemDefinition splashType, int amount);

	bool wantsSplash(ItemDefinition splashType, int amount);
}