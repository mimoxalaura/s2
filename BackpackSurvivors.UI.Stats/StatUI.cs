using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Buffs.Base;
using BackpackSurvivors.Game.Characters;
using BackpackSurvivors.Game.Core;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Player;
using BackpackSurvivors.Game.Relic;
using BackpackSurvivors.Game.Talents;
using BackpackSurvivors.ScriptableObjects.Buffs;
using BackpackSurvivors.ScriptableObjects.Relics;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Steam;
using BackpackSurvivors.UI.Minimap;
using BackpackSurvivors.UI.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Stats;

public class StatUI : ModalUI
{
	[SerializeField]
	private StatItemUI _statItemUIPrefab;

	[SerializeField]
	private ComposedStatItemUI _composedStatItemUI;

	[SerializeField]
	private RelicItemUI _relicPrefab;

	[SerializeField]
	private Transform _offensiveStatContainer;

	[SerializeField]
	private Transform _elementalStatContainer;

	[SerializeField]
	private Transform _physicalStatContainer;

	[SerializeField]
	private Transform _defensiveStatContainer;

	[SerializeField]
	private Transform _utilityStatContainer;

	[SerializeField]
	private Transform _relicStatContainer;

	[SerializeField]
	private Dictionary<Enums.ItemStatType, StatItemUI> _statItems;

	[SerializeField]
	private Dictionary<Enums.DamageType, StatItemUI> _elementalDamageItems;

	[SerializeField]
	private Dictionary<Enums.DamageType, StatItemUI> _physicalDamageItems;

	[SerializeField]
	private CompletionAdventurePlayerCamera _completionAdventurePlayerCamera;

	[SerializeField]
	private Image _toShowOnSoloOpen;

	[SerializeField]
	public BuffVisualUIContainer BuffVisualUIContainer;

	[SerializeField]
	private TextMeshProUGUI _titleText;

	[SerializeField]
	private TextMeshProUGUI _levelText;

	[SerializeField]
	private TextMeshProUGUI _experienceText;

	[SerializeField]
	private Image _experienceImage;

	[SerializeField]
	private Image _characterIcon;

	[SerializeField]
	private TextMeshProUGUI _unusedTalentPoints;

	[SerializeField]
	private GameObject _unusedTalentPointsDescription;

	[SerializeField]
	private GameObject _buffContainer;

	public bool OpenWithSoloBackdrop;

	public bool OpenWithBuffsAndDebuffs;

	public bool Initialized;

