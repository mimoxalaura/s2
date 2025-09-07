using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Level;
using BackpackSurvivors.Game.Levels;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.ScriptableObjects.Classes;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Collection.ListItems.Relic;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureCompletedUINew : ModalUI
{
	public delegate void AdventureCompleteButtonClickedHandler(object sender, EventArgs e);

	[Header("Base")]
	[SerializeField]
	private GameObject _succesfulTitleObject;

	[SerializeField]
	private GameObject _failedTitleObject;

	[SerializeField]
	private GameObject _succesfulBackdropObject;

	[SerializeField]
	private GameObject _failedBackdropObject;

	[Header("Adventure")]
	[SerializeField]
	private Image _backgroundImage;

	[SerializeField]
	private TextMeshProUGUI _adventureTitle;

	[Header("Level Visuals")]
	[SerializeField]
	private AdventureCompletedLevelVisual _adventureCompletedLevelVisualPrefab;

	[SerializeField]
	private AdventureCompletedLevelLine _adventureCompletedLevelLinePrefab;

	[SerializeField]
	private Transform _levelVisualContainerTransform;

	[Header("Hellfire")]
	[SerializeField]
	private TextMeshProUGUI _hellfireText;

	[SerializeField]
	private Image _hellfireLineImage;

	[SerializeField]
	private Material _hellfireLineActiveMaterial;

	[SerializeField]
	private Material _hellfireLineMaxLevelMaterial;

	[Header("Adventure Effects")]
	[SerializeField]
	private Transform _adventureEffectContainerTransform;

	[SerializeField]
	private AdventureEffectDetail _adventureEffectDetailPrefab;

	[Header("Weapons")]
	[SerializeField]
	private Transform _weaponsContainer;

	[SerializeField]
	private AdventureCompletedWeaponUI _adventureCompletedWeaponUIPrefab;

	[Header("Stats")]
	[SerializeField]
	private TextMeshProUGUI _totalGoldEarned;

	[SerializeField]
	private TextMeshProUGUI _titanicSoulsGained;

	[SerializeField]
	private TextMeshProUGUI _enemiesDefeated;

	[SerializeField]
	private TextMeshProUGUI _experienceGained;

	[SerializeField]
	private TextMeshProUGUI _totalAdventureClears;

	[Header("Relics")]
	[SerializeField]
	private Transform _relicsContainer;

	[SerializeField]
	private CollectionRelicUI _collectionRelicUIPrefab;

	[Header("Completion Reward")]
	[SerializeField]
	private Transform _rewardContainerTransform;

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

	[Header("Completion Reward Locked")]
	[SerializeField]
	private GameObject _rewardLockedOverlay;

	[SerializeField]
	private Image _chainLeft;

	[SerializeField]
	private Image _chainRight;

	[SerializeField]
	private AudioClip _chainsAudioClip;

	[Header("Experience")]
	[SerializeField]
	private Image _experienceBar;

	[SerializeField]
	private TextMeshProUGUI _experienceBarText;

	[SerializeField]
	private TextMeshProUGUI _experienceFromKills;

	[SerializeField]
	private TextMeshProUGUI _currentLevel;

	[Header("Leveling")]
	[SerializeField]
	private ParticleSystem _levelUpSystem;

	[SerializeField]
	private Animator _playerAnimator;

	[SerializeField]
	private SerializableDictionaryBase<CharacterSO, Image> _playerImages;

	[SerializeField]
	private int _maxTicks = 50;

	[SerializeField]
	private float _timePerTick = 0.02f;

	[SerializeField]
	private AudioClip _levelAudioClip;

	private float _scaleEffect = 1.3f;

	private float _scaleUpDuration = 0.1f;

	private float _scaleDownDuration = 0.2f;

	private string _shaderNameMetal = "_MetalFade";

	private float _showAnimationTime = 0.5f;

	private float _hideAnimationTime = 0.3f;

	private float _delay = 0.5f;

	public event AdventureCompleteButtonClickedHandler OnAdventureCompleteButtonClicked;

	public void Init(AdventureSO adventure, bool succes)
	{
		SetSuccesOrVictory(succes);
		SetBackgrounds(adventure);
		ShowLevelVisuals(adventure, succes);
		ShowHellfire(adventure);
		ShowWeaponStats();
		ShowStats();
		ShowRelics();
		ShowRewards(adventure);
		ShowRewardLock(succes);
		StartExperienceAnimation();
	}

	private void StartExperienceAnimation()
	{
		foreach (KeyValuePair<CharacterSO, Image> playerImage in _playerImages)
		{
			playerImage.Value.gameObject.SetActive(playerImage.Key == SingletonController<CharactersController>.Instance.ActiveCharacter);
		}
		Time.timeScale = 1f;
		StartCoroutine(RunExperienceCounterBarAnimation());
	}

	public IEnumerator RunExperienceCounterBarAnimation()
	{
		int num = (int)SingletonController<CharactersController>.Instance.OriginalExperience;
		int originalLevel = SingletonController<CharactersController>.Instance.OriginalLevel;
		int num2 = (int)SingletonController<CharactersController>.Instance.TotalCurrentExperience;
		_ = SingletonController<CharactersController>.Instance.CurrentLevel;
		int num3 = num2 - num;
		_experienceFromKills.SetText($"{num3}");
		_currentLevel.SetText($"{originalLevel}");
		float experienceNeededForLevel = SingletonController<CharactersController>.Instance.ExperienceCalculator.GetExperienceNeededForLevel(originalLevel + 1);
		SetExperienceBar(num, experienceNeededForLevel);
		int maximumTicks = Mathf.Min(num3, _maxTicks);
		int remainingExperience = num3;
		int levelIterator = originalLevel;
		int levelStartExperience = num;
		yield return new WaitForSeconds(2f);
		while (remainingExperience > 0)
		{
			int levelTargetExperience = (int)SingletonController<CharactersController>.Instance.ExperienceCalculator.GetExperienceNeededForLevel(levelIterator);
			if (remainingExperience < levelTargetExperience)
			{
				SetExperienceBar(levelStartExperience, levelTargetExperience);
				int experienceToReceiveThisLevel = remainingExperience;
				int experiencePerTick = experienceToReceiveThisLevel / maximumTicks;
				experiencePerTick = Mathf.Max(1, experiencePerTick);
				int ticks = experienceToReceiveThisLevel / experiencePerTick;
				for (int i = 0; i < ticks; i++)
				{
					levelStartExperience += experiencePerTick;
					SetExperienceBar(levelStartExperience, levelTargetExperience);
					_experienceFromKills.SetText($"{remainingExperience - experiencePerTick * i}");
					yield return new WaitForSeconds(_timePerTick);
				}
				remainingExperience -= experienceToReceiveThisLevel;
			}
			else if (levelStartExperience > 0 && remainingExperience >= levelTargetExperience)
			{
				SetExperienceBar(levelStartExperience, levelTargetExperience);
				int experienceToReceiveThisLevel = levelTargetExperience - levelStartExperience;
				int ticks = experienceToReceiveThisLevel / maximumTicks;
				ticks = Mathf.Max(1, ticks);
				int experiencePerTick = experienceToReceiveThisLevel / ticks;
				for (int i = 0; i < experiencePerTick; i++)
				{
					levelStartExperience += ticks;
					SetExperienceBar(levelStartExperience, levelTargetExperience);
					_experienceFromKills.SetText($"{remainingExperience - ticks * i}");
					yield return new WaitForSeconds(_timePerTick);
				}
				levelStartExperience = 0;
				_currentLevel.SetText($"{levelIterator}");
				levelIterator++;
				remainingExperience -= experienceToReceiveThisLevel;
				StartCoroutine(PlaylevelUpAnimation());
			}
			else if (levelStartExperience == 0 && remainingExperience >= levelTargetExperience)
			{
				SetExperienceBar(levelStartExperience, levelTargetExperience);
				int experienceToReceiveThisLevel = levelTargetExperience;
				int experiencePerTick = experienceToReceiveThisLevel / maximumTicks;
				experiencePerTick = Mathf.Max(1, experiencePerTick);
				int ticks = experienceToReceiveThisLevel / experiencePerTick;
				for (int i = 0; i < ticks; i++)
				{
					levelStartExperience += experiencePerTick;
					SetExperienceBar(levelStartExperience, levelTargetExperience);
					_experienceFromKills.SetText($"{remainingExperience - experiencePerTick * i}");
					yield return new WaitForSeconds(_timePerTick);
				}
				remainingExperience -= experienceToReceiveThisLevel;
				levelStartExperience = 0;
				levelIterator++;
				_currentLevel.SetText($"{levelIterator}");
				StartCoroutine(PlaylevelUpAnimation());
			}
		}
		_experienceFromKills.SetText("0");
		_currentLevel.SetText($"{SingletonController<CharactersController>.Instance.CurrentLevel}");
		SetExperienceBar(SingletonController<CharactersController>.Instance.CurrentExperience, new ExperienceCalculator().GetExperienceNeededForLevel(SingletonController<CharactersController>.Instance.CurrentLevel));
		yield return new WaitForSeconds(3f);
		Time.timeScale = 0f;
	}

	private IEnumerator PlaylevelUpAnimation()
	{
		LeanTween.cancel(_currentLevel.gameObject);
		LeanTween.scale(_currentLevel.gameObject, new Vector3(_scaleEffect, _scaleEffect, _scaleEffect), _scaleUpDuration).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.scale(_currentLevel.gameObject, new Vector3(1f, 1f, 1f), _scaleDownDuration).setDelay(_scaleUpDuration).setIgnoreTimeScale(useUnScaledTime: true);
		_levelUpSystem.gameObject.SetActive(value: true);
		_levelUpSystem.Play();
		_playerImages[SingletonController<CharactersController>.Instance.ActiveCharacter].material.SetFloat(_shaderNameMetal, 0f);
		LeanTween.cancel(_playerImages[SingletonController<CharactersController>.Instance.ActiveCharacter].gameObject);
		LeanTween.value(_playerImages[SingletonController<CharactersController>.Instance.ActiveCharacter].gameObject, UpdateMetalFade, 0f, 1f, _showAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(_playerImages[SingletonController<CharactersController>.Instance.ActiveCharacter].gameObject, UpdateMetalFade, 1f, 0f, _hideAnimationTime).setDelay(_delay + _showAnimationTime).setIgnoreTimeScale(useUnScaledTime: true);
		SingletonController<AudioController>.Instance.PlaySFXClip(_levelAudioClip, 1f);
		_playerAnimator.SetTrigger("LevelUp");
		yield return new WaitForSeconds(1.8f);
	}

	private void UpdateMetalFade(float val)
	{
		_playerImages[SingletonController<CharactersController>.Instance.ActiveCharacter].material.SetFloat(_shaderNameMetal, val);
	}

	private void SetExperienceBar(float current, float target)
	{
		float fillAmount = current / target;
		_experienceBar.fillAmount = fillAmount;
		if (GameDatabase.IsDemo && target >= 1E+09f)
		{
			_experienceBarText.SetText("Max level for demo reached");
		}
		else
		{
			_experienceBarText.SetText($"{current}/{target}");
		}
	}

	private void ShowRewardLock(bool succes)
	{
		_rewardLockedOverlay.SetActive(!succes);
		_chainLeft.gameObject.SetActive(!succes);
		_chainRight.gameObject.SetActive(!succes);
		if (!succes)
		{
			LeanTween.value(_chainLeft.gameObject, delegate(float val)
			{
				_chainLeft.fillAmount = val;
			}, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
			LeanTween.value(_chainRight.gameObject, delegate(float val)
			{
				_chainRight.fillAmount = val;
			}, 0f, 1f, 1f).setIgnoreTimeScale(useUnScaledTime: true);
			SingletonController<AudioController>.Instance.PlaySFXClip(_chainsAudioClip, 1f);
		}
	}

	private void SetSuccesOrVictory(bool succes)
	{
		_succesfulTitleObject.SetActive(succes);
		_failedTitleObject.SetActive(!succes);
		_succesfulBackdropObject.SetActive(succes);
		_failedBackdropObject.SetActive(!succes);
	}

	private void SetBackgrounds(AdventureSO adventure)
	{
		_backgroundImage.sprite = adventure.BackgroundImage;
		_adventureTitle.SetText(adventure.AdventureName);
	}

	private void ShowLevelVisuals(AdventureSO adventure, bool succes)
	{
		LevelSO currentLevel = UnityEngine.Object.FindObjectOfType<TimeBasedLevelController>().CurrentLevel;
		foreach (Transform item in _levelVisualContainerTransform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		int num = 0;
		foreach (LevelSO item2 in adventure.Levels.Where((LevelSO x) => x.ShouldShowInList))
		{
			bool flag = false;
			flag = currentLevel.LevelId > item2.LevelId || succes;
			if (num > 0)
			{
				UnityEngine.Object.Instantiate(_adventureCompletedLevelLinePrefab, _levelVisualContainerTransform).Init(flag);
			}
			UnityEngine.Object.Instantiate(_adventureCompletedLevelVisualPrefab, _levelVisualContainerTransform).Init(item2, flag);
			num++;
		}
	}

	private void ShowHellfire(AdventureSO adventure)
	{
		if (SingletonController<DifficultyController>.Instance.ActiveDifficulty > 0)
		{
			_hellfireText.SetText(SingletonController<DifficultyController>.Instance.ActiveDifficulty.ToString());
			_hellfireLineImage.gameObject.SetActive(value: true);
			_hellfireLineImage.material = ((SingletonController<DifficultyController>.Instance.ActiveDifficulty == 9) ? _hellfireLineMaxLevelMaterial : _hellfireLineActiveMaterial);
		}
		else
		{
			_hellfireText.SetText("-");
			_hellfireLineImage.gameObject.SetActive(value: false);
		}
		foreach (Transform item in _adventureEffectContainerTransform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (AdventureEffectSO adventureEffect in adventure.AdventureEffects)
		{
			if (adventureEffect.ActiveFromHellfireLevel <= SingletonController<DifficultyController>.Instance.ActiveDifficulty)
			{
				UnityEngine.Object.Instantiate(_adventureEffectDetailPrefab, _adventureEffectContainerTransform).Init(adventureEffect.Icon, adventureEffect.Name, adventureEffect.Description);
			}
		}
	}

	private void ShowWeaponStats()
	{
		foreach (Transform item in _weaponsContainer)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		new List<WeaponSOAndStats>();
		foreach (WeaponSOAndStats item2 in from x in SingletonController<WeaponDamageAndDPSController>.Instance.GetAll()
			orderby x.Damage descending
			select x)
		{
			UnityEngine.Object.Instantiate(_adventureCompletedWeaponUIPrefab, _weaponsContainer).Init(item2.WeaponSO, item2.Damage, item2.DPS);
		}
	}

	private void ShowStats()
	{
		_totalGoldEarned.SetText(SingletonController<StatisticsController>.Instance.GetAdventureMetricValue(Enums.SingleValueStatisticMetrics.TotalCoinsLooted).ToString());
		_titanicSoulsGained.SetText(SingletonController<StatisticsController>.Instance.GetAdventureMetricValue(Enums.SingleValueStatisticMetrics.TotalTitanicSoulsLooted).ToString());
		_enemiesDefeated.SetText(SingletonController<StatisticsController>.Instance.GetEnemiesKilledInAdventure().ToString());
		_experienceGained.SetText(SingletonController<StatisticsController>.Instance.GetAdventureMetricValue(Enums.SingleValueStatisticMetrics.TotalExperienceGained).ToString());
	}

	private void ShowRelics()
	{
		foreach (Transform item in _relicsContainer)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		foreach (Relic activeRelic in SingletonController<RelicsController>.Instance.ActiveRelics)
		{
			UnityEngine.Object.Instantiate(_collectionRelicUIPrefab, _relicsContainer).Init(activeRelic.RelicSO, unlocked: true, interactable: false);
		}
	}

	private void ShowRewards(AdventureSO adventure)
	{
		foreach (Transform item in _rewardContainerTransform)
		{
			UnityEngine.Object.Destroy(item.gameObject);
		}
		SetFirstTimeCompletionRewards(adventure);
		SetCompletionReward(adventure);
	}

	private void SetFirstTimeCompletionRewards(AdventureSO adventure)
	{
		int currentHellfireLevel = SingletonController<DifficultyController>.Instance.ActiveDifficulty;
		if (SingletonController<StatisticsController>.Instance.HighestCompletedHellfireLevelForAdventure(adventure.AdventureId) >= currentHellfireLevel)
		{
			return;
		}
		foreach (RewardSO item in adventure.CompletionRewards.Where((RewardSO x) => !x.Repeatable && x.HellfireLevel == currentHellfireLevel))
		{
			switch (item.CompletionRewardType)
			{
			case Enums.RewardType.Bag:
				UnityEngine.Object.Instantiate(_adventureRewardBagPrefab, _rewardContainerTransform).Init((BagSO)item.CompletionReward, unlocked: true, interactable: false);
				break;
			case Enums.RewardType.Weapon:
				UnityEngine.Object.Instantiate(_adventureRewardWeaponPrefab, _rewardContainerTransform).Init((WeaponSO)item.CompletionReward, unlocked: true, interactable: false);
				break;
			case Enums.RewardType.Item:
				UnityEngine.Object.Instantiate(_adventureRewardItemPrefab, _rewardContainerTransform).Init((ItemSO)item.CompletionReward, unlocked: true, interactable: false);
				break;
			case Enums.RewardType.Relic:
				UnityEngine.Object.Instantiate(_adventureRewardRelicPrefab, _rewardContainerTransform).Init((RelicSO)item.CompletionReward, unlocked: true, interactable: false);
				break;
			case Enums.RewardType.TitanicSouls:
				UnityEngine.Object.Instantiate(_adventureRewardCurrencyPrefab, _rewardContainerTransform).Init(Enums.CurrencyType.TitanSouls, item.Amount, unlocked: true, interactable: false);
				break;
			}
		}
	}

	private void SetCompletionReward(AdventureSO adventure)
	{
		int activeDifficulty = SingletonController<DifficultyController>.Instance.ActiveDifficulty;
		float titanicSoulMultiplier = adventure.TitanicSoulMultiplier;
		if (activeDifficulty != 9)
		{
			RewardSO rewardSO = adventure.CompletionRewards.FirstOrDefault((RewardSO x) => x.Repeatable && x.CompletionRewardType == Enums.RewardType.TitanicSouls);
			if (rewardSO != null)
			{
				AdventureRewardCurrency adventureRewardCurrency = UnityEngine.Object.Instantiate(_adventureRewardCurrencyPrefab, _rewardContainerTransform);
				float value = (float)rewardSO.Amount + (float)rewardSO.Amount * ((float)activeDifficulty * titanicSoulMultiplier);
				adventureRewardCurrency.Init(Enums.CurrencyType.TitanSouls, value, unlocked: true, interactable: false);
			}
		}
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
		RegisterInputController();
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		StopAllCoroutines();
		base.CloseUI(openDirection);
		_succesfulBackdropObject.SetActive(value: false);
		_failedBackdropObject.SetActive(value: false);
		UnregisterInputController();
	}

	public void ReturnToTownButtonPressed()
	{
		this.OnAdventureCompleteButtonClicked?.Invoke(this, new EventArgs());
	}

	private void RegisterInputController()
	{
		SingletonController<InputController>.Instance.OnAcceptHandler += Instance_OnAcceptHandler;
	}

	private void UnregisterInputController()
	{
		SingletonController<InputController>.Instance.OnAcceptHandler -= Instance_OnAcceptHandler;
	}

	private void Instance_OnAcceptHandler(object sender, EventArgs e)
	{
		ReturnToTownButtonPressed();
	}

	private void OnDestroy()
	{
		UnregisterInputController();
	}
}
