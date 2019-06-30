using System;

public class Client : SingletonComponent<Client>
{
	public static Translate.Phrase loading_loading;

	public static Translate.Phrase loading_connecting;

	public static Translate.Phrase loading_connectionaccepted;

	public static Translate.Phrase loading_connecting_negotiate;

	public static Translate.Phrase loading_level;

	public static Translate.Phrase loading_skinnablewarmup;

	public static Translate.Phrase loading_preloadcomplete;

	public static Translate.Phrase loading_openingscene;

	public static Translate.Phrase loading_clientready;

	public static Translate.Phrase loading_prefabwarmup;

	static Client()
	{
		Client.loading_loading = new Translate.TokenisedPhrase("loading.loading", "Loading");
		Client.loading_connecting = new Translate.TokenisedPhrase("loading.connecting", "Connecting");
		Client.loading_connectionaccepted = new Translate.TokenisedPhrase("loading.connectionaccepted", "Connection Accepted");
		Client.loading_connecting_negotiate = new Translate.TokenisedPhrase("loading.connecting.negotiate", "Negotiating Connection");
		Client.loading_level = new Translate.TokenisedPhrase("loading.loadinglevel", "Loading Level");
		Client.loading_skinnablewarmup = new Translate.TokenisedPhrase("loading.skinnablewarmup", "Skinnable Warmup");
		Client.loading_preloadcomplete = new Translate.TokenisedPhrase("loading.preloadcomplete", "Preload Complete");
		Client.loading_openingscene = new Translate.TokenisedPhrase("loading.openingscene", "Opening Scene");
		Client.loading_clientready = new Translate.TokenisedPhrase("loading.clientready", "Client Ready");
		Client.loading_prefabwarmup = new Translate.TokenisedPhrase("loading.prefabwarmup", "Warming Prefabs [{0}/{1}]");
	}

	public Client()
	{
	}
}