	public override void OpenUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.OpenUI(openDirection);
		Object.FindObjectOfType<Player>().PlayerAnimatorController.AllowAnimationActiveDuringZeroTimescale(allow: true);
		_completionAdventurePlayerCamera.Init(Object.FindObjectOfType<Player>());
		_toShowOnSoloOpen.gameObject.SetActive(OpenWithSoloBackdrop);
		_buffContainer.SetActive(OpenWithBuffsAndDebuffs);
		RefreshCameraRenderingMask();
		_titleText.SetText($"<b>{GetCharacterName()}</b> {GetPlayerName()}");
		float currentExperience = SingletonController<CharactersController>.Instance.CurrentExperience;
		float experienceToNext = SingletonController<CharactersController>.Instance.ExperienceToNext;
		float fillAmount = currentExperience / experienceToNext;
		_experienceImage.fillAmount = fillAmount;
		_characterIcon.sprite = SingletonController<CharactersController>.Instance.ActiveCharacter.Face;
		_levelText.SetText($"lv. {GetCurrentLevel()}");
		if (GameDatabase.IsDemo && GetCurrentLevel() > 20)
		{
			_experienceText.SetText("(demo) max level 20");
		}
		else
		{
			_experienceText.SetText($"{currentExperience}/{experienceToNext}");
		}
		int currentRemainingTalentPoints = GetCurrentRemainingTalentPoints();
		_unusedTalentPoints.SetText($"{currentRemainingTalentPoints}");
		_unusedTalentPoints.gameObject.SetActive(currentRemainingTalentPoints > 0);
		_unusedTalentPointsDescription.gameObject.SetActive(currentRemainingTalentPoints > 0);
	}

	private int GetCurrentRemainingTalentPoints()
	{
		return SingletonController<NewTalentController>.Instance.TalentPoints;
	}

	private int GetCurrentLevel()
	{
		return SingletonController<CharactersController>.Instance.CurrentLevel;
	}

	private object GetPlayerName()
	{
		return SingletonController<SteamController>.Instance.GetSteamName().ToUpper();
	}

	private string GetCharacterName()
	{
		return SingletonController<CharactersController>.Instance.ActiveCharacter.Name;
	}

	public override void CloseUI(Enums.Modal.OpenDirection openDirection = Enums.Modal.OpenDirection.UseGiven)
	{
		base.CloseUI(openDirection);
		Object.FindObjectOfType<Player>().PlayerAnimatorController.AllowAnimationActiveDuringZeroTimescale(allow: false);
	}

	private void RefreshCameraRenderingMask()
	{
		_completionAdventurePlayerCamera.GetComponent<Camera>().enabled = false;
		_completionAdventurePlayerCamera.GetComponent<Camera>().enabled = true;
	}

	public void InitStats(Dictionary<Enums.ItemStatType, float> playerBaseStats, Dictionary<Enums.ItemStatType, List<ItemStatModifier>> complexPlayerBaseStats, Dictionary<Enums.DamageType, float> playerBaseDamage, Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> complexPlayerBaseDamage)
	{
		_statItems = new Dictionary<Enums.ItemStatType, StatItemUI>();
		_elementalDamageItems = new Dictionary<Enums.DamageType, StatItemUI>();
		_physicalDamageItems = new Dictionary<Enums.DamageType, StatItemUI>();
		foreach (KeyValuePair<Enums.ItemStatType, float> item in playerBaseStats.OrderBy((KeyValuePair<Enums.ItemStatType, float> pbs) => pbs.Key.ToString()))
		{
			StatItemUI statItemUI = null;
			switch (EnumHelper.GetItemStatType(item.Key))
			{
			case Enums.ItemStatTypeGroup.Offensive:
				statItemUI = ((item.Key != Enums.ItemStatType.WeaponCapacity) ? Object.Instantiate(_statItemUIPrefab, _offensiveStatContainer) : Object.Instantiate(_composedStatItemUI, _utilityStatContainer));
				break;
			case Enums.ItemStatTypeGroup.Defensive:
				statItemUI = Object.Instantiate(_statItemUIPrefab, _defensiveStatContainer);
				break;
			case Enums.ItemStatTypeGroup.Utility:
				statItemUI = Object.Instantiate(_statItemUIPrefab, _utilityStatContainer);
				break;
			case Enums.ItemStatTypeGroup.Other:
				statItemUI = Object.Instantiate(_statItemUIPrefab, _relicStatContainer);
				break;
			case Enums.ItemStatTypeGroup.Unused:
			case Enums.ItemStatTypeGroup.Hidden:
				continue;
			}
			statItemUI.Init(item.Key, complexPlayerBaseStats[item.Key]);
			_statItems.Add(item.Key, statItemUI);
		}
		AddElementalDamageType(Enums.DamageType.Cold, complexPlayerBaseDamage);
		AddElementalDamageType(Enums.DamageType.Energy, complexPlayerBaseDamage);
		AddElementalDamageType(Enums.DamageType.Fire, complexPlayerBaseDamage);
		AddElementalDamageType(Enums.DamageType.Holy, complexPlayerBaseDamage);
		AddElementalDamageType(Enums.DamageType.Lightning, complexPlayerBaseDamage);
		AddElementalDamageType(Enums.DamageType.Poison, complexPlayerBaseDamage);
		AddElementalDamageType(Enums.DamageType.Void, complexPlayerBaseDamage);
		AddPhysicalDamageType(Enums.DamageType.Blunt, complexPlayerBaseDamage);
		AddPhysicalDamageType(Enums.DamageType.Piercing, complexPlayerBaseDamage);
		AddPhysicalDamageType(Enums.DamageType.Slashing, complexPlayerBaseDamage);
		InitRelics();
		Initialized = true;
	}

	private void AddElementalDamageType(Enums.DamageType damageType, Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> complexPlayerBaseDamage)
	{
		StatItemUI statItemUI = Object.Instantiate(_statItemUIPrefab, _elementalStatContainer);
		statItemUI.Init(damageType, complexPlayerBaseDamage[damageType]);
		_elementalDamageItems.Add(damageType, statItemUI);
	}

	private void AddPhysicalDamageType(Enums.DamageType damageType, Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> complexPlayerBaseDamage)
	{
		StatItemUI statItemUI = Object.Instantiate(_statItemUIPrefab, _physicalStatContainer);
		statItemUI.Init(damageType, complexPlayerBaseDamage[damageType]);
		_physicalDamageItems.Add(damageType, statItemUI);
	}

	public void UpdateStats(Dictionary<Enums.ItemStatType, float> playerStats, Dictionary<Enums.ItemStatType, List<ItemStatModifier>> complexPlayerStats, Dictionary<Enums.ItemStatType, float> basePlayerStats, Dictionary<Enums.DamageType, float> playerDamage, Dictionary<Enums.DamageType, List<DamageTypeValueModifier>> complexPlayerDamage, Dictionary<Enums.DamageType, float> playerBaseDamage)
	{
		if (!Initialized)
		{
			InitStats(playerStats, complexPlayerStats, playerDamage, complexPlayerDamage);
		}
		RefreshCameraRenderingMask();
		foreach (KeyValuePair<Enums.ItemStatType, float> playerStat in playerStats)
		{
			if (EnumHelper.GetItemStatType(playerStat.Key) == Enums.ItemStatTypeGroup.Hidden)
			{
				continue;
			}
			string cleanValue = StringHelper.GetCleanValue(playerStat.Value, playerStat.Key);
			if (_statItems.ContainsKey(playerStat.Key))
			{
				float num = 0f;
				if (basePlayerStats.ContainsKey(playerStat.Key))
				{
					num = basePlayerStats[playerStat.Key];
				}
				float statChange = playerStats[playerStat.Key] - num;
				_statItems[playerStat.Key].UpdateStat(cleanValue, statChange, complexPlayerStats[playerStat.Key]);
				_statItems[playerStat.Key].UpdateTooltip(complexPlayerStats[playerStat.Key]);
			}
		}
		foreach (KeyValuePair<Enums.DamageType, float> item in playerDamage)
		{
			string cleanValue2 = StringHelper.GetCleanValue(item.Value, item.Key);
			if (_elementalDamageItems.ContainsKey(item.Key))
			{
				float num2 = 0f;
				if (playerBaseDamage.ContainsKey(item.Key))
				{
					num2 = playerBaseDamage[item.Key];
				}
				float statChange2 = playerDamage[item.Key] - num2;
				_elementalDamageItems[item.Key].UpdateStat(cleanValue2, statChange2, complexPlayerDamage[item.Key]);
				_elementalDamageItems[item.Key].UpdateTooltip(complexPlayerDamage[item.Key]);
			}
			if (_physicalDamageItems.ContainsKey(item.Key))
			{
				float num3 = 0f;
				if (playerBaseDamage.ContainsKey(item.Key))
				{
					num3 = playerBaseDamage[item.Key];
				}
				float statChange3 = playerDamage[item.Key] - num3;
				_physicalDamageItems[item.Key].UpdateStat(cleanValue2, statChange3, complexPlayerDamage[item.Key]);
				_physicalDamageItems[item.Key].UpdateTooltip(complexPlayerDamage[item.Key]);
			}
		}
	}

	public void InitRelics()
	{
		foreach (Relic activeRelic in SingletonController<RelicsController>.Instance.ActiveRelics)
		{
			RelicAdded(activeRelic.RelicSO);
		}
	}

	public void RelicAdded(RelicSO relicSO)
	{
		Object.Instantiate(_relicPrefab, _relicStatContainer).Init(relicSO);
	}

	public void ClearUIRelics()
	{
		RelicItemUI[] componentsInChildren = _relicStatContainer.GetComponentsInChildren<RelicItemUI>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			Object.Destroy(componentsInChildren[i].gameObject);
		}
	}

	public void AnimateStat(Enums.ItemStatType itemStatType, string value, string colorString = "#ff0000", int numberOfBlinks = 3, float timeBetweenBlinks = 0.1f)
	{
		if (_statItems.ContainsKey(itemStatType))
		{
			_statItems[itemStatType].AnimateStat(value, colorString, numberOfBlinks, timeBetweenBlinks);
		}
	}

	internal void UpdateBuffs(List<BuffSO> buffSOs)
	{
		Debug.LogWarning("StatUI.UpdateBuffs is not implemented");
	}

	internal void ClearBuffs()
	{
		for (int num = BuffVisualUIContainer.transform.childCount - 1; num >= 0; num--)
		{
			Object.Destroy(BuffVisualUIContainer.transform.GetChild(num).gameObject);
		}
	}
}
