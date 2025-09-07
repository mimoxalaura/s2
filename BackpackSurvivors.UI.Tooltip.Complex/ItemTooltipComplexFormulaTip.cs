using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.ScriptableObjects.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BackpackSurvivors.UI.Tooltip.Complex;

public class ItemTooltipComplexFormulaTip : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI _title;

	[SerializeField]
	private TextMeshProUGUI _description;

	[SerializeField]
	private TextMeshProUGUI _calculationText;

	[SerializeField]
	private Image _border;

	private const float _onePercentFloat = 0.01f;

	private const float _oneFloat = 1f;

	internal void Init(FormulaSO formula, Enums.ItemStatType itemStatType, Enums.PlaceableRarity rarity)
	{
		string spriteValue = StringHelper.GetSpriteValue(itemStatType);
		SetTitleAndDescription(formula, spriteValue);
		string multiplierText = GetMultiplierText(formula.Multiplier, itemStatType);
		float singleStep = GetSingleStep(itemStatType);
		int numberOfCountables = GetNumberOfCountables(formula.Multiplier, singleStep);
		string calculationText = GetCalculationText(formula, StringHelper.GetCleanValue(formula.Maximum, itemStatType), multiplierText, numberOfCountables, StringHelper.GetSpriteValue(itemStatType), StringHelper.GetCleanString(itemStatType));
		_calculationText.SetText(calculationText);
		_border.material = MaterialHelper.GetInternalTooltipBorderRarityMaterial(rarity);
	}

	internal void Init(FormulaSO formula, Enums.WeaponStatType weaponStatType, Enums.PlaceableRarity rarity)
	{
		string spriteValue = StringHelper.GetSpriteValue(weaponStatType);
		SetTitleAndDescription(formula, spriteValue);
		string multiplierText = GetMultiplierText(formula.Multiplier, weaponStatType);
		float singleStep = GetSingleStep(weaponStatType);
		int numberOfCountables = GetNumberOfCountables(formula.Multiplier, singleStep);
		string calculationText = GetCalculationText(formula, StringHelper.GetCleanValue(formula.Maximum, weaponStatType), multiplierText, numberOfCountables, StringHelper.GetSpriteValue(weaponStatType), StringHelper.GetCleanString(weaponStatType));
		_calculationText.SetText(calculationText);
		_border.material = MaterialHelper.GetInternalTooltipBorderRarityMaterial(rarity);
	}

	internal void Init(FormulaSO formula, Enums.DamageType damageType, Enums.PlaceableRarity rarity)
	{
		string spriteValue = StringHelper.GetSpriteValue(damageType);
		SetTitleAndDescription(formula, spriteValue);
		string multiplierText = GetMultiplierText(formula.Multiplier, damageType);
		float singleStep = GetSingleStep(damageType);
		int numberOfCountables = GetNumberOfCountables(formula.Multiplier, singleStep);
		string damageOrStatType = StringHelper.GetCleanString(damageType) + " damage";
		string calculationText = GetCalculationText(formula, StringHelper.GetCleanValue(formula.Maximum, damageType), multiplierText, numberOfCountables, StringHelper.GetSpriteValue(damageType), damageOrStatType);
		_calculationText.SetText(calculationText);
		_border.material = MaterialHelper.GetInternalTooltipBorderRarityMaterial(rarity);
	}

	private int GetNumberOfCountables(float multiplier, float singleStep)
	{
		if (multiplier >= singleStep || multiplier == 0f)
		{
			return 1;
		}
		return Mathf.RoundToInt(singleStep / multiplier);
	}

	private string GetMultiplierText(float multiplier, Enums.DamageType damageType)
	{
		float singleStep = GetSingleStep(damageType);
		if (multiplier >= singleStep)
		{
			return StringHelper.GetCleanValue(multiplier, damageType);
		}
		return StringHelper.GetCleanValue(singleStep, damageType);
	}

	private string GetMultiplierText(float multiplier, Enums.WeaponStatType weaponStatType)
	{
		float singleStep = GetSingleStep(weaponStatType);
		if (multiplier >= singleStep)
		{
			return StringHelper.GetCleanValue(multiplier, weaponStatType);
		}
		return StringHelper.GetCleanValue(singleStep, weaponStatType);
	}

	private string GetMultiplierText(float multiplier, Enums.ItemStatType itemStatType)
	{
		float singleStep = GetSingleStep(itemStatType);
		if (itemStatType == Enums.ItemStatType.LuckPercentage)
		{
			return StringHelper.GetCleanValue(multiplier, itemStatType);
		}
		if (Mathf.Abs(multiplier) >= Mathf.Abs(singleStep))
		{
			return StringHelper.GetCleanValue(multiplier, itemStatType);
		}
		return StringHelper.GetCleanValue(singleStep, itemStatType);
	}

	private float GetSingleStep(Enums.DamageType damageType)
	{
		return 0.01f;
	}

	private float GetSingleStep(Enums.WeaponStatType weaponStatType)
	{
		switch (weaponStatType)
		{
		case Enums.WeaponStatType.CritChancePercentage:
		case Enums.WeaponStatType.CritMultiplier:
		case Enums.WeaponStatType.DamagePercentage:
		case Enums.WeaponStatType.CooldownTime:
		case Enums.WeaponStatType.WeaponRange:
		case Enums.WeaponStatType.ExplosionSizePercentage:
		case Enums.WeaponStatType.LifeDrainPercentage:
		case Enums.WeaponStatType.ProjectileSizePercentage:
		case Enums.WeaponStatType.StunChancePercentage:
		case Enums.WeaponStatType.CooldownReductionPercentage:
		case Enums.WeaponStatType.ProjectileDuration:
			return 0.01f;
		default:
			Debug.LogWarning(string.Format("WeaponStatType {0} is not handled in {1}.{2}", weaponStatType, "ItemTooltipComplexFormulaTip", "GetSingleStep"));
			return 1f;
		}
	}

	private float GetSingleStep(Enums.ItemStatType itemStatType)
	{
		switch (itemStatType)
		{
		case Enums.ItemStatType.CritChancePercentage:
		case Enums.ItemStatType.CritMultiplier:
		case Enums.ItemStatType.DamagePercentage:
		case Enums.ItemStatType.SpeedPercentage:
		case Enums.ItemStatType.LuckPercentage:
		case Enums.ItemStatType.CooldownTime:
		case Enums.ItemStatType.DamageReductionPercentageDONOTUSE:
		case Enums.ItemStatType.PickupRadiusPercentage:
		case Enums.ItemStatType.WeaponRange:
		case Enums.ItemStatType.ExplosionSizePercentage:
		case Enums.ItemStatType.LifeDrainPercentage:
		case Enums.ItemStatType.ProjectileSpeed:
		case Enums.ItemStatType.ProjectileSizePercentage:
		case Enums.ItemStatType.StunChancePercentage:
		case Enums.ItemStatType.ExtraCoinChancePercentage:
		case Enums.ItemStatType.CooldownReductionPercentage:
		case Enums.ItemStatType.DodgePercentage:
		case Enums.ItemStatType.ProjectileDuration:
		case Enums.ItemStatType.DamageAgainstNormalEnemies:
		case Enums.ItemStatType.DamageAgainstEliteAndBossEnemies:
		case Enums.ItemStatType.ExperienceGainedPercentage:
			return 0.01f;
		case Enums.ItemStatType.Health:
		case Enums.ItemStatType.Armor:
		case Enums.ItemStatType.Spiked:
		case Enums.ItemStatType.FlatDamage:
		case Enums.ItemStatType.EnemyCount:
		case Enums.ItemStatType.Penetrating:
		case Enums.ItemStatType.ProjectileCount:
		case Enums.ItemStatType.ExtraDash:
		case Enums.ItemStatType.HealthRegeneration:
		case Enums.ItemStatType.MaximumCompanionCount:
		case Enums.ItemStatType.WeaponCapacity:
			return 1f;
		default:
			Debug.LogWarning(string.Format("ItemStatType {0} is not handled in {1}.{2}", itemStatType, "ItemTooltipComplexFormulaTip", "GetSingleStep"));
			return 1f;
		}
	}

	private string GetCalculationText(FormulaSO formula, string cleanMaximum, string cleanMultiplier, int numberOfCountables, string spriteName, string damageOrStatType)
	{
		float formulaResult = formula.GetFormulaResult();
		string text = (formula.EffectIsPositive ? ("<color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase) + ">added</color>") : ("<color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase) + ">removed</color>"));
		if (formulaResult != 0f)
		{
			text = ((formulaResult >= 0f) ? ("<color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.HigherThenBase) + ">added</color>") : ("<color=" + ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.LowerThenBase) + ">removed</color>"));
		}
		string colorStringForTooltip = ColorHelper.GetColorStringForTooltip((formulaResult != 0f) ? ((formulaResult > 0f) ? Enums.TooltipValueDifference.HigherThenBase : Enums.TooltipValueDifference.LowerThenBase) : Enums.TooltipValueDifference.SameAsBase);
		string formulaTypeToCheckAgainstString = GetFormulaTypeToCheckAgainstString(formula, numberOfCountables);
		string empty = string.Empty;
		empty = ((!StringHelper.IsShownAsPercentage(formula.ItemStatType) || formula.TypeToCheckAgainst != Enums.ConditionalStats.TypeToCheckAgainst.StatType) ? ((numberOfCountables > 1) ? $"{numberOfCountables} " : string.Empty) : $"<color={ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase)}>{numberOfCountables}%</color> ");
		string text2 = string.Empty;
		string colorStringForTooltip2 = ColorHelper.GetColorStringForTooltip(Enums.TooltipValueDifference.SameAsBase);
		if (formula.Maximum > 0f)
		{
			text2 = " up to a maximum of <color=" + colorStringForTooltip2 + ">" + cleanMaximum + "</color> <color=#FFFFFF><sprite name=\"" + spriteName + "\"></color> <color=#FFFFFF>" + damageOrStatType + "</color>";
		}
		return "<color=" + colorStringForTooltip + ">" + cleanMultiplier + " </color> <color=#FFFFFF><sprite name=\"" + spriteName + "\"></color> <color=#FFFFFF>" + damageOrStatType + "</color> is " + text + " per " + empty + "<color=#FFFFFF><sprite name=\"" + StringHelper.GetFormulaTypeToCheckAgainstIcon(formula) + "\"></color> <color=#FFFFFF>" + formulaTypeToCheckAgainstString + "</color>" + text2;
	}

	private void SetTitleAndDescription(FormulaSO formula, string targetIconString)
	{
		string text = "<color=#FFFFFF><sprite name=\"" + StringHelper.GetFormulaTypeToCheckAgainstIcon(formula) + "\"></color>";
		string text2 = "<color=#FFFFFF><sprite name=\"" + targetIconString + "\"></color>";
		string empty = string.Empty;
		empty = ((!formula.EffectIsPositive) ? "FormulaTransformNegative" : "FormulaTransform");
		string sourceText = text + " <sprite name=\"" + empty + "\"> " + text2;
		_title.SetText(sourceText);
		_description.SetText(formula.FullDescription);
	}

	private float GetFormulaSourceValue(FormulaSO formula)
	{
		switch (formula.TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
			return SingletonController<BackpackController>.Instance.CountController.GetPlaceableTagCount((Enums.PlaceableTag)formula.PlaceableTagLongValue);
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
			return SingletonController<BackpackController>.Instance.CountController.GetPlaceableTypeCount(formula.PlaceableType);
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return SingletonController<GameController>.Instance.Player.GetCalculatedStat(formula.ItemStatType);
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.Coins);
		default:
			Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2}", formula.TypeToCheckAgainst, "ItemTooltipComplexFormulaTip", "GetFormulaSourceValue"));
			return 0f;
		}
	}

	private string GetCleanValue(float value, FormulaSO formula)
	{
		switch (formula.TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
			return value.ToString();
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
			return value.ToString();
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return StringHelper.GetCleanValue(value, formula.ItemStatType);
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return value.ToString();
		default:
			Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2}", formula.TypeToCheckAgainst, "ItemTooltipComplexFormulaTip", "GetCleanValue"));
			return value.ToString();
		}
	}

	private string GetFormulaTypeToCheckAgainstString(FormulaSO formula, int numberOfCountables)
	{
		bool flag = numberOfCountables == 1;
		string text = (flag ? "tag" : "tags");
		string result = (flag ? "coin" : "coins");
		switch (formula.TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
			return StringHelper.GetCleanString((Enums.PlaceableTag)formula.PlaceableTagLongValue) + " " + text;
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
			return StringHelper.GetCleanString(formula.PlaceableType, flag);
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return StringHelper.GetCleanString(formula.ItemStatType);
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return result;
		default:
			Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2}", formula.TypeToCheckAgainst, "ItemTooltipComplexFormulaTip", "GetFormulaTypeToCheckAgainstString"));
			return string.Empty;
		}
	}
}
