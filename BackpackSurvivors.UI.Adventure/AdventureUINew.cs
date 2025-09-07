using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Shared;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.Game.Unlockables;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.UI.Shared;
using BackpackSurvivors.UI.Stats;
using BackpackSurvivors.UI.Tooltip;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureUINew : ModalUI
{
	public delegate void AdventureSelectedHandler(object sender, AdventureSelectedEventArgs e);

	[Header("Base")]
	[SerializeField]
	private AttackCameraToCanvas _attackCameraToCanvas;

	[Header("Adventure")]
	[SerializeField]
	private Transform _adventureOptionContainer;

	[SerializeField]
	private AdventureOption _adventureOptionPrefab;

	[SerializeField]
	private AdventureNavigationButton _previousAdventureButton;

	[SerializeField]
	private AdventureNavigationButton _nextAdventureButton;

	[SerializeField]
	private Transform _adventureBackgroundContainerTransform;

	[SerializeField]
	private AdventureOptionBackground _adventureBackgroundPrefab;

	[Header("Stats")]
	[SerializeField]
	private AdventureValuePercentage _rewardPercentage;

	[SerializeField]
	private AdventureValuePercentage _enemyCountPercentage;

	[SerializeField]
	private AdventureValuePercentage _enemyHealthPercentage;

	[SerializeField]
	private AdventureValuePercentage _enemyDamagePercentage;

	[SerializeField]
	private AdventureValuePercentage _experiencePercentage;

	[Header("Completion Reward")]
	[SerializeField]
	private AdventureValuePercentage _completionReward;

	[Header("FirstTime Completion")]
	[SerializeField]
	private Transform _firstTimeRewardContainerTransform;

	[SerializeField]
	private AdventureRewardItem _adventureRewardItemPrefab;

	[SerializeField]
	private AdventureRewardWeapon _adventureRewardWeaponPrefab;

	[SerializeField]
	private AdventureRewardBag _adventureRewardBagPrefab;

	[SerializeField]
	private AdventureRewardRelic _adventureRewardRelicPrefab;

	[SerializeField]
	private AdventureRewardCurrency _adventureRewardCurrencyPrefab;

	[SerializeField]
	private GameObject _firstTimeRewardCompletedPrefab;

	[Header("Level Visuals")]
	[SerializeField]
	private Transform _levelVisualContainerTransform;

	[SerializeField]
	private AdventureLevelVisual _levelVisualPrefab;

	[SerializeField]
	private AdventureLevelLine _levelVisualLinePrefab;

	[Header("Adventure Effects")]
	[SerializeField]
	private Transform _adventureEffectContainerTransform;

	[SerializeField]
	private AdventureEffectDetail _adventureEffectDetailPrefab;

	[Header("Hellfire")]
	[SerializeField]
	private GameObject _hellfireLockedOverlay;

	[SerializeField]
	private GameObject _hellfireNotAvailableLockedOverlay;

	[SerializeField]
	private Slider _hellfireSlider;

	[SerializeField]
	private Image _hellfireSliderFillImage;

	[SerializeField]
	private Material _hellfireFillingMaterial;

	[SerializeField]
	private Material _hellfireFullyFilledMaterial;

	[SerializeField]
	private TextMeshProUGUI[] _hellfireLevelTexts;

	[SerializeField]
	private Material _hellfireFullyFilledMaterialOnFinalStep;

	[SerializeField]
	private Material _hellfireNotFullyFilledMaterialOnFinalStep;

	[SerializeField]
	private Color _hellfireTextColorNotSelected;

	[SerializeField]
	private Color _hellfireTextColorSelected;

	[SerializeField]
	private AdventureHellfireLock[] _hellfireLocks;

	[SerializeField]
	private AdventureNavigationButton _lowerHellfireButton;

	[SerializeField]
	private AdventureNavigationButton _upHellfireButton;

	[Header("Audio")]
	[SerializeField]
	private AudioClip _adventureChangedAudioclip;

	[SerializeField]
	private AudioClip _adventureHellfireChangedAudioclip;

	[SerializeField]
	private AudioClip _adventureHellfireMaxedAudioclip;

	private AdventureSO[] _availableAdventures;

	private List<AdventureOption> _availableAdventureOptions;

	private AdventureOption _selectedAdventureOption;

	private int _previousHellfireLevel = -1;

	private float _adventureBackgroundWidth => ((RectTransform)_adventureBackgroundPrefab.transform).sizeDelta.x;

	public event AdventureSelectedHandler OnAdventureSelected;

	private void Start()
	{
		InitializeAvailableAdventures();
	}

	private void RegisterEvents()
	{
		SingletonController<InitializationController>.Instance.ExecuteCallbackWhenInitialized(SingletonController<InputController>.Instance, RegisterInputController);
	}

	private void RegisterInputController()
	{
		SingletonController<InputController>.Instance.OnNextHandler += InputController_OnNextHandler;
		SingletonController<InputController>.Instance.OnPreviousHandler += InputController_OnPreviousHandler;
		SingletonController<InputController>.Instance.OnAcceptHandler += InputController_OnAcceptHandler;
		SingletonController<InputController>.Instance.OnUpHandler += InputController_OnUpHandler;
		SingletonController<InputController>.Instance.OnDownHandler += InputController_OnDownHandler;
	}

	private void InputController_OnDownHandler(object sender, EventArgs e)
	{
		DecreaseHellfireLevel();
	}

	public void DecreaseHellfireLevel()
	{
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.Difficulties);
		unlockedCount = Math.Min(unlockedCount, SingletonController<DifficultyController>.Instance.GetMaxDifficulty());
		if (_hellfireSlider.value > (float)unlockedCount)
		{
			_hellfireSlider.value = unlockedCount;
		}
		else
		{
			_hellfireSlider.value -= 1f;
		}
	}

	private void InputController_OnUpHandler(object sender, EventArgs e)
	{
		IncreaseHellfireLevel();
	}

	public void IncreaseHellfireLevel()
	{
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.Difficulties);
		unlockedCount = Math.Min(unlockedCount, SingletonController<DifficultyController>.Instance.GetMaxDifficulty());
		if (_hellfireSlider.value > (float)unlockedCount)
		{
			_hellfireSlider.value = unlockedCount;
		}
		else
		{
			_hellfireSlider.value += 1f;
		}
	}

	private void UnregisterInputController()
	{
		SingletonController<InputController>.Instance.OnNextHandler -= InputController_OnNextHandler;
		SingletonController<InputController>.Instance.OnPreviousHandler -= InputController_OnPreviousHandler;
		SingletonController<InputController>.Instance.OnAcceptHandler -= InputController_OnAcceptHandler;
		SingletonController<InputController>.Instance.OnUpHandler -= InputController_OnUpHandler;
		SingletonController<InputController>.Instance.OnDownHandler -= InputController_OnDownHandler;
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		_attackCameraToCanvas.Refresh();
		base.OpenUI(openDirection);
		InitializeAvailableAdventures();
		InitializeHellfire();
	}

	public override void AfterOpenUI()
	{
		base.AfterOpenUI();
		RegisterEvents();
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.CloseUI(openDirection);
		UnregisterInputController();
	}

	private void InputController_OnAcceptHandler(object sender, EventArgs e)
	{
		StartAdventureButtonPressed();
	}

	private void InputController_OnPreviousHandler(object sender, EventArgs e)
	{
		SelectedPreviousAdventure();
	}

	private void InputController_OnNextHandler(object sender, EventArgs e)
	{
		SelectedNextAdventure();
	}

	private void UpdateButtonState(int adventureId)
	{
		int num = _availableAdventureOptions.IndexOf(_availableAdventureOptions.FirstOrDefault((AdventureOption x) => x.Adventure.AdventureId == adventureId));
		if (num + 1 < _availableAdventureOptions.Count() && _availableAdventureOptions[num + 1].Adventure.Available)
		{
			_nextAdventureButton.SetInteractable(interactable: true);
		}
		else
		{
			_nextAdventureButton.SetInteractable(interactable: false);
		}
		if (num - 1 >= 0 && _availableAdventureOptions[num - 1].Adventure.Available)
		{
			_previousAdventureButton.SetInteractable(interactable: true);
		}
		else
		{
			_previousAdventureButton.SetInteractable(interactable: false);
		}
	}

	public void StartAdventureButtonPressed()
	{
		this.OnAdventureSelected?.Invoke(this, new AdventureSelectedEventArgs(_selectedAdventureOption.Adventure));
	}

	public void ExitButtonPressed()
	{
		UnityEngine.Object.FindObjectOfType<ModalUiController>().CloseModalUI(Enums.ModalUITypes.Adventure);
	}

	internal void Reset()
	{
		InitializeAvailableAdventures();
	}

	private void InitializeAvailableAdventures()
	{
		_availableAdventureOptions = new List<AdventureOption>();
		_availableAdventures = (from x in GameDatabaseHelper.GetAdventures()
			orderby x.AdventureId
			select x).ToArray();
		SetupAdventureOptions();
	}

	private void SetupAdventureOptions()
	{
		foreach (AdventureOption availableAdventureOption in _availableAdventureOptions)
		{
			availableAdventureOption.OnAdventureOptionSelected -= AdventureOption_OnAdventureOptionSelected;
		}
		foreach (Transform item in _adventureOptionContainer)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Transform item2 in _adventureBackgroundContainerTransform)
		{
			UnityEngine.Object.Destroy(item2.gameObject);
		}
		AdventureSO[] availableAdventures = _availableAdventures;
		foreach (AdventureSO adventure in availableAdventures)
		{
			AdventureOption adventureOption = UnityEngine.Object.Instantiate(_adventureOptionPrefab, _adventureOptionContainer);
			adventureOption.Init(adventure);
			adventureOption.OnAdventureOptionSelected += AdventureOption_OnAdventureOptionSelected;
			_availableAdventureOptions.Add(adventureOption);
		}
		availableAdventures = _availableAdventures;
		foreach (AdventureSO adventure2 in availableAdventures)
		{
			UnityEngine.Object.Instantiate(_adventureBackgroundPrefab, _adventureBackgroundContainerTransform).Init(adventure2);
		}
		_selectedAdventureOption = _availableAdventureOptions.FirstOrDefault((AdventureOption x) => x.Adventure.Available);
		if (_selectedAdventureOption != null)
		{
			AdventureSO adventure3 = _selectedAdventureOption.Adventure;
			SelectAdventure(adventure3.AdventureId);
		}
	}

	private void AdventureOption_OnAdventureOptionSelected(object sender, AdventureOptionSelectedeventArgs e)
	{
		SelectAdventure(e.SelectedAdventure.AdventureId);
	}

	private void SelectedNextAdventure()
	{
		int num = _availableAdventureOptions.IndexOf(_availableAdventureOptions.FirstOrDefault((AdventureOption x) => x.Adventure.AdventureId == _selectedAdventureOption.Adventure.AdventureId));
		if (_availableAdventureOptions.Count() - 1 >= num + 1 && _availableAdventureOptions[num + 1].Adventure.Available)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_adventureChangedAudioclip, 1f);
			SelectAdventure(_availableAdventureOptions[num + 1].Adventure.AdventureId);
		}
	}

	private void SelectedPreviousAdventure()
	{
		int num = _availableAdventureOptions.IndexOf(_availableAdventureOptions.FirstOrDefault((AdventureOption x) => x.Adventure.AdventureId == _selectedAdventureOption.Adventure.AdventureId));
		if (num != 0)
		{
			SingletonController<AudioController>.Instance.PlaySFXClip(_adventureChangedAudioclip, 1f);
			SelectAdventure(_availableAdventureOptions[num - 1].Adventure.AdventureId);
		}
	}

	private void SelectAdventure(int adventureId)
	{
		SingletonController<TooltipController>.Instance.Hide(null);
		_selectedAdventureOption = _availableAdventureOptions.FirstOrDefault((AdventureOption x) => x.Adventure.AdventureId == adventureId);
		if (!_selectedAdventureOption.Adventure.Available)
		{
			return;
		}
		foreach (AdventureOption availableAdventureOption in _availableAdventureOptions)
		{
			if (availableAdventureOption.Adventure.AdventureId == adventureId)
			{
				availableAdventureOption.SetSelect(selected: true);
			}
			else
			{
				availableAdventureOption.SetSelect(selected: false);
			}
		}
		ScrollToAdventureBackground();
		SetAdventureStats();
		SetLevelVisuals();
		SetCompletionReward();
		SetFirstTimeCompletionRewards();
		SetAdventureEffectDetails();
		SetHellfireAvailability(_selectedAdventureOption);
		UpdateButtonState(adventureId);
	}

	private void SetHellfireAvailability(AdventureOption selectedAdventureOption)
	{
		_hellfireNotAvailableLockedOverlay.SetActive(!selectedAdventureOption.Adventure.AllowHellfire);
	}

	private void ScrollToAdventureBackground()
	{
		float adventureBackgroundWidth = _adventureBackgroundWidth;
		float to = (float)_availableAdventureOptions.IndexOf(_availableAdventureOptions.FirstOrDefault((AdventureOption x) => x.Adventure.AdventureId == _selectedAdventureOption.Adventure.AdventureId)) * (0f - adventureBackgroundWidth) - adventureBackgroundWidth / 2f;
		LeanTween.moveLocalX(_adventureBackgroundContainerTransform.gameObject, to, 0.2f).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void SetAdventureStats()
	{
		float value = _hellfireSlider.value;
		if (value == _hellfireSlider.maxValue)
		{
			_rewardPercentage.ChangeValue(value * _selectedAdventureOption.Adventure.MaxHellfireCoinMultiplier);
			_enemyCountPercentage.ChangeValue(value * _selectedAdventureOption.Adventure.MaxHellfireEnemyCountMultiplier);
			_enemyHealthPercentage.ChangeValue(value * _selectedAdventureOption.Adventure.MaxHellfireEnemyHealthMultiplier);
			_enemyDamagePercentage.ChangeValue(value * _selectedAdventureOption.Adventure.MaxHellfireEnemyDamageMultiplier);
			_experiencePercentage.ChangeValue(value * _selectedAdventureOption.Adventure.MaxHellfireExperienceMultiplier);
		}
		else
		{
			_rewardPercentage.ChangeValue(value * _selectedAdventureOption.Adventure.HellfireCoinMultiplier);
			_enemyCountPercentage.ChangeValue(value * _selectedAdventureOption.Adventure.HellfireEnemyCountMultiplier);
			_enemyHealthPercentage.ChangeValue(value * _selectedAdventureOption.Adventure.HellfireEnemyHealthMultiplier);
			_enemyDamagePercentage.ChangeValue(value * _selectedAdventureOption.Adventure.HellfireEnemyDamageMultiplier);
			_experiencePercentage.ChangeValue(value * _selectedAdventureOption.Adventure.HellfireExperienceMultiplier);
		}
	}

	private void SetLevelVisuals()
	{
		foreach (Transform item in _levelVisualContainerTransform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (LevelSO item2 in _selectedAdventureOption.Adventure.Levels.Where((LevelSO x) => x.ShouldShowInList))
		{
			UnityEngine.Object.Instantiate(_levelVisualLinePrefab, _levelVisualContainerTransform).Init(completed: true);
			UnityEngine.Object.Instantiate(_levelVisualPrefab, _levelVisualContainerTransform).Init(item2, completed: true);
		}
	}

	private void SetFirstTimeCompletionRewards()
	{
		foreach (Transform item in _firstTimeRewardContainerTransform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		int currentHellfireLevel = (int)_hellfireSlider.value;
		if (SingletonController<StatisticsController>.Instance.HighestCompletedHellfireLevelForAdventure(_selectedAdventureOption.Adventure.AdventureId) < currentHellfireLevel)
		{
			foreach (RewardSO item2 in _selectedAdventureOption.Adventure.CompletionRewards.Where((RewardSO x) => !x.Repeatable && x.HellfireLevel == currentHellfireLevel))
			{
				switch (item2.CompletionRewardType)
				{
				case Enums.RewardType.Weapon:
					UnityEngine.Object.Instantiate(_adventureRewardWeaponPrefab, _firstTimeRewardContainerTransform).Init((WeaponSO)item2.CompletionReward, unlocked: true, interactable: false);
					break;
				case Enums.RewardType.Item:
					UnityEngine.Object.Instantiate(_adventureRewardItemPrefab, _firstTimeRewardContainerTransform).Init((ItemSO)item2.CompletionReward, unlocked: true, interactable: false);
					break;
				case Enums.RewardType.Bag:
					UnityEngine.Object.Instantiate(_adventureRewardBagPrefab, _firstTimeRewardContainerTransform).Init((BagSO)item2.CompletionReward, unlocked: true, interactable: false);
					break;
				case Enums.RewardType.Relic:
					UnityEngine.Object.Instantiate(_adventureRewardRelicPrefab, _firstTimeRewardContainerTransform).Init((RelicSO)item2.CompletionReward, unlocked: true, interactable: false);
					break;
				case Enums.RewardType.TitanicSouls:
					UnityEngine.Object.Instantiate(_adventureRewardCurrencyPrefab, _firstTimeRewardContainerTransform).Init(Enums.CurrencyType.TitanSouls, item2.Amount, unlocked: true, interactable: false);
					break;
				}
			}
			return;
		}
		UnityEngine.Object.Instantiate(_firstTimeRewardCompletedPrefab, _firstTimeRewardContainerTransform);
	}

	private void SetCompletionReward()
	{
		float value = _hellfireSlider.value;
		float titanicSoulMultiplier = _selectedAdventureOption.Adventure.TitanicSoulMultiplier;
		if (value == _hellfireSlider.maxValue)
		{
			_completionReward.ChangeValue($"{_selectedAdventureOption.Adventure.TitanicSoulBaseValueOnInfinite}/min");
			return;
		}
		RewardSO rewardSO = _selectedAdventureOption.Adventure.CompletionRewards.FirstOrDefault((RewardSO x) => x.Repeatable && x.CompletionRewardType == Enums.RewardType.TitanicSouls);
		if (rewardSO != null)
		{
			_completionReward.ChangeValue((float)rewardSO.Amount + (float)rewardSO.Amount * (value * titanicSoulMultiplier), percentage: false);
		}
	}

	private void InitializeHellfire()
	{
		if (_selectedAdventureOption.Adventure.AllowHellfire)
		{
			bool flag = SingletonController<UnlocksController>.Instance.IsUnlocked(Enums.Unlockable.Difficulties);
			_hellfireLockedOverlay.SetActive(!flag);
			SetLocks();
			_hellfireNotAvailableLockedOverlay.SetActive(value: false);
		}
		else
		{
			_hellfireNotAvailableLockedOverlay.SetActive(value: true);
		}
	}

	private void SetLocks()
	{
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.Difficulties);
		unlockedCount = Math.Min(unlockedCount, SingletonController<DifficultyController>.Instance.GetMaxDifficulty());
		for (int i = 0; i < _hellfireLocks.Length; i++)
		{
			if (i != unlockedCount - 1)
			{
				_hellfireLocks[i].SetLock(locked: false);
			}
			else
			{
				_hellfireLocks[i].SetLock(locked: true);
			}
		}
	}

	public void HellfireValueChanged()
	{
		SingletonController<TooltipController>.Instance.Hide(null);
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.Difficulties);
		unlockedCount = Math.Min(unlockedCount, SingletonController<DifficultyController>.Instance.GetMaxDifficulty());
		if (_hellfireSlider.value > (float)unlockedCount)
		{
			_hellfireSlider.value = unlockedCount;
		}
		else if ((float)_previousHellfireLevel != _hellfireSlider.value)
		{
			_previousHellfireLevel = (int)_hellfireSlider.value;
			float value = _hellfireSlider.value;
			SetAdventureStats();
			SetCompletionReward();
			SetFirstTimeCompletionRewards();
			SetHellfireSliderMaterial(value);
			SetHellfireLevelTextAndImageMaterial(value);
			SetAdventureEffectDetails();
			if (_hellfireSlider.maxValue == value)
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_adventureHellfireMaxedAudioclip, 1f);
			}
			else
			{
				SingletonController<AudioController>.Instance.PlaySFXClip(_adventureHellfireChangedAudioclip, 1f);
			}
			SingletonController<DifficultyController>.Instance.ChangeDifficulty((int)value, save: true);
			_hellfireSlider.interactable = false;
			_hellfireSlider.interactable = true;
			UpdateHellfireButtonState();
		}
	}

	private void UpdateHellfireButtonState()
	{
		int unlockedCount = SingletonController<UnlocksController>.Instance.GetUnlockedCount(Enums.Unlockable.Difficulties);
		unlockedCount = Math.Min(unlockedCount, SingletonController<DifficultyController>.Instance.GetMaxDifficulty());
		int num = (int)_hellfireSlider.value;
		if (num < unlockedCount)
		{
			_upHellfireButton.SetInteractable(interactable: true);
		}
		else
		{
			_upHellfireButton.SetInteractable(interactable: false);
		}
		if (num - 1 >= 0)
		{
			_lowerHellfireButton.SetInteractable(interactable: true);
		}
		else
		{
			_lowerHellfireButton.SetInteractable(interactable: false);
		}
	}

	private void SetAdventureEffectDetails()
	{
		float value = _hellfireSlider.value;
		AdventureSO adventure = _selectedAdventureOption.Adventure;
		foreach (Transform item in _adventureEffectContainerTransform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (AdventureEffectSO adventureEffect in adventure.AdventureEffects)
		{
			if ((float)adventureEffect.ActiveFromHellfireLevel <= value)
			{
				UnityEngine.Object.Instantiate(_adventureEffectDetailPrefab, _adventureEffectContainerTransform).Init(adventureEffect.Icon, adventureEffect.Name, adventureEffect.Description);
			}
		}
	}

	private void SetHellfireLevelTextAndImageMaterial(float selectedValue)
	{
		for (int i = 0; i < _hellfireLevelTexts.Length; i++)
		{
			_hellfireLevelTexts[i].color = ((selectedValue < (float)i) ? _hellfireTextColorSelected : _hellfireTextColorNotSelected);
		}
	}

	private void SetHellfireSliderMaterial(float selectedValue)
	{
		if (_hellfireSlider.maxValue == selectedValue)
		{
			_hellfireSliderFillImage.material = _hellfireFullyFilledMaterial;
		}
		else
		{
			_hellfireSliderFillImage.material = _hellfireFillingMaterial;
		}
	}

	private void OnDestroy()
	{
		UnregisterInputController();
	}
}
