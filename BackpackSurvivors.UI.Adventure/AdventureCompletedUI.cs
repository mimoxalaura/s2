using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Adventure;
using BackpackSurvivors.Game.Difficulty;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Stastistics;
using BackpackSurvivors.ScriptableObjects.Adventures;
using BackpackSurvivors.System;
using BackpackSurvivors.UI.Collection.ListItems.Relic;
using BackpackSurvivors.UI.Minimap;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Adventure;

public class AdventureCompletedUI : ModalUI
{
	private AdventureSO _adventure;

	private int _hellfireLevel;

	private bool _succesfull;

	[SerializeField]
	private Image _succesBackground;

	[SerializeField]
	private Image _currentHellfireLevel;

	[SerializeField]
	private TextMeshProUGUI _currentHellfireText;

	[SerializeField]
	private AdventureRewardUI _adventureRewardUI;

	[SerializeField]
	private Transform _adventureRewardContainer;

	[SerializeField]
	private TextMeshProUGUI _adventureTitle;

	[SerializeField]
	private TextMeshProUGUI _adventureDescription;

	[Header("Statistics")]
	[SerializeField]
	private TextMeshProUGUI _enemiesDefeated;

	[SerializeField]
	private TextMeshProUGUI _experienceGained;

	[SerializeField]
	private TextMeshProUGUI _totalGoldEarned;

	[SerializeField]
	private TextMeshProUGUI _titanicSoulsGained;

	[SerializeField]
	private TextMeshProUGUI _totalAdventureClears;

	[SerializeField]
	private TextMeshProUGUI _runTime;

	[SerializeField]
	private TextMeshProUGUI _bestRunTime;

	[SerializeField]
	private Transform _relicContainer;

	[SerializeField]
	private CollectionRelicUI _relicPrefab;

	[Header("Weapons")]
	[SerializeField]
	private Transform _weaponsContainer;

	[SerializeField]
	private AdventureCompletedWeaponUI _AdventureCompletedWeaponUIPrefab;

	[Header("VFX")]
	[SerializeField]
	private AudioClip _chainsAudioClip;

	[SerializeField]
	private Animator _animator;

	[SerializeField]
	private Image _bloodBackdropImage;

	[SerializeField]
	private Image _bloodBackdropAnimatedImage;

	[SerializeField]
	private GameObject _defeated;

	[SerializeField]
	private GameObject _completed;

	[SerializeField]
	private GameObject _chain1;

	[SerializeField]
	private GameObject _chain2;

	[SerializeField]
	private CompletionAdventurePlayerCamera _completionAdventurePlayerCamera;

	public void Init(AdventureSO adventure, int hellfireLevel, bool succesfull)
	{
		_adventure = adventure;
		_hellfireLevel = hellfireLevel;
		_succesfull = succesfull;
		_defeated.SetActive(!succesfull);
		_completed.SetActive(succesfull);
		_chain1.SetActive(!succesfull);
		_chain2.SetActive(!succesfull);
		UpdateRelics();
		UpdateWeapons();
		UpdateStatistics();
		UpdateDetail(_adventure);
		SetHellfireLevel(_hellfireLevel);
	}

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
	}

	private void UpdateWeapons()
	{
		for (int num = _weaponsContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(_weaponsContainer.GetChild(num).gameObject);
		}
		new List<WeaponSOAndStats>();
		foreach (WeaponSOAndStats item in from x in SingletonController<WeaponDamageAndDPSController>.Instance.GetAll()
			orderby x.Damage descending
			select x)
		{
			Object.Instantiate(_AdventureCompletedWeaponUIPrefab, _weaponsContainer).Init(item.WeaponSO, item.Damage, item.DPS);
		}
	}

	private void UpdateRelics()
	{
		for (int num = _relicContainer.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(_relicContainer.GetChild(num).gameObject);
		}
		foreach (Relic activeRelic in SingletonController<RelicsController>.Instance.ActiveRelics)
		{
			Object.Instantiate(_relicPrefab, _relicContainer).Init(activeRelic.RelicSO, unlocked: true, interactable: false);
		}
	}

	public override void AfterOpenUI()
	{
		base.AfterOpenUI();
		Init(_adventure, _hellfireLevel, _succesfull);
		Object.FindObjectOfType<Player>().PlayerAnimatorController.AllowAnimationActiveDuringZeroTimescale(allow: true);
		_animator.SetBool("Locked", !_succesfull);
		if (!_succesfull)
		{
			FadeAlpha(_bloodBackdropImage, 1f, 1f);
			SingletonController<AudioController>.Instance.PlaySFXClip(_chainsAudioClip, 1f);
		}
		RefreshCameraRenderingMask();
	}

	public void FadeAlpha(Image toFade, float targetAlpha, float duration)
	{
		_bloodBackdropImage = toFade;
		Color color = toFade.color;
		Color to = color;
		to.a = targetAlpha;
		LeanTween.value(toFade.gameObject, updateValueBloodBackdropImageCallback, color, to, duration).setIgnoreTimeScale(useUnScaledTime: true);
		LeanTween.value(toFade.gameObject, updateValueBloodBackdropAnimatedImageCallback, color, to, duration).setIgnoreTimeScale(useUnScaledTime: true);
	}

	private void updateValueBloodBackdropImageCallback(Color val)
	{
		_bloodBackdropImage.color = val;
	}

	private void updateValueBloodBackdropAnimatedImageCallback(Color val)
	{
		_bloodBackdropAnimatedImage.color = val;
	}

	private void RefreshCameraRenderingMask()
	{
	}

	private void UpdateStatistics()
	{
		_totalGoldEarned.SetText(SingletonController<StatisticsController>.Instance.GetAdventureMetricValue(Enums.SingleValueStatisticMetrics.TotalCoinsLooted).ToString());
		_titanicSoulsGained.SetText(SingletonController<StatisticsController>.Instance.GetAdventureMetricValue(Enums.SingleValueStatisticMetrics.TotalTitanicSoulsLooted).ToString());
		_enemiesDefeated.SetText(SingletonController<StatisticsController>.Instance.GetEnemiesKilledInAdventure().ToString());
		_experienceGained.SetText(SingletonController<StatisticsController>.Instance.GetAdventureMetricValue(Enums.SingleValueStatisticMetrics.TotalExperienceGained).ToString());
	}

	public override void AfterCloseUI()
	{
		base.AfterCloseUI();
		Object.FindObjectOfType<Player>().PlayerAnimatorController.AllowAnimationActiveDuringZeroTimescale(allow: false);
		FadeAlpha(_bloodBackdropImage, 0f, 2f);
	}

	internal void SetHellfireLevel(float hellfireLevel)
	{
		_currentHellfireLevel.fillAmount = hellfireLevel / 12f;
		_currentHellfireText.SetText($"{hellfireLevel}/{12}");
	}

	public void UpdateDetail(AdventureSO adventureSO)
	{
		_adventureTitle.SetText(adventureSO.AdventureName);
		_adventureDescription.SetText(adventureSO.Description);
		foreach (Transform item in _adventureRewardContainer)
		{
			Object.Destroy(item.gameObject);
		}
		foreach (RewardSO item2 in adventureSO.CompletionRewards.Where((RewardSO x) => x.HellfireLevel == SingletonController<DifficultyController>.Instance.ActiveDifficulty))
		{
			Object.Instantiate(_adventureRewardUI, _adventureRewardContainer).Init(item2);
		}
	}
}
