using System;
using System.Collections.Generic;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Unlockable;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Saving.SavefileEditorComponents;
using BackpackSurvivors.System.Saving.SavefileEditorComponents.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.System.Saving;

internal class SaveFileEditorUI : MonoBehaviour
{
	[SerializeField]
	private TMP_InputField _savegameGuidInputfield;

	[Header("Base")]
	[SerializeField]
	private SerializableDictionaryBase<GameObject, GameObject> _buttonPanelPairs;

	[Header("Currency")]
	[SerializeField]
	private TMP_InputField _coinsInputfield;

	[SerializeField]
	private TMP_InputField _titanicInputfield;

	[Header("Character")]
	[SerializeField]
	private CharacterSavefileEditor _characterSavefileEditorPrefab;

	[SerializeField]
	private Transform _characterEditorsParentTransform;

	[Header("Unlockables")]
	[SerializeField]
	private UnlockableSavefileEditor _unlockableSavefileEditorPrefab;

	[SerializeField]
	private Transform _unlockablesEditorsParentTransform;

	internal string SaveGameGuidString => _savegameGuidInputfield.text;

	internal void SetSavegameGuid(string savegameGuidsString)
	{
		_savegameGuidInputfield.text = savegameGuidsString;
	}

	internal void LoadCurrencyState(CurrencySaveState currencyState)
	{
		_coinsInputfield.text = currencyState.GetCurrencyValue(Enums.CurrencyType.Coins).ToString();
		_titanicInputfield.text = currencyState.GetCurrencyValue(Enums.CurrencyType.TitanSouls).ToString();
	}

	internal void SetCurrencyState(CurrencySaveState currencyState)
	{
		Dictionary<Enums.CurrencyType, int> metaProgressionCurrencies = new Dictionary<Enums.CurrencyType, int>
		{
			{
				Enums.CurrencyType.Coins,
				int.Parse(_coinsInputfield.text)
			},
			{
				Enums.CurrencyType.TitanSouls,
				int.Parse(_titanicInputfield.text)
			}
		};
		Dictionary<Enums.CraftingResource, int> craftingResources = new Dictionary<Enums.CraftingResource, int>();
		currencyState.SetState(metaProgressionCurrencies, craftingResources);
	}

	internal void LoadCharacterState(CharacterExperienceSaveState characterExperienceState)
	{
		foreach (CharacterSO character in GameDatabaseHelper.GetCharacters())
		{
			int id = character.Id;
			string characterName = character.Name;
			int levelByCharacterId = characterExperienceState.GetLevelByCharacterId(id);
			float experienceByCharacterId = characterExperienceState.GetExperienceByCharacterId(id);
			int activeCharacterId = characterExperienceState.ActiveCharacterId;
			CharacterSavefileEditor characterSavefileEditor = UnityEngine.Object.Instantiate(_characterSavefileEditorPrefab, _characterEditorsParentTransform);
			characterSavefileEditor.LoadCharacter(characterName, id, levelByCharacterId, experienceByCharacterId, activeCharacterId);
			characterSavefileEditor.OnActiveCharacterChanged = (CharacterSavefileEditor.ActiveCharacterChangedHandler)Delegate.Combine(characterSavefileEditor.OnActiveCharacterChanged, new CharacterSavefileEditor.ActiveCharacterChangedHandler(CharacterSavegameEditor_OnActiveCharacterChanged));
		}
	}

	private void CharacterSavegameEditor_OnActiveCharacterChanged(object sender, ActiveCharacterChangedEventArgs e)
	{
		CharacterSavefileEditor[] componentsInChildren = _characterEditorsParentTransform.GetComponentsInChildren<CharacterSavefileEditor>();
		foreach (CharacterSavefileEditor obj in componentsInChildren)
		{
			bool setActive = obj.CharacterId == e.ActivatedCharacterId;
			obj.SetActive(setActive);
		}
	}

	internal void SetCharacterState(CharacterExperienceSaveState characterExperienceSaveState)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Dictionary<int, float> dictionary2 = new Dictionary<int, float>();
		int activeCharacterId = -1;
		CharacterSavefileEditor[] componentsInChildren = _characterEditorsParentTransform.GetComponentsInChildren<CharacterSavefileEditor>();
		foreach (CharacterSavefileEditor characterSavefileEditor in componentsInChildren)
		{
			if (characterSavefileEditor.IsActiveCharacter)
			{
				activeCharacterId = characterSavefileEditor.CharacterId;
			}
			dictionary.Add(characterSavefileEditor.CharacterId, characterSavefileEditor.CharacterLevels);
			dictionary2.Add(characterSavefileEditor.CharacterId, characterSavefileEditor.CharacterExperience);
		}
		characterExperienceSaveState.SetState(activeCharacterId, dictionary, dictionary2);
	}

	internal void LoadUnlockablesState(UnlockedsSaveState unlockedsSaveState)
	{
		foreach (UnlockableSO unlockable in GameDatabaseHelper.GetUnlockables())
		{
			int pointsInvested = (unlockedsSaveState.UnlockedUpgrades.ContainsKey(unlockable.Unlockable) ? unlockedsSaveState.UnlockedUpgrades[unlockable.Unlockable] : 0);
			UnityEngine.Object.Instantiate(_unlockableSavefileEditorPrefab, _unlockablesEditorsParentTransform).LoadUnlockable(unlockable.Unlockable, pointsInvested);
		}
	}

	internal void SetUnlockablesState(UnlockedsSaveState unlockedsSaveState)
	{
		Dictionary<Enums.Unlockable, int> dictionary = new Dictionary<Enums.Unlockable, int>();
		UnlockableSavefileEditor[] componentsInChildren = _unlockablesEditorsParentTransform.GetComponentsInChildren<UnlockableSavefileEditor>();
		foreach (UnlockableSavefileEditor obj in componentsInChildren)
		{
			Enums.Unlockable unlockable = obj.Unlockable;
			int pointsInvested = obj.PointsInvested;
			dictionary.Add(unlockable, pointsInvested);
		}
		unlockedsSaveState.SetState(dictionary);
	}

	private void Start()
	{
		RegisterButtonEvents();
	}

	private void RegisterButtonEvents()
	{
		foreach (KeyValuePair<GameObject, GameObject> buttonPanelPair in _buttonPanelPairs)
		{
			Button button = buttonPanelPair.Key.GetComponent<Button>();
			button.onClick.AddListener(delegate
			{
				RegisterButtonPanelSwitch(buttonPanelPair.Value, button);
			});
		}
	}

	private void RegisterButtonPanelSwitch(GameObject panelToEnable, Button button)
	{
		DisableAllPanels();
		ClearAllButtonColors();
		button.GetComponent<Image>().color = Color.cyan;
		panelToEnable.SetActive(value: true);
	}

	private void ClearAllButtonColors()
	{
		foreach (GameObject key in _buttonPanelPairs.Keys)
		{
			key.GetComponent<Image>().color = Color.white;
		}
	}

	private void DisableAllPanels()
	{
		foreach (GameObject value in _buttonPanelPairs.Values)
		{
			value.SetActive(value: false);
		}
	}

	internal void DeleteAllGeneratedEditors()
	{
		DeleteAllChildrenInTransform<CharacterSavefileEditor>(_characterEditorsParentTransform);
		DeleteAllChildrenInTransform<UnlockableSavefileEditor>(_unlockablesEditorsParentTransform);
	}

	private void DeleteAllChildrenInTransform<T>(Transform transform) where T : MonoBehaviour
	{
		T[] componentsInChildren = transform.GetComponentsInChildren<T>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
	}
}
