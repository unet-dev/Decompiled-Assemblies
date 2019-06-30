using System;

public class WorkshopMainMenu : SingletonComponent<WorkshopMainMenu>
{
	public static Translate.Phrase loading_workshop;

	public static Translate.Phrase loading_workshop_setup;

	public static Translate.Phrase loading_workshop_skinnables;

	public static Translate.Phrase loading_workshop_item;

	static WorkshopMainMenu()
	{
		WorkshopMainMenu.loading_workshop = new Translate.TokenisedPhrase("loading.workshop", "Loading Workshop");
		WorkshopMainMenu.loading_workshop_setup = new Translate.TokenisedPhrase("loading.workshop.initializing", "Setting Up Scene");
		WorkshopMainMenu.loading_workshop_skinnables = new Translate.TokenisedPhrase("loading.workshop.skinnables", "Getting Skinnables");
		WorkshopMainMenu.loading_workshop_item = new Translate.TokenisedPhrase("loading.workshop.item", "Loading Item Data");
	}

	public WorkshopMainMenu()
	{
	}
}