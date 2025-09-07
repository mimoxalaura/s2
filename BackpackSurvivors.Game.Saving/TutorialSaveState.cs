using System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
internal class TutorialSaveState : BaseSaveState
{
	internal bool ShownTownTutorial;

	internal bool ShownTalentTutorial;

	internal bool ShownBackpackTutorial;

	internal bool ShownTitanicSoulTutorial;

	internal TutorialSaveState()
	{
		ShownTownTutorial = false;
		ShownTalentTutorial = false;
		ShownBackpackTutorial = false;
		ShownTitanicSoulTutorial = false;
	}

	public override bool HasData()
	{
		return true;
	}

	internal void SetState(bool shownTownTutorial, bool shownTalentTutorial, bool shownBackpackTutorial, bool shownTitanicSoulTutorial)
	{
		ShownTownTutorial = shownTownTutorial;
		ShownTalentTutorial = shownTalentTutorial;
		ShownBackpackTutorial = shownBackpackTutorial;
		ShownTitanicSoulTutorial = shownTitanicSoulTutorial;
	}
}
