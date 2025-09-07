using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Unlockable;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Tooltip.Triggers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.Game.MainMenu;

public class SaveSlotUIItem : MonoBehaviour
{
	public delegate void SaveSlotUIItemSelectedHandler(object sender, SaveSlotUIItemSelectedEventArgs e);

	public delegate void SaveSlotUIItemHoveredHandler(object sender, SaveSlotUIItemHoveredEventArgs e);

	public delegate void SaveSlotUIItemExitHoveredHandler(object sender, SaveSlotUIItemHoveredEventArgs e);

	[Header("Prefabs")]
	[SerializeField]
	private SaveSlotUnlockedUIItem _saveSlotUnlockedUIItemPrefab;

	[SerializeField]
	private Transform _unlockSlotContainer;

	[Header("Stats")]
	[SerializeField]
	private TextMeshProUGUI _playtime;

	[SerializeField]
	private TextMeshProUGUI _lastPlayed;

	[SerializeField]
	private TextMeshProUGUI _titanSoulCount;

	[SerializeField]
	private TextMeshProUGUI _playerLevel;

	[SerializeField]
	private Image _selectedImage;

	[SerializeField]
	private Image _hoveredImage;

	[Header("Progression")]
	[SerializeField]
	private Image _progressBar;

	[SerializeField]
	private TextMeshProUGUI _progressCount;

	[SerializeField]
	private Sprite _progressFull;

	[SerializeField]
	private Sprite _progressNotFull;

	[Header("UI")]
	[SerializeField]
	private GameObject _emptySlotOverlay;

	[SerializeField]
	private GameObject _notSelectedOverlay;

	[SerializeField]
	private DefaultTooltipTrigger _defaultTooltipTrigger;

	private bool _isSelected;

	public SaveSlotUIItemSelectedHandler OnSaveSlotUIItemSelected;

	public SaveSlotUIItemHoveredHandler OnSaveSlotUIItemHovered;

	public SaveSlotUIItemExitHoveredHandler OnSaveSlotUIItemExitHovered;

	public Guid Key => SaveGame.key;

	public bool HasData => SaveGame.HasData();

	public SaveGame SaveGame { get; private set; }

	public void Init(SaveGame saveGame)
	{
		SaveGame = saveGame;
		if (HasData)
		{
			foreach (KeyValuePair<Enums.Unlockable, int> item in saveGame.UnlockedUpgradesState.UnlockedUpgrades.Where((KeyValuePair<Enums.Unlockable, int> x) => x.Value > 0))
			{
				UnlockableSO unlockableFromType = GameDatabaseHelper.GetUnlockableFromType(item.Key);
				if (unlockableFromType != null)
				{
					UnityEngine.Object.Instantiate(_saveSlotUnlockedUIItemPrefab, _unlockSlotContainer).Init(unlockableFromType, item.Value);
				}
			}
		}
		_playtime.SetText(StringHelper.FromSecondsToCleanTimespan(SaveGame.StatisticsState.PlayedTime));
		_lastPlayed.SetText(SaveGame.StatisticsState.LastPlayed.ToString("dddd, dd MMMM yyyy"));
		_playerLevel.SetText(SaveGame.CharacterExperienceState.GetLevelByCharacterId(SaveGame.CharacterExperienceState.ActiveCharacterId).ToString());
		_titanSoulCount.SetText(SaveGame.CurrencyState.GetCurrencyValue(Enums.CurrencyType.TitanSouls).ToString());
		int totalAvailableUnlocks = SingletonController<CollectionController>.Instance.GetTotalAvailableUnlocks();
		int totalUnlockedCount = saveGame.CollectionsSaveState.GetTotalUnlockedCount();
		float num = (float)totalUnlockedCount / (float)totalAvailableUnlocks;
		decimal num2 = Math.Round((decimal)(num * 100f), 0, MidpointRounding.AwayFromZero);
		_progressCount.SetText($"{num2}%");
		_progressBar.fillAmount = num;
		if (totalUnlockedCount >= totalAvailableUnlocks)
		{
			_progressBar.sprite = _progressFull;
		}
		else
		{
			_progressBar.sprite = _progressNotFull;
		}
		_defaultTooltipTrigger.SetDefaultContent($"{num2}% collected", $"you have <color={ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase)}>{totalUnlockedCount}</color> out of <color={ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase)}>{totalAvailableUnlocks}</color> enemies, items, weapons, relics, bags and recipes found!", active: true);
		_emptySlotOverlay.SetActive(!saveGame.HasData());
	}

	public void Hover()
	{
		if (!_isSelected)
		{
			_notSelectedOverlay.SetActive(value: false);
		}
		OnSaveSlotUIItemHovered?.Invoke(this, new SaveSlotUIItemHoveredEventArgs(Key));
	}

	public void ExitHover()
	{
		if (!_isSelected)
		{
			_notSelectedOverlay.SetActive(value: true);
		}
		OnSaveSlotUIItemExitHovered?.Invoke(this, new SaveSlotUIItemHoveredEventArgs(Key));
	}

	public void Select()
	{
		OnSaveSlotUIItemSelected?.Invoke(this, new SaveSlotUIItemSelectedEventArgs(Key));
	}

	public void SetHover(bool hovered)
	{
		_hoveredImage.gameObject.SetActive(hovered);
	}

	public void SetSelected(bool selected)
	{
		_isSelected = selected;
		_notSelectedOverlay.SetActive(!selected);
		_selectedImage.gameObject.SetActive(selected);
	}
}
