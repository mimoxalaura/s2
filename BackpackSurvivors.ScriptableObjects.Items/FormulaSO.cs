using System;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Game;
using BackpackSurvivors.System;
using UnityEngine;

namespace BackpackSurvivors.ScriptableObjects.Items;

[CreateAssetMenu(fileName = "Formula", menuName = "Game/Stats/Formula", order = 3)]
public class FormulaSO : ScriptableObject
{
	[SerializeField]
	internal string Description;

	[SerializeField]
	internal string FullDescription;

	[SerializeField]
	internal Enums.ConditionalStats.FormulaType FormulaType;

	[SerializeField]
	internal float Multiplier;

	[SerializeField]
	internal Enums.ConditionalStats.TypeToCheckAgainst TypeToCheckAgainst;

	[SerializeField]
	internal Enums.PlaceableType PlaceableType;

	[SerializeField]
	internal long PlaceableTagLongValue;

	[SerializeField]
	internal Enums.ItemStatType ItemStatType;

	[SerializeField]
	internal float Minimum = float.MinValue;

	[SerializeField]
	internal float Maximum = float.MaxValue;

	[SerializeField]
	internal bool EffectIsPositive = true;

	internal float GetFormulaResult()
	{
		float num = 0f;
		switch (FormulaType)
		{
		case Enums.ConditionalStats.FormulaType.Fixed:
			num = Multiplier;
			break;
		case Enums.ConditionalStats.FormulaType.Calculated:
			num = GetCalculationResult();
			break;
		case Enums.ConditionalStats.FormulaType.ComplexCalculation:
			num = GetComplexCalculationResult();
			break;
		default:
			Debug.LogWarning(string.Format("FormulaType {0} is not handled in {1}.{2} ", FormulaType, "FormulaSO", "GetFormulaResult"));
			return 0f;
		}
		return Mathf.Clamp(num, Minimum, Maximum);
	}

	private float GetComplexCalculationResult()
	{
		throw new NotImplementedException();
	}

	private float GetCalculationResult()
	{
		switch (TypeToCheckAgainst)
		{
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTagCount:
			return GetPlaceableTagCount();
		case Enums.ConditionalStats.TypeToCheckAgainst.PlaceableTypeCount:
			return (float)SingletonController<BackpackController>.Instance.CountController.GetPlaceableTypeCount(PlaceableType) * Multiplier;
		case Enums.ConditionalStats.TypeToCheckAgainst.StatType:
			return SingletonController<GameController>.Instance.Player.GetCalculatedStat(ItemStatType) * Multiplier;
		case Enums.ConditionalStats.TypeToCheckAgainst.CoinAmount:
			return (float)SingletonController<CurrencyController>.Instance.GetCurrency(Enums.CurrencyType.Coins) * Multiplier;
		default:
			Debug.LogWarning(string.Format("TypeToCheckAgainst {0} is not handled in {1}.{2} ", TypeToCheckAgainst, "FormulaSO", "GetCalculationResult"));
			return 0f;
		}
	}

	private float GetPlaceableTagCount()
	{
		Enums.PlaceableTag placeableTagLongValue = (Enums.PlaceableTag)PlaceableTagLongValue;
		return (float)SingletonController<BackpackController>.Instance.CountController.GetPlaceableTagCount(placeableTagLongValue) * Multiplier;
	}
}
