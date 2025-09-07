using System.Collections.Generic;
using System.Linq;
using BackpackSurvivors.Game.Backpack;
using BackpackSurvivors.Game.Items;
using BackpackSurvivors.System;
using BackpackSurvivors.System.Helper;

namespace BackpackSurvivors.UI.Stats;

public class ComposedStatItemUI : StatItemUI
{
	public override void Init(Enums.ItemStatType itemStatType, List<ItemStatModifier> itemStatModifiers)
	{
		base.Init(itemStatType, itemStatModifiers);
		string cleanValue = StringHelper.GetCleanValue(itemStatModifiers.Sum((ItemStatModifier x) => x.CalculatedBonus), itemStatType);
		string sourceText = SingletonController<BackpackController>.Instance.CountController.GetPlaceableTypeCount(Enums.PlaceableType.Weapon) + "/" + cleanValue;
		_statValueText.SetText(sourceText);
	}

	internal override void SetValueText(string value, string colorString, bool saveValueColor = true)
	{
		string cleanValue = StringHelper.GetCleanValue(_itemStatModifiers.Sum((ItemStatModifier x) => x.CalculatedBonus), _itemStatType);
		string value2 = SingletonController<BackpackController>.Instance.CountController.GetPlaceableTypeCount(Enums.PlaceableType.Weapon) + "/" + cleanValue;
		base.SetValueText(value2, colorString, saveValueColor);
	}

	public override void UpdateStat(string newValue, float statChange, List<ItemStatModifier> itemStatModifiers)
	{
		base.UpdateStat(newValue, statChange, itemStatModifiers);
	}
}
