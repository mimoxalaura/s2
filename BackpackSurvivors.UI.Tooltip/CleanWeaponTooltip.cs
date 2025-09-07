using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Assets.UI.Tooltips;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Debuffs.Base;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Saving;
using BackpackSurvivors.ScriptableObjects.Debuffs;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using BackpackSurvivors.System.Settings;
using BackpackSurvivors.UI.Tooltip.Complex;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip;

public class CleanWeaponTooltip : BaseTooltip
{
	[Header("Base")]
	[SerializeField]
	private float _baseSpacingHeight = 16f;

	[SerializeField]
	private bool _showCollectedFlag;

	[SerializeField]
	private bool _resizeTooltip;

	[Header("Tags")]
	[SerializeField]
	private Transform _tagContainer;

	[SerializeField]
	private TooltipTagLine _tooltipTagLinePrefab;

	[SerializeField]
	private TooltipTag _tagPrefab;

	[Header("Title")]
	[SerializeField]
	private TextMeshProUGUI _titleText;

	[SerializeField]
	private Transform _titleContainer;

	[Header("Weapon")]
	[SerializeField]
	private TextMeshProUGUI _cooldownRangeTargetingText;

	[SerializeField]
	private TextMeshProUGUI _damageText;

	[Header("Size")]
	[SerializeField]
	private TooltipSizeContainer _tooltipSizeContainer;

	[SerializeField]
	private Transform _tooltipSizeRootContainer;

	[Header("Global")]
	[SerializeField]
	private TextMeshProUGUI _globalStatsText;

	[Header("Starred")]
	[SerializeField]
	private Transform _starContainer;

	[SerializeField]
	private Image _starContentBackdrop;

	[SerializeField]
	private TextMeshProUGUI _starConditionsText;

	[SerializeField]
	private GameObject _starConditionsMarker;

	[SerializeField]
	private TextMeshProUGUI _starStatsText;

	[SerializeField]
	private TextMeshProUGUI _starTitleText;

	[Header("Price")]
	[SerializeField]
	private TextMeshProUGUI _priceText;

	[Header("Border")]
	[SerializeField]
	private Image[] _imagesToSetRarityMaterialOn;

	[SerializeField]
	private Image[] _imagesToSetFadeOn;

	[SerializeField]
	private Image _mainBorder;

	[Header("Complex")]
	[SerializeField]
	private bool _allowShowComplexTooltips = true;

	[SerializeField]
	private GameObject _altHotkeyContainer;

	[SerializeField]
	private GameObject _altHotkeyKeyboard;

	[SerializeField]
	private TextMeshProUGUI _altHotkeyText;

	[SerializeField]
	private GameObject _altHotkeyController;

	[SerializeField]
	private Transform _tipContainer;

	[SerializeField]
	private ItemTooltipComplexTip _itemTooltipComplexTipPrefab;

	[SerializeField]
	private ItemTooltipComplexFormulaTip _itemTooltipComplexFormulaTipPrefab;

	[SerializeField]
	private GameObject _collectedIcon;

	private WeaponSO _activeWeaponSO;

	private WeaponInstance _activeWeaponInstance;

	private bool _hasComplexTooltipToShow;

	private List<GameObject> _inTooltipComplexElements = new List<GameObject>();

	private List<Enums.WeaponStatType> _blacklist = new List<Enums.WeaponStatType>
	{
		Enums.WeaponStatType.CooldownTime,
		Enums.WeaponStatType.CooldownReductionPercentage,
		Enums.WeaponStatType.WeaponRange,
		Enums.WeaponStatType.FlatDamage
	};

	public void SetWeapon(WeaponInstance weapon, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_activeWeaponInstance = weapon;
		_activeWeaponSO = null;
		SetTitle(weapon.BaseWeaponSO.Name, weapon.BaseWeaponSO.ItemRarity);
		SetSizeImage(weapon.BaseWeaponSO);
		SetRarity(weapon.BaseWeaponSO);
		SetDamage(weapon);
		SetRangeAndCooldown(weapon);
		SetupGlobal(weapon);
		SetupStarred(weapon);
		SetPrice(weapon, owner, overriddenPrice);
		SetupComplexTooltips(weapon.BaseWeaponSO);
		SetupNewCollected(weapon.BaseWeaponSO);
		SetupDebug(weapon.BaseWeaponSO);
		Vector2 vector = SetTooltipSize(weapon.BaseWeaponSO);
		SetTags(EnumHelper.GetCombinedPlaceableTags(weapon), weapon.BaseWeaponSO.ItemRarity, vector.x);
	}

	public void SetWeapon(WeaponSO weapon, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_activeWeaponInstance = null;
		_activeWeaponSO = weapon;
		SetTitle(weapon.Name, weapon.ItemRarity);
		SetSizeImage(weapon);
		SetRarity(weapon);
		SetDamage(weapon);
		SetRangeAndCooldown(weapon);
		SetupGlobal(weapon);
		SetupStarred(weapon);
		SetPrice(weapon, owner, overriddenPrice);
		SetupComplexTooltips(weapon);
		SetupNewCollected(weapon);
		SetupDebug(weapon);
		Vector2 vector = SetTooltipSize(weapon);
		SetTags(EnumHelper.GetCombinedPlaceableTags(weapon), weapon.ItemRarity, vector.x);
	}

	private void SetRangeAndCooldown(WeaponSO weapon)
	{
		_cooldownRangeTargetingText.SetText(TooltipRangeAndCooldownHelper.CreateLine(weapon));
	}

	private void SetRangeAndCooldown(WeaponInstance weapon)
	{
		_cooldownRangeTargetingText.SetText(TooltipRangeAndCooldownHelper.CreateLine(weapon));
	}

