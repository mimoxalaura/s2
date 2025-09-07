using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Characters.Events;
using BackpackSurvivors.Game.Combat;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Enemies;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Saving.Events;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using QFSW.QC;
using UnityEngine;

namespace BackpackSurvivors.Game.Characters;

public class CharactersController : SingletonController<CharactersController>
{
	internal delegate void OnCharacterSwitchedHandler(object sender, CharacterSwitchedEventArgs e);

	internal delegate void OnCharacterLoadedHandler(object sender, CharacterSwitchedEventArgs e);

	internal delegate void OnBuffAppliedHandler(object sender, BuffAppliedEventArgs e);

	internal delegate void OnExperienceGainedHandler(object sender, ExperienceGainedEventArgs e);

	internal delegate void OnCharacterLeveledHandler(object sender, CharacterLeveledEventArgs e);

	[SerializeField]
	private int _startingCharacterId;

	[SerializeField]
	private Character _dummyCharacterPrefab;

	private Dictionary<int, int> _characterLevels = new Dictionary<int, int>();

	private Dictionary<int, float> _characterExperience = new Dictionary<int, float>();

	private ExperienceCalculator _experienceCalculator;

	public CharacterSO ActiveCharacter;

	internal int CurrentLevel { get; private set; }

	internal float CurrentExperience { get; private set; }

	internal float TotalCurrentExperience { get; private set; }

	internal float OriginalExperience { get; private set; }

	internal int OriginalLevel { get; private set; }

	internal float ExperienceToNext => _experienceCalculator.GetExperienceNeededForLevel(CurrentLevel);

	internal ExperienceCalculator ExperienceCalculator => _experienceCalculator;

	internal event OnCharacterSwitchedHandler OnCharacterSwitched;

	internal event OnCharacterLoadedHandler OnCharacterLoaded;

	internal event OnExperienceGainedHandler OnExperienceGained;

	internal event OnCharacterLeveledHandler OnCharacterLeveled;

	private void Start()
	{
		RegisterEvents();
		Init();
	}

	[Command("character.addexperience", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void AddExperience(float experience)
	{
		CurrentExperience += experience;
		TotalCurrentExperience += experience;
		int currentLevel = _characterLevels[ActiveCharacter.Id];
		float newExperience = _characterExperience[ActiveCharacter.Id] + experience;
		_characterExperience[ActiveCharacter.Id] = newExperience;
		float experienceForNextLevel = _experienceCalculator.GetExperienceNeededForLevel(currentLevel);
		while (newExperience >= experienceForNextLevel)
		{
			LevelUp(ref currentLevel, ref newExperience, ref experienceForNextLevel);
		}
		CurrentExperience = newExperience;
		this.OnExperienceGained?.Invoke(this, new ExperienceGainedEventArgs(newExperience, experienceForNextLevel, currentLevel, experience));
	}

	private void LevelUp(ref int currentLevel, ref float newExperience, ref float experienceForNextLevel)
	{
		newExperience -= experienceForNextLevel;
		_characterExperience[ActiveCharacter.Id] = newExperience;
		currentLevel++;
		CurrentLevel = currentLevel;
		_characterLevels[ActiveCharacter.Id] = currentLevel;
		experienceForNextLevel = _experienceCalculator.GetExperienceNeededForLevel(currentLevel);
		this.OnCharacterLeveled?.Invoke(this, new CharacterLeveledEventArgs(currentLevel));
	}

	[Command("character.switch", Platform.AllPlatforms, MonoTargetType.Single)]
	internal void DEBUG_SwitchCharacter(int characterId)
	{
		CharacterSO characterById = GameDatabaseHelper.GetCharacterById(characterId);
		SwitchCharacter(characterById);
	}

	internal void SwitchCharacter(CharacterSO characterSO)
	{
		SingletonController<StartingEquipmentController>.Instance.SwitchStartingCharacter(characterSO);
		ActiveCharacter = characterSO;
		if (!_characterLevels.ContainsKey(characterSO.Id))
		{
			_characterLevels.Add(characterSO.Id, 1);
		}
		CurrentLevel = _characterLevels[characterSO.Id];
		if (!_characterExperience.ContainsKey(characterSO.Id))
		{
			_characterExperience.Add(characterSO.Id, 0f);
		}
		CurrentExperience = _characterExperience[characterSO.Id];
		TotalCurrentExperience = CurrentExperience;
		this.OnCharacterSwitched?.Invoke(this, new CharacterSwitchedEventArgs(ActiveCharacter));
	}

	internal void StoreCurrentExperience()
	{
		OriginalExperience = _characterExperience[ActiveCharacter.Id];
		OriginalLevel = _characterLevels[ActiveCharacter.Id];
	}

	internal CharacterExperienceSaveState GetSaveState()
	{
		CharacterExperienceSaveState characterExperienceSaveState = new CharacterExperienceSaveState();
		characterExperienceSaveState.SetState(ActiveCharacter.Id, _characterLevels, _characterExperience);
		return characterExperienceSaveState;
	}

	private void Init()
	{
		InitCharacters();
		InitExperienceCalculator();
	}

	internal Character CreateAndGetDummyCharacter()
	{
		Character character = Object.Instantiate(_dummyCharacterPrefab);
		character.InitGuid();
		Dictionary<Enums.ItemStatType, float> calculatedStats = StatCalculator.CreateItemStatDictionary();
		character.InitCalculatedStats(calculatedStats);
		return character;
	}

	private void InitCharacters()
	{
		foreach (CharacterSO availableCharacter in SingletonController<GameDatabase>.Instance.GameDatabaseSO.AvailableCharacters)
		{
			_characterLevels.Add(availableCharacter.Id, 1);
			_characterExperience.Add(availableCharacter.Id, 0f);
		}
	}

	private void InitExperienceCalculator()
	{
		_experienceCalculator = new ExperienceCalculator();
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<SaveGameController>.Instance, RegisterSaveGameLoadedEvent);
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<EnemyController>.Instance, RegisterEnemyControllerLoadedEvent);
	}

