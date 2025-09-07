using System;
using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Assets.UI.Tooltips;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Effects;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.Game.Input;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.Game.Items.RuneEffects;
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

public class CleanItemTooltip : BaseTooltip
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

	[Header("Size")]
	[SerializeField]
	private TooltipSizeContainer _tooltipSizeContainer;

	[SerializeField]
	private Transform _tooltipSizeRootContainer;

	[Header("Global")]
	[SerializeField]
	private TextMeshProUGUI _globalStatsText;

	[Header("Conditions")]
	[SerializeField]
	private Transform _conditionsContainer;

	[SerializeField]
	private Image _conditionalContentBackdrop;

	[SerializeField]
	private Transform _conditionsMarker;

	[SerializeField]
	private TextMeshProUGUI _conditionsText;

	[SerializeField]
	private TextMeshProUGUI _conditionalStatsText;

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

	[Header("Special")]
	[SerializeField]
	private TextMeshProUGUI _specialTriggerDescription;

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
	private GameObject _altHotkeyController;

	[SerializeField]
	private TextMeshProUGUI _altHotkeyText;

	[SerializeField]
	private Transform _tipContainer;

	[SerializeField]
	private ItemTooltipComplexTip _itemTooltipComplexTipPrefab;

	[SerializeField]
	private ItemTooltipComplexFormulaTip _itemTooltipComplexFormulaTipPrefab;

	[SerializeField]
	private GameObject _collectedIcon;

	private ItemSO _activeItemSO;

	private ItemInstance _activeItemInstance;

	private bool _hasComplexTooltipToShow;

	private bool _shouldShowComplex;

	private List<GameObject> _inTooltipComplexElements = new List<GameObject>();

	public void SetItem(ItemInstance item, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_activeItemInstance = item;
		_activeItemSO = null;
		SetTitle(item.BaseItemSO.Name, item.BaseItemSO.ItemRarity);
		SetRuneDescription(item.ItemSO, item.RuneSpecialEffect);
		SetSizeImage(item.ItemSO);
		SetRarity(item.ItemSO);
		SetupGlobal(item);
		SetupConditional(item);
		SetupStarred(item);
		SetPrice(item, owner, overriddenPrice);
		SetupComplexTooltips(item.ItemSO);
		SetupNewCollected(item.ItemSO);
		SetupDebug(item.ItemSO);
		Vector2 vector = SetTooltipSize(item.ItemSO);
		SetTags(EnumHelper.GetCombinedPlaceableTags(item), item.BaseItemSO.ItemRarity, vector.x);
	}

	public void SetItem(ItemSO item, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
	{
		_activeItemInstance = null;
		_activeItemSO = item;
		SetTitle(item.Name, item.ItemRarity);
		SetRuneDescription(item, item.RuneSpecialEffect);
		SetSizeImage(item);
		SetRarity(item);
		SetupGlobal(item);
		SetupConditional(item);
		SetupStarred(item);
		SetPrice(item, owner, overriddenPrice);
		SetupComplexTooltips(item);
		SetupNewCollected(item);
		SetupDebug(item);
		Vector2 vector = SetTooltipSize(item);
		SetTags(EnumHelper.GetCombinedPlaceableTags(item), item.ItemRarity, vector.x);
	}

	private Vector2 SetTooltipSize(ItemSO item)
	{
		float num = 0f;
		float num2 = 0f;
		num += ((RectTransform)_titleContainer).sizeDelta.y;
		bool hasGlobalStats = HasGlobalStats(item);
		bool flag = HasConditionalStats(item);
		bool hasStarredStats = HasStarredStats(item);
		bool flag2 = HasStarredConditions(item);
		bool hasSpecialTrigger = HasSpecialTrigger(item);
		float num3 = CalculateContentHeight(hasGlobalStats, flag, hasStarredStats, flag2, hasSpecialTrigger);
		num += num3;
		float num4 = CalculateContentWidth(hasGlobalStats, flag, hasStarredStats, hasSpecialTrigger);
		num2 += num4;
		if (flag)
		{
			SetWidthForConditionalMarker();
		}
		if (flag2)
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

	private void SetWidthForConditionalMarker()
	{
		if (!(_conditionsMarker == null))
		{
			((RectTransform)_conditionsMarker).LeanSize(new Vector2(_conditionsText.preferredWidth, 12f), 0f).setIgnoreTimeScale(useUnScaledTime: true);
		}
	}

	private float CalculateContentWidth(bool hasGlobalStats, bool hasConditionalStats, bool hasStarredStats, bool hasSpecialTrigger)
	{
		float y = GetSizeContainerSizeVector().y;
		float a = y;
		a = Mathf.Max(a, _titleText.preferredWidth);
		if (hasSpecialTrigger)
		{
			float preferredWidth = _specialTriggerDescription.preferredWidth;
			a = Mathf.Max(a, preferredWidth + y);
		}
		if (hasGlobalStats)
		{
			float preferredWidth2 = _globalStatsText.preferredWidth;
			a = Mathf.Max(a, preferredWidth2 + y);
		}
		if (hasConditionalStats)
		{
			a = GetConditionalWidth(a);
		}
		if (hasStarredStats)
		{
			float preferredWidth3 = _starConditionsText.preferredWidth;
			preferredWidth3 += _baseSpacingHeight;
			a = Mathf.Max(a, preferredWidth3 + y);
			float preferredWidth4 = _starStatsText.preferredWidth;
			preferredWidth4 += _baseSpacingHeight;
			float x = ((RectTransform)_starContainer).sizeDelta.x;
			preferredWidth4 = MathF.Max(preferredWidth4, x);
			a = Mathf.Max(a, preferredWidth4 + y);
		}
		a += _baseSpacingHeight;
		a += _baseSpacingHeight;
		a += 10f;
		return a + _baseSpacingHeight;
	}

	private float GetConditionalWidth(float actualWidth)
	{
		float x = GetSizeContainerSizeVector().x;
		float preferredWidth = _conditionsText.preferredWidth;
		preferredWidth += _baseSpacingHeight;
		actualWidth = Mathf.Max(actualWidth, preferredWidth + x);
		float preferredWidth2 = _conditionalStatsText.preferredWidth;
		preferredWidth2 += _baseSpacingHeight;
		actualWidth = Mathf.Max(actualWidth, preferredWidth2 + x);
		return actualWidth;
	}

	private Vector2 GetSizeContainerSizeVector()
	{
		return ((RectTransform)_tooltipSizeRootContainer.transform).sizeDelta;
	}

	private float CalculateContentHeight(bool hasGlobalStats, bool hasConditionalStats, bool hasStarredStats, bool hasStarredConditions, bool hasSpecialTrigger)
	{
		float y = GetSizeContainerSizeVector().y;
		float num = 0f;
		num += _baseSpacingHeight * 4f;
		if (hasSpecialTrigger)
		{
			_specialTriggerDescription.ForceMeshUpdate();
			num += _specialTriggerDescription.preferredHeight;
			num += _baseSpacingHeight;
		}
		if (hasGlobalStats)
		{
			_globalStatsText.ForceMeshUpdate();
			num += _globalStatsText.preferredHeight;
			num += _baseSpacingHeight;
		}
		if (hasConditionalStats)
		{
			num += _baseSpacingHeight;
			num += _conditionsText.preferredHeight;
			if (_conditionsMarker != null)
			{
				num += ((RectTransform)_conditionsMarker.transform).sizeDelta.y;
			}
			num += _conditionalStatsText.preferredHeight;
			num += _baseSpacingHeight;
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
		if (_hasComplexTooltipToShow && SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.VisibleOnAlt)
		{
			num += ((RectTransform)_altHotkeyContainer.transform).sizeDelta.y;
		}
		return Mathf.Max(y + 16f, num);
	}

	private void SetupNewCollected(ItemSO item)
	{
		if (_showCollectedFlag)
		{
			_collectedIcon.SetActive(!SingletonController<CollectionController>.Instance.IsItemUnlocked(item.Id));
		}
		else
		{
			_collectedIcon.SetActive(value: false);
		}
	}

	private void SetupStarred(ItemInstance item)
	{
		bool flag = HasStarredStats(item.ItemSO);
		_starContainer.gameObject.SetActive(flag);
		string text = string.Empty;
		if (flag)
		{
			string text2 = (item.ItemSO.StarringEffectIsPositive ? StringHelper.PositiveStarSprite : StringHelper.NegativeStarSprite);
			_starTitleText.SetText(text2 + " STARRED ITEM");
			bool flag2 = item.ItemSO.GlobalStats != null || item.ItemSO.ConditionalStats != null;
			_starContentBackdrop.enabled = flag2;
			bool flag3 = HasStarredConditions(item.ItemSO);
			if (_starConditionsMarker != null)
			{
				_starConditionsMarker.SetActive(flag3);
			}
			if (flag3)
			{
				_starConditionsText.gameObject.SetActive(value: true);
				string empty = string.Empty;
				empty += GetConditions(item.StarWeaponConditions, item, isStarCondition: true);
				_starConditionsText.SetText(empty);
			}
			else
			{
				_starConditionsText.gameObject.SetActive(value: false);
				_starConditionsText.SetText(string.Empty);
			}
			text += GetStats(item, item.StarStatValues, item.StarDamageTypeValues, isGlobal: false, item.StarWeaponConditions);
			text += GetStatsFromFormula(item, item.ItemSO.StarredWeaponStats.FormulaStats, item.ItemSO.StarredWeaponStats.FormulaDamageTypeValues, isGlobal: false, item.StarWeaponConditions);
			text += GetDebuffs(item, item.StarDebuffSOs, isStarred: true, isGlobal: false);
			text += GetEffects(item, item.StarWeaponAttackEffects, isStarred: true, isGlobal: false);
			text += GetOverrides(item, item.StarWeaponDamageTypeValueOverrides, isStarred: true, isGlobal: false);
		}
		_starStatsText.SetText(text);
	}

	private void SetupStarred(ItemSO item)
	{
		bool flag = HasStarredStats(item);
		_starContainer.gameObject.SetActive(flag);
		string text = string.Empty;
		if (flag)
		{
			string text2 = (item.StarringEffectIsPositive ? StringHelper.PositiveStarSprite : StringHelper.NegativeStarSprite);
			_starTitleText.SetText(text2 + " STARRED ITEM");
			bool flag2 = item.GlobalStats != null || item.ConditionalStats != null;
			_starContentBackdrop.enabled = flag2;
			bool flag3 = HasStarredConditions(item);
			if (_starConditionsMarker != null)
			{
				_starConditionsMarker.SetActive(flag3);
			}
			if (flag3)
			{
				string empty = string.Empty;
				empty += GetConditions(item.StarredWeaponConditions, null, isStarCondition: true);
				_starConditionsText.SetText(empty);
			}
			else
			{
				_starConditionsText.SetText(string.Empty);
			}
			text += GetStats(item, item.StarredWeaponStatValues, item.StarredWeaponDamageTypeValues, isGlobal: false, item.StarredWeaponConditions);
			text += GetStatsFromFormula(item, item.StarredWeaponStats.FormulaStats, item.StarredWeaponStats.FormulaDamageTypeValues, isGlobal: false, item.StarredWeaponConditions);
			text += GetDebuffs(item, item.StarredWeaponDebuffSOs, isStarred: true, isGlobal: false);
			text += GetEffects(item, item.StarredWeaponAttackEffects, isStarred: true, isGlobal: false);
			text += GetOverrides(item, item.StarredWeaponDamageTypeValueOverrides, isStarred: true, isGlobal: false);
		}
		_starStatsText.SetText(text);
	}

	private void SetupConditional(ItemInstance item)
	{
		bool flag = HasConditionalStats(item.ItemSO);
		_conditionsContainer.gameObject.SetActive(flag);
		string text = string.Empty;
		string text2 = string.Empty;
		if (flag)
		{
			bool flag2 = item.ItemSO.GlobalStats != null || item.ItemSO.StarredWeaponStats != null;
			_conditionalContentBackdrop.enabled = flag2;
			text += GetConditions(item.ConditionalStatConditions, item, isStarCondition: false);
			_conditionsText.SetText(text);
			text2 += GetStats(item, item.ConditionalStatValues, item.ConditionalDamageTypeValues, isGlobal: false, item.ConditionalStatConditions);
			text2 += GetStatsFromFormula(item, item.ItemSO.ConditionalStats.FormulaStats, item.ItemSO.ConditionalStats.FormulaDamageTypeValues, isGlobal: false, item.ConditionalStatConditions);
			text2 += GetDebuffs(item, item.ConditionalDebuffSOs, isStarred: false, isGlobal: false);
			text2 += GetEffects(item, item.ConditionalWeaponAttackEffects, isStarred: false, isGlobal: false);
			text2 += GetOverrides(item, item.ConditionalWeaponDamageTypeValueOverrides, isStarred: false, isGlobal: false);
		}
		_conditionalStatsText.SetText(text2);
		_conditionsText.SetText(text);
	}

	private void SetupConditional(ItemSO item)
	{
		bool flag = HasConditionalStats(item);
		_conditionsContainer.gameObject.SetActive(flag);
		string text = string.Empty;
		string text2 = string.Empty;
		if (flag)
		{
			bool flag2 = item.GlobalStats != null || item.StarredWeaponStats != null;
			_conditionalContentBackdrop.enabled = flag2;
			text += GetConditions(item.ConditionalStatConditions, null, isStarCondition: false);
			_conditionsText.SetText(text);
			text2 += GetStats(item, item.ConditionalStatValues, item.ConditionalDamageTypeValues, isGlobal: false, item.ConditionalStatConditions);
			text2 += GetStatsFromFormula(item, item.ConditionalStats.FormulaStats, item.ConditionalStats.FormulaDamageTypeValues, isGlobal: false, item.ConditionalStatConditions);
			text2 += GetDebuffs(item, item.ConditionalDebuffSOs, isStarred: false, isGlobal: false);
			text2 += GetEffects(item, item.ConditionalWeaponFilterAttackEffects, isStarred: false, isGlobal: false);
			text2 += GetOverrides(item, item.ConditionalWeaponDamageTypeValueOverrides, isStarred: false, isGlobal: false);
		}
		_conditionalStatsText.SetText(text2);
		_conditionsText.SetText(text);
	}

	private void SetupGlobal(ItemInstance item)
	{
		bool active = HasGlobalStats(item.ItemSO);
		_globalStatsText.gameObject.SetActive(active);
		string empty = string.Empty;
		empty += GetStats(item, item.GlobalStatValues, item.GlobalDamageTypeValues, isGlobal: true);
		empty += GetStatsFromFormula(item, item.ItemSO.GlobalStats.FormulaStats, item.ItemSO.GlobalStats.FormulaDamageTypeValues, isGlobal: true);
		empty += GetEffects(item, item.GlobalAttackEffects, isStarred: false, isGlobal: true);
		empty += GetDebuffs(item, item.GlobalDebuffSOs, isStarred: false, isGlobal: true);
		empty += GetOverrides(item, item.GlobalDamageTypeValueOverrides, isStarred: false, isGlobal: true);
		_globalStatsText.SetText(empty);
	}

	private void SetupGlobal(ItemSO item)
	{
		bool active = HasGlobalStats(item);
		_globalStatsText.gameObject.SetActive(active);
		string empty = string.Empty;
		empty += GetStats(item, item.GlobalStats.StatValues, item.GlobalStats.DamageTypeValues, isGlobal: true);
		empty += GetStatsFromFormula(item, item.GlobalStats.FormulaStats, item.GlobalStats.FormulaDamageTypeValues, isGlobal: true);
		empty += GetEffects(item, item.GlobalAttackEffects, isStarred: false, isGlobal: true);
		empty += GetDebuffs(item, item.GlobalDebuffSOs, isStarred: false, isGlobal: true);
		empty += GetOverrides(item, item.GlobalDamageTypeValueOverrides, isStarred: false, isGlobal: true);
		_globalStatsText.SetText(empty);
	}

	internal void SetTitle(string name, Enums.PlaceableRarity rarity)
	{
		string sourceText = StringHelper.ItemSprite + " <color=" + ColorHelper.GetColorHexcodeForRarity(rarity) + ">" + name + "</color>";
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

	private void SetRuneDescription(ItemSO itemSO, RuneSpecialEffect runeSpecialEffect)
	{
		if (HasSpecialTrigger(itemSO))
		{
			_specialTriggerDescription.gameObject.SetActive(value: true);
			string empty = string.Empty;
			empty += SetupDescriptionTrigger(runeSpecialEffect);
			empty += Environment.NewLine;
			empty += SetupDestructionText(runeSpecialEffect);
			_specialTriggerDescription.SetText(empty);
		}
		else
		{
			_specialTriggerDescription.gameObject.SetActive(value: false);
		}
	}

	private string SetupDescriptionTrigger(RuneSpecialEffect runeSpecialEffect)
	{
		return "<size=16>" + StringHelper.SpecialItemActivationSprite + "</size>  " + runeSpecialEffect.GetDescription();
	}

	private string SetupDestructionText(RuneSpecialEffect runeSpecialEffect)
	{
		string text = StringHelper.GetCleanString(runeSpecialEffect.RuneSpecialEffectDestructionType);
		if (runeSpecialEffect.RuneSpecialEffectDestructionType == Enums.RuneSpecialEffectDestructionType.DestroyAfterX)
		{
			text = text.Replace("[CurrentTriggerCount]", runeSpecialEffect.CurrentTriggerCount.ToString());
			text = text.Replace("[MaxTriggerCount]", runeSpecialEffect.MaxTriggerCount.ToString());
		}
		return StringHelper.GetSpriteValue(runeSpecialEffect.RuneSpecialEffectDestructionType) + "  " + text;
	}

	private void SetRarity(ItemSO itemSO)
	{
		_mainBorder.material = MaterialHelper.GetTooltipBorderRarityMaterial(itemSO.ItemRarity);
		Image[] imagesToSetRarityMaterialOn = _imagesToSetRarityMaterialOn;
		for (int i = 0; i < imagesToSetRarityMaterialOn.Length; i++)
		{
			imagesToSetRarityMaterialOn[i].material = MaterialHelper.GetInternalTooltipBorderRarityMaterial(itemSO.ItemRarity);
		}
		imagesToSetRarityMaterialOn = _imagesToSetFadeOn;
		foreach (Image imageToSetRarityMaterialOn in imagesToSetRarityMaterialOn)
		{
			SetupColoringOnBackdrop(itemSO, imageToSetRarityMaterialOn);
		}
	}

	private static void SetupColoringOnBackdrop(ItemSO itemSO, Image imageToSetRarityMaterialOn)
	{
		if (itemSO.ItemRarity == Enums.PlaceableRarity.Common)
		{
			imageToSetRarityMaterialOn.color = new Color(255f, 255f, 255f, 0.8f);
		}
		else
		{
			imageToSetRarityMaterialOn.color = new Color(255f, 255f, 255f, 0.5f);
		}
	}

	private void SetPrice(ItemSO itemSO, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
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
				_priceText.SetText($"{itemSO.BuyingPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, itemSO.BuyingPrice))
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
			_priceText.SetText($"{itemSO.SellingPrice}  {arg}");
			_priceText.color = Color.white;
		}
	}

	private void SetPrice(ItemInstance itemInstance, Enums.Backpack.DraggableOwner owner, int overriddenPrice)
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
				_priceText.SetText($"{itemInstance.BuyingPrice}  {arg}");
				if (SingletonController<CurrencyController>.Instance.CanAfford(Enums.CurrencyType.Coins, itemInstance.BuyingPrice))
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
			_priceText.SetText($"{itemInstance.SellingPrice}  {arg}");
			_priceText.color = Color.white;
		}
	}

	private string GetStats(ItemInstance item, Dictionary<Enums.ItemStatType, float> statValues, Dictionary<Enums.DamageType, float> damageTypeValues, bool isGlobal, ConditionSO[] conditions = null)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = isGlobal || GetConditionsAreSatisfied(conditions);
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			if (EnumHelper.GetItemStatType(value) != Enums.ItemStatTypeGroup.Hidden)
			{
				float num = 0f;
				if (statValues.ContainsKey(value))
				{
					num = statValues[value];
				}
				if (num != 0f)
				{
					_ = item.CalculatedStats[value];
					text = text + TooltipSimpleStatHelper.CreateLine(value, num, num, item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
				}
			}
		}
		foreach (KeyValuePair<Enums.DamageType, float> item2 in damageTypeValues.OrderBy((KeyValuePair<Enums.DamageType, float> x) => x.Value))
		{
			if (item2.Value == 0f)
			{
				continue;
			}
			foreach (Enums.DamageType item3 in item2.Key.ListFlags())
			{
				text = text + TooltipDamageTypeHelper.CreateLine(new KeyValuePair<Enums.DamageType, float>(item3, item2.Value), item.ItemRarity, isStarred: false, conditionsAreSatisfied) + Environment.NewLine;
			}
		}
		return text;
	}

	private string GetStats(ItemSO item, SerializableDictionaryBase<Enums.ItemStatType, float> statValues, SerializableDictionaryBase<Enums.DamageType, float> damageTypeValues, bool isGlobal, ConditionSO[] conditions = null)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = isGlobal || GetConditionsAreSatisfied(conditions);
		foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
		{
			if (EnumHelper.GetItemStatType(value) != Enums.ItemStatTypeGroup.Hidden)
			{
				float num = 0f;
				if (statValues.ContainsKey(value))
				{
					num = statValues[value];
				}
				if (num != 0f)
				{
					float num2 = 0f;
					num2 = ((!isGlobal || !item.GlobalStats.StatValues.ContainsKey(value)) ? num : item.GlobalStats.StatValues[value]);
					text = text + TooltipSimpleStatHelper.CreateLine(value, num, num2, item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
				}
			}
		}
		foreach (KeyValuePair<Enums.DamageType, float> item2 in damageTypeValues.OrderBy((KeyValuePair<Enums.DamageType, float> x) => x.Value))
		{
			if (item2.Value == 0f)
			{
				continue;
			}
			foreach (Enums.DamageType item3 in item2.Key.ListFlags())
			{
				text = text + TooltipDamageTypeHelper.CreateLine(new KeyValuePair<Enums.DamageType, float>(item3, item2.Value), item.ItemRarity, isStarred: false, conditionsAreSatisfied) + Environment.NewLine;
			}
		}
		return text;
	}

	private string GetStatsFromFormula(ItemInstance item, SerializableDictionaryBase<Enums.ItemStatType, FormulaSO> formulaStats, SerializableDictionaryBase<Enums.DamageType, FormulaSO> formulaDamageTypeValues, bool isGlobal, ConditionSO[] conditions = null)
	{
		if (item.ItemSO.GlobalStats == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		bool conditionsAreSatisfied = isGlobal || GetConditionsAreSatisfied(conditions);
		if (item.ItemSO.GlobalStats.FormulaStats != null)
		{
			foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
			{
				if (EnumHelper.GetItemStatType(value) != Enums.ItemStatTypeGroup.Hidden)
				{
					bool flag = false;
					if (formulaStats.ContainsKey(value))
					{
						flag = true;
					}
					if (flag)
					{
						float formulaResult = formulaStats[value].GetFormulaResult();
						text = text + TooltipFormulaHelper.CreateLine(value, 0f, formulaResult, formulaStats[value], item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
					}
				}
			}
		}
		if (item.ItemSO.GlobalStats.FormulaDamageTypeValues != null)
		{
			foreach (Enums.DamageType value2 in Enum.GetValues(typeof(Enums.DamageType)))
			{
				bool flag2 = false;
				if (formulaDamageTypeValues.ContainsKey(value2))
				{
					flag2 = true;
				}
				if (flag2)
				{
					float calculatedValue = item.CalculatedDamageTypeValues[value2];
					text = text + TooltipFormulaHelper.CreateLine(value2, 0f, calculatedValue, formulaDamageTypeValues[value2], item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
				}
			}
		}
		return text;
	}

	private string GetStatsFromFormula(ItemSO item, SerializableDictionaryBase<Enums.ItemStatType, FormulaSO> formulaStats, SerializableDictionaryBase<Enums.DamageType, FormulaSO> formulaDamageTypeValues, bool isGlobal, ConditionSO[] conditions = null)
	{
		if (item.GlobalStats == null)
		{
			return string.Empty;
		}
		string text = string.Empty;
		bool conditionsAreSatisfied = isGlobal || GetConditionsAreSatisfied(conditions);
		if (item.GlobalStats.FormulaStats != null)
		{
			foreach (Enums.ItemStatType value in Enum.GetValues(typeof(Enums.ItemStatType)))
			{
				if (EnumHelper.GetItemStatType(value) != Enums.ItemStatTypeGroup.Hidden)
				{
					bool flag = false;
					if (formulaStats.ContainsKey(value))
					{
						flag = true;
					}
					if (flag)
					{
						float formulaResult = formulaStats[value].GetFormulaResult();
						text = text + TooltipFormulaHelper.CreateLine(value, 0f, formulaResult, formulaStats[value], item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
					}
				}
			}
		}
		if (item.GlobalStats.FormulaDamageTypeValues != null)
		{
			foreach (Enums.DamageType value2 in Enum.GetValues(typeof(Enums.DamageType)))
			{
				bool flag2 = false;
				if (formulaDamageTypeValues.ContainsKey(value2))
				{
					flag2 = true;
				}
				if (flag2)
				{
					if (item.GlobalStats != null && item.GlobalStats.DamageTypeValues.ContainsKey(value2))
					{
						float calculatedValue = item.GlobalStats.DamageTypeValues[value2];
						text = text + TooltipFormulaHelper.CreateLine(value2, 0f, calculatedValue, formulaDamageTypeValues[value2], item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
					}
					else
					{
						float calculatedValue2 = 0f;
						text = text + TooltipFormulaHelper.CreateLine(value2, 0f, calculatedValue2, formulaDamageTypeValues[value2], item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
					}
				}
			}
		}
		return text;
	}

	internal string GetEffects(ItemInstance item, WeaponAttackEffect[] weaponAttackEffects, bool isStarred, bool isGlobal)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = (isStarred ? GetConditionsAreSatisfied(item.StarWeaponConditions) : GetConditionsAreSatisfied(item.ConditionalStatConditions));
		if (isGlobal)
		{
			conditionsAreSatisfied = true;
		}
		foreach (WeaponAttackEffect weaponAttackEffect in weaponAttackEffects)
		{
			text = text + TooltipEffectHelper.CreateLine(weaponAttackEffect, item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
		}
		return text;
	}

	internal string GetEffects(ItemSO item, WeaponAttackEffect[] weaponAttackEffects, bool isStarred, bool isGlobal)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = (isStarred ? GetConditionsAreSatisfied(item.StarredWeaponConditions) : GetConditionsAreSatisfied(item.ConditionalStatConditions));
		if (isGlobal)
		{
			conditionsAreSatisfied = true;
		}
		foreach (WeaponAttackEffect weaponAttackEffect in weaponAttackEffects)
		{
			text = text + TooltipEffectHelper.CreateLine(weaponAttackEffect, item.ItemRarity, conditionsAreSatisfied) + Environment.NewLine;
		}
		return text;
	}

	internal string GetDebuffs(ItemInstance item, DebuffSO[] debuffSOs, bool isStarred, bool isGlobal)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = (isStarred ? GetConditionsAreSatisfied(item.StarWeaponConditions) : GetConditionsAreSatisfied(item.ConditionalStatConditions));
		if (isGlobal)
		{
			conditionsAreSatisfied = true;
		}
		foreach (DebuffSO debuff in debuffSOs)
		{
			text = text + TooltipDebuffHelper.CreateLine(debuff, conditionsAreSatisfied, item.ItemRarity) + Environment.NewLine;
		}
		return text;
	}

	internal string GetDebuffs(ItemSO item, DebuffSO[] debuffSOs, bool isStarred, bool isGlobal)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = (isStarred ? GetConditionsAreSatisfied(item.StarredWeaponConditions) : GetConditionsAreSatisfied(item.ConditionalStatConditions));
		if (isGlobal)
		{
			conditionsAreSatisfied = true;
		}
		foreach (DebuffSO debuff in debuffSOs)
		{
			text = text + TooltipDebuffHelper.CreateLine(debuff, conditionsAreSatisfied, item.ItemRarity) + Environment.NewLine;
		}
		return text;
	}

	internal string GetOverrides(ItemInstance item, WeaponDamageTypeValueOverride[] weaponDamageTypeValueOverrides, bool isStarred, bool isGlobal)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = (isStarred ? GetConditionsAreSatisfied(item.StarWeaponConditions) : GetConditionsAreSatisfied(item.ConditionalStatConditions));
		if (isGlobal)
		{
			conditionsAreSatisfied = true;
		}
		foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in weaponDamageTypeValueOverrides)
		{
			text = text + TooltipOverrideHelper.CreateLine(weaponDamageTypeValueOverride.SourceWeaponDamageType, weaponDamageTypeValueOverride.TargetWeaponDamageType, conditionsAreSatisfied, item.ItemRarity) + Environment.NewLine;
		}
		return text;
	}

	internal string GetOverrides(ItemSO item, WeaponDamageTypeValueOverride[] weaponDamageTypeValueOverrides, bool isStarred, bool isGlobal)
	{
		string text = string.Empty;
		bool conditionsAreSatisfied = (isStarred ? GetConditionsAreSatisfied(item.StarredWeaponConditions) : GetConditionsAreSatisfied(item.ConditionalStatConditions));
		if (isGlobal)
		{
			conditionsAreSatisfied = true;
		}
		foreach (WeaponDamageTypeValueOverride weaponDamageTypeValueOverride in weaponDamageTypeValueOverrides)
		{
			text = text + TooltipOverrideHelper.CreateLine(weaponDamageTypeValueOverride.SourceWeaponDamageType, weaponDamageTypeValueOverride.TargetWeaponDamageType, conditionsAreSatisfied, item.ItemRarity) + Environment.NewLine;
		}
		return text;
	}

	private string GetConditions(ConditionSO[] conditionalStatConditions, ItemInstance itemInstance, bool isStarCondition)
	{
		string text = string.Empty;
		foreach (ConditionSO conditionSO in conditionalStatConditions)
		{
			List<WeaponInstance> starredWeapons = new List<WeaponInstance>();
			if (itemInstance != null)
			{
				starredWeapons = SingletonController<BackpackController>.Instance.BackpackStorage.GetWeaponsStarredByItem(itemInstance);
			}
			text = text + TooltipConditionHelper.CreateLine(conditionSO, starredWeapons, isStarCondition) + Environment.NewLine;
		}
		return text;
	}

	private void SetSizeImage(ItemSO itemSO)
	{
		_tooltipSizeContainer.SetSize(itemSO.ItemSize, itemSO.StarringEffectIsPositive);
	}

	private void SetupComplexTooltips(ItemSO item)
	{
		_shouldShowComplex = SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.AlwaysVisible || (SingletonController<SettingsController>.Instance.GameplaySettingsController.TooltipComplexity == Enums.TooltipComplexity.VisibleOnAlt && SingletonController<InputController>.Instance.AltIsDown);
		_hasComplexTooltipToShow = ToggleAdditionalTips(item.ItemRarity) && _allowShowComplexTooltips;
		ToggleAltTooltip(_shouldShowComplex);
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
		bool flag6 = false;
		bool flag7 = false;
		bool flag8 = false;
		if (_activeItemSO != null)
		{
			flag = ToggleAdditionalTips(_activeItemSO);
		}
		if (_activeItemInstance != null)
		{
			flag = ToggleAdditionalTips(_activeItemInstance);
		}
		if (_activeItemSO != null)
		{
			flag2 = AddFormulaTips(_activeItemSO.GlobalStats.FormulaStats, rarity);
		}
		if (_activeItemInstance != null)
		{
			flag2 = AddFormulaTips(_activeItemInstance.ItemSO.GlobalStats.FormulaStats, rarity);
		}
		if (_activeItemSO != null && _activeItemSO.ConditionalStats != null)
		{
			flag3 = AddFormulaTips(_activeItemSO.ConditionalStats.FormulaStats, rarity);
		}
		if (_activeItemInstance != null && _activeItemInstance.ItemSO.ConditionalStats != null)
		{
			flag3 = AddFormulaTips(_activeItemInstance.ItemSO.ConditionalStats.FormulaStats, rarity);
		}
		if (_activeItemSO != null && _activeItemSO.StarredWeaponStats != null)
		{
			flag4 = AddFormulaTips(_activeItemSO.StarredWeaponStats.FormulaStats, rarity);
		}
		if (_activeItemInstance != null && _activeItemInstance.ItemSO.StarredWeaponStats != null)
		{
			flag4 = AddFormulaTips(_activeItemInstance.ItemSO.StarredWeaponStats.FormulaStats, rarity);
		}
		if (_activeItemSO != null)
		{
			flag5 = AddFormulaTips(_activeItemSO.GlobalStats.FormulaDamageTypeValues, rarity);
		}
		if (_activeItemInstance != null)
		{
			flag5 = AddFormulaTips(_activeItemInstance.ItemSO.GlobalStats.FormulaDamageTypeValues, rarity);
		}
		if (_activeItemSO != null && _activeItemSO.ConditionalStats != null)
		{
			flag6 = AddFormulaTips(_activeItemSO.ConditionalStats.FormulaDamageTypeValues, rarity);
		}
		if (_activeItemInstance != null && _activeItemInstance.ItemSO.ConditionalStats != null)
		{
			flag6 = AddFormulaTips(_activeItemInstance.ItemSO.ConditionalStats.FormulaDamageTypeValues, rarity);
		}
		if (_activeItemSO != null && _activeItemSO.StarredWeaponStats != null)
		{
			flag7 = AddFormulaTips(_activeItemSO.StarredWeaponStats.FormulaDamageTypeValues, rarity);
		}
		if (_activeItemInstance != null && _activeItemInstance.ItemSO.StarredWeaponStats != null)
		{
			flag7 = AddFormulaTips(_activeItemInstance.ItemSO.StarredWeaponStats.FormulaDamageTypeValues, rarity);
		}
		return flag || flag2 || flag3 || flag4 || flag5 || flag6 || flag7 || flag8;
	}

	private void ToggleInTooltipTips(bool show)
	{
		foreach (GameObject inTooltipComplexElement in _inTooltipComplexElements)
		{
			inTooltipComplexElement.SetActive(show);
		}
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

	private bool ToggleAdditionalTips(ItemInstance item)
	{
		List<TipFeedbackElement> tipFeedbackElements = GetTipFeedbackElements(item);
		foreach (TipFeedbackElement item2 in tipFeedbackElements)
		{
			UnityEngine.Object.Instantiate(_itemTooltipComplexTipPrefab, _tipContainer).Init(item2, item.BaseItemSO.ItemRarity);
		}
		return tipFeedbackElements.Any();
	}

	private bool ToggleAdditionalTips(ItemSO item)
	{
		List<TipFeedbackElement> tipFeedbackElements = GetTipFeedbackElements(item);
		foreach (TipFeedbackElement item2 in tipFeedbackElements)
		{
			UnityEngine.Object.Instantiate(_itemTooltipComplexTipPrefab, _tipContainer).Init(item2, item.ItemRarity);
		}
		return tipFeedbackElements.Any();
	}

	private List<TipFeedbackElement> GetTipFeedbackElements(ItemInstance item)
	{
		List<TipFeedbackElement> list = new List<TipFeedbackElement>();
		list.AddRange(GetTipFeedbackElementsFromDebuffs(item.GlobalDebuffSOs));
		list.AddRange(GetTipFeedbackElementsFromDebuffs(item.ConditionalDebuffSOs));
		list.AddRange(GetTipFeedbackElementsFromDebuffs(item.StarDebuffSOs));
		return list;
	}

	private List<TipFeedbackElement> GetTipFeedbackElements(ItemSO item)
	{
		List<TipFeedbackElement> list = new List<TipFeedbackElement>();
		list.AddRange(GetTipFeedbackElementsFromDebuffs(item.GlobalDebuffSOs));
		list.AddRange(GetTipFeedbackElementsFromDebuffs(item.ConditionalDebuffSOs));
		list.AddRange(GetTipFeedbackElementsFromDebuffs(item.StarredWeaponDebuffSOs));
		return list;
	}

	private bool HasGlobalStats(ItemSO item)
	{
		if (item.GlobalStats != null)
		{
			return item.GlobalStats.ContainsData();
		}
		return false;
	}

	private bool HasConditionalStats(ItemSO item)
	{
		if (item.ConditionalStats != null)
		{
			return item.ConditionalStats.ContainsData();
		}
		return false;
	}

	private bool HasStarredStats(ItemSO item)
	{
		if (item.StarredWeaponStats != null)
		{
			return item.StarredWeaponStats.ContainsData();
		}
		return false;
	}

	private bool HasStarredConditions(ItemSO item)
	{
		if (HasStarredStats(item) && item.StarredWeaponStats.Conditions != null)
		{
			return item.StarredWeaponStats.Conditions.Any();
		}
		return false;
	}

	private bool HasSpecialTrigger(ItemSO item)
	{
		if (item.ItemSubtype == Enums.PlaceableItemSubtype.Special)
		{
			return item.HasSpecialEffect;
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

	private void SetupDebug(ItemSO item)
	{
		if (GameDatabaseHelper.AllowDebugging)
		{
			SetTitle($"{item.Name} ({item.Id})", item.ItemRarity);
		}
	}
}