	private void SetDamage(WeaponSO weapon)
	{
		_damageText.SetText(TooltipDamageHelper.CreateLine(weapon));
	}

	private void SetDamage(WeaponInstance weapon)
	{
		_damageText.SetText(TooltipDamageHelper.CreateLine(weapon));
	}

	private Vector2 SetTooltipSize(WeaponSO item)
	{
		float num = 0f;
		float num2 = 0f;
		num += ((RectTransform)_titleContainer).sizeDelta.y;
		bool hasGlobalStats = HasGlobalStats(item);
		bool hasStarredStats = HasStarredStats(item);
		bool flag = HasStarredConditions(item);
		float num3 = CalculateContentHeight(hasGlobalStats, hasStarredStats, flag);
		num += num3;
		float num4 = CalculateContentWidth(hasGlobalStats, hasStarredStats);
		num2 += num4;
		if (flag)
		{
			SetWidthForStarConditionalMarker();
		}
		Vector2 vector = new Vector2(num2, num);
		if (!_resizeTooltip)
		{
			return vector;
		}
		((RectTransform)base.transform).LeanSize(vector, 0f).setIgnoreTimeScale(useUnScaledTime: true);
		return vector;
	}

	private void SetWidthForStarConditionalMarker()
	{
		if (!(_starConditionsMarker == null))
		{
			((RectTransform)_starConditionsMarker.transform).LeanSize(new Vector2(_starConditionsText.preferredWidth, 12f), 0f).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	private float CalculateContentWidth(bool hasGlobalStats, bool hasStarredStats)
	{
		float y = GetSizeContainerSizeVector().y;
		float a = y;
		a = Mathf.Max(a, _titleText.preferredWidth);
		if (hasGlobalStats)
		{
			float preferredWidth = _globalStatsText.preferredWidth;
			a = Mathf.Max(a, preferredWidth + y);
		}
		if (hasStarredStats)
		{
			float preferredWidth2 = _starConditionsText.preferredWidth;
			preferredWidth2 += _baseSpacingHeight;
			a = Mathf.Max(a, preferredWidth2 + y);
			float preferredWidth3 = _starStatsText.preferredWidth;
			preferredWidth3 += _baseSpacingHeight;
			float x = ((RectTransform)_starContainer).sizeDelta.x;
			preferredWidth3 = MathF.Max(preferredWidth3, x);
			a = Mathf.Max(a, preferredWidth3 + y);
		}
		float num = _damageText.preferredWidth + _cooldownRangeTargetingText.preferredWidth;
		a = Mathf.Max(a, num + y);
		a += _baseSpacingHeight;
		a += _baseSpacingHeight;
		a += 10f;
		a += _baseSpacingHeight;
		return a + 48f;
	}

	private Vector2 GetSizeContainerSizeVector()
	{
		return ((RectTransform)_tooltipSizeRootContainer.transform).sizeDelta;
	}

	private float CalculateContentHeight(bool hasGlobalStats, bool hasStarredStats, bool hasStarredConditions)
	{
		float y = GetSizeContainerSizeVector().y;
		float num = 0f;
		num += _baseSpacingHeight * 5f;
		if (hasGlobalStats)
		{
			_globalStatsText.ForceMeshUpdate();
			num += _globalStatsText.preferredHeight;
		}
		if (hasStarredStats)
		{
			num += _baseSpacingHeight;
			if (hasStarredConditions)
			{
				num += _starConditionsText.preferredHeight;
				if (_starConditionsMarker != null)
				{
					num += ((RectTransform)_starConditionsMarker.transform).sizeDelta.y;
				}
				num += _baseSpacingHeight;
			}
			num += _starStatsText.preferredHeight;
			num += _starTitleText.preferredHeight;
			num += _baseSpacingHeight;
		}
		float num2 = Mathf.Max(_damageText.preferredHeight, _cooldownRangeTargetingText.preferredHeight);
		num += num2;
		if (_hasComplexTooltipToShow && SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.VisibleOnAlt)
		{
			num += ((RectTransform)_altHotkeyContainer.transform).sizeDelta.y;
		}
		return Mathf.Max(y + 16f, num);
	}

	private void SetupNewCollected(WeaponSO weapon)
	{
		if (_showCollectedFlag)
		{
			_collectedIcon.SetActive(!SingletonController<CollectionController>.Instance.IsWeaponUnlocked(weapon.Id));
		}
		else
		{
			_collectedIcon.SetActive(value: false);
		}
	}

	private void SetupStarred(WeaponInstance weapon)
	{
		bool flag = HasStarredStats(weapon.BaseWeaponSO);
		_starContainer.gameObject.SetActive(flag);
		string text = string.Empty;
		if (flag)
		{
			string text2 = (weapon.BaseWeaponSO.StarringEffectIsPositive ? StringHelper.PositiveStarSprite : StringHelper.NegativeStarSprite);
			_starTitleText.SetText(text2 + " STARRED ITEM");
			bool flag2 = weapon.CalculatedStats != null;
			_starContentBackdrop.enabled = flag2;
			bool conditionsSatisfied = true;
			bool flag3 = HasStarredConditions(weapon.BaseWeaponSO);
			if (_starConditionsMarker != null)
			{
				_starConditionsMarker.SetActive(flag3);
			}
			if (flag3)
			{
				_starConditionsText.gameObject.SetActive(value: true);
				string empty = string.Empty;
				empty += GetConditions(weapon.BaseStarConditions, weapon, isStarCondition: true);
				_starConditionsText.SetText(empty);
				conditionsSatisfied = GetConditionsAreSatisfied(weapon.BaseWeaponSO.StarStats.Conditions);
			}
			else
			{
				_starConditionsText.gameObject.SetActive(value: false);
				_starConditionsText.SetText(string.Empty);
			}
			text += GetStarredStats(weapon);
			text += GetStarredStatsFromFormula(weapon);
			text += GetEffects(weapon.BaseWeaponSO.StarStats.WeaponAttackEffects.ToList(), weapon.ItemRarity, conditionsSatisfied);
			text += GetDebuffs(weapon.BaseWeaponSO.StarStats.DebuffSOs.ToList(), weapon.ItemRarity, conditionsSatisfied);
			text += GetOverrides(weapon.BaseWeaponSO.StarStats.WeaponDamageCalculationOverrides, conditionsSatisfied, weapon.ItemRarity);
			text += GetOverrides(weapon.BaseWeaponSO.StarStats.WeaponDamageTypeValueOverrides, conditionsSatisfied, weapon.ItemRarity);
		}
		_starStatsText.SetText(text);
	}

	private void SetupStarred(WeaponSO weapon)
	{
		bool flag = HasStarredStats(weapon);
		_starContainer.gameObject.SetActive(flag);
		string text = string.Empty;
		if (flag)
		{
			string text2 = (weapon.StarringEffectIsPositive ? StringHelper.PositiveStarSprite : StringHelper.NegativeStarSprite);
			_starTitleText.SetText(text2 + " STARRED ITEM");
			bool flag2 = weapon.Stats != null;
			_starContentBackdrop.enabled = flag2;
			bool conditionsSatisfied = true;
			bool flag3 = HasStarredConditions(weapon);
			if (_starConditionsMarker != null)
			{
				_starConditionsMarker.SetActive(flag3);
			}
			if (flag3)
			{
				string empty = string.Empty;
				empty += GetConditions(weapon.StarStats.Conditions, null, isStarCondition: true);
				_starConditionsText.SetText(empty);
				conditionsSatisfied = GetConditionsAreSatisfied(weapon.StarStats.Conditions);
			}
			else
			{
				_starConditionsText.SetText(string.Empty);
			}
			text += GetStarredStats(weapon);
			text += GetStarredStatsFromFormula(weapon);
			text += GetEffects(weapon.StarStats.WeaponAttackEffects.ToList(), weapon.ItemRarity, conditionsSatisfied);
			text += GetDebuffs(weapon.StarStats.DebuffSOs.ToList(), weapon.ItemRarity, conditionsSatisfied);
			text += GetOverrides(weapon.StarStats.WeaponDamageCalculationOverrides, conditionsSatisfied, weapon.ItemRarity);
			text += GetOverrides(weapon.StarStats.WeaponDamageTypeValueOverrides, conditionsSatisfied, weapon.ItemRarity);
		}
		_starStatsText.SetText(text);
	}

	private string GetConditions(ConditionSO[] conditionalStatConditions, WeaponInstance weaponInstancec, bool isStarCondition)
	{
		string text = string.Empty;
		foreach (ConditionSO conditionSO in conditionalStatConditions)
		{
			List<WeaponInstance> starredWeapons = new List<WeaponInstance>();
			if (weaponInstancec != null)
			{
				starredWeapons = SingletonController<BackpackController>.Instance.BackpackStorage.GetWeaponsStarredByWeapon(weaponInstancec);
			}
			text = text + TooltipConditionHelper.CreateLine(conditionSO, starredWeapons, isStarCondition) + Environment.NewLine;
		}
		return text;
	}

	private bool ShouldShowCalculatedWeaponStat(Enums.WeaponStatType weaponStatType, float calculatedValue)
	{
		if (weaponStatType == Enums.WeaponStatType.ProjectileCount)
		{
			return true;
		}
		if (!StatCalculator.TryGetItemStatTypeFromWeaponStatType(weaponStatType, out var itemStatType))
		{
			return false;
		}
		if (new List<Enums.ItemStatType>
		{
			Enums.ItemStatType.DamagePercentage,
			Enums.ItemStatType.ExplosionSizePercentage,
			Enums.ItemStatType.ProjectileSizePercentage
		}.Contains(itemStatType))
		{
			if (SingletonController<GameController>.Instance.Player.TryGetBaseStat(itemStatType, out var baseValue))
			{
				return Mathf.Abs(calculatedValue - baseValue) > 0f;
			}
			return false;
		}
		return calculatedValue > 0f;
	}

	private void SetupGlobal(WeaponInstance weapon)
	{
		bool active = HasGlobalStats(weapon.BaseWeaponSO);
		_globalStatsText.gameObject.SetActive(active);
		string empty = string.Empty;
		empty += GetGlobalStats(weapon);
		empty += GetStatsFromFormula(weapon);
		empty += GetEffects(weapon.WeaponAttackEffects, weapon.ItemRarity.AllFlags(), conditionsSatisfied: true);
		empty += GetDebuffs(weapon.WeaponAttackDebuffHandlers.Select((DebuffHandler x) => x.DebuffSO).ToList(), weapon.ItemRarity, conditionsSatisfied: true);
		empty += GetOverrides(weapon.BaseWeaponStatTypeCalculationOverrides, conditionsSatisfied: true, weapon.ItemRarity);
		empty += GetOverrides(weapon.BaseWeaponDamageCalculationOverrides, conditionsSatisfied: true, weapon.ItemRarity);
		_globalStatsText.SetText(empty);
	}

	private void SetupGlobal(WeaponSO weapon)
	{
		bool active = HasGlobalStats(weapon);
		_globalStatsText.gameObject.SetActive(active);
		string empty = string.Empty;
		empty += GetGlobalStats(weapon);
		empty += GetStatsFromFormula(weapon);
		empty += GetEffects(weapon.WeaponAttackEffects.ToList(), weapon.ItemRarity, conditionsSatisfied: true);
		empty += GetDebuffs(weapon.DebuffAttackEffects.ToList(), weapon.ItemRarity, conditionsSatisfied: true);
		empty += GetOverrides(weapon.Stats.WeaponStatTypeCalculationOverrides, conditionsSatisfied: true, weapon.ItemRarity);
		empty += GetOverrides(weapon.Stats.WeaponDamageCalculationOverrides, conditionsSatisfied: true, weapon.ItemRarity);
		_globalStatsText.SetText(empty);
	}

	internal void SetTitle(string name, Enums.PlaceableRarity rarity)
	{
		string sourceText = StringHelper.WeaponSprite + " <color=" + ColorHelper.GetColorHexcodeForRarity(rarity) + ">" + name + "</color>";
		_titleText.SetText(sourceText);
	}

	private void SetTags(Enums.PlaceableTag combinedPlaceableTags, Enums.PlaceableRarity placeableRarity, float width)
	{
		ClearContainer(_tagContainer);
		List<TooltipTagLine> list = new List<TooltipTagLine>();
		int num = 0;
		float num2 = 0f;
		TooltipTagLine tooltipTagLine = null;
		foreach (Enums.PlaceableTag item in OrderTags(combinedPlaceableTags.ListFlags()))
		{
			TooltipTag tooltipTag = CreateTag(item, placeableRarity);
			float num3 = tooltipTag.PreferredWidth();
			if (tooltipTagLine == null || num3 + num2 > width)
			{
				if (tooltipTagLine != null)
				{
					tooltipTagLine.ReverseOrder();
				}
				tooltipTagLine = UnityEngine.Object.Instantiate(_tooltipTagLinePrefab, _tagContainer);
				list.Add(tooltipTagLine);
				tooltipTagLine.Clear();
				num2 = 0f;
			}
			tooltipTag.transform.SetParent(tooltipTagLine.transform, worldPositionStays: false);
			num2 += num3;
			num++;
		}
		if (tooltipTagLine != null)
		{
			tooltipTagLine.ReverseOrder();
		}
		for (int i = 0; i < _tagContainer.childCount - 1; i++)
		{
			_tagContainer.GetChild(0).SetSiblingIndex(_tagContainer.childCount - 1 - i);
		}
	}

	internal TooltipTag CreateTag(Enums.PlaceableTag tag, Enums.PlaceableRarity rarity)
	{
		TooltipTag tooltipTag = UnityEngine.Object.Instantiate(_tagPrefab);
		tooltipTag.SetTag(tag, rarity);
		return tooltipTag;
	}

	private void SetRarity(WeaponSO WeaponSO)
	{
		_mainBorder.material = MaterialHelper.GetTooltipBorderRarityMaterial(WeaponSO.ItemRarity);
		Image[] imagesToSetRarityMaterialOn = _imagesToSetRarityMaterialOn;
		for (int i = 0; i < imagesToSetRarityMaterialOn.Length; i++)
		{
			imagesToSetRarityMaterialOn[i].material = MaterialHelper.GetInternalTooltipBorderRarityMaterial(WeaponSO.ItemRarity);
		}
		imagesToSetRarityMaterialOn = _imagesToSetFadeOn;
		foreach (Image imageToSetRarityMaterialOn in imagesToSetRarityMaterialOn)
		{
			SetupColoringOnBackdrop(WeaponSO, imageToSetRarityMaterialOn);
		}
	}

	private static void SetupColoringOnBackdrop(WeaponSO WeaponSO, Image imageToSetRarityMaterialOn)
	{
		if (WeaponSO.ItemRarity == Enums.PlaceableRarity.Common)
		{
			imageToSetRarityMaterialOn.color = new Color(255f, 255f, 255f, 0.8f);
		}
		else
		{
			imageToSetRarityMaterialOn.color = new Color(255f, 255f, 255f, 0.5f);
		}
	}

	private void SetPrice(WeaponSO weaponSO, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		string arg = "<size=16>" + StringHelper.CoinSprite + "</size>";
		if (owner == Enums.Backpack.DraggableOwner.Shop)
		{
			if (overriddenPrice > -1)
			{
				_priceText.SetText($"{overriddenPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, overriddenPrice))
				{
					_priceText.color = Color.green;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
			else
			{
				_priceText.SetText($"{weaponSO.BuyingPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, weaponSO.BuyingPrice))
				{
					_priceText.color = Color.white;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
		}
		else
		{
			_priceText.SetText($"{weaponSO.SellingPrice}  {arg}");
			_priceText.color = Color.white;
		}
	}

	private void SetPrice(WeaponInstance WeaponInstance, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		string arg = "<size=16>" + StringHelper.CoinSprite + "</size>";
		if (owner == Enums.Backpack.DraggableOwner.Shop)
		{
			if (overriddenPrice > -1)
			{
				_priceText.SetText($"{overriddenPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, overriddenPrice))
				{
					_priceText.color = Color.green;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
			else
			{
				_priceText.SetText($"{WeaponInstance.BuyingPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, WeaponInstance.BuyingPrice))
				{
					_priceText.color = Color.white;
				}
				else
				{
					_priceText.color = Color.red;
				}
			}
		}
		else
		{
			_priceText.SetText($"{WeaponInstance.SellingPrice}  {arg}");
			_priceText.color = Color.white;
		}
	}

	private string GetGlobalStats(WeaponInstance weaponInstance)
	{
		string text = string.Empty;
		foreach (Enums.WeaponStatType value in Enum.GetValues(typeof(Enums.WeaponStatType)))
		{
			if (!IsOnBlackList(value) && (value != Enums.WeaponStatType.Penetrating || weaponInstance.BaseWeaponSO.WeaponAttackTypeDurationAndPiercing != Enums.WeaponAttackTypeDurationAndPiercing.ShortAndInfinitePiercing) && (value != Enums.WeaponStatType.ProjectileDuration || weaponInstance.BaseWeaponSO.WeaponAttackTypeDurationAndPiercing != Enums.WeaponAttackTypeDurationAndPiercing.ShortAndInfinitePiercing) && (weaponInstance.BaseWeaponType != Enums.WeaponType.Melee || value != Enums.WeaponStatType.ProjectileCount) && (weaponInstance.BaseWeaponType != Enums.WeaponType.Melee || value != Enums.WeaponStatType.ProjectileSpeed))
			{
				float baseValue = 1f;
				if (value == Enums.WeaponStatType.StunChancePercentage)
				{
					baseValue = 0f;
				}
				if (value == Enums.WeaponStatType.LifeDrainPercentage)
				{
					baseValue = 0f;
				}
				if (value == Enums.WeaponStatType.Penetrating)
				{
					baseValue = 0f;
				}
				if (weaponInstance.BaseStatValues.ContainsKey(value))
				{
					baseValue = weaponInstance.BaseStatValues[value];
				}
				float calculatedStat = weaponInstance.GetCalculatedStat(value);
				if (ShouldShowCalculatedWeaponStat(value, calculatedStat))
				{
					text = text + TooltipSimpleStatHelper.CreateLine(value, baseValue, calculatedStat) + Environment.NewLine;
				}
			}
		}
		foreach (KeyValuePair<Enums.DamageType, float> item in weaponInstance.BaseDamageTypeValues.OrderBy((KeyValuePair<Enums.DamageType, float> x) => x.Value))
		{
			foreach (Enums.DamageType item2 in item.Key.ListFlags())
			{
				text = text + TooltipDamageTypeHelper.CreateLine(new KeyValuePair<Enums.DamageType, float>(item2, item.Value), weaponInstance.ItemRarity) + Environment.NewLine;
			}
		}
		return text;
	}

	private string GetGlobalStats(WeaponSO weaponSO)
	{
		string text = string.Empty;
		foreach (Enums.WeaponStatType value in Enum.GetValues(typeof(Enums.WeaponStatType)))
		{
			if (!IsOnBlackList(value) && (value != Enums.WeaponStatType.Penetrating || weaponSO.WeaponType != Enums.WeaponType.Melee) && (value != Enums.WeaponStatType.ProjectileCount || weaponSO.Stats.StatValues[Enums.WeaponStatType.ProjectileCount] != 1f) && (weaponSO.WeaponType != Enums.WeaponType.Melee || value != Enums.WeaponStatType.Penetrating || weaponSO.WeaponAttackTypeDurationAndPiercing != Enums.WeaponAttackTypeDurationAndPiercing.ShortAndInfinitePiercing))
			{
				float num = 0f;
				if (weaponSO.Stats.StatValues.ContainsKey(value))
				{
					num = weaponSO.Stats.StatValues[value];
				}
				if (num != 0f || value == Enums.WeaponStatType.ProjectileCount)
				{
					text = text + TooltipSimpleStatHelper.CreateLine(value, num, num) + Environment.NewLine;
				}
			}
		}
		foreach (KeyValuePair<Enums.DamageType, float> item in weaponSO.Stats.DamageTypeValues.OrderBy((KeyValuePair<Enums.DamageType, float> x) => x.Value))
		{
			foreach (Enums.DamageType item2 in item.Key.ListFlags())
			{
				text = text + TooltipDamageTypeHelper.CreateLine(new KeyValuePair<Enums.DamageType, float>(item2, item.Value), weaponSO.ItemRarity) + Environment.NewLine;
			}
		}
		return text;
	}

	private string GetStarredStats(WeaponInstance weaponInstance)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = GetConditionsAreSatisfied(weaponInstance.BaseWeaponSO.StarStats.Conditions);
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			float num = 0f;
			if (weaponInstance.BaseStarStatValues.ContainsKey(value))
			{
				num = weaponInstance.BaseStarStatValues[value];
			}
			if (num != 0f)
			{
				text = text + TooltipSimpleStatHelper.CreateLine(value, num, num, weaponInstance.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
			}
		}
		foreach (KeyValuePair<Enums.DamageType, float> item in weaponInstance.BaseStarDamageTypeValues.OrderBy((KeyValuePair<Enums.DamageType, float> x) => x.Value))
		{
			foreach (Enums.DamageType item2 in item.Key.ListFlags())
			{
				text = text + TooltipDamageTypeHelper.CreateLine(new KeyValuePair<Enums.DamageType, float>(item2, item.Value), weaponInstance.ItemRarity, isStarred: true, conditionsAreSatisfied) + Environment.NewLine;
			}
		}
		return text;
	}

	private string GetStarredStats(WeaponSO weaponSO)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = GetConditionsAreSatisfied(weaponSO.StarStats.Conditions);
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			float num = 0f;
			if (weaponSO.StarStats.StatValues.ContainsKey(value))
			{
				num = weaponSO.StarStats.StatValues[value];
			}
			if (num != 0f)
			{
				text = text + TooltipSimpleStatHelper.CreateLine(value, num, num, weaponSO.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
			}
		}
		foreach (KeyValuePair<Enums.DamageType, float> item in weaponSO.StarStats.DamageTypeValues.OrderBy((KeyValuePair<Enums.DamageType, float> x) => x.Value))
		{
			foreach (Enums.DamageType item2 in item.Key.ListFlags())
			{
				text = text + TooltipDamageTypeHelper.CreateLine(new KeyValuePair<Enums.DamageType, float>(item2, item.Value), weaponSO.ItemRarity, isStarred: true, conditionsAreSatisfied) + Environment.NewLine;
			}
		}
		return text;
	}

	private string GetStatsFromFormula(WeaponInstance weapon)
	{
		if (weapon.BaseWeaponSO.Stats == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		if (weapon.BaseWeaponSO.Stats.FormulaStats != null)
		{
			foreach (Enums.WeaponStatType value in Enum.GetValues(typeof(Enums.WeaponStatType)))
			{
				bool flag = false;
				if (weapon.BaseWeaponSO.Stats.FormulaStats.ContainsKey(value))
				{
					flag = true;
				}
				if (flag)
				{
					float formulaResult = weapon.BaseWeaponSO.Stats.FormulaStats[value].GetFormulaResult();
					text = text + TooltipFormulaHelper.CreateLine(value, 0f, formulaResult, weapon.ItemRarity, weapon.BaseWeaponSO.Stats.FormulaStats[value]) + Environment.NewLine;
				}
			}
		}
		return text;
	}

	private string GetStatsFromFormula(WeaponSO weapon)
	{
		if (weapon.Stats == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		if (weapon.Stats.FormulaStats != null)
		{
			foreach (Enums.WeaponStatType value in Enum.GetValues(typeof(Enums.WeaponStatType)))
			{
				bool flag = false;
				if (weapon.Stats.FormulaStats.ContainsKey(value))
				{
					flag = true;
				}
				if (flag)
				{
					float formulaResult = weapon.Stats.FormulaStats[value].GetFormulaResult();
					text = text + TooltipFormulaHelper.CreateLine(value, 0f, formulaResult, weapon.ItemRarity, weapon.Stats.FormulaStats[value]) + Environment.NewLine;
				}
			}
		}
		return text;
	}

	private string GetStarredStatsFromFormula(WeaponSO weapon)
	{
		if (weapon.Stats == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		bool conditionsAreSatisfied = GetConditionsAreSatisfied(weapon.StarStats.Conditions);
		if (weapon.Stats.FormulaStats != null)
		{
			foreach (Enums.WeaponStatType value in Enum.GetValues(typeof(Enums.WeaponStatType)))
			{
				bool flag = false;
				if (weapon.Stats.FormulaStats.ContainsKey(value))
				{
					flag = true;
				}
				if (flag)
				{
					float formulaResult = weapon.Stats.FormulaStats[value].GetFormulaResult();
					text = text + TooltipFormulaHelper.CreateLine(value, 0f, formulaResult, weapon.ItemRarity, weapon.Stats.FormulaStats[value], conditionsAreSatisfied) + Environment.NewLine;
				}
			}
		}
		if (weapon.StarStats.FormulaDamageTypeValues != null)
		{
			foreach (Enums.DamageType value2 in Enum.GetValues(typeof(Enums.DamageType)))
			{
				bool flag2 = false;
				if (weapon.StarStats.FormulaDamageTypeValues.ContainsKey(value2))
				{
					flag2 = true;
				}
				if (flag2)
				{
					float formulaResult2 = weapon.StarStats.FormulaDamageTypeValues[value2].GetFormulaResult();
					text = text + TooltipFormulaHelper.CreateLine(value2, 0f, formulaResult2, weapon.StarStats.FormulaDamageTypeValues[value2], weapon.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
				}
			}
		}
		return text;
	}

	private string GetStarredStatsFromFormula(WeaponInstance weapon)
	{
		if (weapon.BaseWeaponSO.StarStats == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		bool conditionsAreSatisfied = GetConditionsAreSatisfied(weapon.BaseWeaponSO.StarStats.Conditions);
		if (weapon.BaseWeaponSO.StarStats.FormulaStats != null)
		{
			foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
			{
				bool flag = false;
				if (weapon.BaseWeaponSO.StarStats.FormulaStats.ContainsKey(value))
				{
					flag = true;
				}
				if (flag)
				{
					float formulaResult = weapon.BaseWeaponSO.StarStats.FormulaStats[value].GetFormulaResult();
					text = text + TooltipFormulaHelper.CreateLine(value, 0f, formulaResult, weapon.BaseWeaponSO.StarStats.FormulaStats[value], weapon.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
				}
			}
		}
		if (weapon.BaseWeaponSO.StarStats.FormulaDamageTypeValues != null)
		{
			foreach (Enums.DamageType value2 in Enum.GetValues(typeof(Enums.DamageType)))
			{
				bool flag2 = false;
				if (weapon.BaseWeaponSO.StarStats.FormulaDamageTypeValues.ContainsKey(value2))
				{
					flag2 = true;
				}
				if (flag2)
				{
					float formulaResult2 = weapon.BaseWeaponSO.StarStats.FormulaDamageTypeValues[value2].GetFormulaResult();
					text = text + TooltipFormulaHelper.CreateLine(value2, 0f, formulaResult2, weapon.BaseWeaponSO.StarStats.FormulaDamageTypeValues[value2], weapon.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
				}
			}
		}
		return text;
	}

	internal string GetEffects(List<WeaponAttackEffect> weaponAttackEffects, Enums.PlaceableRarity rarity, bool conditionsSatisfied)
	{
		string text = string.Empty;
		foreach (WeaponAttackEffect weaponAttackEffect in weaponAttackEffects)
		{
			if (!string.IsNullOrEmpty(weaponAttackEffect.SimpleEffectDescription))
			{
				text = text + TooltipEffectHelper.CreateLine(weaponAttackEffect, rarity, conditionsSatisfied) + Environment.NewLine;
			}
		}
		return text;
	}

	internal string GetDebuffs(List<DebuffSO> debuffSOs, Enums.PlaceableRarity rarity, bool conditionsSatisfied)
	{
		string text = string.Empty;
		foreach (DebuffSO debuffSO in debuffSOs)
		{
			text = text + TooltipDebuffHelper.CreateLine(debuffSO, conditionsSatisfied, rarity) + Environment.NewLine;
		}
		return text;
	}

	internal string GetOverrides(WeaponStatTypeCalculationOverride[] weaponDamageTypeValueOverrides, bool conditionsSatisfied, Enums.PlaceableRarity rarity)
	{
		string text = string.Empty;
		foreach (WeaponStatTypeCalculationOverride weaponStatTypeCalculationOverride in weaponDamageTypeValueOverrides)
		{
			text = text + TooltipOverrideHelper.CreateLine(weaponStatTypeCalculationOverride.Description, conditionsSatisfied, rarity) + Environment.NewLine;
		}
		return text;
	}

	internal string GetOverrides(WeaponDamageTypeValueOverride[] weaponDamageTypeValueOverrides, bool conditionsSatisfied, Enums.PlaceableRarity rarity)
	{
		string text = string.Empty;
		foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in weaponDamageTypeValueOverrides)
		{
			text = text + TooltipOverrideHelper.CreateLine(weaponDamageTypeValueOverride.SourceWeaponDamageType, weaponDamageTypeValueOverride.TargetWeaponDamageType, conditionsSatisfied, rarity) + Environment.NewLine;
		}
		return text;
	}

	internal string GetOverrides(WeaponDamageCalculationOverride[] weaponDamageTypeValueOverrides, bool conditionsSatisfied, Enums.PlaceableRarity rarity)
	{
		string text = string.Empty;
		foreach (WeaponDamageCalculationOverride weaponDamageCalculationOverride in weaponDamageTypeValueOverrides)
		{
			text = text + TooltipOverrideHelper.CreateLine(weaponDamageCalculationOverride.Description, conditionsSatisfied, rarity) + Environment.NewLine;
		}
		return text;
	}

	private void SetSizeImage(WeaponSO WeaponSO)
	{
		_tooltipSizeContainer.SetSize(WeaponSO.ItemSize, WeaponSO.StarringEffectIsPositive);
	}

	private void SetupComplexTooltips(WeaponSO weapon)
	{
		bool show = SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.AlwaysVisible || (SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.VisibleOnAlt && SingletonController<InputController>.Instance.AltIsDown);
		_hasComplexTooltipToShow = ToggleAdditionalTips(weapon.ItemRarity) && _allowShowComplexTooltips;
		ToggleAltTooltip(show);
		SetHotkey();
	}

	internal void SetHotkey()
	{
		if (SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.VisibleOnAlt)
		{
			bool currentControlschemeIsKeyboard = SingletonController<InputController>.Instance.CurrentControlschemeIsKeyboard;
			_altHotkeyContainer.SetActive(_hasComplexTooltipToShow);
			_altHotkeyKeyboard.SetActive(_hasComplexTooltipToShow && currentControlschemeIsKeyboard);
			_altHotkeyController.SetActive(_hasComplexTooltipToShow && !currentControlschemeIsKeyboard);
		}
		else
		{
			_altHotkeyContainer.SetActive(value: false);
		}
	}

	internal override void ToggleAltTooltip(bool show)
	{
		_tipContainer.gameObject.SetActive(show && _hasComplexTooltipToShow);
		if (show)
		{
			_altHotkeyText.color = Color.yellow;
			_altHotkeyKeyboard.GetComponent<Image>().color = Color.yellow;
		}
		else
		{
			_altHotkeyText.color = Color.white;
			_altHotkeyKeyboard.GetComponent<Image>().color = Color.white;
		}
		ToggleInTooltipTips(show && _inTooltipComplexElements.Any());
	}

	public bool ToggleAdditionalTips(Enums.PlaceableRarity rarity)
	{
		ClearContainer(_tipContainer);
		bool flag = false;
		bool flag2 = false;
		bool flag3 = false;
		bool flag4 = false;
		bool flag5 = false;
		if (_activeWeaponSO != null)
		{
			flag = ToggleAdditionalTips(_activeWeaponSO);
		}
		if (_activeWeaponInstance != null)
		{
			flag = ToggleAdditionalTips(_activeWeaponInstance);
		}
		if (_activeWeaponSO != null && _activeWeaponSO.Stats != null)
		{
			flag2 = AddFormulaTips(_activeWeaponSO.Stats.FormulaStats, _activeWeaponSO.ItemRarity);
		}
		if (_activeWeaponInstance != null && _activeWeaponInstance.BaseWeaponSO.Stats != null)
		{
			flag2 = AddFormulaTips(_activeWeaponInstance.BaseWeaponSO.Stats.FormulaStats, _activeWeaponInstance.BaseWeaponSO.ItemRarity);
		}
		if (_activeWeaponSO != null && _activeWeaponSO.StarStats != null)
		{
			flag3 = AddFormulaTips(_activeWeaponSO.StarStats.FormulaStats, _activeWeaponSO.ItemRarity);
		}
		if (_activeWeaponInstance != null && _activeWeaponInstance.BaseWeaponSO.StarStats != null)
		{
			flag3 = AddFormulaTips(_activeWeaponInstance.BaseWeaponSO.StarStats.FormulaStats, _activeWeaponInstance.BaseWeaponSO.ItemRarity);
		}
		if (_activeWeaponSO != null && _activeWeaponSO.StarStats != null)
		{
			flag4 = AddFormulaTips(_activeWeaponSO.StarStats.FormulaDamageTypeValues, _activeWeaponSO.ItemRarity);
		}
		if (_activeWeaponInstance != null && _activeWeaponInstance.BaseWeaponSO.StarStats != null)
		{
			flag4 = AddFormulaTips(_activeWeaponInstance.BaseWeaponSO.StarStats.FormulaDamageTypeValues, _activeWeaponInstance.BaseWeaponSO.ItemRarity);
		}
		return flag || flag5 || flag3 || flag4 || flag2;
	}

	private void ToggleInTooltipTips(bool show)
	{
		foreach (GameObject inTooltipComplexElement in _inTooltipComplexElements)
		{
			inTooltipComplexElement.SetActive(show);
		}
	}

	private bool AddFormulaTips(SerializableDictionaryBase<Enums.WeaponStatType, FormulaSO> formulaStats, Enums.PlaceableRarity rarity)
	{
		if (formulaStats == null)
		{
			return false;
		}
		foreach (KeyValuePair<Enums.WeaponStatType, FormulaSO> formulaStat in formulaStats)
		{
			UnityEngine.Object.Instantiate(_itemTooltipComplexFormulaTipPrefab, _tipContainer).Init(formulaStat.Value, formulaStat.Key, rarity);
		}
		return formulaStats.Any();
	}

	private bool AddFormulaTips(SerializableDictionaryBase<Enums.ItemStatType, FormulaSO> formulaStats, Enums.PlaceableRarity rarity)
	{
		if (formulaStats == null)
		{
			return false;
		}
		foreach (KeyValuePair<Enums.ItemStatType, FormulaSO> formulaStat in formulaStats)
		{
			UnityEngine.Object.Instantiate(_itemTooltipComplexFormulaTipPrefab, _tipContainer).Init(formulaStat.Value, formulaStat.Key, rarity);
		}
		return formulaStats.Any();
	}

	private bool AddFormulaTips(SerializableDictionaryBase<Enums.DamageType, FormulaSO> formulaStats, Enums.PlaceableRarity rarity)
	{
		if (formulaStats == null)
		{
			return false;
		}
		foreach (KeyValuePair<Enums.DamageType, FormulaSO> formulaStat in formulaStats)
		{
			UnityEngine.Object.Instantiate(_itemTooltipComplexFormulaTipPrefab, _tipContainer).Init(formulaStat.Value, formulaStat.Key, rarity);
		}
		return formulaStats.Any();
	}

	private bool ToggleAdditionalTips(WeaponInstance weapon)
	{
		List<TipFeedbackElement> tipFeedbackElements = GetTipFeedbackElements(weapon);
		foreach (TipFeedbackElement item in tipFeedbackElements)
		{
			UnityEngine.Object.Instantiate(_itemTooltipComplexTipPrefab, _tipContainer).Init(item, weapon.BaseWeaponSO.ItemRarity);
		}
		return tipFeedbackElements.Any();
	}

	private bool ToggleAdditionalTips(WeaponSO weapon)
	{
		List<TipFeedbackElement> tipFeedbackElements = GetTipFeedbackElements(weapon);
		foreach (TipFeedbackElement item in tipFeedbackElements)
		{
			UnityEngine.Object.Instantiate(_itemTooltipComplexTipPrefab, _tipContainer).Init(item, weapon.ItemRarity);
		}
		return tipFeedbackElements.Any();
	}

	private List<TipFeedbackElement> GetTipFeedbackElements(WeaponInstance weaponInstance)
	{
		List<TipFeedbackElement> list = new List<TipFeedbackElement>();
		list.AddRange(GetTipFeedbackElementsFromDebuffs(weaponInstance.WeaponAttackDebuffHandlers.Select((DebuffHandler x) => x.DebuffSO).ToArray()));
		return list;
	}

	private List<TipFeedbackElement> GetTipFeedbackElements(WeaponSO weaponSO)
	{
		List<TipFeedbackElement> list = new List<TipFeedbackElement>();
		list.AddRange(GetTipFeedbackElementsFromDebuffs(weaponSO.DebuffAttackEffects));
		return list;
	}

	private bool IsOnBlackList(Enums.WeaponStatType weaponStatType)
	{
		return _blacklist.Contains(weaponStatType);
	}

	private bool HasGlobalStats(WeaponSO item)
	{
		if (item.Stats != null)
		{
			return item.Stats.ContainsData();
		}
		return false;
	}

	private bool HasStarredStats(WeaponSO item)
	{
		if (item.StarStats != null)
		{
			return item.StarStats.ContainsData();
		}
		return false;
	}

	private bool HasStarredConditions(WeaponSO item)
	{
		if (HasStarredStats(item) && item.StarStats.Conditions != null)
		{
			return item.StarStats.Conditions.Any();
		}
		return false;
	}

	private bool GetConditionsAreSatisfied(ConditionSO[] conditions, WeaponInstance weaponInstance = null)
	{
		for (int i = 0; i < conditions.Length; i++)
		{
			Condition condition = new Condition(conditions[i]);
			if (!IsGlobalWeaponlessWeaponCondition(condition, weaponInstance) && !condition.IsConditionSatisfied(weaponInstance))
			{
				return false;
			}
		}
		return true;
	}

	private bool IsGlobalWeaponlessWeaponCondition(Condition condition, WeaponInstance weaponInstance)
	{
		if (condition.ConditionTarget == Enums.ConditionalStats.ConditionTarget.Weapon)
		{
			return weaponInstance == null;
		}
		return false;
	}

	private void SetupDebug(WeaponSO weapon)
	{
		if (GameDatabaseHelper.AllowDebugging)
		{
			SetTitle($"{weapon.Name} ({weapon.Id})", weapon.ItemRarity);
		}
	}
}
