using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Saving.Backpack;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.Game.Talents;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.System;

namespace BackpackSurvivors.Game.Saving;

[Serializable]
public class SaveGame
{
	internal Guid key;

	internal CurrencySaveState CurrencyState { get; private set; }

	internal TalentSaveState TalentsState { get; private set; }

	internal UnlockedsSaveState UnlockedUpgradesState { get; private set; }

	internal StatisticsSaveState StatisticsState { get; private set; }

	internal CharacterExperienceSaveState CharacterExperienceState { get; private set; }

	internal CollectionsSaveState CollectionsSaveState { get; private set; }

	internal UnlockedEquipmentSaveState EquipmentSaveState { get; private set; }

	internal TutorialSaveState TutorialSaveState { get; private set; }

	internal DemoSaveState DemoSaveState { get; private set; }

	internal BackpackSaveState BackpackSaveState { get; private set; }

	internal int SavedAtBuildNumber { get; private set; }

	public SaveGame(Guid saveKey)
	{
		key = saveKey;
		InitCollections();
		SavedAtBuildNumber = SingletonController<SaveGameController>.Instance.BuildNumber;
	}

	public void UpdateSavedAtBuildNumber()
	{
		SavedAtBuildNumber = SingletonController<SaveGameController>.Instance.BuildNumber;
	}

	public bool HasData()
	{
		if ((CurrencyState == null || !CurrencyState.HasData()) && (TalentsState == null || !TalentsState.HasData()) && (UnlockedUpgradesState == null || !UnlockedUpgradesState.HasData()) && (StatisticsState == null || !StatisticsState.HasData()) && (CharacterExperienceState == null || !CharacterExperienceState.HasData()) && (CollectionsSaveState == null || !CollectionsSaveState.HasData()) && (EquipmentSaveState == null || !EquipmentSaveState.HasData()))
		{
			if (DemoSaveState != null)
			{
				return DemoSaveState.HasData();
			}
			return false;
		}
		return true;
	}

	private void InitCollections()
	{
		CurrencyState = new CurrencySaveState();
		TalentsState = new TalentSaveState();
		UnlockedUpgradesState = new UnlockedsSaveState();
		StatisticsState = new StatisticsSaveState();
		CharacterExperienceState = new CharacterExperienceSaveState();
		CollectionsSaveState = new CollectionsSaveState();
		EquipmentSaveState = new UnlockedEquipmentSaveState();
		TutorialSaveState = new TutorialSaveState();
		DemoSaveState = new DemoSaveState(hasShownDemoPopup: false);
	}

	public void RefreshSaveGameState()
	{
		CurrencyState = SingletonController<CurrencyController>.Instance.GetSaveState();
		TalentsState = SingletonController<NewTalentController>.Instance.GetSaveState();
		UnlockedUpgradesState = SingletonController<UnlocksController>.Instance.GetSaveState();
		StatisticsState = SingletonController<StatisticsController>.Instance.GetSaveState();
		CharacterExperienceState = SingletonController<CharactersController>.Instance.GetSaveState();
		CollectionsSaveState = SingletonController<CollectionController>.Instance.GetSaveState();
		EquipmentSaveState = SingletonController<UnlockedEquipmentController>.Instance.GetSaveState();
		TutorialSaveState = SingletonController<TutorialController>.Instance.GetSaveState();
		DemoSaveState = new DemoSaveState(SingletonController<GameController>.Instance.HasShownDemoPopup);
	}

	public void SaveBackpackState()
	{
		BackpackSaveState = SingletonController<BackpackController>.Instance.GetSaveState();
	}

	internal void Validate()
	{
		if (CollectionsSaveState.FoundRecipes == null)
		{
			CollectionsSaveState.FoundRecipes = new List<int>();
		}
	}
}
