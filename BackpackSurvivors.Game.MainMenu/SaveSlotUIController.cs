using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.System;
using Tymski;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.MainMenu;

public class SaveSlotUIController : MonoBehaviour
{
	[SerializeField]
	private SaveSlotUIItem _saveSlotUIItemPrefab;

	[SerializeField]
	private Transform _saveSlotUIItemContainer;

	[SerializeField]
	private Button _loadButton;

	[SerializeField]
	private Button _newGameButton;

	[SerializeField]
	private Button _deleteButton;

	[SerializeField]
	private GameObject _deleteConfirmation;

	[SerializeField]
	private GameObject _deleteConfirmationBackdrop;

	private SaveSlotUIItem _selectedSaveSlotItem;

	private List<SaveSlotUIItem> _saveSlotUIItems;

	[SerializeField]
	private SceneReference _storyScene;

	[SerializeField]
	private SceneReference _townScene;

	[SerializeField]
	private MainMenuController _mainMenuController;

	private void Start()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<CollectionController>.Instance, LoadSavedSettings);
	}

	public void LoadSavedSettings()
	{
		LoadSaveSlots();
	}

	private void LoadSaveSlots()
	{
		ClearCurrentSaveSlotUIItems();
		SingletonController<SaveGameController>.Instance.RefreshSaveGames();
		List<SaveGame> saveGames = SingletonController<SaveGameController>.Instance.SaveGames;
		_saveSlotUIItems = new List<SaveSlotUIItem>();
		bool flag = saveGames.Count((SaveGame s) => s.HasData()) < 3;
		InitNewGameButtonState(flag);
		int num = 0;
		foreach (SaveGame item in saveGames.OrderByDescending((SaveGame x) => x.StatisticsState?.LastPlayed))
		{
			if (num >= 3)
			{
				break;
			}
			if (item.HasData() || flag)
			{
				SaveSlotUIItem saveSlotUIItem = UnityEngine.Object.Instantiate(_saveSlotUIItemPrefab, _saveSlotUIItemContainer);
				saveSlotUIItem.Init(item);
				_saveSlotUIItems.Add(saveSlotUIItem);
				num++;
				saveSlotUIItem.OnSaveSlotUIItemHovered = (SaveSlotUIItem.SaveSlotUIItemHoveredHandler)Delegate.Combine(saveSlotUIItem.OnSaveSlotUIItemHovered, new SaveSlotUIItem.SaveSlotUIItemHoveredHandler(SaveSlotUIItem_OnSaveSlotUIItemHovered));
				saveSlotUIItem.OnSaveSlotUIItemExitHovered = (SaveSlotUIItem.SaveSlotUIItemExitHoveredHandler)Delegate.Combine(saveSlotUIItem.OnSaveSlotUIItemExitHovered, new SaveSlotUIItem.SaveSlotUIItemExitHoveredHandler(SaveSlotUIItem_OnSaveSlotUIItemExitHovered));
				saveSlotUIItem.OnSaveSlotUIItemSelected = (SaveSlotUIItem.SaveSlotUIItemSelectedHandler)Delegate.Combine(saveSlotUIItem.OnSaveSlotUIItemSelected, new SaveSlotUIItem.SaveSlotUIItemSelectedHandler(SaveSlotUIItem_OnSaveSlotUIItemSelected));
			}
		}
	}

	private void ClearCurrentSaveSlotUIItems()
	{
		if (_saveSlotUIItems == null)
		{
			return;
		}
		foreach (SaveSlotUIItem saveSlotUIItem in _saveSlotUIItems)
		{
			saveSlotUIItem.OnSaveSlotUIItemHovered = (SaveSlotUIItem.SaveSlotUIItemHoveredHandler)Delegate.Remove(saveSlotUIItem.OnSaveSlotUIItemHovered, new SaveSlotUIItem.SaveSlotUIItemHoveredHandler(SaveSlotUIItem_OnSaveSlotUIItemHovered));
			saveSlotUIItem.OnSaveSlotUIItemExitHovered = (SaveSlotUIItem.SaveSlotUIItemExitHoveredHandler)Delegate.Remove(saveSlotUIItem.OnSaveSlotUIItemExitHovered, new SaveSlotUIItem.SaveSlotUIItemExitHoveredHandler(SaveSlotUIItem_OnSaveSlotUIItemExitHovered));
			saveSlotUIItem.OnSaveSlotUIItemSelected = (SaveSlotUIItem.SaveSlotUIItemSelectedHandler)Delegate.Remove(saveSlotUIItem.OnSaveSlotUIItemSelected, new SaveSlotUIItem.SaveSlotUIItemSelectedHandler(SaveSlotUIItem_OnSaveSlotUIItemSelected));
			UnityEngine.Object.Destroy(saveSlotUIItem.gameObject);
		}
	}

	public void DeleteSave()
	{
		_deleteConfirmationBackdrop.SetActive(value: true);
		_deleteConfirmation.SetActive(value: true);
		LeanTween.scaleX(_deleteConfirmation, 1f, 0.3f).setEaseInElastic().setEaseOutBounce()
			.setIgnoreTimeScale(useUnScaledTime: true);
	}

	public void ConfirmDeleteSave()
	{
		SingletonController<SaveGameController>.Instance.DeleteProgression(_selectedSaveSlotItem.Key);
		LoadSaveSlots();
		SelectFirstSlot(shouldHaveData: false);
		LeanTween.scaleX(_deleteConfirmation, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
		StartCoroutine(HideConfirmation(0.1f));
		_mainMenuController.SetContinueButton(SingletonController<SaveGameController>.Instance.ActiveSaveGame != null && SingletonController<SaveGameController>.Instance.ActiveSaveGame.HasData());
	}

	public void CancelDeleteSave()
	{
		LeanTween.scaleX(_deleteConfirmation, 0f, 0.1f).setIgnoreTimeScale(useUnScaledTime: true);
		StartCoroutine(HideConfirmation(0.1f));
	}

	private IEnumerator HideConfirmation(float delay)
	{
		_deleteConfirmationBackdrop.SetActive(value: false);
		yield return new WaitForSecondsRealtime(delay);
		_deleteConfirmation.SetActive(value: true);
	}

	public void StartNewSave()
	{
		LeanTween.scale(base.gameObject, new Vector3(1f, 0f, 1f), 0.3f);
		SingletonController<CharactersController>.Instance.SetStartingCharacter();
		SingletonController<SaveGameController>.Instance.SetActiveSaveGame(_selectedSaveSlotItem.SaveGame);
		SingletonController<GameController>.Instance.ClearControllersOfAllState();
		SingletonController<SaveGameController>.Instance.SaveProgression();
		SingletonController<SceneChangeController>.Instance.ChangeScene(_townScene.ScenePath, LoadSceneMode.Single, isLevelScene: false, showLoadingScreen: true, "Fortress");
	}

	public void LoadSave()
	{
		LeanTween.scale(base.gameObject, new Vector3(1f, 0f, 1f), 0.3f);
		SingletonController<SaveGameController>.Instance.SetActiveSaveGame(_selectedSaveSlotItem.SaveGame);
		SingletonController<SceneChangeController>.Instance.ChangeScene(_townScene.ScenePath, LoadSceneMode.Single, isLevelScene: false, showLoadingScreen: true, "Fortress");
	}

	private void SaveSlotUIItem_OnSaveSlotUIItemExitHovered(object sender, SaveSlotUIItemHoveredEventArgs e)
	{
		ResetHovers();
	}

	private void SaveSlotUIItem_OnSaveSlotUIItemHovered(object sender, SaveSlotUIItemHoveredEventArgs e)
	{
		ResetHovers();
		_saveSlotUIItems.FirstOrDefault((SaveSlotUIItem x) => x.Key == e.Key).SetHover(hovered: true);
	}

	private void ResetHovers()
	{
		foreach (SaveSlotUIItem saveSlotUIItem in _saveSlotUIItems)
		{
			saveSlotUIItem.SetHover(hovered: false);
		}
	}

	private void SaveSlotUIItem_OnSaveSlotUIItemSelected(object sender, SaveSlotUIItemSelectedEventArgs e)
	{
		ResetSelected();
		_selectedSaveSlotItem = _saveSlotUIItems.FirstOrDefault((SaveSlotUIItem x) => x.Key == e.Key);
		_selectedSaveSlotItem.SetSelected(selected: true);
		PrepareUI(_selectedSaveSlotItem.HasData);
	}

	private void PrepareUI(bool hasData)
	{
		_loadButton.gameObject.SetActive(hasData);
		_newGameButton.gameObject.SetActive(!hasData);
		_deleteButton.gameObject.SetActive(hasData);
		_loadButton.interactable = hasData;
		_newGameButton.interactable = !hasData;
	}

	private void InitNewGameButtonState(bool newGameAvailable)
	{
		_newGameButton.gameObject.SetActive(newGameAvailable);
		_newGameButton.interactable = newGameAvailable;
	}

	internal void ResetSelected()
	{
		foreach (SaveSlotUIItem saveSlotUIItem in _saveSlotUIItems)
		{
			saveSlotUIItem.SetSelected(selected: false);
		}
	}

	internal void SelectFirstSlot(bool shouldHaveData)
	{
		SaveSlotUIItem saveSlotUIItem = (from s in _saveSlotUIItems.Where((SaveSlotUIItem s) => s.HasData == shouldHaveData).ToList()
			orderby s.SaveGame.StatisticsState.LastPlayed descending
			select s).FirstOrDefault();
		if (saveSlotUIItem != null)
		{
			saveSlotUIItem.SetSelected(selected: true);
			_selectedSaveSlotItem = saveSlotUIItem;
			PrepareUI(saveSlotUIItem.HasData);
		}
	}
}