	internal void RegisterSaveGameLoadedEvent()
	{
		SingletonController<SaveGameController>.Instance.OnSaveGameLoaded += SaveGameController_OnSaveGameLoaded;
	}

	internal void RegisterEnemyControllerLoadedEvent()
	{
		SingletonController<EnemyController>.Instance.OnEnemyKilled += EnemyController_OnEnemyKilled;
	}

	private void EnemyController_OnEnemyKilled(object sender, EnemyKilledEventArgs e)
	{
		int experienceValue = e.Enemy.ExperienceValue;
		float calculatedStat = SingletonController<GameController>.Instance.Player.GetCalculatedStat(Enums.ItemStatType.ExperienceGainedPercentage);
		float experience = (1f + calculatedStat) * (float)experienceValue;
		AddExperience(experience);
	}

	private void SaveGameController_OnSaveGameLoaded(object sender, SaveGameLoadedArgs e)
	{
		if (e.SaveGame.CharacterExperienceState.HasData())
		{
			LoadFromSave(e.SaveGame.CharacterExperienceState);
			base.IsInitialized = true;
		}
	}

	private void LoadFromSave(CharacterExperienceSaveState characterExperienceSaveState)
	{
		Clear();
		foreach (KeyValuePair<int, int> characterLevel in characterExperienceSaveState.CharacterLevels)
		{
			_characterLevels.Add(characterLevel.Key, characterLevel.Value);
		}
		foreach (KeyValuePair<int, float> item in characterExperienceSaveState.CharacterRemainingExperience)
		{
			_characterExperience.Add(item.Key, item.Value);
		}
		ActiveCharacter = GameDatabaseHelper.GetCharacters().FirstOrDefault((CharacterSO x) => x.Id == characterExperienceSaveState.ActiveCharacterId);
		if (ActiveCharacter == null)
		{
			ActiveCharacter = GameDatabaseHelper.GetCharacters().FirstOrDefault();
		}
		SwitchCharacter(ActiveCharacter);
		this.OnCharacterLoaded?.Invoke(this, new CharacterSwitchedEventArgs(ActiveCharacter));
	}

	internal void SwitchCharacter(int characterId)
	{
		ActiveCharacter = GameDatabaseHelper.GetCharacters().FirstOrDefault((CharacterSO x) => x.Id == characterId);
		SwitchCharacter(ActiveCharacter);
	}

	internal void SetStartingCharacter()
	{
		CharacterSO activeCharacter = GameDatabaseHelper.GetCharacters().First((CharacterSO c) => c.Id == _startingCharacterId);
		ActiveCharacter = activeCharacter;
	}

	public override void Clear()
	{
		_characterLevels.Clear();
		_characterExperience.Clear();
	}

	public override void ClearAdventure()
	{
	}
}
